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
	/// Represents an arbitrary-precision decimal floating-point number.
	/// Consists of a integer mantissa and an integer exponent,
	/// both arbitrary-precision.  The value of the number is equal
	/// to mantissa * 10^exponent.
	/// </summary>
	public sealed class DecimalFraction : IComparable<DecimalFraction>, IEquatable<DecimalFraction>
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
		public bool Equals(DecimalFraction obj)
		{
			DecimalFraction other = obj as DecimalFraction;
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
			return Equals(obj as DecimalFraction);
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
		/// Creates a decimal fraction with the value exponent*10^mantissa.
		/// </summary>
		/// <param name="exponent">The decimal exponent.</param>
		/// <param name="mantissa">The unscaled value.</param>
		public DecimalFraction(BigInteger exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a decimal fraction with the value exponentLong*10^mantissa.
		/// </summary>
		/// <param name="exponentLong">The decimal exponent.</param>
		/// <param name="mantissa">The unscaled value.</param>
		public DecimalFraction(long exponentLong, BigInteger mantissa){
			this.exponent=(BigInteger)exponentLong;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a decimal fraction with the given mantissa and an exponent of 0.
		/// </summary>
		/// <param name="mantissa">The desired value of the bigfloat</param>
		public DecimalFraction(BigInteger mantissa){
			this.exponent=BigInteger.Zero;
			this.mantissa=mantissa;
		}

		/// <summary>
		/// Creates a decimal fraction with the given mantissa and an exponent of 0.
		/// </summary>
		/// <param name="mantissaLong">The desired value of the bigfloat</param>
		public DecimalFraction(long mantissaLong){
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
					mantissa*=(BigInteger)10;
					e1=e1-BigInteger.One;
				}
			} else {
				while(e1.CompareTo(e2)<0){
					mantissa*=(BigInteger)10;
					e1=e1+BigInteger.One;
				}
			}
			if(negative)mantissa=-mantissa;
			return mantissa;
		}
		
		/// <summary>
		/// Finds the sum of this object and another decimal fraction.
		/// The result's exponent is set to the lower of the exponents
		/// of the two operands.
		/// </summary>
		public DecimalFraction Add(DecimalFraction decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new DecimalFraction(
					exponent,mantissa+(BigInteger)decfrac.mantissa);
			} else if(expcmp>0){
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

		/// <summary>
		/// Finds the difference between this object and another decimal fraction.
		/// The result's exponent is set to the lower of the exponents
		/// of the two operands.
		/// </summary>
		public DecimalFraction Subtract(DecimalFraction decfrac){
			int expcmp=exponent.CompareTo((BigInteger)decfrac.exponent);
			if(expcmp==0){
				return new DecimalFraction(
					exponent,mantissa-(BigInteger)decfrac.mantissa);
			} else if(expcmp>0){
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
		
		/// <summary>
		/// Multiplies two decimal fractions.  The resulting scale will be the sum
		/// of the scales of the two decimal fractions.
		/// </summary>
		/// <param name="decfrac">Another decimal fraction.</param>
		/// <returns>The product of the two decimal fractions.</returns>
		public DecimalFraction Multiply(DecimalFraction decfrac){
			BigInteger newexp=(this.exponent+(BigInteger)decfrac.exponent);
			return new DecimalFraction(
				newexp,mantissa*(BigInteger)decfrac.mantissa);
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
		/// Compares two decimal fractions.
		/// </summary>
		/// <param name="other">Another decimal fractions.</param>
		/// <returns>Less than 0 if this value is less than the other
		/// value, or greater than 0 if this value is greater than the other
		/// value or if "other" is null, or 0 if both values are equal.</returns>
		public int CompareTo(DecimalFraction decfrac){
			if(decfrac==null)return 1;
			int s=this.Sign;
			int ds=decfrac.Sign;
			if(s!=ds)return (s<ds) ? -1 : 1;
			return decfrac.Subtract(this).Sign;
		}
		
		private bool InsertString(StringBuilder builder, BigInteger index, char c){
			if(index.CompareTo((BigInteger)Int32.MaxValue)>0){
				throw new NotSupportedException();
			}
			int iindex=(int)(BigInteger)index;
			builder.Insert(iindex,c);
			return true;
		}

		private bool InsertString(StringBuilder builder, BigInteger index, char c, BigInteger count){
			if(count.CompareTo((BigInteger)Int32.MaxValue)>0){
				throw new NotSupportedException();
			}
			if(index.CompareTo((BigInteger)Int32.MaxValue)>0){
				throw new NotSupportedException();
			}
			int icount=(int)(BigInteger)count;
			int iindex=(int)(BigInteger)index;
			for(int i=icount-1;i>=0;i--){
				builder.Insert(iindex,c);
			}
			return true;
		}

		private bool AppendString(StringBuilder builder, char c, BigInteger count){
			if(count.CompareTo((BigInteger)Int32.MaxValue)>0){
				throw new NotSupportedException();
			}
			int icount=(int)(BigInteger)count;
			for(int i=icount-1;i>=0;i--){
				builder.Append(c);
			}
			return true;
		}

		private bool InsertString(StringBuilder builder, BigInteger index, string c){
			if(index.CompareTo((BigInteger)Int32.MaxValue)>0){
				throw new NotSupportedException();
			}
			int iindex=(int)(BigInteger)index;
			builder.Insert(iindex,c);
			return true;
		}
		private string ToStringInternal(int mode)
		{
			// Using Java's rules for converting DecimalFraction
			// values to a string
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			builder.Append(this.mantissa.ToString(
				System.Globalization.CultureInfo.InvariantCulture));
			BigInteger adjustedExponent=this.exponent;
			BigInteger scale=-(BigInteger)this.exponent;
			BigInteger sbLength=(BigInteger)(builder.Length);
			BigInteger negaPos=BigInteger.Zero;
			if(builder[0]=='-'){
				sbLength=sbLength-BigInteger.One;
				negaPos=BigInteger.One;
			}
			bool iszero=(this.mantissa.IsZero);
			if(mode==2 && iszero && scale<0){
				// special case for zero in plain
				return builder.ToString();
			}
			adjustedExponent=adjustedExponent+(BigInteger)sbLength;
			adjustedExponent=adjustedExponent-BigInteger.One;
			BigInteger decimalPointAdjust=BigInteger.One;
			BigInteger threshold=(BigInteger)(-6);
			if(mode==1){ // engineering string adjustments
				BigInteger newExponent=adjustedExponent;
				bool adjExponentNegative=(adjustedExponent.Sign<0);
				BigInteger phase=BigInteger.Abs(adjustedExponent)%(BigInteger)3;
				int intphase=(int)(BigInteger)phase;
				if(iszero && (adjustedExponent.CompareTo(threshold)<0 ||
				              scale.Sign<0)){
					if(intphase==1){
						if(adjExponentNegative){
							decimalPointAdjust+=BigInteger.One;
							newExponent+=BigInteger.One;
						} else {
							decimalPointAdjust+=(BigInteger)2;
							newExponent+=(BigInteger)2;
						}
					} else if(intphase==2){
						if(!adjExponentNegative){
							decimalPointAdjust+=BigInteger.One;
							newExponent+=BigInteger.One;
						} else {
							decimalPointAdjust+=(BigInteger)2;
							newExponent+=(BigInteger)2;
						}
					}
					threshold+=BigInteger.One;
				} else {
					if(intphase==1){
						if(!adjExponentNegative){
							decimalPointAdjust+=BigInteger.One;
							newExponent-=BigInteger.One;
						} else {
							decimalPointAdjust+=(BigInteger)2;
							newExponent-=(BigInteger)2;
						}
					} else if(intphase==2){
						if(adjExponentNegative){
							decimalPointAdjust+=BigInteger.One;
							newExponent-=BigInteger.One;
						} else {
							decimalPointAdjust+=(BigInteger)2;
							newExponent-=(BigInteger)2;
						}
					}
				}
				adjustedExponent=newExponent;
			}
			if(mode==2 || ((adjustedExponent.CompareTo(threshold)>=0 &&
			                scale.Sign>=0))){
				BigInteger decimalPoint=-(BigInteger)scale;
				decimalPoint+=(BigInteger)negaPos;
				decimalPoint+=(BigInteger)sbLength;
				if(scale.Sign>0){
					int cmp=decimalPoint.CompareTo(negaPos);
					if(cmp<0){
						InsertString(builder,negaPos,'0',(negaPos-(BigInteger)decimalPoint));
						InsertString(builder,negaPos,"0.");
					} else if(cmp==0){
						InsertString(builder,decimalPoint,"0.");
					} else if(decimalPoint.CompareTo(
						((BigInteger)(builder.Length)+(BigInteger)negaPos))>0){
						InsertString(builder,(sbLength+(BigInteger)negaPos),'.');
						InsertString(builder,(sbLength+(BigInteger)negaPos),'0',
						             (decimalPoint-(BigInteger)(builder.Length)));
					} else {
						InsertString(builder,decimalPoint,'.');
					}
				}
				if(mode==2 && scale.Sign<0){
					BigInteger negscale=-(BigInteger)scale;
					AppendString(builder,'0',negscale);
				}
				return builder.ToString();
			} else {
				if(mode==1 && iszero && decimalPointAdjust.CompareTo(BigInteger.One)>0){
					builder.Append('.');
					AppendString(builder,'0',(decimalPointAdjust-BigInteger.One));
				} else {
					BigInteger tmp=negaPos+(BigInteger)decimalPointAdjust;
					int cmp=tmp.CompareTo((BigInteger)(builder.Length));
					if(cmp>0){
						AppendString(builder,'0',(tmp-(BigInteger)(builder.Length)));
					}
					if(cmp<0){
						InsertString(builder,tmp,'.');
					}
				}
				if(!adjustedExponent.IsZero){
					builder.Append('E');
					builder.Append(adjustedExponent<0 ? '-' : '+');
					BigInteger sbPos=(BigInteger)(builder.Length);
					adjustedExponent=BigInteger.Abs(adjustedExponent);
					while(!adjustedExponent.IsZero){
						BigInteger digit=(adjustedExponent%(BigInteger)10);
						InsertString(builder,sbPos,(char)('0'+(int)digit));
						adjustedExponent/=(BigInteger)10;
					}
				}
				return builder.ToString();
			}
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
					bigmantissa*=(BigInteger)10;
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
					bigmantissa/=(BigInteger)10;
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
			return BigFloat.FromDecimalFraction(this).ToSingle();
		}
		
		/// <summary>
		/// Converts this value to a 64-bit floating-point number.
		/// The half-up rounding mode is used.
		/// </summary>
		/// <returns>The closest 64-bit floating-point number
		/// to this value.</returns>
		public double ToDouble(){
			return BigFloat.FromDecimalFraction(this).ToDouble();
		}

		/// <summary>
		/// Creates a bigfloat from a 32-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 32-bit floating-point number.</param>
		/// <returns>A bigfloat with the same value as "flt".</returns>
		/// <exception cref="OverflowException">"flt" is infinity or not-a-number.</exception>
		public static DecimalFraction FromSingle(float flt){
			int value=ConverterInternal.SingleToInt32Bits(flt);
			int fpExponent=(int)((value>>23) & 0xFF);
			if(fpExponent==255)
				throw new OverflowException("Value is infinity or NaN");
			int fpMantissa=value & 0x7FFFFF;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1<<23);
			if(fpMantissa==0)return new DecimalFraction(0);
			fpExponent-=150;
			while((fpMantissa&1)==0){
				fpExponent++;
				fpMantissa>>=1;
			}
			bool neg=((value>>31)!=0);
			if(fpExponent==0){
				if(neg)fpMantissa=-fpMantissa;
				return new DecimalFraction(fpMantissa);
			} else if(fpExponent>0){
				// Value is an integer
				BigInteger bigmantissa=(BigInteger)fpMantissa;
				bigmantissa<<=fpExponent;
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(bigmantissa);
			} else {
				// Value has a fractional part
				BigInteger bigmantissa=(BigInteger)fpMantissa;
				long scale=fpExponent;
				int exp=-fpExponent;
				for(int i=0;i<exp;i++){
					bigmantissa*=(BigInteger)5;
				}
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(scale,bigmantissa);
			}
		}

		/// <summary>
		/// Creates a bigfloat from a 64-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 64-bit floating-point number.</param>
		/// <returns>A bigfloat with the same value as "dbl"</returns>
		/// <exception cref="OverflowException">"dbl" is infinity or not-a-number.</exception>
		public static DecimalFraction FromDouble(double dbl){
			long value=ConverterInternal.DoubleToInt64Bits(dbl);
			int fpExponent=(int)((value>>52) & 0x7ffL);
			if(fpExponent==2047)
				throw new OverflowException("Value is infinity or NaN");
			long fpMantissa=value & 0xFFFFFFFFFFFFFL;
			if (fpExponent==0)fpExponent++;
			else fpMantissa|=(1L<<52);
			if(fpMantissa==0)return new DecimalFraction(0);
			fpExponent-=1075;
			while((fpMantissa&1)==0){
				fpExponent++;
				fpMantissa>>=1;
			}
			bool neg=((value>>63)!=0);
			if(fpExponent==0){
				if(neg)fpMantissa=-fpMantissa;
				return new DecimalFraction(fpMantissa);
			} else if(fpExponent>0){
				// Value is an integer
				BigInteger bigmantissa=(BigInteger)fpMantissa;
				bigmantissa<<=fpExponent;
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(bigmantissa);
			} else {
				// Value has a fractional part
				BigInteger bigmantissa=(BigInteger)fpMantissa;
				long scale=fpExponent;
				int exp=-fpExponent;
				for(int i=0;i<exp;i++){
					bigmantissa*=(BigInteger)5;
				}
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(scale,bigmantissa);
			}
		}
		
		private static BigInteger BigInt5=(BigInteger)5;
		private static BigInteger BigInt10=(BigInteger)10;
		private static BigInteger BigInt20=(BigInteger)20;
		private static BigInteger BigIntNeg5=(BigInteger)(-5);
		private static BigInteger BigIntNeg10=(BigInteger)(-10);
		private static BigInteger BigIntNeg20=(BigInteger)(-20);
		private static BigInteger BigInt5Pow5=(BigInteger)3125;
		private static BigInteger BigInt5Pow10=(BigInteger)9765625;
		private static BigInteger BigInt5Pow20=(BigInteger)(95367431640625L);
		
		/// <summary>
		/// Creates a decimal fraction from an arbitrary-precision
		/// binary floating-point number.
		/// </summary>
		/// <param name="bigfloat">A bigfloat.</param>
		public static DecimalFraction FromBigFloat(BigFloat bigfloat){
			BigInteger bigintExp=bigfloat.Exponent;
			BigInteger bigintMant=bigfloat.Mantissa;
			if(bigintExp.IsZero){
				// Integer
				return new DecimalFraction(bigintMant);
			} else if(bigintExp>0){
				// Scaled integer
				BigInteger curexp=bigintExp;
				BigInteger bigmantissa=bigintMant;
				bool neg=(bigmantissa.Sign<0);
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				while(curexp>0){
					int shift=64;
					if(curexp.CompareTo((BigInteger)64)<0){
						shift=(int)curexp;
					}
					bigmantissa<<=shift;
					curexp-=(BigInteger)shift;
				}
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(bigmantissa);
			} else {
				// Fractional number
				BigInteger bigmantissa=bigintMant;
				BigInteger curexp=bigintExp;
				while(curexp.Sign<0){
					if(curexp.CompareTo(BigIntNeg20)<=0){
						bigmantissa*=(BigInteger)BigInt5Pow20;
						curexp+=(BigInteger)BigInt20;
					} else if(curexp.CompareTo(BigIntNeg10)<=0){
						bigmantissa*=(BigInteger)BigInt5Pow10;
						curexp+=(BigInteger)BigInt10;
					} else if(curexp.CompareTo(BigIntNeg5)<=0){
						bigmantissa*=(BigInteger)BigInt5Pow5;
						curexp+=(BigInteger)BigInt5;
					} else {
						bigmantissa*=(BigInteger)BigInt5;
						curexp+=BigInteger.One;
					}
				}
				return new DecimalFraction(bigintExp,bigmantissa);
			}
		}
		
		///<summary>
		/// Converts this value to a string.
		///The format of the return value is exactly the same as that of the java.math.BigDecimal.toString() method.
		/// </summary>
		public override string ToString()
		{
			return ToStringInternal(0);
		}


		///<summary>
		/// Same as toString(), except that when an exponent is used it will be a multiple of 3.
		/// The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.
		/// </summary>
		public string ToEngineeringString()
		{
			return ToStringInternal(1);
		}

		///<summary>
		/// Converts this value to a string, but without an exponent part.
		///The format of the return value follows the format of the java.math.BigDecimal.toPlainString() 
		/// method.
		/// </summary>
		public string ToPlainString()
		{
			return ToStringInternal(2);
		}
	}
}