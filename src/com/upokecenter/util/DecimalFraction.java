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
	 * Represents an arbitrary-precision decimal floating-point number.
	 * Consists of a integer mantissa and an integer exponent, both arbitrary-precision.
	 * The value of the number is equal to mantissa * 10^exponent.
	 */
	public final class DecimalFraction implements Comparable<DecimalFraction>
	{
		BigInteger exponent;
		BigInteger mantissa;
		
		public BigInteger getExponent() { return exponent; }
		
		public BigInteger getMantissa() { return mantissa; }
		
		
		public boolean equals(DecimalFraction obj) {
			DecimalFraction other = ((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null);
			if (other == null)
				return false;
			return this.exponent.equals(other.exponent) &&
				this.mantissa.equals(other.mantissa);
		}
		
		@Override public boolean equals(Object obj) {
			return equals(((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null));
		}
		
		@Override public int hashCode() {
			int hashCode_ = 0;
			{
				hashCode_ += 1000000007 * exponent.hashCode();
				hashCode_ += 1000000009 * mantissa.hashCode();
			}
			return hashCode_;
		}
		

		
		public DecimalFraction(BigInteger exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public DecimalFraction(long exponentLong, BigInteger mantissa){
			this.exponent=BigInteger.valueOf(exponentLong);
			this.mantissa=mantissa;
		}

		public DecimalFraction(BigInteger mantissa){
			this.exponent=BigInteger.ZERO;
			this.mantissa=mantissa;
		}

		public DecimalFraction(long mantissaLong){
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
					mantissa=mantissa.multiply(BigInteger.TEN);
					e1=e1.subtract(BigInteger.ONE);
				}
			} else {
				while(e1.compareTo(e2)<0){
					mantissa=mantissa.multiply(BigInteger.TEN);
					e1=e1.add(BigInteger.ONE);
				}
			}
			if(negative)mantissa=mantissa.negate();
			return mantissa;
		}
		
		public DecimalFraction Add(DecimalFraction decfrac) {
			int expcmp=exponent.compareTo(decfrac.exponent);
			if(expcmp==0){
				return new DecimalFraction(
					exponent,mantissa.add(decfrac.mantissa));
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant.add(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa.add(newmant));
			}
		}

		public DecimalFraction Subtract(DecimalFraction decfrac) {
			int expcmp=exponent.compareTo(decfrac.exponent);
			if(expcmp==0){
				return new DecimalFraction(
					exponent,mantissa.subtract(decfrac.mantissa));
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant.subtract(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa.subtract(newmant));
			}
		}
		
		public DecimalFraction Multiply(DecimalFraction decfrac) {
			BigInteger newexp=(this.exponent.add(decfrac.exponent));
			return new DecimalFraction(
				newexp,mantissa.multiply(decfrac.mantissa));
		}

		public int signum() {
				return mantissa.signum();
			}
		
		public boolean isZero() {
				return mantissa.signum()==0;
			}

		public int compareTo(DecimalFraction decfrac) {
			if(decfrac==null)return 1;
			int s=this.signum();
			int ds=decfrac.signum();
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).signum();
		}
		
		private boolean InsertString(StringBuilder builder, BigInteger index, char c) {
			if(index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0){
				throw new UnsupportedOperationException();
			}
			int iindex=index.intValue();
			builder.insert(iindex,c);
			return true;
		}

		private boolean InsertString(StringBuilder builder, BigInteger index, char c, BigInteger count) {
			if(count.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0){
				throw new UnsupportedOperationException();
			}
			if(index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0){
				throw new UnsupportedOperationException();
			}
			int icount=count.intValue();
			int iindex=index.intValue();
			for(int i=icount-1;i>=0;i--){
				builder.insert(iindex,c);
			}
			return true;
		}

		private boolean AppendString(StringBuilder builder, char c, BigInteger count) {
			if(count.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0){
				throw new UnsupportedOperationException();
			}
			int icount=count.intValue();
			for(int i=icount-1;i>=0;i--){
				builder.append(c);
			}
			return true;
		}

		private boolean InsertString(StringBuilder builder, BigInteger index, String c) {
			if(index.compareTo(BigInteger.valueOf(Integer.MAX_VALUE))>0){
				throw new UnsupportedOperationException();
			}
			int iindex=index.intValue();
			builder.insert(iindex,c);
			return true;
		}
		private String ToStringInternal(int mode) {
			// Using Java's rules for converting DecimalFraction
			// values to a String
			StringBuilder builder=new StringBuilder();
			builder.append(this.mantissa.toString());
			BigInteger adjustedExponent=this.exponent;
			BigInteger scale=(this.exponent).negate();
			BigInteger sbLength=BigInteger.valueOf((builder.length()));
			BigInteger negaPos=BigInteger.ZERO;
			if(builder.charAt(0)=='-'){
				sbLength=sbLength.subtract(BigInteger.ONE);
				negaPos=BigInteger.ONE;
			}
			boolean iszero=(this.mantissa.signum()==0);
			if(mode==2 && iszero && scale.signum()<0){
				// special case for zero in plain
				return builder.toString();
			}
			adjustedExponent=adjustedExponent.add(sbLength);
			adjustedExponent=adjustedExponent.subtract(BigInteger.ONE);
			BigInteger decimalPointAdjust=BigInteger.ONE;
			BigInteger threshold=BigInteger.valueOf((-6));
			if(mode==1){ // engineering String adjustments
				BigInteger newExponent=adjustedExponent;
				boolean adjExponentNegative=(adjustedExponent.signum()<0);
				BigInteger phase=(adjustedExponent).abs().remainder(BigInteger.valueOf(3));
				int intphase=phase.intValue();
				if(iszero && (adjustedExponent.compareTo(threshold)<0 ||
				              scale.signum()<0)){
					if(intphase==1){
						if(adjExponentNegative){
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
							newExponent=newExponent.add(BigInteger.ONE);
						} else {
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
							newExponent=newExponent.add(BigInteger.valueOf(2));
						}
					} else if(intphase==2){
						if(!adjExponentNegative){
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
							newExponent=newExponent.add(BigInteger.ONE);
						} else {
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
							newExponent=newExponent.add(BigInteger.valueOf(2));
						}
					}
					threshold=threshold.add(BigInteger.ONE);
				} else {
					if(intphase==1){
						if(!adjExponentNegative){
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
							newExponent=newExponent.subtract(BigInteger.ONE);
						} else {
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
							newExponent=newExponent.subtract(BigInteger.valueOf(2));
						}
					} else if(intphase==2){
						if(adjExponentNegative){
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.ONE);
							newExponent=newExponent.subtract(BigInteger.ONE);
						} else {
							decimalPointAdjust=decimalPointAdjust.add(BigInteger.valueOf(2));
							newExponent=newExponent.subtract(BigInteger.valueOf(2));
						}
					}
				}
				adjustedExponent=newExponent;
			}
			if(mode==2 || ((adjustedExponent.compareTo(threshold)>=0 &&
			                scale.signum()>=0))){
				BigInteger decimalPoint=(scale).negate();
				decimalPoint=decimalPoint.add(negaPos);
				decimalPoint=decimalPoint.add(sbLength);
				if(scale.signum()>0){
					int cmp=decimalPoint.compareTo(negaPos);
					if(cmp<0){
						InsertString(builder,negaPos,'0',(negaPos.subtract(decimalPoint)));
						InsertString(builder,negaPos,"0.");
					} else if(cmp==0){
						InsertString(builder,decimalPoint,"0.");
					} else if(decimalPoint.compareTo(
						(BigInteger.valueOf((builder.length())).add(negaPos)))>0){
						InsertString(builder,(sbLength.add(negaPos)),'.');
						InsertString(builder,(sbLength.add(negaPos)),'0',
						             (decimalPoint.subtract(BigInteger.valueOf((builder.length())))));
					} else {
						InsertString(builder,decimalPoint,'.');
					}
				}
				if(mode==2 && scale.signum()<0){
					BigInteger negscale=(scale).negate();
					AppendString(builder,'0',negscale);
				}
				return builder.toString();
			} else {
				if(mode==1 && iszero && decimalPointAdjust.compareTo(BigInteger.ONE)>0){
					builder.append('.');
					AppendString(builder,'0',(decimalPointAdjust.subtract(BigInteger.ONE)));
				} else {
					BigInteger tmp=negaPos.add(decimalPointAdjust);
					int cmp=tmp.compareTo(BigInteger.valueOf((builder.length())));
					if(cmp>0){
						AppendString(builder,'0',(tmp.subtract(BigInteger.valueOf((builder.length())))));
					}
					if(cmp<0){
						InsertString(builder,tmp,'.');
					}
				}
				if(adjustedExponent.signum()!=0){
					builder.append('E');
					builder.append(adjustedExponent.signum()<0 ? '-' : '+');
					BigInteger sbPos=BigInteger.valueOf((builder.length()));
					adjustedExponent=(adjustedExponent).abs();
					while(adjustedExponent.signum()!=0){
						BigInteger digit=(adjustedExponent.remainder(BigInteger.TEN));
						InsertString(builder,sbPos,(char)('0'+digit.intValue()));
						adjustedExponent=adjustedExponent.divide(BigInteger.TEN);
					}
				}
				return builder.toString();
			}
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
					bigmantissa=bigmantissa.multiply(BigInteger.TEN);
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
					bigmantissa=bigmantissa.divide(BigInteger.TEN);
					curexp=curexp.add(BigInteger.ONE);
				}
				if(neg)bigmantissa=bigmantissa.negate();
				return bigmantissa;
			}
		}
		
		public float ToSingle() {
			return BigFloat.FromDecimalFraction(this).ToSingle();
		}
		
		public double ToDouble() {
			return BigFloat.FromDecimalFraction(this).ToDouble();
		}

		public static DecimalFraction FromSingle(float flt) {
			int value=Float.floatToRawIntBits(flt);
			int fpExponent=(int)((value>>23) & 0xFF);
			if(fpExponent==255)
				throw new ArithmeticException("Value is infinity or NaN");
			int fpMantissa=value & 0x7FFFFF;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1<<23);
			if(fpMantissa==0)return new DecimalFraction(0);
			fpExponent-=150;
			while((fpMantissa&1)==0){
				fpExponent++;
				fpMantissa>>=1;
			}
			boolean neg=((value>>31)!=0);
			if(fpExponent==0){
				if(neg)fpMantissa=-fpMantissa;
				return new DecimalFraction(fpMantissa);
			} else if(fpExponent>0){
				// Value is an integer
				BigInteger bigmantissa=BigInteger.valueOf(fpMantissa);
				bigmantissa=bigmantissa.shiftLeft(fpExponent);
				if(neg)bigmantissa=(bigmantissa).negate();
				return new DecimalFraction(bigmantissa);
			} else {
				// Value has a fractional part
				BigInteger bigmantissa=BigInteger.valueOf(fpMantissa);
				long scale=fpExponent;
				int exp=-fpExponent;
				for(int i=0;i<exp;i++){
					bigmantissa=bigmantissa.multiply(BigInteger.valueOf(5));
				}
				if(neg)bigmantissa=(bigmantissa).negate();
				return new DecimalFraction(scale,bigmantissa);
			}
		}

		public static DecimalFraction FromDouble(double dbl) {
			long value=Double.doubleToRawLongBits(dbl);
			int fpExponent=(int)((value>>52) & 0x7ffL);
			if(fpExponent==2047)
				throw new ArithmeticException("Value is infinity or NaN");
			long fpMantissa=value & 0xFFFFFFFFFFFFFL;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1L<<52);
			if(fpMantissa==0)return new DecimalFraction(0);
			fpExponent-=1075;
			while((fpMantissa&1)==0){
				fpExponent++;
				fpMantissa>>=1;
			}
			boolean neg=((value>>63)!=0);
			if(fpExponent==0){
				if(neg)fpMantissa=-fpMantissa;
				return new DecimalFraction(fpMantissa);
			} else if(fpExponent>0){
				// Value is an integer
				BigInteger bigmantissa=BigInteger.valueOf(fpMantissa);
				bigmantissa=bigmantissa.shiftLeft(fpExponent);
				if(neg)bigmantissa=(bigmantissa).negate();
				return new DecimalFraction(bigmantissa);
			} else {
				// Value has a fractional part
				BigInteger bigmantissa=BigInteger.valueOf(fpMantissa);
				long scale=fpExponent;
				int exp=-fpExponent;
				for(int i=0;i<exp;i++){
					bigmantissa=bigmantissa.multiply(BigInteger.valueOf(5));
				}
				if(neg)bigmantissa=(bigmantissa).negate();
				return new DecimalFraction(scale,bigmantissa);
			}
		}
		
		private static BigInteger BigInt5=BigInteger.valueOf(5);
		private static BigInteger BigInt10=BigInteger.TEN;
		private static BigInteger BigInt20=BigInteger.valueOf(20);
		private static BigInteger BigIntNeg5=BigInteger.valueOf((-5));
		private static BigInteger BigIntNeg10=BigInteger.valueOf((-10));
		private static BigInteger BigIntNeg20=BigInteger.valueOf((-20));
		private static BigInteger BigInt5Pow5=BigInteger.valueOf(3125);
		private static BigInteger BigInt5Pow10=BigInteger.valueOf(9765625);
		private static BigInteger BigInt5Pow20=BigInteger.valueOf((95367431640625L));
		
		public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
			BigInteger bigintExp=bigfloat.getExponent();
			BigInteger bigintMant=bigfloat.getMantissa();
			if(bigintExp.signum()==0){
				// Integer
				return new DecimalFraction(bigintMant);
			} else if(bigintExp.signum()>0){
				// Scaled integer
				BigInteger curexp=bigintExp;
				BigInteger bigmantissa=bigintMant;
				boolean neg=(bigmantissa.signum()<0);
				if(neg)bigmantissa=(bigmantissa).negate();
				while(curexp.signum()>0){
					int shift=64;
					if(curexp.compareTo(BigInteger.valueOf(64))<0){
						shift=curexp.intValue();
					}
					bigmantissa=bigmantissa.shiftLeft(shift);
					curexp=curexp.subtract(BigInteger.valueOf(shift));
				}
				if(neg)bigmantissa=(bigmantissa).negate();
				return new DecimalFraction(bigmantissa);
			} else {
				// Fractional number
				BigInteger bigmantissa=bigintMant;
				BigInteger curexp=bigintExp;
				while(curexp.signum()<0){
					if(curexp.compareTo(BigIntNeg20)<=0){
						bigmantissa=bigmantissa.multiply(BigInt5Pow20);
						curexp=curexp.add(BigInt20);
					} else if(curexp.compareTo(BigIntNeg10)<=0){
						bigmantissa=bigmantissa.multiply(BigInt5Pow10);
						curexp=curexp.add(BigInt10);
					} else if(curexp.compareTo(BigIntNeg5)<=0){
						bigmantissa=bigmantissa.multiply(BigInt5Pow5);
						curexp=curexp.add(BigInt5);
					} else {
						bigmantissa=bigmantissa.multiply(BigInt5);
						curexp=curexp.add(BigInteger.ONE);
					}
				}
				return new DecimalFraction(bigintExp,bigmantissa);
			}
		}
		
		/**
		 * 
		 */
		@Override public String toString() {
			return ToStringInternal(0);
		}

		/**
		 * 
		 */
		public String ToEngineeringString() {
			return ToStringInternal(1);
		}

		/**
		 * 
		 */
		public String ToPlainString() {
			return ToStringInternal(2);
		}
	}
