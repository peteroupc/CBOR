package com.upokecenter.util;



import java.math.*;


	/**
	 * Represents an arbitrary-precision decimal floating-point number.
	 */
	public class DecimalFraction implements Comparable<DecimalFraction>
	{
		BigInteger exponent;
		BigInteger mantissa;
		
		public BigInteger getExponent() { return exponent; }
		
		public BigInteger getMantissa() { return mantissa; }
		
		
		@Override public boolean equals(Object obj) {
			DecimalFraction other = ((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null);
			if (other == null)
				return false;
			return this.exponent.equals(other.exponent) && 
				this.mantissa.equals(other.mantissa);
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

		public BigInteger RescaleByExponentDiff(BigInteger mantissa,
			                      BigInteger e1,
			                      BigInteger e2) {
			boolean negative=(mantissa.signum()<0);
			if(negative)mantissa=mantissa.negate();
			if(e1.compareTo(e2)>0){
				while(e1.compareTo(e2)>0){
					mantissa=mantissa.multiply(BigInteger.valueOf(10));
					e1=e1.subtract(BigInteger.ONE);
				}
			} else {
				while(e1.compareTo(e2)<0){
					mantissa=mantissa.multiply(BigInteger.valueOf(10));
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
				return mantissa.equals(BigInteger.ZERO);
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
			boolean iszero=(this.mantissa.equals(BigInteger.ZERO));
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
				if(!adjustedExponent.equals(BigInteger.ZERO)){
					builder.append('E');
					builder.append(adjustedExponent.signum()<0 ? '-' : '+');
					BigInteger sbPos=BigInteger.valueOf((builder.length()));
					adjustedExponent=(adjustedExponent).abs();
					while(!adjustedExponent.equals(BigInteger.ZERO)){
						BigInteger digit=(adjustedExponent.remainder(BigInteger.valueOf(10)));
						InsertString(builder,sbPos,(char)('0'+digit.intValue()));
						adjustedExponent=adjustedExponent.divide(BigInteger.valueOf(10));
					}
				}
				return builder.toString();
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
