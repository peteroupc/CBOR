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
	/// <para>
	/// Note:  This class doesn't yet implement certain operations,
	/// notably division, that require results to be rounded.  That's
	/// because I haven't decided yet how to incorporate rounding into
	/// the API, since the results of some divisions can't be represented
	/// exactly in a decimal fraction (for example, 1/3).  Should I include
	/// precision and rounding mode, as is done in Java's Big Decimal class,
	/// or should I also include minimum and maximum exponent in the
	/// rounding parameters, for better support when converting to other
	/// decimal number formats?  Or is there a better approach to supporting
	/// rounding?
	/// </para>
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

		
		///<summary>
		/// Creates a decimal fraction from a string that represents a number. 
		/// <para>
		///The format of the string generally consists of:<list type=''>
		/// <item>
		///An optional '-' or '+' character (if '-', the value is negative.)</item>
		/// <item>
		///One or more digits, with a single optional decimal point after the first digit and before the last digit.</item>
		/// <item>
		///Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.</item>
		/// </list>
		///</para>
		/// <para>The format generally follows the definition in java.math.BigDecimal(),
		/// except that the digits must be ASCII digits ('0' through '9').</para>
		/// </summary>
		///<param name='s'>
		///A string that represents a number.</param>
		public static DecimalFraction FromString(String s){
			if(s==null)
				throw new ArgumentNullException("s");
			if(s.Length==0)
				throw new FormatException();
			int offset=0;
			bool negative=false;
			if(s[0]=='+' || s[0]=='-'){
				negative=(s[0]=='-');
				offset++;
			}
			BigInteger bigint=BigInteger.Zero;
			bool haveDecimalPoint=false;
			bool haveDigits=false;
			bool haveExponent=false;
			BigInteger newScale=BigInteger.Zero;
			int i=offset;
			for(;i<s.Length;i++){
				if(s[i]>='0' && s[i]<='9'){
					bigint*=(BigInteger)10;
					int thisdigit=(int)(s[i]-'0');
					bigint+=(BigInteger)((long)thisdigit);
					haveDigits=true;
					if(haveDecimalPoint){
						newScale-=BigInteger.One;
					}
				} else if(s[i]=='.'){
					if(haveDecimalPoint)
						throw new FormatException();
					haveDecimalPoint=true;
				} else if(s[i]=='E' || s[i]=='e'){
					haveExponent=true;
					i++;
					break;
				} else {
					throw new FormatException();
				}
			}
			if(!haveDigits)
				throw new FormatException();
			if(haveExponent){
				BigInteger exponent=BigInteger.Zero;
				offset=1;
				haveDigits=false;
				if(i==s.Length)throw new FormatException();
				if(s[i]=='+' || s[i]=='-'){
					if(s[i]=='-')offset=-1;
					i++;
				}
				for(;i<s.Length;i++){
					if(s[i]>='0' && s[i]<='9'){
						haveDigits=true;
						exponent*=(BigInteger)10;
						int thisdigit=(int)(s[i]-'0')*offset;
						exponent+=(BigInteger)((long)thisdigit);
					} else {
						throw new FormatException();
					}
				}
				if(!haveDigits)
					throw new FormatException();
				newScale+=(BigInteger)exponent;
			} else if(i!=s.Length){
				throw new FormatException();
			}
			if(negative)
				bigint=-(BigInteger)bigint;
			return new DecimalFraction(newScale,bigint);
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
        /// Gets an object with the same value as this one, but
        /// with the sign reversed.
        /// </summary>
		public DecimalFraction Negate(){
			BigInteger neg=-(BigInteger)this.mantissa;
			return new DecimalFraction(this.exponent,neg);
		}

        /// <summary>
        /// Gets the absolute value of this object.
        /// </summary>
		public DecimalFraction Abs(){
			if(this.Sign<0){
				BigInteger neg=-(BigInteger)this.mantissa;
				return new DecimalFraction(this.exponent,neg);
			} else {
				return this;
			}
		}
        	
        /// <summary>
		/// Gets the greater value between two DecimalFraction values.
		/// </summary>
		public DecimalFraction Max(DecimalFraction a, DecimalFraction b){
			if(a==null)throw new ArgumentNullException("a");
			if(b==null)throw new ArgumentNullException("b");
			return a.CompareTo(b)>0 ? a : b;
		}
		
		/// <summary>
		/// Gets the lesser value between two DecimalFraction values.
		/// </summary>
		public DecimalFraction Min(DecimalFraction a, DecimalFraction b){
			if(a==null)throw new ArgumentNullException("a");
			if(b==null)throw new ArgumentNullException("b");
			return a.CompareTo(b)>0 ? b : a;
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
		/// <para>This method is not consistent with the Equals method.</para>
		/// </summary>
		/// <param name="other">Another decimal fraction.</param>
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
		/// Creates a decimal fraction from a 32-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 32-bit floating-point number.</param>
		/// <returns>A decimal fraction with the same value as "flt".</returns>
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
				while(fpExponent<0){
					if(fpExponent<=-20){
						bigmantissa*=(BigInteger)BigInt5Pow20;
						fpExponent+=20;
					} else if(fpExponent<=-10){
						bigmantissa*=(BigInteger)BigInt5Pow10;
						fpExponent+=10;
					} else if(fpExponent<=-5){
						bigmantissa*=(BigInteger)BigInt5Pow5;
						fpExponent+=5;
					} else {
						bigmantissa*=(BigInteger)BigInt5;
						fpExponent+=1;
					}
				}
				if(neg)bigmantissa=-(BigInteger)bigmantissa;
				return new DecimalFraction(scale,bigmantissa);
			}
		}

		/// <summary>
		/// Creates a decimal fraction from a 64-bit floating-point number.
		/// </summary>
		/// <param name="dbl">A 64-bit floating-point number.</param>
		/// <returns>A decimal fraction with the same value as "dbl"</returns>
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
				while(fpExponent<0){
					if(fpExponent<=-20){
						bigmantissa*=(BigInteger)BigInt5Pow20;
						fpExponent+=20;
					} else if(fpExponent<=-10){
						bigmantissa*=(BigInteger)BigInt5Pow10;
						fpExponent+=10;
					} else if(fpExponent<=-5){
						bigmantissa*=(BigInteger)BigInt5Pow5;
						fpExponent+=5;
					} else {
						bigmantissa*=(BigInteger)BigInt5;
						fpExponent+=1;
					}
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
					} else if(curexp.CompareTo((BigInteger)(-4))<=0){
						bigmantissa*=(BigInteger)625;
						curexp+=(BigInteger)4;
					} else if(curexp.CompareTo((BigInteger)(-3))<=0){
						bigmantissa*=(BigInteger)125;
						curexp+=(BigInteger)3;
					} else if(curexp.CompareTo((BigInteger)(-2))<=0){
						bigmantissa*=(BigInteger)25;
						curexp+=(BigInteger)2;
					} else {
						bigmantissa*=(BigInteger)BigInt5;
						curexp+=BigInteger.One;
					}
				}
				return new DecimalFraction(bigintExp,bigmantissa);
			}
		}
		/*
		internal DecimalFraction MovePointLeft(BigInteger steps){
			if(steps.IsZero)return this;
			return new DecimalFraction(this.Exponent-(BigInteger)steps,
			                           this.Mantissa);
		}
		
		internal DecimalFraction MovePointRight(BigInteger steps){
			if(steps.IsZero)return this;
			return new DecimalFraction(this.Exponent+(BigInteger)steps,
			                           this.Mantissa);
		}

		internal DecimalFraction Rescale(BigInteger scale)
		{
			throw new NotImplementedException();
		}
 
		internal DecimalFraction RoundToIntegralValue(BigInteger scale)
		{
			return Rescale(BigInteger.Zero);
		}
		internal DecimalFraction Normalize()
		{
			if(this.Mantissa.IsZero)
				return new DecimalFraction(0);
			BigInteger mant=this.Mantissa;
			BigInteger exp=this.Exponent;
			bool changed=false;
			while((mant%(BigInteger)10)==0){
				mant/=(BigInteger)10;
				exp+=BigInteger.One;
				changed=true;
			}
			if(!changed)return this;
			return new DecimalFraction(exp,mant);
		}
		*/
		
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