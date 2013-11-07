package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;


	/**
	 * Represents an arbitrary-precision binary floating-point number.
	 * Consists of a integer mantissa and an integer exponent, both arbitrary-precision.
	 * The value of the number is equal to mantissa * 2^exponent.
	 */
	public final class BigFloat implements Comparable<BigFloat>
	{
		BigInteger exponent;
		BigInteger mantissa;
		
		public BigInteger getExponent() { return exponent; }
		
		public BigInteger getMantissa() { return mantissa; }
		
		
		public boolean equals(BigFloat obj) {
			BigFloat other = ((obj instanceof BigFloat) ? (BigFloat)obj : null);
			if (other == null)
				return false;
			return this.exponent.equals(other.exponent) &&
				this.mantissa.equals(other.mantissa);
		}
		
		@Override public boolean equals(Object obj) {
			return equals(((obj instanceof BigFloat) ? (BigFloat)obj : null));
		}

		@Override public int hashCode() {
			int hashCode_ = 0;
			{
				hashCode_ += 1000000007 * exponent.hashCode();
				hashCode_ += 1000000009 * mantissa.hashCode();
			}
			return hashCode_;
		}
		

		
		public BigFloat(BigInteger exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public BigFloat(long exponentLong, BigInteger mantissa){
			this.exponent=BigInteger.valueOf(exponentLong);
			this.mantissa=mantissa;
		}

		public BigFloat(BigInteger mantissa){
			this.exponent=BigInteger.ZERO;
			this.mantissa=mantissa;
		}

		public BigFloat(long mantissaLong){
			this.exponent=BigInteger.ZERO;
			this.mantissa=BigInteger.valueOf(mantissaLong);
		}

		private BigInteger RescaleByExponentDiff(BigInteger mantissa,
			                      BigInteger e1,
			                      BigInteger e2) {
			boolean negative=(mantissa.signum()<0);
			if(negative)mantissa=mantissa.negate();
			if(e1.compareTo(e2)>0){
				while(e1.compareTo(e2)>0){
					mantissa=mantissa.shiftRight(1);
					e1=e1.subtract(BigInteger.ONE);
				}
			} else {
				while(e1.compareTo(e2)<0){
					mantissa=mantissa.shiftRight(1);
					e1=e1.add(BigInteger.ONE);
				}
			}
			if(negative)mantissa=mantissa.negate();
			return mantissa;
		}
		
		public BigFloat Add(BigFloat decfrac) {
			int expcmp=exponent.compareTo(decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					exponent,mantissa.add(decfrac.mantissa));
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					decfrac.exponent,newmant.add(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					exponent,mantissa.add(newmant));
			}
		}

		public BigFloat Subtract(BigFloat decfrac) {
			int expcmp=exponent.compareTo(decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					exponent,mantissa.subtract(decfrac.mantissa));
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					decfrac.exponent,newmant.subtract(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					exponent,mantissa.subtract(newmant));
			}
		}
		
		public BigFloat Multiply(BigFloat decfrac) {
			BigInteger newexp=(this.exponent.add(decfrac.exponent));
			return new BigFloat(
				newexp,mantissa.multiply(decfrac.mantissa));
		}

		public int signum() {
				return mantissa.signum();
			}
		
		public boolean isZero() {
				return mantissa.signum()==0;
			}

		public int compareTo(BigFloat decfrac) {
			if(decfrac==null)return 1;
			int s=this.signum();
			int ds=decfrac.signum();
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).signum();
		}
		
		
		private static BigInteger[] DivideWithPrecision(
			BigInteger dividend, // NOTE: Assumes dividend is nonnegative
			int divisor,
			int maxIterations
		) {
			BigInteger bigdivisor=BigInteger.valueOf(divisor);
			BigInteger scale=BigInteger.ZERO;
			while(dividend.compareTo(bigdivisor)<0){
				scale=scale.add(BigInteger.ONE);
				dividend=dividend.multiply(BigInteger.TEN);
			}
			while(dividend.compareTo(bigdivisor.multiply(BigInteger.TEN))>=0){
				scale=scale.subtract(BigInteger.ONE);
				bigdivisor=bigdivisor.multiply(BigInteger.TEN);
			}
			BigInteger bigquotient=BigInteger.ZERO;
			BigInteger bigrem=BigInteger.ZERO;
			for(int i=0;i<maxIterations;i++){
				bigrem=dividend.remainder(bigdivisor);
				BigInteger newquotient=(dividend).divide(bigdivisor);
				bigquotient=bigquotient.add(newquotient);
				if(scale.signum()>=0 && bigrem.signum()==0){
					break;
				}
				scale=scale.add(BigInteger.ONE);
				newquotient=newquotient.multiply(BigInteger.TEN);
				dividend=dividend.multiply(BigInteger.TEN);
			}
			if(bigrem.signum()!=0){
				// Round half-up
				BigInteger halfdivisor=bigdivisor;
				halfdivisor=halfdivisor.shiftRight(1);
				if(bigrem.compareTo(halfdivisor)>=0){
					bigrem=bigrem.add(BigInteger.ONE);
				}
			}
			BigInteger negscale=(scale).negate();
			return new BigInteger[]{
				bigquotient,bigrem,negscale
			};
		}
		
		public static BigFloat FromDecimalFraction(DecimalFraction decfrac) {
			BigInteger bigintExp=decfrac.getExponent();
			BigInteger bigintMant=decfrac.getMantissa();
			if(bigintMant.signum()==0)
				return new BigFloat(0);
			if(bigintExp.signum()==0){
				// Integer
				return new BigFloat(bigintMant);
			} else if(bigintExp.signum()>0){
				// Scaled integer
				BigInteger curexp=bigintExp;
				BigInteger bigmantissa=bigintMant;
				while(curexp.signum()>0){
					bigmantissa=bigmantissa.multiply(BigInteger.TEN);
					curexp=curexp.subtract(BigInteger.ONE);
				}
				return new BigFloat(bigmantissa);
			} else {
				// Fractional number
				BigInteger curexp=bigintExp;
				BigInteger scale=curexp;
				BigInteger bigmantissa=bigintMant;
				boolean neg=(bigmantissa.signum()<0);
				if(neg)bigmantissa=(bigmantissa).negate();
				while(curexp.signum()<0){
					if(curexp.compareTo(BigInteger.valueOf(-10))<=0){						
						BigInteger[] results=DivideWithPrecision(bigmantissa,9765625,20);
						bigmantissa=results[0]; // quotient
						BigInteger newscale=results[2];
						scale=scale.add(newscale);
						curexp=curexp.add(BigInteger.TEN);
					} else {
						BigInteger[] results=DivideWithPrecision(bigmantissa,5,20);
						bigmantissa=results[0]; // quotient
						BigInteger newscale=results[2];
						scale=scale.add(newscale);
						curexp=curexp.add(BigInteger.ONE);
					}
				}
				if(neg)bigmantissa=(bigmantissa).negate();
				return new BigFloat(scale,bigmantissa);
			}
		}
		
		public static BigFloat FromSingle(float flt) {
			int value=Float.floatToRawIntBits(flt);
			int fpExponent=(int)((value>>23) & 0xFF);
			if(fpExponent==255)
				throw new ArithmeticException("Value is infinity or NaN");
			int fpMantissa=value & 0x7FFFFF;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1<<23);
			if(fpMantissa!=0){
				while((fpMantissa&1)==0){
					fpExponent++;
					fpMantissa>>=1;
				}
				if((value>>31)!=0)
					fpMantissa=-fpMantissa;
			}
			return new BigFloat(fpExponent-150,BigInteger.valueOf(fpMantissa));
		}

		public static BigFloat FromDouble(double dbl) {
			long value=Double.doubleToRawLongBits(dbl);
			int fpExponent=(int)((value>>52) & 0x7ffL);
			if(fpExponent==2047)
				throw new ArithmeticException("Value is infinity or NaN");
			long fpMantissa=value & 0xFFFFFFFFFFFFFL;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1L<<52);
			if(fpMantissa!=0){
				while((fpMantissa&1)==0){
					fpExponent++;
					fpMantissa>>=1;
				}
				if((value>>63)!=0)
					fpMantissa=-fpMantissa;
			}
			return new BigFloat(fpExponent-1075,BigInteger.valueOf(fpMantissa));
		}

		public BigInteger ToBigInteger() {
			if(this.getExponent().signum()==0){
				return BigInteger.ZERO;
			} else if(this.getExponent().signum()>0){
				BigInteger curexp=this.getExponent();
				BigInteger bigmantissa=this.getMantissa();
				boolean neg=(bigmantissa.signum()<0);
				if(neg)bigmantissa=bigmantissa.negate();
				while(curexp.signum()>0 && bigmantissa.signum()!=0){
					bigmantissa=bigmantissa.shiftLeft(1);
					curexp=curexp.subtract(BigInteger.ONE);
				}
				if(neg)bigmantissa=bigmantissa.negate();
				return bigmantissa;
			} else {
				BigInteger curexp=this.getExponent();
				BigInteger bigmantissa=this.getMantissa();
				boolean neg=(bigmantissa.signum()<0);
				if(neg)bigmantissa=bigmantissa.negate();
				while(curexp.signum()<0 && bigmantissa.signum()!=0){
					bigmantissa=bigmantissa.shiftRight(1);
					curexp=curexp.add(BigInteger.ONE);
				}
				if(neg)bigmantissa=bigmantissa.negate();
				return bigmantissa;
			}
		}
		
		/**
		 * Converts this value to a 32-bit floating-point number. The half-up
		 * rounding mode is used.
		 * @return The closest 32-bit floating-point number to this value.
		 */
		public float ToSingle() {
			BigInteger bigmant=(this.mantissa).abs();
			BigInteger bigexponent=this.exponent;
			int lastRoundedBit=0;
			if(this.mantissa.signum()==0){
				return 0.0f;
			}
			while(bigmant.compareTo(BigInteger.valueOf((1L<<23)))<0){
				bigmant=bigmant.shiftLeft(1);
				bigexponent=bigexponent.subtract(BigInteger.ONE);
			}
			while(bigmant.compareTo(BigInteger.valueOf((1L<<24)))>=0){
				BigInteger bigsticky=bigmant.and(BigInteger.ONE);
				lastRoundedBit=bigsticky.intValue();
				bigmant=bigmant.shiftRight(1);
				bigexponent=bigexponent.add(BigInteger.ONE);
			}
			if(lastRoundedBit==1){ // Round half-up
				bigmant=bigmant.add(BigInteger.ONE);
				if(bigmant.compareTo(BigInteger.valueOf((1L<<24)))==0){
					bigmant=bigmant.shiftRight(1);
					bigexponent=bigexponent.add(BigInteger.ONE);
				}
			}
			boolean subnormal=false;
			if(bigexponent.compareTo(BigInteger.valueOf(104))>0){
				// exponent too big
				return (this.mantissa.signum()<0) ?
					Float.NEGATIVE_INFINITY :
					Float.POSITIVE_INFINITY;
			} else if(bigexponent.compareTo(BigInteger.valueOf(-149))<0){
				// subnormal
				subnormal=true;
				while(bigexponent.compareTo(BigInteger.valueOf(-149))<0){
					BigInteger bigsticky=bigmant.and(BigInteger.ONE);
					lastRoundedBit=bigsticky.intValue();
					bigmant=bigmant.shiftRight(1);
					bigexponent=bigexponent.add(BigInteger.ONE);
				}
				if(lastRoundedBit==1){ // Round half-up
					bigmant=bigmant.add(BigInteger.ONE);
					if(bigmant.compareTo(BigInteger.valueOf((1L<<24)))==0){
						bigmant=bigmant.shiftRight(1);
						bigexponent=bigexponent.add(BigInteger.ONE);
					}
				}
			}
			if(bigexponent.compareTo(BigInteger.valueOf(-149))<0){
				// exponent too small
				return (this.mantissa.signum()<0) ?
					Float.intBitsToFloat(1<<31) :
					Float.intBitsToFloat(0);
			} else {
				int smallexponent=bigexponent.intValue();
				smallexponent=smallexponent+150;
				bigmant=bigmant.and(BigInteger.valueOf(0x7FFFFFL));
				int smallmantissa=bigmant.intValue();
				if(!subnormal){
					smallmantissa|=(smallexponent<<23);
				}
				if(this.mantissa.signum()<0)smallmantissa|=(1<<31);
				return Float.intBitsToFloat(smallmantissa);
			}
		}

		
		/**
		 * Converts this value to a 64-bit floating-point number. The half-up
		 * rounding mode is used.
		 * @return The closest 64-bit floating-point number to this value.
		 */
		public double ToDouble() {
			BigInteger bigmant=(this.mantissa).abs();
			BigInteger bigexponent=this.exponent;
			int lastRoundedBit=0;
			if(this.mantissa.signum()==0){
				return 0.0;
			}
			while(bigmant.compareTo(BigInteger.valueOf((1L<<52)))<0){
				bigmant=bigmant.shiftLeft(1);
				bigexponent=bigexponent.subtract(BigInteger.ONE);
			}
			while(bigmant.compareTo(BigInteger.valueOf((1L<<53)))>=0){
				BigInteger bigsticky=bigmant.and(BigInteger.ONE);
				lastRoundedBit=bigsticky.intValue();
				bigmant=bigmant.shiftRight(1);
				bigexponent=bigexponent.add(BigInteger.ONE);
			}
			if(lastRoundedBit==1){ // Round half-up
				bigmant=bigmant.add(BigInteger.ONE);
				if(bigmant.compareTo(BigInteger.valueOf((1L<<53)))==0){
					bigmant=bigmant.shiftRight(1);
					bigexponent=bigexponent.add(BigInteger.ONE);
				}
			}
			boolean subnormal=false;
			if(bigexponent.compareTo(BigInteger.valueOf(971))>0){
				// exponent too big
				return (this.mantissa.signum()<0) ?
					Double.NEGATIVE_INFINITY :
					Double.POSITIVE_INFINITY;
			} else if(bigexponent.compareTo(BigInteger.valueOf(-1074))<0){
				// subnormal
				subnormal=true;
				while(bigexponent.compareTo(BigInteger.valueOf(-1074))<0){
					BigInteger bigsticky=bigmant.and(BigInteger.ONE);
					lastRoundedBit=bigsticky.intValue();
					bigmant=bigmant.shiftRight(1);
					bigexponent=bigexponent.add(BigInteger.ONE);
				}
				if(lastRoundedBit==1){ // Round half-up
					bigmant=bigmant.add(BigInteger.ONE);
					if(bigmant.compareTo(BigInteger.valueOf((1L<<53)))==0){
						bigmant=bigmant.shiftRight(1);
						bigexponent=bigexponent.add(BigInteger.ONE);
					}
				}
			}
			if(bigexponent.compareTo(BigInteger.valueOf(-1074))<0){
				// exponent too small
				return (this.mantissa.signum()<0) ?
					Double.longBitsToDouble(1L<<63) :
					Double.longBitsToDouble(0);
			} else {
				long smallexponent=bigexponent.longValue();
				smallexponent=smallexponent+1075;
				bigmant=bigmant.and(BigInteger.valueOf(0xFFFFFFFFFFFFFL));
				long smallmantissa=bigmant.longValue();
				if(!subnormal){
					smallmantissa|=(smallexponent<<52);
				}
				if(this.mantissa.signum()<0)smallmantissa|=(1L<<63);
				return Double.longBitsToDouble(smallmantissa);
			}
		}
		
		/**
		 * 
		 */
		@Override public String toString() {
			return DecimalFraction.FromBigFloat(this).toString();
		}

		/**
		 * 
		 */
		public String ToEngineeringString() {
			return DecimalFraction.FromBigFloat(this).ToEngineeringString();
		}

		/**
		 * 
		 */
		public String ToPlainString() {
			return DecimalFraction.FromBigFloat(this).ToPlainString();
		}
	}
