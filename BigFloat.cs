/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
using System.Numerics;

namespace PeterO
{
	/// <summary>
	/// Represents an arbitrary-precision binary floating-point number.
	/// Consists of a integer mantissa and an integer exponent,
	/// both arbitrary-precision.  The value of the number is equal
	/// to mantissa * 2^exponent.
	/// </summary>
	public sealed class BigFloat : IComparable<BigFloat>, IEquatable<BigFloat>
	{
		BigInteger exponent;
		BigInteger mantissa;
		
		public BigInteger Exponent {
			get { return exponent; }
		}
		
		public BigInteger Mantissa {
			get { return mantissa; }
		}
		
		#region Equals and GetHashCode implementation
		public bool Equals(BigFloat obj)
		{
			BigFloat other = obj as BigFloat;
			if (other == null)
				return false;
			return this.exponent.Equals(other.exponent) &&
				this.mantissa.Equals(other.mantissa);
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as BigFloat);
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

		
		public BigFloat(BigInteger exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public BigFloat(long exponentLong, BigInteger mantissa){
			this.exponent=(BigInteger)exponentLong;
			this.mantissa=mantissa;
		}

		public BigFloat(BigInteger mantissa){
			this.exponent=BigInteger.Zero;
			this.mantissa=mantissa;
		}

		public BigFloat(long mantissaLong){
			this.exponent=BigInteger.Zero;
			this.mantissa=(BigInteger)mantissaLong;
		}

		private BigInteger
			RescaleByExponentDiff(BigInteger mantissa,
			                      BigInteger e1,
			                      BigInteger e2){
			bool negative=(mantissa.Sign<0);
			if(negative)mantissa=-mantissa;
			if(e1.CompareTo(e2)>0){
				while(e1.CompareTo(e2)>0){
					mantissa>>=1;
					e1=e1-BigInteger.One;
				}
			} else {
				while(e1.CompareTo(e2)<0){
					mantissa>>=1;
					e1=e1+BigInteger.One;
				}
			}
			if(negative)mantissa=-mantissa;
			return mantissa;
		}
		
		public BigFloat Add(BigFloat decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					exponent,mantissa+(BigInteger)decfrac.mantissa);
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					decfrac.exponent,newmant+(BigInteger)decfrac.mantissa);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					exponent,mantissa+(BigInteger)newmant);
			}
		}

		public BigFloat Subtract(BigFloat decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					exponent,mantissa-(BigInteger)decfrac.mantissa);
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					decfrac.exponent,newmant-(BigInteger)decfrac.mantissa);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					exponent,mantissa-(BigInteger)newmant);
			}
		}
		
		public BigFloat Multiply(BigFloat decfrac){
			BigInteger newexp=(this.exponent+(BigInteger)decfrac.exponent);
			return new BigFloat(
				newexp,mantissa*(BigInteger)decfrac.mantissa);
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

		public int CompareTo(BigFloat decfrac){
			if(decfrac==null)return 1;
			int s=this.Sign;
			int ds=decfrac.Sign;
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).Sign;
		}
		
		
		private static BigInteger[] DivideWithPrecision(
			BigInteger dividend, // NOTE: Assumes dividend is nonnegative
			int divisor,
			int maxIterations
		){
			BigInteger bigdivisor=(BigInteger)divisor;
			BigInteger scale=BigInteger.Zero;
			while(dividend.CompareTo(bigdivisor)<0){
				scale+=BigInteger.One;
				dividend*=(BigInteger)10;
			}
			while(dividend.CompareTo(bigdivisor*(BigInteger)10)>=0){
				scale-=BigInteger.One;
				bigdivisor*=(BigInteger)10;
			}
			BigInteger bigquotient=BigInteger.Zero;
			BigInteger bigrem=BigInteger.Zero;
			for(int i=0;i<maxIterations;i++){
				bigrem=dividend%(BigInteger)bigdivisor;
				BigInteger newquotient=((BigInteger)dividend)/(BigInteger)bigdivisor;
				bigquotient+=(BigInteger)newquotient;
				if(scale.Sign>=0 && bigrem.IsZero){
					break;
				}
				scale+=BigInteger.One;
				newquotient*=(BigInteger)10;
				dividend*=(BigInteger)10;
			}
			if(!bigrem.IsZero){
				// Round half-up
				BigInteger halfdivisor=bigdivisor;
				halfdivisor>>=1;
				if(bigrem.CompareTo(halfdivisor)>=0){
					bigrem+=BigInteger.One;
				}
			}
			BigInteger negscale=-(BigInteger)scale;
			return new BigInteger[]{
				bigquotient,bigrem,negscale
			};
		}
		
		public static BigFloat FromDecimalFraction(DecimalFraction decfrac){
			BigInteger bigintExp=decfrac.Exponent;
			BigInteger bigintMant=decfrac.Mantissa;
			if(bigintMant.IsZero)
				return new BigFloat(0);
			if(bigintExp.IsZero){
				// Integer
				return new BigFloat(bigintMant);
			} else if(bigintExp.Sign>0){
				// Scaled integer
				BigInteger curexp=bigintExp;
				BigInteger bigmantissa=bigintMant;
				while(curexp.Sign>0){
					bigmantissa*=(BigInteger)10;
					curexp-=BigInteger.One;
				}
				return new BigFloat(bigmantissa);
			} else {
				// Fractional number
				BigInteger curexp=bigintExp;
				BigInteger scale=curexp;
				BigInteger bigmantissa=bigintMant;
				bool neg=(bigmantissa.Sign<0);
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				while(curexp<0){
					if(curexp<=-10){						
						BigInteger[] results=DivideWithPrecision(bigmantissa,9765625,20);
						bigmantissa=results[0]; // quotient
						BigInteger newscale=results[2];
						scale=scale+(BigInteger)newscale;
						curexp+=(BigInteger)10;
					} else {
						BigInteger[] results=DivideWithPrecision(bigmantissa,5,20);
						bigmantissa=results[0]; // quotient
						BigInteger newscale=results[2];
						scale=scale+(BigInteger)newscale;
						curexp+=BigInteger.One;
					}
				}
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new BigFloat(scale,bigmantissa);
			}
		}
		
		public static BigFloat FromSingle(float flt){
			int value=ConverterInternal.SingleToInt32Bits(flt);
			int fpExponent=(int)((value>>23) & 0xFF);
			if(fpExponent==255)
				throw new OverflowException("Value is infinity or NaN");
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
			return new BigFloat(fpExponent-150,(BigInteger)fpMantissa);
		}

		public static BigFloat FromDouble(double dbl){
			long value=ConverterInternal.DoubleToInt64Bits(dbl);
			int fpExponent=(int)((value>>52) & 0x7ffL);
			if(fpExponent==2047)
				throw new OverflowException("Value is infinity or NaN");
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
			return new BigFloat(fpExponent-1075,(BigInteger)fpMantissa);
		}

		public BigInteger ToBigInteger(){
			if(this.Exponent==0){
				return BigInteger.Zero;
			} else if(this.Exponent>0){
				BigInteger curexp=this.Exponent;
				BigInteger bigmantissa=this.Mantissa;
				bool neg=(bigmantissa.Sign<0);
				if(neg)bigmantissa=-bigmantissa;
				while(curexp>0 && !bigmantissa.IsZero){
					bigmantissa<<=1;
					curexp-=BigInteger.One;
				}
				if(neg)bigmantissa=-bigmantissa;
				return bigmantissa;
			} else {
				BigInteger curexp=this.Exponent;
				BigInteger bigmantissa=this.Mantissa;
				bool neg=(bigmantissa.Sign<0);
				if(neg)bigmantissa=-bigmantissa;
				while(curexp<0 && !bigmantissa.IsZero){
					bigmantissa>>=1;
					curexp+=BigInteger.One;
				}
				if(neg)bigmantissa=-bigmantissa;
				return bigmantissa;
			}
		}
		
		/// <summary>
		/// Converts this value to a 32-bit floating-point number.
		/// The half-up rounding mode is used.
		/// </summary>
		/// <returns>The closest 32-bit floating-point number
		/// to this value.</returns>
		public float ToSingle(){
			BigInteger bigmant=BigInteger.Abs(this.mantissa);
			BigInteger bigexponent=this.exponent;
			int lastRoundedBit=0;
			if(this.mantissa.IsZero){
				return 0.0f;
			}
			while(bigmant.CompareTo((BigInteger)(1L<<23))<0){
				bigmant<<=1;
				bigexponent-=BigInteger.One;
			}
			while(bigmant.CompareTo((BigInteger)(1L<<24))>=0){
				BigInteger bigsticky=bigmant&BigInteger.One;
				lastRoundedBit=(int)bigsticky;
				bigmant>>=1;
				bigexponent+=BigInteger.One;
			}
			if(lastRoundedBit==1){ // Round half-up
				bigmant+=BigInteger.One;
				if(bigmant.CompareTo((BigInteger)(1L<<24))==0){
					bigmant>>=1;
					bigexponent+=BigInteger.One;
				}
			}
			bool subnormal=false;
			if(bigexponent>104){
				// exponent too big
				return (this.mantissa.Sign<0) ?
					Single.NegativeInfinity :
					Single.PositiveInfinity;
			} else if(bigexponent<-149){
				// subnormal
				subnormal=true;
				while(bigexponent<-149){
					BigInteger bigsticky=bigmant&BigInteger.One;
					lastRoundedBit=(int)bigsticky;
					bigmant>>=1;
					bigexponent+=BigInteger.One;
				}
				if(lastRoundedBit==1){ // Round half-up
					bigmant+=BigInteger.One;
					if(bigmant.CompareTo((BigInteger)(1L<<24))==0){
						bigmant>>=1;
						bigexponent+=BigInteger.One;
					}
				}
			}
			if(bigexponent<-149){
				// exponent too small
				return (this.mantissa.Sign<0) ?
					ConverterInternal.Int32BitsToSingle(1<<31) :
					ConverterInternal.Int32BitsToSingle(0);
			} else {
				int smallexponent=(int)bigexponent;
				smallexponent=smallexponent+150;
				bigmant=bigmant&(BigInteger)0x7FFFFFL;
				int smallmantissa=(int)bigmant;
				if(!subnormal){
					smallmantissa|=(smallexponent<<23);
				}
				if(this.mantissa.Sign<0)smallmantissa|=(1<<31);
				return ConverterInternal.Int32BitsToSingle(smallmantissa);
			}
		}

		
		/// <summary>
		/// Converts this value to a 64-bit floating-point number.
		/// The half-up rounding mode is used.
		/// </summary>
		/// <returns>The closest 64-bit floating-point number
		/// to this value.</returns>
		public double ToDouble(){
			BigInteger bigmant=BigInteger.Abs(this.mantissa);
			BigInteger bigexponent=this.exponent;
			int lastRoundedBit=0;
			if(this.mantissa.IsZero){
				return 0.0;
			}
			while(bigmant.CompareTo((BigInteger)(1L<<52))<0){
				bigmant<<=1;
				bigexponent-=BigInteger.One;
			}
			while(bigmant.CompareTo((BigInteger)(1L<<53))>=0){
				BigInteger bigsticky=bigmant&(BigInteger)1;
				lastRoundedBit=(int)bigsticky;
				bigmant>>=1;
				bigexponent+=BigInteger.One;
			}
			if(lastRoundedBit==1){ // Round half-up
				bigmant+=BigInteger.One;
				if(bigmant.CompareTo((BigInteger)(1L<<53))==0){
					bigmant>>=1;
					bigexponent+=BigInteger.One;
				}
			}
			bool subnormal=false;
			if(bigexponent>971){
				// exponent too big
				return (this.mantissa.Sign<0) ?
					Double.NegativeInfinity :
					Double.PositiveInfinity;
			} else if(bigexponent<-1074){
				// subnormal
				subnormal=true;
				while(bigexponent<-1074){
					BigInteger bigsticky=bigmant&(BigInteger)1;
					lastRoundedBit=(int)bigsticky;
					bigmant>>=1;
					bigexponent+=BigInteger.One;
				}
				if(lastRoundedBit==1){ // Round half-up
					bigmant+=BigInteger.One;
					if(bigmant.CompareTo((BigInteger)(1L<<53))==0){
						bigmant>>=1;
						bigexponent+=BigInteger.One;
					}
				}
			}
			if(bigexponent<-1074){
				// exponent too small
				return (this.mantissa.Sign<0) ?
					ConverterInternal.Int64BitsToDouble(1L<<63) :
					ConverterInternal.Int64BitsToDouble(0);
			} else {
				long smallexponent=(long)bigexponent;
				smallexponent=smallexponent+1075;
				bigmant=bigmant&(BigInteger)0xFFFFFFFFFFFFFL;
				long smallmantissa=(long)bigmant;
				if(!subnormal){
					smallmantissa|=(smallexponent<<52);
				}
				if(this.mantissa.Sign<0)smallmantissa|=(1L<<63);
				return ConverterInternal.Int64BitsToDouble(smallmantissa);
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
			return DecimalFraction.FromBigFloat(this).ToString();
		}

		///<summary>
		/// </summary>
		///<returns>
		///
		///</returns>
		///<remarks>
		///The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.</remarks>
		public string ToEngineeringString()
		{
			return DecimalFraction.FromBigFloat(this).ToEngineeringString();
		}

		///<summary>
		/// </summary>
		///<returns>
		///
		///</returns>
		///<remarks>
		///The format of the return value follows the format of the java.math.BigDecimal.toPlainString() method.</remarks>
		public string ToPlainString()
		{
			return DecimalFraction.FromBigFloat(this).ToPlainString();
		}
	}
}