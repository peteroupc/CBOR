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
ILong.prototype.intValue=function(other){
return this.lo|0;
}
ILong.prototype.shortValue=function(other){
return (this.lo|0)&0xFFFF;
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
    constructor.CountWords = function(X, N) {
        while (N != 0 && X[N - 1] == 0) N--;
        return (N|0);
    };
    constructor.SetWords = function(r, rstart, a, n) {
        for (var i = 0; i < n; i++) r[rstart + i] = (((a) & 65535)|0);
    };
    constructor.ShiftWordsLeftByBits = function(r, rstart, n, shiftBits) {
        {
            var u, carry = 0;
            if (shiftBits != 0) {
                for (var i = 0; i < n; i++) {
                    u = r[rstart + i];
                    r[rstart + i] = ((((((((((u << (shiftBits|0))|0) | (carry & 65535)))|0)) & 65535))|0));
                    carry = (((u & 65535) >> ((16 - shiftBits)|0))|0);
                }
            }
            return carry;
        }
    };
    constructor.ShiftWordsRightByBits = function(r, rstart, n, shiftBits) {
        var u, carry = 0;
        {
            if (shiftBits != 0) for (var i = n; i > 0; i--) {
                u = r[rstart + i - 1];
                r[rstart + i - 1] = (((((((((((u & 65535) >> (shiftBits|0)) & 65535) | (carry & 65535)))|0)) & 65535))|0));
                carry = (((u & 65535) << ((16 - shiftBits)|0))|0);
            }
            return carry;
        }
    };
    constructor.ShiftWordsRightByBitsSignExtend = function(r, rstart, n, shiftBits) {
        {
            var u, carry = ((65535 << ((16 - shiftBits)|0))|0);
            if (shiftBits != 0) for (var i = n; i > 0; i--) {
                u = r[rstart + i - 1];
                r[rstart + i - 1] = ((((((((((u & 65535) >> (shiftBits|0)) | (carry & 65535)))|0)) & 65535))|0));
                carry = (((u & 65535) << ((16 - shiftBits)|0))|0);
            }
            return carry;
        }
    };
    constructor.ShiftWordsLeftByWords = function(r, rstart, n, shiftWords) {
        shiftWords = (shiftWords < n ? shiftWords : n);
        if (shiftWords != 0) {
            for (var i = n - 1; i >= shiftWords; i--) r[rstart + i] = (((r[rstart + i - shiftWords]) & 65535)|0);
            BigInteger.SetWords(r, rstart, 0, shiftWords);
        }
    };
    constructor.ShiftWordsRightByWords = function(r, rstart, n, shiftWords) {
        shiftWords = (shiftWords < n ? shiftWords : n);
        if (shiftWords != 0) {
            for (var i = 0; i + shiftWords < n; i++) r[rstart + i] = (((r[rstart + i + shiftWords]) & 65535)|0);
            BigInteger.SetWords(r, ((rstart + n - shiftWords)|0), 0, shiftWords);
        }
    };
    constructor.ShiftWordsRightByWordsSignExtend = function(r, rstart, n, shiftWords) {
        shiftWords = (shiftWords < n ? shiftWords : n);
        if (shiftWords != 0) {
            for (var i = 0; i + shiftWords < n; i++) r[rstart + i] = (((r[rstart + i + shiftWords]) & 65535)|0);
            BigInteger.SetWords(r, ((rstart + n - shiftWords)|0), (65535), shiftWords);
        }
    };
    constructor.Compare = function(A, astart, B, bstart, N) {
        while ((N--) != 0) {
            var an = (((A[astart + N])|0) & 65535);
            var bn = (((B[bstart + N])|0) & 65535);
            if (an > bn) return 1; else if (an < bn) return -1;
        }
        return 0;
    };
    constructor.Increment = function(A, Astart, N, B) {
        {
            var tmp = A[Astart];
            A[Astart] = (((((tmp + B)|0) & 65535)|0));
            if ((((A[Astart])|0) & 65535) >= (tmp & 65535)) return 0;
            for (var i = 1; i < N; i++) {
                A[Astart + i] = (((A[Astart + i] + 1) & 65535)|0);
                if (A[Astart + i] != 0) return 0;
            }
            return 1;
        }
    };
    constructor.Decrement = function(A, Astart, N, B) {
        {
            var tmp = A[Astart];
            A[Astart] = (((((tmp - B)|0) & 65535)|0));
            if ((((A[Astart])|0) & 65535) <= (tmp & 65535)) return 0;
            for (var i = 1; i < N; i++) {
                tmp = A[Astart + i];
                A[Astart + i] = (((A[Astart + i] - 1) & 65535)|0);
                if (tmp != 0) return 0;
            }
            return 1;
        }
    };
    constructor.TwosComplement = function(A, Astart, N) {
        BigInteger.Decrement(A, Astart, N, 1);
        for (var i = 0; i < N; i++) A[Astart + i] = (((((~A[Astart + i])|0) & 65535)|0));
    };
    constructor.Add = function(C, cstart, A, astart, B, bstart, N) {
        {
            var u;
            u = 0;
            for (var i = 0; i < N; i += 2) {
                u = (((A[astart + i])|0) & 65535) + (((B[bstart + i])|0) & 65535) + ((u >> 16)|0);
                C[cstart + i] = (((((u & 65535) & 65535))|0));
                u = (((A[astart + i + 1])|0) & 65535) + (((B[bstart + i + 1])|0) & 65535) + ((u >> 16)|0);
                C[cstart + i + 1] = (((((u & 65535) & 65535))|0));
            }
            return ((u|0) >>> 16);
        }
    };
    constructor.Subtract = function(C, cstart, A, astart, B, bstart, N) {
        {
            var u;
            u = 0;
            for (var i = 0; i < N; i += 2) {
                u = (((A[astart + i])|0) & 65535) - (((B[bstart + i])|0) & 65535) - (((u >> 31) & 1)|0);
                C[cstart + i] = (((((u & 65535) & 65535))|0));
                u = (((A[astart + i + 1])|0) & 65535) - (((B[bstart + i + 1])|0) & 65535) - (((u >> 31) & 1)|0);
                C[cstart + i + 1] = (((((u & 65535) & 65535))|0));
            }
            return (((u >> 31) & 1)|0);
        }
    };
    constructor.LinearMultiply = function(productArr, cstart, A, astart, B, N) {
        {
            var carry = 0;
            var Bint = (B & 65535);
            for (var i = 0; i < N; i++) {
                var p;
                p = (((A[astart + i])|0) & 65535) * Bint;
                p = p + (carry & 65535);
                productArr[cstart + i] = (((((p & 65535) & 65535))|0));
                carry = ((p >> 16)|0);
            }
            return carry;
        }
    };
    constructor.Baseline_Square2 = function(R, rstart, A, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (((A[astart])|0) & 65535) * (((A[astart])|0) & 65535);
            R[rstart] = (((((p & 65535) & 65535))|0));
            e = ((p|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2 * 2 - 3] = (((c) & 65535)|0);
            p = (((A[astart + 2 - 1])|0) & 65535) * (((A[astart + 2 - 1])|0) & 65535);
            p += e;
            R[rstart + 2 * 2 - 2] = (((((p & 65535) & 65535))|0));
            R[rstart + 2 * 2 - 1] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Square4 = function(R, rstart, A, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (((A[astart])|0) & 65535) * (((A[astart])|0) & 65535);
            R[rstart] = (((((p & 65535) & 65535))|0));
            e = ((p|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2 * 4 - 3] = (((c) & 65535)|0);
            p = (((A[astart + 4 - 1])|0) & 65535) * (((A[astart + 4 - 1])|0) & 65535);
            p += e;
            R[rstart + 2 * 4 - 2] = (((((p & 65535) & 65535))|0));
            R[rstart + 2 * 4 - 1] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Square8 = function(R, rstart, A, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (((A[astart])|0) & 65535) * (((A[astart])|0) & 65535);
            R[rstart] = (((((p & 65535) & 65535))|0));
            e = ((p|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 5] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 6] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 7] = (((c) & 65535)|0);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 8] = (((c) & 65535)|0);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 9] = (((c) & 65535)|0);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 10] = (((c) & 65535)|0);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 11] = (((c) & 65535)|0);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 12] = (((c) & 65535)|0);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2 * 8 - 3] = (((c) & 65535)|0);
            p = (((A[astart + 8 - 1])|0) & 65535) * (((A[astart + 8 - 1])|0) & 65535);
            p += e;
            R[rstart + 2 * 8 - 2] = (((((p & 65535) & 65535))|0));
            R[rstart + 2 * 8 - 1] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Square16 = function(R, rstart, A, astart) {
        {
            var p;
            var c;
            var d;
            var e;
            p = (((A[astart])|0) & 65535) * (((A[astart])|0) & 65535);
            R[rstart] = (((((p & 65535) & 65535))|0));
            e = ((p|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 5] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 6] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 7] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 8] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 9] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 10] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 11] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 12] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 13] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 14] = (((c) & 65535)|0);
            p = (((A[astart])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 15] = (((c) & 65535)|0);
            p = (((A[astart + 1])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 16] = (((c) & 65535)|0);
            p = (((A[astart + 2])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 17] = (((c) & 65535)|0);
            p = (((A[astart + 3])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 18] = (((c) & 65535)|0);
            p = (((A[astart + 4])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 19] = (((c) & 65535)|0);
            p = (((A[astart + 5])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 20] = (((c) & 65535)|0);
            p = (((A[astart + 6])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 21] = (((c) & 65535)|0);
            p = (((A[astart + 7])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 11])|0) & 65535) * (((A[astart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 22] = (((c) & 65535)|0);
            p = (((A[astart + 8])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 23] = (((c) & 65535)|0);
            p = (((A[astart + 9])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 12])|0) & 65535) * (((A[astart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 24] = (((c) & 65535)|0);
            p = (((A[astart + 10])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 25] = (((c) & 65535)|0);
            p = (((A[astart + 11])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 13])|0) & 65535) * (((A[astart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 26] = (((c) & 65535)|0);
            p = (((A[astart + 12])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 27] = (((c) & 65535)|0);
            p = (((A[astart + 13])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            p = (((A[astart + 14])|0) & 65535) * (((A[astart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 28] = (((c) & 65535)|0);
            p = (((A[astart + 14])|0) & 65535) * (((A[astart + 15])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            d = ((((d << 1) + (((c|0) >> 15) & 1)))|0);
            c <<= 1;
            e = e + (c & 65535);
            c = (e & 65535);
            e = d + ((e|0) >>> 16);
            R[rstart + 2 * 16 - 3] = (((c) & 65535)|0);
            p = (((A[astart + 16 - 1])|0) & 65535) * (((A[astart + 16 - 1])|0) & 65535);
            p += e;
            R[rstart + 2 * 16 - 2] = (((((p & 65535) & 65535))|0));
            R[rstart + 2 * 16 - 1] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Multiply2 = function(R, rstart, A, astart, B, bstart) {
        {
            var p;
            var c;
            var d;
            p = (((A[astart])|0) & 65535) * (((B[bstart])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            R[rstart] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p += d;
            R[rstart + 1 + 1] = (((((p & 65535) & 65535))|0));
            R[rstart + 1 + 2] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Multiply4 = function(R, rstart, A, astart, B, bstart) {
        {
            var p;
            var c;
            var d;
            p = (((A[astart])|0) & 65535) * (((B[bstart])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            R[rstart] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 5] = (((c) & 65535)|0);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p += d;
            R[rstart + 5 + 1] = (((((p & 65535) & 65535))|0));
            R[rstart + 5 + 2] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Multiply8 = function(R, rstart, A, astart, B, bstart) {
        {
            var p;
            var c;
            var d;
            p = (((A[astart])|0) & 65535) * (((B[bstart])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            R[rstart] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 5] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 6] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 7] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 8] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 9] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 10] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 11] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 12] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 13] = (((c) & 65535)|0);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p += d;
            R[rstart + 13 + 1] = (((((p & 65535) & 65535))|0));
            R[rstart + 13 + 2] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.Baseline_Multiply16 = function(R, rstart, A, astart, B, bstart) {
        {
            var p;
            var c;
            var d;
            p = (((A[astart])|0) & 65535) * (((B[bstart])|0) & 65535);
            c = (p & 65535);
            d = ((p|0) >>> 16);
            R[rstart] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 1] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 2] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 3] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 4] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 5] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 6] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 7] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 8] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 9] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 10] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 11] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 12] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 13] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 14] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 15] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 1])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 1])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 16] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 2])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 2])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 17] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 3])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 3])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 18] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 4])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 4])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 19] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 5])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 5])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 20] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 6])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 6])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 21] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 7])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 7])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 22] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 8])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 8])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 23] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 9])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 9])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 24] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 10])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 10])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 25] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 11])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 11])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 26] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 12])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 12])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 27] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 13])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 13])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 28] = (((c) & 65535)|0);
            c = (d & 65535);
            d = ((d|0) >>> 16);
            p = (((A[astart + 14])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 14])|0) & 65535);
            p = p + (c & 65535);
            c = (p & 65535);
            d = d + ((p|0) >>> 16);
            R[rstart + 29] = (((c) & 65535)|0);
            p = (((A[astart + 15])|0) & 65535) * (((B[bstart + 15])|0) & 65535);
            p += d;
            R[rstart + 30] = (((((p & 65535) & 65535))|0));
            R[rstart + 31] = (((((p >> 16)|0) & 65535)|0));
        }
    };
    constructor.s_recursionLimit = 16;
    constructor.RecursiveMultiply = function(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, N) {
        if (N <= BigInteger.s_recursionLimit) {
            N >>= 2;
            switch(N) {
                case 0:
                    BigInteger.Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
                    break;
                case 1:
                    BigInteger.Baseline_Multiply4(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
                    break;
                case 2:
                    BigInteger.Baseline_Multiply8(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
                    break;
                case 4:
                    BigInteger.Baseline_Multiply16(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
                    break;
                default:
                    throw "exception";
            }
        } else {
            var N2 = ((N / 2)|0);
            var AN2 = BigInteger.Compare(Aarr, Astart, Aarr, ((Astart + N2)|0), N2) > 0 ? 0 : N2;
            BigInteger.Subtract(Rarr, Rstart, Aarr, ((Astart + AN2)|0), Aarr, ((Astart + (N2 ^ AN2))|0), N2);
            var BN2 = BigInteger.Compare(Barr, Bstart, Barr, ((Bstart + N2)|0), N2) > 0 ? 0 : N2;
            BigInteger.Subtract(Rarr, ((Rstart + N2)|0), Barr, ((Bstart + BN2)|0), Barr, ((Bstart + (N2 ^ BN2))|0), N2);
            BigInteger.RecursiveMultiply(Rarr, ((Rstart + N)|0), Tarr, ((Tstart + N)|0), Aarr, ((Astart + N2)|0), Barr, ((Bstart + N2)|0), N2);
            BigInteger.RecursiveMultiply(Tarr, Tstart, Tarr, ((Tstart + N)|0), Rarr, Rstart, Rarr, ((Rstart + N2)|0), N2);
            BigInteger.RecursiveMultiply(Rarr, Rstart, Tarr, ((Tstart + N)|0), Aarr, Astart, Barr, Bstart, N2);
            var c2 = BigInteger.Add(Rarr, ((Rstart + N)|0), Rarr, ((Rstart + N)|0), Rarr, ((Rstart + N2)|0), N2);
            var c3 = c2;
            c2 += BigInteger.Add(Rarr, ((Rstart + N2)|0), Rarr, ((Rstart + N)|0), Rarr, (Rstart), N2);
            c3 += BigInteger.Add(Rarr, ((Rstart + N)|0), Rarr, ((Rstart + N)|0), Rarr, ((Rstart + N + N2)|0), N2);
            if (AN2 == BN2) c3 -= BigInteger.Subtract(Rarr, ((Rstart + N2)|0), Rarr, ((Rstart + N2)|0), Tarr, Tstart, N); else c3 += BigInteger.Add(Rarr, ((Rstart + N2)|0), Rarr, ((Rstart + N2)|0), Tarr, Tstart, N);
            c3 += BigInteger.Increment(Rarr, ((Rstart + N)|0), N2, (c2|0));
            BigInteger.Increment(Rarr, ((Rstart + N + N2)|0), N2, (c3|0));
        }
    };
    constructor.RecursiveSquare = function(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, N) {
        if (N <= BigInteger.s_recursionLimit) {
            N >>= 2;
            switch(N) {
                case 0:
                    BigInteger.Baseline_Square2(Rarr, Rstart, Aarr, Astart);
                    break;
                case 1:
                    BigInteger.Baseline_Square4(Rarr, Rstart, Aarr, Astart);
                    break;
                case 2:
                    BigInteger.Baseline_Square8(Rarr, Rstart, Aarr, Astart);
                    break;
                case 4:
                    BigInteger.Baseline_Square16(Rarr, Rstart, Aarr, Astart);
                    break;
                default:
                    throw "exception";
            }
        } else {
            var N2 = ((N / 2)|0);
            BigInteger.RecursiveSquare(Rarr, Rstart, Tarr, ((Tstart + N)|0), Aarr, Astart, N2);
            BigInteger.RecursiveSquare(Rarr, ((Rstart + N)|0), Tarr, ((Tstart + N)|0), Aarr, ((Astart + N2)|0), N2);
            BigInteger.RecursiveMultiply(Tarr, Tstart, Tarr, ((Tstart + N)|0), Aarr, Astart, Aarr, ((Astart + N2)|0), N2);
            var carry = BigInteger.Add(Rarr, ((Rstart + N2)|0), Rarr, ((Rstart + N2)|0), Tarr, Tstart, N);
            carry += BigInteger.Add(Rarr, ((Rstart + N2)|0), Rarr, ((Rstart + N2)|0), Tarr, Tstart, N);
            BigInteger.Increment(Rarr, ((Rstart + N + N2)|0), N2, (carry|0));
        }
    };
    constructor.Multiply = function(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, N) {
        BigInteger.RecursiveMultiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, N);
    };
    constructor.Square = function(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, N) {
        BigInteger.RecursiveSquare(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, N);
    };
    constructor.AsymmetricMultiply = function(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, NA, Barr, Bstart, NB) {
        if (NA == NB) {
            if (Astart == Bstart && Aarr == Barr) {
                BigInteger.Square(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, NA);
            } else if (NA == 2) BigInteger.Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart); else BigInteger.Multiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);
            return;
        }
        if (NA > NB) {
            var tmp1 = Aarr;
            Aarr = Barr;
            Barr = tmp1;
            var tmp3 = Astart;
            Astart = Bstart;
            Bstart = tmp3;
            var tmp2 = NA;
            NA = NB;
            NB = tmp2;
        }
        if (NA == 2 && Aarr[Astart + 1] == 0) {
            switch(Aarr[Astart]) {
                case 0:
                    BigInteger.SetWords(Rarr, Rstart, 0, NB + 2);
                    return;
                case 1:
                    for (var arrfillI = 0; arrfillI < (NB|0); arrfillI++) Rarr[Rstart + arrfillI] = Barr[Bstart + arrfillI];
                    Rarr[Rstart + NB] = (((0) & 65535)|0);
                    Rarr[Rstart + NB + 1] = (((0) & 65535)|0);
                    return;
                default:
                    Rarr[Rstart + NB] = ((((BigInteger.LinearMultiply(Rarr, Rstart, Barr, Bstart, Aarr[Astart], NB)) & 65535)|0));
                    Rarr[Rstart + NB + 1] = (((0) & 65535)|0);
                    return;
            }
        }
        if (NA == 2) {
            var i;
            if (((NB / 2)|0) % 2 == 0) {
                BigInteger.Baseline_Multiply2(Rarr, Rstart, Aarr, Astart, Barr, Bstart);
                for (var arrfillI = 0; arrfillI < 2; arrfillI++) Tarr[((Tstart + 2 * 2)|0) + arrfillI] = Rarr[((Rstart + 2)|0) + arrfillI];
                for (i = 2 * 2; i < NB; i += 2 * 2) BigInteger.Baseline_Multiply2(Tarr, ((Tstart + 2 + i)|0), Aarr, Astart, Barr, ((Bstart + i)|0));
                for (i = 2; i < NB; i += 2 * 2) BigInteger.Baseline_Multiply2(Rarr, ((Rstart + i)|0), Aarr, Astart, Barr, ((Bstart + i)|0));
            } else {
                for (i = 0; i < NB; i += 2 * 2) BigInteger.Baseline_Multiply2(Rarr, ((Rstart + i)|0), Aarr, Astart, Barr, ((Bstart + i)|0));
                for (i = 2; i < NB; i += 2 * 2) BigInteger.Baseline_Multiply2(Tarr, ((Tstart + 2 + i)|0), Aarr, Astart, Barr, ((Bstart + i)|0));
            }
        } else {
            var i;
            if (((NB / NA)|0) % 2 == 0) {
                BigInteger.Multiply(Rarr, Rstart, Tarr, Tstart, Aarr, Astart, Barr, Bstart, NA);
                for (var arrfillI = 0; arrfillI < (NA|0); arrfillI++) Tarr[((Tstart + 2 * NA)|0) + arrfillI] = Rarr[((Rstart + NA)|0) + arrfillI];
                for (i = 2 * NA; i < NB; i += 2 * NA) BigInteger.Multiply(Tarr, ((Tstart + NA + i)|0), Tarr, Tstart, Aarr, Astart, Barr, ((Bstart + i)|0), NA);
                for (i = NA; i < NB; i += 2 * NA) BigInteger.Multiply(Rarr, ((Rstart + i)|0), Tarr, Tstart, Aarr, Astart, Barr, ((Bstart + i)|0), NA);
            } else {
                for (i = 0; i < NB; i += 2 * NA) BigInteger.Multiply(Rarr, ((Rstart + i)|0), Tarr, Tstart, Aarr, Astart, Barr, ((Bstart + i)|0), NA);
                for (i = NA; i < NB; i += 2 * NA) BigInteger.Multiply(Tarr, ((Tstart + NA + i)|0), Tarr, Tstart, Aarr, Astart, Barr, ((Bstart + i)|0), NA);
            }
        }
        if (BigInteger.Add(Rarr, ((Rstart + NA)|0), Rarr, ((Rstart + NA)|0), Tarr, ((Tstart + 2 * NA)|0), NB - NA) != 0) BigInteger.Increment(Rarr, ((Rstart + NB)|0), NA, 1);
    };
    constructor.MakeUint = function(first, second) {
        return ((((first & 65535) | ((second|0) << 16))|0));
    };
    constructor.GetLowHalf = function(val) {
        return (val & 65535);
    };
    constructor.GetHighHalf = function(val) {
        return ((val >>> 16)|0);
    };
    constructor.GetHighHalfAsBorrow = function(val) {
        return ((0 - (val >>> 16))|0);
    };
    constructor.BitPrecision = function(numberValue) {
        if (numberValue == 0) return 0;
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
            if ((numberValue >> 15) == 0) --i;
        }
        return i;
    };
    constructor.BitPrecisionInt = function(numberValue) {
        if (numberValue == 0) return 0;
        var i = 32;
        {
            if ((numberValue >> 16) == 0) {
                numberValue <<= 16;
                i -= 16;
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
    constructor.Divide32By16 = function(dividendLow, divisorShort, returnRemainder) {
        var tmpInt;
        var dividendHigh = 0;
        var intDivisor = (divisorShort & 65535);
        for (var i = 0; i < 32; i++) {
            tmpInt = dividendHigh >> 31;
            dividendHigh <<= 1;
            dividendHigh = ((((dividendHigh | (((dividendLow >> 31) & 1)|0)))|0));
            dividendLow <<= 1;
            tmpInt |= dividendHigh;
            if (((tmpInt >> 31) != 0) || (tmpInt >= intDivisor)) {
                {
                    dividendHigh -= intDivisor;
                    dividendLow += 1;
                }
            }
        }
        return (returnRemainder ? (dividendHigh & 65535) : (dividendLow & 65535));
    };
    constructor.DivideUnsigned = function(x, y) {
        {
            var iy = (y & 65535);
            if ((x >> 31) == 0) {
                return ((((((x|0) / iy)|0) & 65535))|0);
            } else {
                return BigInteger.Divide32By16(x, y, false);
            }
        }
    };
    constructor.RemainderUnsigned = function(x, y) {
        {
            var iy = (y & 65535);
            if ((x >> 31) == 0) {
                return ((((x|0) % iy) & 65535)|0);
            } else {
                return BigInteger.Divide32By16(x, y, true);
            }
        }
    };
    constructor.DivideThreeWordsByTwo = function(A, Astart, B0, B1) {
        var Q;
        {
            if (((B1 + 1)|0) == 0) Q = A[Astart + 2]; else if ((B1 & 65535) > 0) Q = BigInteger.DivideUnsigned(BigInteger.MakeUint(A[Astart + 1], A[Astart + 2]), ((((B1|0) + 1) & 65535)|0)); else Q = BigInteger.DivideUnsigned(BigInteger.MakeUint(A[Astart], A[Astart + 1]), B0);
            var Qint = (Q & 65535);
            var B0int = (B0 & 65535);
            var B1int = (B1 & 65535);
            var p = B0int * Qint;
            var u = (((A[Astart])|0) & 65535) - (((BigInteger.GetLowHalf(p))|0) & 65535);
            A[Astart] = ((((BigInteger.GetLowHalf(u)) & 65535)|0));
            u = (((A[Astart + 1])|0) & 65535) - (((BigInteger.GetHighHalf(p))|0) & 65535) - (((BigInteger.GetHighHalfAsBorrow(u))|0) & 65535) - (B1int * Qint);
            A[Astart + 1] = ((((BigInteger.GetLowHalf(u)) & 65535)|0));
            A[Astart + 2] = ((((A[Astart + 2] + BigInteger.GetHighHalf(u)) & 65535)|0));
            while (A[Astart + 2] != 0 || (((A[Astart + 1])|0) & 65535) > (B1 & 65535) || (A[Astart + 1] == B1 && (((A[Astart])|0) & 65535) >= (B0 & 65535))) {
                u = (((A[Astart])|0) & 65535) - B0int;
                A[Astart] = ((((BigInteger.GetLowHalf(u)) & 65535)|0));
                u = (((A[Astart + 1])|0) & 65535) - B1int - (((BigInteger.GetHighHalfAsBorrow(u))|0) & 65535);
                A[Astart + 1] = ((((BigInteger.GetLowHalf(u)) & 65535)|0));
                A[Astart + 2] = ((((A[Astart + 2] + BigInteger.GetHighHalf(u)) & 65535)|0));
                Q++;
            }
        }
        return Q;
    };
    constructor.DivideFourWordsByTwo = function(T, Al, Ah, B) {
        if (B == 0) return BigInteger.MakeUint(BigInteger.GetLowHalf(Al), BigInteger.GetHighHalf(Ah)); else {
            var Q = [0, 0];
            T[0] = ((((BigInteger.GetLowHalf(Al)) & 65535)|0));
            T[1] = ((((BigInteger.GetHighHalf(Al)) & 65535)|0));
            T[2] = ((((BigInteger.GetLowHalf(Ah)) & 65535)|0));
            T[3] = ((((BigInteger.GetHighHalf(Ah)) & 65535)|0));
            Q[1] = (((((BigInteger.DivideThreeWordsByTwo(T, 1, BigInteger.GetLowHalf(B), BigInteger.GetHighHalf(B))) & 65535))|0));
            Q[0] = (((((BigInteger.DivideThreeWordsByTwo(T, 0, BigInteger.GetLowHalf(B), BigInteger.GetHighHalf(B))) & 65535))|0));
            return BigInteger.MakeUint(Q[0], Q[1]);
        }
    };
    constructor.AtomicDivide = function(Q, Qstart, A, Astart, B, Bstart) {
        var T = [0, 0, 0, 0];
        var q = BigInteger.DivideFourWordsByTwo(T, BigInteger.MakeUint(A[Astart], A[Astart + 1]), BigInteger.MakeUint(A[Astart + 2], A[Astart + 3]), BigInteger.MakeUint(B[Bstart], B[Bstart + 1]));
        Q[Qstart] = ((((BigInteger.GetLowHalf(q)) & 65535)|0));
        Q[Qstart + 1] = ((((BigInteger.GetHighHalf(q)) & 65535)|0));
    };
    constructor.CorrectQuotientEstimate = function(Rarr, Rstart, Tarr, Tstart, Qarr, Qstart, Barr, Bstart, N) {
        {
            if (N == 2) BigInteger.Baseline_Multiply2(Tarr, Tstart, Qarr, Qstart, Barr, Bstart); else BigInteger.AsymmetricMultiply(Tarr, Tstart, Tarr, ((Tstart + (N + 2))|0), Qarr, Qstart, 2, Barr, Bstart, N);
            BigInteger.Subtract(Rarr, Rstart, Rarr, Rstart, Tarr, Tstart, N + 2);
            while (Rarr[Rstart + N] != 0 || BigInteger.Compare(Rarr, Rstart, Barr, Bstart, N) >= 0) {
                Rarr[Rstart + N] = (((((Rarr[Rstart + N] - ((BigInteger.Subtract(Rarr, Rstart, Rarr, Rstart, Barr, Bstart, N))|0)) & 65535))|0));
                Qarr[Qstart] = (((Qarr[Qstart] + 1) & 65535)|0);
                Qarr[Qstart + 1] = (((((Qarr[Qstart + 1] + (((Qarr[Qstart] == 0) ? 1 : 0)|0)) & 65535))|0));
            }
        }
    };
    constructor.Divide = function(Rarr, Rstart, Qarr, Qstart, TA, Tstart, Aarr, Astart, NAint, Barr, Bstart, NBint) {
        var NA = (NAint|0);
        var NB = (NBint|0);
        var TBarr = TA;
        var TParr = TA;
        var TBstart = ((Tstart + (NA + 2))|0);
        var TPstart = ((Tstart + (NA + 2 + NB))|0);
        {
            var shiftWords = ((Barr[Bstart + NB - 1] == 0 ? 1 : 0)|0);
            TBarr[TBstart] = (((0) & 65535)|0);
            TBarr[TBstart + NB - 1] = (((0) & 65535)|0);
            for (var arrfillI = 0; arrfillI < NB - shiftWords; arrfillI++) TBarr[((TBstart + shiftWords)|0) + arrfillI] = Barr[Bstart + arrfillI];
            var shiftBits = ((16 - BigInteger.BitPrecision(TBarr[TBstart + NB - 1]))|0);
            BigInteger.ShiftWordsLeftByBits(TBarr, TBstart, NB, shiftBits);
            TA[0] = (((0) & 65535)|0);
            TA[NA] = (((0) & 65535)|0);
            TA[NA + 1] = (((0) & 65535)|0);
            for (var arrfillI = 0; arrfillI < NAint; arrfillI++) TA[((Tstart + shiftWords)|0) + arrfillI] = Aarr[Astart + arrfillI];
            BigInteger.ShiftWordsLeftByBits(TA, Tstart, NA + 2, shiftBits);
            if (TA[Tstart + NA + 1] == 0 && (((TA[Tstart + NA])|0) & 65535) <= 1) {
                Qarr[Qstart + NA - NB + 1] = (((0) & 65535)|0);
                Qarr[Qstart + NA - NB] = (((0) & 65535)|0);
                while (TA[NA] != 0 || BigInteger.Compare(TA, ((Tstart + NA - NB)|0), TBarr, TBstart, NB) >= 0) {
                    TA[NA] = (((((TA[NA] - ((BigInteger.Subtract(TA, ((Tstart + NA - NB)|0), TA, ((Tstart + NA - NB)|0), TBarr, TBstart, NB))|0)) & 65535))|0));
                    Qarr[Qstart + NA - NB] = (((Qarr[Qstart + NA - NB] + 1) & 65535)|0);
                }
            } else {
                NA += 2;
            }
            var BT = [0, 0];
            BT[0] = (((((TBarr[TBstart + NB - 2] + 1)|0) & 65535)|0));
            BT[1] = ((((((TBarr[TBstart + NB - 1] + ((BT[0] == 0 ? 1 : 0)|0))|0) & 65535))|0));
            for (var i = NA - 2; i >= NB; i -= 2) {
                BigInteger.AtomicDivide(Qarr, ((Qstart + i - NB)|0), TA, ((Tstart + i - 2)|0), BT, 0);
                BigInteger.CorrectQuotientEstimate(TA, ((Tstart + i - NB)|0), TParr, TPstart, Qarr, ((Qstart + (i - NB))|0), TBarr, TBstart, NB);
            }
            if (Rarr != null) {
                for (var arrfillI = 0; arrfillI < NB; arrfillI++) Rarr[Rstart + arrfillI] = TA[((Tstart + shiftWords)|0) + arrfillI];
                BigInteger.ShiftWordsRightByBits(Rarr, Rstart, NB, shiftBits);
            }
        }
    };
    constructor.RoundupSizeTable = [2, 2, 2, 4, 4, 8, 8, 8, 8];
    constructor.RoundupSize = function(n) {
        if (n <= 8) return BigInteger.RoundupSizeTable[n]; else if (n <= 16) return 16; else if (n <= 32) return 32; else if (n <= 64) return 64; else return 1 << ((BigInteger.BitPrecisionInt(n - 1))|0);
    };
    prototype.negative = null;
    prototype.wordCount = -1;
    prototype.reg = null;
    
    constructor.fromByteArray = function(bytes, littleEndian) {
        var bigint = new BigInteger();
        bigint.fromByteArrayInternal(bytes, littleEndian);
        return bigint;
    };
    prototype.fromByteArrayInternal = function(bytes, littleEndian) {
        if (bytes == null) throw ("bytes");
        if (bytes.length == 0) {
            this.reg = [0, 0];
            this.wordCount = 0;
        } else {
            this.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((((((bytes.length)|0) + 1) / 2)|0)); arrfillI++) this.reg[arrfillI] = 0;
            var jIndex = (littleEndian) ? bytes.length - 1 : 0;
            var negative = ((bytes[jIndex]) & 128) != 0;
            var j = 0;
            for (var i = 0; i < bytes.length; i += 2, j++) {
                var index = (littleEndian) ? i : bytes.length - 1 - i;
                var index2 = (littleEndian) ? i + 1 : bytes.length - 2 - i;
                this.reg[j] = (((((((((bytes[index])|0) & 255)|0)) & 65535))|0));
                if (index2 >= 0 && index2 < bytes.length) {
                    this.reg[j] = (((((this.reg[j] | (((((bytes[index2])|0) << 8)|0))) & 65535))|0));
                } else if (negative) {
                    
                    this.reg[j] = ((((this.reg[j] | (65280)) & 65535)|0));
                }
            }
            this.negative = negative;
            if (negative) {
                for (; j < this.reg.length; j++) {
                    this.reg[j] = (((65535) & 65535)|0);
                }
                
                BigInteger.TwosComplement(this.reg, 0, ((this.reg.length)|0));
            }
            this.wordCount = this.CalcWordCount();
        }
    };
    prototype.Allocate = function(length) {
        this.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(length); arrfillI++) this.reg[arrfillI] = 0;
        
        this.negative = false;
        this.wordCount = 0;
        return this;
    };
    constructor.GrowForCarry = function(a, carry) {
        var oldLength = a.length;
        var ret = BigInteger.CleanGrow(a, BigInteger.RoundupSize(oldLength + 1));
        ret[oldLength] = (((carry) & 65535)|0);
        return ret;
    };
    constructor.CleanGrow = function(a, size) {
        if (size > a.length) {
            var newa = [];
            for (var arrfillI = 0; arrfillI < size; arrfillI++) newa[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < a.length; arrfillI++) newa[0 + arrfillI] = a[0 + arrfillI];
            return newa;
        }
        return a;
    };
    prototype.SetBitInternal = function(n, value) {
        if (value) {
            this.reg = BigInteger.CleanGrow(this.reg, BigInteger.RoundupSize(BigInteger.BitsToWords(n + 1)));
            this.reg[((n / 16)|0)] = (((((this.reg[((n / 16)|0)] | ((((1) & 65535) << ((n & 15)|0))|0)) & 65535))|0));
            this.wordCount = this.CalcWordCount();
        } else {
            if (((n / 16)|0) < this.reg.length) this.reg[((n / 16)|0)] = (((((this.reg[((n / 16)|0)] & ((((~((((1) & 65535)|0) << ((n % 16)|0))))|0))) & 65535))|0));
            this.wordCount = this.CalcWordCount();
        }
    };
    
    prototype.testBit = function(index) {
        if (index < 0) throw ("index");
        if (this.signum() < 0) {
            var tcindex = 0;
            var wordpos = ((index / 16)|0);
            if (wordpos >= this.reg.length) return true;
            while (tcindex < wordpos && this.reg[tcindex] == 0) {
                tcindex++;
            }
            var tc;
            {
                tc = this.reg[wordpos];
                if (tcindex == wordpos) tc--;
                tc = ((~tc)|0);
            }
            return (((tc >> ((index & 15)|0)) & 1) != 0);
        } else {
            return this.GetUnsignedBit(index);
        }
    };
    
    prototype.GetUnsignedBit = function(n) {
        if (((n / 16)|0) >= this.reg.length) return false; else return (((this.reg[((n / 16)|0)] >> ((n & 15)|0)) & 1) != 0);
    };
    prototype.InitializeInt = function(numberValue) {
        var iut;
        {
            this.negative = (numberValue < 0);
            if (numberValue == -2147483648) {
                this.reg = [0, 0];
                this.reg[0] = (((0) & 65535)|0);
                this.reg[1] = (((32768) & 65535)|0);
                this.wordCount = 2;
            } else {
                iut = ((numberValue < 0) ? ((-numberValue)|0) : (numberValue|0));
                this.reg = [0, 0];
                this.reg[0] = (iut & 65535);
                this.reg[1] = (((((iut >> 16)|0) & 65535)|0));
                this.wordCount = (this.reg[1] != 0 ? 2 : (this.reg[0] == 0 ? 0 : 1));
            }
        }
        return this;
    };
    
    prototype.toByteArray = function(littleEndian) {
        var sign = this.signum();
        if (sign == 0) {
            return [0];
        } else if (sign > 0) {
            var byteCount = this.ByteCount();
            var bc = this.BitLength();
            var byteArrayLength = byteCount;
            if ((bc & 7) == 0 && this.GetUnsignedBit(bc - 1)) {
                byteArrayLength++;
            }
            var bytes = [];
            for (var arrfillI = 0; arrfillI < byteArrayLength; arrfillI++) bytes[arrfillI] = 0;
            var j = 0;
            for (var i = 0; i < byteCount; i += 2, j++) {
                var index = (littleEndian) ? i : bytes.length - 1 - i;
                var index2 = (littleEndian) ? i + 1 : bytes.length - 2 - i;
                bytes[index] = (((this.reg[j]) & 255)|0);
                if (index2 >= 0 && index2 < byteArrayLength) {
                    bytes[index2] = (((this.reg[j] >> 8) & 255)|0);
                }
            }
            return bytes;
        } else {
            var regdata = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) regdata[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) regdata[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.TwosComplement(regdata, 0, ((regdata.length)|0));
            var byteCount = regdata.length * 2;
            for (var i = regdata.length - 1; i >= 0; i--) {
                if (regdata[i] == (65535)) {
                    byteCount -= 2;
                } else if ((regdata[i] & 65408) == 65408) {
                    
                    byteCount -= 1;
                    break;
                } else if ((regdata[i] & 32768) == 32768) {
                    
                    break;
                } else {
                    
                    byteCount += 1;
                    break;
                }
            }
            if (byteCount == 0) byteCount = 1;
            var bytes = [];
            for (var arrfillI = 0; arrfillI < byteCount; arrfillI++) bytes[arrfillI] = 0;
            bytes[(littleEndian) ? bytes.length - 1 : 0] = 255;
            byteCount = (byteCount < regdata.length * 2 ? byteCount : regdata.length * 2);
            var j = 0;
            for (var i = 0; i < byteCount; i += 2, j++) {
                var index = (littleEndian) ? i : bytes.length - 1 - i;
                var index2 = (littleEndian) ? i + 1 : bytes.length - 2 - i;
                bytes[index] = (((regdata[j]) & 255)|0);
                if (index2 >= 0 && index2 < byteCount) {
                    bytes[index2] = (((regdata[j] >> 8) & 255)|0);
                }
            }
            return bytes;
        }
    };
    
    prototype.shiftLeft = function(numberBits) {
        if (numberBits == 0) return this;
        if (numberBits < 0) {
            if (numberBits == -2147483648) return this.shiftRight(1).shiftRight(2147483647);
            return this.shiftRight(-numberBits);
        }
        var ret = new BigInteger();
        var numWords = ((this.wordCount)|0);
        var shiftWords = ((numberBits >> 4)|0);
        var shiftBits = ((numberBits & 15)|0);
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
    
    prototype.shiftRight = function(numberBits) {
        if (numberBits == 0) return this;
        if (numberBits < 0) {
            if (numberBits == -2147483648) return this.shiftLeft(1).shiftLeft(2147483647);
            return this.shiftLeft(-numberBits);
        }
        var ret = new BigInteger();
        var numWords = ((this.wordCount)|0);
        var shiftWords = ((numberBits >> 4)|0);
        var shiftBits = ((numberBits & 15)|0);
        ret.negative = this.negative;
        ret.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(numWords); arrfillI++) ret.reg[arrfillI] = 0;
        for (var arrfillI = 0; arrfillI < numWords; arrfillI++) ret.reg[0 + arrfillI] = this.reg[0 + arrfillI];
        if (this.signum() < 0) {
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
            BigInteger.ShiftWordsRightByWordsSignExtend(ret.reg, 0, numWords, shiftWords);
            if (numWords > shiftWords) BigInteger.ShiftWordsRightByBitsSignExtend(ret.reg, 0, numWords - shiftWords, shiftBits);
            BigInteger.TwosComplement(ret.reg, 0, ((ret.reg.length)|0));
        } else {
            BigInteger.ShiftWordsRightByWords(ret.reg, 0, numWords, shiftWords);
            if (numWords > shiftWords) BigInteger.ShiftWordsRightByBits(ret.reg, 0, numWords - shiftWords, shiftBits);
        }
        ret.wordCount = ret.CalcWordCount();
        return ret;
    };
    
    constructor.valueOf = function(longerValue_obj) {
        var longerValue = JSInteropFactory.createLong(longerValue_obj);
        if (longerValue.signum() == 0) return BigInteger.ZERO;
        if (longerValue.equalsInt(1)) return BigInteger.ONE;
        var ret = new BigInteger();
        {
            ret.negative = (longerValue.signum() < 0);
            ret.reg = [0, 0, 0, 0];
            if (longerValue.equals(JSInteropFactory.LONG_MIN_VALUE())) {
                ret.reg[0] = (((0) & 65535)|0);
                ret.reg[1] = (((0) & 65535)|0);
                ret.reg[2] = (((0) & 65535)|0);
                ret.reg[3] = (((32768) & 65535)|0);
                ret.wordCount = 4;
            } else {
                var ut = longerValue;
                if (ut.signum() < 0) ut = ut.negate();
                ret.reg[0] = (((((ut.andInt(65535).shortValue()) & 65535))|0));
                ut = ut.shiftRight(16);
                ret.reg[1] = (((((ut.andInt(65535).shortValue()) & 65535))|0));
                ut = ut.shiftRight(16);
                ret.reg[2] = (((((ut.andInt(65535).shortValue()) & 65535))|0));
                ut = ut.shiftRight(16);
                ret.reg[3] = (((((ut.andInt(65535).shortValue()) & 65535))|0));
                
                ret.wordCount = 4;
                while (ret.wordCount != 0 && ret.reg[ret.wordCount - 1] == 0) ret.wordCount--;
            }
        }
        return ret;
    };
    
    prototype.intValue = function() {
        var c = ((this.wordCount)|0);
        if (c == 0) return 0;
        if (c > 2) throw "exception";
        if (c == 2 && (this.reg[1] & 32768) != 0) {
            if ((((this.reg[1] & 32767)|0) | this.reg[0]) == 0 && this.negative) {
                return -2147483648;
            } else {
                throw "exception";
            }
        } else {
            var ivv = (((this.reg[0])|0) & 65535);
            if (c > 1) ivv |= (((this.reg[1])|0) & 65535) << 16;
            if (this.negative) ivv = -ivv;
            return ivv;
        }
    };
    prototype.HasTinyValue = function() {
        var c = ((this.wordCount)|0);
        if (c > 2) return false;
        if (c == 2 && (this.reg[1] & 32768) != 0) {
            return (this.negative && this.reg[1] == (32768) && this.reg[0] == 0);
        }
        return true;
    };
    prototype.HasSmallValue = function() {
        var c = ((this.wordCount)|0);
        if (c > 4) return false;
        if (c == 4 && (this.reg[3] & 32768) != 0) {
            return (this.negative && this.reg[3] == (32768) && this.reg[2] == 0 && this.reg[1] == 0 && this.reg[0] == 0);
        }
        return true;
    };
    
    prototype.longValue = function() {
        var count = this.wordCount;
        if (count == 0) return JSInteropFactory.createLong(0);
        if (count > 4) throw "exception";
        if (count == 4 && (this.reg[3] & 32768) != 0) {
            if (this.negative && this.reg[3] == (32768) && this.reg[2] == 0 && this.reg[1] == 0 && this.reg[0] == 0) {
                return JSInteropFactory.LONG_MIN_VALUE();
            } else {
                throw "exception";
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
            if (this.negative) vv = vv.negate();
            return vv;
        }
    };
    constructor.Power2 = function(e) {
        var r = new BigInteger().Allocate(BigInteger.BitsToWords((e + 1)|0));
        r.SetBitInternal((e|0), true);
        
        return r;
    };
    
    prototype.PowBigIntVar = function(power) {
        if ((power) == null) throw ("power");
        var sign = power.signum();
        if (sign < 0) throw ("power is negative");
        var thisVar = this;
        if (sign == 0) return BigInteger.ONE; else if (power.equals(BigInteger.ONE)) return this; else if (power.wordCount == 1 && power.reg[0] == 2) return thisVar.multiply(thisVar); else if (power.wordCount == 1 && power.reg[0] == 3) return (thisVar.multiply(thisVar)).multiply(thisVar);
        
        var r = BigInteger.ONE;
        while (power.signum() != 0) {
            if (power.testBit(0)) {
                r = (r.multiply(thisVar));
            }
            power = power.shiftRight(1);
            if (power.signum() != 0) {
                thisVar = (thisVar.multiply(thisVar));
            }
        }
        return r;
    };
    
    prototype.pow = function(powerSmall) {
        if (powerSmall < 0) throw ("power is negative");
        var thisVar = this;
        if (powerSmall == 0) return BigInteger.ONE; else if (powerSmall == 1) return this; else if (powerSmall == 2) return thisVar.multiply(thisVar); else if (powerSmall == 3) return (thisVar.multiply(thisVar)).multiply(thisVar);
        
        var r = BigInteger.ONE;
        while (powerSmall != 0) {
            if ((powerSmall & 1) != 0) {
                r = (r.multiply(thisVar));
            }
            powerSmall >>= 1;
            if (powerSmall != 0) {
                thisVar = (thisVar.multiply(thisVar));
            }
        }
        return r;
    };
    
    prototype.negate = function() {
        var bigintRet = new BigInteger();
        bigintRet.reg = this.reg;
        
        bigintRet.wordCount = this.wordCount;
        bigintRet.negative = (this.wordCount != 0) && (!this.negative);
        return bigintRet;
    };
    
    prototype.abs = function() {
        return this.signum() >= 0 ? this : this.negate();
    };
    
    prototype.CalcWordCount = function() {
        return ((BigInteger.CountWords(this.reg, this.reg.length))|0);
    };
    
    prototype.ByteCount = function() {
        var wc = this.wordCount;
        if (wc == 0) return 0;
        var s = this.reg[wc - 1];
        wc = (wc - 1) << 1;
        if (s == 0) return wc;
        return ((s >> 8) == 0) ? wc + 1 : wc + 2;
    };
    
    prototype.BitLength = function() {
        var wc = this.wordCount;
        if (wc != 0) return (((wc - 1) * 16 + BigInteger.BitPrecision(this.reg[wc - 1]))|0); else return 0;
    };
    constructor.vec = "0123456789ABCDEF";
    constructor.ReverseChars = function(chars, offset, length) {
        var half = length >> 1;
        var right = offset + length - 1;
        for (var i = 0; i < half; i++, right--) {
            var value = chars[offset + i];
            chars[offset + i] = chars[right];
            chars[right] = value;
        }
    };
    prototype.SmallValueToString = function() {
        var value = this.longValue();
        if (value.equals(JSInteropFactory.LONG_MIN_VALUE())) return "-9223372036854775808";
        var neg = (value.signum() < 0);
        var chars = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        var count = 0;
        if (neg) {
            chars[0] = '-';
            count++;
            value = value.negate();
        }
        while (value.signum() != 0) {
            var digit = BigInteger.vec.charAt(value.remainderWithUnsignedDivisor(10).intValue());
            chars[count++] = digit;
            value = value.divideWithUnsignedDivisor(10);
        }
        if (neg) BigInteger.ReverseChars(chars, 1, count - 1); else BigInteger.ReverseChars(chars, 0, count);
        var tmpbuilder = JSInteropFactory.createStringBuilder(16);
        for (var arrfillI = 0; arrfillI < count; arrfillI++) tmpbuilder.append(chars[arrfillI]);
        return tmpbuilder.toString();
    };
    
    prototype.toString = function() {
        if (this.signum() == 0) return "0";
        if (this.HasSmallValue()) {
            return this.SmallValueToString();
        }
        var tempReg = [];
        for (var arrfillI = 0; arrfillI < this.wordCount; arrfillI++) tempReg[arrfillI] = 0;
        for (var arrfillI = 0; arrfillI < tempReg.length; arrfillI++) tempReg[0 + arrfillI] = this.reg[0 + arrfillI];
        var wordCount = tempReg.length;
        while (wordCount != 0 && tempReg[wordCount - 1] == 0) wordCount--;
        var i = 0;
        var s = [];
        for (var arrfillI = 0; arrfillI < (((this.BitLength() / 3)|0) + 1); arrfillI++) s[arrfillI] = 0;
        while (wordCount != 0) {
            if (wordCount == 1 && tempReg[0] > 0 && tempReg[0] < 10000) {
                var rest = tempReg[0];
                while (rest != 0) {
                    s[i++] = BigInteger.vec.charAt(rest % 10);
                    rest = ((rest / 10)|0);
                }
                break;
            } else {
                var remainderSmall = BigInteger.FastDivideAndRemainder(tempReg, wordCount, 10000);
                
                while (wordCount != 0 && tempReg[wordCount - 1] == 0) wordCount--;
                for (var j = 0; j < 4; j++) {
                    s[i++] = BigInteger.vec.charAt((remainderSmall % 10)|0);
                    remainderSmall = ((remainderSmall / 10)|0);
                }
            }
        }
        BigInteger.ReverseChars(s, 0, i);
        if (this.signum() < 0) {
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
    
    constructor.fromString = function(str) {
        if (str == null) throw ("str");
        if ((str.length) <= 0) throw ("str.length" + " not less than " + "0" + " (" + (JSInteropFactory.createLong(str.length)) + ")");
        var offset = 0;
        var negative = false;
        if (str.charAt(0) == '-') {
            offset++;
            negative = true;
        }
        var bigint = new BigInteger().Allocate(4);
        var haveDigits = false;
        for (var i = offset; i < str.length; i++) {
            var c = str.charAt(i);
            if (c < '0' || c > '9') throw ("Illegal character found");
            haveDigits = true;
            var digit = ((c - '0')|0);
            
            var carry = BigInteger.LinearMultiply(bigint.reg, 0, bigint.reg, 0, 10, bigint.reg.length);
            if (carry != 0) bigint.reg = BigInteger.GrowForCarry(bigint.reg, carry);
            
            if (digit != 0 && BigInteger.Increment(bigint.reg, 0, bigint.reg.length, (digit|0)) != 0) bigint.reg = BigInteger.GrowForCarry(bigint.reg, 1);
        }
        if (!haveDigits) throw ("No digits");
        bigint.wordCount = bigint.CalcWordCount();
        bigint.negative = (bigint.wordCount != 0 && negative);
        return bigint;
    };
    
    prototype.gcd = function(bigintSecond) {
        if ((bigintSecond) == null) throw ("bigintSecond");
        if (this.signum() == 0) return (bigintSecond).abs();
        if (bigintSecond.signum() == 0) return (this).abs();
        var thisValue = this.abs();
        bigintSecond = bigintSecond.abs();
        if (bigintSecond.equals(BigInteger.ONE) || thisValue.equals(bigintSecond)) return bigintSecond;
        if (thisValue.equals(BigInteger.ONE)) return thisValue;
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
    };
    
    prototype.ModPow = function(pow, mod) {
        if ((pow) == null) throw ("pow");
        if (pow.signum() < 0) throw ("pow is negative");
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
    constructor.PositiveAdd = function(sum, bigintAddend, bigintAugend) {
        var carry;
        if (bigintAddend.reg.length == bigintAugend.reg.length) carry = BigInteger.Add(sum.reg, 0, bigintAddend.reg, 0, bigintAugend.reg, 0, ((bigintAddend.reg.length)|0)); else if (bigintAddend.reg.length > bigintAugend.reg.length) {
            carry = BigInteger.Add(sum.reg, 0, bigintAddend.reg, 0, bigintAugend.reg, 0, ((bigintAugend.reg.length)|0));
            for (var arrfillI = 0; arrfillI < bigintAddend.reg.length - bigintAugend.reg.length; arrfillI++) sum.reg[bigintAugend.reg.length + arrfillI] = bigintAddend.reg[bigintAugend.reg.length + arrfillI];
            carry = BigInteger.Increment(sum.reg, bigintAugend.reg.length, ((bigintAddend.reg.length - bigintAugend.reg.length)|0), (carry|0));
        } else {
            carry = BigInteger.Add(sum.reg, 0, bigintAddend.reg, 0, bigintAugend.reg, 0, ((bigintAddend.reg.length)|0));
            for (var arrfillI = 0; arrfillI < bigintAugend.reg.length - bigintAddend.reg.length; arrfillI++) sum.reg[bigintAddend.reg.length + arrfillI] = bigintAugend.reg[bigintAddend.reg.length + arrfillI];
            carry = BigInteger.Increment(sum.reg, bigintAddend.reg.length, ((bigintAugend.reg.length - bigintAddend.reg.length)|0), (carry|0));
        }
        if (carry != 0) {
            var len = BigInteger.RoundupSize(((sum.reg.length / 2)|0) + 1);
            sum.reg = BigInteger.CleanGrow(sum.reg, len);
            sum.reg[((sum.reg.length / 2)|0)] = (((1) & 65535)|0);
        }
        sum.negative = false;
        sum.wordCount = sum.CalcWordCount();
        sum.ShortenArray();
    };
    constructor.PositiveSubtract = function(diff, minuend, subtrahend) {
        var aSize = minuend.wordCount;
        aSize += aSize % 2;
        var bSize = subtrahend.wordCount;
        bSize += bSize % 2;
        if (aSize == bSize) {
            if (BigInteger.Compare(minuend.reg, 0, subtrahend.reg, 0, (aSize|0)) >= 0) {
                
                BigInteger.Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (aSize|0));
                diff.negative = false;
            } else {
                
                
                BigInteger.Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (aSize|0));
                diff.negative = true;
            }
        } else if (aSize > bSize) {
            
            
            var borrow = ((BigInteger.Subtract(diff.reg, 0, minuend.reg, 0, subtrahend.reg, 0, (bSize|0)))|0);
            for (var arrfillI = 0; arrfillI < aSize - bSize; arrfillI++) diff.reg[bSize + arrfillI] = minuend.reg[bSize + arrfillI];
            borrow = ((BigInteger.Decrement(diff.reg, bSize, ((aSize - bSize)|0), borrow))|0);
            
            diff.negative = false;
        } else {
            
            var borrow = ((BigInteger.Subtract(diff.reg, 0, subtrahend.reg, 0, minuend.reg, 0, (aSize|0)))|0);
            for (var arrfillI = 0; arrfillI < bSize - aSize; arrfillI++) diff.reg[aSize + arrfillI] = subtrahend.reg[aSize + arrfillI];
            borrow = ((BigInteger.Decrement(diff.reg, aSize, ((bSize - aSize)|0), borrow))|0);
            
            diff.negative = true;
        }
        diff.wordCount = diff.CalcWordCount();
        diff.ShortenArray();
        if (diff.wordCount == 0) diff.negative = false;
    };
    
    prototype.equals = function(obj) {
        var other = ((obj.constructor==BigInteger) ? obj : null);
        if (other == null) return false;
        return other.compareTo(this) == 0;
    };
    
    prototype.hashCode = function() {
        var hashCodeValue = 0;
        {
            hashCodeValue += 1000000007 * this.signum();
            if (this.reg != null) {
                for (var i = 0; i < this.wordCount; i++) {
                    hashCodeValue += 1000000013 * this.reg[i];
                }
            }
        }
        return hashCodeValue;
    };
    
    prototype.add = function(bigintAugend) {
        if ((bigintAugend) == null) throw ("bigintAugend");
        var sum = new BigInteger().Allocate((this.reg.length > bigintAugend.reg.length ? this.reg.length : bigintAugend.reg.length)|0);
        if (this.signum() >= 0) {
            if (bigintAugend.signum() >= 0) BigInteger.PositiveAdd(sum, this, bigintAugend); else BigInteger.PositiveSubtract(sum, this, bigintAugend);
        } else {
            
            
            if (bigintAugend.signum() >= 0) {
                BigInteger.PositiveSubtract(sum, bigintAugend, this);
            } else {
                
                BigInteger.PositiveAdd(sum, this, bigintAugend);
                
                sum.negative = sum.signum() != 0;
            }
        }
        return sum;
    };
    
    prototype.subtract = function(subtrahend) {
        if ((subtrahend) == null) throw ("subtrahend");
        var diff = new BigInteger().Allocate((this.reg.length > subtrahend.reg.length ? this.reg.length : subtrahend.reg.length)|0);
        if (this.signum() >= 0) {
            if (subtrahend.signum() >= 0) BigInteger.PositiveSubtract(diff, this, subtrahend); else BigInteger.PositiveAdd(diff, this, subtrahend);
        } else {
            if (subtrahend.signum() >= 0) {
                BigInteger.PositiveAdd(diff, this, subtrahend);
                diff.negative = diff.signum() != 0;
            } else {
                BigInteger.PositiveSubtract(diff, subtrahend, this);
            }
        }
        return diff;
    };
    prototype.ShortenArray = function() {
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
    constructor.PositiveMultiply = function(product, bigintA, bigintB) {
        if (bigintA.wordCount == 1) {
            var wc = bigintB.wordCount;
            product.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(wc + 1); arrfillI++) product.reg[arrfillI] = 0;
            product.reg[wc] = ((((BigInteger.LinearMultiply(product.reg, 0, bigintB.reg, 0, bigintA.reg[0], wc)) & 65535)|0));
            product.negative = false;
            product.wordCount = product.CalcWordCount();
            return;
        } else if (bigintB.wordCount == 1) {
            var wc = bigintA.wordCount;
            product.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(wc + 1); arrfillI++) product.reg[arrfillI] = 0;
            product.reg[wc] = ((((BigInteger.LinearMultiply(product.reg, 0, bigintA.reg, 0, bigintB.reg[0], wc)) & 65535)|0));
            product.negative = false;
            product.wordCount = product.CalcWordCount();
            return;
        } else if (bigintA.equals(bigintB)) {
            var aSize = BigInteger.RoundupSize(bigintA.wordCount);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(aSize + aSize); arrfillI++) product.reg[arrfillI] = 0;
            product.negative = false;
            var workspace = [];
            for (var arrfillI = 0; arrfillI < aSize + aSize; arrfillI++) workspace[arrfillI] = 0;
            BigInteger.Square(product.reg, 0, workspace, 0, bigintA.reg, 0, aSize);
        } else {
            var aSize = BigInteger.RoundupSize(bigintA.wordCount);
            var bSize = BigInteger.RoundupSize(bigintB.wordCount);
            product.reg = [];
            for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(aSize + bSize); arrfillI++) product.reg[arrfillI] = 0;
            product.negative = false;
            var workspace = [];
            for (var arrfillI = 0; arrfillI < aSize + bSize; arrfillI++) workspace[arrfillI] = 0;
            BigInteger.AsymmetricMultiply(product.reg, 0, workspace, 0, bigintA.reg, 0, aSize, bigintB.reg, 0, bSize);
        }
        product.wordCount = product.CalcWordCount();
        product.ShortenArray();
    };
    
    prototype.multiply = function(bigintMult) {
        if ((bigintMult) == null) throw ("bigintMult");
        var product = new BigInteger();
        if (this.wordCount == 0 || bigintMult.wordCount == 0) return BigInteger.ZERO;
        if (this.wordCount == 1 && this.reg[0] == 1) return this.negative ? bigintMult.negate() : bigintMult;
        if (bigintMult.wordCount == 1 && bigintMult.reg[0] == 1) return bigintMult.negative ? this.negate() : this;
        BigInteger.PositiveMultiply(product, this, bigintMult);
        if ((this.signum() >= 0) != (bigintMult.signum() >= 0)) product.NegateInternal();
        return product;
    };
    constructor.OperandLength = function(a) {
        for (var i = a.length - 1; i >= 0; i--) {
            if (a[i] != 0) return i + 1;
        }
        return 0;
    };
    constructor.DivideWithRemainderAnyLength = function(a, b, quotResult, modResult) {
        var lengthA = BigInteger.OperandLength(a);
        var lengthB = BigInteger.OperandLength(b);
        if (lengthB == 0) throw ("The divisor is zero.");
        
        if (lengthA == 0) {
            
            if (modResult != null) for (var arrfillI = 0; arrfillI < (0) + (modResult.length); arrfillI++) modResult[arrfillI] = 0;
            if (quotResult != null) for (var arrfillI = 0; arrfillI < (0) + (quotResult.length); arrfillI++) quotResult[arrfillI] = 0;
            
            return;
        }
        if (lengthA < lengthB) {
            
            if (modResult != null) {
                var tmpa = [];
                for (var arrfillI = 0; arrfillI < a.length; arrfillI++) tmpa[arrfillI] = 0;
                for (var arrfillI = 0; arrfillI < a.length; arrfillI++) tmpa[0 + arrfillI] = a[0 + arrfillI];
                for (var arrfillI = 0; arrfillI < (0) + (modResult.length); arrfillI++) modResult[arrfillI] = 0;
                for (var arrfillI = 0; arrfillI < (tmpa.length < modResult.length ? tmpa.length : modResult.length); arrfillI++) modResult[0 + arrfillI] = tmpa[0 + arrfillI];
            }
            if (quotResult != null) {
                
                for (var arrfillI = 0; arrfillI < (0) + (quotResult.length); arrfillI++) quotResult[arrfillI] = 0;
            }
            return;
        }
        if (lengthA == 1 && lengthB == 1) {
            var a0 = ((a[0])|0) & 65535;
            var b0 = ((b[0])|0) & 65535;
            var result = (((a0 / b0)|0)|0);
            var mod = (modResult != null) ? ((a0 % b0)|0) : 0;
            if (quotResult != null) {
                for (var arrfillI = 0; arrfillI < (0) + (quotResult.length); arrfillI++) quotResult[arrfillI] = 0;
                quotResult[0] = (((result) & 65535)|0);
            }
            if (modResult != null) {
                for (var arrfillI = 0; arrfillI < (0) + (modResult.length); arrfillI++) modResult[arrfillI] = 0;
                modResult[0] = (((mod) & 65535)|0);
            }
            return;
        }
        lengthA += lengthA % 2;
        if (lengthA > a.length) throw ("no room");
        lengthB += lengthB % 2;
        if (lengthB > b.length) throw ("no room");
        var tempbuf = [];
        for (var arrfillI = 0; arrfillI < lengthA + 3 * (lengthB + 2); arrfillI++) tempbuf[arrfillI] = 0;
        BigInteger.Divide(modResult, 0, quotResult, 0, tempbuf, 0, a, 0, lengthA, b, 0, lengthB);
        
        if (quotResult != null) {
            var quotEnd = lengthA - lengthB + 2;
            for (var arrfillI = quotEnd; arrfillI < (quotEnd) + (quotResult.length - quotEnd); arrfillI++) quotResult[arrfillI] = 0;
        }
    };
    constructor.BitsToWords = function(bitCount) {
        return ((((bitCount + 16 - 1) / (16))|0));
    };
    constructor.FastRemainder = function(dividendReg, count, divisorSmall) {
        var i = count;
        var remainder = 0;
        while ((i--) > 0) {
            remainder = BigInteger.RemainderUnsigned(BigInteger.MakeUint(dividendReg[i], remainder), divisorSmall);
        }
        return remainder;
    };
    constructor.FastDivide = function(quotientReg, count, divisorSmall) {
        var i = count;
        var remainder = 0;
        var idivisor = (divisorSmall & 65535);
        while ((i--) > 0) {
            var currentDividend = (((((((quotientReg[i])|0) & 65535) | ((remainder|0) << 16)))|0));
            if ((currentDividend >> 31) == 0) {
                quotientReg[i] = (((((((currentDividend / idivisor)|0)|0) & 65535))|0));
                if (i > 0) remainder = ((currentDividend % idivisor)|0);
            } else {
                quotientReg[i] = ((((BigInteger.DivideUnsigned(currentDividend, divisorSmall)) & 65535)|0));
                if (i > 0) remainder = BigInteger.RemainderUnsigned(currentDividend, divisorSmall);
            }
        }
    };
    constructor.FastDivideAndRemainder = function(quotientReg, count, divisorSmall) {
        var i = count;
        var remainder = 0;
        var idivisor = (divisorSmall & 65535);
        while ((i--) > 0) {
            var currentDividend = (((((((quotientReg[i])|0) & 65535) | ((remainder|0) << 16)))|0));
            if ((currentDividend >> 31) == 0) {
                quotientReg[i] = (((((((currentDividend / idivisor)|0)|0) & 65535))|0));
                remainder = ((currentDividend % idivisor)|0);
            } else {
                quotientReg[i] = ((((BigInteger.DivideUnsigned(currentDividend, divisorSmall)) & 65535)|0));
                remainder = BigInteger.RemainderUnsigned(currentDividend, divisorSmall);
            }
        }
        return remainder;
    };
    
    prototype.divide = function(bigintDivisor) {
        if ((bigintDivisor) == null) throw ("bigintDivisor");
        var aSize = this.wordCount;
        var bSize = bigintDivisor.wordCount;
        if (bSize == 0) throw "exception";
        if (aSize < bSize) {
            
            return BigInteger.ZERO;
        }
        if (aSize <= 4 && bSize <= 4 && this.HasTinyValue() && bigintDivisor.HasTinyValue()) {
            var aSmall = this.intValue();
            var bSmall = bigintDivisor.intValue();
            if (aSmall != -2147483648 || bSmall != -1) {
                var result = ((aSmall / bSmall)|0);
                return new BigInteger().InitializeInt(result);
            }
        }
        var quotient;
        if (bSize == 1) {
            
            quotient = new BigInteger();
            quotient.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) quotient.reg[arrfillI] = 0;
            quotient.wordCount = this.wordCount;
            quotient.negative = this.negative;
            for (var arrfillI = 0; arrfillI < quotient.reg.length; arrfillI++) quotient.reg[0 + arrfillI] = this.reg[0 + arrfillI];
            BigInteger.FastDivide(quotient.reg, aSize, bigintDivisor.reg[0]);
            quotient.wordCount = quotient.CalcWordCount();
            if (quotient.wordCount != 0) {
                quotient.negative = (this.signum() < 0) ^ (bigintDivisor.signum() < 0);
            } else {
                quotient.negative = false;
            }
            return quotient;
        }
        quotient = new BigInteger();
        aSize += aSize % 2;
        bSize += bSize % 2;
        quotient.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((aSize - bSize + 2)|0); arrfillI++) quotient.reg[arrfillI] = 0;
        quotient.negative = false;
        BigInteger.DivideWithRemainderAnyLength(this.reg, bigintDivisor.reg, quotient.reg, null);
        quotient.wordCount = quotient.CalcWordCount();
        quotient.ShortenArray();
        if ((this.signum() < 0) ^ (bigintDivisor.signum() < 0)) {
            quotient.NegateInternal();
        }
        return quotient;
    };
    
    prototype.divideAndRemainder = function(divisor) {
        if ((divisor) == null) throw ("divisor");
        var quotient;
        var aSize = this.wordCount;
        var bSize = divisor.wordCount;
        if (bSize == 0) throw "exception";
        if (aSize < bSize) {
            
            return [BigInteger.ZERO, this];
        }
        if (bSize == 1) {
            
            quotient = new BigInteger();
            quotient.reg = [];
            for (var arrfillI = 0; arrfillI < this.reg.length; arrfillI++) quotient.reg[arrfillI] = 0;
            quotient.wordCount = this.wordCount;
            quotient.negative = this.negative;
            for (var arrfillI = 0; arrfillI < quotient.reg.length; arrfillI++) quotient.reg[0 + arrfillI] = this.reg[0 + arrfillI];
            var smallRemainder = (((BigInteger.FastDivideAndRemainder(quotient.reg, aSize, divisor.reg[0]))|0) & 65535);
            quotient.wordCount = quotient.CalcWordCount();
            quotient.ShortenArray();
            if (quotient.wordCount != 0) {
                quotient.negative = (this.signum() < 0) ^ (divisor.signum() < 0);
            } else {
                quotient.negative = false;
            }
            if (this.signum() < 0) smallRemainder = -smallRemainder;
            return [quotient, new BigInteger().InitializeInt(smallRemainder)];
        }
        var remainder = new BigInteger();
        quotient = new BigInteger();
        aSize += aSize % 2;
        bSize += bSize % 2;
        remainder.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(bSize|0); arrfillI++) remainder.reg[arrfillI] = 0;
        remainder.negative = false;
        quotient.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((aSize - bSize + 2)|0); arrfillI++) quotient.reg[arrfillI] = 0;
        quotient.negative = false;
        BigInteger.DivideWithRemainderAnyLength(this.reg, divisor.reg, quotient.reg, remainder.reg);
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
        if (divisor.signum() < 0) quotient.NegateInternal();
        return [quotient, remainder];
    };
    
    prototype.mod = function(divisor) {
        if ((divisor) == null) throw ("divisor");
        if (divisor.signum() < 0) {
            throw ("Divisor is negative");
        }
        var rem = this.remainder(divisor);
        if (rem.signum() < 0) rem = divisor.subtract(rem);
        return rem;
    };
    
    prototype.remainder = function(divisor) {
        if (this.PositiveCompare(divisor) < 0) {
            if (divisor.signum() == 0) throw "exception";
            return this;
        }
        var remainder = new BigInteger();
        var aSize = this.wordCount;
        var bSize = divisor.wordCount;
        if (bSize == 0) throw "exception";
        if (aSize < bSize) {
            
            return this;
        }
        if (bSize == 1) {
            var shortRemainder = BigInteger.FastRemainder(this.reg, this.wordCount, divisor.reg[0]);
            var smallRemainder = (shortRemainder & 65535);
            if (this.signum() < 0) smallRemainder = -smallRemainder;
            return new BigInteger().InitializeInt(smallRemainder);
        }
        aSize += aSize % 2;
        bSize += bSize % 2;
        remainder.reg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize(bSize|0); arrfillI++) remainder.reg[arrfillI] = 0;
        remainder.negative = false;
        var quotientReg = [];
        for (var arrfillI = 0; arrfillI < BigInteger.RoundupSize((aSize - bSize + 2)|0); arrfillI++) quotientReg[arrfillI] = 0;
        BigInteger.DivideWithRemainderAnyLength(this.reg, divisor.reg, quotientReg, remainder.reg);
        remainder.wordCount = remainder.CalcWordCount();
        remainder.ShortenArray();
        if (this.signum() < 0 && remainder.signum() != 0) {
            remainder.NegateInternal();
        }
        return remainder;
    };
    prototype.NegateInternal = function() {
        if (this.wordCount != 0) this.negative = (this.signum() > 0);
    };
    prototype.PositiveCompare = function(t) {
        var size = this.wordCount, tSize = t.wordCount;
        if (size == tSize) return BigInteger.Compare(this.reg, 0, t.reg, 0, (size|0)); else return size > tSize ? 1 : -1;
    };
    
    prototype.compareTo = function(other) {
        if (other == null) return 1;
        var size = this.wordCount, tSize = other.wordCount;
        var sa = (size == 0 ? 0 : (this.negative ? -1 : 1));
        var sb = (tSize == 0 ? 0 : (other.negative ? -1 : 1));
        if (sa != sb) return (sa < sb) ? -1 : 1;
        if (sa == 0) return 0;
        var cmp = 0;
        if (size == tSize) cmp = BigInteger.Compare(this.reg, 0, other.reg, 0, (size|0)); else cmp = size > tSize ? 1 : -1;
        return (sa > 0) ? cmp : -cmp;
    };
    
    prototype.signum = function() {
        if (this.wordCount == 0) return 0;
        return (this.negative) ? -1 : 1;
    };
    
    prototype.isZero = function() {
        return (this.wordCount == 0);
    };
    
    prototype.Sqrt = function(bi) {
        if (this.signum() < 0) return BigInteger.ZERO;
        var bigintX = null;
        var bigintY = BigInteger.Power2((((this.BitLength() + 1) / 2)|0));
        do {
            bigintX = bigintY;
            bigintY = bi.divide(bigintX);
            bigintY = bigintY.add(bigintX);
            bigintY = bigintY.shiftRight(1);
        } while (bigintY.compareTo(bigintX) < 0);
        return bigintX;
    };
    
    prototype.isEven = function() {
        return !this.GetUnsignedBit(0);
    };
    constructor.ZERO = new BigInteger().InitializeInt(0);
    constructor.ONE = new BigInteger().InitializeInt(1);
    constructor.TEN = new BigInteger().InitializeInt(10);
})(BigInteger,BigInteger.prototype);


