package com.upokecenter.util;


import java.math.*;

	
	class DecimalFraction
	{
		long exponent;
		
		public long getExponent() { return exponent; }
		BigInteger mantissa;
		
		public BigInteger getMantissa() { return mantissa; }
		
		
		@Override public boolean equals(Object obj) {
			DecimalFraction other = ((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null);
			if (other == null)
				return false;
			return this.exponent == other.exponent && this.mantissa == other.mantissa;
		}
		
		@Override public int hashCode() {
			int hashCode_ = 0;
			{
				hashCode_ += 1000000007 * ((Object)exponent).hashCode();
				hashCode_ += 1000000009 * mantissa.hashCode();
			}
			return hashCode_;
		}
		

		
		public DecimalFraction(long exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public BigInteger RescaleByExponentDiff(BigInteger mantissa, long e1, long e2) {
			boolean negative=(mantissa.signum()<0);
			if(negative)mantissa=mantissa.negate();
			if(e1>e2){
				while(e1>e2){
					mantissa*=10;
					e1--;
				}
			} else {
				while(e1<e2){
					mantissa*=10;
					e1++;
				}
			}
			if(negative)mantissa=mantissa.negate();
			return mantissa;
		}
		
		public DecimalFraction Add(DecimalFraction decfrac) {
			if(exponent==decfrac.exponent){
				return new DecimalFraction(
					exponent,mantissa+BigInteger.valueOf(decfrac.mantissa));
			} else if(exponent>decfrac.exponent){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant+BigInteger.valueOf(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa+newmant);
			}
		}

		public DecimalFraction Subtract(DecimalFraction decfrac) {
			if(exponent==decfrac.exponent){
				return new DecimalFraction(
					exponent,mantissa-BigInteger.valueOf(decfrac.mantissa));
			} else if(exponent>decfrac.exponent){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant-BigInteger.valueOf(decfrac.mantissa));
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa-newmant);
			}
		}
		
		public DecimalFraction Multiply(DecimalFraction decfrac) {
			long newexp=checked(this.exponent+decfrac.exponent);
			return new DecimalFraction(newexp,mantissa*decfrac.mantissa);
		}

		public int getSign() {
				return mantissa.signum();
			}
		
		public boolean isZero() {
				return mantissa.equals(BigInteger.ZERO);
			}

		public int CompareTo(DecimalFraction decfrac) {
			if(decfrac==null)return 1;
			int s=this.signum();
			int ds=decfrac.signum();
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).signum();
		}
		
		private String ToStringInternal(int mode) {
			// Using Java's rules for converting BigDecimal values to a String
			StringBuilder sb=new StringBuilder();
			sb.append(this.mantissa.toString());
			long adjustedExponent=this.exponent;
			long scale=-this.exponent;
			long sbLength=sb.length;
			long negaPos=0;
			if(sb.set(0,='-'){
				sbLength--);
				negaPos=1;
			}
			boolean iszero=(this.mantissa.equals(BigInteger.ZERO));
			if(mode==2 && iszero && scale<0){
				// special case for zero in plain
				return sb.toString();
			}
			adjustedExponent+=sbLength-1;
			int decimalPointAdjust=1;
			int threshold=-6;
			if(mode==1){ // engineering String adjustments
				long newExponent=adjustedExponent;
				if(iszero && (adjustedExponent<-6 || scale<0)){
					if((Math.Abs(adjustedExponent)%3)==1){
						decimalPointAdjust+=(adjustedExponent<0) ? 1 : 2;
						newExponent-=(adjustedExponent<0) ? -1 : -2;
					} else if((Math.Abs(adjustedExponent)%3)==2){
						decimalPointAdjust+=(adjustedExponent<0) ? 2 : 1;
						newExponent-=(adjustedExponent<0) ? -2 : -1;
					}
					threshold+=1;
					
				} else {
					if((Math.Abs(adjustedExponent)%3)==1){
						decimalPointAdjust+=(adjustedExponent<0) ? 2 : 1;
						newExponent-=(adjustedExponent<0) ? 2 : 1;
					} else if((Math.Abs(adjustedExponent)%3)==2){
						decimalPointAdjust+=(adjustedExponent<0) ? 1 : 2;
						newExponent-=(adjustedExponent<0) ? 1 : 2;
					}
				}
				adjustedExponent=newExponent;
			}
			if(mode==2 || ((adjustedExponent>=threshold && scale>=0))){
				long decimalPoint=sbLength-scale+negaPos;
				if(scale>0){
					if(decimalPoint<negaPos){
						sb.Insert((int)negaPos,"0",(int)(negaPos-decimalPoint));
						sb.Insert((int)negaPos,"0.");
					} else if(decimalPoint==negaPos){
						sb.Insert((int)decimalPoint,"0.");
					} else if(decimalPoint>sb.length+negaPos){
						sb.Insert((int)(sbLength+negaPos),".");
						sb.Insert((int)(sbLength+negaPos),"0",(int)(decimalPoint-sb.length));
					} else {
						sb.Insert((int)decimalPoint,".");
					}
				}
				if(mode==2 && scale<0){
					for(long i=0;i<-scale;i++){
						sb.append('0');
					}
				}
				return sb.toString();
			} else {
				if(mode==1 && iszero && decimalPointAdjust>1){
					sb.append(".");
					sb.append('0',decimalPointAdjust-1);
				} else {
					if(negaPos+decimalPointAdjust>sb.length){
						sb.append('0',(int)((negaPos+decimalPointAdjust)-sb.length));
					}
					if((negaPos+decimalPointAdjust<sb.length)){
						sb.Insert((int)negaPos+decimalPointAdjust,".");
					}
				}
				if(adjustedExponent!=0){
					sb.append("E");
					sb.append(adjustedExponent<0 ? "-" : "+");
					int sbPos=sb.length;
					if(adjustedExponent<0)
						adjustedExponent=-adjustedExponent;
					if(adjustedExponent==0){
						sb.append("0");
					} else {
						while(adjustedExponent>0){
							sb.Insert(sbPos,(char)('0'+(int)(adjustedExponent%10)));
							adjustedExponent/=10;
						}
					}
				}
				return sb.toString();
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
