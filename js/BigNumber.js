(function(){
/*
Written in 2013 by Peter O.

Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
if(typeof StringBuilder=="undefined"){
var StringBuilder=function(){
this.str="";
}
}
StringBuilder.prototype.append=function(ch){
if(typeof ch=="number")
 this.str+=String.fromCharCode(ch);
else
 this.str+=ch
}
StringBuilder.prototype.length=function(){
return this.str.length
}
StringBuilder.prototype.charAt=function(index){
// Get the character code, since that's what the caller expects
return this.str.charCodeAt(index)
}
StringBuilder.prototype.toString=function(){
return this.str;
}
if(typeof JSInteropFactory=="undefined"){
var JSInteropFactory={};
}
/////////////////////////////////////
// Adapted by Peter O. from CryptoPP by Wei Dai
JSInteropFactory.divideFourWordsByTwo=function(dividend, divisor){
 var t0=(dividend.lo&0xFFFF);
 var t1=(dividend.lo>>>16);
 var t2=(dividend.hi&0xFFFF);
 var t3=(dividend.hi>>>16);
 var b0=(divisor&0xFFFF);
 var b1=(divisor>>>16);
 var ret=JSInteropFactory.divideThreeWordsByTwo(t1,t2,t3,b0,b1);
 var qhigh=ret[0];
 ret=JSInteropFactory.divideThreeWordsByTwo(t0,ret[1],ret[2],b0,b1);
 var qlow=ret[0];
 return [(qlow|(qhigh<<16))>>>0,(ret[1]|(ret[2]<<16))>>>0];
}

// Adapted by Peter O. from CryptoPP by Wei Dai
JSInteropFactory.divideThreeWordsByTwo=function(a0,a1,a2,b0,b1){
  var q=0;
  if(b1==0xFFFF)
   q=a2;
  else if(b1>0)
   q=((((a1|(a2<<16))>>>0)/((b1+1)&0xFFFF))&0xFFFF)|0;
  else
   q=((((a1|(a2<<16))>>>0)/b0)&0xFFFF)|0;
  var p=(b0*q)>>>0;
  var u=(a0-(p&0xFFFF))>>>0;
  a0=(u&0xFFFF);
  u=(a1-(p>>>16)-(((0-(u>>>16)))&0xFFFF)-((b1*q)>>>0))>>>0;
  a1=(u&0xFFFF);
  a2=((a2+(u>>>16))&0xFFFF);
  while(a2!=0 || a1>b1 || (a1==b1 && a0>=b0)){
   u=(a0-b0)>>>0;
   a0=(u&0xFFFF);
   u=(a1-b1-(((0-(u>>>16)))&0xFFFF))>>>0;
   a1=(u&0xFFFF);
   a2=((a2+(u>>>16))&0xFFFF);
   q++;
   q&=0xFFFF;
  }
  return [q,a0,a1,a2]
}

JSInteropFactory.divide64By32=function(dividendLow,dividendHigh,divisor){
  var remainder=0
  var currentDividend=new ILong(dividendHigh,0);
  var result=JSInteropFactory.divideFourWordsByTwo(currentDividend,divisor);
  var quotientHigh=result[0];
  remainder=result[1];
  currentDividend=new ILong(dividendLow,remainder);
  result=JSInteropFactory.divideFourWordsByTwo(currentDividend,divisor);
  var quotientLow=result[0];
  return new ILong(quotientLow,quotientHigh);
}

/////////////////////////////////////////////
var ILong=function(lo,hi){
// Convert lo and hi to unsigned
this.lo=lo>>>0
this.hi=hi>>>0
}
ILong.prototype.signum=function(){
if((this.lo|this.hi)==0)return 0;
return ((this.hi>>>31)!=0) ? -1 : 1;
}
ILong.prototype.equals=function(other){
 return this.lo==other.lo && this.hi==other.hi
}
ILong.prototype.negate=function(){
var ret=new ILong(this.lo,this.hi);
if((this.lo|this.hi)!=0)ret._twosComplement();
return ret;
}
ILong.prototype.or=function(other){
return new ILong(this.lo|other.lo,this.hi|other.hi);
}
ILong.prototype.andInt=function(otherUnsigned){
return new ILong(this.lo&(otherUnsigned>>>0),this.hi);
}
ILong.prototype.intValue=function(){
return this.lo|0;
}
ILong.prototype.shortValue=function(){
return (this.lo|0)&0xFFFF;
}
ILong.prototype.compareToLongAsInts=function(otherLo,otherHi){
 otherHi|=0;
 // Signed comparison of high words
 if(otherHi!=(this.hi|0)){
  return (otherHi>(this.hi|0)) ? -1 : 1;
 }
 otherLo=otherLo>>>0;
 // Unsigned comparison of low words
 if(otherLo!=this.lo){
  return (otherLo>this.lo) ? -1 : 1;
 }
 return 0;
}
ILong.prototype.compareToInt=function(other){
 other|=0;
 var otherHi=(other<0) ? -1 : 0;
 // Signed comparison of high words
 if(otherHi!=(this.hi|0)){
  return (otherHi>(this.hi|0)) ? -1 : 1;
 }
 other=other>>>0;
 // Unsigned comparison of low words
 if(other!=this.lo){
  return (other>this.lo) ? -1 : 1;
 }
 return 0;
}
ILong.prototype.equalsInt=function(other){
 if(other<0){
  return (~this.hi)==0 && this.lo==(other>>>0);
 } else {
  return this.hi==0 && this.lo==(other>>>0);
 }
}
ILong.prototype._twosComplement=function(){
 if(this.lo==0){
  this.hi=((this.hi-1)>>>0);
 }
 this.lo=((this.lo-1)>>>0);
 this.lo=(~this.lo)>>>0;
 this.hi=(~this.hi)>>>0;
}
ILong.prototype.remainderWithUnsignedDivisor=function(divisor){
 if((this.hi>>>31)!=0){
  // value is negative
  var ret=new ILong(this.lo,this.hi);
  ret._twosComplement();
  // NOTE: since divisor is unsigned, overflow is impossible
  ret=ret._remainderUnsignedDividendUnsigned(divisor);
  ret._twosComplement();
  return ret;
 } else {
  return this._remainderUnsignedDividendUnsigned(divisor);
 }
}
ILong.prototype.divideWithUnsignedDivisor=function(divisor){
 if((this.hi>>>31)!=0){
  // value is negative
  var ret=new ILong(this.lo,this.hi);
  ret._twosComplement();
  // NOTE: since divisor is unsigned, overflow is impossible
  ret=ret._divideUnsignedDividendUnsigned(divisor);
  ret._twosComplement();
  return ret;
 } else {
  return this._divideUnsignedDividendUnsigned(divisor);
 }
}

ILong.prototype._divideUnsignedDividendUnsigned=function(divisor){
 divisor|=0;
 if(divisor<0)throw new RuntimeException("value is less than 0");
 if(divisor==1)return this;
		if (this.hi==0){
    return new ILong((this.lo>>>0)/divisor,0);
		} else {
    var rem=JSInteropFactory.divide64By32(this.lo,this.hi,divisor);
    return rem;
		}
}

ILong.prototype._remainderUnsignedDividendUnsigned=function(divisor){
 divisor|=0;
 if(divisor<0)throw new RuntimeException("value is less than 0");
 if(divisor==1)return this;
		if (divisor < 0x10000 || this.hi==0)
		{
    var r=this.hi%divisor;
    r=((this.lo>>>16)|(r<<16))%divisor;
    return new ILong(
      (((this.lo&0xFFFF)|(r<<16))%divisor)&0xFFFF,
      0
    );
		} else {
    var rem=JSInteropFactory.divideFourWordsByTwo(this,divisor);
    return new ILong(rem[1],(rem[1]>>>31)==0 ? 0 : (1<<31));
		}
}

ILong.prototype.shiftLeft=function(len){
 if(len<=0)return this;
 if(len>=64){
  return JSInteropFactory.LONG_ZERO;
 } else if(len>=32){
  return new ILong(0,this.lo<<(len-32));
 } else if(this.lo==0){
  return new ILong(0,this.hi<<len);
 } else {
  var newhigh=this.hi<<len;
  var newlow=this.lo<<len;
  newhigh|=(this.lo>>>(32-len));
  return new ILong(newlow,newhigh);
 }
}
ILong.prototype.shiftRight=function(len){
 if(len<=0)return this;
 if(len>=64){
  return ((this.hi>>>31)!=0) ?
    JSInteropFactory.LONG_MAX_VALUE() :
    JSInteropFactory.LONG_MIN_VALUE();
 } else if(len>=32){
  return new ILong((this.hi>>len-32),((this.hi>>>31)!=0) ? (~0) : 0);
 } else if(this.hi==0){
  return new ILong(this.lo>>>len,0);
 } else {
  var newhigh=this.hi>>len;
  var newlow=this.lo>>>len;
  newlow|=(this.hi<<(32-len));
  return new ILong(newlow,newhigh);
 }
}
JSInteropFactory.createStringBuilder=function(param){
 return new StringBuilder();
}
JSInteropFactory.createLong=function(param){
 if(param.constructor==ILong)return param;
 return new ILong(param,(param<0) ? (~0) : 0);
}
JSInteropFactory.createLongFromInts=function(a,b){
 return new ILong(a>>>0,b>>>0);
}
JSInteropFactory.LONG_MIN_VALUE_=new ILong(0,(1<<31));
JSInteropFactory.LONG_MAX_VALUE_=new ILong(~0,~0);
JSInteropFactory.LONG_MIN_VALUE=function(){
 return JSInteropFactory.LONG_MIN_VALUE_;
}
JSInteropFactory.LONG_MAX_VALUE=function(){
 return JSInteropFactory.LONG_MAX_VALUE_;
}
JSInteropFactory.LONG_ZERO=new ILong(0,0)
var Extras={}
Extras.IntegersToDouble=function(){throw "Not implemented"}
Extras.DoubleToIntegers=function(){throw "Not implemented"}
if(typeof exports!=="undefined"){
exports.Extras=Extras;
exports.JSInteropFactory=JSInteropFactory;
exports.ILong=ILong;
exports.StringBuilder=StringBuilder;
}

var BigInteger =

function() {

};
(function(constructor,prototype){
    constructor['CountWords'] = constructor.CountWords = function(array, n) {
        while (n != 0 && array[n - 1] == 0) {
            --n;
        }
        return (n|0);
    };
    constructor['ShiftWordsLeftByBits'] = constructor.ShiftWordsLeftByBits = function(r, rstart, n, shiftBits) {
        {
            var u, carry = 0;
            if (shiftBits != 0) {
                for (var i = 0; i < n; ++i) {
                    u = r[rstart + i];
                    r[rstart + i] = ((((((((((u << (shiftBits|0))|0) | (carry & 65535)))|0)) & 65535))|0));
                    carry = (((u & 65535) >> ((16 - shiftBits)|0))|0);
                }
            }
            return carry;
        }
    };
    constructor['ShiftWordsRightByBits'] = constructor.ShiftWordsRightByBits = function(r, rstart, n, shiftBits) {
        var u, carry = 0;
        {
            if (shiftBits != 0) {
                for (var i = n; i > 0; --i) {
                    u = r[rstart + i - 1];
                    r[rstart + i - 1] = (((((((((((u & 65535) >> (shiftBits|0)) & 65535) | (carry & 65535)))|0)) & 65535))|0));
                    carry = (((u & 65535) << ((16 - shiftBits)|0))|0);
                }
            }
            return carry;
        }
    };
    constructor['ShiftWordsRightByBitsSignExtend'] = constructor.ShiftWordsRightByBitsSignExtend = function(r, rstart, n, shiftBits) {
        {
            var u, carry = ((65535 << ((16 - shiftBits)|0))|0);
            if (shiftBits != 0) {
                for (var i = n; i > 0; --i) {
                    u = r[rstart + i - 1];
                    r[rstart + i - 1] = ((((((((((u & 65535) >> (shiftBits|0)) | (carry & 65535)))|0)) & 65535))|0));
                    carry = (((u & 65535) << ((16 - shiftBits)|0))|0);
                }
            }
            return carry;
        }
    };
    constructor['ShiftWordsLeftByWords'] = constructor.ShiftWordsLeftByWords = function(r, rstart, n, shiftWords) {
        shiftWords = (shiftWords < n ? shiftWords : n);
        if (shiftWords != 0) {
            for (var i = n - 1; i >= shiftWords; --i) {
                r[rstart + i] = (r[rstart + i - shiftWords] & 65535);
            }
            for (var arrfillI = rstart; arrfillI < (rstart) + (shiftWords); arrfillI++) r[arrfillI] = 0;
        }
    };
    constructor['ShiftWordsRightByWordsSignExtend'] = constructor.ShiftWordsRightByWordsSignExtend = function(r, rstart, n, shiftWords) {
        shiftWords = (shiftWords < n ? shiftWords : n);
        if (shiftWords != 0) {
            for (var i = 0; i + shiftWords < n; ++i) {
                r[rstart + i] = (r[rstart + i + shiftWords] & 65535);
            }
            rstart = rstart + (n - shiftWords);
            for (var i = 0; i < shiftWords; ++i) {
                r[rstart + i] = (65535 & 65535);
            }
        }
    };
    constructor['Compare'] = constructor.Compare = function(words1, astart, words2, bstart, n) {
        while ((n--) != 0) {
            var an = ((words1[astart + n])|0) & 65535;
            var bn = ((words2[bstart + n])|0) & 65535;
            if (an > bn) {
                return 1;
            } else if (an < bn) {
                return -1;
            }
        }
        return 0;
    };
    constructor['CompareUnevenSize'] = constructor.CompareUnevenSize = function(words1, astart, acount, words2, bstart, bcount) {
        var n = acount;
        if (acount > bcount) {
            while ((acount--) != bcount) {
                if (words1[astart + acount] != 0) {
                    return 1;
                }
            }
            n = bcount;
        } else if (bcount > acount) {
            while ((bcount--) != acount) {
                if (words1[astart + acount] != 0) {
                    return -1;
                }
            }
            n = acount;
        }
        while ((n--) != 0) {
            var an = ((words1[astart + n])|0) & 65535;
            var bn = ((words2[bstart + n])|0) & 65535;
            if (an > bn) {
                return 1;
            } else if (an < bn) {
                return -1;
            }
        }
        return 0;
    };
    constructor['CompareWithOneBiggerWords1'] = constructor.CompareWithOneBiggerWords1 = function(words1, astart, words2, bstart, words1Count) {
        if (words1[astart + words1Count - 1] != 0) {
            return 1;
        }
        --words1Count;
        while ((words1Count--) != 0) {
            var an = ((words1[astart + words1Count])|0) & 65535;
            var bn = ((words2[bstart + words1Count])|0) & 65535;
            if (an > bn) {
                return 1;
            } else if (an < bn) {
                return -1;
            }
        }
        return 0;
    };
    constructor['Increment'] = constructor.Increment = function(words1, words1Start, n, words2) {
        {
            var tmp = words1[words1Start];
            words1[words1Start] = (((tmp + words2) & 65535));
            if ((words1[words1Start] & 65535) >= (tmp & 65535)) {
                return 0;
            }
            for (var i = 1; i < n; ++i) {
                words1[words1Start + i] = ((words1[words1Start + i] + 1) & 65535);
                if (words1[words1Start + i] != 0) {
                    return 0;
                }
            }
            return 1;
        }
    };
    constructor['Decrement'] = constructor.Decrement = function(words1, words1Start, n, words2) {
        {
            var tmp = words1[words1Start];
            words1[words1Start] = (((tmp - words2) & 65535));
            if ((words1[words1Start] & 65535) <= (tmp & 65535)) {
                return 0;
            }
            for (var i = 1; i < n; ++i) {
                tmp = words1[words1Start + i];
                words1[words1Start + i] = ((words1[words1Start + i] - 1) & 65535);
                if (tmp != 0) {
                    return 0;
                }
            }
            return 1;
        }
    };
    constructor['TwosComplement'] = constructor.TwosComplement = function(words1, words1Start, n) {
        BigInteger.Decrement(words1, words1Start, n, 1);
        for (var i = 0; i < n; ++i) {
            words1[words1Start + i] = (((~words1[words1Start + i]) & 65535));
        }
    };
    constructor['Add'] = constructor.Add = function(c, cstart, words1, astart, words2, bstart, n) {
        {
            var u;
            u = 0;
            for (var i = 0; i < n; i += 2) {
                u = (words1[astart + i] & 65535) + (words2[bstart + i] & 65535) + ((u >> 16)|0);
                c[cstart + i] = (u & 65535);
                u = (words1[astart + i + 1] & 65535) + (words2[bstart + i + 1] & 65535) + ((u >> 16)|0);
                c[cstart + i + 1] = (u & 65535);
            }
            return ((u|0) >>> 16);
        }
    };
    constructor['AddOneByOne'] = constructor.AddOneByOne = function(c, cstart, words1, astart, words2, bstart, n) {
        {
            var u;
            u = 0;
            for (var i = 0; i < n; i += 1) {
                u = (words1[astart + i] & 65535) + (words2[bstart + i] & 65535) + ((u >> 16)|0);
                c[cstart + i] = (u & 65535);
            }
            return ((u|0) >>> 16);
        }
    };
    constructor['SubtractOneBiggerWords1'] = constructor.SubtractOneBiggerWords1 = function(c, cstart, words1, astart, words2, bstart, words1Count) {
        {
            var u;
            u = 0;
            var cm1 = words1Count - 1;
            for (var i = 0; i < cm1; i += 1) {
                u = (words1[astart] & 65535) - (words2[bstart] & 65535) - ((u >> 31) & 1);
                c[cstart++] = (u & 65535);
                ++astart;
                ++bstart;
            }
            u = (words1[astart] & 65535) - ((u >> 31) & 1);
            c[cstart++] = (u & 65535);
            return ((u >> 31) & 1);
        }
    };
    constructor['SubtractOneBiggerWords2'] = constructor.SubtractOneBiggerWords2 = function(c, cstart, words1, astart, words2, bstart, words2Count) {
        {
            var u;
            u = 0;
            var cm1 = words2Count - 1;
            for (var i = 0; i < cm1; i += 1) {
                u = (words1[astart] & 65535) - (words2[bstart] & 65535) - ((u >> 31) & 1);
                c[cstart++] = (u & 65535);
                ++astart;
                ++bstart;
            }
            u = 0 - (words2[bstart] & 65535) - ((u >> 31) & 1);
            c[cstart++] = (u & 65535);
            return ((u >> 31) & 1);
        }
    };
    constructor['AddUnevenSize'] = constructor.AddUnevenSize = function(c, cstart, wordsBigger, astart, acount, wordsSmaller, bstart, bcount) {
        {
            var u;
            u = 0;
            for (var i = 0; i < bcount; i += 1) {
                u = (wordsBigger[astart + i] & 65535) + (wordsSmaller[bstart + i] & 65535) + ((u >> 16)|0);
                c[cstart + i] = (u & 65535);
            }
            for (var i = bcount; i < acount; i += 1) {
                u = (wordsBigger[astart + i] & 65535) + ((u >> 16)|0);
                c[cstart + i] = (u & 65535);
            }
            return ((u|0) >>> 16);
        }
    };
    constructor['Subtract'] = constructor.Subtract = function(c, cstart, words1, astart, words2, bstart, n) {
        {
            var u;
            u = 0;
            for (var i = 0; i < n; i += 2) {
                u = (words1[astart] & 65535) - (words2[bstart] & 65535) - ((u >> 31) & 1);
                c[cstart++] = (u & 65535);
                ++astart;
                ++bstart;
                u = (words1[astart] & 65535) - (words2[bstart] & 65535) - ((u >> 31) & 1);
                c[cstart++] = (u & 65535);
                ++astart;
                ++bstart;
            }
            return ((u >> 31) & 1);
        }
    };
    constructor['SubtractOneByOne'] = constructor.SubtractOneByOne = function(c, cstart, words1, astart, words2, bstart, n) {
        {
            var u;
            u = 0;
            for (var i = 0; i < n; i += 1) {
                u = (words1[astart] & 65535) - (words2[bstart] & 65535) - ((u >> 31) & 1);
                c[cstart++] = (u & 65535);
                ++astart;
                ++bstart;
            }
            return ((u >> 31) & 1);
        }
    };
    constructor['LinearMultiplyAdd'] = constructor.LinearMultiplyAdd = function(productArr, cstart, words1, astart, words2, n) {
        {
            var carry = 0;
            var bint = (words2|0) & 65535;
            for (var i = 0; i < n; ++i) {
                var p;
                p = (words1[astart + i] & 65535) * bint;
                p = p + (carry & 65535);
                p = p + (productArr[cstart + i] & 65535);
                productArr[cstart + i] = (p & 65535);
                carry = ((p >> 16)|0);
            }
            return carry;
        }
    };
    constructor['LinearMultiply'] = constructor.LinearMultiply = function(productArr, cstart, words1, astart, words2, n) {
        {
            var carry = 0;
            var bint = (words2|0) & 65535;
            for (var i = 0; i < n; ++i) {
                var p;
                p = (words1[astart + i] & 65535) * bint;
                p = p + (carry & 65535);
                productArr[cstart + i] = (p & 65535);
                carry = ((p >> 16)|0);
            }
            return carry;
        }
    };
    constructor['Baseline_Square2'] = constructor.Baseline_Square2 = function(result, rstart, words1, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (words1[astart] & 65535) * (words1[astart] & 65535);
            result[rstart] = (p & 65535);
            e = ((p|0) >>> 16);
            p = (words1[astart] & 65535) * (words1[astart + 1] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 1] = (c & 65535);
            p = (words1[astart + 1] & 65535) * (words1[astart + 1] & 65535);
            p = p + (e);
            result[rstart + 2] = (p & 65535);
            result[rstart + 3] = (((p >> 16) & 65535));
        }
    };
    constructor['Baseline_Square4'] = constructor.Baseline_Square4 = function(result, rstart, words1, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (words1[astart] & 65535) * (words1[astart] & 65535);
            result[rstart] = (p & 65535);
            e = ((p|0) >>> 16);
            p = (words1[astart] & 65535) * (words1[astart + 1] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 1] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 2] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 1] & 65535) * (words1[astart + 1] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 2] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 3] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 2] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 3] = (c & 65535);
            p = (words1[astart + 1] & 65535) * (words1[astart + 3] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 2] & 65535) * (words1[astart + 2] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 4] = (c & 65535);
            p = (words1[astart + 2] & 65535) * (words1[astart + 3] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + (2 * 4) - 3] = (c & 65535);
            p = (words1[astart + 3] & 65535) * (words1[astart + 3] & 65535);
            p = p + (e);
            result[rstart + 6] = (p & 65535);
            result[rstart + 7] = (((p >> 16) & 65535));
        }
    };
    constructor['Baseline_Square8'] = constructor.Baseline_Square8 = function(result, rstart, words1, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (words1[astart] & 65535) * (words1[astart] & 65535);
            result[rstart] = (p & 65535);
            e = ((p|0) >>> 16);
            p = (words1[astart] & 65535) * (words1[astart + 1] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 1] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 2] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 1] & 65535) * (words1[astart + 1] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 2] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 3] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 2] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 3] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 4] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 3] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 2] & 65535) * (words1[astart + 2] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 4] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 5] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 4] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 2] & 65535) * (words1[astart + 3] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 5] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 6] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 5] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 2] & 65535) * (words1[astart + 4] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 3] & 65535) * (words1[astart + 3] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 6] = (c & 65535);
            p = (words1[astart] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 1] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 2] & 65535) * (words1[astart + 5] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 3] & 65535) * (words1[astart + 4] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 7] = (c & 65535);
            p = (words1[astart + 1] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 2] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 3] & 65535) * (words1[astart + 5] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 4] & 65535) * (words1[astart + 4] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 8] = (c & 65535);
            p = (words1[astart + 2] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 3] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (words1[astart + 4] & 65535) * (words1[astart + 5] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 9] = (c & 65535);
            p = (words1[astart + 3] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 4] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 5] & 65535) * (words1[astart + 5] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 10] = (c & 65535);
            p = (words1[astart + 4] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            p = (words1[astart + 5] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 11] = (c & 65535);
            p = (words1[astart + 5] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (words1[astart + 6] & 65535) * (words1[astart + 6] & 65535);
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 12] = (c & 65535);
            p = (words1[astart + 6] & 65535) * (words1[astart + 7] & 65535);
            c = (p|0);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e|0);
            e = d + ((e|0) >>> 16);
            result[rstart + 13] = (c & 65535);
            p = (words1[astart + 7] & 65535) * (words1[astart + 7] & 65535);
            p = p + (e);
            result[rstart + 14] = (p & 65535);
            result[rstart + 15] = (((p >> 16) & 65535));
        }
    };
    constructor['Baseline_Multiply2'] = constructor.Baseline_Multiply2 = function(result, rstart, words1, astart, words2, bstart) {
        {
            var p;
            var c;
            var d;
            var a0 = ((words1[astart])|0) & 65535;
            var a1 = ((words1[astart + 1])|0) & 65535;
            var b0 = ((words2[bstart])|0) & 65535;
            var b1 = ((words2[bstart + 1])|0) & 65535;
            p = a0 * b0;
            c = (p|0);
            d = ((p|0) >>> 16);
            result[rstart] = (c & 65535);
            c = (d|0);
            d = ((d|0) >>> 16);
            p = a0 * b1;
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = a1 * b0;
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            result[rstart + 1] = (c & 65535);
            p = a1 * b1;
            p = p + (d);
            result[rstart + 2] = (p & 65535);
            result[rstart + 3] = (((p >> 16) & 65535));
        }
    };
    constructor['Baseline_Multiply4'] = constructor.Baseline_Multiply4 = function(result, rstart, words1, astart, words2, bstart) {
        var mask = 65535;
        {
            var p;
            var c;
            var d;
            var a0 = ((words1[astart])|0) & mask;
            var b0 = ((words2[bstart])|0) & mask;
            p = a0 * b0;
            c = (p|0);
            d = ((p|0) >> 16) & mask;
            result[rstart] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = a0 * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * b0;
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 1] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = a0 * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * b0;
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 2] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = a0 * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * b0;
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 3] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 4] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 5] = (c & 65535);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + (d);
            result[rstart + 6] = (p & 65535);
            result[rstart + 7] = (((p >> 16) & 65535));
        }
    };
    constructor['Baseline_Multiply8'] = constructor.Baseline_Multiply8 = function(result, rstart, words1, astart, words2, bstart) {
        var mask = 65535;
        {
            var p;
            var c;
            var d;
            p = (((words1[astart])|0) & mask) * (((words2[bstart])|0) & mask);
            c = (p|0);
            d = ((p|0) >> 16) & mask;
            result[rstart] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 1] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 2] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 3] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 4] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 5] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 6] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 7] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 1])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 1])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 8] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 2])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 2])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 9] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 3])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 3])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 10] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 4])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 4])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 11] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 5])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 5])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 12] = (c & 65535);
            c = (d|0);
            d = ((d|0) >> 16) & mask;
            p = (((words1[astart + 6])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 6])|0) & mask);
            p = p + ((c|0) & mask);
            c = (p|0);
            d = d + (((p|0) >> 16) & mask);
            result[rstart + 13] = (c & 65535);
            p = (((words1[astart + 7])|0) & mask) * (((words2[bstart + 7])|0) & mask);
            p = p + (d);
            result[rstart + 14] = (p & 65535);
            result[rstart + 15] = (((p >> 16) & 65535));
        }
    };
    constructor['RecursionLimit'] = constructor.RecursionLimit = 10;
    constructor['SameSizeMultiply'] = constructor.SameSizeMultiply = function(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, count) {
        if (count <= BigInteger.RecursionLimit) {
            if (count == 2) {
                BigInteger.Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
            } else if (count == 4) {
                BigInteger.Baseline_Multiply4(resultArr, resultStart, words1, words1Start, words2, words2Start);
            } else if (count == 8) {
                BigInteger.Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
            } else {
                BigInteger.SchoolbookMultiply(resultArr, resultStart, words1, words1Start, count, words2, words2Start, count);
            }
        } else {
            var countA = count;
            while (countA != 0 && words1[words1Start + countA - 1] == 0) {
                --countA;
            }
            var countB = count;
            while (countB != 0 && words2[words2Start + countB - 1] == 0) {
                --countB;
            }
            var offset2For1 = 0;
            var offset2For2 = 0;
            if (countA == 0 || countB == 0) {
                for (var arrfillI = resultStart; arrfillI < (resultStart) + (count << 1); arrfillI++) resultArr[arrfillI] = 0;
                return;
            }
            if ((count & 1) == 0) {
                var count2 = count >> 1;
                if (countA <= count2 && countB <= count2) {
                    for (var arrfillI = resultStart + count; arrfillI < (resultStart + count) + (count); arrfillI++) resultArr[arrfillI] = 0;
                    if (count2 == 8) {
                        BigInteger.Baseline_Multiply8(resultArr, resultStart, words1, words1Start, words2, words2Start);
                    } else {
                        BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, count2);
                    }
                    return;
                }
                var resultMediumHigh = resultStart + count;
                var resultHigh = resultMediumHigh + count2;
                var resultMediumLow = resultStart + count2;
                var tsn = tempStart + count;
                offset2For1 = BigInteger.Compare(words1, words1Start, words1, words1Start + count2, count2) > 0 ? 0 : count2;
                BigInteger.SubtractOneByOne(resultArr, resultStart, words1, words1Start + offset2For1, words1, ((words1Start + (count2 ^ offset2For1))|0), count2);
                offset2For2 = BigInteger.Compare(words2, words2Start, words2, ((words2Start + count2)|0), count2) > 0 ? 0 : count2;
                BigInteger.SubtractOneByOne(resultArr, resultMediumLow, words2, words2Start + offset2For2, words2, ((words2Start + (count2 ^ offset2For2))|0), count2);
                BigInteger.SameSizeMultiply(resultArr, resultMediumHigh, tempArr, tsn, words1, ((words1Start + count2)|0), words2, ((words2Start + count2)|0), count2);
                BigInteger.SameSizeMultiply(tempArr, tempStart, tempArr, tsn, resultArr, resultStart, resultArr, (resultMediumLow|0), count2);
                BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tsn, words1, words1Start, words2, words2Start, count2);
                var c2 = BigInteger.AddOneByOne(resultArr, resultMediumHigh, resultArr, resultMediumHigh, resultArr, resultMediumLow, count2);
                var c3 = c2;
                c2 = c2 + (BigInteger.AddOneByOne(resultArr, resultMediumLow, resultArr, resultMediumHigh, resultArr, resultStart, count2));
                c3 = c3 + (BigInteger.AddOneByOne(resultArr, resultMediumHigh, resultArr, resultMediumHigh, resultArr, resultHigh, count2));
                if (offset2For1 == offset2For2) {
                    c3 -= BigInteger.SubtractOneByOne(resultArr, resultMediumLow, resultArr, resultMediumLow, tempArr, tempStart, count);
                } else {
                    c3 = c3 + (BigInteger.AddOneByOne(resultArr, resultMediumLow, resultArr, resultMediumLow, tempArr, tempStart, count));
                }
                c3 = c3 + (BigInteger.Increment(resultArr, resultMediumHigh, count2, (c2|0)));
                if (c3 != 0) {
                    BigInteger.Increment(resultArr, resultHigh, count2, (c3|0));
                }
            } else {
                var countHigh = count >> 1;
                var countLow = count - countHigh;
                var tsnShorter = countHigh + countHigh;
                offset2For1 = BigInteger.CompareWithOneBiggerWords1(words1, words1Start, words1, words1Start + countLow, countLow) > 0 ? 0 : countLow;
                if (offset2For1 == 0) {
                    BigInteger.SubtractOneBiggerWords1(resultArr, resultStart, words1, words1Start, words1, words1Start + countLow, countLow);
                } else {
                    BigInteger.SubtractOneBiggerWords2(resultArr, resultStart, words1, words1Start + countLow, words1, words1Start, countLow);
                }
                offset2For2 = BigInteger.CompareWithOneBiggerWords1(words2, words2Start, words2, words2Start + countLow, countLow) > 0 ? 0 : countLow;
                if (offset2For2 == 0) {
                    BigInteger.SubtractOneBiggerWords1(tempArr, tempStart, words2, words2Start, words2, words2Start + countLow, countLow);
                } else {
                    BigInteger.SubtractOneBiggerWords2(tempArr, tempStart, words2, words2Start + countLow, words2, words2Start, countLow);
                }
                var shorterOffset = countHigh << 1;
                var longerOffset = countLow << 1;
                BigInteger.SameSizeMultiply(tempArr, tempStart + shorterOffset, resultArr, resultStart + shorterOffset, resultArr, resultStart, tempArr, tempStart, countLow);
                var resultTmp0 = tempArr[tempStart + shorterOffset];
                var resultTmp1 = tempArr[tempStart + shorterOffset + 1];
                BigInteger.SameSizeMultiply(resultArr, resultStart + longerOffset, resultArr, resultStart, words1, words1Start + countLow, words2, words2Start + countLow, countHigh);
                BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, countLow);
                tempArr[tempStart + shorterOffset] = (resultTmp0 & 65535);
                tempArr[tempStart + shorterOffset + 1] = (resultTmp1 & 65535);
                var countMiddle = countLow << 1;
                var c2 = BigInteger.AddOneByOne(resultArr, resultStart + countMiddle, resultArr, resultStart + countMiddle, resultArr, resultStart + countLow, countLow);
                var c3 = c2;
                c2 = c2 + (BigInteger.AddOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countMiddle, resultArr, resultStart, countLow));
                c3 = c3 + (BigInteger.AddUnevenSize(resultArr, resultStart + countMiddle, resultArr, resultStart + countMiddle, countLow, resultArr, resultStart + countMiddle + countLow, countLow - 2));
                if (offset2For1 == offset2For2) {
                    c3 -= BigInteger.SubtractOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countLow, tempArr, tempStart + shorterOffset, countLow << 1);
                } else {
                    c3 = c3 + (BigInteger.AddOneByOne(resultArr, resultStart + countLow, resultArr, resultStart + countLow, tempArr, tempStart + shorterOffset, countLow << 1));
                }
                c3 = c3 + (BigInteger.Increment(resultArr, resultStart + countMiddle, countLow, (c2|0)));
                if (c3 != 0) {
                    BigInteger.Increment(resultArr, resultStart + countMiddle + countLow, countLow - 2, (c3|0));
                }
            }
        }
    };
    constructor['RecursiveSquare'] = constructor.RecursiveSquare = function(resultArr, resultStart, tempArr, tempStart, words1, words1Start, count) {
        if (count <= BigInteger.RecursionLimit) {
            if (count == 2) {
                BigInteger.Baseline_Square2(resultArr, resultStart, words1, words1Start);
            } else if (count == 4) {
                BigInteger.Baseline_Square4(resultArr, resultStart, words1, words1Start);
            } else if (count == 8) {
                BigInteger.Baseline_Square8(resultArr, resultStart, words1, words1Start);
            } else {
                BigInteger.SchoolbookSquare(resultArr, resultStart, words1, words1Start, count);
            }
        } else if ((count & 1) == 0) {
            var count2 = count >> 1;
            BigInteger.RecursiveSquare(resultArr, resultStart, tempArr, tempStart + count, words1, words1Start, count2);
            BigInteger.RecursiveSquare(resultArr, resultStart + count, tempArr, tempStart + count, words1, words1Start + count2, count2);
            BigInteger.SameSizeMultiply(tempArr, tempStart, tempArr, tempStart + count, words1, words1Start, words1, words1Start + count2, count2);
            var carry = BigInteger.AddOneByOne(resultArr, ((resultStart + count2)|0), resultArr, ((resultStart + count2)|0), tempArr, tempStart, count);
            carry = carry + (BigInteger.AddOneByOne(resultArr, ((resultStart + count2)|0), resultArr, ((resultStart + count2)|0), tempArr, tempStart, count));
            BigInteger.Increment(resultArr, ((resultStart + count + count2)|0), count2, (carry|0));
        } else {
            BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words1, words1Start, count);
        }
    };
    constructor['SchoolbookSquare'] = constructor.SchoolbookSquare = function(resultArr, resultStart, words1, words1Start, words1Count) {
        var cstart;
        for (var i = 0; i < words1Count; ++i) {
            cstart = resultStart + i;
            {
                var carry = 0;
                var valueBint = ((words1[words1Start + i])|0) & 65535;
                for (var j = 0; j < words1Count; ++j) {
                    var p;
                    p = (words1[words1Start + j] & 65535) * valueBint;
                    p = p + (carry & 65535);
                    if (i != 0) {
                        p = p + (resultArr[cstart + j] & 65535);
                    }
                    resultArr[cstart + j] = (p & 65535);
                    carry = ((p >> 16)|0);
                }
                resultArr[cstart + words1Count] = (carry & 65535);
            }
        }
    };
    constructor['SchoolbookMultiply'] = constructor.SchoolbookMultiply = function(resultArr, resultStart, words1, words1Start, words1Count, words2, words2Start, words2Count) {
        var cstart;
        if (words1Count < words2Count) {
            for (var i = 0; i < words1Count; ++i) {
                cstart = resultStart + i;
                {
                    var carry = 0;
                    var valueBint = ((words1[words1Start + i])|0) & 65535;
                    for (var j = 0; j < words2Count; ++j) {
                        var p;
                        p = (words2[words2Start + j] & 65535) * valueBint;
                        p = p + (carry & 65535);
                        if (i != 0) {
                            p = p + (resultArr[cstart + j] & 65535);
                        }
                        resultArr[cstart + j] = (p & 65535);
                        carry = ((p >> 16)|0);
                    }
                    resultArr[cstart + words2Count] = (carry & 65535);
                }
            }
        } else {
            for (var i = 0; i < words2Count; ++i) {
                cstart = resultStart + i;
                {
                    var carry = 0;
                    var valueBint = ((words2[words2Start + i])|0) & 65535;
                    for (var j = 0; j < words1Count; ++j) {
                        var p;
                        p = (words1[words1Start + j] & 65535) * valueBint;
                        p = p + (carry & 65535);
                        if (i != 0) {
                            p = p + (resultArr[cstart + j] & 65535);
                        }
                        resultArr[cstart + j] = (p & 65535);
                        carry = ((p >> 16)|0);
                    }
                    resultArr[cstart + words1Count] = (carry & 65535);
                }
            }
        }
    };
    constructor['ChunkedLinearMultiply'] = constructor.ChunkedLinearMultiply = function(productArr, cstart, tempArr, tempStart, words1, astart, acount, words2, bstart, bcount) {
        {
            var carryPos = 0;
            for (var arrfillI = cstart; arrfillI < (cstart) + (bcount); arrfillI++) productArr[arrfillI] = 0;
            for (var i = 0; i < acount; i += bcount) {
                var diff = acount - i;
                if (diff > bcount) {
                    BigInteger.SameSizeMultiply(tempArr, tempStart, tempArr, tempStart + bcount + bcount, words1, astart + i, words2, bstart, bcount);
                    BigInteger.AddUnevenSize(tempArr, tempStart, tempArr, tempStart, bcount + bcount, productArr, cstart + carryPos, bcount);
                    for (var arrfillI = 0; arrfillI < bcount + bcount; arrfillI++) productArr[cstart + i + arrfillI] = tempArr[tempStart + arrfillI];
                    carryPos = carryPos + (bcount);
                } else {
                    BigInteger.AsymmetricMultiply(tempArr, tempStart, tempArr, tempStart + diff + bcount, words1, astart + i, diff, words2, bstart, bcount);
                    BigInteger.AddUnevenSize(tempArr, tempStart, tempArr, tempStart, diff + bcount, productArr, cstart + carryPos, bcount);
                    for (var arrfillI = 0; arrfillI < diff + bcount; arrfillI++) productArr[cstart + i + arrfillI] = tempArr[tempStart + arrfillI];
                }
            }
        }
    };
    constructor['AsymmetricMultiply'] = constructor.AsymmetricMultiply = function(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words1Count, words2, words2Start, words2Count) {
        if (words1Count == words2Count) {
            if (words1Start == words2Start && words1 == words2) {
                BigInteger.RecursiveSquare(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words1Count);
            } else if (words1Count == 2) {
                BigInteger.Baseline_Multiply2(resultArr, resultStart, words1, words1Start, words2, words2Start);
            } else {
                BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, words1Count);
            }
            return;
        }
        if (words1Count > words2Count) {
            var tmp1 = words1;
            words1 = words2;
            words2 = tmp1;
            var tmp3 = words1Start;
            words1Start = words2Start;
            words2Start = tmp3;
            var tmp2 = words1Count;
            words1Count = words2Count;
            words2Count = tmp2;
        }
        if (words1Count == 1 || (words1Count == 2 && words1[words1Start + 1] == 0)) {
            switch(words1[words1Start]) {
                case 0:
                    for (var arrfillI = resultStart; arrfillI < (resultStart) + (words2Count + 2); arrfillI++) resultArr[arrfillI] = 0;
                    return;
                case 1:
                    for (var arrfillI = 0; arrfillI < (words2Count|0); arrfillI++) resultArr[resultStart + arrfillI] = words2[words2Start + arrfillI];
                    resultArr[resultStart + words2Count] = 0;
                    resultArr[resultStart + words2Count + 1] = 0;
                    return;
                default:
                    resultArr[resultStart + words2Count] = (((BigInteger.LinearMultiply(resultArr, resultStart, words2, words2Start, words1[words1Start], words2Count)) & 65535));
                    resultArr[resultStart + words2Count + 1] = 0;
                    return;
            }
        } else if (words1Count == 2 && (words2Count & 1) == 0) {
            var a0 = ((words1[words1Start])|0) & 65535;
            var a1 = ((words1[words1Start + 1])|0) & 65535;
            resultArr[resultStart + words2Count] = 0;
            resultArr[resultStart + words2Count + 1] = 0;
            BigInteger.AtomicMultiplyOpt(resultArr, resultStart, a0, a1, words2, words2Start, 0, words2Count);
            BigInteger.AtomicMultiplyAddOpt(resultArr, resultStart, a0, a1, words2, words2Start, 2, words2Count);
            return;
        } else if (words1Count <= 10 && words2Count <= 10) {
            BigInteger.SchoolbookMultiply(resultArr, resultStart, words1, words1Start, words1Count, words2, words2Start, words2Count);
        } else {
            var wordsRem = words2Count % words1Count;
            var evenmult = ((words2Count / words1Count)|0) & 1;
            var i;
            if (wordsRem == 0) {
                if (evenmult == 0) {
                    BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, words1Count);
                    for (var arrfillI = 0; arrfillI < words1Count; arrfillI++) tempArr[((tempStart + (words1Count << 1))|0) + arrfillI] = resultArr[resultStart + words1Count + arrfillI];
                    for (i = words1Count << 1; i < words2Count; i += words1Count << 1) {
                        BigInteger.SameSizeMultiply(tempArr, tempStart + words1Count + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
                    }
                    for (i = words1Count; i < words2Count; i += words1Count << 1) {
                        BigInteger.SameSizeMultiply(resultArr, resultStart + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
                    }
                } else {
                    for (i = 0; i < words2Count; i += words1Count << 1) {
                        BigInteger.SameSizeMultiply(resultArr, resultStart + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
                    }
                    for (i = words1Count; i < words2Count; i += words1Count << 1) {
                        BigInteger.SameSizeMultiply(tempArr, tempStart + words1Count + i, tempArr, tempStart, words1, words1Start, words2, words2Start + i, words1Count);
                    }
                }
                if (BigInteger.Add(resultArr, resultStart + words1Count, resultArr, resultStart + words1Count, tempArr, tempStart + (words1Count << 1), words2Count - words1Count) != 0) {
                    BigInteger.Increment(resultArr, ((resultStart + words2Count)|0), words1Count, 1);
                }
            } else if ((words1Count + words2Count) >= (words1Count << 2)) {
                BigInteger.ChunkedLinearMultiply(resultArr, resultStart, tempArr, tempStart, words2, words2Start, words2Count, words1, words1Start, words1Count);
            } else if (words1Count + 1 == words2Count) {
                BigInteger.SameSizeMultiply(resultArr, resultStart, tempArr, tempStart, words1, words1Start, words2, words2Start, words1Count);
                var carry = BigInteger.LinearMultiplyAdd(resultArr, resultStart + words1Count, words1, words1Start + words1Count, words2[words2Start + words1Count], words1Count);
                resultArr[resultStart + words1Count + words1Count] = (carry & 65535);
            } else {
                var t2 = [];
                for (var arrfillI = 0; arrfillI < words1Count << 2; arrfillI++) t2[arrfillI] = 0;
                BigInteger.ChunkedLinearMultiply(resultArr, resultStart, t2, 0, words2, words2Start, words2Count, words1, words1Start, words1Count);
            }
        }
    };
    constructor['MakeUint'] = constructor.MakeUint = function(first, second) {
        return ((((first & 65535) | ((second|0) << 16))|0));
    };
    constructor['GetLowHalf'] = constructor.GetLowHalf = function(val) {
        return (val & 65535);
    };
    constructor['GetHighHalf'] = constructor.GetHighHalf = function(val) {
        return ((val >>> 16)|0);
    };
    constructor['GetHighHalfAsBorrow'] = constructor.GetHighHalfAsBorrow = function(val) {
        return ((0 - (val >>> 16))|0);
    };
    constructor['BitPrecision'] = constructor.BitPrecision = function(numberValue) {
        if (numberValue == 0) {
            return 0;
        }
        var i = 16;
        {
            if ((numberValue >> 8) == 0) {
                numberValue <<= 8;
                i -= 8;
            }
            if ((numberValue >> 12) == 0) {
                numberValue <<= 4;
                i -= 4;
            }
            if ((numberValue >> 14) == 0) {
                numberValue <<= 2;
                i -= 2;
            }
            if ((numberValue >> 15) == 0) {
                --i;
            }
        }
        return i;
    };
    constructor['Divide32By16'] = constructor.Divide32By16 = function(dividendLow, divisorShort, returnRemainder) {
        var tmpInt;
        var dividendHigh = 0;
        var intDivisor = (divisorShort|0) & 65535;
        for (var i = 0; i < 32; ++i) {
            tmpInt = dividendHigh >> 31;
            dividendHigh <<= 1;
            dividendHigh = ((((dividendHigh | ((dividendLow >> 31) & 1)))|0));
            dividendLow <<= 1;
            tmpInt |= dividendHigh;
            if (((tmpInt >> 31) != 0) || (tmpInt >= intDivisor)) {
                {
                    dividendHigh -= intDivisor;
                    ++dividendLow;
                }
            }
        }
        return returnRemainder ? (dividendHigh & 65535) : (dividendLow & 65535);
    };
    constructor['DivideUnsigned'] = constructor.DivideUnsigned = function(x, y) {
        {
            var iy = (y|0) & 65535;
            if ((x >> 31) == 0) {
                return (((((x|0) / iy) & 65535))|0);
            } else {
                return BigInteger.Divide32By16(x, y, false);
            }
        }
    };
    constructor['RemainderUnsigned'] = constructor.RemainderUnsigned = function(x, y) {
        {
            var iy = (y|0) & 65535;
            if ((x >> 31) == 0) {
                return (((x|0) % iy) & 65535);
            } else {
                return BigInteger.Divide32By16(x, y, true);
            }
        }
    };
    constructor['DivideThreeWordsByTwo'] = constructor.DivideThreeWordsByTwo = function(words1, words1Start, valueB0, valueB1) {
        var valueQ;
        {
            if (((valueB1 + 1)|0) == 0) {
                valueQ = words1[words1Start + 2];
            } else if (valueB1 != 0) {
                valueQ = BigInteger.DivideUnsigned(BigInteger.MakeUint(words1[words1Start + 1], words1[words1Start + 2]), (((valueB1|0) + 1) & 65535));
            } else {
                valueQ = BigInteger.DivideUnsigned(BigInteger.MakeUint(words1[words1Start], words1[words1Start + 1]), valueB0);
            }
            var valueQint = (valueQ|0) & 65535;
            var valueB0int = (valueB0|0) & 65535;
            var valueB1int = (valueB1|0) & 65535;
            var p = valueB0int * valueQint;
            var u = (words1[words1Start] & 65535) - (p & 65535);
            words1[words1Start] = (((BigInteger.GetLowHalf(u)) & 65535));
            u = (words1[words1Start + 1] & 65535) - (p >>> 16) - ((BigInteger.GetHighHalfAsBorrow(u)) & 65535) - (valueB1int * valueQint);
            words1[words1Start + 1] = (((BigInteger.GetLowHalf(u)) & 65535));
            words1[words1Start + 2] = (((words1[words1Start + 2] + BigInteger.GetHighHalf(u)) & 65535));
            while (words1[words1Start + 2] != 0 || (words1[words1Start + 1] & 65535) > (valueB1 & 65535) || (words1[words1Start + 1] == valueB1 && (words1[words1Start] & 65535) >= (valueB0 & 65535))) {
                u = (words1[words1Start] & 65535) - valueB0int;
                words1[words1Start] = (((BigInteger.GetLowHalf(u)) & 65535));
                u = (words1[words1Start + 1] & 65535) - valueB1int - ((BigInteger.GetHighHalfAsBorrow(u)) & 65535);
                words1[words1Start + 1] = (((BigInteger.GetLowHalf(u)) & 65535));
                words1[words1Start + 2] = (((words1[words1Start + 2] + BigInteger.GetHighHalf(u)) & 65535));
                ++valueQ;
            }
        }
        return valueQ;
    };
    constructor['DivideFourWordsByTwo'] = constructor.DivideFourWordsByTwo = function(quotient, quotientStart, words1, words1Start, word2A, word2B, temp) {
        if (word2A == 0 && word2B == 0) {
            quotient[quotientStart] = (words1[words1Start + 2] & 65535);
            quotient[quotientStart + 1] = (words1[words1Start + 3] & 65535);
        } else {
            temp[0] = (words1[words1Start] & 65535);
            temp[1] = (words1[words1Start + 1] & 65535);
            temp[2] = (words1[words1Start + 2] & 65535);
            temp[3] = (words1[words1Start + 3] & 65535);
            var valueQ1 = BigInteger.DivideThreeWordsByTwo(temp, 1, word2A, word2B);
            var valueQ0 = BigInteger.DivideThreeWordsByTwo(temp, 0, word2A, word2B);
            quotient[quotientStart] = (valueQ0 & 65535);
            quotient[quotientStart + 1] = (valueQ1 & 65535);
        }
    };
    constructor['AtomicMultiplyOpt'] = constructor.AtomicMultiplyOpt = function(c, valueCstart, valueA0, valueA1, words2, words2Start, istart, iend) {
        var s;
        var d;
        var first1MinusFirst0 = ((valueA1|0) - valueA0) & 65535;
        valueA1 &= 65535;
        valueA0 &= 65535;
        {
            if (valueA1 >= valueA0) {
                for (var i = istart; i < iend; i += 4) {
                    var valueB0 = ((words2[words2Start + i])|0) & 65535;
                    var valueB1 = ((words2[words2Start + i + 1])|0) & 65535;
                    var csi = valueCstart + i;
                    if (valueB0 >= valueB1) {
                        s = 0;
                        d = first1MinusFirst0 * (((valueB0|0) - valueB1) & 65535);
                    } else {
                        s = (first1MinusFirst0|0);
                        d = (s & 65535) * (((valueB0|0) - valueB1) & 65535);
                    }
                    var valueA0B0 = valueA0 * valueB0;
                    c[csi] = (((((valueA0B0 & 65535) & 65535))|0));
                    var a0b0high = (valueA0B0 >>> 16);
                    var valueA1B1 = valueA1 * valueB1;
                    var tempInt;
                    tempInt = a0b0high + (valueA0B0 & 65535) + (d & 65535) + (valueA1B1 & 65535);
                    c[csi + 1] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = valueA1B1 + ((tempInt >> 16) & 65535) + a0b0high + ((d >> 16) & 65535) + ((valueA1B1 >> 16) & 65535) - (s & 65535);
                    c[csi + 2] = (((((tempInt & 65535) & 65535))|0));
                    c[csi + 3] = (((((((tempInt >> 16) & 65535)) & 65535))|0));
                }
            } else {
                for (var i = istart; i < iend; i += 4) {
                    var valueB0 = ((words2[words2Start + i])|0) & 65535;
                    var valueB1 = ((words2[words2Start + i + 1])|0) & 65535;
                    var csi = valueCstart + i;
                    if (valueB0 > valueB1) {
                        s = (((valueB0|0) - valueB1) & 65535);
                        d = first1MinusFirst0 * (s & 65535);
                    } else {
                        s = 0;
                        d = (((valueA0|0) - valueA1) & 65535) * (((valueB1|0) - valueB0) & 65535);
                    }
                    var valueA0B0 = valueA0 * valueB0;
                    var a0b0high = (valueA0B0 >>> 16);
                    c[csi] = (((((valueA0B0 & 65535) & 65535))|0));
                    var valueA1B1 = valueA1 * valueB1;
                    var tempInt;
                    tempInt = a0b0high + (valueA0B0 & 65535) + (d & 65535) + (valueA1B1 & 65535);
                    c[csi + 1] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = valueA1B1 + ((tempInt >> 16) & 65535) + a0b0high + ((d >> 16) & 65535) + ((valueA1B1 >> 16) & 65535) - (s & 65535);
                    c[csi + 2] = (((((tempInt & 65535) & 65535))|0));
                    c[csi + 3] = (((((((tempInt >> 16) & 65535)) & 65535))|0));
                }
            }
        }
    };
    constructor['AtomicMultiplyAddOpt'] = constructor.AtomicMultiplyAddOpt = function(c, valueCstart, valueA0, valueA1, words2, words2Start, istart, iend) {
        var s;
        var d;
        var first1MinusFirst0 = ((valueA1|0) - valueA0) & 65535;
        valueA1 &= 65535;
        valueA0 &= 65535;
        {
            if (valueA1 >= valueA0) {
                for (var i = istart; i < iend; i += 4) {
                    var b0 = ((words2[words2Start + i])|0) & 65535;
                    var b1 = ((words2[words2Start + i + 1])|0) & 65535;
                    var csi = valueCstart + i;
                    if (b0 >= b1) {
                        s = 0;
                        d = first1MinusFirst0 * (((b0|0) - b1) & 65535);
                    } else {
                        s = (first1MinusFirst0|0);
                        d = (s & 65535) * (((b0|0) - b1) & 65535);
                    }
                    var valueA0B0 = valueA0 * b0;
                    var a0b0high = (valueA0B0 >>> 16);
                    var tempInt;
                    tempInt = valueA0B0 + (c[csi] & 65535);
                    c[csi] = (((((tempInt & 65535) & 65535))|0));
                    var valueA1B1 = valueA1 * b1;
                    var a1b1low = valueA1B1 & 65535;
                    var a1b1high = ((valueA1B1 >> 16)|0) & 65535;
                    tempInt = ((tempInt >> 16) & 65535) + (valueA0B0 & 65535) + (d & 65535) + a1b1low + (c[csi + 1] & 65535);
                    c[csi + 1] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = ((tempInt >> 16) & 65535) + a1b1low + a0b0high + ((d >> 16) & 65535) + a1b1high - (s & 65535) + (c[csi + 2] & 65535);
                    c[csi + 2] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = ((tempInt >> 16) & 65535) + a1b1high + (c[csi + 3] & 65535);
                    c[csi + 3] = (((((tempInt & 65535) & 65535))|0));
                    if ((tempInt >> 16) != 0) {
                        c[csi + 4] = ((c[csi + 4] + 1) & 65535);
                        c[csi + 5] = (((((c[csi + 5] + (((c[csi + 4] == 0) ? 1 : 0)|0)) & 65535))|0));
                    }
                }
            } else {
                for (var i = istart; i < iend; i += 4) {
                    var valueB0 = ((words2[words2Start + i])|0) & 65535;
                    var valueB1 = ((words2[words2Start + i + 1])|0) & 65535;
                    var csi = valueCstart + i;
                    if (valueB0 > valueB1) {
                        s = (((valueB0|0) - valueB1) & 65535);
                        d = first1MinusFirst0 * (s & 65535);
                    } else {
                        s = 0;
                        d = (((valueA0|0) - valueA1) & 65535) * (((valueB1|0) - valueB0) & 65535);
                    }
                    var valueA0B0 = valueA0 * valueB0;
                    var a0b0high = (valueA0B0 >>> 16);
                    var tempInt;
                    tempInt = valueA0B0 + (c[csi] & 65535);
                    c[csi] = (((((tempInt & 65535) & 65535))|0));
                    var valueA1B1 = valueA1 * valueB1;
                    var a1b1low = valueA1B1 & 65535;
                    var a1b1high = (valueA1B1 >>> 16);
                    tempInt = ((tempInt >> 16) & 65535) + (valueA0B0 & 65535) + (d & 65535) + a1b1low + (c[csi + 1] & 65535);
                    c[csi + 1] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = ((tempInt >> 16) & 65535) + a1b1low + a0b0high + ((d >> 16) & 65535) + a1b1high - (s & 65535) + (c[csi + 2] & 65535);
                    c[csi + 2] = (((((tempInt & 65535) & 65535))|0));
                    tempInt = ((tempInt >> 16) & 65535) + a1b1high + (c[csi + 3] & 65535);
                    c[csi + 3] = (((((tempInt & 65535) & 65535))|0));
                    if ((tempInt >> 16) != 0) {
                        c[csi + 4] = ((c[csi + 4] + 1) & 65535);
                        c[csi + 5] = (((((c[csi + 5] + (((c[csi + 4] == 0) ? 1 : 0)|0)) & 65535))|0));
                    }
                }
            }
        }
    };
    constructor['Divide'] = constructor.Divide = function(remainderArr, remainderStart, quotientArr, quotientStart, tempArr, tempStart, words1, words1Start, words1Count, words2, words2Start, words2Count) {
        if (words2Count == 0) {
            throw new Error("division by zero");
        }
        if (words2Count == 1) {
            if (words2[words2Start] == 0) {
                throw new Error("division by zero");
            }
            var smallRemainder = ((BigInteger.FastDivideAndRemainder(quotientArr, quotientStart, words1, words1Start, words1Count, words2[words2Start]))|0) & 65535;
            remainderArr[remainderStart] = (smallRemainder & 65535);
            return;
        }
        var quot = quotientArr;
        if (quotientArr == null) {
            quot = [0, 0];
        }
        var valueTBstart = ((tempStart + (words1Count + 2))|0);
        var valueTPstart = ((tempStart + (words1Count + 2 + words2Count))|0);
        {
            var shiftWords = ((words2[words2Start + words2Count - 1] == 0 ? 1 : 0)|0);
            tempArr[valueTBstart] = 0;
            tempArr[valueTBstart + words2Count - 1] = 0;
            for (var arrfillI = 0; arrfillI < words2Count - shiftWords; arrfillI++) tempArr[((valueTBstart + shiftWords)|0) + arrfillI] = words2[words2Start + arrfillI];
            var shiftBits = ((16 - BigInteger.BitPrecision(tempArr[valueTBstart + words2Count - 1]))|0);
            BigInteger.ShiftWordsLeftByBits(tempArr, valueTBstart, words2Count, shiftBits);
            tempArr[0] = 0;
            tempArr[words1Count] = 0;
            tempArr[words1Count + 1] = 0;
            for (var arrfillI = 0; arrfillI < words1Count; arrfillI++) tempArr[((tempStart + shiftWords)|0) + arrfillI] = words1[words1Start + arrfillI];
            BigInteger.ShiftWordsLeftByBits(tempArr, tempStart, words1Count + 2, shiftBits);
            if (tempArr[tempStart + words1Count + 1] == 0 && (tempArr[tempStart + words1Count] & 65535) <= 1) {
                if (quotientArr != null) {
                    quotientArr[quotientStart + words1Count - words2Count + 1] = 0;
                    quotientArr[quotientStart + words1Count - words2Count] = 0;
                }
                while (tempArr[words1Count] != 0 || BigInteger.Compare(tempArr, ((tempStart + words1Count - words2Count)|0), tempArr, valueTBstart, words2Count) >= 0) {
                    tempArr[words1Count] = (((((tempArr[words1Count] - ((BigInteger.Subtract(tempArr, tempStart + words1Count - words2Count, tempArr, tempStart + words1Count - words2Count, tempArr, valueTBstart, words2Count))|0)) & 65535))|0));
                    if (quotientArr != null) {
                        quotientArr[quotientStart + words1Count - words2Count] = ((quotientArr[quotientStart + words1Count - words2Count] + 1) & 65535);
                    }
                }
            } else {
                words1Count = words1Count + (2);
            }
            var valueBT0 = ((tempArr[valueTBstart + words2Count - 2] + 1)|0);
            var valueBT1 = ((tempArr[valueTBstart + words2Count - 1] + ((valueBT0 == 0 ? 1 : 0)|0))|0);
            var valueTAtomic = [0, 0, 0, 0];
            for (var i = words1Count - 2; i >= words2Count; i -= 2) {
                var qs = (quotientArr == null) ? 0 : quotientStart + i - words2Count;
                BigInteger.DivideFourWordsByTwo(quot, qs, tempArr, ((tempStart + i - 2)|0), valueBT0, valueBT1, valueTAtomic);
                var valueRstart2 = tempStart + i - words2Count;
                var n = words2Count;
                {
                    var quotient0 = quot[qs];
                    var quotient1 = quot[qs + 1];
                    if (quotient1 == 0) {
                        var carry = BigInteger.LinearMultiply(tempArr, valueTPstart, tempArr, valueTBstart, (quotient0|0), n);
                        tempArr[valueTPstart + n] = (carry & 65535);
                        tempArr[valueTPstart + n + 1] = 0;
                    } else if (n == 2) {
                        BigInteger.Baseline_Multiply2(tempArr, valueTPstart, quot, qs, tempArr, valueTBstart);
                    } else {
                        tempArr[valueTPstart + n] = 0;
                        tempArr[valueTPstart + n + 1] = 0;
                        quotient0 &= 65535;
                        quotient1 &= 65535;
                        BigInteger.AtomicMultiplyOpt(tempArr, valueTPstart, quotient0, quotient1, tempArr, valueTBstart, 0, n);
                        BigInteger.AtomicMultiplyAddOpt(tempArr, valueTPstart, quotient0, quotient1, tempArr, valueTBstart, 2, n);
                    }
                    BigInteger.Subtract(tempArr, valueRstart2, tempArr, valueRstart2, tempArr, valueTPstart, n + 2);
                    while (tempArr[valueRstart2 + n] != 0 || BigInteger.Compare(tempArr, valueRstart2, tempArr, valueTBstart, n) >= 0) {
                        tempArr[valueRstart2 + n] = (((((tempArr[valueRstart2 + n] - ((BigInteger.Subtract(tempArr, valueRstart2, tempArr, valueRstart2, tempArr, valueTBstart, n))|0)) & 65535))|0));
                        if (quotientArr != null) {
                            quotientArr[qs] = ((quotientArr[qs] + 1) & 65535);
                            quotientArr[qs + 1] = (((((quotientArr[qs + 1] + (((quotientArr[qs] == 0) ? 1 : 0)|0)) & 65535))|0));
                        }
                    }
                }
            }
            if (remainderArr != null) {
                for (var arrfillI = 0; arrfillI < words2Count; arrfillI++) remainderArr[remainderStart + arrfillI] = tempArr[((tempStart + shiftWords)|0) + arrfillI];
                BigInteger.ShiftWordsRightByBits(remainderArr, remainderStart, words2Count, shiftBits);
            }
        }
    };
    constructor['RoundupSize'] = constructor.RoundupSize = function(n) {
        return n + (n & 1);
    };
    prototype['negative'] = prototype.negative = null;
    prototype['wordCount'] = prototype.wordCount = -1;
    prototype['reg'] = prototype.reg = null;

    constructor['fromByteArray'] = constructor.fromByteArray = function(bytes, littleEndian) {
        var bigint = new BigInteger();
        bigint.fromByteArrayInternal(bytes, littleEndian);
        return bigint;
    };
    prototype['fromByteArrayInternal'] = prototype.fromByteArrayInternal = function(bytes, littleEndian) {
        if (bytes == null) {
            throw new Error("bytes");
        }
        if (bytes.length == 0) {
            this.reg = [0, 0];
            this.wordCount = 0;
        } else {
            var len = bytes.length;
            var wordLength = ((len|0) + 1) >> 1;
            wordLength = BigInteger.RoundupSize(wordLength);
            this.reg = [];
            for (var arrfillI = 0; arrfillI < wordLength; arrfillI++) this.reg[arrfillI] = 0;
            var valueJIndex = littleEndian ? len - 1 : 0;
            var negative = (bytes[valueJIndex] & 128) != 0;
            this.negative = negative;
            var j = 0;
            if (!negative) {
                for (var i = 0; i < len; i += 2, j++) {
                    var index = littleEndian ? i : len - 1 - i;
                    var index2 = littleEndian ? i + 1 : len - 2 - i;
                    this.reg[j] = ((((((bytes[index] & 255)) & 65535))|0));
                    if (index2 >= 0 && index2 < len) {
                        this.reg[j] = (((((this.reg[j] | (((((bytes[index2])|0) << 8)|0))) & 65535))|0));
                    }
                }
            } else {
                for (var i = 0; i < len; i += 2, j++) {
                    var index = littleEndian ? i : len - 1 - i;
                    var index2 = littleEndian ? i + 1 : len - 2 - i;
                    this.reg[j] = ((((((bytes[index] & 255)) & 65535))|0));
                    if (index2 >= 0 && index2 < len) {
                        this.reg[j] = (((((this.reg[j] | (((((bytes[index2])|0) << 8)|0))) & 65535))|0));
                    } else {

                        this.reg[j] = (((this.reg[j] | (65280)) & 65535));
                    }
                }
                for (; j < this.reg.length; ++j) {
                    this.reg[j] = (65535 & 65535);
                }

                BigInteger.TwosComplement(this.reg, 0, ((this.reg.length)|0));
            }
            this.wordCount = this.reg.length;
            while (this.wordCount != 0 && this.reg[this.wordCount - 1] == 0) {
                --this.wordCount;
            }
        }
    };
    prototype['Allocate'] = prototype.Allocate = function(length) {
        this.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(length); arrfillI++) this.reg[arrfillI] = 0;

        this.negative = false;
        this.wordCount = 0;
        return this;
    };
    constructor['GrowForCarry'] = constructor.GrowForCarry = function(a, carry) {
        var oldLength = a.length;
        var ret = BigInteger.CleanGrow(a, BigInteger.RoundupSize(oldLength + 1));
        ret[oldLength] = (carry & 65535);
        return ret;
    };
    constructor['CleanGrow'] = constructor.CleanGrow = function(a, size) {
        if (size > a.length) {
            var newa = [];
            for (var arrfillI = 0; arrfillI < size; arrfillI++) newa[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < a.length; arrfillI++) newa[0 + arrfillI] = a[0 + arrfillI];
            return newa;
        }
        return a;
    };
    prototype['SetBitInternal'] = prototype.SetBitInternal = function(n, value) {
        if (value) {
            this.reg = BigInteger.CleanGrow(this.reg, BigInteger.RoundupSize(BigInteger.BitsToWords(n + 1)));
            this.reg[(n >> 4)] = (((((this.reg[n >> 4] | ((1 << (n & 15))|0)) & 65535))|0));
            this.wordCount = this.CalcWordCount();
        } else {
            if ((n >> 4) < this.reg.length) {
                this.reg[(n >> 4)] = (((((this.reg[n >> 4] & (((~(1 << ((n % 16)|0)))|0))) & 65535))|0));
            }
            this.wordCount = this.CalcWordCount();
        }
    };

    prototype['testBit'] = prototype.testBit = function(index) {
        if (index < 0) {
            throw new Error("index");
        }
        if (this.wordCount == 0) {
            return false;
        }
        if (this.negative) {
            var tcindex = 0;
            var wordpos = ((index / 16)|0);
            if (wordpos >= this.reg.length) {
                return true;
            }
            while (tcindex < wordpos && this.reg[tcindex] == 0) {
                ++tcindex;
            }
            var tc;
            {
                tc = this.reg[wordpos];
                if (tcindex == wordpos) {
                    --tc;
                }
                tc = ((~tc)|0);
            }
            return (((tc >> (index & 15)) & 1) != 0);
        } else {
            return this.GetUnsignedBit(index);
        }
    };
    prototype['GetUnsignedBit'] = prototype.GetUnsignedBit = function(n) {
        if ((n >> 4) >= this.reg.length) {
            return false;
        } else {
            return (((this.reg[n >> 4] >> (n & 15)) & 1) != 0);
        }
    };
    prototype['InitializeInt'] = prototype.InitializeInt = function(numberValue) {
        var iut;
        {
            this.negative = numberValue < 0;
            if (numberValue == -2147483648) {
                this.reg = [0, 0];
                this.reg[0] = 0;
                this.reg[1] = (32768 & 65535);
                this.wordCount = 2;
            } else {
                iut = ((numberValue < 0) ? -numberValue : numberValue);
                this.reg = [0, 0];
                this.reg[0] = (iut & 65535);
                this.reg[1] = (((iut >> 16) & 65535));
                this.wordCount = this.reg[1] != 0 ? 2 : (this.reg[0] == 0 ? 0 : 1);
            }
        }
        return this;
    };

    prototype['toByteArray'] = prototype.toByteArray = function(littleEndian) {
        var sign = this.signum();
        if (sign == 0) {
            return [0];
        } else if (sign > 0) {
            var byteCount = this.ByteCount();
            var byteArrayLength = byteCount;
            if (this.GetUnsignedBit((byteCount * 8) - 1)) {
                ++byteArrayLength;
            }
            var bytes = [];
            for (var arrfillI = 0; arrfillI < byteArrayLength; arrfillI++) bytes[arrfillI] = 0;
            var j = 0;
            for (var i = 0; i < byteCount; i += 2, j++) {
                var index = littleEndian ? i : bytes.length - 1 - i;
                var index2 = littleEndian ? i + 1 : bytes.length - 2 - i;
                bytes[index] = ((this.reg[j] & 255)|0);
                if (index2 >= 0 && index2 < byteArrayLength) {
                    bytes[index2] = ((this.reg[j] >> 8) & 255);
                }
            }
            return bytes;
        } else {
            var regdata = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) regdata[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) regdata[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.TwosComplement(regdata, 0, ((regdata.length)|0));
            var byteCount = regdata.length * 2;
            for (var i = regdata.length - 1; i >= 0; --i) {
                if (regdata[i] == (65535)) {
                    byteCount -= 2;
                } else if ((regdata[i] & 65408) == 65408) {

                    --byteCount;
                    break;
                } else if ((regdata[i] & 32768) == 32768) {

                    break;
                } else {

                    ++byteCount;
                    break;
                }
            }
            if (byteCount == 0) {
                byteCount = 1;
            }
            var bytes = [];
            for (var arrfillI = 0; arrfillI < byteCount; arrfillI++) bytes[arrfillI] = 0;
            bytes[littleEndian ? bytes.length - 1 : 0] = 255;
            byteCount = (byteCount < regdata.length * 2 ? byteCount : regdata.length * 2);
            var j = 0;
            for (var i = 0; i < byteCount; i += 2, j++) {
                var index = littleEndian ? i : bytes.length - 1 - i;
                var index2 = littleEndian ? i + 1 : bytes.length - 2 - i;
                bytes[index] = (regdata[j] & 255);
                if (index2 >= 0 && index2 < byteCount) {
                    bytes[index2] = ((regdata[j] >> 8) & 255);
                }
            }
            return bytes;
        }
    };

    prototype['shiftLeft'] = prototype.shiftLeft = function(numberBits) {
        if (numberBits == 0) {
            return this;
        }
        if (numberBits < 0) {
            if (numberBits == -2147483648) {
                return this.shiftRight(1).shiftRight(2147483647);
            }
            return this.shiftRight(-numberBits);
        }
        var ret = new BigInteger();
        var numWords = ((this.wordCount)|0);
        var shiftWords = ((numberBits >> 4)|0);
        var shiftBits = (numberBits & 15);
        var neg = numWords > 0 && this.negative;
        if (!neg) {
            ret.negative = false;
            ret.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(numWords + BigInteger.BitsToWords(numberBits|0)); arrfillI++) ret.reg[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < numWords; arrfillI++) ret.reg[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
            BigInteger.ShiftWordsLeftByBits(ret.reg, (shiftWords|0), numWords + BigInteger.BitsToWords(shiftBits), shiftBits);
            ret.wordCount = ret.CalcWordCount();
        } else {
            ret.negative = true;
            ret.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(numWords + BigInteger.BitsToWords(numberBits|0)); arrfillI++) ret.reg[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < numWords; arrfillI++) ret.reg[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
            BigInteger.ShiftWordsLeftByWords(ret.reg, 0, numWords + shiftWords, shiftWords);
            BigInteger.ShiftWordsLeftByBits(ret.reg, (shiftWords|0), numWords + BigInteger.BitsToWords(shiftBits), shiftBits);
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
            ret.wordCount = ret.CalcWordCount();
        }
        return ret;
    };

    prototype['shiftRight'] = prototype.shiftRight = function(numberBits) {
        if (numberBits == 0 || this.wordCount == 0) {
            return this;
        }
        if (numberBits < 0) {
            if (numberBits == -2147483648) {
                return this.shiftLeft(1).shiftLeft(2147483647);
            }
            return this.shiftLeft(-numberBits);
        }
        var ret;
        var numWords = ((this.wordCount)|0);
        var shiftWords = ((numberBits >> 4)|0);
        var shiftBits = (numberBits & 15);
        if (this.negative) {
            ret = new BigInteger();
            ret.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) ret.reg[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < numWords; arrfillI++) ret.reg[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
            BigInteger.ShiftWordsRightByWordsSignExtend(ret.reg, 0, numWords, shiftWords);
            if (numWords > shiftWords) {
                BigInteger.ShiftWordsRightByBitsSignExtend(ret.reg, 0, numWords - shiftWords, shiftBits);
            }
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
            ret.wordCount = ret.reg.length;
        } else {
            if (shiftWords >= numWords) {
                return BigInteger.ZERO;
            }
            ret = new BigInteger();
            ret.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) ret.reg[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < numWords - shiftWords; arrfillI++) ret.reg[0 + arrfillI] = this.reg[shiftWords + arrfillI];
            if (shiftBits != 0) {
                BigInteger.ShiftWordsRightByBits(ret.reg, 0, numWords - shiftWords, shiftBits);
            }
            ret.wordCount = numWords - shiftWords;
        }
        ret.negative = this.negative;
        while (ret.wordCount != 0 && ret.reg[ret.wordCount - 1] == 0) {
            ret.wordCount--;
        }
        if (shiftWords > 2) {
            this.ShortenArray();
        }
        return ret;
    };

    constructor['valueOf'] = constructor.valueOf = function(longerValue_obj) {
        var longerValue = JSInteropFactory.createLong(longerValue_obj);
        if (longerValue.signum() == 0) {
            return BigInteger.ZERO;
        }
        if (longerValue.equalsInt(1)) {
            return BigInteger.ONE;
        }
        var ret = new BigInteger();
        {
            ret.negative = longerValue.signum() < 0;
            ret.reg = [0, 0, 0, 0];
            if (longerValue.equals(JSInteropFactory.LONG_MIN_VALUE())) {
                ret.reg[0] = 0;
                ret.reg[1] = 0;
                ret.reg[2] = 0;
                ret.reg[3] = (32768 & 65535);
                ret.wordCount = 4;
            } else {
                var ut = longerValue;
                if (ut.signum() < 0) {
                    ut = ut.negate();
                }
                ret.reg[0] = (((ut.andInt(65535).shortValue()) & 65535));
                ut = ut.shiftRight(16);
                ret.reg[1] = (((ut.andInt(65535).shortValue()) & 65535));
                ut = ut.shiftRight(16);
                ret.reg[2] = (((ut.andInt(65535).shortValue()) & 65535));
                ut = ut.shiftRight(16);
                ret.reg[3] = (((ut.andInt(65535).shortValue()) & 65535));

                ret.wordCount = 4;
                while (ret.wordCount != 0 && ret.reg[ret.wordCount - 1] == 0) {
                    ret.wordCount--;
                }
            }
        }
        return ret;
    };

    prototype['intValue'] = prototype.intValue = function() {
        var c = ((this.wordCount)|0);
        if (c == 0) {
            return 0;
        }
        if (c > 2) {
            throw new Error();
        }
        if (c == 2 && (this.reg[1] & 32768) != 0) {
            if ((((this.reg[1] & 32767)|0) | this.reg[0]) == 0 && this.negative) {
                return -2147483648;
            } else {
                throw new Error();
            }
        } else {
            var ivv = ((this.reg[0])|0) & 65535;
            if (c > 1) {
                ivv |= ((this.reg[1]) & 65535) << 16;
            }
            if (this.negative) {
                ivv = -ivv;
            }
            return ivv;
        }
    };

    prototype['canFitInInt'] = prototype.canFitInInt = function() {
        var c = ((this.wordCount)|0);
        if (c > 2) {
            return false;
        }
        if (c == 2 && (this.reg[1] & 32768) != 0) {
            return this.negative && this.reg[1] == (32768) && this.reg[0] == 0;
        }
        return true;
    };
    prototype['HasSmallValue'] = prototype.HasSmallValue = function() {
        var c = ((this.wordCount)|0);
        if (c > 4) {
            return false;
        }
        if (c == 4 && (this.reg[3] & 32768) != 0) {
            return this.negative && this.reg[3] == (32768) && this.reg[2] == 0 && this.reg[1] == 0 && this.reg[0] == 0;
        }
        return true;
    };

    prototype['longValue'] = prototype.longValue = function() {
        var count = this.wordCount;
        if (count == 0) {
            return JSInteropFactory.createLong(0);
        }
        if (count > 4) {
            throw new Error();
        }
        if (count == 4 && (this.reg[3] & 32768) != 0) {
            if (this.negative && this.reg[3] == (32768) && this.reg[2] == 0 && this.reg[1] == 0 && this.reg[0] == 0) {
                return JSInteropFactory.LONG_MIN_VALUE();
            } else {
                throw new Error();
            }
        } else {
            var tmp = ((this.reg[0])|0) & 65535;
            var vv = JSInteropFactory.createLong(tmp);
            if (count > 1) {
                tmp = ((this.reg[1])|0) & 65535;
                vv = vv.or((JSInteropFactory.createLong(tmp)).shiftLeft(16));
            }
            if (count > 2) {
                tmp = ((this.reg[2])|0) & 65535;
                vv = vv.or((JSInteropFactory.createLong(tmp)).shiftLeft(32));
            }
            if (count > 3) {
                tmp = ((this.reg[3])|0) & 65535;
                vv = vv.or((JSInteropFactory.createLong(tmp)).shiftLeft(48));
            }
            if (this.negative) {
                vv = vv.negate();
            }
            return vv;
        }
    };
    constructor['Power2'] = constructor.Power2 = function(e) {
        var r = new BigInteger().Allocate(BigInteger.BitsToWords((e + 1)|0));
        r.SetBitInternal((e|0), true);

        return r;
    };

    prototype['PowBigIntVar'] = prototype.PowBigIntVar = function(power) {
        if (power == null) {
            throw new Error("power");
        }
        var sign = power.signum();
        if (sign < 0) {
            throw new Error("power is negative");
        }
        var thisVar = this;
        if (sign == 0) {
            return BigInteger.ONE;
        } else if (power.equals(BigInteger.ONE)) {
            return this;
        } else if (power.wordCount == 1 && power.reg[0] == 2) {
            return thisVar.multiply(thisVar);
        } else if (power.wordCount == 1 && power.reg[0] == 3) {
            return (thisVar.multiply(thisVar)).multiply(thisVar);
        }
        var r = BigInteger.ONE;
        while (power.signum() != 0) {
            if (power.testBit(0)) {
                r = r.multiply(thisVar);
            }
            power = power.shiftRight(1);
            if (power.signum() != 0) {
                thisVar = thisVar.multiply(thisVar);
            }
        }
        return r;
    };

    prototype['pow'] = prototype.pow = function(powerSmall) {
        if (powerSmall < 0) {
            throw new Error("power is negative");
        }
        var thisVar = this;
        if (powerSmall == 0) {

            return BigInteger.ONE;
        } else if (powerSmall == 1) {
            return this;
        } else if (powerSmall == 2) {
            return thisVar.multiply(thisVar);
        } else if (powerSmall == 3) {
            return (thisVar.multiply(thisVar)).multiply(thisVar);
        }
        var r = BigInteger.ONE;
        while (powerSmall != 0) {
            if ((powerSmall & 1) != 0) {
                r = r.multiply(thisVar);
            }
            powerSmall >>= 1;
            if (powerSmall != 0) {
                thisVar = thisVar.multiply(thisVar);
            }
        }
        return r;
    };

    prototype['negate'] = prototype.negate = function() {
        var bigintRet = new BigInteger();
        bigintRet.reg = this.reg;

        bigintRet.wordCount = this.wordCount;
        bigintRet.negative = (this.wordCount != 0) && (!this.negative);
        return bigintRet;
    };

    prototype['abs'] = prototype.abs = function() {
        return (this.wordCount == 0 || !this.negative) ? this : this.negate();
    };
    prototype['CalcWordCount'] = prototype.CalcWordCount = function() {
        return ((BigInteger.CountWords(this.reg, this.reg.length))|0);
    };
    prototype['ByteCount'] = prototype.ByteCount = function() {
        var wc = this.wordCount;
        if (wc == 0) {
            return 0;
        }
        var s = this.reg[wc - 1];
        wc = (wc - 1) << 1;
        if (s == 0) {
            return wc;
        }
        return ((s >> 8) == 0) ? wc + 1 : wc + 2;
    };

    prototype['getUnsignedBitLength'] = prototype.getUnsignedBitLength = function() {
        var wc = this.wordCount;
        if (wc != 0) {
            var numberValue = ((this.reg[wc - 1])|0) & 65535;
            wc = (wc - 1) << 4;
            if (numberValue == 0) {
                return wc;
            }
            wc = wc + (16);
            {
                if ((numberValue >> 8) == 0) {
                    numberValue <<= 8;
                    wc -= 8;
                }
                if ((numberValue >> 12) == 0) {
                    numberValue <<= 4;
                    wc -= 4;
                }
                if ((numberValue >> 14) == 0) {
                    numberValue <<= 2;
                    wc -= 2;
                }
                if ((numberValue >> 15) == 0) {
                    --wc;
                }
            }
            return wc;
        } else {
            return 0;
        }
    };
    constructor['getUnsignedBitLengthEx'] = constructor.getUnsignedBitLengthEx = function(numberValue, wordCount) {
        var wc = wordCount;
        if (wc != 0) {
            wc = (wc - 1) << 4;
            if (numberValue == 0) {
                return wc;
            }
            wc = wc + (16);
            {
                if ((numberValue >> 8) == 0) {
                    numberValue <<= 8;
                    wc -= 8;
                }
                if ((numberValue >> 12) == 0) {
                    numberValue <<= 4;
                    wc -= 4;
                }
                if ((numberValue >> 14) == 0) {
                    numberValue <<= 2;
                    wc -= 2;
                }
                if ((numberValue >> 15) == 0) {
                    --wc;
                }
            }
            return wc;
        } else {
            return 0;
        }
    };

    prototype['bitLength'] = prototype.bitLength = function() {
        var wc = this.wordCount;
        if (wc != 0) {
            var numberValue = ((this.reg[wc - 1])|0) & 65535;
            wc = (wc - 1) << 4;
            if (numberValue == (this.negative ? 1 : 0)) {
                return wc;
            }
            wc = wc + (16);
            {
                if (this.negative) {
                    --numberValue;
                    numberValue &= 65535;
                }
                if ((numberValue >> 8) == 0) {
                    numberValue <<= 8;
                    wc -= 8;
                }
                if ((numberValue >> 12) == 0) {
                    numberValue <<= 4;
                    wc -= 4;
                }
                if ((numberValue >> 14) == 0) {
                    numberValue <<= 2;
                    wc -= 2;
                }
                return ((numberValue >> 15) == 0) ? wc - 1 : wc;
            }
        } else {
            return 0;
        }
    };
    constructor['HexChars'] = constructor.HexChars = "0123456789ABCDEF";
    constructor['ReverseChars'] = constructor.ReverseChars = function(chars, offset, length) {
        var half = length >> 1;
        var right = offset + length - 1;
        for (var i = 0; i < half; i++, right--) {
            var value = chars[offset + i];
            chars[offset + i] = chars[right];
            chars[right] = value;
        }
    };
    prototype['SmallValueToString'] = prototype.SmallValueToString = function() {
        var value = this.longValue();
        if (value.equals(JSInteropFactory.LONG_MIN_VALUE())) {
            return "-9223372036854775808";
        }
        var neg = value.signum() < 0;
        var chars = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        var count = 0;
        if (neg) {
            chars[0] = '-';
            ++count;
            value = value.negate();
        }
        while (value.signum() != 0) {
            var digit = BigInteger.HexChars.charAt(value.remainderWithUnsignedDivisor(10).intValue());
            chars[count++] = digit;
            value = value.divideWithUnsignedDivisor(10);
        }
        if (neg) {
            BigInteger.ReverseChars(chars, 1, count - 1);
        } else {
            BigInteger.ReverseChars(chars, 0, count);
        }
        var tmpbuilder = JSInteropFactory.createStringBuilder(16);
        for (var arrfillI = 0; arrfillI < count; arrfillI++) tmpbuilder.append(chars[arrfillI]);
        return tmpbuilder.toString();
    };
    constructor['ApproxLogTenOfTwo'] = constructor.ApproxLogTenOfTwo = function(bitlen) {
        var bitlenLow = bitlen & 65535;
        var bitlenHigh = (bitlen >>> 16);
        var resultLow = 0;
        var resultHigh = 0;
        {
            var p;
            var c;
            var d;
            p = bitlenLow * 34043;
            d = ((p|0) >>> 16);
            c = (d|0);
            d = ((d|0) >>> 16);
            p = bitlenLow * 8346;
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = bitlenHigh * 34043;
            p = p + (c & 65535);
            d = d + ((p|0) >>> 16);
            c = (d|0);
            d = ((d|0) >>> 16);
            p = bitlenLow * 154;
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = bitlenHigh * 8346;
            p = p + (c & 65535);
            c = (p|0);
            d = d + ((p|0) >>> 16);
            p = (c|0) & 65535;
            c = (p|0);
            resultLow = c;
            c = (d|0);
            d = ((d|0) >>> 16);
            p = bitlenHigh * 154;
            p = p + (c & 65535);
            resultHigh = (p|0);
            var result = (resultLow|0) & 65535;
            result |= (resultHigh & 65535) << 16;
            return (result & 2147483647) >> 9;
        }
    };

    prototype['getDigitCount'] = prototype.getDigitCount = function() {
        if (this.signum() == 0) {
            return 1;
        }
        if (this.HasSmallValue()) {
            var value = this.longValue();
            if (value.equals(JSInteropFactory.LONG_MIN_VALUE())) {
                return 19;
            }
            if (value.signum() < 0) {
                value = value.negate();
            }
            if (value.compareToInt(1000000000) >= 0) {
                if (value.compareToLongAsInts(-1486618624, 232830643) >= 0) {
                    return 19;
                }
                if (value.compareToLongAsInts(1569325056, 23283064) >= 0) {
                    return 18;
                }
                if (value.compareToLongAsInts(1874919424, 2328306) >= 0) {
                    return 17;
                }
                if (value.compareToLongAsInts(-1530494976, 232830) >= 0) {
                    return 16;
                }
                if (value.compareToLongAsInts(276447232, 23283) >= 0) {
                    return 15;
                }
                if (value.compareToLongAsInts(1316134912, 2328) >= 0) {
                    return 14;
                }
                if (value.compareToLongAsInts(-727379968, 232) >= 0) {
                    return 13;
                }
                if (value.compareToLongAsInts(1215752192, 23) >= 0) {
                    return 12;
                }
                if (value.compareToLongAsInts(1410065408, 2) >= 0) {
                    return 11;
                }
                if (value.compareToInt(1000000000) >= 0) {
                    return 10;
                }
                return 9;
            } else {
                var v2 = value.intValue();
                if (v2 >= 100000000) {
                    return 9;
                }
                if (v2 >= 10000000) {
                    return 8;
                }
                if (v2 >= 1000000) {
                    return 7;
                }
                if (v2 >= 100000) {
                    return 6;
                }
                if (v2 >= 10000) {
                    return 5;
                }
                if (v2 >= 1000) {
                    return 4;
                }
                if (v2 >= 100) {
                    return 3;
                }
                if (v2 >= 10) {
                    return 2;
                }
                return 1;
            }
        }
        var bitlen = this.getUnsignedBitLength();
        if (bitlen <= 2135) {

            var minDigits = 1 + (((bitlen - 1) * 631305) >> 21);
            var maxDigits = 1 + ((bitlen * 631305) >> 21);
            if (minDigits == maxDigits) {

                return minDigits;
            }
        } else if (bitlen <= 6432162) {

            var minDigits = BigInteger.ApproxLogTenOfTwo(bitlen - 1);
            var maxDigits = BigInteger.ApproxLogTenOfTwo(bitlen);
            if (minDigits == maxDigits) {

                return 1 + minDigits;
            }
        }
        var tempReg = null;
        var wordCount = this.wordCount;
        var i = 0;
        while (wordCount != 0) {
            if (wordCount == 1) {
                var rest = ((tempReg[0])|0) & 65535;
                if (rest >= 10000) {
                    i = i + (5);
                } else if (rest >= 1000) {
                    i = i + (4);
                } else if (rest >= 100) {
                    i = i + (3);
                } else if (rest >= 10) {
                    i = i + (2);
                } else {
                    ++i;
                }
                break;
            } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 32767) {
                var rest = ((tempReg[0])|0) & 65535;
                rest |= (tempReg[1] & 65535) << 16;
                if (rest >= 1000000000) {
                    i = i + (10);
                } else if (rest >= 100000000) {
                    i = i + (9);
                } else if (rest >= 10000000) {
                    i = i + (8);
                } else if (rest >= 1000000) {
                    i = i + (7);
                } else if (rest >= 100000) {
                    i = i + (6);
                } else if (rest >= 10000) {
                    i = i + (5);
                } else if (rest >= 1000) {
                    i = i + (4);
                } else if (rest >= 100) {
                    i = i + (3);
                } else if (rest >= 10) {
                    i = i + (2);
                } else {
                    ++i;
                }
                break;
            } else {
                var wci = wordCount;
                var remainderShort = 0;
                var quo, rem;
                var firstdigit = false;
                var dividend = (tempReg == null) ? this.reg : tempReg;

                while ((wci--) > 0) {
                    var curValue = ((dividend[wci])|0) & 65535;
                    var currentDividend = (((curValue | ((remainderShort|0) << 16))|0));
                    quo = ((currentDividend / 10000)|0);
                    if (!firstdigit && quo != 0) {
                        firstdigit = true;

                        bitlen = BigInteger.getUnsignedBitLengthEx(quo, wci + 1);
                        if (bitlen <= 2135) {

                            var minDigits = 1 + (((bitlen - 1) * 631305) >> 21);
                            var maxDigits = 1 + ((bitlen * 631305) >> 21);
                            if (minDigits == maxDigits) {

                                return i + minDigits + 4;
                            }
                        } else if (bitlen <= 6432162) {

                            var minDigits = BigInteger.ApproxLogTenOfTwo(bitlen - 1);
                            var maxDigits = BigInteger.ApproxLogTenOfTwo(bitlen);
                            if (minDigits == maxDigits) {

                                return i + 1 + minDigits + 4;
                            }
                        }
                    }
                    if (tempReg == null) {
                        if (quo != 0) {
                            tempReg = [];
                            for (var arrfillI = 0; arrfillI < this.wordCount; arrfillI++) tempReg[arrfillI] = 0;
                            for (var arrfillI = 0; arrfillI < tempReg.length; arrfillI++) tempReg[0 + arrfillI] = this.reg[0 + arrfillI];

                            wordCount = wci + 1;
                            tempReg[wci] = (quo & 65535);
                        }
                    } else {
                        tempReg[wci] = (quo & 65535);
                    }
                    rem = currentDividend - (10000 * quo);
                    remainderShort = (rem|0);
                }

                while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
                    --wordCount;
                }
                i = i + (4);
            }
        }
        return i;
    };

    prototype['toString'] = prototype.toString = function() {
        if (this.signum() == 0) {
            return "0";
        }
        if (this.HasSmallValue()) {
            return this.SmallValueToString();
        }
        var tempReg = [];
        for (var arrfillI = 0; arrfillI < this.wordCount; arrfillI++) tempReg[arrfillI] = 0;
        for (var arrfillI = 0; arrfillI < tempReg.length; arrfillI++) tempReg[0 + arrfillI] = this.reg[0 + arrfillI];
        var wordCount = tempReg.length;
        while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
            --wordCount;
        }
        var i = 0;
        var s = [];
        for (var arrfillI = 0; arrfillI < (wordCount << 4) + 1; arrfillI++) s[arrfillI] = 0;
        while (wordCount != 0) {
            if (wordCount == 1 && tempReg[0] > 0 && tempReg[0] <= 32767) {
                var rest = tempReg[0];
                while (rest != 0) {

                    var newrest = (rest * 26215) >> 18;
                    s[i++] = BigInteger.HexChars.charAt(rest - (newrest * 10));
                    rest = newrest;
                }
                break;
            } else if (wordCount == 2 && tempReg[1] > 0 && tempReg[1] <= 32767) {
                var rest = ((tempReg[0])|0) & 65535;
                rest |= (tempReg[1] & 65535) << 16;
                while (rest != 0) {
                    var newrest = ((rest / 10)|0);
                    s[i++] = BigInteger.HexChars.charAt(rest - (newrest * 10));
                    rest = newrest;
                }
                break;
            } else {
                var wci = wordCount;
                var remainderShort = 0;
                var quo, rem;

                while ((wci--) > 0) {
                    var currentDividend = (((((tempReg[wci] & 65535) | ((remainderShort|0) << 16)))|0));
                    quo = ((currentDividend / 10000)|0);
                    tempReg[wci] = (quo & 65535);
                    rem = currentDividend - (10000 * quo);
                    remainderShort = (rem|0);
                }
                var remainderSmall = remainderShort;

                while (wordCount != 0 && tempReg[wordCount - 1] == 0) {
                    --wordCount;
                }

                var newrest = (remainderSmall * 3277) >> 15;
                s[i++] = BigInteger.HexChars.charAt((remainderSmall - (newrest * 10))|0);
                remainderSmall = newrest;
                newrest = (remainderSmall * 3277) >> 15;
                s[i++] = BigInteger.HexChars.charAt((remainderSmall - (newrest * 10))|0);
                remainderSmall = newrest;
                newrest = (remainderSmall * 3277) >> 15;
                s[i++] = BigInteger.HexChars.charAt((remainderSmall - (newrest * 10))|0);
                remainderSmall = newrest;
                s[i++] = BigInteger.HexChars.charAt(remainderSmall);
            }
        }
        BigInteger.ReverseChars(s, 0, i);
        if (this.negative) {
            var sb = JSInteropFactory.createStringBuilder(i + 1);
            sb.append('-');
            for (var arrfillI = 0; arrfillI < (0) + (i); arrfillI++) sb.append(s[arrfillI]);
            return sb.toString();
        } else {
            var tmpbuilder = JSInteropFactory.createStringBuilder(16);
            for (var arrfillI = 0; arrfillI < i; arrfillI++) tmpbuilder.append(s[arrfillI]);
            return tmpbuilder.toString();
        }
    };

    constructor['fromString'] = constructor.fromString = function(str) {
        if (str == null) {
            throw new Error("str");
        }
        return BigInteger.fromSubstring(str, 0, str.length);
    };
    constructor['MaxSafeInt'] = constructor.MaxSafeInt = 214748363;
    constructor['fromSubstring'] = constructor.fromSubstring = function(str, index, endIndex) {
        if (str == null) {
            throw new Error("str");
        }
        if (index < 0) {
            throw new Error("\"str\" not greater or equal to 0 (" + (JSInteropFactory.createLong(index)) + ")");
        }
        if (index > str.length) {
            throw new Error("\"str\" not less or equal to " + (JSInteropFactory.createLong(str.length)) + " (" + (JSInteropFactory.createLong(index)) + ")");
        }
        if (endIndex < 0) {
            throw new Error("\"index\" not greater or equal to 0 (" + (JSInteropFactory.createLong(endIndex)) + ")");
        }
        if (endIndex > str.length) {
            throw new Error("\"index\" not less or equal to " + (JSInteropFactory.createLong(str.length)) + " (" + (JSInteropFactory.createLong(endIndex)) + ")");
        }
        if (endIndex < index) {
            throw new Error("\"endIndex\" not greater or equal to " + (JSInteropFactory.createLong(index)) + " (" + (JSInteropFactory.createLong(endIndex)) + ")");
        }
        if (index == endIndex) {
            throw new Error("No digits");
        }
        var negative = false;
        if (str.charAt(0) == '-') {
            ++index;
            negative = true;
        }
        var bigint = new BigInteger().Allocate(4);
        var haveDigits = false;
        var haveSmallInt = true;
        var smallInt = 0;
        for (var i = index; i < endIndex; ++i) {
            var c = str.charAt(i);
            if (c < '0' || c > '9') {
                throw new Error("Illegal character found");
            }
            haveDigits = true;
            var digit = ((c.charCodeAt(0))-48);
            if (haveSmallInt && smallInt < BigInteger.MaxSafeInt) {
                smallInt *= 10;
                smallInt = smallInt + (digit);
            } else {
                if (haveSmallInt) {
                    bigint.reg[0] = (((smallInt & 65535) & 65535));
                    bigint.reg[1] = (((smallInt >>> 16) & 65535));
                    haveSmallInt = false;
                }

                var carry = 0;
                var n = bigint.reg.length;
                for (var j = 0; j < n; ++j) {
                    var p;
                    {
                        p = ((bigint.reg[j]) & 65535) * 10;
                        p = p + (carry & 65535);
                        bigint.reg[j] = (p & 65535);
                        carry = ((p >> 16)|0);
                    }
                }
                if (carry != 0) {
                    bigint.reg = BigInteger.GrowForCarry(bigint.reg, carry);
                }

                if (digit != 0) {
                    var d = bigint.reg[0] & 65535;
                    if (d <= 65526) {
                        bigint.reg[0] = (((d + digit) & 65535));
                    } else if (BigInteger.Increment(bigint.reg, 0, bigint.reg.length, (digit|0)) != 0) {
                        bigint.reg = BigInteger.GrowForCarry(bigint.reg, 1);
                    }
                }
            }
        }
        if (!haveDigits) {
            throw new Error("No digits");
        }
        if (haveSmallInt) {
            bigint.reg[0] = (((smallInt & 65535) & 65535));
            bigint.reg[1] = (((smallInt >>> 16) & 65535));
        }
        bigint.wordCount = bigint.CalcWordCount();
        bigint.negative = bigint.wordCount != 0 && negative;
        return bigint;
    };

    prototype['getLowestSetBit'] = prototype.getLowestSetBit = function() {
        var retSetBit = 0;
        for (var i = 0; i < this.wordCount; ++i) {
            var c = this.reg[i];
            if (c == 0) {
                retSetBit = retSetBit + (16);
            } else {
                if (((c << 15) & 65535) != 0) {
                    return retSetBit + 0;
                }
                if (((c << 14) & 65535) != 0) {
                    return retSetBit + 1;
                }
                if (((c << 13) & 65535) != 0) {
                    return retSetBit + 2;
                }
                if (((c << 12) & 65535) != 0) {
                    return retSetBit + 3;
                }
                if (((c << 11) & 65535) != 0) {
                    return retSetBit + 4;
                }
                if (((c << 10) & 65535) != 0) {
                    return retSetBit + 5;
                }
                if (((c << 9) & 65535) != 0) {
                    return retSetBit + 6;
                }
                if (((c << 8) & 65535) != 0) {
                    return retSetBit + 7;
                }
                if (((c << 7) & 65535) != 0) {
                    return retSetBit + 8;
                }
                if (((c << 6) & 65535) != 0) {
                    return retSetBit + 9;
                }
                if (((c << 5) & 65535) != 0) {
                    return retSetBit + 10;
                }
                if (((c << 4) & 65535) != 0) {
                    return retSetBit + 11;
                }
                if (((c << 3) & 65535) != 0) {
                    return retSetBit + 12;
                }
                if (((c << 2) & 65535) != 0) {
                    return retSetBit + 13;
                }
                if (((c << 1) & 65535) != 0) {
                    return retSetBit + 14;
                }
                return retSetBit + 15;
            }
        }
        return 0;
    };

    prototype['gcd'] = prototype.gcd = function(bigintSecond) {
        if (bigintSecond == null) {
            throw new Error("bigintSecond");
        }
        if (this.signum() == 0) {
            return (bigintSecond).abs();
        }
        if (bigintSecond.signum() == 0) {
            return (this).abs();
        }
        var thisValue = this.abs();
        bigintSecond = bigintSecond.abs();
        if (bigintSecond.equals(BigInteger.ONE) || thisValue.equals(bigintSecond)) {
            return bigintSecond;
        }
        if (thisValue.equals(BigInteger.ONE)) {
            return thisValue;
        }
        var expOfTwo = (this.getLowestSetBit() < bigintSecond.getLowestSetBit() ? this.getLowestSetBit() : bigintSecond.getLowestSetBit());
        if (thisValue.wordCount <= 10 && bigintSecond.wordCount <= 10) {
            while (true) {
                var bigintA = (thisValue.subtract(bigintSecond)).abs();
                if (bigintA.signum() == 0) {
                    if (expOfTwo != 0) {
                        thisValue = thisValue.shiftLeft(expOfTwo);
                    }
                    return thisValue;
                }
                var setbit = bigintA.getLowestSetBit();
                bigintA = bigintA.shiftRight(setbit);
                bigintSecond = (thisValue.compareTo(bigintSecond) < 0) ? thisValue : bigintSecond;
                thisValue = bigintA;
            }
        } else {
            var temp;
            while (thisValue.signum() != 0) {
                if (thisValue.compareTo(bigintSecond) < 0) {
                    temp = thisValue;
                    thisValue = bigintSecond;
                    bigintSecond = temp;
                }
                thisValue = thisValue.remainder(bigintSecond);
            }
            return bigintSecond;
        }
    };

    prototype['ModPow'] = prototype.ModPow = function(pow, mod) {
        if (pow == null) {
            throw new Error("pow");
        }
        if (pow.signum() < 0) {
            throw new Error("pow is negative");
        }
        var r = BigInteger.ONE;
        var v = this;
        while (pow.signum() != 0) {
            if (pow.testBit(0)) {
                r = (r.multiply(v)).remainder(mod);
            }
            pow = pow.shiftRight(1);
            if (pow.signum() != 0) {
                v = (v.multiply(v)).remainder(mod);
            }
        }
        return r;
    };
    constructor['PositiveSubtract'] = constructor.PositiveSubtract = function(bigintDiff, minuend, subtrahend) {
        var words1Size = minuend.wordCount;
        words1Size = words1Size + (words1Size & 1);
        var words2Size = subtrahend.wordCount;
        words2Size = words2Size + (words2Size & 1);
        if (words1Size == words2Size) {
            if (BigInteger.Compare(minuend.reg, 0, subtrahend.reg, 0, (words1Size|0)) >= 0) {

                BigInteger.Subtract(bigintDiff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (words1Size|0));
                bigintDiff.negative = false;
            } else {

                BigInteger.Subtract(bigintDiff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (words1Size|0));
                bigintDiff.negative = true;
            }
        } else if (words1Size > words2Size) {

            var borrow = ((BigInteger.Subtract(bigintDiff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (words2Size|0)))|0);
            for (var arrfillI = 0; arrfillI < words1Size - words2Size; arrfillI++) bigintDiff.reg[words2Size + arrfillI] = minuend.reg[words2Size + arrfillI];
            borrow = ((BigInteger.Decrement(bigintDiff.reg, words2Size, ((words1Size - words2Size)|0), borrow))|0);

            bigintDiff.negative = false;
        } else {

            var borrow = ((BigInteger.Subtract(bigintDiff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (words1Size|0)))|0);
            for (var arrfillI = 0; arrfillI < words2Size - words1Size; arrfillI++) bigintDiff.reg[words1Size + arrfillI] = subtrahend.reg[words1Size + arrfillI];
            borrow = ((BigInteger.Decrement(bigintDiff.reg, words1Size, ((words2Size - words1Size)|0), borrow))|0);

            bigintDiff.negative = true;
        }
        bigintDiff.wordCount = bigintDiff.CalcWordCount();
        bigintDiff.ShortenArray();
        if (bigintDiff.wordCount == 0) {
            bigintDiff.negative = false;
        }
    };

    prototype['equals'] = prototype.equals = function(obj) {
        var other = ((obj.constructor==BigInteger) ? obj : null);
        if (other == null) {
            return false;
        }
        if (this.wordCount == other.wordCount) {
            if (this.negative != other.negative) {
                return false;
            }
            for (var i = 0; i < this.wordCount; ++i) {
                if (this.reg[i] != other.reg[i]) {
                    return false;
                }
            }
            return true;
        }
        return false;
    };

    prototype['hashCode'] = prototype.hashCode = function() {
        var hashCodeValue = 0;
        {
            hashCodeValue = hashCodeValue + (1000000007 * this.signum());
            if (this.reg != null) {
                for (var i = 0; i < this.wordCount; ++i) {
                    hashCodeValue = hashCodeValue + (1000000013 * this.reg[i]);
                }
            }
        }
        return hashCodeValue;
    };

    prototype['add'] = prototype.add = function(bigintAugend) {
        if (bigintAugend == null) {
            throw new Error("bigintAugend");
        }
        var sum;
        if (this.wordCount == 0) {
            return bigintAugend;
        }
        if (bigintAugend.wordCount == 0) {
            return this;
        }
        if (bigintAugend.wordCount == 1 && this.wordCount == 1) {
            if (this.negative == bigintAugend.negative) {
                var intSum = ((this.reg[0]) & 65535) + ((bigintAugend.reg[0]) & 65535);
                sum = new BigInteger();
                sum.reg = [0, 0];
                sum.reg[0] = (intSum & 65535);
                sum.reg[1] = (((intSum >> 16) & 65535));
                sum.wordCount = ((intSum >> 16) == 0) ? 1 : 2;
                sum.negative = this.negative;
                return sum;
            } else {
                var a = ((this.reg[0])|0) & 65535;
                var b = ((bigintAugend.reg[0])|0) & 65535;
                if (a == b) {
                    return BigInteger.ZERO;
                }
                if (a > b) {
                    a -= b;
                    sum = new BigInteger();
                    sum.reg = [0, 0];
                    sum.reg[0] = (a & 65535);
                    sum.wordCount = 1;
                    sum.negative = this.negative;
                    return sum;
                } else {
                    b -= a;
                    sum = new BigInteger();
                    sum.reg = [0, 0];
                    sum.reg[0] = (b & 65535);
                    sum.wordCount = 1;
                    sum.negative = !this.negative;
                    return sum;
                }
            }
        }
        sum = new BigInteger().Allocate((this.reg.length > bigintAugend.reg.length ? this.reg.length : bigintAugend.reg.length)|0);
        if ((!this.negative) == (!bigintAugend.negative)) {

            var carry;
            var addendCount = this.wordCount;
            var augendCount = bigintAugend.wordCount;
            var desiredLength = (addendCount > augendCount ? addendCount : augendCount);
            if (addendCount == augendCount) {
                carry = BigInteger.AddOneByOne(sum.reg, 0, this.reg, 0, bigintAugend.reg, 0, (addendCount|0));
            } else if (addendCount > augendCount) {

                carry = BigInteger.AddOneByOne(sum.reg, 0, this.reg, 0, bigintAugend.reg, 0, augendCount);
                for (var arrfillI = 0; arrfillI < addendCount - augendCount; arrfillI++) sum.reg[augendCount + arrfillI] = this.reg[augendCount + arrfillI];
                if (carry != 0) {
                    carry = BigInteger.Increment(sum.reg, augendCount, addendCount - augendCount, (carry|0));
                }
            } else {

                carry = BigInteger.AddOneByOne(sum.reg, 0, this.reg, 0, bigintAugend.reg, 0, (addendCount|0));
                for (var arrfillI = 0; arrfillI < augendCount - addendCount; arrfillI++) sum.reg[addendCount + arrfillI] = bigintAugend.reg[addendCount + arrfillI];
                if (carry != 0) {
                    carry = BigInteger.Increment(sum.reg, addendCount, ((augendCount - addendCount)|0), (carry|0));
                }
            }
            var needShorten = true;
            if (carry != 0) {
                var nextIndex = desiredLength;
                var len = BigInteger.RoundupSize(nextIndex + 1);
                sum.reg = BigInteger.CleanGrow(sum.reg, len);
                sum.reg[nextIndex] = (carry & 65535);
                needShorten = false;
            }
            sum.negative = false;
            sum.wordCount = sum.CalcWordCount();
            if (needShorten) {
                sum.ShortenArray();
            }
            sum.negative = this.negative && sum.signum() != 0;
        } else if (this.negative) {
            BigInteger.PositiveSubtract(sum, bigintAugend, this);
        } else {

            BigInteger.PositiveSubtract(sum, this, bigintAugend);
        }

        return sum;
    };

    prototype['subtract'] = prototype.subtract = function(subtrahend) {
        if (subtrahend == null) {
            throw new Error("subtrahend");
        }
        if (this.wordCount == 0) {
            return subtrahend.negate();
        }
        if (subtrahend.wordCount == 0) {
            return this;
        }
        return this.add(subtrahend.negate());
    };
    prototype['ShortenArray'] = prototype.ShortenArray = function() {
        if (this.reg.length > 32) {
            var newLength = BigInteger.RoundupSize(this.wordCount);
            if (newLength < this.reg.length && (this.reg.length - newLength) >= 16) {

                var newreg = [];
                for (var arrfillI = 0; arrfillI < newLength; arrfillI++) newreg[arrfillI] = 0;
                for (var arrfillI = 0; arrfillI < (newLength < this.reg.length ? newLength : this.reg.length); arrfillI++) newreg[0 + arrfillI] = this.reg[0 + arrfillI];
                this.reg = newreg;
            }
        }
    };

    prototype['multiply'] = prototype.multiply = function(bigintMult) {
        if (bigintMult == null) {
            throw new Error("bigintMult");
        }
        if (this.wordCount == 0 || bigintMult.wordCount == 0) {
            return BigInteger.ZERO;
        }
        if (this.wordCount == 1 && this.reg[0] == 1) {
            return this.negative ? bigintMult.negate() : bigintMult;
        }
        if (bigintMult.wordCount == 1 && bigintMult.reg[0] == 1) {
            return bigintMult.negative ? this.negate() : this;
        }
        var product = new BigInteger();
        var needShorten = true;
        if (this.wordCount == 1) {
            var wc = bigintMult.wordCount;
            var regLength = BigInteger.RoundupSize(wc + 1);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < regLength; arrfillI++) product.reg[arrfillI] = 0;
            product.reg[wc] = (((BigInteger.LinearMultiply(product.reg, 0, bigintMult.reg, 0, this.reg[0], wc)) & 65535));
            product.negative = false;
            product.wordCount = product.reg.length;
            needShorten = false;
        } else if (bigintMult.wordCount == 1) {
            var wc = this.wordCount;
            var regLength = BigInteger.RoundupSize(wc + 1);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < regLength; arrfillI++) product.reg[arrfillI] = 0;
            product.reg[wc] = (((BigInteger.LinearMultiply(product.reg, 0, this.reg, 0, bigintMult.reg[0], wc)) & 65535));
            product.negative = false;
            product.wordCount = product.reg.length;
            needShorten = false;
        } else if (this.wordCount <= 10 && bigintMult.wordCount <= 10) {
            var wc = this.wordCount + bigintMult.wordCount;
            wc = BigInteger.RoundupSize(wc);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < wc; arrfillI++) product.reg[arrfillI] = 0;
            product.negative = false;
            product.wordCount = product.reg.length;
            BigInteger.SchoolbookMultiply(product.reg, 0, this.reg, 0, this.wordCount, bigintMult.reg, 0, bigintMult.wordCount);
            needShorten = false;
        } else if (this.equals(bigintMult)) {
            var words1Size = BigInteger.RoundupSize(this.wordCount);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < words1Size + words1Size; arrfillI++) product.reg[arrfillI] = 0;
            product.wordCount = product.reg.length;
            product.negative = false;
            var workspace = [];
            for (var arrfillI = 0; arrfillI < words1Size + words1Size; arrfillI++) workspace[arrfillI] = 0;
            BigInteger.RecursiveSquare(product.reg, 0, workspace, 0, this.reg, 0, words1Size);
        } else {
            var words1Size = this.wordCount;
            var words2Size = bigintMult.wordCount;
            words1Size = BigInteger.RoundupSize(words1Size);
            words2Size = BigInteger.RoundupSize(words2Size);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(words1Size + words2Size); arrfillI++) product.reg[arrfillI] = 0;
            product.negative = false;
            var workspace = [];
            for (var arrfillI = 0; arrfillI < words1Size + words2Size; arrfillI++) workspace[arrfillI] = 0;
            product.wordCount = product.reg.length;
            BigInteger.AsymmetricMultiply(product.reg, 0, workspace, 0, this.reg, 0, words1Size, bigintMult.reg, 0, words2Size);
        }

        while (product.wordCount != 0 && product.reg[product.wordCount - 1] == 0) {
            product.wordCount--;
        }
        if (needShorten) {
            product.ShortenArray();
        }
        if (this.negative != bigintMult.negative) {
            product.NegateInternal();
        }
        return product;
    };
    constructor['BitsToWords'] = constructor.BitsToWords = function(bitCount) {
        return (bitCount + 15) >> 4;
    };
    constructor['FastRemainder'] = constructor.FastRemainder = function(dividendReg, count, divisorSmall) {
        var i = count;
        var remainder = 0;
        while ((i--) > 0) {
            remainder = BigInteger.RemainderUnsigned(BigInteger.MakeUint(dividendReg[i], remainder), divisorSmall);
        }
        return remainder;
    };
    constructor['FastDivide'] = constructor.FastDivide = function(quotientReg, dividendReg, count, divisorSmall) {
        var i = count;
        var remainderShort = 0;
        var idivisor = (divisorSmall|0) & 65535;
        var quo, rem;
        while ((i--) > 0) {
            var currentDividend = (((((dividendReg[i] & 65535) | ((remainderShort|0) << 16)))|0));
            if ((currentDividend >> 31) == 0) {
                quo = ((currentDividend / idivisor)|0);
                quotientReg[i] = (quo & 65535);
                if (i > 0) {
                    rem = currentDividend - (idivisor * quo);
                    remainderShort = (rem|0);
                }
            } else {
                quotientReg[i] = (((BigInteger.DivideUnsigned(currentDividend, divisorSmall)) & 65535));
                if (i > 0) {
                    remainderShort = BigInteger.RemainderUnsigned(currentDividend, divisorSmall);
                }
            }
        }
    };
    constructor['FastDivideAndRemainder'] = constructor.FastDivideAndRemainder = function(quotientReg, quotientStart, dividendReg, dividendStart, count, divisorSmall) {
        var i = count;
        var remainderShort = 0;
        var idivisor = (divisorSmall|0) & 65535;
        var quo, rem;
        while ((i--) > 0) {
            var currentDividend = (((((dividendReg[dividendStart + i] & 65535) | ((remainderShort|0) << 16)))|0));
            if ((currentDividend >> 31) == 0) {
                quo = ((currentDividend / idivisor)|0);
                quotientReg[quotientStart + i] = (quo & 65535);
                rem = currentDividend - (idivisor * quo);
                remainderShort = (rem|0);
            } else {
                quotientReg[quotientStart + i] = (((BigInteger.DivideUnsigned(currentDividend, divisorSmall)) & 65535));
                remainderShort = BigInteger.RemainderUnsigned(currentDividend, divisorSmall);
            }
        }
        return remainderShort;
    };

    prototype['divide'] = prototype.divide = function(bigintDivisor) {
        if (bigintDivisor == null) {
            throw new Error("bigintDivisor");
        }
        var words1Size = this.wordCount;
        var words2Size = bigintDivisor.wordCount;

        if (words2Size == 0) {
            throw new Error();
        }
        if (words1Size < words2Size) {

            return BigInteger.ZERO;
        }
        if (words1Size <= 2 && words2Size <= 2 && this.canFitInInt() && bigintDivisor.canFitInInt()) {
            var valueASmall = this.intValue();
            var valueBSmall = bigintDivisor.intValue();
            if (valueASmall != -2147483648 || valueBSmall != -1) {
                var result = ((valueASmall / valueBSmall)|0);
                return new BigInteger().InitializeInt(result);
            }
        }
        var quotient;
        if (words2Size == 1) {

            quotient = new BigInteger();
            quotient.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) quotient.reg[arrfillI] = 0;
            quotient.wordCount = this.wordCount;
            quotient.negative = this.negative;
            BigInteger.FastDivide(quotient.reg, this.reg, words1Size, bigintDivisor.reg[0]);
            while (quotient.wordCount != 0 && quotient.reg[quotient.wordCount - 1] == 0) {
                quotient.wordCount--;
            }
            if (quotient.wordCount != 0) {
                quotient.negative = this.negative ^ bigintDivisor.negative;
                return quotient;
            } else {
                return BigInteger.ZERO;
            }
        }

        quotient = new BigInteger();
        words1Size = words1Size + (words1Size & 1);
        words2Size = words2Size + (words2Size & 1);
        quotient.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((words1Size - words2Size + 2)|0); arrfillI++) quotient.reg[arrfillI] = 0;
        quotient.negative = false;
        var tempbuf = [];
        for (var arrfillI = 0; arrfillI < words1Size + (3 * (words2Size + 2)); arrfillI++) tempbuf[arrfillI] = 0;
        BigInteger.Divide(null, 0, quotient.reg, 0, tempbuf, 0, this.reg, 0, words1Size, bigintDivisor.reg, 0, words2Size);
        quotient.wordCount = quotient.CalcWordCount();
        quotient.ShortenArray();
        if ((this.signum() < 0) ^ (bigintDivisor.signum() < 0)) {
            quotient.NegateInternal();
        }
        return quotient;
    };

    prototype['divideAndRemainder'] = prototype.divideAndRemainder = function(divisor) {
        if (divisor == null) {
            throw new Error("divisor");
        }
        var quotient;
        var words1Size = this.wordCount;
        var words2Size = divisor.wordCount;
        if (words2Size == 0) {
            throw new Error();
        }
        if (words1Size < words2Size) {

            return [BigInteger.ZERO, this];
        }
        if (words2Size == 1) {

            quotient = new BigInteger();
            quotient.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) quotient.reg[arrfillI] = 0;
            quotient.wordCount = this.wordCount;
            quotient.negative = this.negative;
            var smallRemainder = ((BigInteger.FastDivideAndRemainder(quotient.reg, 0, this.reg, 0, words1Size, divisor.reg[0]))|0) & 65535;
            while (quotient.wordCount != 0 && quotient.reg[quotient.wordCount - 1] == 0) {
                quotient.wordCount--;
            }
            quotient.ShortenArray();
            if (quotient.wordCount != 0) {
                quotient.negative = this.negative ^ divisor.negative;
            } else {
                quotient = BigInteger.ZERO;
            }
            if (this.negative) {
                smallRemainder = -smallRemainder;
            }
            return [quotient, new BigInteger().InitializeInt(smallRemainder)];
        }
        if (this.wordCount == 2 && divisor.wordCount == 2 && (this.reg[1] >> 15) != 0 && (divisor.reg[1] >> 15) != 0) {
            var a = ((this.reg[0])|0) & 65535;
            var b = ((divisor.reg[0])|0) & 65535;
            {
                a |= ((this.reg[1]) & 65535) << 16;
                b |= ((divisor.reg[1]) & 65535) << 16;
                var quo = ((a / b)|0);
                if (this.negative) {
                    quo = -quo;
                }
                var rem = a - (b * quo);
                return [new BigInteger().InitializeInt(quo), new BigInteger().InitializeInt(rem)];
            }
        }
        var remainder = new BigInteger();
        quotient = new BigInteger();
        words1Size = words1Size + (words1Size & 1);
        words2Size = words2Size + (words2Size & 1);
        remainder.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(words2Size|0); arrfillI++) remainder.reg[arrfillI] = 0;
        remainder.negative = false;
        quotient.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((words1Size - words2Size + 2)|0); arrfillI++) quotient.reg[arrfillI] = 0;
        quotient.negative = false;
        var tempbuf = [];
        for (var arrfillI = 0; arrfillI < words1Size + (3 * (words2Size + 2)); arrfillI++) tempbuf[arrfillI] = 0;
        BigInteger.Divide(remainder.reg, 0, quotient.reg, 0, tempbuf, 0, this.reg, 0, words1Size, divisor.reg, 0, words2Size);
        remainder.wordCount = remainder.CalcWordCount();
        quotient.wordCount = quotient.CalcWordCount();

        remainder.ShortenArray();
        quotient.ShortenArray();
        if (this.signum() < 0) {
            quotient.NegateInternal();
            if (remainder.signum() != 0) {
                remainder.NegateInternal();
            }
        }
        if (divisor.signum() < 0) {
            quotient.NegateInternal();
        }
        return [quotient, remainder];
    };

    prototype['mod'] = prototype.mod = function(divisor) {
        if (divisor == null) {
            throw new Error("divisor");
        }
        if (divisor.signum() < 0) {
            throw new Error("Divisor is negative");
        }
        var rem = this.remainder(divisor);
        if (rem.signum() < 0) {
            rem = divisor.subtract(rem);
        }
        return rem;
    };

    prototype['remainder'] = prototype.remainder = function(divisor) {
        if (divisor == null) {
            throw new Error("divisor");
        }
        var words1Size = this.wordCount;
        var words2Size = divisor.wordCount;
        if (words2Size == 0) {
            throw new Error();
        }
        if (words1Size < words2Size) {

            return this;
        }
        if (words2Size == 1) {
            var shortRemainder = BigInteger.FastRemainder(this.reg, this.wordCount, divisor.reg[0]);
            var smallRemainder = (shortRemainder|0) & 65535;
            if (this.negative) {
                smallRemainder = -smallRemainder;
            }
            return new BigInteger().InitializeInt(smallRemainder);
        }
        if (this.PositiveCompare(divisor) < 0) {
            if (divisor.signum() == 0) {
                throw new Error();
            }
            return this;
        }
        var remainder = new BigInteger();
        words1Size = words1Size + (words1Size & 1);
        words2Size = words2Size + (words2Size & 1);
        remainder.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(words2Size|0); arrfillI++) remainder.reg[arrfillI] = 0;
        remainder.negative = false;
        var tempbuf = [];
        for (var arrfillI = 0; arrfillI < words1Size + (3 * (words2Size + 2)); arrfillI++) tempbuf[arrfillI] = 0;
        BigInteger.Divide(remainder.reg, 0, null, 0, tempbuf, 0, this.reg, 0, words1Size, divisor.reg, 0, words2Size);
        remainder.wordCount = remainder.CalcWordCount();
        remainder.ShortenArray();
        if (this.signum() < 0 && remainder.signum() != 0) {
            remainder.NegateInternal();
        }
        return remainder;
    };
    prototype['NegateInternal'] = prototype.NegateInternal = function() {
        if (this.wordCount != 0) {
            this.negative = this.signum() > 0;
        }
    };
    prototype['PositiveCompare'] = prototype.PositiveCompare = function(t) {
        var size = this.wordCount, tempSize = t.wordCount;
        if (size == tempSize) {
            return BigInteger.Compare(this.reg, 0, t.reg, 0, (size|0));
        } else {
            return size > tempSize ? 1 : -1;
        }
    };

    prototype['compareTo'] = prototype.compareTo = function(other) {
        if (other == null) {
            return 1;
        }
        if (this == other) {
            return 0;
        }
        var size = this.wordCount, tempSize = other.wordCount;
        var sa = size == 0 ? 0 : (this.negative ? -1 : 1);
        var sb = tempSize == 0 ? 0 : (other.negative ? -1 : 1);
        if (sa != sb) {
            return (sa < sb) ? -1 : 1;
        }
        if (sa == 0) {
            return 0;
        }
        if (size == tempSize) {
            if (size == 1 && this.reg[0] == other.reg[0]) {
                return 0;
            } else {
                var words1 = this.reg;
                var words2 = other.reg;
                while ((size--) != 0) {
                    var an = ((words1[size])|0) & 65535;
                    var bn = ((words2[size])|0) & 65535;
                    if (an > bn) {
                        return (sa > 0) ? 1 : -1;
                    } else if (an < bn) {
                        return (sa > 0) ? -1 : 1;
                    }
                }
                return 0;
            }
        } else {
            return ((size > tempSize) ^ (sa <= 0)) ? 1 : -1;
        }
    };

    prototype['signum'] = prototype.signum = function() {
        if (this.wordCount == 0) {
            return 0;
        }
        return this.negative ? -1 : 1;
    };

    prototype['isZero'] = prototype.isZero = function() {
        return this.wordCount == 0;
    };

    prototype['sqrt'] = prototype.sqrt = function() {
        var srrem = this.sqrtWithRemainder();
        return srrem[0];
    };
    constructor['WordsToBigInt'] = constructor.WordsToBigInt = function(words, start, count) {
        if (count == 0) {
            return BigInteger.ZERO;
        }
        var newwords = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(count); arrfillI++) newwords[arrfillI] = 0;
        for (var arrfillI = 0; arrfillI < count; arrfillI++) newwords[0 + arrfillI] = words[start + arrfillI];
        var ret = new BigInteger();
        ret.reg = newwords;
        ret.wordCount = count;
        return ret;
    };
    prototype['sqrtWithRemainder'] = prototype.sqrtWithRemainder = function() {
        if (this.signum() <= 0) {
            return [BigInteger.ZERO, BigInteger.ZERO];
        }
        if (this.equals(BigInteger.ONE)) {
            return [BigInteger.ONE, BigInteger.ZERO];
        }
        var bigintX;
        var bigintY;
        var thisValue = this;
        var powerBits = (((thisValue.getUnsignedBitLength() + 1) / 2)|0);
        if (thisValue.canFitInInt()) {
            var smallValue = thisValue.intValue();
            if (smallValue == 0) {
                return [BigInteger.ZERO, BigInteger.ZERO];
            }
            var smallintX = 0;
            var smallintY = 1 << powerBits;
            do {
                smallintX = smallintY;
                smallintY = ((smallValue / smallintX)|0);
                smallintY = smallintY + (smallintX);
                smallintY >>= 1;
            } while (smallintY < smallintX);
            smallintY = smallintX * smallintX;
            smallintY = smallValue - smallintY;
            return [BigInteger.valueOf(smallintX), BigInteger.valueOf(smallintY)];
        } else {
            bigintX = null;
            bigintY = BigInteger.Power2(powerBits);
            do {
                bigintX = bigintY;
                bigintY = thisValue.divide(bigintX);
                bigintY = bigintY.add(bigintX);
                bigintY = bigintY.shiftRight(1);
            } while (bigintY.compareTo(bigintX) < 0);
            bigintY = bigintX.multiply(bigintX);
            bigintY = thisValue.subtract(bigintY);
            return [bigintX, bigintY];
        }
    };

    prototype['isEven'] = prototype.isEven = function() {
        return !this.GetUnsignedBit(0);
    };
    constructor['ZERO'] = constructor.ZERO = new BigInteger().InitializeInt(0);
    constructor['ONE'] = constructor.ONE = new BigInteger().InitializeInt(1);
    constructor['TEN'] = constructor.TEN = new BigInteger().InitializeInt(10);
})(BigInteger,BigInteger.prototype);

if(typeof exports!=="undefined")exports['BigInteger']=BigInteger;
if(typeof window!=="undefined")window['BigInteger']=BigInteger;

var FastInteger =

function(value) {

    this.smallValue = value;
};
(function(constructor,prototype){
    constructor.MutableNumber = function FastInteger$MutableNumber(val) {

        if (val < 0) {
            throw new Error("Only positive integers are supported");
        }
        this.data = [0, 0, 0, 0];
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = ((val & 0xFFFFFFFF)|0);
    };
    (function(constructor,prototype){
        prototype.data = null;
        prototype.wordCount = null;
        constructor.FromBigInteger = function(bigintVal) {
            var mnum = new FastInteger.MutableNumber(0);
            if (bigintVal.signum() < 0) {
                throw new Error("Only positive integers are supported");
            }
            var bytes = bigintVal.toByteArray(true);
            var len = bytes.length;
            var newWordCount = (4 > ((len / 4)|0) + 1 ? 4 : ((len / 4)|0) + 1);
            if (newWordCount > mnum.data.length) {
                mnum.data = [];
                for (var arrfillI = 0; arrfillI < newWordCount; arrfillI++) mnum.data[arrfillI] = 0;
            }
            mnum.wordCount = newWordCount;
            {
                for (var i = 0; i < len; i += 4) {
                    var x = ((bytes[i])|0) & 255;
                    if (i + 1 < len) {
                        x |= (bytes[i + 1] & 255) << 8;
                    }
                    if (i + 2 < len) {
                        x |= (bytes[i + 2] & 255) << 16;
                    }
                    if (i + 3 < len) {
                        x |= (bytes[i + 3] & 255) << 24;
                    }
                    mnum.data[i >> 2] = x;
                }
            }
            while (mnum.wordCount != 0 && mnum.data[mnum.wordCount - 1] == 0) {
                mnum.wordCount--;
            }
            return mnum;
        };
        prototype.SetInt = function(val) {
            if (val < 0) {
                throw new Error("Only positive integers are supported");
            }
            this.wordCount = (val == 0) ? 0 : 1;
            this.data[0] = ((val & 0xFFFFFFFF)|0);
            return this;
        };
        prototype.ToBigInteger = function() {
            if (this.wordCount == 1 && (this.data[0] >> 31) == 0) {
                return BigInteger.valueOf((this.data[0])|0);
            }
            var bytes = [];
            for (var arrfillI = 0; arrfillI < (this.wordCount * 4) + 1; arrfillI++) bytes[arrfillI] = 0;
            for (var i = 0; i < this.wordCount; ++i) {
                bytes[i * 4] = ((this.data[i] & 255)|0);
                bytes[(i * 4) + 1] = ((this.data[i] >> 8) & 255);
                bytes[(i * 4) + 2] = ((this.data[i] >> 16) & 255);
                bytes[(i * 4) + 3] = ((this.data[i] >> 24) & 255);
            }
            bytes[bytes.length - 1] = 0;
            return BigInteger.fromByteArray(bytes, true);
        };
        prototype.GetLastWordsInternal = function(numWords32Bit) {
            var ret = [];
            for (var arrfillI = 0; arrfillI < numWords32Bit; arrfillI++) ret[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < (numWords32Bit < this.wordCount ? numWords32Bit : this.wordCount); arrfillI++) ret[0 + arrfillI] = this.data[0 + arrfillI];
            return ret;
        };
        prototype.CanFitInInt32 = function() {
            return this.wordCount == 0 || (this.wordCount == 1 && (this.data[0] >> 31) == 0);
        };
        prototype.ToInt32 = function() {
            return this.wordCount == 0 ? 0 : this.data[0];
        };
        prototype.Copy = function() {
            var mbi = new FastInteger.MutableNumber(0);
            if (this.wordCount > mbi.data.length) {
                mbi.data = [];
                for (var arrfillI = 0; arrfillI < this.wordCount; arrfillI++) mbi.data[arrfillI] = 0;
            }
            for (var arrfillI = 0; arrfillI < this.wordCount; arrfillI++) mbi.data[0 + arrfillI] = this.data[0 + arrfillI];
            mbi.wordCount = this.wordCount;
            return mbi;
        };
        prototype.Multiply = function(multiplicand) {
            if (multiplicand < 0) {
                throw new Error("Only positive multiplicands are supported");
            } else if (multiplicand != 0) {
                var carry = 0;
                if (this.wordCount == 0) {
                    if (this.data.length == 0) {
                        this.data = [0, 0, 0, 0];
                    }
                    this.data[0] = 0;
                    this.wordCount = 1;
                }
                var result0, result1, result2, result3;
                if (multiplicand < 65536) {
                    for (var i = 0; i < this.wordCount; ++i) {
                        var x0 = this.data[i];
                        var x1 = x0;
                        var y0 = multiplicand;
                        x0 &= 65535;
                        x1 = (x1 >>> 16);
                        var temp = (x0 * y0);
                        result1 = (temp >>> 16);
                        result0 = temp & 65535;
                        result2 = 0;
                        temp = (x1 * y0);
                        result2 = result2 + (temp >>> 16);
                        result1 += temp & 65535;
                        result2 = result2 + (result1 >>> 16);
                        result1 &= 65535;
                        result3 = (result2 >>> 16);
                        result2 &= 65535;
                        x0 = ((result0 | (result1 << 16))|0);
                        x1 = ((result2 | (result3 << 16))|0);
                        var x2 = (x0 + carry);
                        if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & 2147483647) < (x0 & 2147483647)) : ((x2 >> 31) == 0)) {
                            x1 = (x1 + 1);
                        }
                        this.data[i] = x2;
                        carry = x1;
                    }
                } else {
                    for (var i = 0; i < this.wordCount; ++i) {
                        var x0 = this.data[i];
                        var x1 = x0;
                        var y0 = multiplicand;
                        var y1 = y0;
                        x0 &= 65535;
                        y0 &= 65535;
                        x1 = (x1 >>> 16);
                        y1 = (y1 >>> 16);
                        var temp = (x0 * y0);
                        result1 = (temp >>> 16);
                        result0 = temp & 65535;
                        temp = (x0 * y1);
                        result2 = (temp >>> 16);
                        result1 += temp & 65535;
                        result2 = result2 + (result1 >>> 16);
                        result1 &= 65535;
                        temp = (x1 * y0);
                        result2 = result2 + (temp >>> 16);
                        result1 += temp & 65535;
                        result2 = result2 + (result1 >>> 16);
                        result1 &= 65535;
                        result3 = (result2 >>> 16);
                        result2 &= 65535;
                        temp = (x1 * y1);
                        result3 = result3 + (temp >>> 16);
                        result2 += temp & 65535;
                        result3 = result3 + (result2 >>> 16);
                        result2 &= 65535;
                        x0 = ((result0 | (result1 << 16))|0);
                        x1 = ((result2 | (result3 << 16))|0);
                        var x2 = (x0 + carry);
                        if (((x2 >> 31) == (x0 >> 31)) ? ((x2 & 2147483647) < (x0 & 2147483647)) : ((x2 >> 31) == 0)) {
                            x1 = (x1 + 1);
                        }
                        this.data[i] = x2;
                        carry = x1;
                    }
                }
                if (carry != 0) {
                    if (this.wordCount >= this.data.length) {
                        var newdata = [];
                        for (var arrfillI = 0; arrfillI < this.wordCount + 20; arrfillI++) newdata[arrfillI] = 0;
                        for (var arrfillI = 0; arrfillI < this.data.length; arrfillI++) newdata[0 + arrfillI] = this.data[0 + arrfillI];
                        this.data = newdata;
                    }
                    this.data[this.wordCount] = carry;
                    ++this.wordCount;
                }
                while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
                    --this.wordCount;
                }
            } else {
                if (this.data.length > 0) {
                    this.data[0] = 0;
                }
                this.wordCount = 0;
            }
            return this;
        };
        prototype.signum = function() {
            return this.wordCount == 0 ? 0 : 1;
        };
        prototype.isEvenNumber = function() {
            return this.wordCount == 0 || (this.data[0] & 1) == 0;
        };
        prototype.CompareToInt = function(val) {
            if (val < 0 || this.wordCount > 1) {
                return 1;
            }
            if (this.wordCount == 0) {
                return (val == 0) ? 0 : -1;
            } else if (this.data[0] == val) {
                return 0;
            } else {
                return (((this.data[0] >> 31) == (val >> 31)) ? ((this.data[0] & 2147483647) < (val & 2147483647)) : ((this.data[0] >> 31) == 0)) ? -1 : 1;
            }
        };
        prototype.SubtractInt = function(other) {
            if (other < 0) {
                throw new Error("Only positive values are supported");
            } else if (other != 0) {
                {
                    if (this.wordCount == 0) {
                        if (this.data.length == 0) {
                            this.data = [0, 0, 0, 0];
                        }
                        this.data[0] = 0;
                        this.wordCount = 1;
                    }
                    var borrow;
                    var u;
                    var a = this.data[0];
                    u = a - other;
                    borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other != 0)) ? 1 : 0;
                    this.data[0] = (u|0);
                    if (borrow != 0) {
                        for (var i = 1; i < this.wordCount; ++i) {
                            u = this.data[i] - borrow;
                            borrow = (((this.data[i] >> 31) == (u >> 31)) ? ((this.data[i] & 2147483647) < (u & 2147483647)) : ((this.data[i] >> 31) == 0)) ? 1 : 0;
                            this.data[i] = (u|0);
                        }
                    }
                    while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
                        --this.wordCount;
                    }
                }
            }
            return this;
        };
        prototype.Subtract = function(other) {
            {
                {
                    var neededSize = (this.wordCount > other.wordCount) ? this.wordCount : other.wordCount;
                    if (this.data.length < neededSize) {
                        var newdata = [];
                        for (var arrfillI = 0; arrfillI < neededSize + 20; arrfillI++) newdata[arrfillI] = 0;
                        for (var arrfillI = 0; arrfillI < this.data.length; arrfillI++) newdata[0 + arrfillI] = this.data[0 + arrfillI];
                        this.data = newdata;
                    }
                    neededSize = (this.wordCount < other.wordCount) ? this.wordCount : other.wordCount;
                    var u = 0;
                    var borrow = 0;
                    for (var i = 0; i < neededSize; ++i) {
                        var a = this.data[i];
                        u = (a - other.data[i]) - borrow;
                        borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
                        this.data[i] = (u|0);
                    }
                    if (borrow != 0) {
                        for (var i = neededSize; i < this.wordCount; ++i) {
                            var a = this.data[i];
                            u = (a - other.data[i]) - borrow;
                            borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
                            this.data[i] = (u|0);
                        }
                    }
                    while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
                        --this.wordCount;
                    }
                    return this;
                }
            }
        };
        prototype.compareTo = function(other) {
            if (this.wordCount != other.wordCount) {
                return (this.wordCount < other.wordCount) ? -1 : 1;
            }
            var valueN = this.wordCount;
            while ((valueN--) != 0) {
                var an = this.data[valueN];
                var bn = other.data[valueN];
                if (((an >> 31) == (bn >> 31)) ? ((an & 2147483647) < (bn & 2147483647)) : ((an >> 31) == 0)) {
                    return -1;
                } else if (an != bn) {
                    return 1;
                }
            }
            return 0;
        };
        prototype.Add = function(augend) {
            if (augend < 0) {
                throw new Error("Only positive augends are supported");
            } else if (augend != 0) {
                var carry = 0;
                if (this.wordCount == 0) {
                    if (this.data.length == 0) {
                        this.data = [0, 0, 0, 0];
                    }
                    this.data[0] = 0;
                    this.wordCount = 1;
                }
                for (var i = 0; i < this.wordCount; ++i) {
                    var u;
                    var a = this.data[i];
                    u = (a + augend) + carry;
                    carry = ((((u >> 31) == (a >> 31)) ? ((u & 2147483647) < (a & 2147483647)) : ((u >> 31) == 0)) || (u == a && augend != 0)) ? 1 : 0;
                    this.data[i] = u;
                    if (carry == 0) {
                        return this;
                    }
                    augend = 0;
                }
                if (carry != 0) {
                    if (this.wordCount >= this.data.length) {
                        var newdata = [];
                        for (var arrfillI = 0; arrfillI < this.wordCount + 20; arrfillI++) newdata[arrfillI] = 0;
                        for (var arrfillI = 0; arrfillI < this.data.length; arrfillI++) newdata[0 + arrfillI] = this.data[0 + arrfillI];
                        this.data = newdata;
                    }
                    this.data[this.wordCount] = carry;
                    ++this.wordCount;
                }
            }
            while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) {
                --this.wordCount;
            }
            return this;
        };
    })(FastInteger.MutableNumber,FastInteger.MutableNumber.prototype);

    prototype.smallValue = null;
    prototype.mnum = null;
    prototype.largeValue = null;
    prototype.integerMode = 0;
    constructor.valueInt32MinValue = BigInteger.valueOf(-2147483648);
    constructor.valueInt32MaxValue = BigInteger.valueOf(2147483647);
    constructor.valueNegativeInt32MinValue = (FastInteger.valueInt32MinValue).negate();
    constructor.Copy = function(value) {
        var fi = new FastInteger(value.smallValue);
        fi.integerMode = value.integerMode;
        fi.largeValue = value.largeValue;
        fi.mnum = (value.mnum == null || value.integerMode != 1) ? null : value.mnum.Copy();
        return fi;
    };
    constructor.FromBig = function(bigintVal) {
        if (bigintVal.canFitInInt()) {
            return new FastInteger(bigintVal.intValue());
        } else if (bigintVal.signum() > 0) {
            var fi = new FastInteger(0);
            fi.integerMode = 1;
            fi.mnum = FastInteger.MutableNumber.FromBigInteger(bigintVal);
            return fi;
        } else {
            var fi = new FastInteger(0);
            fi.integerMode = 2;
            fi.largeValue = bigintVal;
            return fi;
        }
    };

    prototype.AsInt32 = function() {
        switch(this.integerMode) {
            case 0:
                return this.smallValue;
            case 1:
                return this.mnum.ToInt32();
            case 2:
                return this.largeValue.intValue();
            default:
                throw new Error();
        }
    };

    prototype.compareTo = function(val) {
        switch((this.integerMode << 2) | val.integerMode) {
            case (0 << 2) | 0:
                {
                    var vsv = val.smallValue;
                    return (this.smallValue == vsv) ? 0 : (this.smallValue < vsv ? -1 : 1);
                }
            case (0 << 2) | 1:
                return -val.mnum.CompareToInt(this.smallValue);
            case (0 << 2) | 2:
                return this.AsBigInteger().compareTo(val.largeValue);
            case (1 << 2) | 0:
                return this.mnum.CompareToInt(val.smallValue);
            case (1 << 2) | 1:
                return this.mnum.compareTo(val.mnum);
            case (1 << 2) | 2:
                return this.AsBigInteger().compareTo(val.largeValue);
            case (2 << 2) | 0:
            case (2 << 2) | 1:
            case (2 << 2) | 2:
                return this.largeValue.compareTo(val.AsBigInteger());
            default:
                throw new Error();
        }
    };

    prototype.Abs = function() {
        return (this.signum() < 0) ? this.Negate() : this;
    };
    constructor.WordsToBigInteger = function(words) {
        var wordCount = words.length;
        if (wordCount == 1 && (words[0] >> 31) == 0) {
            return BigInteger.valueOf((words[0])|0);
        }
        var bytes = [];
        for (var arrfillI = 0; arrfillI < (wordCount * 4) + 1; arrfillI++) bytes[arrfillI] = 0;
        for (var i = 0; i < wordCount; ++i) {
            bytes[(i * 4) + 0] = (words[i] & 255);
            bytes[(i * 4) + 1] = ((words[i] >> 8) & 255);
            bytes[(i * 4) + 2] = ((words[i] >> 16) & 255);
            bytes[(i * 4) + 3] = ((words[i] >> 24) & 255);
        }
        bytes[bytes.length - 1] = 0;
        return BigInteger.fromByteArray(bytes, true);
    };
    constructor.GetLastWords = function(bigint, numWords32Bit) {
        return FastInteger.MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    };

    prototype.SetInt = function(val) {
        this.smallValue = val;
        this.integerMode = 0;
        return this;
    };

    prototype.RepeatedSubtract = function(divisor) {
        if (this.integerMode == 1) {
            var count = 0;
            if (divisor.integerMode == 1) {
                while (this.mnum.compareTo(divisor.mnum) >= 0) {
                    this.mnum.Subtract(divisor.mnum);
                    ++count;
                }
                return count;
            } else if (divisor.integerMode == 0 && divisor.smallValue >= 0) {
                if (this.mnum.CanFitInInt32()) {
                    var small = this.mnum.ToInt32();
                    count = ((small / divisor.smallValue)|0);
                    this.mnum.SetInt(small % divisor.smallValue);
                } else {
                    var dmnum = new FastInteger.MutableNumber(divisor.smallValue);
                    while (this.mnum.compareTo(dmnum) >= 0) {
                        this.mnum.Subtract(dmnum);
                        ++count;
                    }
                }
                return count;
            } else {
                var bigrem;
                var bigquo;
                {
                    var divrem = (this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
                    bigquo = divrem[0];
                    bigrem = divrem[1];
                }
                var smallquo = bigquo.intValue();
                this.integerMode = 2;
                this.largeValue = bigrem;
                return smallquo;
            }
        } else {
            var bigrem;
            var bigquo;
            {
                var divrem = (this.AsBigInteger()).divideAndRemainder(divisor.AsBigInteger());
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            var smallquo = bigquo.intValue();
            this.integerMode = 2;
            this.largeValue = bigrem;
            return smallquo;
        }
    };

    prototype.Multiply = function(val) {
        if (val == 0) {
            this.smallValue = 0;
            this.integerMode = 0;
        } else {
            switch(this.integerMode) {
                case 0:
                    var apos = this.smallValue > 0;
                    var bpos = val > 0;
                    if ((apos && ((!bpos && ((-2147483648 / this.smallValue)|0) > val) || (bpos && this.smallValue > ((2147483647 / val)|0)))) || (!apos && ((!bpos && this.smallValue != 0 && ((2147483647 / this.smallValue)|0) > val) || (bpos && this.smallValue < ((-2147483648 / val)|0))))) {

                        if (apos && bpos) {

                            this.integerMode = 1;
                            this.mnum = new FastInteger.MutableNumber(this.smallValue);
                            this.mnum.Multiply(val);
                        } else {

                            this.integerMode = 2;
                            this.largeValue = BigInteger.valueOf(this.smallValue);
                            this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
                        }
                    } else {
                        this.smallValue *= (val|0);
                    }
                    break;
                case 1:
                    if (val < 0) {
                        this.integerMode = 2;
                        this.largeValue = this.mnum.ToBigInteger();
                        this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
                    } else {
                        this.mnum.Multiply(val);
                    }
                    break;
                case 2:
                    this.largeValue = this.largeValue.multiply(BigInteger.valueOf(val));
                    break;
                default:
                    throw new Error();
            }
        }
        return this;
    };

    prototype.Negate = function() {
        switch(this.integerMode) {
            case 0:
                if (this.smallValue == -2147483648) {

                    this.integerMode = 1;
                    this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.valueNegativeInt32MinValue);
                } else {
                    this.smallValue = -this.smallValue;
                }
                break;
            case 1:
                this.integerMode = 2;
                this.largeValue = this.mnum.ToBigInteger();
                this.largeValue = (this.largeValue).negate();
                break;
            case 2:
                this.largeValue = (this.largeValue).negate();
                break;
            default:
                throw new Error();
        }
        return this;
    };

    prototype.Subtract = function(val) {
        var valValue;
        switch(this.integerMode) {
            case 0:
                if (val.integerMode == 0) {
                    var vsv = val.smallValue;
                    if ((vsv < 0 && 2147483647 + vsv < this.smallValue) || (vsv > 0 && -2147483648 + vsv > this.smallValue)) {

                        this.integerMode = 2;
                        this.largeValue = BigInteger.valueOf(this.smallValue);
                        this.largeValue = this.largeValue.subtract(BigInteger.valueOf(vsv));
                    } else {
                        this.smallValue -= vsv;
                    }
                } else {
                    this.integerMode = 2;
                    this.largeValue = BigInteger.valueOf(this.smallValue);
                    valValue = val.AsBigInteger();
                    this.largeValue = this.largeValue.subtract(valValue);
                }
                break;
            case 1:
                if (val.integerMode == 1) {

                    this.mnum.Subtract(val.mnum);
                } else if (val.integerMode == 0 && val.smallValue >= 0) {
                    this.mnum.SubtractInt(val.smallValue);
                } else {
                    this.integerMode = 2;
                    this.largeValue = this.mnum.ToBigInteger();
                    valValue = val.AsBigInteger();
                    this.largeValue = this.largeValue.subtract(valValue);
                }
                break;
            case 2:
                valValue = val.AsBigInteger();
                this.largeValue = this.largeValue.subtract(valValue);
                break;
            default:
                throw new Error();
        }
        return this;
    };

    prototype.SubtractInt = function(val) {
        if (val == -2147483648) {
            return this.AddBig(FastInteger.valueNegativeInt32MinValue);
        } else if (this.integerMode == 0) {
            if ((val < 0 && 2147483647 + val < this.smallValue) || (val > 0 && -2147483648 + val > this.smallValue)) {

                this.integerMode = 2;
                this.largeValue = BigInteger.valueOf(this.smallValue);
                this.largeValue = this.largeValue.subtract(BigInteger.valueOf(val));
            } else {
                this.smallValue -= val;
            }
            return this;
        } else {
            return this.AddInt(-val);
        }
    };

    prototype.AddBig = function(bigintVal) {
        switch(this.integerMode) {
            case 0:
                {
                    if (bigintVal.canFitInInt()) {
                        return this.AddInt(bigintVal.intValue());
                    }
                    return this.Add(FastInteger.FromBig(bigintVal));
                }
            case 1:
                this.integerMode = 2;
                this.largeValue = this.mnum.ToBigInteger();
                this.largeValue = this.largeValue.add(bigintVal);
                break;
            case 2:
                this.largeValue = this.largeValue.add(bigintVal);
                break;
            default:
                throw new Error();
        }
        return this;
    };

    prototype.SubtractBig = function(bigintVal) {
        if (this.integerMode == 2) {
            this.largeValue = this.largeValue.subtract(bigintVal);
            return this;
        } else {
            var sign = bigintVal.signum();
            if (sign == 0) {
                return this;
            }

            if (sign < 0 && bigintVal.compareTo(FastInteger.valueInt32MinValue) > 0) {
                return this.AddInt(-(bigintVal.intValue()));
            }
            if (sign > 0 && bigintVal.compareTo(FastInteger.valueInt32MaxValue) <= 0) {
                return this.SubtractInt(bigintVal.intValue());
            }
            bigintVal = bigintVal.negate();
            return this.AddBig(bigintVal);
        }
    };

    prototype.Add = function(val) {
        var valValue;
        switch(this.integerMode) {
            case 0:
                if (val.integerMode == 0) {
                    if ((this.smallValue < 0 && ((val.smallValue)|0) < -2147483648 - this.smallValue) || (this.smallValue > 0 && ((val.smallValue)|0) > 2147483647 - this.smallValue)) {

                        if (val.smallValue >= 0) {
                            this.integerMode = 1;
                            this.mnum = new FastInteger.MutableNumber(this.smallValue);
                            this.mnum.Add(val.smallValue);
                        } else {
                            this.integerMode = 2;
                            this.largeValue = BigInteger.valueOf(this.smallValue);
                            this.largeValue = this.largeValue.add(BigInteger.valueOf(val.smallValue));
                        }
                    } else {
                        this.smallValue += val.smallValue;
                    }
                } else {
                    this.integerMode = 2;
                    this.largeValue = BigInteger.valueOf(this.smallValue);
                    valValue = val.AsBigInteger();
                    this.largeValue = this.largeValue.add(valValue);
                }
                break;
            case 1:
                if (val.integerMode == 0 && val.smallValue >= 0) {
                    this.mnum.Add(val.smallValue);
                } else {
                    this.integerMode = 2;
                    this.largeValue = this.mnum.ToBigInteger();
                    valValue = val.AsBigInteger();
                    this.largeValue = this.largeValue.add(valValue);
                }
                break;
            case 2:
                valValue = val.AsBigInteger();
                this.largeValue = this.largeValue.add(valValue);
                break;
            default:
                throw new Error();
        }
        return this;
    };

    prototype.Remainder = function(divisor) {

        if (divisor != 0) {
            switch(this.integerMode) {
                case 0:
                    this.smallValue %= divisor;
                    break;
                case 1:
                    this.largeValue = this.mnum.ToBigInteger();
                    this.largeValue = this.largeValue.remainder(BigInteger.valueOf(divisor));
                    this.smallValue = this.largeValue.intValue();
                    this.integerMode = 0;
                    break;
                case 2:
                    this.largeValue = this.largeValue.remainder(BigInteger.valueOf(divisor));
                    this.smallValue = this.largeValue.intValue();
                    this.integerMode = 0;
                    break;
                default:
                    throw new Error();
            }
        } else {
            throw new Error();
        }
        return this;
    };

    prototype.Increment = function() {
        if (this.integerMode == 0) {
            if (this.smallValue != 2147483647) {
                ++this.smallValue;
            } else {
                this.integerMode = 1;
                this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.valueNegativeInt32MinValue);
            }
            return this;
        } else {
            return this.AddInt(1);
        }
    };

    prototype.Decrement = function() {
        if (this.integerMode == 0) {
            if (this.smallValue != -2147483648) {
                --this.smallValue;
            } else {
                this.integerMode = 1;
                this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.valueInt32MinValue);
                this.mnum.SubtractInt(1);
            }
            return this;
        } else {
            return this.SubtractInt(1);
        }
    };

    prototype.Divide = function(divisor) {
        if (divisor != 0) {
            switch(this.integerMode) {
                case 0:
                    if (divisor == -1 && this.smallValue == -2147483648) {

                        this.integerMode = 1;
                        this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.valueNegativeInt32MinValue);
                    } else {
                        this.smallValue = ((this.smallValue / divisor)|0);
                    }
                    break;
                case 1:
                    this.integerMode = 2;
                    this.largeValue = this.mnum.ToBigInteger();
                    this.largeValue = this.largeValue.divide(BigInteger.valueOf(divisor));
                    if (this.largeValue.signum() == 0) {
                        this.integerMode = 0;
                        this.smallValue = 0;
                    }
                    break;
                case 2:
                    this.largeValue = this.largeValue.divide(BigInteger.valueOf(divisor));
                    if (this.largeValue.signum() == 0) {
                        this.integerMode = 0;
                        this.smallValue = 0;
                    }
                    break;
                default:
                    throw new Error();
            }
        } else {
            throw new Error();
        }
        return this;
    };

    prototype.isEvenNumber = function() {
        switch(this.integerMode) {
            case 0:
                return (this.smallValue & 1) == 0;
            case 1:
                return this.mnum.isEvenNumber();
            case 2:
                return this.largeValue.testBit(0) == false;
            default:
                throw new Error();
        }
    };

    prototype.AddInt = function(val) {
        var valValue;
        switch(this.integerMode) {
            case 0:
                if ((this.smallValue < 0 && (val|0) < -2147483648 - this.smallValue) || (this.smallValue > 0 && (val|0) > 2147483647 - this.smallValue)) {

                    if (val >= 0) {
                        this.integerMode = 1;
                        this.mnum = new FastInteger.MutableNumber(this.smallValue);
                        this.mnum.Add(val);
                    } else {
                        this.integerMode = 2;
                        this.largeValue = BigInteger.valueOf(this.smallValue);
                        this.largeValue = this.largeValue.add(BigInteger.valueOf(val));
                    }
                } else {
                    this.smallValue = this.smallValue + (val);
                }
                break;
            case 1:
                if (val >= 0) {
                    this.mnum.Add(val);
                } else {
                    this.integerMode = 2;
                    this.largeValue = this.mnum.ToBigInteger();
                    valValue = BigInteger.valueOf(val);
                    this.largeValue = this.largeValue.add(valValue);
                }
                break;
            case 2:
                valValue = BigInteger.valueOf(val);
                this.largeValue = this.largeValue.add(valValue);
                break;
            default:
                throw new Error();
        }
        return this;
    };

    prototype.CanFitInInt32 = function() {
        switch(this.integerMode) {
            case 0:
                return true;
            case 1:
                return this.mnum.CanFitInInt32();
            case 2:
                {
                    return this.largeValue.canFitInInt();
                }
            default:
                throw new Error();
        }
    };

    prototype.toString = function() {
        switch(this.integerMode) {
            case 0:
                return (((this.smallValue)|0) + "");
            case 1:
                return this.mnum.ToBigInteger().toString();
            case 2:
                return this.largeValue.toString();
            default:
                return "";
        }
    };

    prototype.signum = function() {
        switch(this.integerMode) {
            case 0:
                return ((this.smallValue == 0) ? 0 : ((this.smallValue < 0) ? -1 : 1));
            case 1:
                return this.mnum.signum();
            case 2:
                return this.largeValue.signum();
            default:
                return 0;
        }
    };

    prototype.isValueZero = function() {
        switch(this.integerMode) {
            case 0:
                return this.smallValue == 0;
            case 1:
                return this.mnum.signum() == 0;
            case 2:
                return this.largeValue.signum() == 0;
            default:
                return false;
        }
    };

    prototype.CompareToInt = function(val) {
        switch(this.integerMode) {
            case 0:
                return (val == this.smallValue) ? 0 : (this.smallValue < val ? -1 : 1);
            case 1:
                return this.mnum.ToBigInteger().compareTo(BigInteger.valueOf(val));
            case 2:
                return this.largeValue.compareTo(BigInteger.valueOf(val));
            default:
                return 0;
        }
    };

    prototype.AsBigInteger = function() {
        switch(this.integerMode) {
            case 0:
                return BigInteger.valueOf(this.smallValue);
            case 1:
                return this.mnum.ToBigInteger();
            case 2:
                return this.largeValue;
            default:
                throw new Error();
        }
    };
})(FastInteger,FastInteger.prototype);

var BitShiftAccumulator =

function(bigint, lastDiscarded, olderDiscarded) {

    if (bigint.signum() < 0) {
        throw new Error("bigint is negative");
    }
    if (bigint.canFitInInt()) {
        this.isSmall = true;
        this.shiftedSmall = bigint.intValue();
    } else {
        this.shiftedBigInt = bigint;
    }
    this.discardedBitCount = new FastInteger(0);
    this.bitsAfterLeftmost = (olderDiscarded != 0) ? 1 : 0;
    this.bitLeftmost = (lastDiscarded != 0) ? 1 : 0;
};
(function(constructor,prototype){
    constructor.SmallBitLength = 32;
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
    prototype.GetDigitLength = function() {
        if (this.knownBitLength == null) {
            this.knownBitLength = this.CalcKnownBitLength();
        }
        return FastInteger.Copy(this.knownBitLength);
    };
    prototype.ShiftToDigits = function(bits) {
        if (bits.signum() < 0) {
            throw new Error("bits is negative");
        }
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
        if (this.isSmall) {
            return BigInteger.valueOf(this.shiftedSmall);
        } else {
            return this.shiftedBigInt;
        }
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
        if (smallNumber < 0) {
            throw new Error("longInt is negative");
        }
        var bsa = new BitShiftAccumulator(BigInteger.ZERO, 0, 0);
        bsa.shiftedSmall = smallNumber;
        bsa.discardedBitCount = new FastInteger(0);
        bsa.isSmall = true;
        return bsa;
    };

    prototype.ShiftRight = function(fastint) {
        if (fastint.signum() <= 0) {
            return;
        }
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
                if (this.isSmall ? this.shiftedSmall == 0 : this.shiftedBigInt.signum() == 0) {
                    break;
                }
            }
        }
    };
    prototype.ShiftRightBig = function(bits) {
        if (bits <= 0) {
            return;
        }
        if (this.shiftedBigInt.signum() == 0) {
            this.discardedBitCount.AddInt(bits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.isSmall = true;
            this.shiftedSmall = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        if (this.knownBitLength == null) {
            this.knownBitLength = this.GetDigitLength();
        }
        this.discardedBitCount.AddInt(bits);
        var cmp = this.knownBitLength.CompareToInt(bits);
        if (cmp < 0) {

            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitsAfterLeftmost |= this.shiftedBigInt.signum() == 0 ? 0 : 1;
            this.bitLeftmost = 0;
            this.isSmall = true;
            this.shiftedSmall = 0;
            this.knownBitLength = new FastInteger(1);
        } else {

            var bs = bits;
            this.knownBitLength.SubtractInt(bits);
            if (bs == 1) {
                var odd = !this.shiftedBigInt.testBit(0) == false;
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(1);
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = odd ? 1 : 0;
            } else {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                var lowestSetBit = this.shiftedBigInt.getLowestSetBit();
                if (lowestSetBit < bs - 1) {

                    this.bitsAfterLeftmost |= 1;
                    this.bitLeftmost = this.shiftedBigInt.testBit(bs - 1) ? 1 : 0;
                } else if (lowestSetBit > bs - 1) {

                    this.bitLeftmost = 0;
                } else {

                    this.bitLeftmost = 1;
                }
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(bs);
            }
            if (this.knownBitLength.CompareToInt(BitShiftAccumulator.SmallBitLength) < 0) {

                this.isSmall = true;
                this.shiftedSmall = this.shiftedBigInt.intValue();
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };
    prototype.CalcKnownBitLength = function() {
        if (this.isSmall) {
            var kb = BitShiftAccumulator.SmallBitLength;
            for (var i = BitShiftAccumulator.SmallBitLength - 1; i >= 0; --i) {
                if ((this.shiftedSmall & (1 << i)) != 0) {
                    break;
                } else {
                    --kb;
                }
            }

            if (kb == 0) {
                ++kb;
            }

            return new FastInteger(kb);
        } else {
            if (this.shiftedBigInt.signum() == 0) {
                return new FastInteger(1);
            }
            return new FastInteger(this.shiftedBigInt.bitLength());
        }
    };
    prototype.ShiftBigToBits = function(bits) {

        if (this.knownBitLength != null) {
            if (this.knownBitLength.CompareToInt(bits) <= 0) {
                return;
            }
        }
        if (this.knownBitLength == null) {
            this.knownBitLength = this.GetDigitLength();
        }
        if (this.knownBitLength.CompareToInt(bits) <= 0) {
            return;
        }

        if (this.knownBitLength.CompareToInt(bits) > 0) {
            var bs = 0;
            if (this.knownBitLength.CanFitInInt32()) {
                bs = this.knownBitLength.AsInt32();
                bs -= bits;
            } else {
                var bitShift = FastInteger.Copy(this.knownBitLength).SubtractInt(bits);
                if (!bitShift.CanFitInInt32()) {
                    this.ShiftRight(bitShift);
                    return;
                } else {
                    bs = bitShift.AsInt32();
                }
            }
            this.knownBitLength.SetInt(bits);
            this.discardedBitCount.AddInt(bs);
            if (bs == 1) {
                var odd = !this.shiftedBigInt.testBit(0) == false;
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(1);
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = odd ? 1 : 0;
            } else {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                var lowestSetBit = this.shiftedBigInt.getLowestSetBit();
                if (lowestSetBit < bs - 1) {

                    this.bitsAfterLeftmost |= 1;
                    this.bitLeftmost = this.shiftedBigInt.testBit(bs - 1) ? 1 : 0;
                } else if (lowestSetBit > bs - 1) {

                    this.bitLeftmost = 0;
                } else {

                    this.bitLeftmost = 1;
                }
                this.shiftedBigInt = this.shiftedBigInt.shiftRight(bs);
            }
            if (bits < BitShiftAccumulator.SmallBitLength) {

                this.isSmall = true;
                this.shiftedSmall = this.shiftedBigInt.intValue();
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };

    prototype.ShiftRightInt = function(bits) {
        if (this.isSmall) {
            this.ShiftRightSmall(bits);
        } else {
            this.ShiftRightBig(bits);
        }
    };
    prototype.ShiftRightSmall = function(bits) {
        if (bits <= 0) {
            return;
        }
        if (this.shiftedSmall == 0) {
            this.discardedBitCount.AddInt(bits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var kb = BitShiftAccumulator.SmallBitLength;
        for (var i = BitShiftAccumulator.SmallBitLength - 1; i >= 0; --i) {
            if ((this.shiftedSmall & (1 << i)) != 0) {
                break;
            } else {
                --kb;
            }
        }
        var shift = ((kb < bits ? kb : bits)|0);
        var shiftingMoreBits = bits > kb;
        kb -= shift;
        this.knownBitLength = new FastInteger(kb);
        this.discardedBitCount.AddInt(bits);
        this.bitsAfterLeftmost |= this.bitLeftmost;

        this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall << (BitShiftAccumulator.SmallBitLength - shift + 1)) != 0) ? 1 : 0;

        this.bitLeftmost = ((this.shiftedSmall >> (shift - 1)) & 1);
        this.shiftedSmall >>= shift;
        if (shiftingMoreBits) {

            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    };

    prototype.ShiftToDigitsInt = function(bits) {
        if (bits < 0) {
            throw new Error("bits is negative");
        }
        if (this.isSmall) {
            this.ShiftSmallToBits(bits);
        } else {
            this.ShiftBigToBits(bits);
        }
    };
    prototype.ShiftSmallToBits = function(bits) {
        if (this.knownBitLength != null) {
            if (this.knownBitLength.CompareToInt(bits) <= 0) {
                return;
            }
        }
        if (this.knownBitLength == null) {
            this.knownBitLength = this.GetDigitLength();
        }
        if (this.knownBitLength.CompareToInt(bits) <= 0) {
            return;
        }
        var kbl = this.knownBitLength.AsInt32();

        if (kbl > bits) {
            var bitShift = kbl - (bits|0);
            var shift = (bitShift|0);
            this.knownBitLength = new FastInteger(bits);
            this.discardedBitCount.AddInt(bitShift);
            this.bitsAfterLeftmost |= this.bitLeftmost;

            this.bitsAfterLeftmost |= (shift > 1 && (this.shiftedSmall << (BitShiftAccumulator.SmallBitLength - shift + 1)) != 0) ? 1 : 0;

            this.bitLeftmost = ((this.shiftedSmall >> (shift - 1)) & 1);
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            this.shiftedSmall >>= shift;
        } else {
            this.knownBitLength = new FastInteger(kbl);
        }
    };
})(BitShiftAccumulator,BitShiftAccumulator.prototype);

var DigitShiftAccumulator =

function(bigint, lastDiscarded, olderDiscarded) {

    if (bigint.canFitInInt()) {
        this.shiftedSmall = bigint.intValue();
        if (this.shiftedSmall < 0) {
            throw new Error("bigint is negative");
        }
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
    prototype.GetDigitLength = function() {
        if (this.knownBitLength == null) {
            this.knownBitLength = this.CalcKnownDigitLength();
        }
        return FastInteger.Copy(this.knownBitLength);
    };
    prototype.shiftedSmall = null;
    prototype.isSmall = null;
    prototype.discardedBitCount = null;
    prototype.getDiscardedDigitCount = function() {
        if (this.discardedBitCount == null) {
            this.discardedBitCount = new FastInteger(0);
        }
        return this.discardedBitCount;
    };
    constructor.valueTen = BigInteger.TEN;
    prototype.getShiftedInt = function() {
        if (this.isSmall) {
            return BigInteger.valueOf(this.shiftedSmall);
        } else {
            return this.shiftedBigInt;
        }
    };
    constructor.FastParseLong = function(str, offset, length) {

        if (length > 9) {
            throw new Error("length not less or equal to 9 (" + (length) + ")");
        }
        var ret = 0;
        for (var i = 0; i < length; ++i) {
            var digit = ((str.charCodeAt(offset + i)-48)|0);
            ret *= 10;
            ret = ret + (digit);
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
        if (fastint == null) {
            throw new Error("fastint");
        }
        if (fastint.signum() <= 0) {
            return;
        }
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
                if (this.isSmall ? this.shiftedSmall == 0 : this.shiftedBigInt.signum() == 0) {
                    break;
                }
            }
        }
    };
    prototype.ShiftRightBig = function(digits) {
        if (digits <= 0) {
            return;
        }
        if (this.shiftedBigInt.signum() == 0) {
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.AddInt(digits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }

        if (digits == 1) {
            var bigrem;
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(BigInteger.TEN);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = bigrem.intValue();
            this.shiftedBigInt = bigquo;
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.AddInt(digits);
            if (this.knownBitLength != null) {
                if (bigquo.signum() == 0) {
                    this.knownBitLength.SetInt(0);
                } else {
                    this.knownBitLength.Decrement();
                }
            }
            return;
        }
        var startCount = (4 < digits - 1 ? 4 : digits - 1);
        if (startCount > 0) {
            var bigrem;
            var radixPower = DecimalUtility.FindPowerOfTen(startCount);
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(radixPower);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            if (bigrem.signum() != 0) {
                this.bitsAfterLeftmost |= 1;
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.shiftedBigInt = bigquo;
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.AddInt(startCount);
            digits -= startCount;
            if (this.shiftedBigInt.signum() == 0) {

                this.isSmall = true;
                this.shiftedSmall = 0;
                this.knownBitLength = new FastInteger(1);
                this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
                this.bitLeftmost = 0;
                return;
            }
        }
        if (digits == 1) {
            var bigrem;
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(DigitShiftAccumulator.valueTen);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = bigrem.intValue();
            this.shiftedBigInt = bigquo;
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.Increment();
            if (this.knownBitLength == null) {
                this.knownBitLength = this.GetDigitLength();
            } else {
                this.knownBitLength.Decrement();
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            return;
        }
        if (this.knownBitLength == null) {
            this.knownBitLength = this.GetDigitLength();
        }
        if (new FastInteger(digits).Decrement().compareTo(this.knownBitLength) >= 0) {

            this.bitsAfterLeftmost |= this.shiftedBigInt.signum() == 0 ? 0 : 1;
            this.isSmall = true;
            this.shiftedSmall = 0;
            this.knownBitLength = new FastInteger(1);
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.AddInt(digits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            return;
        }
        if (this.shiftedBigInt.canFitInInt()) {
            this.isSmall = true;
            this.shiftedSmall = this.shiftedBigInt.intValue();
            this.ShiftRightSmall(digits);
            return;
        }
        var str = this.shiftedBigInt.toString();

        var digitLength = str.length;
        var bitDiff = 0;
        if (digits > digitLength) {
            bitDiff = digits - digitLength;
        }
        if (this.discardedBitCount == null) {
            this.discardedBitCount = new FastInteger(0);
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
                this.shiftedBigInt = BigInteger.fromSubstring(str, 0, newLength);
            }
        }
        for (var i = str.length - 1; i >= 0; --i) {
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = ((str.charCodeAt(i)-48)|0);
            --digitShift;
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

        if (this.knownBitLength != null) {
            if (this.knownBitLength.CompareToInt(digits) <= 0) {
                return;
            }
        }
        var str;
        if (this.knownBitLength == null) {
            this.knownBitLength = this.GetDigitLength();
        }
        if (this.knownBitLength.CompareToInt(digits) <= 0) {
            return;
        }
        var digitDiff = FastInteger.Copy(this.knownBitLength).SubtractInt(digits);
        if (digitDiff.CompareToInt(1) == 0) {
            var bigrem;
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(DigitShiftAccumulator.valueTen);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = bigrem.intValue();
            this.shiftedBigInt = bigquo;
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.Add(digitDiff);
            this.knownBitLength.Subtract(digitDiff);
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            return;
        } else if (digitDiff.CompareToInt(9) <= 0) {
            var bigrem;
            var diffInt = digitDiff.AsInt32();
            var radixPower = DecimalUtility.FindPowerOfTen(diffInt);
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(radixPower);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            var rem = bigrem.intValue();
            this.bitsAfterLeftmost |= this.bitLeftmost;
            for (var i = 0; i < diffInt; ++i) {
                if (i == diffInt - 1) {
                    this.bitLeftmost = rem % 10;
                } else {
                    this.bitsAfterLeftmost |= rem % 10;
                    rem = ((rem / 10)|0);
                }
            }
            this.shiftedBigInt = bigquo;
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.Add(digitDiff);
            this.knownBitLength.Subtract(digitDiff);
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            return;
        } else if (digitDiff.CompareToInt(2147483647) <= 0) {
            var bigrem;
            var radixPower = DecimalUtility.FindPowerOfTen(digitDiff.AsInt32() - 1);
            var bigquo;
            {
                var divrem = (this.shiftedBigInt).divideAndRemainder(radixPower);
                bigquo = divrem[0];
                bigrem = divrem[1];
            }
            this.bitsAfterLeftmost |= this.bitLeftmost;
            if (bigrem.signum() != 0) {
                this.bitsAfterLeftmost |= 1;
            }
            {
                var bigquo2;
                {
                    var divrem = (bigquo).divideAndRemainder(DigitShiftAccumulator.valueTen);
                    bigquo2 = divrem[0];
                    bigrem = divrem[1];
                }
                this.bitLeftmost = bigrem.intValue();
                this.shiftedBigInt = bigquo2;
            }
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.Add(digitDiff);
            this.knownBitLength.Subtract(digitDiff);
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
            return;
        }
        str = this.shiftedBigInt.toString();

        var digitLength = str.length;
        this.knownBitLength = new FastInteger(digitLength);

        if (digitLength > digits) {
            var digitShift = digitLength - digits;
            this.knownBitLength.SubtractInt(digitShift);
            var newLength = ((digitLength - digitShift)|0);

            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            if (digitShift <= 2147483647) {
                this.discardedBitCount.AddInt(digitShift|0);
            } else {
                this.discardedBitCount.AddBig(BigInteger.valueOf(digitShift));
            }
            for (var i = str.length - 1; i >= 0; --i) {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = ((str.charCodeAt(i)-48)|0);
                --digitShift;
                if (digitShift <= 0) {
                    break;
                }
            }
            if (newLength <= 9) {
                this.isSmall = true;
                this.shiftedSmall = DigitShiftAccumulator.FastParseLong(str, 0, newLength);
            } else {
                this.shiftedBigInt = BigInteger.fromSubstring(str, 0, newLength);
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };

    prototype.ShiftRightInt = function(digits) {
        if (this.isSmall) {
            this.ShiftRightSmall(digits);
        } else {
            this.ShiftRightBig(digits);
        }
    };
    prototype.ShiftRightSmall = function(digits) {
        if (digits <= 0) {
            return;
        }
        if (this.shiftedSmall == 0) {
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(0);
            }
            this.discardedBitCount.AddInt(digits);
            this.bitsAfterLeftmost |= this.bitLeftmost;
            this.bitLeftmost = 0;
            this.knownBitLength = new FastInteger(1);
            return;
        }
        var kb = 0;
        var tmp = this.shiftedSmall;
        while (tmp > 0) {
            ++kb;
            tmp = ((tmp / 10)|0);
        }

        if (kb == 0) {
            ++kb;
        }
        this.knownBitLength = new FastInteger(kb);
        if (this.discardedBitCount == null) {
            this.discardedBitCount = new FastInteger(0);
        }
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
                --digits;
                this.shiftedSmall = ((this.shiftedSmall / 10)|0);
                this.knownBitLength.Decrement();
            }
        }
        this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
    };

    prototype.ShiftToDigits = function(bits) {
        if (bits.CanFitInInt32()) {
            var intval = bits.AsInt32();
            if (intval < 0) {
                throw new Error("bits is negative");
            }
            this.ShiftToDigitsInt(intval);
        } else {
            if (bits.signum() < 0) {
                throw new Error("bits is negative");
            }
            this.knownBitLength = this.CalcKnownDigitLength();
            var bigintDiff = this.knownBitLength.AsBigInteger();
            var bitsBig = bits.AsBigInteger();
            bigintDiff = bigintDiff.subtract(bitsBig);
            if (bigintDiff.signum() > 0) {

                this.ShiftRight(FastInteger.FromBig(bigintDiff));
            }
        }
    };

    prototype.ShiftToDigitsInt = function(digits) {
        if (this.isSmall) {
            this.ShiftToBitsSmall(digits);
        } else {
            this.ShiftToBitsBig(digits);
        }
    };
    prototype.CalcKnownDigitLength = function() {
        if (this.isSmall) {
            var kb = 0;
            var v2 = this.shiftedSmall;
            if (v2 >= 1000000000) {
                kb = 10;
            } else if (v2 >= 100000000) {
                kb = 9;
            } else if (v2 >= 10000000) {
                kb = 8;
            } else if (v2 >= 1000000) {
                kb = 7;
            } else if (v2 >= 100000) {
                kb = 6;
            } else if (v2 >= 10000) {
                kb = 5;
            } else if (v2 >= 1000) {
                kb = 4;
            } else if (v2 >= 100) {
                kb = 3;
            } else if (v2 >= 10) {
                kb = 2;
            } else {
                kb = 1;
            }
            return new FastInteger(kb);
        } else {
            return new FastInteger(this.shiftedBigInt.getDigitCount());
        }
    };
    prototype.ShiftToBitsSmall = function(digits) {
        var kb = 0;
        var v2 = this.shiftedSmall;
        if (v2 >= 1000000000) {
            kb = 10;
        } else if (v2 >= 100000000) {
            kb = 9;
        } else if (v2 >= 10000000) {
            kb = 8;
        } else if (v2 >= 1000000) {
            kb = 7;
        } else if (v2 >= 100000) {
            kb = 6;
        } else if (v2 >= 10000) {
            kb = 5;
        } else if (v2 >= 1000) {
            kb = 4;
        } else if (v2 >= 100) {
            kb = 3;
        } else if (v2 >= 10) {
            kb = 2;
        } else {
            kb = 1;
        }
        this.knownBitLength = new FastInteger(kb);
        if (kb > digits) {
            var digitShift = ((kb - digits)|0);
            var newLength = ((kb - digitShift)|0);
            this.knownBitLength = new FastInteger(1 > newLength ? 1 : newLength);
            if (this.discardedBitCount == null) {
                this.discardedBitCount = new FastInteger(digitShift);
            } else {
                this.discardedBitCount.AddInt(digitShift);
            }
            for (var i = 0; i < digitShift; ++i) {
                var digit = ((this.shiftedSmall % 10)|0);
                this.shiftedSmall = ((this.shiftedSmall / 10)|0);
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = digit;
            }
            this.bitsAfterLeftmost = (this.bitsAfterLeftmost != 0) ? 1 : 0;
        }
    };
})(DigitShiftAccumulator,DigitShiftAccumulator.prototype);

var DecimalUtility = function() {

};
(function(constructor,prototype){
    constructor.valueBigIntPowersOfTen = [BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000), BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000), BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1410065408, 2)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1215752192, 23)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-727379968, 232)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1316134912, 2328)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(276447232, 23283)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1530494976, 232830)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1874919424, 2328306)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1569325056, 23283064)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1486618624, 232830643))];
    constructor.valueBigIntPowersOfFive = [BigInteger.ONE, BigInteger.valueOf(5), BigInteger.valueOf(25), BigInteger.valueOf(125), BigInteger.valueOf(625), BigInteger.valueOf(3125), BigInteger.valueOf(15625), BigInteger.valueOf(78125), BigInteger.valueOf(390625), BigInteger.valueOf(1953125), BigInteger.valueOf(9765625), BigInteger.valueOf(48828125), BigInteger.valueOf(244140625), BigInteger.valueOf(1220703125), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1808548329, 1)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(452807053, 7)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-2030932031, 35)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1564725563, 177)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(766306777, 888)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-463433411, 4440)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(1299066613, 111022)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-2094601527, 555111)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-1883073043, 2775557)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-825430623, 13877787)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(167814181, 69388939)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(839070905, 346944695)), BigInteger.valueOf(JSInteropFactory.createLongFromInts(-99612771, 1734723475))];
    constructor.ShiftLeftOne = function(arr) {
        {
            var carry = 0;
            for (var i = 0; i < arr.length; ++i) {
                var item = arr[i];
                arr[i] = ((arr[i] << 1)|0) | (carry|0);
                carry = ((item >> 31) != 0) ? 1 : 0;
            }
            return carry;
        }
    };
    constructor.CountTrailingZeros = function(numberValue) {
        if (numberValue == 0) {
            return 32;
        }
        var i = 0;
        {
            if ((numberValue << 16) == 0) {
                numberValue >>= 16;
                i = i + (16);
            }
            if ((numberValue << 24) == 0) {
                numberValue >>= 8;
                i = i + (8);
            }
            if ((numberValue << 28) == 0) {
                numberValue >>= 4;
                i = i + (4);
            }
            if ((numberValue << 30) == 0) {
                numberValue >>= 2;
                i = i + (2);
            }
            if ((numberValue << 31) == 0) {
                ++i;
            }
        }
        return i;
    };
    constructor.BitPrecisionInt = function(numberValue) {
        if (numberValue == 0) {
            return 0;
        }
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
            if ((numberValue >> 31) == 0) {
                --i;
            }
        }
        return i;
    };
    constructor.ShiftAwayTrailingZerosTwoElements = function(arr) {
        var a0 = arr[0];
        var a1 = arr[1];
        var tz = DecimalUtility.CountTrailingZeros(a0);
        if (tz == 0) {
            return 0;
        }
        {
            if (tz < 32) {
                var carry = a1 << (32 - tz);
                arr[0] = (((a0 >> tz) & (2147483647 >> (tz - 1)))|0) | (carry|0);
                arr[1] = (a1 >> tz) & (2147483647 >> (tz - 1));
                return tz;
            } else {
                tz = DecimalUtility.CountTrailingZeros(a1);
                if (tz == 32) {
                    arr[0] = 0;
                } else if (tz > 0) {
                    arr[0] = (a1 >> tz) & (2147483647 >> (tz - 1));
                } else {
                    arr[0] = a1;
                }
                arr[1] = 0;
                return 32 + tz;
            }
        }
    };
    constructor.HasBitSet = function(arr, bit) {
        return (bit >> 5) < arr.length && (arr[bit >> 5] & (1 << (bit & 31))) != 0;
    };
    constructor.PowerCache = function DecimalUtility$PowerCache() {

        this.outputs = [];
        this.outputs.length = DecimalUtility.PowerCache.MaxSize;
        this.inputs = [];
        this.inputs.length = DecimalUtility.PowerCache.MaxSize;
        this.inputsInts = [];
        for (var arrfillI = 0; arrfillI < DecimalUtility.PowerCache.MaxSize; arrfillI++) this.inputsInts[arrfillI] = 0;
    };
    (function(constructor,prototype){
        constructor.MaxSize = 64;
        prototype.outputs = null;
        prototype.inputs = null;
        prototype.inputsInts = null;
        prototype.size = null;
        prototype.FindCachedPowerOrSmaller = function(bi) {
            var ret = null;
            var minValue = null;
            {
                for (var i = 0; i < this.size; ++i) {
                    if (this.inputs[i].compareTo(bi) <= 0 && (minValue == null || this.inputs[i].compareTo(minValue) >= 0)) {

                        ret = [this.inputs[i], this.outputs[i]];
                        minValue = this.inputs[i];
                    }
                }
            }
            return ret;
        };

        prototype.GetCachedPower = function(bi) {
            {
                for (var i = 0; i < this.size; ++i) {
                    if (bi.equals(this.inputs[i])) {
                        if (i != 0) {
                            var tmp;

                            tmp = this.inputs[i];
                            this.inputs[i] = this.inputs[0];
                            this.inputs[0] = tmp;
                            var tmpi = this.inputsInts[i];
                            this.inputsInts[i] = this.inputsInts[0];
                            this.inputsInts[0] = tmpi;
                            tmp = this.outputs[i];
                            this.outputs[i] = this.outputs[0];
                            this.outputs[0] = tmp;

                            if (i != 1) {
                                tmp = this.inputs[i];
                                this.inputs[i] = this.inputs[1];
                                this.inputs[1] = tmp;
                                tmpi = this.inputsInts[i];
                                this.inputsInts[i] = this.inputsInts[1];
                                this.inputsInts[1] = tmpi;
                                tmp = this.outputs[i];
                                this.outputs[i] = this.outputs[1];
                                this.outputs[1] = tmp;
                            }
                        }
                        return this.outputs[0];
                    }
                }
            }
            return null;
        };

        prototype.GetCachedPowerInt = function(bi) {
            {
                for (var i = 0; i < this.size; ++i) {
                    if (this.inputsInts[i] >= 0 && this.inputsInts[i] == bi) {
                        if (i != 0) {
                            var tmp;

                            tmp = this.inputs[i];
                            this.inputs[i] = this.inputs[0];
                            this.inputs[0] = tmp;
                            var tmpi = this.inputsInts[i];
                            this.inputsInts[i] = this.inputsInts[0];
                            this.inputsInts[0] = tmpi;
                            tmp = this.outputs[i];
                            this.outputs[i] = this.outputs[0];
                            this.outputs[0] = tmp;

                            if (i != 1) {
                                tmp = this.inputs[i];
                                this.inputs[i] = this.inputs[1];
                                this.inputs[1] = tmp;
                                tmpi = this.inputsInts[i];
                                this.inputsInts[i] = this.inputsInts[1];
                                this.inputsInts[1] = tmpi;
                                tmp = this.outputs[i];
                                this.outputs[i] = this.outputs[1];
                                this.outputs[1] = tmp;
                            }
                        }
                        return this.outputs[0];
                    }
                }
            }
            return null;
        };

        prototype.AddPower = function(input, output) {
            {
                if (this.size < DecimalUtility.PowerCache.MaxSize) {

                    for (var i = this.size; i > 0; --i) {
                        this.inputs[i] = this.inputs[i - 1];
                        this.inputsInts[i] = this.inputsInts[i - 1];
                        this.outputs[i] = this.outputs[i - 1];
                    }
                    this.inputs[0] = input;
                    this.inputsInts[0] = input.canFitInInt() ? input.intValue() : -1;
                    this.outputs[0] = output;
                    ++this.size;
                } else {

                    for (var i = DecimalUtility.PowerCache.MaxSize - 1; i > 0; --i) {
                        this.inputs[i] = this.inputs[i - 1];
                        this.inputsInts[i] = this.inputsInts[i - 1];
                        this.outputs[i] = this.outputs[i - 1];
                    }
                    this.inputs[0] = input;
                    this.inputsInts[0] = input.canFitInInt() ? input.intValue() : -1;
                    this.outputs[0] = output;
                }
            }
        };
    })(DecimalUtility.PowerCache,DecimalUtility.PowerCache.prototype);

    constructor.powerOfFiveCache = new DecimalUtility.PowerCache();
    constructor.powerOfTenCache = new DecimalUtility.PowerCache();
    constructor.FindPowerOfFiveFromBig = function(diff) {
        var sign = diff.signum();
        if (sign < 0) {
            return BigInteger.ZERO;
        }
        if (sign == 0) {
            return BigInteger.ONE;
        }
        var intcurexp = FastInteger.FromBig(diff);
        if (intcurexp.CompareToInt(54) <= 0) {
            return DecimalUtility.FindPowerOfFive(intcurexp.AsInt32());
        }
        var mantissa = BigInteger.ONE;
        var bigpow;
        var origdiff = diff;
        bigpow = DecimalUtility.powerOfFiveCache.GetCachedPower(origdiff);
        if (bigpow != null) {
            return bigpow;
        }
        var otherPower = DecimalUtility.powerOfFiveCache.FindCachedPowerOrSmaller(origdiff);
        if (otherPower != null) {
            intcurexp.SubtractBig(otherPower[0]);
            bigpow = otherPower[1];
            mantissa = bigpow;
        } else {
            bigpow = BigInteger.ZERO;
        }
        while (intcurexp.signum() > 0) {
            if (intcurexp.CompareToInt(27) <= 0) {
                bigpow = DecimalUtility.FindPowerOfFive(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else if (intcurexp.CompareToInt(9999999) <= 0) {
                bigpow = (DecimalUtility.FindPowerOfFive(1)).pow(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else {
                if (bigpow.signum() == 0) {
                    bigpow = (DecimalUtility.FindPowerOfFive(1)).pow(9999999);
                }
                mantissa = mantissa.multiply(bigpow);
                intcurexp.AddInt(-9999999);
            }
        }
        DecimalUtility.powerOfFiveCache.AddPower(origdiff, mantissa);
        return mantissa;
    };
    constructor.valueBigInt36 = BigInteger.valueOf(36);
    constructor.FindPowerOfTenFromBig = function(bigintExponent) {
        var sign = bigintExponent.signum();
        if (sign < 0) {
            return BigInteger.ZERO;
        }
        if (sign == 0) {
            return BigInteger.ONE;
        }
        if (bigintExponent.compareTo(DecimalUtility.valueBigInt36) <= 0) {
            return DecimalUtility.FindPowerOfTen(bigintExponent.intValue());
        }
        var intcurexp = FastInteger.FromBig(bigintExponent);
        var mantissa = BigInteger.ONE;
        var bigpow = BigInteger.ZERO;
        while (intcurexp.signum() > 0) {
            if (intcurexp.CompareToInt(18) <= 0) {
                bigpow = DecimalUtility.FindPowerOfTen(intcurexp.AsInt32());
                mantissa = mantissa.multiply(bigpow);
                break;
            } else if (intcurexp.CompareToInt(9999999) <= 0) {
                var val = intcurexp.AsInt32();
                bigpow = DecimalUtility.FindPowerOfFive(val);
                bigpow = bigpow.shiftLeft(val);
                mantissa = mantissa.multiply(bigpow);
                break;
            } else {
                if (bigpow.signum() == 0) {
                    bigpow = DecimalUtility.FindPowerOfFive(9999999);
                    bigpow = bigpow.shiftLeft(9999999);
                }
                mantissa = mantissa.multiply(bigpow);
                intcurexp.AddInt(-9999999);
            }
        }
        return mantissa;
    };
    constructor.valueFivePower40 = (BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204))).multiply(BigInteger.valueOf(JSInteropFactory.createLongFromInts(1977800241, 22204)));
    constructor.FindPowerOfFive = function(precision) {
        if (precision < 0) {
            return BigInteger.ZERO;
        }
        if (precision == 0) {
            return BigInteger.ONE;
        }
        var bigpow;
        var ret;
        if (precision <= 27) {
            return DecimalUtility.valueBigIntPowersOfFive[(precision|0)];
        }
        if (precision == 40) {
            return DecimalUtility.valueFivePower40;
        }
        var startPrecision = precision;
        bigpow = DecimalUtility.powerOfFiveCache.GetCachedPowerInt(precision);
        if (bigpow != null) {
            return bigpow;
        }
        var origPrecision = BigInteger.valueOf(precision);
        if (precision <= 54) {
            if ((precision & 1) == 0) {
                ret = DecimalUtility.valueBigIntPowersOfFive[((precision >> 1)|0)];
                ret = ret.multiply(ret);
                DecimalUtility.powerOfFiveCache.AddPower(origPrecision, ret);
                return ret;
            } else {
                ret = DecimalUtility.valueBigIntPowersOfFive[27];
                bigpow = DecimalUtility.valueBigIntPowersOfFive[(precision|0) - 27];
                ret = ret.multiply(bigpow);
                DecimalUtility.powerOfFiveCache.AddPower(origPrecision, ret);
                return ret;
            }
        }
        if (precision > 40 && precision <= 94) {
            ret = DecimalUtility.valueFivePower40;
            bigpow = DecimalUtility.FindPowerOfFive(precision - 40);
            ret = ret.multiply(bigpow);
            DecimalUtility.powerOfFiveCache.AddPower(origPrecision, ret);
            return ret;
        }
        var otherPower;
        var first = true;
        bigpow = BigInteger.ZERO;
        while (true) {
            otherPower = DecimalUtility.powerOfFiveCache.FindCachedPowerOrSmaller(BigInteger.valueOf(precision));
            if (otherPower != null) {
                var otherPower0 = otherPower[0];
                var otherPower1 = otherPower[1];
                precision -= otherPower0.intValue();
                if (first) {
                    bigpow = otherPower[1];
                } else {
                    bigpow = bigpow.multiply(otherPower1);
                }
                first = false;
            } else {
                break;
            }
        }
        ret = !first ? bigpow : BigInteger.ONE;
        while (precision > 0) {
            if (precision <= 27) {
                bigpow = DecimalUtility.valueBigIntPowersOfFive[(precision|0)];
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                break;
            } else if (precision <= 9999999) {

                bigpow = (DecimalUtility.valueBigIntPowersOfFive[1]).pow(precision);
                if (precision != startPrecision) {
                    var bigprec = BigInteger.valueOf(precision);
                    DecimalUtility.powerOfFiveCache.AddPower(bigprec, bigpow);
                }
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                break;
            } else {
                if (bigpow.signum() == 0) {
                    bigpow = DecimalUtility.FindPowerOfFive(9999999);
                }
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                precision -= 9999999;
            }
        }
        DecimalUtility.powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
    };
    constructor.FindPowerOfTen = function(precision) {
        if (precision < 0) {
            return BigInteger.ZERO;
        }
        if (precision == 0) {
            return BigInteger.ONE;
        }
        var bigpow;
        var ret;
        if (precision <= 18) {
            return DecimalUtility.valueBigIntPowersOfTen[(precision|0)];
        }
        var startPrecision = precision;
        bigpow = DecimalUtility.powerOfTenCache.GetCachedPowerInt(precision);
        if (bigpow != null) {
            return bigpow;
        }
        var origPrecision = BigInteger.valueOf(precision);
        if (precision <= 27) {
            var prec = (precision|0);
            ret = DecimalUtility.valueBigIntPowersOfFive[prec];
            ret = ret.shiftLeft(prec);
            DecimalUtility.powerOfTenCache.AddPower(origPrecision, ret);
            return ret;
        }
        if (precision <= 36) {
            if ((precision & 1) == 0) {
                ret = DecimalUtility.valueBigIntPowersOfTen[((precision >> 1)|0)];
                ret = ret.multiply(ret);
                DecimalUtility.powerOfTenCache.AddPower(origPrecision, ret);
                return ret;
            } else {
                ret = DecimalUtility.valueBigIntPowersOfTen[18];
                bigpow = DecimalUtility.valueBigIntPowersOfTen[(precision|0) - 18];
                ret = ret.multiply(bigpow);
                DecimalUtility.powerOfTenCache.AddPower(origPrecision, ret);
                return ret;
            }
        }
        var otherPower;
        var first = true;
        bigpow = BigInteger.ZERO;
        while (true) {
            otherPower = DecimalUtility.powerOfTenCache.FindCachedPowerOrSmaller(BigInteger.valueOf(precision));
            if (otherPower != null) {
                var otherPower0 = otherPower[0];
                var otherPower1 = otherPower[1];
                precision -= otherPower0.intValue();
                if (first) {
                    bigpow = otherPower[1];
                } else {
                    bigpow = bigpow.multiply(otherPower1);
                }
                first = false;
            } else {
                break;
            }
        }
        ret = !first ? bigpow : BigInteger.ONE;
        while (precision > 0) {
            if (precision <= 18) {
                bigpow = DecimalUtility.valueBigIntPowersOfTen[(precision|0)];
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                break;
            } else if (precision <= 9999999) {

                bigpow = DecimalUtility.FindPowerOfFive(precision);
                bigpow = bigpow.shiftLeft(precision);
                if (precision != startPrecision) {
                    var bigprec = BigInteger.valueOf(precision);
                    DecimalUtility.powerOfTenCache.AddPower(bigprec, bigpow);
                }
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                break;
            } else {
                if (bigpow.signum() == 0) {
                    bigpow = DecimalUtility.FindPowerOfTen(9999999);
                }
                if (first) {
                    ret = bigpow;
                } else {
                    ret = ret.multiply(bigpow);
                }
                first = false;
                precision -= 9999999;
            }
        }
        DecimalUtility.powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
    };
})(DecimalUtility,DecimalUtility.prototype);

var Rounding={};Rounding.Up=0;Rounding['Up']=0;Rounding.Down=1;Rounding['Down']=1;Rounding.Ceiling=2;Rounding['Ceiling']=2;Rounding.Floor=3;Rounding['Floor']=3;Rounding.HalfUp=4;Rounding['HalfUp']=4;Rounding.HalfDown=5;Rounding['HalfDown']=5;Rounding.HalfEven=6;Rounding['HalfEven']=6;Rounding.Unnecessary=7;Rounding['Unnecessary']=7;Rounding.ZeroFiveUp=8;Rounding['ZeroFiveUp']=8;

if(typeof exports!=="undefined")exports['Rounding']=Rounding;
if(typeof window!=="undefined")window['Rounding']=Rounding;

var PrecisionContext =

function(precision, rounding, exponentMinSmall, exponentMaxSmall, clampNormalExponents) {

    if (precision < 0) {
        throw new Error("precision not greater or equal to 0 (" + (precision) + ")");
    }
    if (exponentMinSmall > exponentMaxSmall) {
        throw new Error("exponentMinSmall not less or equal to " + (exponentMaxSmall) + " (" + (exponentMinSmall) + ")");
    }
    this.bigintPrecision = precision == 0 ? BigInteger.ZERO : BigInteger.valueOf(precision);
    this.rounding = rounding;
    this.clampNormalExponents = clampNormalExponents;
    this.hasExponentRange = true;
    this.exponentMax = exponentMaxSmall == 0 ? BigInteger.ZERO : BigInteger.valueOf(exponentMaxSmall);
    this.exponentMin = exponentMinSmall == 0 ? BigInteger.ZERO : BigInteger.valueOf(exponentMinSmall);
};
(function(constructor,prototype){
    prototype['exponentMax'] = prototype.exponentMax = null;
    prototype['getEMax'] = prototype.getEMax = function() {
        return this.hasExponentRange ? this.exponentMax : BigInteger.ZERO;
    };
    prototype['traps'] = prototype.traps = null;
    prototype['getTraps'] = prototype.getTraps = function() {
        return this.traps;
    };
    prototype['exponentMin'] = prototype.exponentMin = null;
    prototype['hasExponentRange'] = prototype.hasExponentRange = null;
    prototype['getHasExponentRange'] = prototype.getHasExponentRange = function() {
        return this.hasExponentRange;
    };
    prototype['getEMin'] = prototype.getEMin = function() {
        return this.hasExponentRange ? this.exponentMin : BigInteger.ZERO;
    };
    prototype['bigintPrecision'] = prototype.bigintPrecision = null;
    prototype['getPrecision'] = prototype.getPrecision = function() {
        return this.bigintPrecision;
    };
    prototype['rounding'] = prototype.rounding = null;
    prototype['clampNormalExponents'] = prototype.clampNormalExponents = null;
    prototype['getClampNormalExponents'] = prototype.getClampNormalExponents = function() {
        return this.hasExponentRange ? this.clampNormalExponents : false;
    };
    prototype['getRounding'] = prototype.getRounding = function() {
        return this.rounding;
    };
    prototype['flags'] = prototype.flags = null;
    prototype['hasFlags'] = prototype.hasFlags = null;
    prototype['getHasFlags'] = prototype.getHasFlags = function() {
        return this.hasFlags;
    };
    constructor['FlagInexact'] = constructor.FlagInexact = 1;
    constructor['FlagRounded'] = constructor.FlagRounded = 2;
    constructor['FlagSubnormal'] = constructor.FlagSubnormal = 4;
    constructor['FlagUnderflow'] = constructor.FlagUnderflow = 8;
    constructor['FlagOverflow'] = constructor.FlagOverflow = 16;
    constructor['FlagClamped'] = constructor.FlagClamped = 32;
    constructor['FlagInvalid'] = constructor.FlagInvalid = 64;
    constructor['FlagDivideByZero'] = constructor.FlagDivideByZero = 128;
    prototype['getFlags'] = prototype.getFlags = function() {
        return this.flags;
    };
    prototype['setFlags'] = prototype.setFlags = function(value) {
        if (!this.getHasFlags()) {
            throw new Error("Can't set flags");
        }
        this.flags = value;
    };
    prototype['ExponentWithinRange'] = prototype.ExponentWithinRange = function(exponent) {
        if (exponent == null) {
            throw new Error("exponent");
        }
        if (!this.getHasExponentRange()) {
            return true;
        }
        if (this.bigintPrecision.signum() == 0) {
            return exponent.compareTo(this.getEMax()) <= 0;
        } else {
            var bigint = exponent;
            bigint = bigint.add(this.bigintPrecision);
            bigint = bigint.subtract(BigInteger.ONE);
            if (bigint.compareTo(this.getEMin()) < 0) {
                return false;
            }
            if (exponent.compareTo(this.getEMax()) > 0) {
                return false;
            }
            return true;
        }
    };
    prototype['toString'] = prototype.toString = function() {
        return "[PrecisionContext ExponentMax=" + this.exponentMax + ", Traps=" + this.traps + ", ExponentMin=" + this.exponentMin + ", HasExponentRange=" + this.hasExponentRange + ", BigintPrecision=" + this.bigintPrecision + ", Rounding=" + this.rounding + ", ClampNormalExponents=" + this.clampNormalExponents + ", Flags=" + this.flags + ", HasFlags=" + this.hasFlags + "]";
    };
    prototype['WithRounding'] = prototype.WithRounding = function(rounding) {
        var pc = this.Copy();
        pc.rounding = rounding;
        return pc;
    };
    prototype['WithBlankFlags'] = prototype.WithBlankFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = true;
        pc.flags = 0;
        return pc;
    };
    prototype['WithTraps'] = prototype.WithTraps = function(traps) {
        var pc = this.Copy();
        pc.hasFlags = true;
        pc.traps = traps;
        return pc;
    };
    prototype['WithExponentClamp'] = prototype.WithExponentClamp = function(clamp) {
        var pc = this.Copy();
        pc.clampNormalExponents = clamp;
        return pc;
    };
    prototype['WithExponentRange'] = prototype.WithExponentRange = function(exponentMinSmall, exponentMaxSmall) {
        if (exponentMinSmall > exponentMaxSmall) {
            throw new Error("exponentMin greater than exponentMax");
        }
        var pc = this.Copy();
        pc.hasExponentRange = true;
        pc.exponentMin = BigInteger.valueOf(exponentMinSmall);
        pc.exponentMax = BigInteger.valueOf(exponentMaxSmall);
        return pc;
    };
    prototype['WithBigExponentRange'] = prototype.WithBigExponentRange = function(exponentMin, exponentMax) {
        if (exponentMin == null) {
            throw new Error("exponentMin");
        }
        if (exponentMin.compareTo(exponentMax) > 0) {
            throw new Error("exponentMin greater than exponentMax");
        }
        var pc = this.Copy();
        pc.hasExponentRange = true;
        pc.exponentMin = exponentMin;
        pc.exponentMax = exponentMax;
        return pc;
    };
    prototype['WithNoFlags'] = prototype.WithNoFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = false;
        pc.flags = 0;
        return pc;
    };
    prototype['WithUnlimitedExponents'] = prototype.WithUnlimitedExponents = function() {
        var pc = this.Copy();
        pc.hasExponentRange = false;
        return pc;
    };
    prototype['WithPrecision'] = prototype.WithPrecision = function(precision) {
        if (precision < 0) {
            throw new Error("precision not greater or equal to 0 (" + (precision) + ")");
        }
        var pc = this.Copy();
        pc.bigintPrecision = BigInteger.valueOf(precision);
        return pc;
    };
    prototype['WithBigPrecision'] = prototype.WithBigPrecision = function(bigintPrecision) {
        if (bigintPrecision == null) {
            throw new Error("bigintPrecision");
        }
        if (bigintPrecision.signum() < 0) {
            throw new Error("precision not greater or equal to 0 (" + bigintPrecision + ")");
        }
        var pc = this.Copy();
        pc.bigintPrecision = bigintPrecision;
        return pc;
    };
    prototype['Copy'] = prototype.Copy = function() {
        var pcnew = new PrecisionContext(0, this.rounding, 0, 0, this.clampNormalExponents);
        pcnew.hasFlags = this.hasFlags;
        pcnew.flags = this.flags;
        pcnew.exponentMax = this.exponentMax;
        pcnew.exponentMin = this.exponentMin;
        pcnew.hasExponentRange = this.hasExponentRange;
        pcnew.bigintPrecision = this.bigintPrecision;
        pcnew.rounding = this.rounding;
        pcnew.clampNormalExponents = this.clampNormalExponents;
        return pcnew;
    };
    constructor['ForPrecision'] = constructor.ForPrecision = function(precision) {
        return new PrecisionContext(precision, Rounding.HalfUp, 0, 0, false).WithUnlimitedExponents();
    };
    constructor['ForRounding'] = constructor.ForRounding = function(rounding) {
        return new PrecisionContext(0, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor['ForPrecisionAndRounding'] = constructor.ForPrecisionAndRounding = function(precision, rounding) {
        return new PrecisionContext(precision, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor['Unlimited'] = constructor.Unlimited = PrecisionContext.ForPrecision(0);
    constructor['Binary16'] = constructor.Binary16 = PrecisionContext.ForPrecisionAndRounding(11, Rounding.HalfEven).WithExponentClamp(true).WithExponentRange(-14, 15);
    constructor['Binary32'] = constructor.Binary32 = PrecisionContext.ForPrecisionAndRounding(24, Rounding.HalfEven).WithExponentClamp(true).WithExponentRange(-126, 127);
    constructor['Binary64'] = constructor.Binary64 = PrecisionContext.ForPrecisionAndRounding(53, Rounding.HalfEven).WithExponentClamp(true).WithExponentRange(-1022, 1023);
    constructor['Binary128'] = constructor.Binary128 = PrecisionContext.ForPrecisionAndRounding(113, Rounding.HalfEven).WithExponentClamp(true).WithExponentRange(-16382, 16383);
    constructor['Decimal32'] = constructor.Decimal32 = new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);
    constructor['Decimal64'] = constructor.Decimal64 = new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);
    constructor['Decimal128'] = constructor.Decimal128 = new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);
    constructor['Basic'] = constructor.Basic = PrecisionContext.ForPrecisionAndRounding(9, Rounding.HalfUp);
    constructor['CliDecimal'] = constructor.CliDecimal = new PrecisionContext(96, Rounding.HalfEven, 0, 28, true);
})(PrecisionContext,PrecisionContext.prototype);

if(typeof exports!=="undefined")exports['PrecisionContext']=PrecisionContext;
if(typeof window!=="undefined")window['PrecisionContext']=PrecisionContext;

var BigNumberFlags = function() {

};
(function(constructor,prototype){
    constructor.FlagNegative = 1;
    constructor.FlagQuietNaN = 4;
    constructor.FlagSignalingNaN = 8;
    constructor.FlagInfinity = 2;
    constructor.FlagSpecial = BigNumberFlags.FlagQuietNaN | BigNumberFlags.FlagSignalingNaN | BigNumberFlags.FlagInfinity;
    constructor.FlagNaN = BigNumberFlags.FlagQuietNaN | BigNumberFlags.FlagSignalingNaN;
    constructor.FiniteOnly = 0;
    constructor.FiniteAndNonFinite = 1;
    constructor.X3Dot274 = 2;
})(BigNumberFlags,BigNumberFlags.prototype);

var RadixMath = function(helper) {

    this.helper = helper;
    this.support = helper.GetArithmeticSupport();
    this.thisRadix = helper.GetRadix();
};
(function(constructor,prototype){
    constructor.IntegerModeFixedScale = 1;
    constructor.IntegerModeRegular = 0;
    prototype.helper = null;
    prototype.thisRadix = null;
    prototype.support = null;
    prototype.ReturnQuietNaN = function(thisValue, ctx) {
        var mant = (this.helper.GetMantissa(thisValue)).abs();
        var mantChanged = false;
        if (mant.signum() != 0 && ctx != null && ctx.getPrecision().signum() != 0) {
            var limit = this.helper.MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()));
            if (mant.compareTo(limit) >= 0) {
                mant = mant.remainder(limit);
                mantChanged = true;
            }
        }
        var flags = this.helper.GetFlags(thisValue);
        if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return thisValue;
        }
        flags &= BigNumberFlags.FlagNegative;
        flags |= BigNumberFlags.FlagQuietNaN;
        return this.helper.CreateNewWithFlags(mant, BigInteger.ZERO, flags);
    };
    prototype.SquareRootHandleSpecial = function(thisValue, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
            if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
                return this.SignalingNaNInvalid(thisValue, ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
                return this.ReturnQuietNaN(thisValue, ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & BigNumberFlags.FlagNegative) != 0) {
                    return this.SignalInvalid(ctx);
                }
                return thisValue;
            }
        }
        var sign = this.helper.GetSign(thisValue);
        if (sign < 0) {
            return this.SignalInvalid(ctx);
        }
        return null;
    };
    prototype.DivisionHandleSpecial = function(thisValue, other, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, other, ctx);
            if (result != null) {
                return result;
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0 && (otherFlags & BigNumberFlags.FlagInfinity) != 0) {

                return this.SignalInvalid(ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                return this.EnsureSign(thisValue, ((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative) != 0);
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {

                if (ctx != null && ctx.getHasExponentRange() && ctx.getPrecision().signum() > 0) {
                    if (ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
                    }
                    var bigexp = ctx.getEMin();
                    var bigprec = ctx.getPrecision();
                    bigexp = bigexp.subtract(bigprec);
                    bigexp = bigexp.add(BigInteger.ONE);
                    thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
                    return this.helper.CreateNewWithFlags(BigInteger.ZERO, bigexp, thisFlags);
                }
                thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
                return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, thisFlags), ctx);
            }
        }
        return null;
    };
    prototype.RemainderHandleSpecial = function(thisValue, other, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, other, ctx);
            if (result != null) {
                return result;
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                return this.SignalInvalid(ctx);
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
                return this.RoundToPrecision(thisValue, ctx);
            }
        }
        if (this.helper.GetMantissa(other).signum() == 0) {
            return this.SignalInvalid(ctx);
        }
        return null;
    };
    prototype.MinMaxHandleSpecial = function(thisValue, otherValue, ctx, isMinOp, compareAbs) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(otherValue);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {

            if ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
                return this.SignalingNaNInvalid(thisValue, ctx);
            }
            if ((this.helper.GetFlags(otherValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
                return this.SignalingNaNInvalid(otherValue, ctx);
            }

            if ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
                if ((this.helper.GetFlags(otherValue) & BigNumberFlags.FlagQuietNaN) != 0) {

                    return this.ReturnQuietNaN(thisValue, ctx);
                }

                return this.RoundToPrecision(otherValue, ctx);
            }
            if ((this.helper.GetFlags(otherValue) & BigNumberFlags.FlagQuietNaN) != 0) {

                return this.RoundToPrecision(thisValue, ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                if (compareAbs && (otherFlags & BigNumberFlags.FlagInfinity) == 0) {

                    return isMinOp ? this.RoundToPrecision(otherValue, ctx) : thisValue;
                }

                if (isMinOp) {

                    return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ? thisValue : this.RoundToPrecision(otherValue, ctx);
                } else {

                    return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ? thisValue : this.RoundToPrecision(otherValue, ctx);
                }
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
                if (compareAbs) {

                    return isMinOp ? this.RoundToPrecision(thisValue, ctx) : otherValue;
                }
                if (isMinOp) {
                    return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ? this.RoundToPrecision(thisValue, ctx) : otherValue;
                } else {
                    return ((otherFlags & BigNumberFlags.FlagNegative) != 0) ? this.RoundToPrecision(thisValue, ctx) : otherValue;
                }
            }
        }
        return null;
    };
    prototype.HandleNotANumber = function(thisValue, other, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);

        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(other, ctx);
        }

        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(other, ctx);
        }
        return null;
    };
    prototype.MultiplyAddHandleSpecial = function(op1, op2, op3, ctx) {
        var op1Flags = this.helper.GetFlags(op1);

        if ((op1Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(op1, ctx);
        }
        var op2Flags = this.helper.GetFlags(op2);
        if ((op2Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(op2, ctx);
        }
        var op3Flags = this.helper.GetFlags(op3);
        if ((op3Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(op3, ctx);
        }

        if ((op1Flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(op1, ctx);
        }
        if ((op2Flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(op2, ctx);
        }

        if ((op1Flags & BigNumberFlags.FlagInfinity) != 0) {

            if ((op2Flags & BigNumberFlags.FlagSpecial) == 0 && this.helper.GetMantissa(op2).signum() == 0) {
                return this.SignalInvalid(ctx);
            }
        }
        if ((op2Flags & BigNumberFlags.FlagInfinity) != 0) {

            if ((op1Flags & BigNumberFlags.FlagSpecial) == 0 && this.helper.GetMantissa(op1).signum() == 0) {
                return this.SignalInvalid(ctx);
            }
        }

        if ((op3Flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(op3, ctx);
        }
        return null;
    };
    prototype.ValueOf = function(value, ctx) {
        if (ctx == null || !ctx.getHasExponentRange() || ctx.ExponentWithinRange(BigInteger.ZERO)) {
            return this.helper.ValueOf(value);
        }
        return this.RoundToPrecision(this.helper.ValueOf(value), ctx);
    };
    prototype.CompareToHandleSpecialReturnInt = function(thisValue, other) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {

            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {
                    return 0;
                }
                return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ? 1 : -1;
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {
                    return 0;
                }
                return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ? -1 : 1;
            }
        }
        return 2;
    };
    prototype.CompareToHandleSpecial = function(thisValue, other, treatQuietNansAsSignaling, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {

            if ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagSignalingNaN) != 0) {
                return this.SignalingNaNInvalid(thisValue, ctx);
            }
            if ((this.helper.GetFlags(other) & BigNumberFlags.FlagSignalingNaN) != 0) {
                return this.SignalingNaNInvalid(other, ctx);
            }
            if (treatQuietNansAsSignaling) {
                if ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
                    return this.SignalingNaNInvalid(thisValue, ctx);
                }
                if ((this.helper.GetFlags(other) & BigNumberFlags.FlagQuietNaN) != 0) {
                    return this.SignalingNaNInvalid(other, ctx);
                }
            } else {

                if ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagQuietNaN) != 0) {
                    return this.ReturnQuietNaN(thisValue, ctx);
                }
                if ((this.helper.GetFlags(other) & BigNumberFlags.FlagQuietNaN) != 0) {
                    return this.ReturnQuietNaN(other, ctx);
                }
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {
                    return this.ValueOf(0, null);
                }
                return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ? this.ValueOf(1, null) : this.ValueOf(-1, null);
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {
                    return this.ValueOf(0, null);
                }
                return ((otherFlags & BigNumberFlags.FlagNegative) == 0) ? this.ValueOf(-1, null) : this.ValueOf(1, null);
            }
        }
        return null;
    };
    prototype.SignalingNaNInvalid = function(value, ctx) {
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
        }
        return this.ReturnQuietNaN(value, ctx);
    };
    prototype.SignalInvalid = function(ctx) {
        if (this.support == BigNumberFlags.FiniteOnly) {
            throw new Error("Invalid operation");
        }
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
        }
        return this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagQuietNaN);
    };
    prototype.SignalInvalidWithMessage = function(ctx, str) {
        if (this.support == BigNumberFlags.FiniteOnly) {
            throw new Error(str);
        }
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
        }
        return this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagQuietNaN);
    };
    prototype.SignalOverflow = function(neg) {
        return this.support == BigNumberFlags.FiniteOnly ? null : this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, (neg ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity);
    };
    prototype.SignalOverflow2 = function(pc, neg) {
        if (pc != null) {
            var roundingOnOverflow = pc.getRounding();
            if (pc.getHasFlags()) {
                pc.setFlags(pc.getFlags() | (PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
            }
            if (pc.getPrecision().signum() != 0 && pc.getHasExponentRange() && (roundingOnOverflow == Rounding.Down || roundingOnOverflow == Rounding.ZeroFiveUp || (roundingOnOverflow == Rounding.Ceiling && neg) || (roundingOnOverflow == Rounding.Floor && !neg))) {

                var overflowMant = BigInteger.ZERO;
                var fastPrecision = FastInteger.FromBig(pc.getPrecision());
                overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                overflowMant = overflowMant.subtract(BigInteger.ONE);
                var clamp = FastInteger.FromBig(pc.getEMax()).Increment().Subtract(fastPrecision);
                return this.helper.CreateNewWithFlags(overflowMant, clamp.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
            }
        }
        return this.SignalOverflow(neg);
    };
    prototype.SignalDivideByZero = function(ctx, neg) {
        if (this.support == BigNumberFlags.FiniteOnly) {
            throw new Error("Division by zero");
        }
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagDivideByZero));
        }
        return this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity | (neg ? BigNumberFlags.FlagNegative : 0));
    };
    prototype.Round = function(accum, rounding, neg, fastint) {
        var incremented = false;
        if (rounding == Rounding.HalfEven) {
            var radix = this.thisRadix;
            if (accum.getLastDiscardedDigit() >= ((radix / 2)|0)) {
                if (accum.getLastDiscardedDigit() > ((radix / 2)|0) || accum.getOlderDiscardedDigits() != 0) {
                    incremented = true;
                } else if (!fastint.isEvenNumber()) {
                    incremented = true;
                }
            }
        } else if (rounding == Rounding.ZeroFiveUp) {
            var radix = this.thisRadix;
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                if (radix == 2) {
                    incremented = true;
                } else {
                    var lastDigit = FastInteger.Copy(fastint).Remainder(radix).AsInt32();
                    if (lastDigit == 0 || lastDigit == ((radix / 2)|0)) {
                        incremented = true;
                    }
                }
            }
        } else if (rounding != Rounding.Down) {
            incremented = this.RoundGivenDigits(accum.getLastDiscardedDigit(), accum.getOlderDiscardedDigits(), rounding, neg, BigInteger.ZERO);
        }
        return incremented;
    };
    prototype.RoundGivenDigits = function(lastDiscarded, olderDiscarded, rounding, neg, bigval) {
        var incremented = false;
        var radix = this.thisRadix;
        if (rounding == Rounding.HalfUp) {
            if (lastDiscarded >= ((radix / 2)|0)) {
                incremented = true;
            }
        } else if (rounding == Rounding.HalfEven) {

            if (lastDiscarded >= ((radix / 2)|0)) {
                if (lastDiscarded > ((radix / 2)|0) || olderDiscarded != 0) {
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
            if (lastDiscarded > ((radix / 2)|0) || (lastDiscarded == ((radix / 2)|0) && olderDiscarded != 0)) {
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
    prototype.RoundGivenBigInt = function(accum, rounding, neg, bigval) {
        return this.RoundGivenDigits(accum.getLastDiscardedDigit(), accum.getOlderDiscardedDigits(), rounding, neg, bigval);
    };
    prototype.RescaleByExponentDiff = function(mantissa, e1, e2) {
        if (mantissa.signum() == 0) {
            return BigInteger.ZERO;
        }
        var diff = FastInteger.FromBig(e1).SubtractBig(e2).Abs();
        return this.helper.MultiplyByRadixPower(mantissa, diff);
    };
    prototype.EnsureSign = function(val, negative) {
        if (val == null) {
            return val;
        }
        var flags = this.helper.GetFlags(val);
        if ((negative && (flags & BigNumberFlags.FlagNegative) == 0) || (!negative && (flags & BigNumberFlags.FlagNegative) != 0)) {
            flags &= ~BigNumberFlags.FlagNegative;
            flags |= negative ? BigNumberFlags.FlagNegative : 0;
            return this.helper.CreateNewWithFlags(this.helper.GetMantissa(val), this.helper.GetExponent(val), flags);
        }
        return val;
    };

    prototype.DivideToIntegerNaturalScale = function(thisValue, divisor, ctx) {
        var desiredScale = FastInteger.FromBig(this.helper.GetExponent(thisValue)).SubtractBig(this.helper.GetExponent(divisor));
        var ctx2 = PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()).WithBlankFlags();
        var ret = this.DivideInternal(thisValue, divisor, ctx2, RadixMath.IntegerModeFixedScale, BigInteger.ZERO);
        if ((ctx2.getFlags() & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero));
            }
            return ret;
        }
        var neg = (this.helper.GetSign(thisValue) < 0) ^ (this.helper.GetSign(divisor) < 0);

        if (this.helper.GetMantissa(ret).signum() == 0) {

            var dividendExp = this.helper.GetExponent(thisValue);
            var divisorExp = this.helper.GetExponent(divisor);
            ret = this.helper.CreateNewWithFlags(BigInteger.ZERO, dividendExp.subtract(divisorExp), this.helper.GetFlags(ret));
        } else {
            if (desiredScale.signum() < 0) {

                desiredScale.Negate();
                var bigmantissa = (this.helper.GetMantissa(ret)).abs();
                bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, desiredScale);
                var exponentDivisor = this.helper.GetExponent(divisor);
                ret = this.helper.CreateNewWithFlags(bigmantissa, this.helper.GetExponent(thisValue).subtract(exponentDivisor), this.helper.GetFlags(ret));
            } else if (desiredScale.signum() > 0) {

                var bigmantissa = (this.helper.GetMantissa(ret)).abs();
                var fastexponent = FastInteger.FromBig(this.helper.GetExponent(ret));
                var bigradix = BigInteger.valueOf(this.thisRadix);
                while (true) {
                    if (desiredScale.compareTo(fastexponent) == 0) {
                        break;
                    }
                    var bigrem;
                    var bigquo;
                    {
                        var divrem = (bigmantissa).divideAndRemainder(bigradix);
                        bigquo = divrem[0];
                        bigrem = divrem[1];
                    }
                    if (bigrem.signum() != 0) {
                        break;
                    }
                    bigmantissa = bigquo;
                    fastexponent.Increment();
                }
                ret = this.helper.CreateNewWithFlags(bigmantissa, fastexponent.AsBigInteger(), this.helper.GetFlags(ret));
            }
        }
        if (ctx != null) {
            ret = this.RoundToPrecision(ret, ctx);
        }
        ret = this.EnsureSign(ret, neg);
        return ret;
    };

    prototype.DivideToIntegerZeroScale = function(thisValue, divisor, ctx) {
        var ctx2 = PrecisionContext.ForRounding(Rounding.Down).WithBigPrecision(ctx == null ? BigInteger.ZERO : ctx.getPrecision()).WithBlankFlags();
        var ret = this.DivideInternal(thisValue, divisor, ctx2, RadixMath.IntegerModeFixedScale, BigInteger.ZERO);
        if ((ctx2.getFlags() & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (ctx2.getFlags() & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)));
            }
            return ret;
        }
        if (ctx != null) {
            ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
            ret = this.RoundToPrecision(ret, ctx2);
            if ((ctx2.getFlags() & PrecisionContext.FlagRounded) != 0) {
                return this.SignalInvalid(ctx);
            }
        }
        return ret;
    };

    prototype.Abs = function(value, ctx) {
        var flags = this.helper.GetFlags(value);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(value, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(value, ctx);
        }
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
            return this.RoundToPrecision(this.helper.CreateNewWithFlags(this.helper.GetMantissa(value), this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative), ctx);
        }
        return this.RoundToPrecision(value, ctx);
    };

    prototype.Negate = function(value, ctx) {
        var flags = this.helper.GetFlags(value);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(value, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(value, ctx);
        }
        var mant = this.helper.GetMantissa(value);
        if ((flags & BigNumberFlags.FlagInfinity) == 0 && mant.signum() == 0) {
            if ((flags & BigNumberFlags.FlagNegative) == 0) {

                return this.RoundToPrecision(this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative), ctx);
            } else if (ctx != null && ctx.getRounding() == Rounding.Floor) {

                return this.RoundToPrecision(this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags | BigNumberFlags.FlagNegative), ctx);
            } else {
                return this.RoundToPrecision(this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative), ctx);
            }
        }
        flags ^= BigNumberFlags.FlagNegative;
        return this.RoundToPrecision(this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags), ctx);
    };
    prototype.AbsRaw = function(value) {
        return this.EnsureSign(value, false);
    };
    prototype.IsFinite = function(val) {
        return (this.helper.GetFlags(val) & BigNumberFlags.FlagSpecial) == 0;
    };
    prototype.IsNegative = function(val) {
        return (this.helper.GetFlags(val) & BigNumberFlags.FlagNegative) != 0;
    };
    prototype.NegateRaw = function(val) {
        if (val == null) {
            return val;
        }
        var sign = this.helper.GetFlags(val) & BigNumberFlags.FlagNegative;
        return this.helper.CreateNewWithFlags(this.helper.GetMantissa(val), this.helper.GetExponent(val), sign == 0 ? BigNumberFlags.FlagNegative : 0);
    };
    constructor.TransferFlags = function(ctxDst, ctxSrc) {
        if (ctxDst != null && ctxDst.getHasFlags()) {
            if ((ctxSrc.getFlags() & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)) != 0) {
                ctxDst.setFlags(ctxDst.getFlags() | (ctxSrc.getFlags() & (PrecisionContext.FlagInvalid | PrecisionContext.FlagDivideByZero)));
            } else {
                ctxDst.setFlags(ctxDst.getFlags() | (ctxSrc.getFlags()));
            }
        }
    };

    prototype.Remainder = function(thisValue, divisor, ctx) {
        var ctx2 = ctx == null ? null : ctx.WithBlankFlags();
        var ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
        if (ret != null) {
            RadixMath.TransferFlags(ctx, ctx2);
            return ret;
        }
        ret = this.DivideToIntegerZeroScale(thisValue, divisor, ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
            return this.SignalInvalid(ctx);
        }
        ret = this.Add(thisValue, this.NegateRaw(this.Multiply(ret, divisor, null)), ctx2);
        ret = this.EnsureSign(ret, (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
        RadixMath.TransferFlags(ctx, ctx2);
        return ret;
    };

    prototype.RemainderNear = function(thisValue, divisor, ctx) {
        var ctx2 = ctx == null ? PrecisionContext.ForRounding(Rounding.HalfEven).WithBlankFlags() : ctx.WithRounding(Rounding.HalfEven).WithBlankFlags();
        var ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
        if (ret != null) {
            RadixMath.TransferFlags(ctx, ctx2);
            return ret;
        }
        ret = this.DivideInternal(thisValue, divisor, ctx2, RadixMath.IntegerModeFixedScale, BigInteger.ZERO);
        if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
            return this.SignalInvalid(ctx);
        }
        ctx2 = ctx2.WithBlankFlags();
        ret = this.RoundToPrecision(ret, ctx2);
        if ((ctx2.getFlags() & (PrecisionContext.FlagRounded | PrecisionContext.FlagInvalid)) != 0) {
            return this.SignalInvalid(ctx);
        }
        ctx2 = ctx == null ? PrecisionContext.Unlimited.WithBlankFlags() : ctx.WithBlankFlags();
        var ret2 = this.Add(thisValue, this.NegateRaw(this.Multiply(ret, divisor, null)), ctx2);
        if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
            return this.SignalInvalid(ctx);
        }
        if (this.helper.GetFlags(ret2) == 0 && this.helper.GetMantissa(ret2).signum() == 0) {
            ret2 = this.EnsureSign(ret2, (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
        }
        RadixMath.TransferFlags(ctx, ctx2);
        return ret2;
    };

    prototype.Pi = function(ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }

        var a = this.helper.ValueOf(1);
        var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(BigInteger.TEN)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp);
        var two = this.helper.ValueOf(2);
        var b = this.Divide(a, this.SquareRoot(two, ctxdiv), ctxdiv);
        var four = this.helper.ValueOf(4);
        var half = ((this.thisRadix & 1) == 0) ? this.helper.CreateNewWithFlags(BigInteger.valueOf((this.thisRadix / 2)|0), BigInteger.ZERO.subtract(BigInteger.ONE), 0) : null;
        var t = this.Divide(a, four, ctxdiv);
        var more = true;
        var lastCompare = 0;
        var vacillations = 0;
        var lastGuess = null;
        var guess = null;
        var powerTwo = BigInteger.ONE;
        while (more) {
            lastGuess = guess;
            var aplusB = this.Add(a, b, null);
            var newA = (half == null) ? this.Divide(aplusB, two, ctxdiv) : this.Multiply(aplusB, half, null);
            var valueAMinusNewA = this.Add(a, this.NegateRaw(newA), null);
            if (!a.equals(b)) {
                var atimesB = this.Multiply(a, b, ctxdiv);
                b = this.SquareRoot(atimesB, ctxdiv);
            }
            a = newA;
            guess = this.Multiply(aplusB, aplusB, null);
            guess = this.Divide(guess, this.Multiply(t, four, null), ctxdiv);
            var newGuess = guess;
            if (lastGuess != null) {
                var guessCmp = this.compareTo(lastGuess, newGuess);
                if (guessCmp == 0) {
                    more = false;
                } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 && guessCmp < 0)) {

                    ++vacillations;
                    if (vacillations > 3 && guessCmp > 0) {

                        more = false;
                    }
                }
                lastCompare = guessCmp;
            }
            if (more) {
                var tmpT = this.Multiply(valueAMinusNewA, valueAMinusNewA, null);
                tmpT = this.Multiply(tmpT, this.helper.CreateNewWithFlags(powerTwo, BigInteger.ZERO, 0), null);
                t = this.Add(t, this.NegateRaw(tmpT), ctxdiv);
                powerTwo = powerTwo.shiftLeft(1);
            }
            guess = newGuess;
        }
        return this.RoundToPrecision(guess, ctx);
    };
    prototype.LnInternal = function(thisValue, workingPrecision, ctx) {
        var more = true;
        var lastCompare = 0;
        var vacillations = 0;
        var ctxdiv = ctx.WithBigPrecision(workingPrecision.add(BigInteger.valueOf(6))).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp);
        var z = this.Add(this.NegateRaw(thisValue), this.helper.ValueOf(1), null);
        var zpow = this.Multiply(z, z, ctxdiv);
        var guess = this.NegateRaw(z);
        var lastGuess = null;
        var denom = BigInteger.valueOf(2);
        while (more) {
            lastGuess = guess;
            var tmp = this.Divide(zpow, this.helper.CreateNewWithFlags(denom, BigInteger.ZERO, 0), ctxdiv);
            var newGuess = this.Add(guess, this.NegateRaw(tmp), ctxdiv);
            {
                var guessCmp = this.compareTo(lastGuess, newGuess);
                if (guessCmp == 0) {
                    more = false;
                } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 && guessCmp < 0)) {

                    ++vacillations;
                    if (vacillations > 3 && guessCmp > 0) {

                        more = false;
                    }
                }
                lastCompare = guessCmp;
            }
            guess = newGuess;
            if (more) {
                zpow = this.Multiply(zpow, z, ctxdiv);
                denom = denom.add(BigInteger.ONE);
            }
        }
        return this.RoundToPrecision(guess, ctx);
    };
    prototype.ExpInternal = function(thisValue, workingPrecision, ctx) {
        var one = this.helper.ValueOf(1);
        var ctxdiv = ctx.WithBigPrecision(workingPrecision.add(BigInteger.valueOf(6))).WithRounding(this.thisRadix == 2 ? Rounding.Down : Rounding.ZeroFiveUp);
        var bigintN = BigInteger.valueOf(2);
        var facto = BigInteger.ONE;

        var guess = this.Add(one, thisValue, null);
        var lastGuess = guess;
        var pow = thisValue;
        var more = true;
        var lastCompare = 0;
        var vacillations = 0;
        while (more) {
            lastGuess = guess;

            pow = this.Multiply(pow, thisValue, ctxdiv);
            facto = facto.multiply(bigintN);
            var tmp = this.Divide(pow, this.helper.CreateNewWithFlags(facto, BigInteger.ZERO, 0), ctxdiv);
            var newGuess = this.Add(guess, tmp, ctxdiv);

            {
                var guessCmp = this.compareTo(lastGuess, newGuess);
                if (guessCmp == 0) {
                    more = false;
                } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 && guessCmp < 0)) {

                    ++vacillations;
                    if (vacillations > 3 && guessCmp > 0) {

                        more = false;
                    }
                }
                lastCompare = guessCmp;
            }
            guess = newGuess;
            if (more) {
                bigintN = bigintN.add(BigInteger.ONE);
            }
        }
        return this.RoundToPrecision(guess, ctx);
    };
    prototype.PowerIntegral = function(thisValue, powIntBig, ctx) {
        var sign = powIntBig.signum();
        var one = this.helper.ValueOf(1);
        if (sign == 0) {

            return this.RoundToPrecision(one, ctx);
        } else if (powIntBig.equals(BigInteger.ONE)) {
            return this.RoundToPrecision(thisValue, ctx);
        } else if (powIntBig.equals(BigInteger.valueOf(2))) {
            return this.Multiply(thisValue, thisValue, ctx);
        } else if (powIntBig.equals(BigInteger.valueOf(3))) {
            return this.Multiply(thisValue, this.Multiply(thisValue, thisValue, null), ctx);
        }
        var retvalNeg = this.IsNegative(thisValue) && powIntBig.testBit(0);
        var error = this.helper.CreateShiftAccumulator((powIntBig).abs()).GetDigitLength();
        error.AddInt(6);
        var bigError = error.AsBigInteger();
        var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(bigError)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
        if (sign < 0) {

            thisValue = this.Divide(one, thisValue, ctxdiv);
            if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
                return this.SignalOverflow2(ctx, retvalNeg);
            }
            powIntBig = powIntBig.negate();
        }
        var r = one;

        while (powIntBig.signum() != 0) {

            if (powIntBig.testBit(0)) {
                r = this.Multiply(r, thisValue, ctxdiv);

                if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {
                    return this.SignalOverflow2(ctx, retvalNeg);
                }
            }
            powIntBig = powIntBig.shiftRight(1);
            if (powIntBig.signum() != 0) {
                ctxdiv.setFlags(0);
                var tmp = this.Multiply(thisValue, thisValue, ctxdiv);

                if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {

                    return this.SignalOverflow2(ctx, retvalNeg);
                }
                thisValue = tmp;
            }
        }
        return this.RoundToPrecision(r, ctx);
    };
    prototype.ExtendPrecision = function(thisValue, ctx) {
        if (ctx == null || ctx.getPrecision().signum() == 0) {
            return this.RoundToPrecision(thisValue, ctx);
        }
        var mant = (this.helper.GetMantissa(thisValue)).abs();
        var digits = this.helper.CreateShiftAccumulator(mant).GetDigitLength();
        var fastPrecision = FastInteger.FromBig(ctx.getPrecision());
        var exponent = this.helper.GetExponent(thisValue);
        if (digits.compareTo(fastPrecision) < 0) {
            fastPrecision.Subtract(digits);
            mant = this.helper.MultiplyByRadixPower(mant, fastPrecision);
            var bigPrec = fastPrecision.AsBigInteger();
            exponent = exponent.subtract(bigPrec);
        }
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact));
        }
        return this.RoundToPrecision(this.helper.CreateNewWithFlags(mant, exponent, 0), ctx);
    };
    prototype.IsWithinExponentRangeForPow = function(thisValue, ctx) {
        if (ctx == null || !ctx.getHasExponentRange()) {
            return true;
        }
        var digits = this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs()).GetDigitLength();
        var exp = this.helper.GetExponent(thisValue);
        var fi = FastInteger.FromBig(exp);
        fi.Add(digits);
        fi.Decrement();

        if (fi.signum() < 0) {
            fi.Negate().Divide(2).Negate();
        }

        exp = fi.AsBigInteger();
        if (exp.compareTo(ctx.getEMin()) < 0 || exp.compareTo(ctx.getEMax()) > 0) {
            return false;
        }
        return true;
    };

    prototype.Power = function(thisValue, pow, ctx) {
        var ret = this.HandleNotANumber(thisValue, pow, ctx);
        if (ret != null) {
            return ret;
        }
        var thisSign = this.helper.GetSign(thisValue);
        var powSign = this.helper.GetSign(pow);
        var thisFlags = this.helper.GetFlags(thisValue);
        var powFlags = this.helper.GetFlags(pow);
        if (thisSign == 0 && powSign == 0) {

            return this.SignalInvalid(ctx);
        }
        if (thisSign < 0 && (powFlags & BigNumberFlags.FlagInfinity) != 0) {

            return this.SignalInvalid(ctx);
        }
        if (thisSign > 0 && (thisFlags & BigNumberFlags.FlagInfinity) == 0 && (powFlags & BigNumberFlags.FlagInfinity) != 0) {

            var cmp = this.compareTo(thisValue, this.helper.ValueOf(1));
            if (cmp < 0) {

                if (powSign < 0) {

                    return this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity);
                } else {

                    return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), ctx);
                }
            } else if (cmp == 0) {

                return this.ExtendPrecision(this.helper.ValueOf(1), ctx);
            } else {

                if (powSign > 0) {

                    return pow;
                } else {

                    return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), ctx);
                }
            }
        }
        var powExponent = this.helper.GetExponent(pow);
        var isPowIntegral = powExponent.signum() > 0;
        var isPowOdd = false;
        var powInt = null;
        if (!isPowIntegral) {
            powInt = this.Quantize(pow, this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), PrecisionContext.ForRounding(Rounding.Down));
            isPowIntegral = this.compareTo(powInt, pow) == 0;
            isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
        } else {
            if (powExponent.equals(BigInteger.ZERO)) {
                isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
            } else if (this.thisRadix % 2 == 0) {

                isPowOdd = false;
            } else {
                powInt = this.Quantize(pow, this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), PrecisionContext.ForRounding(Rounding.Down));
                isPowOdd = !this.helper.GetMantissa(powInt).testBit(0) == false;
            }
        }

        var isResultNegative = false;
        if ((thisFlags & BigNumberFlags.FlagNegative) != 0 && (powFlags & BigNumberFlags.FlagInfinity) == 0 && isPowIntegral && isPowOdd) {
            isResultNegative = true;
        }
        if (thisSign == 0 && powSign != 0) {
            var infinityFlags = (powSign < 0) ? BigNumberFlags.FlagInfinity : 0;
            if (isResultNegative) {
                infinityFlags |= BigNumberFlags.FlagNegative;
            }
            thisValue = this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, infinityFlags);
            if ((infinityFlags & BigNumberFlags.FlagInfinity) == 0) {
                thisValue = this.RoundToPrecision(thisValue, ctx);
            }
            return thisValue;
        }
        if ((!isPowIntegral || powSign < 0) && (ctx == null || ctx.getPrecision().signum() == 0)) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null or has unlimited precision, and pow's exponent is not an integer or is negative");
        }
        if (thisSign < 0 && !isPowIntegral) {
            return this.SignalInvalid(ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {

            if (powSign > 0) {
                return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, (isResultNegative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity), ctx);
            } else if (powSign < 0) {
                return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, isResultNegative ? BigNumberFlags.FlagNegative : 0), ctx);
            } else {
                return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ONE, BigInteger.ZERO, 0), ctx);
            }
        }
        if (powSign == 0) {
            return this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ONE, BigInteger.ZERO, 0), ctx);
        }
        if (isPowIntegral) {

            if (this.compareTo(thisValue, this.helper.ValueOf(1)) == 0) {
                if (!this.IsWithinExponentRangeForPow(pow, ctx)) {
                    return this.SignalInvalid(ctx);
                }
                return this.helper.ValueOf(1);
            }
            if (powInt == null) {
                powInt = this.Quantize(pow, this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), PrecisionContext.ForRounding(Rounding.Down));
            }
            var signedMant = (this.helper.GetMantissa(powInt)).abs();
            if (powSign < 0) {
                signedMant = signedMant.negate();
            }

            return this.PowerIntegral(thisValue, signedMant, ctx);
        }

        if (this.compareTo(thisValue, this.helper.ValueOf(1)) == 0 && powSign > 0) {
            if (!this.IsWithinExponentRangeForPow(pow, ctx)) {
                return this.SignalInvalid(ctx);
            }
            return this.ExtendPrecision(this.helper.ValueOf(1), ctx);
        }

        if (this.thisRadix == 10 || this.thisRadix == 2) {
            var half = (this.thisRadix == 10) ? this.helper.CreateNewWithFlags(BigInteger.valueOf(5), BigInteger.ZERO.subtract(BigInteger.ONE), 0) : this.helper.CreateNewWithFlags(BigInteger.ONE, BigInteger.ZERO.subtract(BigInteger.ONE), 0);
            if (this.compareTo(pow, half) == 0 && this.IsWithinExponentRangeForPow(pow, ctx) && this.IsWithinExponentRangeForPow(thisValue, ctx)) {
                var ctxCopy = ctx.WithBlankFlags();
                thisValue = this.SquareRoot(thisValue, ctxCopy);
                ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
                ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
                if ((ctxCopy.getFlags() & PrecisionContext.FlagSubnormal) != 0) {
                    ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagUnderflow));
                }
                thisValue = this.ExtendPrecision(thisValue, ctxCopy);
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
                }
                return thisValue;
            }
        }
        var guardDigitCount = this.thisRadix == 2 ? 32 : 10;
        var guardDigits = BigInteger.valueOf(guardDigitCount);
        var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(guardDigits)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
        var lnresult = this.Ln(thisValue, ctxdiv);

        lnresult = this.Multiply(lnresult, pow, ctxdiv);

        ctxdiv = ctx.WithBlankFlags();
        lnresult = this.Exp(lnresult, ctxdiv);

        if ((ctxdiv.getFlags() & (PrecisionContext.FlagClamped | PrecisionContext.FlagOverflow)) != 0) {
            if (!this.IsWithinExponentRangeForPow(thisValue, ctx)) {
                return this.SignalInvalid(ctx);
            }
            if (!this.IsWithinExponentRangeForPow(pow, ctx)) {
                return this.SignalInvalid(ctx);
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxdiv.getFlags()));
        }
        return lnresult;
    };

    prototype.Log10 = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null or has unlimited precision");
        }
        var flags = this.helper.GetFlags(thisValue);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {

            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {

            return this.ReturnQuietNaN(thisValue, ctx);
        }
        var sign = this.helper.GetSign(thisValue);
        if (sign < 0) {
            return this.SignalInvalid(ctx);
        }
        if ((flags & BigNumberFlags.FlagInfinity) != 0) {
            return thisValue;
        }
        var ctxCopy = ctx.WithBlankFlags();
        var one = this.helper.ValueOf(1);

        if (sign == 0) {

            thisValue = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity), ctxCopy);
        } else if (this.compareTo(thisValue, one) == 0) {

            thisValue = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), ctxCopy);
        } else {
            var exp = this.helper.GetExponent(thisValue);
            var mant = (this.helper.GetMantissa(thisValue)).abs();
            if (mant.equals(BigInteger.ONE) && this.thisRadix == 10) {

                thisValue = this.RoundToPrecision(this.helper.CreateNewWithFlags(exp, BigInteger.ZERO, exp.signum() < 0 ? BigNumberFlags.FlagNegative : 0), ctxCopy);
            } else {
                var mantissa = this.helper.GetMantissa(thisValue);
                var expTmp = FastInteger.FromBig(exp);
                var tenBig = BigInteger.TEN;
                while (true) {
                    var bigrem;
                    var bigquo;
                    {
                        var divrem = (mantissa).divideAndRemainder(tenBig);
                        bigquo = divrem[0];
                        bigrem = divrem[1];
                    }
                    if (bigrem.signum() != 0) {
                        break;
                    }
                    mantissa = bigquo;
                    expTmp.Increment();
                }
                if (mantissa.compareTo(BigInteger.ONE) == 0 && (this.thisRadix == 10 || expTmp.signum() == 0 || exp.signum() == 0)) {

                    thisValue = this.RoundToPrecision(this.helper.CreateNewWithFlags(expTmp.AsBigInteger(), BigInteger.ZERO, expTmp.signum() < 0 ? BigNumberFlags.FlagNegative : 0), ctxCopy);
                } else {
                    var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(BigInteger.TEN)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
                    var ten = this.helper.ValueOf(10);
                    var logNatural = this.Ln(thisValue, ctxdiv);
                    var logTen = this.LnTenConstant(ctxdiv);

                    thisValue = this.Divide(logNatural, logTen, ctx);

                    if (ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
                    }
                }
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
        }
        return thisValue;
    };
    constructor.PowerOfTwo = function(fi) {
        if (fi.signum() <= 0) {
            return BigInteger.ONE;
        }
        if (fi.CanFitInInt32()) {
            var val = fi.AsInt32();
            if (val <= 30) {
                val = 1 << val;
                return BigInteger.valueOf(val);
            }
            return BigInteger.ONE.shiftLeft(val);
        } else {
            var bi = BigInteger.ONE;
            var fi2 = FastInteger.Copy(fi);
            while (fi2.signum() > 0) {
                var count = 1000000;
                if (fi2.CompareToInt(1000000) < 0) {
                    count = bi.intValue();
                }
                bi = bi.shiftLeft(count);
                fi2.SubtractInt(count);
            }
            return bi;
        }
    };

    prototype.LnTenConstant = function(ctx) {
        if (ctx == null) {
            throw new Error("ctx");
        }
        var thisValue = this.helper.ValueOf(10);
        var two = this.helper.ValueOf(2);
        var error;
        var bigError;
        error = new FastInteger(10);
        bigError = error.AsBigInteger();
        var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(bigError)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
        for (var i = 0; i < 9; ++i) {
            thisValue = this.SquareRoot(thisValue, ctxdiv.WithUnlimitedExponents());
        }

        thisValue = this.Divide(this.helper.ValueOf(1), thisValue, ctxdiv);
        thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
        thisValue = this.NegateRaw(thisValue);
        thisValue = this.Multiply(thisValue, this.helper.ValueOf(1 << 9), ctx);
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact));
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
        }
        return thisValue;
    };

    prototype.Ln = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        var flags = this.helper.GetFlags(thisValue);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {

            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {

            return this.ReturnQuietNaN(thisValue, ctx);
        }
        var sign = this.helper.GetSign(thisValue);
        if (sign < 0) {
            return this.SignalInvalid(ctx);
        }
        if ((flags & BigNumberFlags.FlagInfinity) != 0) {
            return thisValue;
        }
        var ctxCopy = ctx.WithBlankFlags();
        var one = this.helper.ValueOf(1);
        if (sign == 0) {
            return this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity);
        } else {
            var cmpOne = this.compareTo(thisValue, one);
            var ctxdiv = null;
            if (cmpOne == 0) {

                thisValue = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), ctxCopy);
            } else if (cmpOne < 0) {

                var error = new FastInteger(10);
                var bigError = error.AsBigInteger();
                ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(bigError)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
                var quarter = this.Divide(one, this.helper.ValueOf(4), ctxCopy);
                if (this.compareTo(thisValue, quarter) <= 0) {

                    var half = this.Multiply(quarter, this.helper.ValueOf(2), null);
                    var roots = new FastInteger(0);

                    while (this.compareTo(thisValue, half) < 0) {
                        thisValue = this.SquareRoot(thisValue, ctxdiv.WithUnlimitedExponents());
                        roots.Increment();
                    }
                    thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
                    var bigintRoots = RadixMath.PowerOfTwo(roots);

                    thisValue = this.Multiply(thisValue, this.helper.CreateNewWithFlags(bigintRoots, BigInteger.ZERO, 0), ctxCopy);
                } else {
                    var smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
                    var closeToOne = this.Add(one, this.NegateRaw(smallfrac), null);
                    if (this.compareTo(thisValue, closeToOne) >= 0) {

                        error = this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs()).GetDigitLength();
                        error.AddInt(6);
                        error.AddBig(ctx.getPrecision());
                        bigError = error.AsBigInteger();
                        thisValue = this.LnInternal(thisValue, error.AsBigInteger(), ctxCopy);
                    } else {
                        thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
                    }
                }
                if (ctx.getHasFlags()) {
                    ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
                    ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
                }
            } else {

                var two = this.helper.ValueOf(2);
                if (this.compareTo(thisValue, two) >= 0) {
                    var roots = new FastInteger(0);
                    var error;
                    var bigError;
                    error = new FastInteger(10);
                    bigError = error.AsBigInteger();
                    ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(bigError)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
                    var smallfrac = this.Divide(one, this.helper.ValueOf(10), ctxdiv);
                    var closeToOne = this.Add(one, smallfrac, null);

                    while (this.compareTo(thisValue, closeToOne) >= 0) {
                        thisValue = this.SquareRoot(thisValue, ctxdiv.WithUnlimitedExponents());
                        roots.Increment();
                    }

                    thisValue = this.Divide(one, thisValue, ctxdiv);
                    thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxdiv);
                    thisValue = this.NegateRaw(thisValue);
                    var bigintRoots = RadixMath.PowerOfTwo(roots);

                    thisValue = this.Multiply(thisValue, this.helper.CreateNewWithFlags(bigintRoots, BigInteger.ZERO, 0), ctxCopy);
                } else {
                    var error;
                    var bigError;
                    error = new FastInteger(10);
                    bigError = error.AsBigInteger();
                    ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(bigError)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
                    var smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
                    var closeToOne = this.Add(one, smallfrac, null);
                    if (this.compareTo(thisValue, closeToOne) >= 0) {

                        thisValue = this.Divide(one, thisValue, ctxdiv);
                        thisValue = this.LnInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
                        thisValue = this.NegateRaw(thisValue);
                    } else {
                        error = this.helper.CreateShiftAccumulator((this.helper.GetMantissa(thisValue)).abs()).GetDigitLength();
                        error.AddInt(6);
                        error.AddBig(ctx.getPrecision());
                        bigError = error.AsBigInteger();

                        thisValue = this.LnInternal(thisValue, error.AsBigInteger(), ctxCopy);
                    }
                }
                if (ctx.getHasFlags()) {
                    ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagInexact));
                    ctxCopy.setFlags(ctxCopy.getFlags() | (PrecisionContext.FlagRounded));
                }
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
        }
        return thisValue;
    };

    prototype.Exp = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        var flags = this.helper.GetFlags(thisValue);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {

            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {

            return this.ReturnQuietNaN(thisValue, ctx);
        }
        var ctxCopy = ctx.WithBlankFlags();
        if ((flags & BigNumberFlags.FlagInfinity) != 0) {
            if ((flags & BigNumberFlags.FlagNegative) != 0) {
                var retval = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, BigInteger.ZERO, 0), ctxCopy);
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
                }
                return retval;
            }
            return thisValue;
        }
        var sign = this.helper.GetSign(thisValue);
        var one = this.helper.ValueOf(1);
        var guardDigits = this.thisRadix == 2 ? ctx.getPrecision().add(BigInteger.TEN) : BigInteger.TEN;
        var ctxdiv = ctx.WithBigPrecision(ctx.getPrecision().add(guardDigits)).WithRounding(this.thisRadix == 2 ? Rounding.HalfEven : Rounding.ZeroFiveUp).WithBlankFlags();
        if (sign == 0) {
            thisValue = this.RoundToPrecision(one, ctxCopy);
        } else if (sign > 0 && this.compareTo(thisValue, one) < 0) {
            thisValue = this.ExpInternal(thisValue, ctxdiv.getPrecision(), ctxCopy);
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
            }
        } else if (sign < 0) {
            var val = this.Exp(this.NegateRaw(thisValue), ctxdiv);
            if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0 || !this.IsFinite(val)) {

                var newMax;
                ctxdiv.setFlags(0);
                newMax = ctx.getEMax();
                var expdiff = ctx.getEMin();
                expdiff = newMax.subtract(expdiff);
                newMax = newMax.add(expdiff);
                ctxdiv = ctxdiv.WithBigExponentRange(ctxdiv.getEMin(), newMax);
                thisValue = this.Exp(this.NegateRaw(thisValue), ctxdiv);
                if ((ctxdiv.getFlags() & PrecisionContext.FlagOverflow) != 0) {

                    if (ctx.getHasFlags()) {
                        var newFlags = PrecisionContext.FlagInexact | PrecisionContext.FlagSubnormal | PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
                        ctx.setFlags(ctx.getFlags() | (newFlags));
                    }

                    var ctxdivPrec = ctxdiv.getPrecision();
                    newMax = ctx.getEMin();
                    newMax = newMax.subtract(ctxdivPrec);
                    newMax = newMax.add(BigInteger.ONE);
                    thisValue = this.helper.CreateNewWithFlags(BigInteger.ZERO, newMax, 0);
                    return this.RoundToPrecisionInternal(thisValue, 0, 1, null, false, false, ctx);
                }
            } else {
                thisValue = val;
            }

            thisValue = this.Divide(one, thisValue, ctxCopy);

            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
            }
        } else {
            var intpart = this.Quantize(thisValue, one, PrecisionContext.ForRounding(Rounding.Down));
            if (this.compareTo(thisValue, this.helper.ValueOf(50000)) > 0 && ctx.getHasExponentRange()) {

                this.PowerIntegral(this.helper.ValueOf(2), BigInteger.valueOf(50000), ctxCopy);
                if ((ctxCopy.getFlags() & PrecisionContext.FlagOverflow) != 0) {

                    return this.SignalOverflow2(ctx, false);
                }
                ctxCopy.setFlags(0);

                this.PowerIntegral(this.helper.ValueOf(2), this.helper.GetMantissa(intpart), ctxCopy);
                if ((ctxCopy.getFlags() & PrecisionContext.FlagOverflow) != 0) {

                    return this.SignalOverflow2(ctx, false);
                }
                ctxCopy.setFlags(0);
            }
            var fracpart = this.Add(thisValue, this.NegateRaw(intpart), null);
            fracpart = this.Add(one, this.Divide(fracpart, intpart, ctxdiv), null);
            ctxdiv.setFlags(0);

            thisValue = this.ExpInternal(fracpart, ctxdiv.getPrecision(), ctxdiv);

            if ((ctxdiv.getFlags() & PrecisionContext.FlagUnderflow) != 0) {
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (ctxdiv.getFlags()));
                }
            }
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
            }
            thisValue = this.PowerIntegral(thisValue, this.helper.GetMantissa(intpart), ctxCopy);
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxCopy.getFlags()));
        }
        return thisValue;
    };
    constructor.NthRootWithRemainder = function(value, root) {
        if (root <= 0) {
            throw new Error("root not greater than 0 (" + (JSInteropFactory.createLong(root)) + ")");
        }
        if (value.signum() < 0) {
            throw new Error("value.signum() not greater or equal to 0 (" + (JSInteropFactory.createLong(value).signum()) + ")");
        }
        if (value.signum() == 0) {
            return [BigInteger.ZERO, BigInteger.ZERO];
        }
        if (value.equals(BigInteger.ONE)) {
            return [BigInteger.ONE, BigInteger.ZERO];
        }
        if (root == 1) {
            return [value, BigInteger.ZERO];
        }
        if (root == 2) {
            return value.sqrtWithRemainder();
        }
        var nm1 = root - 1;
        var bits = value.bitLength();
        var bitsdn = ((bits / root)|0);
        var bigintGuess = BigInteger.ONE.shiftLeft(bitsdn);
        var lastGuess = bigintGuess;
        while (true) {
            var bigintTmp = value;
            bigintTmp = bigintTmp.divide(bigintGuess).pow(nm1);
            var bigintTmp2 = bigintGuess;
            bigintGuess = bigintGuess.multiply(BigInteger.valueOf(nm1));
            bigintGuess = bigintTmp.add(bigintTmp2);
            bigintGuess = bigintGuess.divide(BigInteger.valueOf(root));
            if (bigintGuess.equals(lastGuess)) {

                lastGuess = (bigintGuess).pow(root);
                lastGuess = value.subtract(lastGuess);
                return [bigintGuess, lastGuess];
            }
            lastGuess = bigintGuess;
        }
    };

    prototype.SquareRoot = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        var ret = this.SquareRootHandleSpecial(thisValue, ctx);
        if (ret != null) {
            return ret;
        }
        var ctxtmp = ctx.WithBlankFlags();
        var currentExp = this.helper.GetExponent(thisValue);
        var origExp = currentExp;
        var idealExp;
        idealExp = currentExp;
        idealExp = idealExp.divide(BigInteger.valueOf(2));
        if (currentExp.signum() < 0 && currentExp.testBit(0)) {

            idealExp = idealExp.subtract(BigInteger.ONE);
        }

        if (this.helper.GetSign(thisValue) == 0) {
            ret = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, idealExp, this.helper.GetFlags(thisValue)), ctxtmp);
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags()));
            }
            return ret;
        }
        var mantissa = (this.helper.GetMantissa(thisValue)).abs();
        var accum = this.helper.CreateShiftAccumulator(mantissa);
        var digitCount = accum.GetDigitLength();
        var targetPrecision = FastInteger.FromBig(ctx.getPrecision());
        var precision = FastInteger.Copy(targetPrecision).Multiply(2).AddInt(2);
        var rounded = false;
        var inexact = false;
        if (digitCount.compareTo(precision) < 0) {
            var diff = FastInteger.Copy(precision).Subtract(digitCount);

            if ((!diff.isEvenNumber()) ^ (origExp.testBit(0))) {
                diff.Increment();
            }
            var bigdiff = diff.AsBigInteger();
            currentExp = currentExp.subtract(bigdiff);
            mantissa = this.helper.MultiplyByRadixPower(mantissa, diff);
        } else if (digitCount.compareTo(precision) < 0) {
            var diff = FastInteger.Copy(digitCount).Subtract(precision);
            accum.ShiftRight(diff);
            var bigdiff = diff.AsBigInteger();
            currentExp = currentExp.add(bigdiff);
            mantissa = accum.getShiftedInt();
            rounded = true;
            inexact = (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0;
        }
        var sr = mantissa.sqrtWithRemainder();
        digitCount = this.helper.CreateShiftAccumulator(sr[0]).GetDigitLength();
        var squareRootRemainder = sr[1];

        mantissa = sr[0];
        if (squareRootRemainder.signum() != 0) {
            rounded = true;
            inexact = true;
        }
        var oldexp = currentExp;
        currentExp = currentExp.divide(BigInteger.valueOf(2));
        if (oldexp.signum() < 0 && oldexp.testBit(0)) {

            currentExp = currentExp.subtract(BigInteger.ONE);
        }
        var retval = this.helper.CreateNewWithFlags(mantissa, currentExp, 0);

        retval = this.RoundToPrecisionInternal(retval, 0, inexact ? 1 : 0, null, false, false, ctxtmp);
        currentExp = this.helper.GetExponent(retval);

        if ((ctxtmp.getFlags() & PrecisionContext.FlagUnderflow) == 0) {
            var expcmp = currentExp.compareTo(idealExp);
            if (expcmp <= 0 || !this.IsFinite(retval)) {
                retval = this.ReduceToPrecisionAndIdealExponent(retval, ctx.getHasExponentRange() ? ctxtmp : null, inexact ? targetPrecision : null, FastInteger.FromBig(idealExp));
            }
        }
        if (ctx.getHasFlags() && ctx.getClampNormalExponents() && !this.helper.GetExponent(retval).equals(idealExp) && (ctxtmp.getFlags() & PrecisionContext.FlagInexact) == 0) {
            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
        }
        if ((ctxtmp.getFlags() & PrecisionContext.FlagOverflow) != 0) {
            rounded = true;
        }

        currentExp = this.helper.GetExponent(retval);
        if (rounded) {
            ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
        } else {
            if (currentExp.compareTo(idealExp) > 0) {

                ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
            } else {

                ctxtmp.setFlags(ctxtmp.getFlags() & ~(PrecisionContext.FlagRounded));
            }
        }
        if (inexact) {
            ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagRounded));
            ctxtmp.setFlags(ctxtmp.getFlags() | (PrecisionContext.FlagInexact));
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags()));
        }
        return retval;
    };

    prototype.NextMinus = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        if (!ctx.getHasExponentRange()) {
            return this.SignalInvalidWithMessage(ctx, "doesn't satisfy ctx.getHasExponentRange()");
        }
        var flags = this.helper.GetFlags(thisValue);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagInfinity) != 0) {
            if ((flags & BigNumberFlags.FlagNegative) != 0) {
                return thisValue;
            } else {
                var bigexp2 = ctx.getEMax();
                var bigprec = ctx.getPrecision();
                bigexp2 = bigexp2.add(BigInteger.ONE);
                bigexp2 = bigexp2.subtract(bigprec);
                var overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()));
                overflowMant = overflowMant.subtract(BigInteger.ONE);
                return this.helper.CreateNewWithFlags(overflowMant, bigexp2, 0);
            }
        }
        var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).Increment();
        var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.compareTo(minexp) <= 0) {

            minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        }
        var quantum = this.helper.CreateNewWithFlags(BigInteger.ONE, minexp.AsBigInteger(), BigNumberFlags.FlagNegative);
        var ctx2;
        ctx2 = ctx.WithRounding(Rounding.Floor);
        return this.Add(thisValue, quantum, ctx2);
    };

    prototype.NextToward = function(thisValue, otherValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        if (!ctx.getHasExponentRange()) {
            return this.SignalInvalidWithMessage(ctx, "doesn't satisfy ctx.getHasExponentRange()");
        }
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(otherValue);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, otherValue, ctx);
            if (result != null) {
                return result;
            }
        }
        var ctx2;
        var cmp = this.compareTo(thisValue, otherValue);
        if (cmp == 0) {
            return this.RoundToPrecision(this.EnsureSign(thisValue, (otherFlags & BigNumberFlags.FlagNegative) != 0), ctx.WithNoFlags());
        } else {
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                if ((thisFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (otherFlags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative))) {

                    return thisValue;
                } else {
                    var bigexp2 = ctx.getEMax();
                    var bigprec = ctx.getPrecision();
                    bigexp2 = bigexp2.add(BigInteger.ONE);
                    bigexp2 = bigexp2.subtract(bigprec);
                    var overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()));
                    overflowMant = overflowMant.subtract(BigInteger.ONE);
                    return this.helper.CreateNewWithFlags(overflowMant, bigexp2, thisFlags & BigNumberFlags.FlagNegative);
                }
            }
            var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).Increment();
            var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
            if (bigexp.compareTo(minexp) < 0) {

                minexp = FastInteger.Copy(bigexp).SubtractInt(2);
            } else {

                minexp.SubtractInt(2);
            }
            var quantum = this.helper.CreateNewWithFlags(BigInteger.ONE, minexp.AsBigInteger(), (cmp > 0) ? BigNumberFlags.FlagNegative : 0);
            var val = thisValue;
            ctx2 = ctx.WithRounding((cmp > 0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
            val = this.Add(val, quantum, ctx2);
            if ((ctx2.getFlags() & (PrecisionContext.FlagOverflow | PrecisionContext.FlagUnderflow)) == 0) {

                ctx2.setFlags(0);
            }
            if ((ctx2.getFlags() & PrecisionContext.FlagUnderflow) != 0) {
                var bigmant = (this.helper.GetMantissa(val)).abs();
                var maxmant = this.helper.MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()).Decrement());
                if (bigmant.compareTo(maxmant) >= 0 || ctx.getPrecision().compareTo(BigInteger.ONE) == 0) {

                    ctx2.setFlags(0);
                }
            }
            if (ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
            }
            return val;
        }
    };

    prototype.NextPlus = function(thisValue, ctx) {
        if (ctx == null) {
            return this.SignalInvalidWithMessage(ctx, "ctx is null");
        }
        if (ctx.getPrecision().signum() == 0) {
            return this.SignalInvalidWithMessage(ctx, "ctx has unlimited precision");
        }
        if (!ctx.getHasExponentRange()) {
            return this.SignalInvalidWithMessage(ctx, "doesn't satisfy ctx.getHasExponentRange()");
        }
        var flags = this.helper.GetFlags(thisValue);
        if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((flags & BigNumberFlags.FlagInfinity) != 0) {
            if ((flags & BigNumberFlags.FlagNegative) != 0) {
                var bigexp2 = ctx.getEMax();
                var bigprec = ctx.getPrecision();
                bigexp2 = bigexp2.add(BigInteger.ONE);
                bigexp2 = bigexp2.subtract(bigprec);
                var overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, FastInteger.FromBig(ctx.getPrecision()));
                overflowMant = overflowMant.subtract(BigInteger.ONE);
                return this.helper.CreateNewWithFlags(overflowMant, bigexp2, BigNumberFlags.FlagNegative);
            } else {
                return thisValue;
            }
        }
        var minexp = FastInteger.FromBig(ctx.getEMin()).SubtractBig(ctx.getPrecision()).Increment();
        var bigexp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.compareTo(minexp) <= 0) {

            minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        }
        var quantum = this.helper.CreateNewWithFlags(BigInteger.ONE, minexp.AsBigInteger(), 0);
        var ctx2;
        var val = thisValue;
        ctx2 = ctx.WithRounding(Rounding.Ceiling);
        return this.Add(val, quantum, ctx2);
    };

    prototype.DivideToExponent = function(thisValue, divisor, desiredExponent, ctx) {
        if (ctx != null && !ctx.ExponentWithinRange(desiredExponent)) {
            return this.SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + desiredExponent.toString());
        }
        var ctx2 = (ctx == null) ? PrecisionContext.ForRounding(Rounding.HalfDown) : ctx.WithUnlimitedExponents().WithPrecision(0);
        var ret = this.DivideInternal(thisValue, divisor, ctx2, RadixMath.IntegerModeFixedScale, desiredExponent);
        if (ctx2.getPrecision().signum() == 0 && this.IsFinite(ret)) {

            ret = this.Quantize(ret, ret, ctx2);
            if ((ctx2.getFlags() & PrecisionContext.FlagInvalid) != 0) {
                ctx2.setFlags(PrecisionContext.FlagInvalid);
            }
        }
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
        }
        return ret;
    };

    prototype.Divide = function(thisValue, divisor, ctx) {
        return this.DivideInternal(thisValue, divisor, ctx, RadixMath.IntegerModeRegular, BigInteger.ZERO);
    };
    prototype.RoundToScaleStatus = function(remainder, divisor, ctx) {
        var rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
        var lastDiscarded = 0;
        var olderDiscarded = 0;
        if (remainder.signum() != 0) {
            if (rounding == Rounding.HalfDown || rounding == Rounding.HalfUp || rounding == Rounding.HalfEven) {
                var halfDivisor = divisor.shiftRight(1);
                var cmpHalf = remainder.compareTo(halfDivisor);
                if ((cmpHalf == 0) && divisor.testBit(0) == false) {

                    lastDiscarded = ((this.thisRadix / 2)|0);
                    olderDiscarded = 0;
                } else if (cmpHalf > 0) {

                    lastDiscarded = ((this.thisRadix / 2)|0);
                    olderDiscarded = 1;
                } else {

                    lastDiscarded = 0;
                    olderDiscarded = 1;
                }
            } else {

                if (rounding == Rounding.Unnecessary) {

                    return null;
                }
                lastDiscarded = 1;
                olderDiscarded = 1;
            }
        }
        return [lastDiscarded, olderDiscarded];
    };
    prototype.RoundToScale = function(mantissa, remainder, divisor, desiredExponent, shift, neg, ctx) {
        var accum;
        var rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
        var lastDiscarded = 0;
        var olderDiscarded = 0;
        if (remainder.signum() != 0) {
            if (rounding == Rounding.HalfDown || rounding == Rounding.HalfUp || rounding == Rounding.HalfEven) {
                var halfDivisor = divisor.shiftRight(1);
                var cmpHalf = remainder.compareTo(halfDivisor);
                if ((cmpHalf == 0) && divisor.testBit(0) == false) {

                    lastDiscarded = ((this.thisRadix / 2)|0);
                    olderDiscarded = 0;
                } else if (cmpHalf > 0) {

                    lastDiscarded = ((this.thisRadix / 2)|0);
                    olderDiscarded = 1;
                } else {

                    lastDiscarded = 0;
                    olderDiscarded = 1;
                }
            } else {

                if (rounding == Rounding.Unnecessary) {
                    return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                }
                lastDiscarded = 1;
                olderDiscarded = 1;
            }
        }
        var flags = 0;
        var newmantissa = mantissa;
        if (shift.isValueZero()) {
            if ((lastDiscarded | olderDiscarded) != 0) {
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (rounding == Rounding.Unnecessary) {
                    return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                }
                if (this.RoundGivenDigits(lastDiscarded, olderDiscarded, rounding, neg, newmantissa)) {
                    newmantissa = newmantissa.add(BigInteger.ONE);
                }
            }
        } else {
            accum = this.helper.CreateShiftAccumulatorWithDigits(mantissa, lastDiscarded, olderDiscarded);
            accum.ShiftRight(shift);
            newmantissa = accum.getShiftedInt();
            if (accum.getDiscardedDigitCount().signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                if (mantissa.signum() != 0) {
                    flags |= PrecisionContext.FlagRounded;
                }
                if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                    if (rounding == Rounding.Unnecessary) {
                        return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                    }
                }
                if (this.RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
                    newmantissa = newmantissa.add(BigInteger.ONE);
                }
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (flags));
        }
        return this.helper.CreateNewWithFlags(newmantissa, desiredExponent, neg ? BigNumberFlags.FlagNegative : 0);
    };
    prototype.DivideInternal = function(thisValue, divisor, ctx, integerMode, desiredExponent) {
        var ret = this.DivisionHandleSpecial(thisValue, divisor, ctx);
        if (ret != null) {
            return ret;
        }
        var signA = this.helper.GetSign(thisValue);
        var signB = this.helper.GetSign(divisor);
        if (signB == 0) {
            if (signA == 0) {
                return this.SignalInvalid(ctx);
            }
            var flagsNeg = ((this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0) ^ ((this.helper.GetFlags(divisor) & BigNumberFlags.FlagNegative) != 0);
            return this.SignalDivideByZero(ctx, flagsNeg);
        }
        var radix = this.thisRadix;
        if (signA == 0) {
            var retval = null;
            if (integerMode == RadixMath.IntegerModeFixedScale) {
                var newflags = (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) ^ (this.helper.GetFlags(divisor) & BigNumberFlags.FlagNegative);
                retval = this.helper.CreateNewWithFlags(BigInteger.ZERO, desiredExponent, newflags);
            } else {
                var dividendExp = this.helper.GetExponent(thisValue);
                var divisorExp = this.helper.GetExponent(divisor);
                var newflags = (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) ^ (this.helper.GetFlags(divisor) & BigNumberFlags.FlagNegative);
                retval = this.RoundToPrecision(this.helper.CreateNewWithFlags(BigInteger.ZERO, dividendExp.subtract(divisorExp), newflags), ctx);
            }
            return retval;
        } else {
            var mantissaDividend = (this.helper.GetMantissa(thisValue)).abs();
            var mantissaDivisor = (this.helper.GetMantissa(divisor)).abs();
            var expDividend = FastInteger.FromBig(this.helper.GetExponent(thisValue));
            var expDivisor = FastInteger.FromBig(this.helper.GetExponent(divisor));
            var expdiff = FastInteger.Copy(expDividend).Subtract(expDivisor);
            var adjust = new FastInteger(0);
            var result = new FastInteger(0);
            var naturalExponent = FastInteger.Copy(expdiff);
            var hasPrecision = ctx != null && ctx.getPrecision().signum() != 0;
            var resultNeg = (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != (this.helper.GetFlags(divisor) & BigNumberFlags.FlagNegative);
            var fastPrecision = (!hasPrecision) ? new FastInteger(0) : FastInteger.FromBig(ctx.getPrecision());
            var dividendPrecision = null;
            var divisorPrecision = null;
            if (integerMode == RadixMath.IntegerModeFixedScale) {
                var shift;
                var rem;
                var fastDesiredExponent = FastInteger.FromBig(desiredExponent);
                if (ctx != null && ctx.getHasFlags() && fastDesiredExponent.compareTo(naturalExponent) > 0) {

                    ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                }
                if (expdiff.compareTo(fastDesiredExponent) <= 0) {
                    shift = FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
                    var quo;
                    {
                        var divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
                        quo = divrem[0];
                        rem = divrem[1];
                    }
                    return this.RoundToScale(quo, rem, mantissaDivisor, desiredExponent, shift, resultNeg, ctx);
                } else if (ctx != null && ctx.getPrecision().signum() != 0 && FastInteger.Copy(expdiff).SubtractInt(8).compareTo(fastPrecision) > 0) {

                    return this.SignalInvalidWithMessage(ctx, "Result can't fit the precision");
                } else {
                    shift = FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
                    mantissaDividend = this.helper.MultiplyByRadixPower(mantissaDividend, shift);
                    var quo;
                    {
                        var divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
                        quo = divrem[0];
                        rem = divrem[1];
                    }
                    return this.RoundToScale(quo, rem, mantissaDivisor, desiredExponent, new FastInteger(0), resultNeg, ctx);
                }
            }
            if (integerMode == RadixMath.IntegerModeRegular) {
                var rem = null;
                var quo = null;

                {
                    var divrem = (mantissaDividend).divideAndRemainder(mantissaDivisor);
                    quo = divrem[0];
                    rem = divrem[1];
                }
                if (rem.signum() == 0) {

                    if (resultNeg) {
                        quo = quo.negate();
                    }
                    return this.RoundToPrecision(this.helper.CreateNewWithFlags(quo, naturalExponent.AsBigInteger(), resultNeg ? BigNumberFlags.FlagNegative : 0), ctx);
                } else {
                    rem = null;
                    quo = null;
                }
                if (hasPrecision) {
                    var divid = mantissaDividend;
                    var shift = FastInteger.FromBig(ctx.getPrecision());
                    dividendPrecision = this.helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
                    divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
                    if (dividendPrecision.compareTo(divisorPrecision) <= 0) {
                        divisorPrecision.Subtract(dividendPrecision);
                        divisorPrecision.Increment();
                        shift.Add(divisorPrecision);
                        divid = this.helper.MultiplyByRadixPower(divid, shift);
                    } else {

                        dividendPrecision.Subtract(divisorPrecision);
                        if (dividendPrecision.compareTo(shift) <= 0) {
                            shift.Subtract(dividendPrecision);
                            shift.Increment();
                            divid = this.helper.MultiplyByRadixPower(divid, shift);
                        } else {

                            shift.SetInt(0);
                        }
                    }
                    dividendPrecision = this.helper.CreateShiftAccumulator(divid).GetDigitLength();
                    divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
                    if (shift.signum() != 0 || quo == null) {

                        {
                            var divrem = (divid).divideAndRemainder(mantissaDivisor);
                            quo = divrem[0];
                            rem = divrem[1];
                        }
                    }

                    var digitStatus = this.RoundToScaleStatus(rem, mantissaDivisor, ctx);
                    if (digitStatus == null) {
                        return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                    }
                    var natexp = FastInteger.Copy(naturalExponent).Subtract(shift);
                    var ctxcopy = ctx.WithBlankFlags();
                    var retval2 = this.helper.CreateNewWithFlags(quo, natexp.AsBigInteger(), resultNeg ? BigNumberFlags.FlagNegative : 0);
                    retval2 = this.RoundToPrecisionWithShift(retval2, ctxcopy, digitStatus[0], digitStatus[1], null, false);
                    if ((ctxcopy.getFlags() & PrecisionContext.FlagInexact) != 0) {
                        if (ctx.getHasFlags()) {
                            ctx.setFlags(ctx.getFlags() | (ctxcopy.getFlags()));
                        }
                        return retval2;
                    } else {
                        if (ctx.getHasFlags()) {
                            ctx.setFlags(ctx.getFlags() | (ctxcopy.getFlags()));
                            ctx.setFlags(ctx.getFlags() & ~(PrecisionContext.FlagRounded));
                        }
                        return this.ReduceToPrecisionAndIdealExponent(retval2, ctx, rem.signum() == 0 ? null : fastPrecision, expdiff);
                    }
                }
            }

            var mantcmp = mantissaDividend.compareTo(mantissaDivisor);
            if (mantcmp < 0) {

                dividendPrecision = this.helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
                divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
                divisorPrecision.Subtract(dividendPrecision);
                if (divisorPrecision.isValueZero()) {
                    divisorPrecision.Increment();
                }

                mantissaDividend = this.helper.MultiplyByRadixPower(mantissaDividend, divisorPrecision);
                adjust.Add(divisorPrecision);
                if (mantissaDividend.compareTo(mantissaDivisor) < 0) {

                    if (radix == 2) {
                        mantissaDividend = mantissaDividend.shiftLeft(1);
                    } else {
                        mantissaDividend = mantissaDividend.multiply(BigInteger.valueOf(radix));
                    }
                    adjust.Increment();
                }
            } else if (mantcmp > 0) {

                dividendPrecision = this.helper.CreateShiftAccumulator(mantissaDividend).GetDigitLength();
                divisorPrecision = this.helper.CreateShiftAccumulator(mantissaDivisor).GetDigitLength();
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
                    adjust.Increment();
                }
            }
            if (mantcmp == 0) {
                result = new FastInteger(1);
                mantissaDividend = BigInteger.ZERO;
            } else {
                {
                    if (!this.helper.HasTerminatingRadixExpansion(mantissaDividend, mantissaDivisor)) {
                        return this.SignalInvalidWithMessage(ctx, "Result would have a nonterminating expansion");
                    }
                    var divs = FastInteger.FromBig(mantissaDivisor);
                    var divd = FastInteger.FromBig(mantissaDividend);
                    var divisorFits = divs.CanFitInInt32();
                    var smallDivisor = divisorFits ? divs.AsInt32() : 0;
                    var halfRadix = ((radix / 2)|0);
                    var divsHalfRadix = null;
                    if (radix != 2) {
                        divsHalfRadix = FastInteger.FromBig(mantissaDivisor).Multiply(halfRadix);
                    }
                    while (true) {
                        var remainderZero = false;
                        var count = 0;
                        if (divd.CanFitInInt32()) {
                            if (divisorFits) {
                                var smallDividend = divd.AsInt32();
                                count = ((smallDividend / smallDivisor)|0);
                                divd.SetInt(smallDividend % smallDivisor);
                            } else {
                                count = 0;
                            }
                        } else {
                            if (divsHalfRadix != null) {
                                count = count + (halfRadix * divd.RepeatedSubtract(divsHalfRadix));
                            }
                            count = count + (divd.RepeatedSubtract(divs));
                        }
                        result.AddInt(count);
                        remainderZero = divd.isValueZero();
                        if (remainderZero && adjust.signum() >= 0) {
                            mantissaDividend = divd.AsBigInteger();
                            break;
                        }
                        adjust.Increment();
                        result.Multiply(radix);
                        divd.Multiply(radix);
                    }
                }
            }

            var exp = FastInteger.Copy(expdiff).Subtract(adjust);
            var rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
            var lastDiscarded = 0;
            var olderDiscarded = 0;
            if (mantissaDividend.signum() != 0) {
                if (rounding == Rounding.HalfDown || rounding == Rounding.HalfEven || rounding == Rounding.HalfUp) {
                    var halfDivisor = mantissaDivisor.shiftRight(1);
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
                } else {
                    if (rounding == Rounding.Unnecessary) {
                        return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                    }
                    lastDiscarded = 1;
                    olderDiscarded = 1;
                }
            }
            var bigResult = result.AsBigInteger();
            if (ctx != null && ctx.getHasFlags() && exp.compareTo(expdiff) > 0) {

                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
            }
            var bigexp = exp.AsBigInteger();
            var retval = this.helper.CreateNewWithFlags(bigResult, bigexp, resultNeg ? BigNumberFlags.FlagNegative : 0);
            return this.RoundToPrecisionWithShift(retval, ctx, lastDiscarded, olderDiscarded, null, false);
        }
    };

    prototype.MinMagnitude = function(a, b, ctx) {
        if (a == null) {
            throw new Error("a");
        }
        if (b == null) {
            throw new Error("b");
        }

        var result = this.MinMaxHandleSpecial(a, b, ctx, true, true);
        if (result != null) {
            return result;
        }
        var cmp = this.compareTo(this.AbsRaw(a), this.AbsRaw(b));
        if (cmp == 0) {
            return this.Min(a, b, ctx);
        }
        return (cmp < 0) ? this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx);
    };

    prototype.MaxMagnitude = function(a, b, ctx) {
        if (a == null) {
            throw new Error("a");
        }
        if (b == null) {
            throw new Error("b");
        }

        var result = this.MinMaxHandleSpecial(a, b, ctx, false, true);
        if (result != null) {
            return result;
        }
        var cmp = this.compareTo(this.AbsRaw(a), this.AbsRaw(b));
        if (cmp == 0) {
            return this.Max(a, b, ctx);
        }
        return (cmp > 0) ? this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx);
    };

    prototype.Max = function(a, b, ctx) {
        if (a == null) {
            throw new Error("a");
        }
        if (b == null) {
            throw new Error("b");
        }

        var result = this.MinMaxHandleSpecial(a, b, ctx, false, false);
        if (result != null) {
            return result;
        }
        var cmp = this.compareTo(a, b);
        if (cmp != 0) {
            return cmp < 0 ? this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx);
        }
        var flagNegA = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
        if (flagNegA != (this.helper.GetFlags(b) & BigNumberFlags.FlagNegative)) {
            return (flagNegA != 0) ? this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx);
        }
        if (flagNegA == 0) {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx);
        } else {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx);
        }
    };

    prototype.Min = function(a, b, ctx) {
        if (a == null) {
            throw new Error("a");
        }
        if (b == null) {
            throw new Error("b");
        }

        var result = this.MinMaxHandleSpecial(a, b, ctx, true, false);
        if (result != null) {
            return result;
        }
        var cmp = this.compareTo(a, b);
        if (cmp != 0) {
            return cmp > 0 ? this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx);
        }
        var signANeg = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
        if (signANeg != (this.helper.GetFlags(b) & BigNumberFlags.FlagNegative)) {
            return (signANeg != 0) ? this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx);
        }
        if (signANeg == 0) {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx);
        } else {
            return this.helper.GetExponent(a).compareTo(this.helper.GetExponent(b)) > 0 ? this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx);
        }
    };

    prototype.Multiply = function(thisValue, other, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, other, ctx);
            if (result != null) {
                return result;
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((otherFlags & BigNumberFlags.FlagSpecial) == 0 && this.helper.GetMantissa(other).signum() == 0) {
                    return this.SignalInvalid(ctx);
                }
                return this.EnsureSign(thisValue, ((thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative)) != 0);
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {

                if ((thisFlags & BigNumberFlags.FlagSpecial) == 0 && this.helper.GetMantissa(thisValue).signum() == 0) {
                    return this.SignalInvalid(ctx);
                }
                return this.EnsureSign(other, ((thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative)) != 0);
            }
        }
        var bigintOp2 = this.helper.GetExponent(other);
        var newexp = this.helper.GetExponent(thisValue).add(bigintOp2);
        var mantissaOp2 = this.helper.GetMantissa(other);

        thisFlags = (thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags & BigNumberFlags.FlagNegative);
        var ret = this.helper.CreateNewWithFlags(this.helper.GetMantissa(thisValue).multiply(mantissaOp2), newexp, thisFlags);
        if (ctx != null) {
            ret = this.RoundToPrecision(ret, ctx);
        }
        return ret;
    };

    prototype.MultiplyAndAdd = function(thisValue, multiplicand, augend, ctx) {
        var ctx2 = PrecisionContext.Unlimited.WithBlankFlags();
        var ret = this.MultiplyAddHandleSpecial(thisValue, multiplicand, augend, ctx);
        if (ret != null) {
            return ret;
        }
        ret = this.Add(this.Multiply(thisValue, multiplicand, ctx2), augend, ctx);
        if (ctx != null && ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (ctx2.getFlags()));
        }
        return ret;
    };

    prototype.RoundToBinaryPrecision = function(thisValue, context) {
        return this.RoundToBinaryPrecisionWithShift(thisValue, context, 0, 0, null, false);
    };
    prototype.RoundToBinaryPrecisionWithShift = function(thisValue, context, lastDiscarded, olderDiscarded, shift, adjustNegativeZero) {
        return this.RoundToPrecisionInternal(thisValue, lastDiscarded, olderDiscarded, shift, true, adjustNegativeZero, context);
    };

    prototype.Plus = function(thisValue, context) {
        return this.RoundToPrecisionInternal(thisValue, 0, 0, null, false, true, context);
    };

    prototype.RoundToPrecision = function(thisValue, context) {
        return this.RoundToPrecisionInternal(thisValue, 0, 0, null, false, false, context);
    };
    prototype.RoundToPrecisionWithShift = function(thisValue, context, lastDiscarded, olderDiscarded, shift, adjustNegativeZero) {
        return this.RoundToPrecisionInternal(thisValue, lastDiscarded, olderDiscarded, shift, false, adjustNegativeZero, context);
    };

    prototype.Quantize = function(thisValue, otherValue, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(otherValue);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, otherValue, ctx);
            if (result != null) {
                return result;
            }
            if (((thisFlags & otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
                return this.RoundToPrecision(thisValue, ctx);
            }
            if (((thisFlags | otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
                return this.SignalInvalid(ctx);
            }
        }
        var expOther = this.helper.GetExponent(otherValue);
        if (ctx != null && !ctx.ExponentWithinRange(expOther)) {

            return this.SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + expOther.toString());
        }
        var tmpctx = (ctx == null ? PrecisionContext.ForRounding(Rounding.HalfEven) : ctx.Copy()).WithBlankFlags();
        var mantThis = (this.helper.GetMantissa(thisValue)).abs();
        var expThis = this.helper.GetExponent(thisValue);
        var expcmp = expThis.compareTo(expOther);
        var negativeFlag = this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative;
        var ret = null;
        if (expcmp == 0) {

            ret = this.RoundToPrecision(thisValue, tmpctx);
        } else if (mantThis.signum() == 0) {

            ret = this.helper.CreateNewWithFlags(BigInteger.ZERO, expOther, negativeFlag);
            ret = this.RoundToPrecision(ret, tmpctx);
        } else if (expcmp > 0) {

            var radixPower = FastInteger.FromBig(expThis).SubtractBig(expOther);
            if (tmpctx.getPrecision().signum() > 0 && radixPower.compareTo(FastInteger.FromBig(tmpctx.getPrecision()).AddInt(10)) > 0) {

                return this.SignalInvalidWithMessage(ctx, "Result too high for current precision");
            }
            mantThis = this.helper.MultiplyByRadixPower(mantThis, radixPower);
            ret = this.helper.CreateNewWithFlags(mantThis, expOther, negativeFlag);
            ret = this.RoundToPrecision(ret, tmpctx);
        } else {

            var shift = FastInteger.FromBig(expOther).SubtractBig(expThis);
            ret = this.RoundToPrecisionWithShift(thisValue, tmpctx, 0, 0, shift, false);
        }
        if ((tmpctx.getFlags() & PrecisionContext.FlagOverflow) != 0) {

            return this.SignalInvalid(ctx);
        }
        if (ret == null || !this.helper.GetExponent(ret).equals(expOther)) {

            return this.SignalInvalid(ctx);
        }
        ret = this.EnsureSign(ret, negativeFlag != 0);
        if (ctx != null && ctx.getHasFlags()) {
            var flags = tmpctx.getFlags();
            flags &= ~PrecisionContext.FlagUnderflow;
            ctx.setFlags(ctx.getFlags() | (flags));
        }
        return ret;
    };

    prototype.RoundToExponentExact = function(thisValue, expOther, ctx) {
        if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
            return this.RoundToPrecision(thisValue, ctx);
        } else {
            var pctx = (ctx == null) ? null : ctx.WithPrecision(0).WithBlankFlags();
            var ret = this.Quantize(thisValue, this.helper.CreateNewWithFlags(BigInteger.ONE, expOther, 0), pctx);
            if (ctx != null && ctx.getHasFlags()) {
                ctx.setFlags(ctx.getFlags() | (pctx.getFlags()));
            }
            return ret;
        }
    };

    prototype.RoundToExponentSimple = function(thisValue, expOther, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, thisValue, ctx);
            if (result != null) {
                return result;
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                return thisValue;
            }
        }
        if (this.helper.GetExponent(thisValue).compareTo(expOther) >= 0) {
            return this.RoundToPrecision(thisValue, ctx);
        } else {
            if (ctx != null && !ctx.ExponentWithinRange(expOther)) {
                return this.SignalInvalidWithMessage(ctx, "Exponent not within exponent range: " + expOther.toString());
            }
            var bigmantissa = (this.helper.GetMantissa(thisValue)).abs();
            var shift = FastInteger.FromBig(expOther).SubtractBig(this.helper.GetExponent(thisValue));
            var accum = this.helper.CreateShiftAccumulator(bigmantissa);
            accum.ShiftRight(shift);
            bigmantissa = accum.getShiftedInt();
            thisValue = this.helper.CreateNewWithFlags(bigmantissa, expOther, thisFlags);
            return this.RoundToPrecisionWithShift(thisValue, ctx, accum.getLastDiscardedDigit(), accum.getOlderDiscardedDigits(), null, false);
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

    prototype.ReduceToPrecisionAndIdealExponent = function(thisValue, ctx, precision, idealExp) {
        var ret = this.RoundToPrecision(thisValue, ctx);
        if (ret != null && (this.helper.GetFlags(ret) & BigNumberFlags.FlagSpecial) == 0) {
            var bigmant = (this.helper.GetMantissa(ret)).abs();
            var exp = FastInteger.FromBig(this.helper.GetExponent(ret));
            if (bigmant.signum() == 0) {
                exp = new FastInteger(0);
            } else {
                var radix = this.thisRadix;
                var digits = (precision == null) ? null : this.helper.CreateShiftAccumulator(bigmant).GetDigitLength();
                var bigradix = BigInteger.valueOf(radix);
                var bitToTest = 0;
                var bitsToShift = new FastInteger(0);
                while (bigmant.signum() != 0) {
                    if (precision != null && digits.compareTo(precision) == 0) {
                        break;
                    }
                    if (idealExp != null && exp.compareTo(idealExp) == 0) {
                        break;
                    }
                    if (this.thisRadix == 2) {
                        if (bitToTest < 2147483647) {
                            if (bigmant.testBit(bitToTest)) {
                                break;
                            }
                            ++bitToTest;
                            bitsToShift.Increment();
                        } else {
                            if (bigmant.testBit(0)) {
                                break;
                            }
                            bigmant = bigmant.shiftRight(1);
                        }
                    } else {
                        var bigrem;
                        var bigquo;
                        {
                            var divrem = (bigmant).divideAndRemainder(bigradix);
                            bigquo = divrem[0];
                            bigrem = divrem[1];
                        }
                        if (bigrem.signum() != 0) {
                            break;
                        }
                        bigmant = bigquo;
                    }
                    exp.Increment();
                    if (digits != null) {
                        digits.Decrement();
                    }
                }
                if (this.thisRadix == 2 && !bitsToShift.isValueZero()) {
                    while (bitsToShift.CompareToInt(1000000) > 0) {
                        bigmant = bigmant.shiftRight(1000000);
                        bitsToShift.SubtractInt(1000000);
                    }
                    var tmpshift = bitsToShift.AsInt32();
                    bigmant = bigmant.shiftRight(tmpshift);
                }
            }
            var flags = this.helper.GetFlags(thisValue);
            ret = this.helper.CreateNewWithFlags(bigmant, exp.AsBigInteger(), flags);
            if (ctx != null && ctx.getClampNormalExponents()) {
                var ctxtmp = ctx.WithBlankFlags();
                ret = this.RoundToPrecision(ret, ctxtmp);
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (ctxtmp.getFlags() & ~PrecisionContext.FlagClamped));
                }
            }
            ret = this.EnsureSign(ret, (flags & BigNumberFlags.FlagNegative) != 0);
        }
        return ret;
    };

    prototype.Reduce = function(thisValue, ctx) {
        return this.ReduceToPrecisionAndIdealExponent(thisValue, ctx, null, null);
    };

    prototype.RoundToPrecisionInternal = function(thisValue, lastDiscarded, olderDiscarded, shift, binaryPrec, adjustNegativeZero, ctx) {
        if (ctx == null) {
            ctx = PrecisionContext.Unlimited.WithRounding(Rounding.HalfEven);
        }

        if (ctx.getPrecision().signum() == 0 && !ctx.getHasExponentRange() && (lastDiscarded | olderDiscarded) == 0 && (shift == null || shift.isValueZero())) {
            return thisValue;
        }
        var thisFlags = this.helper.GetFlags(thisValue);
        if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
            if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInvalid));
                }
                return this.ReturnQuietNaN(thisValue, ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
                return this.ReturnQuietNaN(thisValue, ctx);
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                return thisValue;
            }
        }

        var fastPrecision = ctx.getPrecision().canFitInInt() ? new FastInteger(ctx.getPrecision().intValue()) : FastInteger.FromBig(ctx.getPrecision());
        if (fastPrecision.signum() < 0) {
            return this.SignalInvalidWithMessage(ctx, "precision not greater or equal to 0 (" + fastPrecision + ")");
        }
        if (this.thisRadix == 2 || fastPrecision.isValueZero()) {

            binaryPrec = false;
        }
        var accum = null;
        var fastEMin = null;
        var fastEMax = null;

        if (ctx != null && ctx.getHasExponentRange()) {
            fastEMax = ctx.getEMax().canFitInInt() ? new FastInteger(ctx.getEMax().intValue()) : FastInteger.FromBig(ctx.getEMax());
            fastEMin = ctx.getEMin().canFitInInt() ? new FastInteger(ctx.getEMin().intValue()) : FastInteger.FromBig(ctx.getEMin());
        }
        var rounding = (ctx == null) ? Rounding.HalfEven : ctx.getRounding();
        var unlimitedPrec = fastPrecision.isValueZero();
        if (!binaryPrec) {

            if (fastPrecision.signum() > 0 && (shift == null || shift.isValueZero()) && (thisFlags & BigNumberFlags.FlagSpecial) == 0) {
                var mantabs = (this.helper.GetMantissa(thisValue)).abs();
                if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) != 0 && mantabs.signum() == 0 && (ctx.getRounding() != Rounding.Floor)) {

                    thisValue = this.EnsureSign(thisValue, false);
                    thisFlags = 0;
                }
                accum = this.helper.CreateShiftAccumulatorWithDigits(mantabs, lastDiscarded, olderDiscarded);
                var digitCount = accum.GetDigitLength();
                if (digitCount.compareTo(fastPrecision) <= 0) {
                    if (!this.RoundGivenDigits(lastDiscarded, olderDiscarded, ctx.getRounding(), (thisFlags & BigNumberFlags.FlagNegative) != 0, mantabs)) {
                        if (ctx.getHasFlags() && (lastDiscarded | olderDiscarded) != 0) {
                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
                        }
                        if (!ctx.getHasExponentRange()) {
                            return thisValue;
                        }
                        var bigexp = this.helper.GetExponent(thisValue);
                        var fastExp = bigexp.canFitInInt() ? new FastInteger(bigexp.intValue()) : FastInteger.FromBig(bigexp);
                        var fastAdjustedExp = FastInteger.Copy(fastExp).Add(fastPrecision).Decrement();
                        var fastNormalMin = FastInteger.Copy(fastEMin).Add(fastPrecision).Decrement();
                        if (fastAdjustedExp.compareTo(fastEMax) <= 0 && fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                            return thisValue;
                        }
                    } else {
                        if (ctx.getHasFlags() && (lastDiscarded | olderDiscarded) != 0) {
                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagInexact | PrecisionContext.FlagRounded));
                        }
                        var stillWithinPrecision = false;
                        mantabs = mantabs.add(BigInteger.ONE);
                        if (digitCount.compareTo(fastPrecision) < 0) {
                            stillWithinPrecision = true;
                        } else {
                            var radixPower = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                            stillWithinPrecision = mantabs.compareTo(radixPower) < 0;
                        }
                        if (stillWithinPrecision) {
                            if (!ctx.getHasExponentRange()) {
                                return this.helper.CreateNewWithFlags(mantabs, this.helper.GetExponent(thisValue), thisFlags);
                            }
                            var bigexp = this.helper.GetExponent(thisValue);
                            var fastExp = bigexp.canFitInInt() ? new FastInteger(bigexp.intValue()) : FastInteger.FromBig(bigexp);
                            var fastAdjustedExp = FastInteger.Copy(fastExp).Add(fastPrecision).Decrement();
                            var fastNormalMin = FastInteger.Copy(fastEMin).Add(fastPrecision).Decrement();
                            if (fastAdjustedExp.compareTo(fastEMax) <= 0 && fastAdjustedExp.compareTo(fastNormalMin) >= 0) {
                                return this.helper.CreateNewWithFlags(mantabs, bigexp, thisFlags);
                            }
                        }
                    }
                }
            }
        }
        if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) != 0 && this.helper.GetMantissa(thisValue).signum() == 0 && (rounding != Rounding.Floor)) {

            thisValue = this.EnsureSign(thisValue, false);
            thisFlags = 0;
        }
        var neg = (thisFlags & BigNumberFlags.FlagNegative) != 0;
        var bigmantissa = (this.helper.GetMantissa(thisValue)).abs();

        var oldmantissa = bigmantissa;
        var mantissaWasZero = oldmantissa.signum() == 0 && (lastDiscarded | olderDiscarded) == 0;
        var maxMantissa = BigInteger.ONE;
        var exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        var flags = 0;
        if (accum == null) {
            accum = this.helper.CreateShiftAccumulatorWithDigits(bigmantissa, lastDiscarded, olderDiscarded);
        }
        if (binaryPrec) {
            var prec = FastInteger.Copy(fastPrecision);
            while (prec.signum() > 0) {
                var bitShift = (prec.CompareToInt(1000000) >= 0) ? 1000000 : prec.AsInt32();
                maxMantissa = maxMantissa.shiftLeft(bitShift);
                prec.SubtractInt(bitShift);
            }
            maxMantissa = maxMantissa.subtract(BigInteger.ONE);
            var accumMaxMant = this.helper.CreateShiftAccumulator(maxMantissa);

            fastPrecision = accumMaxMant.GetDigitLength();
        }
        if (shift != null && shift.signum() != 0) {
            accum.ShiftRight(shift);
        }
        if (!unlimitedPrec) {
            accum.ShiftToDigits(fastPrecision);
        } else {
            fastPrecision = accum.GetDigitLength();
        }
        if (binaryPrec) {
            while (accum.getShiftedInt().compareTo(maxMantissa) > 0) {
                accum.ShiftRightInt(1);
            }
        }
        var discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
        exp.Add(discardedBits);
        var adjExponent = FastInteger.Copy(exp).Add(accum.GetDigitLength()).Decrement();

        var newAdjExponent = adjExponent;
        var clamp = null;
        var earlyRounded = BigInteger.ZERO;
        if (binaryPrec && fastEMax != null && adjExponent.compareTo(fastEMax) == 0) {

            var expdiff = FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
            var currMantissa = accum.getShiftedInt();
            currMantissa = this.helper.MultiplyByRadixPower(currMantissa, expdiff);
            if (currMantissa.compareTo(maxMantissa) > 0) {

                adjExponent.Increment();
            }
        }

        if (ctx.getHasFlags() && fastEMin != null && !unlimitedPrec && adjExponent.compareTo(fastEMin) < 0) {
            earlyRounded = accum.getShiftedInt();
            if (this.RoundGivenBigInt(accum, rounding, neg, earlyRounded)) {
                earlyRounded = earlyRounded.add(BigInteger.ONE);
                if (earlyRounded.testBit(0) == false || (this.thisRadix & 1) != 0) {
                    var accum2 = this.helper.CreateShiftAccumulator(earlyRounded);
                    var newDigitLength = accum2.GetDigitLength();

                    if (binaryPrec || newDigitLength.compareTo(fastPrecision) > 0) {
                        newDigitLength = FastInteger.Copy(fastPrecision);
                    }
                    newAdjExponent = FastInteger.Copy(exp).Add(newDigitLength).Decrement();
                }
            }
        }
        if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
            if (mantissaWasZero) {
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (flags | PrecisionContext.FlagClamped));
                }
                if (ctx.getClampNormalExponents()) {

                    if (binaryPrec && this.thisRadix != 2) {
                        fastPrecision = this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
                    }
                    var clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
                    if (fastEMax.compareTo(clampExp) > 0) {
                        if (ctx.getHasFlags()) {
                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
                        }
                        fastEMax = clampExp;
                    }
                }
                return this.helper.CreateNewWithFlags(oldmantissa, fastEMax.AsBigInteger(), thisFlags);
            } else {

                flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (rounding == Rounding.Unnecessary) {
                    return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                }
                if (!unlimitedPrec && (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg))) {

                    var overflowMant = BigInteger.ZERO;
                    if (binaryPrec) {
                        overflowMant = maxMantissa;
                    } else {
                        overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                        overflowMant = overflowMant.subtract(BigInteger.ONE);
                    }
                    if (ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (flags));
                    }
                    clamp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
                    return this.helper.CreateNewWithFlags(overflowMant, clamp.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
                }
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (flags));
                }
                return this.SignalOverflow(neg);
            }
        } else if (fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {

            var fastETiny = FastInteger.Copy(fastEMin).Subtract(fastPrecision).Increment();
            if (ctx.getHasFlags()) {
                if (earlyRounded.signum() != 0) {
                    if (newAdjExponent.compareTo(fastEMin) < 0) {
                        flags |= PrecisionContext.FlagSubnormal;
                    }
                }
            }

            var subExp = FastInteger.Copy(exp);

            if (subExp.compareTo(fastETiny) < 0) {

                var expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
                expdiff.Add(discardedBits);
                accum = this.helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
                accum.ShiftRight(expdiff);
                var newmantissa = accum.getShiftedIntFast();
                if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (rounding == Rounding.Unnecessary) {
                        return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                    }
                }
                if (accum.getDiscardedDigitCount().signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (ctx.getHasFlags()) {
                        if (!mantissaWasZero) {
                            flags |= PrecisionContext.FlagRounded;
                        }
                        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                            flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                        }
                    }
                    if (this.Round(accum, rounding, neg, newmantissa)) {
                        newmantissa.Increment();
                    }
                }
                if (ctx.getHasFlags()) {
                    if (newmantissa.isValueZero()) {
                        flags |= PrecisionContext.FlagClamped;
                    }
                    if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) {
                        flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
                    }
                    ctx.setFlags(ctx.getFlags() | (flags));
                }
                bigmantissa = newmantissa.AsBigInteger();
                if (ctx.getClampNormalExponents()) {

                    if (binaryPrec && this.thisRadix != 2) {
                        fastPrecision = this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
                    }
                    var clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
                    if (fastETiny.compareTo(clampExp) > 0) {
                        if (bigmantissa.signum() != 0) {
                            expdiff = FastInteger.Copy(fastETiny).Subtract(clampExp);
                            bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, expdiff);
                        }
                        if (ctx.getHasFlags()) {
                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
                        }
                        fastETiny = clampExp;
                    }
                }
                return this.helper.CreateNewWithFlags(newmantissa.AsBigInteger(), fastETiny.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
            }
        }
        var recheckOverflow = false;
        if (accum.getDiscardedDigitCount().signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (bigmantissa.signum() != 0) {
                flags |= PrecisionContext.FlagRounded;
            }
            bigmantissa = accum.getShiftedInt();
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (rounding == Rounding.Unnecessary) {
                    return this.SignalInvalidWithMessage(ctx, "Rounding was required");
                }
            }
            if (this.RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
                var oldDigitLength = accum.GetDigitLength();
                bigmantissa = bigmantissa.add(BigInteger.ONE);
                if (binaryPrec) {
                    recheckOverflow = true;
                }

                if (!unlimitedPrec && (bigmantissa.testBit(0) == false || (this.thisRadix & 1) != 0) && (binaryPrec || oldDigitLength.compareTo(fastPrecision) >= 0)) {
                    accum = this.helper.CreateShiftAccumulator(bigmantissa);
                    var newDigitLength = accum.GetDigitLength();
                    if (binaryPrec || newDigitLength.compareTo(fastPrecision) > 0) {
                        var neededShift = FastInteger.Copy(newDigitLength).Subtract(fastPrecision);
                        accum.ShiftRight(neededShift);
                        if (binaryPrec) {
                            while (accum.getShiftedInt().compareTo(maxMantissa) > 0) {
                                accum.ShiftRightInt(1);
                            }
                        }
                        if (accum.getDiscardedDigitCount().signum() != 0) {
                            exp.Add(accum.getDiscardedDigitCount());
                            discardedBits.Add(accum.getDiscardedDigitCount());
                            bigmantissa = accum.getShiftedInt();
                            if (!binaryPrec) {
                                recheckOverflow = true;
                            }
                        }
                    }
                }
            }
        }
        if (recheckOverflow && fastEMax != null) {

            adjExponent = FastInteger.Copy(exp);
            adjExponent.Add(accum.GetDigitLength()).Decrement();
            if (binaryPrec && fastEMax != null && adjExponent.compareTo(fastEMax) == 0) {

                var expdiff = FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
                var currMantissa = accum.getShiftedInt();
                currMantissa = this.helper.MultiplyByRadixPower(currMantissa, expdiff);
                if (currMantissa.compareTo(maxMantissa) > 0) {

                    adjExponent.Increment();
                }
            }
            if (adjExponent.compareTo(fastEMax) > 0) {
                flags |= PrecisionContext.FlagOverflow | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                if (!unlimitedPrec && (rounding == Rounding.Down || rounding == Rounding.ZeroFiveUp || (rounding == Rounding.Ceiling && neg) || (rounding == Rounding.Floor && !neg))) {

                    var overflowMant = BigInteger.ZERO;
                    if (binaryPrec) {
                        overflowMant = maxMantissa;
                    } else {
                        overflowMant = this.helper.MultiplyByRadixPower(BigInteger.ONE, fastPrecision);
                        overflowMant = overflowMant.subtract(BigInteger.ONE);
                    }
                    if (ctx.getHasFlags()) {
                        ctx.setFlags(ctx.getFlags() | (flags));
                    }
                    clamp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
                    return this.helper.CreateNewWithFlags(overflowMant, clamp.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
                }
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (flags));
                }
                return this.SignalOverflow(neg);
            }
        }
        if (ctx.getHasFlags()) {
            ctx.setFlags(ctx.getFlags() | (flags));
        }
        if (ctx.getClampNormalExponents()) {

            if (binaryPrec && this.thisRadix != 2) {
                fastPrecision = this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
            }
            var clampExp = FastInteger.Copy(fastEMax).Increment().Subtract(fastPrecision);
            if (exp.compareTo(clampExp) > 0) {
                if (bigmantissa.signum() != 0) {
                    var expdiff = FastInteger.Copy(exp).Subtract(clampExp);
                    bigmantissa = this.helper.MultiplyByRadixPower(bigmantissa, expdiff);
                }
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagClamped));
                }
                exp = clampExp;
            }
        }
        return this.helper.CreateNewWithFlags(bigmantissa, exp.AsBigInteger(), neg ? BigNumberFlags.FlagNegative : 0);
    };

    prototype.AddCore = function(mant1, mant2, exponent, flags1, flags2, ctx) {
        var neg1 = (flags1 & BigNumberFlags.FlagNegative) != 0;
        var neg2 = (flags2 & BigNumberFlags.FlagNegative) != 0;
        var negResult = false;

        if (neg1 != neg2) {

            mant1 = mant1.subtract(mant2);
            var mant1Sign = mant1.signum();
            negResult = neg1 ^ (mant1Sign == 0 ? neg2 : (mant1Sign < 0));
        } else {

            mant1 = mant1.add(mant2);
            negResult = neg1;
        }
        if (mant1.signum() == 0 && negResult) {

            if (!((neg1 && neg2) || ((neg1 ^ neg2) && ctx != null && ctx.getRounding() == Rounding.Floor))) {
                negResult = false;
            }
        }

        return this.helper.CreateNewWithFlags(mant1, exponent, negResult ? BigNumberFlags.FlagNegative : 0);
    };

    prototype.Add = function(thisValue, other, ctx) {
        var thisFlags = this.helper.GetFlags(thisValue);
        var otherFlags = this.helper.GetFlags(other);
        if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
            var result = this.HandleNotANumber(thisValue, other, ctx);
            if (result != null) {
                return result;
            }
            if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
                if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
                    if ((thisFlags & BigNumberFlags.FlagNegative) != (otherFlags & BigNumberFlags.FlagNegative)) {
                        return this.SignalInvalid(ctx);
                    }
                }
                return thisValue;
            }
            if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
                return other;
            }
        }
        var expcmp = this.helper.GetExponent(thisValue).compareTo(this.helper.GetExponent(other));
        var retval = null;
        var op1MantAbs = (this.helper.GetMantissa(thisValue)).abs();
        var op2MantAbs = (this.helper.GetMantissa(other)).abs();
        if (expcmp == 0) {
            retval = this.AddCore(op1MantAbs, op2MantAbs, this.helper.GetExponent(thisValue), thisFlags, otherFlags, ctx);
        } else {

            var op1 = thisValue;
            var op2 = other;
            var op1Exponent = this.helper.GetExponent(op1);
            var op2Exponent = this.helper.GetExponent(op2);
            var resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
            var fastOp1Exp = FastInteger.FromBig(op1Exponent);
            var fastOp2Exp = FastInteger.FromBig(op2Exponent);
            var expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
            if (ctx != null && ctx.getPrecision().signum() > 0) {

                var fastPrecision = FastInteger.FromBig(ctx.getPrecision());

                if (FastInteger.Copy(expdiff).compareTo(fastPrecision) > 0) {
                    var expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
                    if (expcmp2 < 0) {
                        if (op2MantAbs.signum() != 0) {

                            var digitLength1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                            if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2).compareTo(fastOp2Exp) < 0) {

                                var tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(4).Subtract(digitLength1).SubtractBig(ctx.getPrecision());
                                var newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                                if (newDiff.compareTo(expdiff) < 0) {

                                    var sameSign = this.helper.GetSign(thisValue) == this.helper.GetSign(other);
                                    var oneOpIsZero = op1MantAbs.signum() == 0;
                                    var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                                    if (digitLength2.compareTo(fastPrecision) < 0) {

                                        var precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                                        if (!oneOpIsZero && !sameSign) {
                                            precisionDiff.AddInt(2);
                                        }
                                        op2MantAbs = this.helper.MultiplyByRadixPower(op2MantAbs, precisionDiff);
                                        var bigintTemp = precisionDiff.AsBigInteger();
                                        op2Exponent = op2Exponent.subtract(bigintTemp);
                                        if (!oneOpIsZero && !sameSign) {
                                            op2MantAbs = op2MantAbs.subtract(BigInteger.ONE);
                                        }
                                        other = this.helper.CreateNewWithFlags(op2MantAbs, op2Exponent, this.helper.GetFlags(other));
                                        var shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                        if (oneOpIsZero && ctx != null && ctx.getHasFlags()) {
                                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                                        }
                                        return this.RoundToPrecisionWithShift(other, ctx, (oneOpIsZero || sameSign) ? 0 : 1, (oneOpIsZero && !sameSign) ? 0 : 1, shift, false);
                                    } else {
                                        if (!oneOpIsZero && !sameSign) {
                                            op2MantAbs = this.helper.MultiplyByRadixPower(op2MantAbs, new FastInteger(2));
                                            op2Exponent = op2Exponent.subtract(BigInteger.valueOf(2));
                                            op2MantAbs = op2MantAbs.subtract(BigInteger.ONE);
                                            other = this.helper.CreateNewWithFlags(op2MantAbs, op2Exponent, this.helper.GetFlags(other));
                                            var shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                            return this.RoundToPrecisionWithShift(other, ctx, 0, 0, shift, false);
                                        } else {
                                            var shift2 = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                            if (!sameSign && ctx != null && ctx.getHasFlags()) {
                                                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                                            }
                                            return this.RoundToPrecisionWithShift(other, ctx, 0, sameSign ? 1 : 0, shift2, false);
                                        }
                                    }
                                }
                            }
                        }
                    } else if (expcmp2 > 0) {
                        if (op1MantAbs.signum() != 0) {

                            var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                            if (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2).compareTo(fastOp1Exp) < 0) {

                                var tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(4).Subtract(digitLength2).SubtractBig(ctx.getPrecision());
                                var newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                                if (newDiff.compareTo(expdiff) < 0) {

                                    var sameSign = this.helper.GetSign(thisValue) == this.helper.GetSign(other);
                                    var oneOpIsZero = op2MantAbs.signum() == 0;
                                    digitLength2 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                                    if (digitLength2.compareTo(fastPrecision) < 0) {

                                        var precisionDiff = FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                                        if (!oneOpIsZero && !sameSign) {
                                            precisionDiff.AddInt(2);
                                        }
                                        op1MantAbs = this.helper.MultiplyByRadixPower(op1MantAbs, precisionDiff);
                                        var bigintTemp = precisionDiff.AsBigInteger();
                                        op1Exponent = op1Exponent.subtract(bigintTemp);
                                        if (!oneOpIsZero && !sameSign) {
                                            op1MantAbs = op1MantAbs.subtract(BigInteger.ONE);
                                        }
                                        thisValue = this.helper.CreateNewWithFlags(op1MantAbs, op1Exponent, this.helper.GetFlags(thisValue));
                                        var shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                        if (oneOpIsZero && ctx != null && ctx.getHasFlags()) {
                                            ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                                        }
                                        return this.RoundToPrecisionWithShift(thisValue, ctx, (oneOpIsZero || sameSign) ? 0 : 1, (oneOpIsZero && !sameSign) ? 0 : 1, shift, false);
                                    } else {
                                        if (!oneOpIsZero && !sameSign) {
                                            op1MantAbs = this.helper.MultiplyByRadixPower(op1MantAbs, new FastInteger(2));
                                            op1Exponent = op1Exponent.subtract(BigInteger.valueOf(2));
                                            op1MantAbs = op1MantAbs.subtract(BigInteger.ONE);
                                            thisValue = this.helper.CreateNewWithFlags(op1MantAbs, op1Exponent, this.helper.GetFlags(thisValue));
                                            var shift = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                            return this.RoundToPrecisionWithShift(thisValue, ctx, 0, 0, shift, false);
                                        } else {
                                            var shift2 = FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                                            if (!sameSign && ctx != null && ctx.getHasFlags()) {
                                                ctx.setFlags(ctx.getFlags() | (PrecisionContext.FlagRounded));
                                            }
                                            return this.RoundToPrecisionWithShift(thisValue, ctx, 0, sameSign ? 1 : 0, shift2, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    expcmp = op1Exponent.compareTo(op2Exponent);
                    resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
                }
            }
            if (expcmp > 0) {

                op1MantAbs = this.RescaleByExponentDiff(op1MantAbs, op1Exponent, op2Exponent);

                retval = this.AddCore(op1MantAbs, op2MantAbs, resultExponent, thisFlags, otherFlags, ctx);
            } else {

                op2MantAbs = this.RescaleByExponentDiff(op2MantAbs, op1Exponent, op2Exponent);

                retval = this.AddCore(op1MantAbs, op2MantAbs, resultExponent, thisFlags, otherFlags, ctx);
            }
        }
        if (ctx != null) {
            retval = this.RoundToPrecision(retval, ctx);
        }
        return retval;
    };

    prototype.CompareToWithContext = function(thisValue, numberObject, treatQuietNansAsSignaling, ctx) {
        if (numberObject == null) {
            return this.SignalInvalid(ctx);
        }
        var result = this.CompareToHandleSpecial(thisValue, numberObject, treatQuietNansAsSignaling, ctx);
        if (result != null) {
            return result;
        }
        return this.ValueOf(this.compareTo(thisValue, numberObject), null);
    };

    prototype.compareTo = function(thisValue, numberObject) {
        if (numberObject == null) {
            return 1;
        }
        var flagsThis = this.helper.GetFlags(thisValue);
        var flagsOther = this.helper.GetFlags(numberObject);
        if ((flagsThis & BigNumberFlags.FlagNaN) != 0) {
            if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
                return 0;
            }
            return 1;
        }

        if ((flagsOther & BigNumberFlags.FlagNaN) != 0) {
            return -1;
        }

        var s = this.CompareToHandleSpecialReturnInt(thisValue, numberObject);
        if (s <= 1) {
            return s;
        }
        s = this.helper.GetSign(thisValue);
        var ds = this.helper.GetSign(numberObject);
        if (s != ds) {
            return (s < ds) ? -1 : 1;
        }
        if (ds == 0 || s == 0) {

            return 0;
        }
        var expcmp = this.helper.GetExponent(thisValue).compareTo(this.helper.GetExponent(numberObject));

        var mantcmp = (this.helper.GetMantissa(thisValue)).abs().compareTo((this.helper.GetMantissa(numberObject)).abs());
        if (s < 0) {
            mantcmp = -mantcmp;
        }
        if (mantcmp == 0) {

            return s < 0 ? -expcmp : expcmp;
        }
        if (expcmp == 0) {
            return mantcmp;
        }
        var op1Exponent = this.helper.GetExponent(thisValue);
        var op2Exponent = this.helper.GetExponent(numberObject);
        var fastOp1Exp = FastInteger.FromBig(op1Exponent);
        var fastOp2Exp = FastInteger.FromBig(op2Exponent);
        var expdiff = FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();

        if (expdiff.CompareToInt(100) >= 0) {
            var op1MantAbs = (this.helper.GetMantissa(thisValue)).abs();
            var op2MantAbs = (this.helper.GetMantissa(numberObject)).abs();
            var precision1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
            var precision2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
            var maxPrecision = null;
            if (precision1.compareTo(precision2) > 0) {
                maxPrecision = precision1;
            } else {
                maxPrecision = precision2;
            }

            if (FastInteger.Copy(expdiff).compareTo(maxPrecision) > 0) {
                var expcmp2 = fastOp1Exp.compareTo(fastOp2Exp);
                if (expcmp2 < 0) {
                    if (op2MantAbs.signum() != 0) {

                        var digitLength1 = this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
                        if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2).compareTo(fastOp2Exp) < 0) {

                            var tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(8).Subtract(digitLength1).Subtract(maxPrecision);
                            var newDiff = FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                            if (newDiff.compareTo(expdiff) < 0) {
                                if (s == ds) {
                                    return (s < 0) ? 1 : -1;
                                } else {
                                    op1Exponent = tmp.AsBigInteger();
                                }
                            }
                        }
                    }
                } else if (expcmp2 > 0) {
                    if (op1MantAbs.signum() != 0) {

                        var digitLength2 = this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
                        if (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2).compareTo(fastOp1Exp) < 0) {

                            var tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(8).Subtract(digitLength2).Subtract(maxPrecision);
                            var newDiff = FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                            if (newDiff.compareTo(expdiff) < 0) {
                                if (s == ds) {
                                    return (s < 0) ? -1 : 1;
                                } else {
                                    op2Exponent = tmp.AsBigInteger();
                                }
                            }
                        }
                    }
                }
                expcmp = op1Exponent.compareTo(op2Exponent);
            }
        }
        if (expcmp > 0) {
            var newmant = this.RescaleByExponentDiff(this.helper.GetMantissa(thisValue), op1Exponent, op2Exponent);
            var othermant = (this.helper.GetMantissa(numberObject)).abs();
            newmant = (newmant).abs();
            mantcmp = newmant.compareTo(othermant);
            return (s < 0) ? -mantcmp : mantcmp;
        } else {
            var newmant = this.RescaleByExponentDiff(this.helper.GetMantissa(numberObject), op1Exponent, op2Exponent);
            var othermant = (this.helper.GetMantissa(thisValue)).abs();
            newmant = (newmant).abs();
            mantcmp = othermant.compareTo(newmant);
            return (s < 0) ? -mantcmp : mantcmp;
        }
    };
})(RadixMath,RadixMath.prototype);

var TrapException =

function(flag, ctx, result) {
    RuntimeException.call(this, TrapException.FlagToMessage(flag));
    this.error = flag;
    this.ctx = (ctx == null) ? null : ctx.Copy();
    this.result = result;
};
(function(constructor,prototype){
    constructor['serialVersionUID'] = constructor.serialVersionUID = 1;
    prototype['result'] = prototype.result = null;
    prototype['ctx'] = prototype.ctx = null;
    prototype['getContext'] = prototype.getContext = function() {
        return this.ctx;
    };
    prototype['error'] = prototype.error = null;
    prototype['getResult'] = prototype.getResult = function() {
        return this.result;
    };
    prototype['getError'] = prototype.getError = function() {
        return this.error;
    };
    constructor['FlagToMessage'] = constructor.FlagToMessage = function(flag) {
        if (flag == PrecisionContext.FlagClamped) {
            return "Clamped";
        } else if (flag == PrecisionContext.FlagDivideByZero) {
            return "DivideByZero";
        } else if (flag == PrecisionContext.FlagInexact) {
            return "Inexact";
        } else if (flag == PrecisionContext.FlagInvalid) {
            return "Invalid";
        } else if (flag == PrecisionContext.FlagOverflow) {
            return "Overflow";
        } else if (flag == PrecisionContext.FlagRounded) {
            return "Rounded";
        } else if (flag == PrecisionContext.FlagSubnormal) {
            return "Subnormal";
        } else if (flag == PrecisionContext.FlagUnderflow) {
            return "Underflow";
        }
        return "Trap";
    };
})(TrapException,TrapException.prototype);

if(typeof exports!=="undefined")exports['TrapException']=TrapException;
if(typeof window!=="undefined")window['TrapException']=TrapException;

var TrappableRadixMath = function(math) {

    this.math = math;
};
(function(constructor,prototype){
    constructor.GetTrappableContext = function(ctx) {
        if (ctx == null) {
            return null;
        }
        if (ctx.getTraps() == 0) {
            return ctx;
        }
        return ctx.WithBlankFlags();
    };
    prototype.TriggerTraps = function(result, src, dst) {
        if (src == null || src.getFlags() == 0) {
            return result;
        }
        if (dst != null && dst.getHasFlags()) {
            dst.setFlags(dst.getFlags() | (src.getFlags()));
        }
        var traps = (dst != null) ? dst.getTraps() : 0;
        traps &= src.getFlags();
        if (traps == 0) {
            return result;
        }
        var mutexConditions = traps & (~(PrecisionContext.FlagClamped | PrecisionContext.FlagInexact | PrecisionContext.FlagRounded | PrecisionContext.FlagSubnormal));
        if (mutexConditions != 0) {
            for (var i = 0; i < 32; ++i) {
                var flag = mutexConditions & (i << 1);
                if (flag != 0) {
                    throw new TrapException(flag, dst, result);
                }
            }
        }
        if ((traps & PrecisionContext.FlagSubnormal) != 0) {
            throw new TrapException(traps & PrecisionContext.FlagSubnormal, dst, result);
        }
        if ((traps & PrecisionContext.FlagInexact) != 0) {
            throw new TrapException(traps & PrecisionContext.FlagInexact, dst, result);
        }
        if ((traps & PrecisionContext.FlagRounded) != 0) {
            throw new TrapException(traps & PrecisionContext.FlagRounded, dst, result);
        }
        if ((traps & PrecisionContext.FlagClamped) != 0) {
            throw new TrapException(traps & PrecisionContext.FlagClamped, dst, result);
        }
        return result;
    };
    prototype.math = null;

    prototype.DivideToIntegerNaturalScale = function(thisValue, divisor, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.DivideToIntegerNaturalScale(thisValue, divisor, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.DivideToIntegerZeroScale = function(thisValue, divisor, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.DivideToIntegerZeroScale(thisValue, divisor, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Abs = function(value, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Abs(value, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Negate = function(value, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Negate(value, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Remainder = function(thisValue, divisor, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Remainder(thisValue, divisor, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RemainderNear = function(thisValue, divisor, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RemainderNear(thisValue, divisor, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Pi = function(ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Pi(tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Power = function(thisValue, pow, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Power(thisValue, pow, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Log10 = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Log10(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Ln = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Ln(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Exp = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Exp(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.SquareRoot = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.SquareRoot(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.NextMinus = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.NextMinus(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.NextToward = function(thisValue, otherValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.NextToward(thisValue, otherValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.NextPlus = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.NextPlus(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.DivideToExponent = function(thisValue, divisor, desiredExponent, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.DivideToExponent(thisValue, divisor, desiredExponent, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Divide = function(thisValue, divisor, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Divide(thisValue, divisor, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.MinMagnitude = function(a, b, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.MinMagnitude(a, b, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.MaxMagnitude = function(a, b, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.MaxMagnitude(a, b, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Max = function(a, b, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Max(a, b, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Min = function(a, b, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Min(a, b, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Multiply = function(thisValue, other, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Multiply(thisValue, other, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.MultiplyAndAdd = function(thisValue, multiplicand, augend, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.MultiplyAndAdd(thisValue, multiplicand, augend, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RoundToBinaryPrecision = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RoundToBinaryPrecision(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Plus = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Plus(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RoundToPrecision = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RoundToPrecision(thisValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Quantize = function(thisValue, otherValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Quantize(thisValue, otherValue, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RoundToExponentExact = function(thisValue, expOther, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RoundToExponentExact(thisValue, expOther, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RoundToExponentSimple = function(thisValue, expOther, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RoundToExponentSimple(thisValue, expOther, ctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.RoundToExponentNoRoundedFlag = function(thisValue, exponent, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.RoundToExponentNoRoundedFlag(thisValue, exponent, ctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Reduce = function(thisValue, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Reduce(thisValue, ctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.Add = function(thisValue, other, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.Add(thisValue, other, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.CompareToWithContext = function(thisValue, numberObject, treatQuietNansAsSignaling, ctx) {
        var tctx = TrappableRadixMath.GetTrappableContext(ctx);
        var result = this.math.CompareToWithContext(thisValue, numberObject, treatQuietNansAsSignaling, tctx);
        return this.TriggerTraps(result, tctx, ctx);
    };

    prototype.compareTo = function(thisValue, numberObject) {
        return this.math.compareTo(thisValue, numberObject);
    };
})(TrappableRadixMath,TrappableRadixMath.prototype);

var ExtendedDecimal =

function() {

};
(function(constructor,prototype){
    constructor['MaxSafeInt'] = constructor.MaxSafeInt = 214748363;
    prototype['exponent'] = prototype.exponent = null;
    prototype['unsignedMantissa'] = prototype.unsignedMantissa = null;
    prototype['flags'] = prototype.flags = null;
    prototype['getExponent'] = prototype.getExponent = function() {
        return this.exponent;
    };
    prototype['getUnsignedMantissa'] = prototype.getUnsignedMantissa = function() {
        return this.unsignedMantissa;
    };
    prototype['getMantissa'] = prototype.getMantissa = function() {
        return this.isNegative() ? ((this.unsignedMantissa).negate()) : this.unsignedMantissa;
    };
    prototype['EqualsInternal'] = prototype.EqualsInternal = function(otherValue) {
        if (otherValue == null) {
            return false;
        }
        return this.flags == otherValue.flags && this.unsignedMantissa.equals(otherValue.unsignedMantissa) && this.exponent.equals(otherValue.exponent);
    };
    prototype['equals'] = prototype.equals = function(obj) {
        return this.EqualsInternal((obj.constructor==ExtendedDecimal) ? obj : null);
    };
    prototype['hashCode'] = prototype.hashCode = function() {
        var hashCode_ = 0;
        {
            hashCode_ = hashCode_ + (1000000007 * this.exponent.hashCode());
            hashCode_ = hashCode_ + (1000000009 * this.unsignedMantissa.hashCode());
            hashCode_ = hashCode_ + (1000000009 * this.flags);
        }
        return hashCode_;
    };
    constructor['Create'] = constructor.Create = function(mantissa, exponent) {
        if (mantissa == null) {
            throw new Error("mantissa");
        }
        if (exponent == null) {
            throw new Error("exponent");
        }
        var ex = new ExtendedDecimal();
        ex.exponent = exponent;
        var sign = mantissa == null ? 0 : mantissa.signum();
        ex.unsignedMantissa = sign < 0 ? ((mantissa).negate()) : mantissa;
        ex.flags = (sign < 0) ? BigNumberFlags.FlagNegative : 0;
        return ex;
    };
    constructor['CreateWithFlags'] = constructor.CreateWithFlags = function(mantissa, exponent, flags) {
        var ext = ExtendedDecimal.Create(mantissa, exponent);
        ext.flags = flags;
        return ext;
    };

    constructor['FromString'] = constructor.FromString = function(str, ctx) {
        if (str == null) {
            throw new Error("str");
        }
        if (str.length == 0) {
            throw new Error();
        }
        var offset = 0;
        var negative = false;
        if (str.charAt(0) == '+' || str.charAt(0) == '-') {
            negative = str.charAt(0) == '-';
            ++offset;
        }
        var mantInt = 0;
        var mant = null;
        var mantBuffer = 0;
        var mantBufferMult = 1;
        var expBuffer = 0;
        var expBufferMult = 1;
        var haveDecimalPoint = false;
        var haveDigits = false;
        var haveExponent = false;
        var newScaleInt = 0;
        var newScale = null;
        var i = offset;
        if (i + 8 == str.length) {
            if ((str.charAt(i) == 'I' || str.charAt(i) == 'i') && (str.charAt(i + 1) == 'N' || str.charAt(i + 1) == 'n') && (str.charAt(i + 2) == 'F' || str.charAt(i + 2) == 'f') && (str.charAt(i + 3) == 'I' || str.charAt(i + 3) == 'i') && (str.charAt(i + 4) == 'N' || str.charAt(i + 4) == 'n') && (str.charAt(i + 5) == 'I' || str.charAt(i + 5) == 'i') && (str.charAt(i + 6) == 'T' || str.charAt(i + 6) == 't') && (str.charAt(i + 7) == 'Y' || str.charAt(i + 7) == 'y')) {
                return negative ? ExtendedDecimal.NegativeInfinity : ExtendedDecimal.PositiveInfinity;
            }
        }
        if (i + 3 == str.length) {
            if ((str.charAt(i) == 'I' || str.charAt(i) == 'i') && (str.charAt(i + 1) == 'N' || str.charAt(i + 1) == 'n') && (str.charAt(i + 2) == 'F' || str.charAt(i + 2) == 'f')) {
                return negative ? ExtendedDecimal.NegativeInfinity : ExtendedDecimal.PositiveInfinity;
            }
        }
        if (i + 3 <= str.length) {

            if ((str.charAt(i) == 'N' || str.charAt(i) == 'n') && (str.charAt(i + 1) == 'A' || str.charAt(i + 1) == 'a') && (str.charAt(i + 2) == 'N' || str.charAt(i + 2) == 'n')) {
                if (i + 3 == str.length) {
                    if (!negative) {
                        return ExtendedDecimal.NaN;
                    }
                    return ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagQuietNaN);
                }
                i = i + (3);
                var digitCount = new FastInteger(0);
                var maxDigits = null;
                haveDigits = false;
                if (ctx != null && ctx.getPrecision().signum() != 0) {
                    maxDigits = FastInteger.FromBig(ctx.getPrecision());
                    if (ctx.getClampNormalExponents()) {
                        maxDigits.Decrement();
                    }
                }
                for (; i < str.length; ++i) {
                    if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                        var thisdigit = ((str.charCodeAt(i)-48)|0);
                        haveDigits = haveDigits || thisdigit != 0;
                        if (mantInt > ExtendedDecimal.MaxSafeInt) {
                            if (mant == null) {
                                mant = new FastInteger(mantInt);
                                mantBuffer = thisdigit;
                                mantBufferMult = 10;
                            } else {
                                if (mantBufferMult >= 1000000000) {
                                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                                    mantBuffer = thisdigit;
                                    mantBufferMult = 10;
                                } else {
                                    mantBufferMult *= 10;
                                    mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                                    mantBuffer = mantBuffer + (thisdigit);
                                }
                            }
                        } else {
                            mantInt *= 10;
                            mantInt = mantInt + (thisdigit);
                        }
                        if (haveDigits && maxDigits != null) {
                            digitCount.Increment();
                            if (digitCount.compareTo(maxDigits) > 0) {

                                throw new Error();
                            }
                        }
                    } else {
                        throw new Error();
                    }
                }
                if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                }
                var bigmant = (mant == null) ? (BigInteger.valueOf(mantInt)) : mant.AsBigInteger();
                return ExtendedDecimal.CreateWithFlags(bigmant, BigInteger.ZERO, (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagQuietNaN);
            }
        }
        if (i + 4 <= str.length) {

            if ((str.charAt(i) == 'S' || str.charAt(i) == 's') && (str.charAt(i + 1) == 'N' || str.charAt(i + 1) == 'n') && (str.charAt(i + 2) == 'A' || str.charAt(i + 2) == 'a') && (str.charAt(i + 3) == 'N' || str.charAt(i + 3) == 'n')) {
                if (i + 4 == str.length) {
                    if (!negative) {
                        return ExtendedDecimal.SignalingNaN;
                    }
                    return ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagSignalingNaN);
                }
                i = i + (4);
                var digitCount = new FastInteger(0);
                var maxDigits = null;
                haveDigits = false;
                if (ctx != null && ctx.getPrecision().signum() != 0) {
                    maxDigits = FastInteger.FromBig(ctx.getPrecision());
                    if (ctx.getClampNormalExponents()) {
                        maxDigits.Decrement();
                    }
                }
                for (; i < str.length; ++i) {
                    if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                        var thisdigit = ((str.charCodeAt(i)-48)|0);
                        haveDigits = haveDigits || thisdigit != 0;
                        if (mantInt > ExtendedDecimal.MaxSafeInt) {
                            if (mant == null) {
                                mant = new FastInteger(mantInt);
                                mantBuffer = thisdigit;
                                mantBufferMult = 10;
                            } else {
                                if (mantBufferMult >= 1000000000) {
                                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                                    mantBuffer = thisdigit;
                                    mantBufferMult = 10;
                                } else {
                                    mantBufferMult *= 10;
                                    mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                                    mantBuffer = mantBuffer + (thisdigit);
                                }
                            }
                        } else {
                            mantInt *= 10;
                            mantInt = mantInt + (thisdigit);
                        }
                        if (haveDigits && maxDigits != null) {
                            digitCount.Increment();
                            if (digitCount.compareTo(maxDigits) > 0) {

                                throw new Error();
                            }
                        }
                    } else {
                        throw new Error();
                    }
                }
                if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                }
                var bigmant = (mant == null) ? (BigInteger.valueOf(mantInt)) : mant.AsBigInteger();
                return ExtendedDecimal.CreateWithFlags(bigmant, BigInteger.ZERO, (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagSignalingNaN);
            }
        }

        for (; i < str.length; ++i) {
            if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                var thisdigit = ((str.charCodeAt(i)-48)|0);
                if (mantInt > ExtendedDecimal.MaxSafeInt) {
                    if (mant == null) {
                        mant = new FastInteger(mantInt);
                        mantBuffer = thisdigit;
                        mantBufferMult = 10;
                    } else {
                        if (mantBufferMult >= 1000000000) {
                            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                            mantBuffer = thisdigit;
                            mantBufferMult = 10;
                        } else {
                            mantBufferMult *= 10;
                            mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                            mantBuffer = mantBuffer + (thisdigit);
                        }
                    }
                } else {
                    mantInt *= 10;
                    mantInt = mantInt + (thisdigit);
                }
                haveDigits = true;
                if (haveDecimalPoint) {
                    if (newScaleInt == -2147483648) {
                        if (newScale == null) {
                            newScale = new FastInteger(newScaleInt);
                        }
                        newScale.AddInt(-1);
                    } else {
                        --newScaleInt;
                    }
                }
            } else if (str.charAt(i) == '.') {
                if (haveDecimalPoint) {
                    throw new Error();
                }
                haveDecimalPoint = true;
            } else if (str.charAt(i) == 'E' || str.charAt(i) == 'e') {
                haveExponent = true;
                ++i;
                break;
            } else {
                throw new Error();
            }
        }
        if (!haveDigits) {
            throw new Error();
        }
        if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
        }
        if (haveExponent) {
            var exp = null;
            var expInt = 0;
            offset = 1;
            haveDigits = false;
            if (i == str.length) {
                throw new Error();
            }
            if (str.charAt(i) == '+' || str.charAt(i) == '-') {
                if (str.charAt(i) == '-') {
                    offset = -1;
                }
                ++i;
            }
            for (; i < str.length; ++i) {
                if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
                    haveDigits = true;
                    var thisdigit = ((str.charCodeAt(i)-48)|0);
                    if (expInt > ExtendedDecimal.MaxSafeInt) {
                        if (exp == null) {
                            exp = new FastInteger(expInt);
                            expBuffer = thisdigit;
                            expBufferMult = 10;
                        } else {
                            if (expBufferMult >= 1000000000) {
                                exp.Multiply(expBufferMult).AddInt(expBuffer);
                                expBuffer = thisdigit;
                                expBufferMult = 10;
                            } else {

                                expBufferMult = (expBufferMult << 3) + (expBufferMult << 1);
                                expBuffer = (expBuffer << 3) + (expBuffer << 1);
                                expBuffer = expBuffer + (thisdigit);
                            }
                        }
                    } else {
                        expInt *= 10;
                        expInt = expInt + (thisdigit);
                    }
                } else {
                    throw new Error();
                }
            }
            if (!haveDigits) {
                throw new Error();
            }
            if (exp != null && (expBufferMult != 1 || expBuffer != 0)) {
                exp.Multiply(expBufferMult).AddInt(expBuffer);
            }
            if (offset >= 0 && newScaleInt == 0 && newScale == null && exp == null) {
                newScaleInt = expInt;
            } else if (exp == null) {
                if (newScale == null) {
                    newScale = new FastInteger(newScaleInt);
                }
                if (offset < 0) {
                    newScale.SubtractInt(expInt);
                } else if (expInt != 0) {
                    newScale.AddInt(expInt);
                }
            } else {
                if (newScale == null) {
                    newScale = new FastInteger(newScaleInt);
                }
                if (offset < 0) {
                    newScale.Subtract(exp);
                } else {
                    newScale.Add(exp);
                }
            }
        }
        if (i != str.length) {
            throw new Error();
        }
        var ret = new ExtendedDecimal();
        ret.unsignedMantissa = (mant == null) ? (BigInteger.valueOf(mantInt)) : mant.AsBigInteger();
        ret.exponent = (newScale == null) ? (BigInteger.valueOf(newScaleInt)) : newScale.AsBigInteger();
        ret.flags = negative ? BigNumberFlags.FlagNegative : 0;
        if (ctx != null) {
            ret = ret.RoundToPrecision(ctx);
        }
        return ret;
    };
    constructor['DecimalMathHelper'] = constructor.DecimalMathHelper = function ExtendedDecimal$DecimalMathHelper(){};
    (function(constructor,prototype){

        prototype['GetRadix'] = prototype.GetRadix = function() {
            return 10;
        };

        prototype['GetSign'] = prototype.GetSign = function(value) {
            return value.signum();
        };

        prototype['GetMantissa'] = prototype.GetMantissa = function(value) {
            return value.unsignedMantissa;
        };

        prototype['GetExponent'] = prototype.GetExponent = function(value) {
            return value.exponent;
        };

        prototype['CreateShiftAccumulatorWithDigits'] = prototype.CreateShiftAccumulatorWithDigits = function(bigint, lastDigit, olderDigits) {
            return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
        };

        prototype['CreateShiftAccumulator'] = prototype.CreateShiftAccumulator = function(bigint) {
            return new DigitShiftAccumulator(bigint, 0, 0);
        };

        prototype['HasTerminatingRadixExpansion'] = prototype.HasTerminatingRadixExpansion = function(numerator, denominator) {

            var gcd = numerator.gcd(denominator);
            denominator = denominator.divide(gcd);
            if (denominator.signum() == 0) {
                return false;
            }

            while (denominator.testBit(0) == false) {
                denominator = denominator.shiftRight(1);
            }

            while (true) {
                var bigrem;
                var bigquo;
                {
                    var divrem = (denominator).divideAndRemainder(BigInteger.valueOf(5));
                    bigquo = divrem[0];
                    bigrem = divrem[1];
                }
                if (bigrem.signum() != 0) {
                    break;
                }
                denominator = bigquo;
            }
            return denominator.compareTo(BigInteger.ONE) == 0;
        };

        prototype['MultiplyByRadixPower'] = prototype.MultiplyByRadixPower = function(bigint, power) {
            if (power.signum() <= 0) {
                return bigint;
            }
            if (bigint.signum() == 0) {
                return bigint;
            }
            var bigtmp = null;
            if (bigint.compareTo(BigInteger.ONE) != 0) {
                if (power.CanFitInInt32()) {
                    bigtmp = DecimalUtility.FindPowerOfTen(power.AsInt32());
                    bigint = bigint.multiply(bigtmp);
                } else {
                    bigtmp = DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
                    bigint = bigint.multiply(bigtmp);
                }
                return bigint;
            } else {
                if (power.CanFitInInt32()) {
                    return DecimalUtility.FindPowerOfTen(power.AsInt32());
                } else {
                    return DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
                }
            }
        };

        prototype['GetFlags'] = prototype.GetFlags = function(value) {
            return value.flags;
        };

        prototype['CreateNewWithFlags'] = prototype.CreateNewWithFlags = function(mantissa, exponent, flags) {
            return ExtendedDecimal.CreateWithFlags(mantissa, exponent, flags);
        };

        prototype['GetArithmeticSupport'] = prototype.GetArithmeticSupport = function() {
            return BigNumberFlags.FiniteAndNonFinite;
        };

        prototype['ValueOf'] = prototype.ValueOf = function(val) {
            if (val == 0) {
                return ExtendedDecimal.Zero;
            }
            if (val == 1) {
                return ExtendedDecimal.One;
            }
            return ExtendedDecimal.FromInt64(val);
        };
    })(ExtendedDecimal.DecimalMathHelper,ExtendedDecimal.DecimalMathHelper.prototype);

    constructor['AppendString'] = constructor.AppendString = function(builder, c, count) {
        if (count.CompareToInt(2147483647) > 0 || count.signum() < 0) {
            throw new Error();
        }
        var icount = count.AsInt32();
        for (var i = icount - 1; i >= 0; --i) {
            builder.append(c);
        }
        return true;
    };
    prototype['ToStringInternal'] = prototype.ToStringInternal = function(mode) {

        var negative = (this.flags & BigNumberFlags.FlagNegative) != 0;
        if ((this.flags & BigNumberFlags.FlagInfinity) != 0) {
            return negative ? "-Infinity" : "Infinity";
        }
        if ((this.flags & BigNumberFlags.FlagSignalingNaN) != 0) {
            if (this.unsignedMantissa.signum() == 0) {
                return negative ? "-sNaN" : "sNaN";
            }
            return negative ? "-sNaN" + (this.unsignedMantissa).abs().toString() : "sNaN" + (this.unsignedMantissa).abs().toString();
        }
        if ((this.flags & BigNumberFlags.FlagQuietNaN) != 0) {
            if (this.unsignedMantissa.signum() == 0) {
                return negative ? "-NaN" : "NaN";
            }
            return negative ? "-NaN" + (this.unsignedMantissa).abs().toString() : "NaN" + (this.unsignedMantissa).abs().toString();
        }
        var mantissaString = (this.unsignedMantissa).abs().toString();
        var scaleSign = -this.exponent.signum();
        if (scaleSign == 0) {
            return negative ? "-" + mantissaString : mantissaString;
        }
        var iszero = this.unsignedMantissa.signum() == 0;
        if (mode == 2 && iszero && scaleSign < 0) {

            return negative ? "-" + mantissaString : mantissaString;
        }
        var builderLength = new FastInteger(mantissaString.length);
        var adjustedExponent = FastInteger.FromBig(this.exponent);
        var thisExponent = FastInteger.Copy(adjustedExponent);
        adjustedExponent.Add(builderLength).AddInt(-1);
        var decimalPointAdjust = new FastInteger(1);
        var threshold = new FastInteger(-6);
        if (mode == 1) {

            var newExponent = FastInteger.Copy(adjustedExponent);
            var adjExponentNegative = adjustedExponent.signum() < 0;
            var intphase = FastInteger.Copy(adjustedExponent).Abs().Remainder(3).AsInt32();
            if (iszero && (adjustedExponent.compareTo(threshold) < 0 || scaleSign < 0)) {
                if (intphase == 1) {
                    if (adjExponentNegative) {
                        decimalPointAdjust.Increment();
                        newExponent.Increment();
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(2);
                    }
                } else if (intphase == 2) {
                    if (!adjExponentNegative) {
                        decimalPointAdjust.Increment();
                        newExponent.Increment();
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(2);
                    }
                }
                threshold.Increment();
            } else {
                if (intphase == 1) {
                    if (!adjExponentNegative) {
                        decimalPointAdjust.Increment();
                        newExponent.AddInt(-1);
                    } else {
                        decimalPointAdjust.AddInt(2);
                        newExponent.AddInt(-2);
                    }
                } else if (intphase == 2) {
                    if (adjExponentNegative) {
                        decimalPointAdjust.Increment();
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
                var decimalPoint = FastInteger.Copy(thisExponent).Add(builderLength);
                var cmp = decimalPoint.CompareToInt(0);
                var builder = null;
                if (cmp < 0) {
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    if (negative) {
                        builder.append('-');
                    }
                    builder.append("0.");
                    ExtendedDecimal.AppendString(builder, '0', FastInteger.Copy(decimalPoint).Negate());
                    builder.append(mantissaString);
                } else if (cmp == 0) {
                    if (!decimalPoint.CanFitInInt32()) {
                        throw new Error();
                    }
                    var tmpInt = decimalPoint.AsInt32();
                    if (tmpInt < 0) {
                        tmpInt = 0;
                    }
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    if (negative) {
                        builder.append('-');
                    }
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append("0.");
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else if (decimalPoint.CompareToInt(mantissaString.length) > 0) {
                    var insertionPoint = builderLength;
                    if (!insertionPoint.CanFitInInt32()) {
                        throw new Error();
                    }
                    var tmpInt = insertionPoint.AsInt32();
                    if (tmpInt < 0) {
                        tmpInt = 0;
                    }
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    if (negative) {
                        builder.append('-');
                    }
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    ExtendedDecimal.AppendString(builder, '0', FastInteger.Copy(decimalPoint).SubtractInt(builder.length));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else {
                    if (!decimalPoint.CanFitInInt32()) {
                        throw new Error();
                    }
                    var tmpInt = decimalPoint.AsInt32();
                    if (tmpInt < 0) {
                        tmpInt = 0;
                    }
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    if (negative) {
                        builder.append('-');
                    }
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                }
                return builder.toString();
            } else if (mode == 2 && scaleSign < 0) {
                var negscale = FastInteger.Copy(thisExponent);
                var builder = JSInteropFactory.createStringBuilder(16);
                if (negative) {
                    builder.append('-');
                }
                builder.append(mantissaString);
                ExtendedDecimal.AppendString(builder, '0', negscale);
                return builder.toString();
            } else if (!negative) {
                return mantissaString;
            } else {
                return "-" + mantissaString;
            }
        } else {
            var builder = null;
            if (mode == 1 && iszero && decimalPointAdjust.CompareToInt(1) > 0) {
                builder = JSInteropFactory.createStringBuilder(16);
                if (negative) {
                    builder.append('-');
                }
                builder.append(mantissaString);
                builder.append('.');
                ExtendedDecimal.AppendString(builder, '0', FastInteger.Copy(decimalPointAdjust).AddInt(-1));
            } else {
                var tmp = FastInteger.Copy(decimalPointAdjust);
                var cmp = tmp.CompareToInt(mantissaString.length);
                if (cmp > 0) {
                    tmp.SubtractInt(mantissaString.length);
                    builder = JSInteropFactory.createStringBuilder(16);
                    if (negative) {
                        builder.append('-');
                    }
                    builder.append(mantissaString);
                    ExtendedDecimal.AppendString(builder, '0', tmp);
                } else if (cmp < 0) {

                    if (!tmp.CanFitInInt32()) {
                        throw new Error();
                    }
                    var tmpInt = tmp.AsInt32();
                    if (tmp.signum() < 0) {
                        tmpInt = 0;
                    }
                    var tmpFast = new FastInteger(mantissaString.length).AddInt(6);
                    builder = JSInteropFactory.createStringBuilder(tmpFast.CompareToInt(2147483647) > 0 ? 2147483647 : tmpFast.AsInt32());
                    if (negative) {
                        builder.append('-');
                    }
                    for (var arrfillI = 0; arrfillI < (0) + (tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                    builder.append('.');
                    for (var arrfillI = tmpInt; arrfillI < (tmpInt) + (mantissaString.length - tmpInt); arrfillI++) builder.append(mantissaString.charAt(arrfillI));
                } else if (adjustedExponent.signum() == 0 && !negative) {
                    return mantissaString;
                } else if (adjustedExponent.signum() == 0 && negative) {
                    return "-" + mantissaString;
                } else {
                    builder = JSInteropFactory.createStringBuilder(16);
                    if (negative) {
                        builder.append('-');
                    }
                    builder.append(mantissaString);
                }
            }
            if (adjustedExponent.signum() != 0) {
                builder.append(adjustedExponent.signum() < 0 ? "E-" : "E+");
                adjustedExponent.Abs();
                var builderReversed = JSInteropFactory.createStringBuilder(16);
                while (adjustedExponent.signum() != 0) {
                    var digit = FastInteger.Copy(adjustedExponent).Remainder(10).AsInt32();

                    builderReversed.append(48 + digit);
                    adjustedExponent.Divide(10);
                }
                var count = builderReversed.length();
                for (var i = 0; i < count; ++i) {
                    builder.append(builderReversed.charAt(count - 1 - i));
                }
            }
            return builder.toString();
        }
    };

    prototype['ToBigInteger'] = prototype.ToBigInteger = function() {
        var sign = this.getExponent().signum();
        if (sign == 0) {
            var bigmantissa = this.getMantissa();
            return bigmantissa;
        } else if (sign > 0) {
            var bigmantissa = this.getMantissa();
            var bigexponent = DecimalUtility.FindPowerOfTenFromBig(this.getExponent());
            bigmantissa = bigmantissa.multiply(bigexponent);
            return bigmantissa;
        } else {
            var bigmantissa = this.getMantissa();
            var bigexponent = this.getExponent();
            bigexponent = bigexponent.negate();
            bigexponent = DecimalUtility.FindPowerOfTenFromBig(bigexponent);
            bigmantissa = bigmantissa.divide(bigexponent);
            return bigmantissa;
        }
    };
    constructor['valueOneShift62'] = constructor.valueOneShift62 = BigInteger.ONE.shiftLeft(62);

    prototype['ToExtendedFloat'] = prototype.ToExtendedFloat = function() {
        if (this.IsNaN() || this.IsInfinity()) {
            return ExtendedFloat.CreateWithFlags(this.unsignedMantissa, this.exponent, this.flags);
        }
        var bigintExp = this.getExponent();
        var bigintMant = this.getMantissa();
        if (bigintMant.signum() == 0) {
            return this.isNegative() ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
        }
        if (bigintExp.signum() == 0) {

            return ExtendedFloat.FromBigInteger(bigintMant);
        } else if (bigintExp.signum() > 0) {

            var bigmantissa = bigintMant;
            bigintExp = DecimalUtility.FindPowerOfTenFromBig(bigintExp);
            bigmantissa = bigmantissa.multiply(bigintExp);
            return ExtendedFloat.FromBigInteger(bigmantissa);
        } else {

            var scale = FastInteger.FromBig(bigintExp);
            var bigmantissa = bigintMant;
            var neg = bigmantissa.signum() < 0;
            var remainder;
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            var negscale = FastInteger.Copy(scale).Negate();
            var divisor = DecimalUtility.FindPowerOfFiveFromBig(negscale.AsBigInteger());
            while (true) {
                var quotient;
                {
                    var divrem = (bigmantissa).divideAndRemainder(divisor);
                    quotient = divrem[0];
                    remainder = divrem[1];
                }

                if (remainder.signum() != 0 && quotient.compareTo(ExtendedDecimal.valueOneShift62) < 0) {

                    var bits = FastInteger.GetLastWords(quotient, 2);
                    var shift = 0;
                    if ((bits[0] | bits[1]) != 0) {

                        var bitPrecision = DecimalUtility.BitPrecisionInt(bits[1]);
                        if (bitPrecision != 0) {
                            bitPrecision = bitPrecision + (32);
                        } else {
                            bitPrecision = DecimalUtility.BitPrecisionInt(bits[0]);
                        }
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
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedFloat.Create(bigmantissa, scale.AsBigInteger());
        }
    };

    prototype['ToSingle'] = prototype.ToSingle = function() {
        if (this.IsPositiveInfinity()) {
            return Number.POSITIVE_INFINITY;
        }
        if (this.IsNegativeInfinity()) {
            return Number.NEGATIVE_INFINITY;
        }
        if (this.isNegative() && this.signum() == 0) {
            return Float.intBitsToFloat(1 << 31);
        }
        return this.ToExtendedFloat().ToSingle();
    };

    prototype['ToDouble'] = prototype.ToDouble = function() {
        if (this.IsPositiveInfinity()) {
            return Number.POSITIVE_INFINITY;
        }
        if (this.IsNegativeInfinity()) {
            return Number.NEGATIVE_INFINITY;
        }
        if (this.isNegative() && this.signum() == 0) {
            return Extras.IntegersToDouble([((1 << 31)|0), 0]);
        }
        return this.ToExtendedFloat().ToDouble();
    };

    constructor['FromSingle'] = constructor.FromSingle = function(flt) {
        var value = Float.floatToRawIntBits(flt);
        var neg = (value >> 31) != 0;
        var floatExponent = ((value >> 23) & 255);
        var valueFpMantissa = value & 8388607;
        if (floatExponent == 255) {
            if (valueFpMantissa == 0) {
                return neg ? ExtendedDecimal.NegativeInfinity : ExtendedDecimal.PositiveInfinity;
            }

            var quiet = (valueFpMantissa & 4194304) != 0;
            valueFpMantissa &= 2097151;
            var info = BigInteger.valueOf(valueFpMantissa);
            info = info.subtract(BigInteger.ONE);
            if (info.signum() == 0) {
                return quiet ? ExtendedDecimal.NaN : ExtendedDecimal.SignalingNaN;
            } else {
                return ExtendedDecimal.CreateWithFlags(info, BigInteger.ZERO, (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
            }
        }
        if (floatExponent == 0) {
            ++floatExponent;
        } else {
            valueFpMantissa |= 1 << 23;
        }
        if (valueFpMantissa == 0) {
            return neg ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
        }
        floatExponent -= 150;
        while ((valueFpMantissa & 1) == 0) {
            ++floatExponent;
            valueFpMantissa >>= 1;
        }
        if (floatExponent == 0) {
            if (neg) {
                valueFpMantissa = -valueFpMantissa;
            }
            return ExtendedDecimal.FromInt64(valueFpMantissa);
        } else if (floatExponent > 0) {

            var bigmantissa = BigInteger.valueOf(valueFpMantissa);
            bigmantissa = bigmantissa.shiftLeft(floatExponent);
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedDecimal.FromBigInteger(bigmantissa);
        } else {

            var bigmantissa = BigInteger.valueOf(valueFpMantissa);
            var bigexponent = DecimalUtility.FindPowerOfFive(-floatExponent);
            bigmantissa = bigmantissa.multiply(bigexponent);
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedDecimal.Create(bigmantissa, BigInteger.valueOf(floatExponent));
        }
    };

    constructor['FromBigInteger'] = constructor.FromBigInteger = function(bigint) {
        return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    };

    constructor['FromInt64'] = constructor.FromInt64 = function(valueSmall_obj) {
        var valueSmall = JSInteropFactory.createLong(valueSmall_obj);
        var bigint = BigInteger.valueOf(valueSmall);
        return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    };

    constructor['FromInt32'] = constructor.FromInt32 = function(valueSmaller) {
        var bigint = BigInteger.valueOf(valueSmaller);
        return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    };

    constructor['FromDouble'] = constructor.FromDouble = function(dbl) {
        var value = Extras.DoubleToIntegers(dbl);
        var floatExponent = ((value[1] >> 20) & 2047);
        var neg = (value[1] >> 31) != 0;
        if (floatExponent == 2047) {
            if ((value[1] & 1048575) == 0 && value[0] == 0) {
                return neg ? ExtendedDecimal.NegativeInfinity : ExtendedDecimal.PositiveInfinity;
            }

            var quiet = (value[1] & 524288) != 0;
            value[1] = value[1] & 262143;
            var info = FastInteger.WordsToBigInteger(value);
            info = info.subtract(BigInteger.ONE);
            if (info.signum() == 0) {
                return quiet ? ExtendedDecimal.NaN : ExtendedDecimal.SignalingNaN;
            } else {
                return ExtendedDecimal.CreateWithFlags(info, BigInteger.ZERO, (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
            }
        }
        value[1] = value[1] & 1048575;

        if (floatExponent == 0) {
            ++floatExponent;
        } else {
            value[1] = value[1] | 1048576;
        }
        if ((value[1] | value[0]) != 0) {
            floatExponent = floatExponent + (DecimalUtility.ShiftAwayTrailingZerosTwoElements(value));
        } else {
            return neg ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
        }
        floatExponent -= 1075;
        var valueFpMantissaBig = FastInteger.WordsToBigInteger(value);
        if (floatExponent == 0) {
            if (neg) {
                valueFpMantissaBig = valueFpMantissaBig.negate();
            }
            return ExtendedDecimal.FromBigInteger(valueFpMantissaBig);
        } else if (floatExponent > 0) {

            var bigmantissa = valueFpMantissaBig;
            bigmantissa = bigmantissa.shiftLeft(floatExponent);
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedDecimal.FromBigInteger(bigmantissa);
        } else {

            var bigmantissa = valueFpMantissaBig;
            var exp = DecimalUtility.FindPowerOfFive(-floatExponent);
            bigmantissa = bigmantissa.multiply(exp);
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedDecimal.Create(bigmantissa, BigInteger.valueOf(floatExponent));
        }
    };

    constructor['FromExtendedFloat'] = constructor.FromExtendedFloat = function(bigfloat) {
        if (bigfloat == null) {
            throw new Error("bigfloat");
        }
        if (bigfloat.IsNaN() || bigfloat.IsInfinity()) {
            var flags = (bigfloat.isNegative() ? BigNumberFlags.FlagNegative : 0) | (bigfloat.IsInfinity() ? BigNumberFlags.FlagInfinity : 0) | (bigfloat.IsQuietNaN() ? BigNumberFlags.FlagQuietNaN : 0) | (bigfloat.IsSignalingNaN() ? BigNumberFlags.FlagSignalingNaN : 0);
            return ExtendedDecimal.CreateWithFlags(bigfloat.getUnsignedMantissa(), bigfloat.getExponent(), flags);
        }
        var bigintExp = bigfloat.getExponent();
        var bigintMant = bigfloat.getMantissa();
        if (bigintMant.signum() == 0) {
            return bigfloat.isNegative() ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
        }
        if (bigintExp.signum() == 0) {

            return ExtendedDecimal.FromBigInteger(bigintMant);
        } else if (bigintExp.signum() > 0) {

            var intcurexp = FastInteger.FromBig(bigintExp);
            var bigmantissa = bigintMant;
            var neg = bigmantissa.signum() < 0;
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            while (intcurexp.signum() > 0) {
                var shift = 512;
                if (intcurexp.CompareToInt(512) < 0) {
                    shift = intcurexp.AsInt32();
                }
                bigmantissa = bigmantissa.shiftLeft(shift);
                intcurexp.AddInt(-shift);
            }
            if (neg) {
                bigmantissa = (bigmantissa).negate();
            }
            return ExtendedDecimal.FromBigInteger(bigmantissa);
        } else {

            var bigmantissa = bigintMant;
            var negbigintExp = (bigintExp).negate();
            negbigintExp = DecimalUtility.FindPowerOfFiveFromBig(negbigintExp);
            bigmantissa = bigmantissa.multiply(negbigintExp);
            return ExtendedDecimal.Create(bigmantissa, bigintExp);
        }
    };

    prototype['toString'] = prototype.toString = function() {
        return this.ToStringInternal(0);
    };

    prototype['ToEngineeringString'] = prototype.ToEngineeringString = function() {
        return this.ToStringInternal(1);
    };

    prototype['ToPlainString'] = prototype.ToPlainString = function() {
        return this.ToStringInternal(2);
    };
    constructor['One'] = constructor.One = ExtendedDecimal.Create(BigInteger.ONE, BigInteger.ZERO);
    constructor['Zero'] = constructor.Zero = ExtendedDecimal.Create(BigInteger.ZERO, BigInteger.ZERO);
    constructor['NegativeZero'] = constructor.NegativeZero = ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagNegative);
    constructor['Ten'] = constructor.Ten = ExtendedDecimal.Create(BigInteger.TEN, BigInteger.ZERO);
    constructor['NaN'] = constructor.NaN = ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagQuietNaN);
    constructor['SignalingNaN'] = constructor.SignalingNaN = ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagSignalingNaN);
    constructor['PositiveInfinity'] = constructor.PositiveInfinity = ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity);
    constructor['NegativeInfinity'] = constructor.NegativeInfinity = ExtendedDecimal.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    prototype['IsPositiveInfinity'] = prototype.IsPositiveInfinity = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    };

    prototype['IsNegativeInfinity'] = prototype.IsNegativeInfinity = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    };

    prototype['IsNaN'] = prototype.IsNaN = function() {
        return (this.flags & (BigNumberFlags.FlagQuietNaN | BigNumberFlags.FlagSignalingNaN)) != 0;
    };

    prototype['IsInfinity'] = prototype.IsInfinity = function() {
        return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    };

    prototype['isFinite'] = prototype.isFinite = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNaN)) == 0;
    };

    prototype['isNegative'] = prototype.isNegative = function() {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
    };

    prototype['IsQuietNaN'] = prototype.IsQuietNaN = function() {
        return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    };

    prototype['IsSignalingNaN'] = prototype.IsSignalingNaN = function() {
        return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    };

    prototype['signum'] = prototype.signum = function() {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) && this.unsignedMantissa.signum() == 0) ? 0 : (((this.flags & BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
    };

    prototype['isZero'] = prototype.isZero = function() {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) && this.unsignedMantissa.signum() == 0;
    };

    prototype['DivideToSameExponent'] = prototype.DivideToSameExponent = function(divisor, rounding) {
        return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    };

    prototype['Reduce'] = prototype.Reduce = function(ctx) {
        return ExtendedDecimal.math.Reduce(this, ctx);
    };

    prototype['RemainderNaturalScale'] = prototype.RemainderNaturalScale = function(divisor, ctx) {
        return this.Subtract(this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null), ctx);
    };

    prototype['Divide'] = prototype.Divide = function(divisor, ctx) {
        return ExtendedDecimal.math.Divide(this, divisor, ctx);
    };

    prototype['DivideToExponent'] = prototype.DivideToExponent = function(divisor, exponent, ctx) {
        return ExtendedDecimal.math.DivideToExponent(this, divisor, exponent, ctx);
    };

    prototype['Abs'] = prototype.Abs = function(context) {
        return ExtendedDecimal.math.Abs(this, context);
    };

    prototype['Negate'] = prototype.Negate = function(context) {
        return ExtendedDecimal.math.Negate(this, context);
    };

    prototype['Subtract'] = prototype.Subtract = function(numberObject, ctx) {
        if (numberObject == null) {
            throw new Error("numberObject");
        }
        var negated = numberObject;
        if ((numberObject.flags & BigNumberFlags.FlagNaN) == 0) {
            var newflags = numberObject.flags ^ BigNumberFlags.FlagNegative;
            negated = ExtendedDecimal.CreateWithFlags(numberObject.unsignedMantissa, numberObject.exponent, newflags);
        }
        return this.Add(negated, ctx);
    };
    constructor['math'] = constructor.math = new TrappableRadixMath(new RadixMath(new ExtendedDecimal.DecimalMathHelper()));

    prototype['DivideToIntegerNaturalScale'] = prototype.DivideToIntegerNaturalScale = function(divisor, ctx) {
        return ExtendedDecimal.math.DivideToIntegerNaturalScale(this, divisor, ctx);
    };

    prototype['DivideToIntegerZeroScale'] = prototype.DivideToIntegerZeroScale = function(divisor, ctx) {
        return ExtendedDecimal.math.DivideToIntegerZeroScale(this, divisor, ctx);
    };

    prototype['Remainder'] = prototype.Remainder = function(divisor, ctx) {
        return ExtendedDecimal.math.Remainder(this, divisor, ctx);
    };

    prototype['RemainderNear'] = prototype.RemainderNear = function(divisor, ctx) {
        return ExtendedDecimal.math.RemainderNear(this, divisor, ctx);
    };

    prototype['NextMinus'] = prototype.NextMinus = function(ctx) {
        return ExtendedDecimal.math.NextMinus(this, ctx);
    };

    prototype['NextPlus'] = prototype.NextPlus = function(ctx) {
        return ExtendedDecimal.math.NextPlus(this, ctx);
    };

    prototype['NextToward'] = prototype.NextToward = function(otherValue, ctx) {
        return ExtendedDecimal.math.NextToward(this, otherValue, ctx);
    };

    constructor['Max'] = constructor.Max = function(first, second, ctx) {
        return ExtendedDecimal.math.Max(first, second, ctx);
    };

    constructor['Min'] = constructor.Min = function(first, second, ctx) {
        return ExtendedDecimal.math.Min(first, second, ctx);
    };

    constructor['MaxMagnitude'] = constructor.MaxMagnitude = function(first, second, ctx) {
        return ExtendedDecimal.math.MaxMagnitude(first, second, ctx);
    };

    constructor['MinMagnitude'] = constructor.MinMagnitude = function(first, second, ctx) {
        return ExtendedDecimal.math.MinMagnitude(first, second, ctx);
    };

    prototype['compareTo'] = prototype.compareTo = function(other) {
        return ExtendedDecimal.math.compareTo(this, other);
    };

    prototype['CompareToWithContext'] = prototype.CompareToWithContext = function(other, ctx) {
        return ExtendedDecimal.math.CompareToWithContext(this, other, false, ctx);
    };

    prototype['CompareToSignal'] = prototype.CompareToSignal = function(other, ctx) {
        return ExtendedDecimal.math.CompareToWithContext(this, other, true, ctx);
    };

    prototype['Add'] = prototype.Add = function(numberObject, ctx) {
        return ExtendedDecimal.math.Add(this, numberObject, ctx);
    };

    prototype['Quantize'] = prototype.Quantize = function(otherValue, ctx) {
        return ExtendedDecimal.math.Quantize(this, otherValue, ctx);
    };

    prototype['RoundToIntegralExact'] = prototype.RoundToIntegralExact = function(ctx) {
        return ExtendedDecimal.math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    };

    prototype['RoundToIntegralNoRoundedFlag'] = prototype.RoundToIntegralNoRoundedFlag = function(ctx) {
        return ExtendedDecimal.math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    };

    prototype['RoundToExponentExact'] = prototype.RoundToExponentExact = function(exponent, ctx) {
        return ExtendedDecimal.math.RoundToExponentExact(this, exponent, ctx);
    };

    prototype['RoundToExponent'] = prototype.RoundToExponent = function(exponent, ctx) {
        return ExtendedDecimal.math.RoundToExponentSimple(this, exponent, ctx);
    };

    prototype['Multiply'] = prototype.Multiply = function(op, ctx) {
        return ExtendedDecimal.math.Multiply(this, op, ctx);
    };

    prototype['MultiplyAndAdd'] = prototype.MultiplyAndAdd = function(op, augend, ctx) {
        return ExtendedDecimal.math.MultiplyAndAdd(this, op, augend, ctx);
    };

    prototype['MultiplyAndSubtract'] = prototype.MultiplyAndSubtract = function(op, subtrahend, ctx) {
        if (subtrahend == null) {
            throw new Error("numberObject");
        }
        var negated = subtrahend;
        if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
            var newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
            negated = ExtendedDecimal.CreateWithFlags(subtrahend.unsignedMantissa, subtrahend.exponent, newflags);
        }
        return ExtendedDecimal.math.MultiplyAndAdd(this, op, negated, ctx);
    };

    prototype['RoundToPrecision'] = prototype.RoundToPrecision = function(ctx) {
        return ExtendedDecimal.math.RoundToPrecision(this, ctx);
    };

    prototype['Plus'] = prototype.Plus = function(ctx) {
        return ExtendedDecimal.math.Plus(this, ctx);
    };

    prototype['RoundToBinaryPrecision'] = prototype.RoundToBinaryPrecision = function(ctx) {
        return ExtendedDecimal.math.RoundToBinaryPrecision(this, ctx);
    };

    prototype['SquareRoot'] = prototype.SquareRoot = function(ctx) {
        return ExtendedDecimal.math.SquareRoot(this, ctx);
    };

    prototype['Exp'] = prototype.Exp = function(ctx) {
        return ExtendedDecimal.math.Exp(this, ctx);
    };

    prototype['Log'] = prototype.Log = function(ctx) {
        return ExtendedDecimal.math.Ln(this, ctx);
    };

    prototype['Log10'] = prototype.Log10 = function(ctx) {
        return ExtendedDecimal.math.Log10(this, ctx);
    };

    prototype['Pow'] = prototype.Pow = function(exponent, ctx) {
        return ExtendedDecimal.math.Power(this, exponent, ctx);
    };

    constructor['PI'] = constructor.PI = function(ctx) {
        return ExtendedDecimal.math.Pi(ctx);
    };
})(ExtendedDecimal,ExtendedDecimal.prototype);

if(typeof exports!=="undefined")exports['ExtendedDecimal']=ExtendedDecimal;
if(typeof window!=="undefined")window['ExtendedDecimal']=ExtendedDecimal;

var ExtendedFloat =

function() {

};
(function(constructor,prototype){
    prototype['exponent'] = prototype.exponent = null;
    prototype['unsignedMantissa'] = prototype.unsignedMantissa = null;
    prototype['flags'] = prototype.flags = null;
    prototype['getExponent'] = prototype.getExponent = function() {
        return this.exponent;
    };
    prototype['getUnsignedMantissa'] = prototype.getUnsignedMantissa = function() {
        return this.unsignedMantissa;
    };
    prototype['getMantissa'] = prototype.getMantissa = function() {
        return this.isNegative() ? ((this.unsignedMantissa).negate()) : this.unsignedMantissa;
    };
    prototype['EqualsInternal'] = prototype.EqualsInternal = function(otherValue) {
        if (otherValue == null) {
            return false;
        }
        return this.exponent.equals(otherValue.exponent) && this.unsignedMantissa.equals(otherValue.unsignedMantissa) && this.flags == otherValue.flags;
    };
    prototype['equals'] = prototype.equals = function(obj) {
        return this.EqualsInternal((obj.constructor==ExtendedFloat) ? obj : null);
    };
    prototype['hashCode'] = prototype.hashCode = function() {
        var hashCode_ = 0;
        {
            hashCode_ = hashCode_ + (1000000007 * this.exponent.hashCode());
            hashCode_ = hashCode_ + (1000000009 * this.unsignedMantissa.hashCode());
            hashCode_ = hashCode_ + (1000000009 * this.flags);
        }
        return hashCode_;
    };
    constructor['Create'] = constructor.Create = function(mantissa, exponent) {
        if (mantissa == null) {
            throw new Error("mantissa");
        }
        if (exponent == null) {
            throw new Error("exponent");
        }
        var ex = new ExtendedFloat();
        ex.exponent = exponent;
        var sign = mantissa == null ? 0 : mantissa.signum();
        ex.unsignedMantissa = sign < 0 ? ((mantissa).negate()) : mantissa;
        ex.flags = (sign < 0) ? BigNumberFlags.FlagNegative : 0;
        return ex;
    };
    constructor['CreateWithFlags'] = constructor.CreateWithFlags = function(mantissa, exponent, flags) {
        var ext = ExtendedFloat.Create(mantissa, exponent);
        ext.flags = flags;
        return ext;
    };

    constructor['FromString'] = constructor.FromString = function(str, ctx) {
        if (str == null) {
            throw new Error("str");
        }
        return ExtendedDecimal.FromString(str, ctx).ToExtendedFloat();
    };
    constructor['valueBigShiftIteration'] = constructor.valueBigShiftIteration = BigInteger.valueOf(1000000);
    constructor['valueShiftIteration'] = constructor.valueShiftIteration = 1000000;
    constructor['ShiftLeft'] = constructor.ShiftLeft = function(val, bigShift) {
        if (val.signum() == 0) {
            return val;
        }
        while (bigShift.compareTo(ExtendedFloat.valueBigShiftIteration) > 0) {
            val = val.shiftLeft(1000000);
            bigShift = bigShift.subtract(ExtendedFloat.valueBigShiftIteration);
        }
        var lastshift = bigShift.intValue();
        val = val.shiftLeft(lastshift);
        return val;
    };
    constructor['ShiftLeftInt'] = constructor.ShiftLeftInt = function(val, shift) {
        if (val.signum() == 0) {
            return val;
        }
        while (shift > ExtendedFloat.valueShiftIteration) {
            val = val.shiftLeft(1000000);
            shift -= ExtendedFloat.valueShiftIteration;
        }
        var lastshift = (shift|0);
        val = val.shiftLeft(lastshift);
        return val;
    };
    constructor['BinaryMathHelper'] = constructor.BinaryMathHelper = function ExtendedFloat$BinaryMathHelper(){};
    (function(constructor,prototype){

        prototype['GetRadix'] = prototype.GetRadix = function() {
            return 2;
        };

        prototype['GetSign'] = prototype.GetSign = function(value) {
            return value.signum();
        };

        prototype['GetMantissa'] = prototype.GetMantissa = function(value) {
            return value.getMantissa();
        };

        prototype['GetExponent'] = prototype.GetExponent = function(value) {
            return value.exponent;
        };

        prototype['CreateShiftAccumulatorWithDigits'] = prototype.CreateShiftAccumulatorWithDigits = function(bigint, lastDigit, olderDigits) {
            return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
        };

        prototype['CreateShiftAccumulator'] = prototype.CreateShiftAccumulator = function(bigint) {
            return new BitShiftAccumulator(bigint, 0, 0);
        };

        prototype['HasTerminatingRadixExpansion'] = prototype.HasTerminatingRadixExpansion = function(num, den) {
            var gcd = num.gcd(den);
            if (gcd.signum() == 0) {
                return false;
            }
            den = den.divide(gcd);
            while (den.testBit(0) == false) {
                den = den.shiftRight(1);
            }
            return den.equals(BigInteger.ONE);
        };

        prototype['MultiplyByRadixPower'] = prototype.MultiplyByRadixPower = function(bigint, power) {
            if (power.signum() <= 0) {
                return bigint;
            }
            if (bigint.signum() < 0) {
                bigint = bigint.negate();
                if (power.CanFitInInt32()) {
                    bigint = ExtendedFloat.ShiftLeftInt(bigint, power.AsInt32());
                    bigint = bigint.negate();
                } else {
                    bigint = ExtendedFloat.ShiftLeft(bigint, power.AsBigInteger());
                    bigint = bigint.negate();
                }
                return bigint;
            } else {
                if (power.CanFitInInt32()) {
                    return ExtendedFloat.ShiftLeftInt(bigint, power.AsInt32());
                } else {
                    return ExtendedFloat.ShiftLeft(bigint, power.AsBigInteger());
                }
            }
        };

        prototype['GetFlags'] = prototype.GetFlags = function(value) {
            return value.flags;
        };

        prototype['CreateNewWithFlags'] = prototype.CreateNewWithFlags = function(mantissa, exponent, flags) {
            return ExtendedFloat.CreateWithFlags(mantissa, exponent, flags);
        };

        prototype['GetArithmeticSupport'] = prototype.GetArithmeticSupport = function() {
            return BigNumberFlags.FiniteAndNonFinite;
        };

        prototype['ValueOf'] = prototype.ValueOf = function(val) {
            return ExtendedFloat.FromInt64(val);
        };
    })(ExtendedFloat.BinaryMathHelper,ExtendedFloat.BinaryMathHelper.prototype);

    prototype['ToBigInteger'] = prototype.ToBigInteger = function() {
        var expsign = this.getExponent().signum();
        if (expsign == 0) {

            return this.getMantissa();
        } else if (expsign > 0) {

            var curexp = this.getExponent();
            var bigmantissa = this.getMantissa();
            if (bigmantissa.signum() == 0) {
                return bigmantissa;
            }
            var neg = bigmantissa.signum() < 0;
            if (neg) {
                bigmantissa = bigmantissa.negate();
            }
            bigmantissa = ExtendedFloat.ShiftLeft(bigmantissa, curexp);
            if (neg) {
                bigmantissa = bigmantissa.negate();
            }
            return bigmantissa;
        } else {

            var curexp = this.getExponent();
            var bigmantissa = this.getMantissa();
            if (bigmantissa.signum() == 0) {
                return bigmantissa;
            }
            var neg = bigmantissa.signum() < 0;
            if (neg) {
                bigmantissa = bigmantissa.negate();
            }
            while (curexp.signum() < 0 && bigmantissa.signum() != 0) {
                var shift = 4096;
                if (curexp.compareTo(BigInteger.valueOf(-4096)) > 0) {
                    shift = -(curexp.intValue());
                }
                bigmantissa = bigmantissa.shiftRight(shift);
                curexp = curexp.add(BigInteger.valueOf(shift));
            }
            if (neg) {
                bigmantissa = bigmantissa.negate();
            }
            return bigmantissa;
        }
    };
    constructor['valueOneShift23'] = constructor.valueOneShift23 = BigInteger.ONE.shiftLeft(23);
    constructor['valueOneShift52'] = constructor.valueOneShift52 = BigInteger.ONE.shiftLeft(52);

    prototype['ToSingle'] = prototype.ToSingle = function() {
        if (this.IsPositiveInfinity()) {
            return Number.POSITIVE_INFINITY;
        }
        if (this.IsNegativeInfinity()) {
            return Number.NEGATIVE_INFINITY;
        }
        if (this.IsNaN()) {
            var nan = 2139095041;
            if (this.isNegative()) {
                nan |= ((1 << 31)|0);
            }
            if (this.IsQuietNaN()) {

                nan |= 4194304;
            } else {

                nan |= 2097152;
            }
            if (this.getUnsignedMantissa().signum() != 0) {

                var bigdata = this.getUnsignedMantissa().remainder(BigInteger.valueOf(2097152));
                nan |= bigdata.intValue();
            }
            return Float.intBitsToFloat(nan);
        }
        if (this.isNegative() && this.signum() == 0) {
            return Float.intBitsToFloat(1 << 31);
        }
        var bigmant = (this.unsignedMantissa).abs();
        var bigexponent = FastInteger.FromBig(this.exponent);
        var bitLeftmost = 0;
        var bitsAfterLeftmost = 0;
        if (this.unsignedMantissa.signum() == 0) {
            return 0.0;
        }
        var smallmant = 0;
        var fastSmallMant;
        if (bigmant.compareTo(ExtendedFloat.valueOneShift23) < 0) {
            smallmant = bigmant.intValue();
            var exponentchange = 0;
            while (smallmant < (1 << 23)) {
                smallmant <<= 1;
                ++exponentchange;
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
            fastSmallMant.Increment();
            if (fastSmallMant.CompareToInt(1 << 24) == 0) {
                fastSmallMant = new FastInteger(1 << 23);
                bigexponent.Increment();
            }
        }
        var subnormal = false;
        if (bigexponent.CompareToInt(104) > 0) {

            return this.isNegative() ? Number.NEGATIVE_INFINITY : Number.POSITIVE_INFINITY;
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
                fastSmallMant.Increment();
                if (fastSmallMant.CompareToInt(1 << 24) == 0) {
                    fastSmallMant = new FastInteger(1 << 23);
                    bigexponent.Increment();
                }
            }
        }
        if (bigexponent.CompareToInt(-149) < 0) {

            return this.isNegative() ? Float.intBitsToFloat(1 << 31) : Float.intBitsToFloat(0);
        } else {
            var smallexponent = bigexponent.AsInt32();
            smallexponent = smallexponent + (150);
            var smallmantissa = ((fastSmallMant.AsInt32())|0) & 8388607;
            if (!subnormal) {
                smallmantissa |= smallexponent << 23;
            }
            if (this.isNegative()) {
                smallmantissa |= 1 << 31;
            }
            return Float.intBitsToFloat(smallmantissa);
        }
    };

    prototype['ToDouble'] = prototype.ToDouble = function() {
        if (this.IsPositiveInfinity()) {
            return Number.POSITIVE_INFINITY;
        }
        if (this.IsNegativeInfinity()) {
            return Number.NEGATIVE_INFINITY;
        }
        if (this.IsNaN()) {
            var nan = [1, 2146435072];
            if (this.isNegative()) {
                nan[1] = nan[1] | ((1 << 31)|0);
            }
            if (this.IsQuietNaN()) {
                nan[1] = nan[1] | 524288;
            } else {

                nan[1] = nan[1] | 262144;
            }
            if (this.getUnsignedMantissa().signum() != 0) {

                var words = FastInteger.GetLastWords(this.getUnsignedMantissa(), 2);
                nan[0] = words[0];
                nan[1] = words[1] & 262143;
            }
            return Extras.IntegersToDouble(nan);
        }
        if (this.isNegative() && this.signum() == 0) {
            return Extras.IntegersToDouble([((1 << 31)|0), 0]);
        }
        var bigmant = (this.unsignedMantissa).abs();
        var bigexponent = FastInteger.FromBig(this.exponent);
        var bitLeftmost = 0;
        var bitsAfterLeftmost = 0;
        if (this.unsignedMantissa.signum() == 0) {
            return 0.0;
        }
        var mantissaBits;
        if (bigmant.compareTo(ExtendedFloat.valueOneShift52) < 0) {
            mantissaBits = FastInteger.GetLastWords(bigmant, 2);

            while (!DecimalUtility.HasBitSet(mantissaBits, 52)) {
                DecimalUtility.ShiftLeftOne(mantissaBits);
                bigexponent.Decrement();
            }
        } else {
            var accum = new BitShiftAccumulator(bigmant, 0, 0);
            accum.ShiftToDigitsInt(53);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);
        }

        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || DecimalUtility.HasBitSet(mantissaBits, 0))) {

            mantissaBits[0] = ((mantissaBits[0] + 1)|0);
            if (mantissaBits[0] == 0) {
                mantissaBits[1] = ((mantissaBits[1] + 1)|0);
            }
            if (mantissaBits[0] == 0 && mantissaBits[1] == (1 << 21)) {

                mantissaBits[1] = mantissaBits[1] >> 1;

                bigexponent.Increment();
            }
        }
        var subnormal = false;
        if (bigexponent.CompareToInt(971) > 0) {

            return this.isNegative() ? Number.NEGATIVE_INFINITY : Number.POSITIVE_INFINITY;
        } else if (bigexponent.CompareToInt(-1074) < 0) {

            subnormal = true;

            var accum = new BitShiftAccumulator(FastInteger.WordsToBigInteger(mantissaBits), 0, 0);
            var fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
            accum.ShiftRight(fi);
            bitsAfterLeftmost = accum.getOlderDiscardedDigits();
            bitLeftmost = accum.getLastDiscardedDigit();
            bigexponent.Add(accum.getDiscardedDigitCount());
            mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);

            if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || DecimalUtility.HasBitSet(mantissaBits, 0))) {

                mantissaBits[0] = ((mantissaBits[0] + 1)|0);
                if (mantissaBits[0] == 0) {
                    mantissaBits[1] = ((mantissaBits[1] + 1)|0);
                }
                if (mantissaBits[0] == 0 && mantissaBits[1] == (1 << 21)) {

                    mantissaBits[1] = mantissaBits[1] >> 1;

                    bigexponent.Increment();
                }
            }
        }
        if (bigexponent.CompareToInt(-1074) < 0) {

            return this.isNegative() ? Extras.IntegersToDouble([0, ((-2147483648)|0)]) : 0.0;
        } else {
            bigexponent.AddInt(1075);

            mantissaBits[1] = mantissaBits[1] & 1048575;
            if (!subnormal) {
                var smallexponent = bigexponent.AsInt32() << 20;
                mantissaBits[1] = mantissaBits[1] | smallexponent;
            }
            if (this.isNegative()) {
                mantissaBits[1] = mantissaBits[1] | ((1 << 31)|0);
            }
            return Extras.IntegersToDouble(mantissaBits);
        }
    };

    constructor['FromSingle'] = constructor.FromSingle = function(flt) {
        var value = Float.floatToRawIntBits(flt);
        var neg = (value >> 31) != 0;
        var floatExponent = ((value >> 23) & 255);
        var valueFpMantissa = value & 8388607;
        var bigmant;
        if (floatExponent == 255) {
            if (valueFpMantissa == 0) {
                return neg ? ExtendedFloat.NegativeInfinity : ExtendedFloat.PositiveInfinity;
            }

            var quiet = (valueFpMantissa & 4194304) != 0;
            valueFpMantissa &= 2097151;
            bigmant = BigInteger.valueOf(valueFpMantissa);
            bigmant = bigmant.subtract(BigInteger.ONE);
            if (bigmant.signum() == 0) {
                return quiet ? ExtendedFloat.NaN : ExtendedFloat.SignalingNaN;
            } else {
                return ExtendedFloat.CreateWithFlags(bigmant, BigInteger.ZERO, (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
            }
        }
        if (floatExponent == 0) {
            ++floatExponent;
        } else {
            valueFpMantissa |= 1 << 23;
        }
        if (valueFpMantissa == 0) {
            return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
        }
        while ((valueFpMantissa & 1) == 0) {
            ++floatExponent;
            valueFpMantissa >>= 1;
        }
        if (neg) {
            valueFpMantissa = -valueFpMantissa;
        }
        bigmant = BigInteger.valueOf(valueFpMantissa);
        return ExtendedFloat.Create(bigmant, BigInteger.valueOf(floatExponent - 150));
    };
    constructor['FromBigInteger'] = constructor.FromBigInteger = function(bigint) {
        return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    };
    constructor['FromInt64'] = constructor.FromInt64 = function(valueSmall_obj) {
        var valueSmall = JSInteropFactory.createLong(valueSmall_obj);
        var bigint = BigInteger.valueOf(valueSmall);
        return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    };

    constructor['FromInt32'] = constructor.FromInt32 = function(valueSmaller) {
        var bigint = BigInteger.valueOf(valueSmaller);
        return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    };

    constructor['FromDouble'] = constructor.FromDouble = function(dbl) {
        var value = Extras.DoubleToIntegers(dbl);
        var floatExponent = ((value[1] >> 20) & 2047);
        var neg = (value[1] >> 31) != 0;
        if (floatExponent == 2047) {
            if ((value[1] & 1048575) == 0 && value[0] == 0) {
                return neg ? ExtendedFloat.NegativeInfinity : ExtendedFloat.PositiveInfinity;
            }

            var quiet = (value[1] & 524288) != 0;
            value[1] = value[1] & 262143;
            var info = FastInteger.WordsToBigInteger(value);
            info = info.subtract(BigInteger.ONE);
            if (info.signum() == 0) {
                return quiet ? ExtendedFloat.NaN : ExtendedFloat.SignalingNaN;
            } else {
                return ExtendedFloat.CreateWithFlags(info, BigInteger.ZERO, (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
            }
        }
        value[1] = value[1] & 1048575;

        if (floatExponent == 0) {
            ++floatExponent;
        } else {
            value[1] = value[1] | 1048576;
        }
        if ((value[1] | value[0]) != 0) {
            floatExponent = floatExponent + (DecimalUtility.ShiftAwayTrailingZerosTwoElements(value));
        } else {
            return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
        }
        return ExtendedFloat.CreateWithFlags(FastInteger.WordsToBigInteger(value), BigInteger.valueOf(floatExponent - 1075), neg ? BigNumberFlags.FlagNegative : 0);
    };

    prototype['ToExtendedDecimal'] = prototype.ToExtendedDecimal = function() {
        return ExtendedDecimal.FromExtendedFloat(this);
    };

    prototype['toString'] = prototype.toString = function() {
        return ExtendedDecimal.FromExtendedFloat(this).toString();
    };

    prototype['ToEngineeringString'] = prototype.ToEngineeringString = function() {
        return this.ToExtendedDecimal().ToEngineeringString();
    };

    prototype['ToPlainString'] = prototype.ToPlainString = function() {
        return this.ToExtendedDecimal().ToPlainString();
    };
    constructor['One'] = constructor.One = ExtendedFloat.Create(BigInteger.ONE, BigInteger.ZERO);
    constructor['Zero'] = constructor.Zero = ExtendedFloat.Create(BigInteger.ZERO, BigInteger.ZERO);
    constructor['NegativeZero'] = constructor.NegativeZero = ExtendedFloat.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagNegative);
    constructor['Ten'] = constructor.Ten = ExtendedFloat.Create(BigInteger.TEN, BigInteger.ZERO);
    constructor['NaN'] = constructor.NaN = ExtendedFloat.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagQuietNaN);
    constructor['SignalingNaN'] = constructor.SignalingNaN = ExtendedFloat.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagSignalingNaN);
    constructor['PositiveInfinity'] = constructor.PositiveInfinity = ExtendedFloat.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity);
    constructor['NegativeInfinity'] = constructor.NegativeInfinity = ExtendedFloat.CreateWithFlags(BigInteger.ZERO, BigInteger.ZERO, BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    prototype['IsPositiveInfinity'] = prototype.IsPositiveInfinity = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    };

    prototype['IsNegativeInfinity'] = prototype.IsNegativeInfinity = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    };

    prototype['IsNaN'] = prototype.IsNaN = function() {
        return (this.flags & (BigNumberFlags.FlagQuietNaN | BigNumberFlags.FlagSignalingNaN)) != 0;
    };

    prototype['IsInfinity'] = prototype.IsInfinity = function() {
        return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    };

    prototype['isFinite'] = prototype.isFinite = function() {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNaN)) == 0;
    };

    prototype['isNegative'] = prototype.isNegative = function() {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
    };

    prototype['IsQuietNaN'] = prototype.IsQuietNaN = function() {
        return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    };

    prototype['IsSignalingNaN'] = prototype.IsSignalingNaN = function() {
        return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    };

    prototype['signum'] = prototype.signum = function() {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) && this.unsignedMantissa.signum() == 0) ? 0 : (((this.flags & BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
    };

    prototype['isZero'] = prototype.isZero = function() {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) && this.unsignedMantissa.signum() == 0;
    };

    prototype['DivideToSameExponent'] = prototype.DivideToSameExponent = function(divisor, rounding) {
        return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    };

    prototype['Reduce'] = prototype.Reduce = function(ctx) {
        return ExtendedFloat.math.Reduce(this, ctx);
    };

    prototype['RemainderNaturalScale'] = prototype.RemainderNaturalScale = function(divisor, ctx) {
        return this.Subtract(this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null), ctx);
    };

    prototype['Divide'] = prototype.Divide = function(divisor, ctx) {
        return ExtendedFloat.math.Divide(this, divisor, ctx);
    };

    prototype['DivideToExponent'] = prototype.DivideToExponent = function(divisor, exponent, ctx) {
        return ExtendedFloat.math.DivideToExponent(this, divisor, exponent, ctx);
    };

    prototype['Abs'] = prototype.Abs = function(context) {
        return ExtendedFloat.math.Abs(this, context);
    };

    prototype['Negate'] = prototype.Negate = function(context) {
        return ExtendedFloat.math.Negate(this, context);
    };

    prototype['Subtract'] = prototype.Subtract = function(numberObject, ctx) {
        if (numberObject == null) {
            throw new Error("numberObject");
        }
        var negated = numberObject;
        if ((numberObject.flags & BigNumberFlags.FlagNaN) == 0) {
            var newflags = numberObject.flags ^ BigNumberFlags.FlagNegative;
            negated = ExtendedFloat.CreateWithFlags(numberObject.unsignedMantissa, numberObject.exponent, newflags);
        }
        return this.Add(negated, ctx);
    };
    constructor['math'] = constructor.math = new TrappableRadixMath(new RadixMath(new ExtendedFloat.BinaryMathHelper()));

    prototype['DivideToIntegerNaturalScale'] = prototype.DivideToIntegerNaturalScale = function(divisor, ctx) {
        return ExtendedFloat.math.DivideToIntegerNaturalScale(this, divisor, ctx);
    };

    prototype['DivideToIntegerZeroScale'] = prototype.DivideToIntegerZeroScale = function(divisor, ctx) {
        return ExtendedFloat.math.DivideToIntegerZeroScale(this, divisor, ctx);
    };

    prototype['Remainder'] = prototype.Remainder = function(divisor, ctx) {
        return ExtendedFloat.math.Remainder(this, divisor, ctx);
    };

    prototype['RemainderNear'] = prototype.RemainderNear = function(divisor, ctx) {
        return ExtendedFloat.math.RemainderNear(this, divisor, ctx);
    };

    prototype['NextMinus'] = prototype.NextMinus = function(ctx) {
        return ExtendedFloat.math.NextMinus(this, ctx);
    };

    prototype['NextPlus'] = prototype.NextPlus = function(ctx) {
        return ExtendedFloat.math.NextPlus(this, ctx);
    };

    prototype['NextToward'] = prototype.NextToward = function(otherValue, ctx) {
        return ExtendedFloat.math.NextToward(this, otherValue, ctx);
    };

    constructor['Max'] = constructor.Max = function(first, second, ctx) {
        return ExtendedFloat.math.Max(first, second, ctx);
    };

    constructor['Min'] = constructor.Min = function(first, second, ctx) {
        return ExtendedFloat.math.Min(first, second, ctx);
    };

    constructor['MaxMagnitude'] = constructor.MaxMagnitude = function(first, second, ctx) {
        return ExtendedFloat.math.MaxMagnitude(first, second, ctx);
    };

    constructor['MinMagnitude'] = constructor.MinMagnitude = function(first, second, ctx) {
        return ExtendedFloat.math.MinMagnitude(first, second, ctx);
    };

    prototype['compareTo'] = prototype.compareTo = function(other) {
        return ExtendedFloat.math.compareTo(this, other);
    };

    prototype['CompareToWithContext'] = prototype.CompareToWithContext = function(other, ctx) {
        return ExtendedFloat.math.CompareToWithContext(this, other, false, ctx);
    };

    prototype['CompareToSignal'] = prototype.CompareToSignal = function(other, ctx) {
        return ExtendedFloat.math.CompareToWithContext(this, other, true, ctx);
    };

    prototype['Add'] = prototype.Add = function(numberObject, ctx) {
        return ExtendedFloat.math.Add(this, numberObject, ctx);
    };

    prototype['Quantize'] = prototype.Quantize = function(otherValue, ctx) {
        return ExtendedFloat.math.Quantize(this, otherValue, ctx);
    };

    prototype['RoundToIntegralExact'] = prototype.RoundToIntegralExact = function(ctx) {
        return ExtendedFloat.math.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    };

    prototype['RoundToIntegralNoRoundedFlag'] = prototype.RoundToIntegralNoRoundedFlag = function(ctx) {
        return ExtendedFloat.math.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    };

    prototype['RoundToExponentExact'] = prototype.RoundToExponentExact = function(exponent, ctx) {
        return ExtendedFloat.math.RoundToExponentExact(this, exponent, ctx);
    };

    prototype['RoundToExponent'] = prototype.RoundToExponent = function(exponent, ctx) {
        return ExtendedFloat.math.RoundToExponentSimple(this, exponent, ctx);
    };

    prototype['Multiply'] = prototype.Multiply = function(op, ctx) {
        return ExtendedFloat.math.Multiply(this, op, ctx);
    };

    prototype['MultiplyAndAdd'] = prototype.MultiplyAndAdd = function(op, augend, ctx) {
        return ExtendedFloat.math.MultiplyAndAdd(this, op, augend, ctx);
    };

    prototype['MultiplyAndSubtract'] = prototype.MultiplyAndSubtract = function(op, subtrahend, ctx) {
        if (subtrahend == null) {
            throw new Error("numberObject");
        }
        var negated = subtrahend;
        if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
            var newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
            negated = ExtendedFloat.CreateWithFlags(subtrahend.unsignedMantissa, subtrahend.exponent, newflags);
        }
        return ExtendedFloat.math.MultiplyAndAdd(this, op, negated, ctx);
    };

    prototype['RoundToPrecision'] = prototype.RoundToPrecision = function(ctx) {
        return ExtendedFloat.math.RoundToPrecision(this, ctx);
    };

    prototype['Plus'] = prototype.Plus = function(ctx) {
        return ExtendedFloat.math.Plus(this, ctx);
    };

    prototype['RoundToBinaryPrecision'] = prototype.RoundToBinaryPrecision = function(ctx) {
        return ExtendedFloat.math.RoundToBinaryPrecision(this, ctx);
    };

    prototype['SquareRoot'] = prototype.SquareRoot = function(ctx) {
        return ExtendedFloat.math.SquareRoot(this, ctx);
    };

    prototype['Exp'] = prototype.Exp = function(ctx) {
        return ExtendedFloat.math.Exp(this, ctx);
    };

    prototype['Log'] = prototype.Log = function(ctx) {
        return ExtendedFloat.math.Ln(this, ctx);
    };

    prototype['Log10'] = prototype.Log10 = function(ctx) {
        return ExtendedFloat.math.Log10(this, ctx);
    };

    prototype['Pow'] = prototype.Pow = function(exponent, ctx) {
        return ExtendedFloat.math.Power(this, exponent, ctx);
    };

    constructor['PI'] = constructor.PI = function(ctx) {
        return ExtendedFloat.math.Pi(ctx);
    };
})(ExtendedFloat,ExtendedFloat.prototype);

if(typeof exports!=="undefined")exports['ExtendedFloat']=ExtendedFloat;
if(typeof window!=="undefined")window['ExtendedFloat']=ExtendedFloat;
})();