if(typeof exports!=="undefined")exports.BigInteger=BigInteger;

var FastInteger = 

function(value) {

    this.smallValue = value;
};
(function(constructor,prototype){
    constructor.MutableNumber = function FastInteger$MutableNumber(val) {

        if (val < 0) throw ("Only positive integers are supported");
        this.data = [0, 0, 0, 0];
        this.wordCount = (val == 0) ? 0 : 1;
        this.data[0] = (((val) & 0xFFFFFFFF)|0);
    };
    (function(constructor,prototype){
        prototype.data = null;
        prototype.wordCount = null;
        constructor.FromBigInteger = function(bigintVal) {
            var mnum = new FastInteger.MutableNumber(0);
            if (bigintVal.signum() < 0) throw ("Only positive integers are supported");
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
                        x |= (((bytes[i + 1])|0) & 255) << 8;
                    }
                    if (i + 2 < len) {
                        x |= (((bytes[i + 2])|0) & 255) << 16;
                    }
                    if (i + 3 < len) {
                        x |= (((bytes[i + 3])|0) & 255) << 24;
                    }
                    mnum.data[i >> 2] = x;
                }
            }
            while (mnum.wordCount != 0 && mnum.data[mnum.wordCount - 1] == 0) mnum.wordCount--;
            return mnum;
        };
        prototype.ToBigInteger = function() {
            if (this.wordCount == 1 && (this.data[0] >> 31) == 0) {
                return BigInteger.valueOf((this.data[0])|0);
            }
            var bytes = [];
            for (var arrfillI = 0; arrfillI < this.wordCount * 4 + 1; arrfillI++) bytes[arrfillI] = 0;
            for (var i = 0; i < this.wordCount; i++) {
                bytes[i * 4 + 0] = (((this.data[i]) & 255)|0);
                bytes[i * 4 + 1] = (((this.data[i] >> 8) & 255)|0);
                bytes[i * 4 + 2] = (((this.data[i] >> 16) & 255)|0);
                bytes[i * 4 + 3] = (((this.data[i] >> 24) & 255)|0);
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
            if (multiplicand < 0) throw ("Only positive multiplicands are supported"); else if (multiplicand != 0) {
                var carry = 0;
                if (this.wordCount == 0) {
                    if (this.data.length == 0) this.data = [0, 0, 0, 0];
                    this.data[0] = 0;
                    this.wordCount = 1;
                }
                var result0, result1, result2, result3;
                if (multiplicand < 65536) {
                    for (var i = 0; i < this.wordCount; i++) {
                        var x0 = this.data[i];
                        var x1 = x0;
                        var y0 = multiplicand;
                        x0 &= (65535);
                        x1 = (x1 >>> 16);
                        var temp = (x0 * y0);
                        result1 = (temp >>> 16);
                        result0 = temp & 65535;
                        result2 = 0;
                        temp = (x1 * y0);
                        result2 += (temp >>> 16);
                        result1 += temp & 65535;
                        result2 += (result1 >>> 16);
                        result1 = result1 & 65535;
                        result3 = (result2 >>> 16);
                        result2 = result2 & 65535;
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
                    for (var i = 0; i < this.wordCount; i++) {
                        var x0 = this.data[i];
                        var x1 = x0;
                        var y0 = multiplicand;
                        var y1 = y0;
                        x0 &= (65535);
                        y0 &= (65535);
                        x1 = (x1 >>> 16);
                        y1 = (y1 >>> 16);
                        var temp = (x0 * y0);
                        result1 = (temp >>> 16);
                        result0 = temp & 65535;
                        temp = (x0 * y1);
                        result2 = (temp >>> 16);
                        result1 += temp & 65535;
                        result2 += (result1 >>> 16);
                        result1 = result1 & 65535;
                        temp = (x1 * y0);
                        result2 += (temp >>> 16);
                        result1 += temp & 65535;
                        result2 += (result1 >>> 16);
                        result1 = result1 & 65535;
                        result3 = (result2 >>> 16);
                        result2 = result2 & 65535;
                        temp = (x1 * y1);
                        result3 += (temp >>> 16);
                        result2 += temp & 65535;
                        result3 += (result2 >>> 16);
                        result2 = result2 & 65535;
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
                    this.wordCount++;
                }
                while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) this.wordCount--;
            } else {
                if (this.data.length > 0) this.data[0] = 0;
                this.wordCount = 0;
            }
            return this;
        };
        prototype.signum = function() {
            return (this.wordCount == 0 ? 0 : 1);
        };
        prototype.isEvenNumber = function() {
            return (this.wordCount == 0 || (this.data[0] & 1) == 0);
        };
        prototype.CompareToInt = function(val) {
            if (val < 0 || this.wordCount > 1) return 1;
            if (this.wordCount == 0) {
                return (val == 0) ? 0 : -1;
            } else if (this.data[0] == val) {
                return 0;
            } else {
                return (((this.data[0] >> 31) == (val >> 31)) ? ((this.data[0] & 2147483647) < (val & 2147483647)) : ((this.data[0] >> 31) == 0)) ? -1 : 1;
            }
        };
        prototype.SubtractInt = function(other) {
            if (other < 0) throw ("Only positive values are supported"); else if (other != 0) {
                {
                    if (this.wordCount == 0) {
                        if (this.data.length == 0) this.data = [0, 0, 0, 0];
                        this.data[0] = 0;
                        this.wordCount = 1;
                    }
                    var borrow;
                    var u;
                    var a = this.data[0];
                    u = (a - other);
                    borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other != 0)) ? 1 : 0;
                    this.data[0] = (u|0);
                    if (borrow != 0) {
                        for (var i = 1; i < this.wordCount; i++) {
                            u = (this.data[i]) - borrow;
                            borrow = ((((this.data[i] >> 31) == (u >> 31)) ? ((this.data[i] & 2147483647) < (u & 2147483647)) : ((this.data[i] >> 31) == 0))) ? 1 : 0;
                            this.data[i] = (u|0);
                        }
                    }
                    while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) this.wordCount--;
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
                    for (var i = 0; i < neededSize; i++) {
                        var a = this.data[i];
                        u = (a - other.data[i]) - borrow;
                        borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
                        this.data[i] = (u|0);
                    }
                    if (borrow != 0) {
                        for (var i = neededSize; i < this.wordCount; i++) {
                            var a = this.data[i];
                            u = (a - other.data[i]) - borrow;
                            borrow = ((((a >> 31) == (u >> 31)) ? ((a & 2147483647) < (u & 2147483647)) : ((a >> 31) == 0)) || (a == u && other.data[i] != 0)) ? 1 : 0;
                            this.data[i] = (u|0);
                        }
                    }
                    while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) this.wordCount--;
                    return this;
                }
            }
        };
        prototype.compareTo = function(other) {
            if (this.wordCount != other.wordCount) {
                return (this.wordCount < other.wordCount) ? -1 : 1;
            }
            var N = this.wordCount;
            while ((N--) != 0) {
                var an = this.data[N];
                var bn = other.data[N];
                if (((an >> 31) == (bn >> 31)) ? ((an & 2147483647) < (bn & 2147483647)) : ((an >> 31) == 0)) {
                    return -1;
                } else if (an != bn) {
                    return 1;
                }
            }
            return 0;
        };
        prototype.Add = function(augend) {
            if (augend < 0) throw ("Only positive augends are supported"); else if (augend != 0) {
                var carry = 0;
                if (this.wordCount == 0) {
                    if (this.data.length == 0) this.data = [0, 0, 0, 0];
                    this.data[0] = 0;
                    this.wordCount = 1;
                }
                for (var i = 0; i < this.wordCount; i++) {
                    var u;
                    var a = this.data[i];
                    u = (a + augend) + carry;
                    carry = ((((u >> 31) == (a >> 31)) ? ((u & 2147483647) < (a & 2147483647)) : ((u >> 31) == 0)) || (u == a && augend != 0)) ? 1 : 0;
                    this.data[i] = u;
                    if (carry == 0) return this;
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
                    this.wordCount++;
                }
            }
            while (this.wordCount != 0 && this.data[this.wordCount - 1] == 0) this.wordCount--;
            return this;
        };
    })(FastInteger.MutableNumber,FastInteger.MutableNumber.prototype);

    prototype.smallValue = null;
    prototype.mnum = null;
    prototype.largeValue = null;
    prototype.integerMode = 0;
    constructor.Int32MinValue = BigInteger.valueOf(-2147483648);
    constructor.Int32MaxValue = BigInteger.valueOf(2147483647);
    constructor.NegativeInt32MinValue = (FastInteger.Int32MinValue).negate();
    constructor.Copy = function(value) {
        var fi = new FastInteger(value.smallValue);
        fi.integerMode = value.integerMode;
        fi.largeValue = value.largeValue;
        fi.mnum = (value.mnum == null || value.integerMode != 1) ? null : value.mnum.Copy();
        return fi;
    };
    constructor.FromBig = function(bigintVal) {
        var sign = bigintVal.signum();
        if (sign == 0 || (sign < 0 && bigintVal.compareTo(FastInteger.Int32MinValue) >= 0) || (sign > 0 && bigintVal.compareTo(FastInteger.Int32MaxValue) <= 0)) {
            return new FastInteger(bigintVal.intValue());
        } else if (sign > 0) {
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
                throw "exception";
        }
    };
    
    prototype.compareTo = function(val) {
        switch((this.integerMode << 2) | val.integerMode) {
            case ((0 << 2) | 0):
                {
                    var vsv = val.smallValue;
                    return (this.smallValue == vsv) ? 0 : (this.smallValue < vsv ? -1 : 1);
                }
            case ((0 << 2) | 1):
                return -(val.mnum.CompareToInt(this.smallValue));
            case ((0 << 2) | 2):
                return this.AsBigInteger().compareTo(val.largeValue);
            case ((1 << 2) | 0):
                return this.mnum.CompareToInt(val.smallValue);
            case ((1 << 2) | 1):
                return this.mnum.compareTo(val.mnum);
            case ((1 << 2) | 2):
                return this.AsBigInteger().compareTo(val.largeValue);
            case ((2 << 2) | 0):
            case ((2 << 2) | 1):
            case ((2 << 2) | 2):
                return this.largeValue.compareTo(val.AsBigInteger());
            default:
                throw "exception";
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
        for (var arrfillI = 0; arrfillI < wordCount * 4 + 1; arrfillI++) bytes[arrfillI] = 0;
        for (var i = 0; i < wordCount; i++) {
            bytes[i * 4 + 0] = (((words[i]) & 255)|0);
            bytes[i * 4 + 1] = (((words[i] >> 8) & 255)|0);
            bytes[i * 4 + 2] = (((words[i] >> 16) & 255)|0);
            bytes[i * 4 + 3] = (((words[i] >> 24) & 255)|0);
        }
        bytes[bytes.length - 1] = 0;
        return BigInteger.fromByteArray(bytes, true);
    };
    constructor.GetLastWords = function(bigint, numWords32Bit) {
        return FastInteger.MutableNumber.FromBigInteger(bigint).GetLastWordsInternal(numWords32Bit);
    };
    
    prototype.Multiply = function(val) {
        if (val == 0) {
            this.smallValue = 0;
            this.integerMode = 0;
        } else {
            switch(this.integerMode) {
                case 0:
                    var apos = (this.smallValue > 0);
                    var bpos = (val > 0);
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
                    throw "exception";
            }
        }
        return this;
    };
    
    prototype.Negate = function() {
        switch(this.integerMode) {
            case 0:
                if (this.smallValue == -2147483648) {
                    
                    this.integerMode = 1;
                    this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.NegativeInt32MinValue);
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
                throw "exception";
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
                throw "exception";
        }
        return this;
    };
    
    prototype.SubtractInt = function(val) {
        if (val == -2147483648) {
            return this.AddBig(FastInteger.NegativeInt32MinValue);
        } else {
            return this.AddInt(-val);
        }
    };
    
    prototype.AddBig = function(bigintVal) {
        switch(this.integerMode) {
            case 0:
                {
                    var sign = bigintVal.signum();
                    if (sign == 0 || (sign < 0 && bigintVal.compareTo(FastInteger.Int32MinValue) >= 0) || (sign > 0 && bigintVal.compareTo(FastInteger.Int32MaxValue) <= 0)) {
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
                throw "exception";
        }
        return this;
    };
    
    prototype.SubtractBig = function(bigintVal) {
        if (this.integerMode == 2) {
            this.largeValue = this.largeValue.subtract(bigintVal);
            return this;
        } else {
            var sign = bigintVal.signum();
            if (sign == 0) return this;
            
            if (sign < 0 && bigintVal.compareTo(FastInteger.Int32MinValue) > 0) {
                return this.AddInt(-(bigintVal.intValue()));
            }
            if (sign > 0 && bigintVal.compareTo(FastInteger.Int32MaxValue) <= 0) {
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
                throw "exception";
        }
        return this;
    };
    
    prototype.Mod = function(divisor) {
        
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
                    throw "exception";
            }
        } else {
            throw "exception";
        }
        return this;
    };
    
    prototype.Divide = function(divisor) {
        if (divisor != 0) {
            switch(this.integerMode) {
                case 0:
                    if (divisor == -1 && this.smallValue == -2147483648) {
                        
                        this.integerMode = 1;
                        this.mnum = FastInteger.MutableNumber.FromBigInteger(FastInteger.NegativeInt32MinValue);
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
                    throw "exception";
            }
        } else {
            throw "exception";
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
                throw "exception";
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
                    this.smallValue += val;
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
                throw "exception";
        }
        return this;
    };
    
    prototype.CanFitInInt32 = function() {
        var sign;
        switch(this.integerMode) {
            case 0:
                return true;
            case 1:
                return this.mnum.CanFitInInt32();
            case 2:
                {
                    sign = this.largeValue.signum();
                    if (sign == 0) return true;
                    if (sign < 0) return this.largeValue.compareTo(FastInteger.Int32MinValue) >= 0;
                    return this.largeValue.compareTo(FastInteger.Int32MaxValue) <= 0;
                }
            default:
                throw "exception";
        }
    };
    
    prototype.toString = function() {
        switch(this.integerMode) {
            case 0:
                return (((this.smallValue)|0)+"");
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
    
    prototype.MinInt32 = function(val) {
        return this.CompareToInt(val) < 0 ? this.AsInt32() : val;
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
                throw "exception";
        }
    };
})(FastInteger,FastInteger.prototype);


if(typeof exports!=="undefined")exports.FastInteger=FastInteger;

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
        this.knownBitLength = BitShiftAccumulator.ByteArrayBitLength(bytes);
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
    constructor.ByteArrayBitLength = function(bytes) {
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
            
            return BitShiftAccumulator.ByteArrayBitLength(bytes);
        }
    };
    
    prototype.ShiftBigToBits = function(bits) {
        var bytes = this.shiftedBigInt.toByteArray(true);
        this.knownBitLength = BitShiftAccumulator.ByteArrayBitLength(bytes);
        
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
            this.bitLeftmost = ((str.charCodeAt(i)-48)|0);
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
            var newLength = ((digitLength - digitShift)|0);
            if (digitShift <= 2147483647) this.discardedBitCount.AddInt(digitShift|0); else this.discardedBitCount.AddBig(BigInteger.valueOf(digitShift));
            for (var i = str.length - 1; i >= 0; i--) {
                this.bitsAfterLeftmost |= this.bitLeftmost;
                this.bitLeftmost = ((str.charCodeAt(i)-48)|0);
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

var Rounding={};Rounding.Down="Down";Rounding.Up="Up";Rounding.HalfDown="HalfDown";Rounding.HalfUp="HalfUp";Rounding.HalfEven="HalfEven";Rounding.Ceiling="Ceiling";Rounding.Floor="Floor";Rounding.ZeroFiveUp="ZeroFiveUp";Rounding.Unnecessary="Unnecessary";

if(typeof exports!=="undefined")exports.Rounding=Rounding;

var PrecisionContext = 

function(precision, rounding, exponentMinSmall, exponentMaxSmall, clampNormalExponents) {

    if ((precision) < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + (precision) + ")");
    if ((exponentMinSmall) > exponentMaxSmall) throw ("exponentMinSmall" + " not less or equal to " + (exponentMaxSmall) + " (" + (exponentMinSmall) + ")");
    this.bigintPrecision = BigInteger.valueOf(precision);
    this.rounding = rounding;
    this.clampNormalExponents = clampNormalExponents;
    this.hasExponentRange = true;
    this.exponentMax = BigInteger.valueOf(exponentMaxSmall);
    this.exponentMin = BigInteger.valueOf(exponentMinSmall);
};
(function(constructor,prototype){
    prototype.exponentMax = null;
    prototype.getEMax = function() {
        return this.hasExponentRange ? this.exponentMax : BigInteger.ZERO;
    };
    prototype.exponentMin = null;
    prototype.hasExponentRange = null;
    prototype.getHasExponentRange = function() {
        return this.hasExponentRange;
    };
    prototype.getEMin = function() {
        return this.hasExponentRange ? this.exponentMin : BigInteger.ZERO;
    };
    prototype.bigintPrecision = null;
    prototype.getPrecision = function() {
        return this.bigintPrecision;
    };
    prototype.rounding = null;
    prototype.clampNormalExponents = null;
    prototype.getClampNormalExponents = function() {
        return this.hasExponentRange ? this.clampNormalExponents : false;
    };
    prototype.getRounding = function() {
        return this.rounding;
    };
    prototype.flags = null;
    prototype.hasFlags = null;
    prototype.getHasFlags = function() {
        return this.hasFlags;
    };
    constructor.FlagInexact = 1;
    constructor.FlagRounded = 2;
    constructor.FlagSubnormal = 4;
    constructor.FlagUnderflow = 8;
    constructor.FlagOverflow = 16;
    constructor.FlagClamped = 32;
    prototype.getFlags = function() {
        return this.flags;
    };
    prototype.setFlags = function(value) {
        if (!this.getHasFlags()) throw ("Can't set flags");
        this.flags = value;
    };
    prototype.ExponentWithinRange = function(exponent) {
        if ((exponent) == null) throw ("exponent");
        if (!this.getHasExponentRange()) return true;
        if (this.bigintPrecision.signum() == 0) {
            return exponent.compareTo(this.getEMax()) <= 0;
        } else {
            var bigint = exponent;
            bigint = bigint.add(this.bigintPrecision);
            bigint = bigint.subtract(BigInteger.ONE);
            if (bigint.compareTo(this.getEMin()) < 0) return false;
            if (exponent.compareTo(this.getEMax()) > 0) return false;
            return true;
        }
    };
    prototype.WithRounding = function(rounding) {
        var pc = this.Copy();
        pc.rounding = rounding;
        return pc;
    };
    prototype.WithBlankFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = true;
        pc.flags = 0;
        return pc;
    };
    prototype.WithExponentClamp = function(clamp) {
        var pc = this.Copy();
        pc.clampNormalExponents = clamp;
        return pc;
    };
    prototype.WithExponentRange = function(exponentMin, exponentMax) {
        if ((exponentMin) == null) throw ("exponentMin");
        if (exponentMin.compareTo(exponentMax) > 0) throw ("exponentMin greater than exponentMax");
        var pc = this.Copy();
        pc.hasExponentRange = true;
        pc.exponentMin = exponentMin;
        pc.exponentMax = exponentMax;
        return pc;
    };
    prototype.WithNoFlags = function() {
        var pc = this.Copy();
        pc.hasFlags = false;
        pc.flags = 0;
        return pc;
    };
    prototype.WithUnlimitedExponents = function() {
        var pc = this.Copy();
        pc.hasExponentRange = false;
        return pc;
    };
    prototype.WithPrecision = function(precision) {
        if (precision < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + (precision) + ")");
        var pc = this.Copy();
        pc.bigintPrecision = BigInteger.valueOf(precision);
        return pc;
    };
    prototype.WithBigPrecision = function(bigintPrecision) {
        if ((bigintPrecision) == null) throw ("bigintPrecision");
        if (bigintPrecision.signum() < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + bigintPrecision + ")");
        var pc = this.Copy();
        pc.bigintPrecision = bigintPrecision;
        return pc;
    };
    prototype.Copy = function() {
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
    constructor.ForPrecision = function(precision) {
        return new PrecisionContext(precision, Rounding.HalfUp, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.ForRounding = function(rounding) {
        return new PrecisionContext(0, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.ForPrecisionAndRounding = function(precision, rounding) {
        return new PrecisionContext(precision, rounding, 0, 0, false).WithUnlimitedExponents();
    };
    constructor.Unlimited = PrecisionContext.ForPrecision(0);
    constructor.Decimal32 = new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);
    constructor.Decimal64 = new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);
    constructor.Decimal128 = new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);
    constructor.CliDecimal = new PrecisionContext(96, Rounding.HalfEven, 0, 28, true);
})(PrecisionContext,PrecisionContext.prototype);


if(typeof exports!=="undefined")exports.PrecisionContext=PrecisionContext;

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
            ret = this.RoundToPrecision(ret, ctx);
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
            ret = this.RoundToPrecision(ret, ctx);
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
            } else {
                
                minexp.SubtractInt(2);
            }
            var bigdir = BigInteger.ONE;
            if (cmp > 0) {
                bigdir = (bigdir).negate();
            }
            var quantum = this.helper.CreateNew(bigdir, minexp.AsBigInteger());
            var val = thisValue;
            ctx2 = ctx.WithRounding((cmp > 0) ? Rounding.Floor : Rounding.Ceiling).WithBlankFlags();
            val = this.Add(val, quantum, ctx2);
            if ((ctx2.getFlags() & (PrecisionContext.FlagOverflow | PrecisionContext.FlagUnderflow)) == 0) {
                
                ctx2.setFlags(0);
            }
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
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
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
            var naturalExponent = FastInteger.Copy(expdiff);
            var fastDesiredExponent = FastInteger.FromBig(desiredExponent);
            var negA = (signA < 0);
            var negB = (signB < 0);
            if (negA) mantissaDividend = mantissaDividend.negate();
            if (negB) mantissaDivisor = mantissaDivisor.negate();
            var fastPrecision = (ctx == null) ? new FastInteger(0) : FastInteger.FromBig(ctx.getPrecision());
            if (integerMode == RadixMath.IntegerModeFixedScale) {
                var shift;
                var rem;
                if (ctx != null && ctx.getHasFlags() && FastInteger.FromBig(desiredExponent).compareTo(naturalExponent) > 0) {
                    
                    ctx.setFlags(ctx.getFlags() | PrecisionContext.FlagRounded);
                }
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
            if (ctx != null && ctx.getHasFlags() && exp.compareTo(naturalExponent) > 0) {
                
                ctx.setFlags(ctx.getFlags() | PrecisionContext.FlagRounded);
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
        return this.RoundToPrecisionWithShift(thisValue, context, lastDiscarded, olderDiscarded, new FastInteger(0));
    };
    prototype.RoundToPrecisionWithShift = function(thisValue, context, lastDiscarded, olderDiscarded, shift) {
        if ((context) == null) return thisValue;
        if ((context.getPrecision()).signum() == 0 && !context.getHasExponentRange() && (lastDiscarded | olderDiscarded) == 0 && shift.signum() == 0) return thisValue;
        var fastEMin = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMin()) : null;
        var fastEMax = (context.getHasExponentRange()) ? FastInteger.FromBig(context.getEMax()) : null;
        var fastPrecision = FastInteger.FromBig(context.getPrecision());
        if (fastPrecision.signum() > 0 && fastPrecision.CompareToInt(18) <= 0 && (lastDiscarded | olderDiscarded) == 0 && shift.signum() == 0) {
            
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
        var dfrac = this.RoundToPrecisionInternal(thisValue, fastPrecision, context.getRounding(), fastEMin, fastEMax, lastDiscarded, olderDiscarded, shift, context.getHasFlags() ? signals : null);
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
            
            var shift = FastInteger.FromBig(expOther).SubtractBig(expThis);
            if (signThis < 0) mantThis = mantThis.negate();
            ret = this.helper.CreateNew(mantThis, expOther);
            ret = this.RoundToPrecisionWithShift(ret, tmpctx, 0, 0, shift);
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
                var ctxtmp = ctx.WithBlankFlags();
                ret = this.RoundToPrecision(ret, ctxtmp);
                if (ctx.getHasFlags()) {
                    ctx.setFlags(ctx.getFlags() | (ctx.getFlags() & ~PrecisionContext.FlagClamped));
                }
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
            return this.RoundToPrecisionInternal(thisValue, precision, rounding, fastEMin, fastEMax, lastDiscarded, olderDiscarded, null, signals);
        }
        var neg = this.helper.GetMantissa(thisValue).signum() < 0;
        var bigmantissa = this.helper.GetMantissa(thisValue);
        if (neg) bigmantissa = bigmantissa.negate();
        
        var oldmantissa = bigmantissa;
        var mantissaWasZero = (oldmantissa.signum() == 0 && (lastDiscarded | olderDiscarded) == 0);
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
            if (mantissaWasZero) {
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
            if (!mantissaWasZero) flags |= PrecisionContext.FlagSubnormal;
            if (exp.compareTo(fastETiny) < 0) {
                var expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
                expdiff.Add(discardedBits);
                accum = this.helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
                accum.ShiftRight(expdiff);
                var newmantissa = accum.getShiftedInt();
                if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (!mantissaWasZero) flags |= PrecisionContext.FlagRounded;
                    if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                        flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
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
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
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
    prototype.RoundToPrecisionInternal = function(thisValue, precision, rounding, fastEMin, fastEMax, lastDiscarded, olderDiscarded, shift, signals) {
        if (precision.signum() < 0) throw ("precision" + " not greater or equal to " + "0" + " (" + precision + ")");
        var bigmantissa = this.helper.GetMantissa(thisValue);
        var neg = bigmantissa.signum() < 0;
        if (neg) bigmantissa = bigmantissa.negate();
        
        var oldmantissa = bigmantissa;
        var mantissaWasZero = (oldmantissa.signum() == 0 && (lastDiscarded | olderDiscarded) == 0);
        var exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
        var flags = 0;
        var accum = this.helper.CreateShiftAccumulatorWithDigits(bigmantissa, lastDiscarded, olderDiscarded);
        var unlimitedPrec = (precision.signum() == 0);
        if (shift != null) {
            accum.ShiftRight(shift);
            exp.Subtract(accum.getDiscardedDigitCount());
        }
        if (precision.signum() > 0) {
            accum.ShiftToDigits(precision);
        } else {
            precision = accum.GetDigitLength();
        }
        var discardedBits = FastInteger.Copy(accum.getDiscardedDigitCount());
        var fastPrecision = precision;
        exp.Add(discardedBits);
        var adjExponent = FastInteger.Copy(exp).Add(accum.GetDigitLength()).SubtractInt(1);
        var newAdjExponent = adjExponent;
        var clamp = null;
        
        var earlyRounded = null;
        if (signals != null && fastEMin != null && adjExponent.compareTo(fastEMin) < 0) {
            earlyRounded = accum.getShiftedInt();
            if (this.RoundGivenBigInt(accum, rounding, neg, earlyRounded)) {
                earlyRounded = earlyRounded.add(BigInteger.ONE);
                if (earlyRounded.testBit(0) == false) {
                    var accum2 = this.helper.CreateShiftAccumulator(earlyRounded);
                    accum2.ShiftToDigits(fastPrecision);
                    if ((accum2.getDiscardedDigitCount()).signum() != 0) {
                        earlyRounded = accum2.getShiftedInt();
                    }
                    newAdjExponent = FastInteger.Copy(exp).Add(accum2.GetDigitLength()).SubtractInt(1);
                }
            }
        }
        if (fastEMax != null && adjExponent.compareTo(fastEMax) > 0) {
            if (mantissaWasZero) {
                if (signals != null) {
                    signals[0] = flags | PrecisionContext.FlagClamped;
                }
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
            if (signals != null) {
                if (earlyRounded.signum() != 0) {
                    if (newAdjExponent.compareTo(fastEMin) < 0) {
                        flags |= PrecisionContext.FlagSubnormal;
                    }
                }
            }
            if (exp.compareTo(fastETiny) < 0) {
                var expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
                expdiff.Add(discardedBits);
                accum = this.helper.CreateShiftAccumulatorWithDigits(oldmantissa, lastDiscarded, olderDiscarded);
                accum.ShiftRight(expdiff);
                var newmantissa = accum.getShiftedIntFast();
                if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (rounding == Rounding.Unnecessary) throw ("Rounding was required");
                }
                if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                    if (signals != null) {
                        if (!mantissaWasZero) flags |= PrecisionContext.FlagRounded;
                        if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                            flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
                        }
                    }
                    if (this.Round(accum, rounding, neg, newmantissa)) {
                        newmantissa.AddInt(1);
                    }
                }
                if (signals != null) {
                    if (newmantissa.signum() == 0) flags |= PrecisionContext.FlagClamped;
                    if ((flags & (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) == (PrecisionContext.FlagSubnormal | PrecisionContext.FlagInexact)) flags |= PrecisionContext.FlagUnderflow | PrecisionContext.FlagRounded;
                    signals[0] = flags;
                }
                if (neg) newmantissa.Negate();
                return this.helper.CreateNew(newmantissa.AsBigInteger(), fastETiny.AsBigInteger());
            }
        }
        var expChanged = false;
        if ((accum.getDiscardedDigitCount()).signum() != 0 || (accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
            if (bigmantissa.signum() != 0) flags |= PrecisionContext.FlagRounded;
            bigmantissa = accum.getShiftedInt();
            if ((accum.getLastDiscardedDigit() | accum.getOlderDiscardedDigits()) != 0) {
                flags |= PrecisionContext.FlagInexact | PrecisionContext.FlagRounded;
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
                var thisdigit = ((str.charCodeAt(i)-48)|0);
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
                    var thisdigit = ((str.charCodeAt(i)-48)|0);
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
                    
                    builderReversed.append(48 + digit);
                    adjustedExponent.Divide(10);
                }
                var count = builderReversed.length();
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
    
    prototype.Abs = function(context) {
        if (this.signum() < 0) {
            return this.Negate(context);
        } else {
            return this.RoundToPrecision(context);
        }
    };
    
    prototype.Negate = function(context) {
        var neg = (this.mantissa).negate();
        return new DecimalFraction(neg, this.exponent).RoundToPrecision(context);
    };
    
    prototype.Subtract = function(decfrac, ctx) {
        if ((decfrac) == null) throw ("decfrac");
        return this.Add(decfrac.Negate(null), ctx);
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
        if (neg) ret = ret.Negate(null);
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
    
    prototype.DivideToSameExponent = function(divisor, rounding) {
        return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    };
    
    prototype.RemainderNaturalScale = function(divisor, ctx) {
        return this.Subtract(this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null), ctx);
    };
    
    prototype.Abs = function(context) {
        if (this.signum() < 0) {
            return this.Negate(context);
        } else {
            return this.RoundToPrecision(context);
        }
    };
    
    prototype.Negate = function(context) {
        var neg = (this.mantissa).negate();
        return new BigFloat(neg, this.exponent).RoundToPrecision(context);
    };
    
    prototype.Subtract = function(decfrac, ctx) {
        if ((decfrac) == null) throw ("decfrac");
        return this.Add(decfrac.Negate(null), ctx);
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
})();
