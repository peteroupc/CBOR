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
	/// <para>
	/// Note:  This class doesn't yet implement certain operations,
	/// notably division, that require results to be rounded.  That's
	/// because I haven't decided yet how to incorporate rounding into
	/// the API, since the results of some divisions can't be represented
	/// exactly in a bigfloat (for example, 1/10).  Should I include
	/// precision and rounding mode, as is done in Java's Big Decimal class,
	/// or should I also include minimum and maximum exponent in the
	/// rounding parameters, for better support when converting to other
	/// decimal number formats?  Or is there a better approach to supporting
	/// rounding?
	/// </para>
	/// </summary>
	public sealed class BigFloat : IComparable<BigFloat>, IEquatable<BigFloat>
	{
		BigInteger exponent;
		BigInteger mantissa;
		
		/// <summary>
		/// Gets this object's exponent.  This object's value will be an integer
		/// if the exponent is positive or zero.
		/// </summary>
		public BigInteger Exponent {
			get { return exponent; }
		}
		
		/// <summary>
		/// Gets this object's unscaled value.
		/// </summary>
		public BigInteger Mantissa {
			get { return mantissa; }
		}
		
		#region Equals and GetHashCode implementation
		/// <summary>
		/// Determines whether this object is equal to another object.
		/// </summary>
		public bool Equals(BigFloat obj)
		{
			BigFloat other = obj as BigFloat;
			if (other == null)
				return false;
			return this.exponent.Equals(other.exponent) &&
				this.mantissa.Equals(other.mantissa);
		}
		
		/// <summary>
		/// Determines whether this object is equal to another object.
		/// </summary>
		public override bool Equals(object obj)
		{
			return Equals(obj as BigFloat);
		}

		/// <summary>
		/// Calculates this object's hash code.
		/// </summary>
		/// <returns>This object's hash code.</returns>
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

		/// <summary>
		/// Creates a bigfloat with the value exponent*2^mantissa.
		/// </summary>
		/// <param name="mantissa">The unscaled value.</param>
		/// <param name="exponent">The binary exponent.</param>
		public BigFloat(BigInteger mantissa,BigInteger exponent){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a bigfloat with the value exponentLong*2^mantissa.
		/// </summary>
		/// <param name="mantissa">The unscaled value.</param>
		/// <param name="exponentLong">The binary exponent.</param>
		public BigFloat(BigInteger mantissa, long exponentLong){
			this.exponent=(BigInteger)exponentLong;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a bigfloat with the given mantissa and an exponent of 0.
		/// </summary>
		/// <param name="mantissa">The desired value of the bigfloat</param>
		public BigFloat(BigInteger mantissa){
			this.exponent=BigInteger.Zero;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a bigfloat with the given mantissa and an exponent of 0.
		/// </summary>
		/// <param name="mantissaLong">The desired value of the bigfloat</param>
		public BigFloat(long mantissaLong){
			this.exponent=BigInteger.Zero;
			this.mantissa=(BigInteger)mantissaLong;
		}

		private BigInteger FastMaxExponent=(BigInteger)2000;
		private BigInteger FastMinExponent=(BigInteger)(-2000);
		
		private BigInteger
			RescaleByExponentDiff(BigInteger mantissa,
			                      BigInteger e1,
			                      BigInteger e2){
			bool negative=(mantissa.Sign<0);
			if(negative)mantissa=-mantissa;
			if(e1.CompareTo(FastMinExponent)>=0 &&
			   e1.CompareTo(FastMaxExponent)<=0 &&
			   e2.CompareTo(FastMinExponent)>=0 &&
			   e2.CompareTo(FastMaxExponent)<=0){
				int e1long=(int)(BigInteger)e1;
				int e2long=(int)(BigInteger)e2;
				e1long=Math.Abs(e1long-e2long);
				if(e1long!=0){
					mantissa<<=e1long;
				}
			} else {
				if(e1.CompareTo(e2)>0){
					while(e1.CompareTo(e2)>0){
						mantissa<<=1;
						e1=e1-BigInteger.One;
					}
				} else {
					while(e1.CompareTo(e2)<0){
						mantissa<<=1;
						e1=e1+BigInteger.One;
					}
				}
			}
			if(negative)mantissa=-mantissa;
			return mantissa;
		}
		
		/// <summary>
		/// Gets an object with the same value as this one, but
		/// with the sign reversed.
		/// </summary>
		public BigFloat Negate(){
			BigInteger neg=-(BigInteger)this.mantissa;
			return new BigFloat(neg,this.exponent);
		}

		/// <summary>
		/// Gets the absolute value of this object.
		/// </summary>
		public BigFloat Abs(){
			if(this.Sign<0){
				return Negate();
			} else {
				return this;
			}
		}

		/// <summary>
		/// Gets the greater value between two BigFloat values.
		/// </summary>
		public BigFloat Max(BigFloat a, BigFloat b){
			if(a==null)throw new ArgumentNullException("a");
			if(b==null)throw new ArgumentNullException("b");
			return a.CompareTo(b)>0 ? a : b;
		}
		
		/// <summary>
		/// Gets the lesser value between two BigFloat values.
		/// </summary>
		public BigFloat Min(BigFloat a, BigFloat b){
			if(a==null)throw new ArgumentNullException("a");
			if(b==null)throw new ArgumentNullException("b");
			return a.CompareTo(b)>0 ? b : a;
		}

		/// <summary>
		/// Finds the sum of this object and another bigfloat.
		/// The result's exponent is set to the lower of the exponents
		/// of the two operands.
		/// </summary>
		public BigFloat Add(BigFloat decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					mantissa+(BigInteger)decfrac.mantissa,exponent);
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					newmant+(BigInteger)decfrac.mantissa,decfrac.exponent);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					newmant+(BigInteger)this.mantissa,exponent);
			}
		}

		/// <summary>
		/// Finds the difference between this object and another bigfloat.
		/// The result's exponent is set to the lower of the exponents
		/// of the two operands.
		/// </summary>
		public BigFloat Subtract(BigFloat decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new BigFloat(
					this.mantissa-(BigInteger)decfrac.mantissa,exponent);
			} else if(expcmp>0){
				BigInteger newmant=RescaleByExponentDiff(
					mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					newmant-(BigInteger)decfrac.mantissa,decfrac.exponent);
			} else {
				BigInteger newmant=RescaleByExponentDiff(
					decfrac.mantissa,exponent,decfrac.exponent);
				return new BigFloat(
					this.mantissa-(BigInteger)newmant,exponent);
			}
		}
		
		
		/// <summary>
		/// Multiplies two bigfloats.  The resulting scale will be the sum
		/// of the scales of the two bigfloats.
		/// </summary>
		/// <param name="decfrac">Another bigfloat.</param>
		/// <returns>The product of the two bigfloats.</returns>
		public BigFloat Multiply(BigFloat decfrac){
			BigInteger newexp=(this.exponent+(BigInteger)decfrac.exponent);
			return new BigFloat(
				mantissa*(BigInteger)decfrac.mantissa,newexp);
		}

		/// <summary>
		/// Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.
		/// </summary>
		public int Sign {
			get {
				return mantissa.Sign;
			}
		}
		
		/// <summary>
		/// Gets whether this object's value equals 0.
		/// </summary>
		public bool IsZero {
			get {
				return mantissa.IsZero;
			}
		}

		/// <summary>
		/// Compares two bigfloats.
		/// <para>This method is not consistent with the Equals method.</para>
		/// </summary>
		/// <param name="other">Another bigfloat.</param>
		/// <returns>Less than 0 if this value is less than the other
		/// value, or greater than 0 if this value is greater than the other
		/// value or if "other" is null, or 0 if both values are equal.</returns>
		public int CompareTo(BigFloat other){
			if(other==null)return 1;
			int s=this.Sign;
			int ds=other.Sign;
			if(s!=ds)return (s<ds) ? -1 : 1;
			return other.Subtract(this).Sign;
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
		
		/// <summary>
		/// Creates a bigfloat from an arbitrary-precision decimal fraction.
		/// Note that if the decimal fraction contains a negative exponent,
		/// the resulting value might not be exact.
		/// </summary>
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
				return new BigFloat(bigmantissa,scale);
			}
		}
		
		/// <summary>
		/// Creates a bigfloat from a 32-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 32-bit floating-point number.</param>
		/// <returns>A bigfloat with the same value as "flt".</returns>
		/// <exception cref="OverflowException">"flt" is infinity or not-a-number.</exception>
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
			return new BigFloat((BigInteger)((long)fpMantissa),fpExponent-150);
		}

		/// <summary>
		/// Creates a bigfloat from a 64-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 64-bit floating-point number.</param>
		/// <returns>A bigfloat with the same value as "dbl"</returns>
		/// <exception cref="OverflowException">"dbl" is infinity or not-a-number.</exception>
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
			return new BigFloat((BigInteger)((long)fpMantissa),fpExponent-1075);
		}

		/// <summary>
		/// Converts this value to an arbitrary-precision integer.
		/// Any fractional part in this value will be discarded when
		/// converting to a big integer.
		/// </summary>
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
		/// to this value. The return value can be positive
		/// infinity or negative infinity if this value exceeds the
		/// range of a 32-bit floating point number.</returns>
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
		/// to this value. The return value can be positive
		/// infinity or negative infinity if this value exceeds the
		/// range of a 64-bit floating point number.</returns>
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
		/// Converts this value to a string.
		///The format of the return value is exactly the same as that of the java.math.BigDecimal.toString() method.
		/// </summary>
		public override string ToString()
		{
			return DecimalFraction.FromBigFloat(this).ToString();
		}

		///<summary>
		/// Same as toString(), except that when an exponent is used it will be a multiple of 3.
		/// The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.
		/// </summary>
		public string ToEngineeringString()
		{
			return DecimalFraction.FromBigFloat(this).ToEngineeringString();
		}

		///<summary>
		/// Converts this value to a string, but without an exponent part.
		///The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
		/// method.
		/// </summary>
		public string ToPlainString()
		{
			return DecimalFraction.FromBigFloat(this).ToPlainString();
		}
	}
}