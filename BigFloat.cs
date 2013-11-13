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

namespace PeterO {
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
  public sealed class BigFloat : IComparable<BigFloat>, IEquatable<BigFloat> {
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
    /// Determines whether this object's mantissa and exponent
    /// are equal to those of another object.
    /// </summary>
    public bool Equals(BigFloat obj) {
      BigFloat other = obj as BigFloat;
      if (other == null)
        return false;
      return this.exponent.Equals(other.exponent) &&
        this.mantissa.Equals(other.mantissa);
    }

    /// <summary>
    /// Determines whether this object's mantissa and exponent
    /// are equal to those of another object and that
    /// object is a Bigfloat.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as BigFloat);
    }

    /// <summary>
    /// Calculates this object's hash code.
    /// </summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
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
    public BigFloat(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a bigfloat with the value exponentLong*2^mantissa.
    /// </summary>
    /// <param name="mantissa">The unscaled value.</param>
    /// <param name="exponentLong">The binary exponent.</param>
    public BigFloat(BigInteger mantissa, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a bigfloat with the given mantissa and an exponent of 0.
    /// </summary>
    /// <param name="mantissa">The desired value of the bigfloat</param>
    public BigFloat(BigInteger mantissa) {
      this.exponent = BigInteger.Zero;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a bigfloat with the given mantissa and an exponent of 0.
    /// </summary>
    /// <param name="mantissaLong">The desired value of the bigfloat</param>
    public BigFloat(long mantissaLong) {
      this.exponent = BigInteger.Zero;
      this.mantissa = (BigInteger)mantissaLong;
    }

    private BigInteger FastMaxExponent = (BigInteger)2000;
    private BigInteger FastMinExponent = (BigInteger)(-2000);

    private BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      bool negative = (mantissa.Sign < 0);
      if (negative) mantissa = -mantissa;
      if (e1.CompareTo(FastMinExponent) >= 0 &&
          e1.CompareTo(FastMaxExponent) <= 0 &&
          e2.CompareTo(FastMinExponent) >= 0 &&
          e2.CompareTo(FastMaxExponent) <= 0) {
        int e1long = (int)(BigInteger)e1;
        int e2long = (int)(BigInteger)e2;
        e1long = Math.Abs(e1long - e2long);
        if (e1long != 0) {
          mantissa <<= e1long;
        }
      } else {
        if (e1.CompareTo(e2) > 0) {
          while (e1.CompareTo(e2) > 0) {
            mantissa <<= 1;
            e1 = e1 - BigInteger.One;
          }
        } else {
          while (e1.CompareTo(e2) < 0) {
            mantissa <<= 1;
            e1 = e1 + BigInteger.One;
          }
        }
      }
      if (negative) mantissa = -mantissa;
      return mantissa;
    }

    /// <summary>
    /// Gets an object with the same value as this one, but
    /// with the sign reversed.
    /// </summary>
    public BigFloat Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new BigFloat(neg, this.exponent);
    }

    /// <summary>
    /// Gets the absolute value of this object.
    /// </summary>
    public BigFloat Abs() {
      if (this.Sign < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /// <summary>
    /// Gets the greater value between two BigFloat values.
    /// </summary>
    public BigFloat Max(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      return a.CompareTo(b) > 0 ? a : b;
    }

    /// <summary>
    /// Gets the lesser value between two BigFloat values.
    /// </summary>
    public BigFloat Min(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      return a.CompareTo(b) > 0 ? b : a;
    }

    /// <summary>
    /// Finds the sum of this object and another bigfloat.
    /// The result's exponent is set to the lower of the exponents
    /// of the two operands.
    /// </summary>
    public BigFloat Add(BigFloat decfrac) {
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      if (expcmp == 0) {
        return new BigFloat(
          mantissa + (BigInteger)decfrac.mantissa, exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant + (BigInteger)decfrac.mantissa, decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant + (BigInteger)this.mantissa, exponent);
      }
    }

    /// <summary>
    /// Finds the difference between this object and another bigfloat.
    /// The result's exponent is set to the lower of the exponents
    /// of the two operands.
    /// </summary>
    public BigFloat Subtract(BigFloat decfrac) {
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      if (expcmp == 0) {
        return new BigFloat(
          this.mantissa - (BigInteger)decfrac.mantissa, exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          newmant - (BigInteger)decfrac.mantissa, decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new BigFloat(
          this.mantissa - (BigInteger)newmant, exponent);
      }
    }


    /// <summary>
    /// Multiplies two bigfloats.  The resulting scale will be the sum
    /// of the scales of the two bigfloats.
    /// </summary>
    /// <param name="decfrac">Another bigfloat.</param>
    /// <returns>The product of the two bigfloats.</returns>
    public BigFloat Multiply(BigFloat decfrac) {
      BigInteger newexp = (this.exponent + (BigInteger)decfrac.exponent);
      return new BigFloat(
        mantissa * (BigInteger)decfrac.mantissa, newexp);
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
    /// Compares the mathematical values of two bigfloats.
    /// <para>This method is not consistent with the Equals method
    /// because two different bigfloats with the same mathematical
    /// value, but different exponents, will compare as equal.</para>
    /// </summary>
    /// <param name="other">Another bigfloat.</param>
    /// <returns>Less than 0 if this value is less than the other
    /// value, or greater than 0 if this value is greater than the other
    /// value or if "other" is null, or 0 if both values are equal.</returns>
    public int CompareTo(BigFloat other) {
      if (other == null) return 1;
      int s = this.Sign;
      int ds = other.Sign;
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.CompareTo((BigInteger)other.exponent);
      if (expcmp == 0) {
        return mantissa.CompareTo((BigInteger)other.mantissa);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, other.exponent);
        return newmant.CompareTo((BigInteger)other.mantissa);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          other.mantissa, exponent, other.exponent);
        return this.mantissa.CompareTo((BigInteger)newmant);
      }
    }
    
    /// <summary>
    /// Creates a bigfloat from an arbitrary-precision decimal fraction.
    /// Note that if the decimal fraction contains a negative exponent,
    /// the resulting value might not be exact.
    /// </summary>
    public static BigFloat FromDecimalFraction(DecimalFraction decfrac) {
      BigInteger bigintExp = decfrac.Exponent;
      BigInteger bigintMant = decfrac.Mantissa;
      if (bigintMant.IsZero)
        return new BigFloat(0);
      if (bigintExp.IsZero) {
        // Integer
        return new BigFloat(bigintMant);
      } else if (bigintExp.Sign > 0) {
        // Scaled integer
        BigInteger bigmantissa = bigintMant;
        bigmantissa*=(BigInteger)(DecimalFraction.FindPowerOfTen(bigintExp));
        return new BigFloat(bigmantissa);
      } else {
        // Fractional number, keep dividing by
        // 5 while the exponent is less than 0
        BigInteger scale = bigintExp;
        BigInteger bigmantissa = bigintMant;
        bool neg = (bigmantissa.Sign < 0);
        // Make positive because DivideWithPrecision assumes the
        // dividend is positive
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        BigInteger negbigintExp = -(BigInteger)bigintExp;
        BigInteger[] results = DecimalFraction.DivideWithPrecision(
          bigmantissa,
          DecimalFraction.FindPowerOfFive(negbigintExp), 21);
        bigmantissa = results[0]; // quotient
        BigInteger newscale = results[2];
        scale = scale + (BigInteger)newscale;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new BigFloat(bigmantissa, scale);
      }
    }

    /// <summary>
    /// Creates a bigfloat from a 32-bit floating-point number.
    /// </summary>
    /// <param name="dbl">A 32-bit floating-point number.</param>
    /// <returns>A bigfloat with the same value as "flt".</returns>
    /// <exception cref="OverflowException">"flt" is infinity or not-a-number.</exception>
    public static BigFloat FromSingle(float flt) {
      int value = ConverterInternal.SingleToInt32Bits(flt);
      int fpExponent = (int)((value >> 23) & 0xFF);
      if (fpExponent == 255)
        throw new OverflowException("Value is infinity or NaN");
      int fpMantissa = value & 0x7FFFFF;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1 << 23);
      if (fpMantissa != 0) {
        while ((fpMantissa & 1) == 0) {
          fpExponent++;
          fpMantissa >>= 1;
        }
        if ((value >> 31) != 0)
          fpMantissa = -fpMantissa;
      }
      return new BigFloat((BigInteger)((long)fpMantissa), fpExponent - 150);
    }

    /// <summary>
    /// Creates a bigfloat from a 64-bit floating-point number.
    /// </summary>
    /// <param name="dbl">A 64-bit floating-point number.</param>
    /// <returns>A bigfloat with the same value as "dbl"</returns>
    /// <exception cref="OverflowException">"dbl" is infinity or not-a-number.</exception>
    public static BigFloat FromDouble(double dbl) {
      long value = ConverterInternal.DoubleToInt64Bits(dbl);
      int fpExponent = (int)((value >> 52) & 0x7ffL);
      if (fpExponent == 2047)
        throw new OverflowException("Value is infinity or NaN");
      long fpMantissa = value & 0xFFFFFFFFFFFFFL;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1L << 52);
      if (fpMantissa != 0) {
        while ((fpMantissa & 1) == 0) {
          fpExponent++;
          fpMantissa >>= 1;
        }
        if ((value >> 63) != 0)
          fpMantissa = -fpMantissa;
      }
      return new BigFloat((BigInteger)((long)fpMantissa), fpExponent - 1075);
    }

    /// <summary>
    /// Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when
    /// converting to a big integer.
    /// </summary>
    public BigInteger ToBigInteger() {
      if (this.Exponent == 0) {
        // Integer
        return this.Mantissa;
      } else if (this.Exponent > 0) {
        // Integer with trailing zeros
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if(bigmantissa.IsZero)
          return bigmantissa;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -bigmantissa;
        while(curexp>0 && !bigmantissa.IsZero){
          int shift=1024;
          if(curexp.CompareTo((BigInteger)shift)<0){
            shift=(int)curexp;
          }
          bigmantissa<<=shift;
          curexp-=(BigInteger)shift;
        }
        if (neg) bigmantissa = -bigmantissa;
        return bigmantissa;
      } else {
        // Has fractional parts,
        // shift right without rounding
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if(bigmantissa.IsZero)
          return bigmantissa;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -bigmantissa;
        while(curexp<0 && !bigmantissa.IsZero){
          int shift=1024;
          if(curexp.CompareTo((BigInteger)(-1024))>0){
            shift=-((int)curexp);
          }
          bigmantissa>>=shift;
          curexp+=(BigInteger)shift;
        }
        if (neg) bigmantissa = -bigmantissa;
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
    public float ToSingle() {
      BigInteger bigmant = BigInteger.Abs(this.mantissa);
      BigInteger bigexponent = this.exponent;
      int lastRoundedBit = 0;
      if (this.mantissa.IsZero) {
        return 0.0f;
      }
      if(bigmant.CompareTo((BigInteger)(1L << 23)) < 0){
        long smallmant=(long)bigmant;
        int exponentchange=0;
        while(smallmant<(1L<<23)){
          smallmant<<=1;
          exponentchange++;
        }
        bigexponent-=(BigInteger)exponentchange;
        bigmant=(BigInteger)smallmant;
      } else {
        // Shift right 20 bits at a time
        while (bigmant.CompareTo((BigInteger)(1L << 43)) >= 0) {
          BigInteger bigsticky = bigmant & (BigInteger)0xFFFFF;
          lastRoundedBit = ((int)bigsticky)>>19;
          bigmant >>= 20;
          bigexponent += (BigInteger)20;
        }
        // Shift right 1 bit at a time
        while (bigmant.CompareTo((BigInteger)(1L << 24)) >= 0) {
          BigInteger bigsticky = bigmant & BigInteger.One;
          lastRoundedBit = (int)bigsticky;
          bigmant >>= 1;
          bigexponent += BigInteger.One;
        }
      }
      if (lastRoundedBit == 1) { // Round half-up
        bigmant += BigInteger.One;
        if (bigmant.CompareTo((BigInteger)(1L << 24)) == 0) {
          bigmant >>= 1;
          bigexponent += BigInteger.One;
        }
      }
      bool subnormal = false;
      if (bigexponent > 104) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Single.NegativeInfinity :
          Single.PositiveInfinity;
      } else if (bigexponent < -149) {
        // subnormal
        subnormal = true;
        // Shift 80 bits at a time while number
        // remains subnormal
        while (bigexponent < -228) {
          if(bigmant.IsZero){
            lastRoundedBit=0;
            bigexponent=(BigInteger)(-149);
            break;
          } else {
            bigmant >>= 79;
            BigInteger bigsticky = bigmant & BigInteger.One;
            lastRoundedBit = (int)bigsticky;
            bigmant >>= 1;
          }
          bigexponent += (BigInteger)80;
        }
        // Shift 20 bits at a time while number
        // remains subnormal
        while (bigexponent < -168) {
          if(bigmant.IsZero){
            lastRoundedBit=0;
            bigexponent=(BigInteger)(-149);
            break;
          } else {
            bigmant >>= 19;
            BigInteger bigsticky = bigmant & BigInteger.One;
            lastRoundedBit = (int)bigsticky;
            bigmant >>= 1;
          }
          bigexponent += (BigInteger)20;
        }
        // Shift 1 bit at a time while number
        // remains subnormal
        while (bigexponent < -149) {
          if(bigmant.IsZero){
            lastRoundedBit=0;
            bigexponent=(BigInteger)(-149);
            break;
          } else {
            BigInteger bigsticky = bigmant & BigInteger.One;
            lastRoundedBit = (int)bigsticky;
            bigmant >>= 1;
          }
          bigexponent += BigInteger.One;
        }
        if (lastRoundedBit == 1) { // Round half-up
          bigmant += BigInteger.One;
          if (bigmant.CompareTo((BigInteger)(1L << 24)) == 0) {
            bigmant >>= 1;
            bigexponent += BigInteger.One;
          }
        }
      }
      if (bigexponent < -149) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          ConverterInternal.Int32BitsToSingle(1 << 31) :
          ConverterInternal.Int32BitsToSingle(0);
      } else {
        int smallexponent = (int)bigexponent;
        smallexponent = smallexponent + 150;
        bigmant = bigmant & (BigInteger)0x7FFFFFL;
        int smallmantissa = (int)bigmant;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 23);
        }
        if (this.mantissa.Sign < 0) smallmantissa |= (1 << 31);
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
    public double ToDouble() {
      BigInteger bigmant = BigInteger.Abs(this.mantissa);
      BigInteger bigexponent = this.exponent;
      int lastRoundedBit = 0;
      if (this.mantissa.IsZero) {
        return 0.0;
      }
      if(bigmant.CompareTo((BigInteger)(1L << 52)) < 0){
        long smallmant=(long)bigmant;
        int exponentchange=0;
        while(smallmant<(1L<<52)){
          smallmant<<=1;
          exponentchange++;
        }
        bigexponent-=(BigInteger)exponentchange;
        bigmant=(BigInteger)smallmant;
      } else {
        BigInteger OneShift72=BigInteger.One<<72;
        // Shift right 20 bits at a time
        while (bigmant.CompareTo(OneShift72) >= 0) {
          bigmant >>= 19;
          BigInteger bigsticky = bigmant & BigInteger.One;
          lastRoundedBit = (int)bigsticky;
          bigmant >>= 1;
          bigexponent += (BigInteger)20;
        }
        // Shift right 1 bit at a time
        while (bigmant.CompareTo((BigInteger)(1L << 53)) >= 0) {
          BigInteger bigsticky = bigmant & BigInteger.One;
          lastRoundedBit = (int)bigsticky;
          bigmant >>= 1;
          bigexponent += BigInteger.One;
        }
      }
      if (lastRoundedBit == 1) { // Round half-up
        bigmant += BigInteger.One;
        if (bigmant.CompareTo((BigInteger)(1L << 53)) == 0) {
          bigmant >>= 1;
          bigexponent += BigInteger.One;
        }
      }
      bool subnormal = false;
      if (bigexponent > 971) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Double.NegativeInfinity :
          Double.PositiveInfinity;
      } else if (bigexponent < -1074) {
        // subnormal
        subnormal = true;
        // Shift 20 bits at a time while number
        // remains subnormal
        while (bigexponent < -1093) {
          if(bigmant.IsZero){
            lastRoundedBit=0;
            bigexponent=(BigInteger)(-1074);
            break;
          } else {
            BigInteger bigsticky = bigmant & (BigInteger)1;
            lastRoundedBit = (int)bigsticky;
            bigmant >>= 20;
          }
          bigexponent += (BigInteger)20;
        }
        // Shift 1 bit at a time while number
        // remains subnormal
        while (bigexponent < -1074) {
          if(bigmant.IsZero){
            lastRoundedBit=0;
            bigexponent=(BigInteger)(-1074);
            break;
          } else {
            BigInteger bigsticky = bigmant & (BigInteger)1;
            lastRoundedBit = (int)bigsticky;
            bigmant >>= 1;
          }
          bigexponent += BigInteger.One;
        }
        if (lastRoundedBit == 1) { // Round half-up
          bigmant += BigInteger.One;
          if (bigmant.CompareTo((BigInteger)(1L << 53)) == 0) {
            bigmant >>= 1;
            bigexponent += BigInteger.One;
          }
        }
      }
      if (bigexponent < -1074) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          ConverterInternal.Int64BitsToDouble(1L << 63) :
          ConverterInternal.Int64BitsToDouble(0);
      } else {
        long smallexponent = (long)bigexponent;
        smallexponent = smallexponent + 1075;
        bigmant = bigmant & (BigInteger)0xFFFFFFFFFFFFFL;
        long smallmantissa = (long)bigmant;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 52);
        }
        if (this.mantissa.Sign < 0) smallmantissa |= (1L << 63);
        return ConverterInternal.Int64BitsToDouble(smallmantissa);
      }
    }

    ///<summary>
    /// Converts this value to a string.
    ///The format of the return value is exactly the same as that of the java.math.BigDecimal.toString() method.
    /// </summary>
    public override string ToString() {
      return DecimalFraction.FromBigFloat(this).ToString();
    }

    ///<summary>
    /// Same as toString(), except that when an exponent is used it will be a multiple of 3.
    /// The format of the return value follows the format of the java.math.BigDecimal.toEngineeringString() method.
    /// </summary>
    public string ToEngineeringString() {
      return DecimalFraction.FromBigFloat(this).ToEngineeringString();
    }

    ///<summary>
    /// Converts this value to a string, but without an exponent part.
    ///The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method.
    /// </summary>
    public string ToPlainString() {
      return DecimalFraction.FromBigFloat(this).ToPlainString();
    }
  }
}