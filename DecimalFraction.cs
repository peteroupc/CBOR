
using System;
using System.Numerics;

namespace PeterO
{	
	class DecimalFraction
	{
		long exponent;
		
		public long Exponent {
			get { return exponent; }
		}
		BigInteger mantissa;
		
		public BigInteger Mantissa {
			get { return mantissa; }
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			DecimalFraction other = obj as DecimalFraction;
			if (other == null)
				return false;
			return this.exponent == other.exponent && this.mantissa == other.mantissa;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * exponent.GetHashCode();
				hashCode += 1000000009 * mantissa.GetHashCode();
			}
			return hashCode;
		}
		#endregion

		
		public DecimalFraction(long exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public DecimalFraction(BigInteger mantissa){
			this.exponent=0;
			this.mantissa=mantissa;
		}

		public BigInteger
			RescaleByExponentDiff(BigInteger mantissa, long e1, long e2){
			bool negative=(mantissa.Sign<0);
			if(negative)mantissa=-mantissa;
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
			if(negative)mantissa=-mantissa;
			return mantissa;
		}
		
		public DecimalFraction Add(DecimalFraction decfrac){
			if(exponent==decfrac.exponent){
				return new DecimalFraction(
					exponent,mantissa+(BigInteger)decfrac.mantissa);
			} else if(exponent>decfrac.exponent){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant+(BigInteger)decfrac.mantissa);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa+(BigInteger)newmant);
			}
		}

		public DecimalFraction Subtract(DecimalFraction decfrac){
			if(exponent==decfrac.exponent){
				return new DecimalFraction(
					exponent,mantissa-(BigInteger)decfrac.mantissa);
			} else if(exponent>decfrac.exponent){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					decfrac.exponent,newmant-(BigInteger)decfrac.mantissa);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new DecimalFraction(
					exponent,mantissa-(BigInteger)newmant);
			}
		}
		
		public DecimalFraction Multiply(DecimalFraction decfrac){
			long newexp=checked(this.exponent+decfrac.exponent);
			return new DecimalFraction(newexp,mantissa*decfrac.mantissa);
		}

		public int Sign {
			get {
				return mantissa.Sign;
			}
		}
		
		public bool IsZero {
			get {
				return mantissa.IsZero;
			}
		}

		public int CompareTo(DecimalFraction decfrac){
			if(decfrac==null)return 1;
			int s=this.Sign;
			int ds=decfrac.Sign;
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).Sign;
		}
		
		private string ToStringInternal(int mode)
		{
			// Using Java's rules for converting BigDecimal values to a string
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			sb.Append(this.mantissa.ToString(System.Globalization.CultureInfo.InvariantCulture));
			long adjustedExponent=this.exponent;
			long scale=-this.exponent;
			long sbLength=sb.Length;
			long negaPos=0;
			if(sb[0]=='-'){
				sbLength--;
				negaPos=1;
			}
			bool iszero=(this.mantissa.IsZero);
			if(mode==2 && iszero && scale<0){
				// special case for zero in plain
				return sb.ToString();
			}
			adjustedExponent+=sbLength-1;
			int decimalPointAdjust=1;
			int threshold=-6;
			if(mode==1){ // engineering string adjustments
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
					} else if(decimalPoint>sb.Length+negaPos){
						sb.Insert((int)(sbLength+negaPos),".");
						sb.Insert((int)(sbLength+negaPos),"0",(int)(decimalPoint-sb.Length));
					} else {
						sb.Insert((int)decimalPoint,".");
					}
				}
				if(mode==2 && scale<0){
					for(long i=0;i<-scale;i++){
						sb.Append('0');
					}
				}
				return sb.ToString();
			} else {
				if(mode==1 && iszero && decimalPointAdjust>1){
					sb.Append(".");
					sb.Append('0',decimalPointAdjust-1);
				} else {
					if(negaPos+decimalPointAdjust>sb.Length){
						sb.Append('0',(int)((negaPos+decimalPointAdjust)-sb.Length));
					}
					if((negaPos+decimalPointAdjust<sb.Length)){
						sb.Insert((int)negaPos+decimalPointAdjust,".");
					}
				}
				if(adjustedExponent!=0){
					sb.Append("E");
					sb.Append(adjustedExponent<0 ? "-" : "+");
					int sbPos=sb.Length;
					if(adjustedExponent<0)
						adjustedExponent=-adjustedExponent;
					if(adjustedExponent==0){
						sb.Append("0");
					} else {
						while(adjustedExponent>0){
							sb.Insert(sbPos,(char)('0'+(int)(adjustedExponent%10)));
							adjustedExponent/=10;
						}
					}
				}
				return sb.ToString();
			}
		}
		///<summary>
		/// </summary>
		///<returns>
		///
		///</returns>
		///<remarks>
		///The format of the return value is exactly the same as that of the java.math.BigDecimal.toString() method.</remarks>
		public override string ToString()
		{
			return ToStringInternal(0);
		}

		///<summary>
		/// </summary>
		///<returns>
		///
		///</returns>
		///<remarks>
		///The format of the return value is exactly the same as that of the java.math.BigDecimal.toEngineeringString() method.</remarks>
		public string ToEngineeringString()
		{
			return ToStringInternal(1);
		}

		///<summary>
		/// </summary>
		///<returns>
		///
		///</returns>
		///<remarks>
		///The format of the return value is exactly the same as that of the java.math.BigDecimal.toPlainString() method.</remarks>
		public string ToPlainString()
		{
			return ToStringInternal(2);
		}
	}
}