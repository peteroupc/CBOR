
using System;
using System.Text;
using System.Numerics;

namespace PeterO
{
	/// <summary>
	/// Represents an arbitrary-precision decimal floating-point number.
	/// </summary>
	public class DecimalFraction : IComparable<DecimalFraction>
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
		public override bool Equals(object obj)
		{
			DecimalFraction other = obj as DecimalFraction;
			if (other == null)
				return false;
			return this.exponent.Equals(other.exponent) && 
				this.mantissa.Equals(other.mantissa);
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

		
		public DecimalFraction(BigInteger exponent, BigInteger mantissa){
			this.exponent=exponent;
			this.mantissa=mantissa;
		}

		public DecimalFraction(long exponentLong, BigInteger mantissa){
			this.exponent=(BigInteger)exponentLong;
			this.mantissa=mantissa;
		}

		public DecimalFraction(BigInteger mantissa){
			this.exponent=BigInteger.Zero;
			this.mantissa=mantissa;
		}

		public DecimalFraction(long mantissaLong){
			this.exponent=BigInteger.Zero;
			this.mantissa=(BigInteger)mantissaLong;
		}

		public BigInteger
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
		
		public DecimalFraction Multiply(DecimalFraction decfrac){
			BigInteger newexp=(this.exponent+(BigInteger)decfrac.exponent);
			return new DecimalFraction(
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
		///The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.</remarks>
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
		///The format of the return value follows the format of the java.math.BigDecimal.toPlainString() method.</remarks>
		public string ToPlainString()
		{
			return ToStringInternal(2);
		}
	}
}