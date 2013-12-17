/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;
//using System.Numerics;
namespace PeterO {
    /// <summary> Represents an arbitrary-precision binary floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number is equal to mantissa
    /// * 2^exponent. </summary>
  public sealed class BigFloat : IComparable<BigFloat>, IEquatable<BigFloat> {
    BigInteger exponent;
    BigInteger mantissa;
    /// <summary> Gets this object's exponent. This object's value will
    /// be an integer if the exponent is positive or zero. </summary>
    public BigInteger Exponent {
      get { return exponent; }
    }
    /// <summary> Gets this object's unscaled value. </summary>
    public BigInteger Mantissa {
      get { return mantissa; }
    }
    #region Equals and GetHashCode implementation
    /// <summary> Determines whether this object's mantissa and exponent
    /// are equal to those of another object. </summary>
    /// <returns/><param name='other'> A BigFloat object.</param>
    private bool EqualsInternal(BigFloat other) {
      BigFloat otherValue = other as BigFloat;
      if (otherValue == null)
        return false;
      return this.exponent.Equals(otherValue.exponent) &&
        this.mantissa.Equals(otherValue.mantissa);
    }
    /// <summary> </summary>
    /// <param name='other'>A BigFloat object.</param>
    /// <returns></returns>
public bool Equals(BigFloat other) {
      return EqualsInternal(other);
    }
    /// <summary> Determines whether this object's mantissa and exponent
    /// are equal to those of another object and that object is a Bigfloat.
    /// </summary>
    /// <returns> True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'> A Object object.</param>
    public override bool Equals(object obj) {
      return EqualsInternal(obj as BigFloat);
    }
    /// <summary> Calculates this object's hash code. </summary>
    /// <returns> This object's hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * exponent.GetHashCode();
        hashCode += 1000000009 * mantissa.GetHashCode();
      }
      return hashCode;
    }
    #endregion
    /// <summary> Creates a bigfloat with the value exponent*2^mantissa.
    /// </summary>
    /// <param name='mantissa'> The unscaled value.</param>
    /// <param name='exponent'> The binary exponent.</param>
    public BigFloat(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }
    
    private static BigInteger BigShiftIteration = (BigInteger)1000000;
    private static int ShiftIteration = 1000000;
    private static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      while (bigShift.CompareTo(BigShiftIteration) > 0) {
        val <<= 1000000;
        bigShift -= (BigInteger)BigShiftIteration;
      }
      int lastshift = (int)bigShift;
      val <<= lastshift;
      return val;
    }
    private static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      while (shift > ShiftIteration) {
        val <<= 1000000;
        shift -= ShiftIteration;
      }
      int lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }
    
    public static BigFloat FromBigInteger(BigInteger bigint){
      return new BigFloat(bigint,BigInteger.Zero);
    }
    
    public static BigFloat FromInt64(long numberValue){
      BigInteger bigint=(BigInteger)numberValue;
      return new BigFloat(bigint,BigInteger.Zero);
    }
    
    /// <summary> Creates a bigfloat from an arbitrary-precision decimal
    /// fraction. Note that if the bigfloat contains a negative exponent,
    /// the resulting value might not be exact. </summary>
    /// <returns></returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
    public static BigFloat FromDecimalFraction(DecimalFraction decfrac) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      BigInteger bigintExp = decfrac.Exponent;
      BigInteger bigintMant = decfrac.Mantissa;
      if (bigintMant.IsZero)
        return BigFloat.Zero;
      if (bigintExp.IsZero) {
        // Integer
        return FromBigInteger(bigintMant);
      } else if (bigintExp.Sign > 0) {
        // Scaled integer
        BigInteger bigmantissa = bigintMant;
        bigmantissa *= (BigInteger)(DecimalFraction.FindPowerOfTenFromBig(bigintExp));
        return FromBigInteger(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = (bigmantissa.Sign < 0);
        BigInteger remainder;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        FastInteger negscale = FastInteger.Copy(scale).Negate();
        BigInteger divisor = DecimalFraction.FindPowerOfFiveFromBig(
          negscale.AsBigInteger());
        while (true) {
          BigInteger quotient = BigInteger.DivRem(bigmantissa, divisor, out remainder);
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (!remainder.IsZero &&
              quotient.CompareTo(OneShift62) < 0) {
            // At this point, the quotient has 62 or fewer bits
            int[] bits=FastInteger.GetLastWords(quotient,2);
            int shift=0;
            if((bits[0]|bits[1])!=0){
              // Quotient's integer part is nonzero.
              // Get the number of bits of the quotient
              int bitPrecision=DecimalFraction.BitPrecisionInt(bits[1]);
              if(bitPrecision!=0)
                bitPrecision+=32;
              else
                bitPrecision=DecimalFraction.BitPrecisionInt(bits[0]);
              shift=63-bitPrecision;
              scale.SubtractInt(shift);
            } else {
              // Integer part of quotient is 0
              shift=1;
              scale.SubtractInt(shift);
            }
            // shift by that many bits, but not less than 1
            bigmantissa<<=shift;
          } else {
            bigmantissa = quotient;
            break;
          }
        }
        // Round half-even
        BigInteger halfDivisor = divisor;
        halfDivisor >>= 1;
        int cmp = remainder.CompareTo(halfDivisor);
        // No need to check for exactly half since all powers
        // of five are odd
        if (cmp > 0) {
          // Greater than half
          bigmantissa += BigInteger.One;
        }
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new BigFloat(bigmantissa, scale.AsBigInteger());
      }
    }
    /// <summary> Creates a bigfloat from a 32-bit floating-point number.
    /// </summary>
    /// <param name='flt'> A 32-bit floating-point number.</param>
    /// <returns> A bigfloat with the same value as "flt".</returns>
    /// <exception cref='OverflowException'> "flt" is infinity or not-a-number.</exception>
    public static BigFloat FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
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
      return new BigFloat((BigInteger)((long)fpMantissa), 
                          (BigInteger)(fpExponent - 150));
    }
    /// <summary> Creates a bigfloat from a 64-bit floating-point number.
    /// </summary>
    /// <param name='dbl'> A 64-bit floating-point number.</param>
    /// <returns> A bigfloat with the same value as "dbl"</returns>
    /// <exception cref='OverflowException'> "dbl" is infinity or not-a-number.</exception>
    public static BigFloat FromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      int fpExponent = (int)((value[1] >> 20) & 0x7ff);
      bool neg=(value[1]>>31)!=0;
      if (fpExponent == 2047)
        throw new OverflowException("Value is infinity or NaN");
      value[1]&=0xFFFFF; // Mask out the exponent and sign
      if (fpExponent == 0) fpExponent++;
      else value[1]|=0x100000;
      if ((value[1]|value[0]) != 0) {
        fpExponent+=DecimalFraction.ShiftAwayTrailingZerosTwoElements(value);
      }
      BigFloat ret=new BigFloat(FastInteger.WordsToBigInteger(value),
                                (BigInteger)(fpExponent - 1075));
      if(neg)ret=ret.Negate();
      return ret;
    }
    /// <summary> Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer. </summary>
    /// <returns></returns>
    public BigInteger ToBigInteger() {
      int expsign=this.Exponent.Sign;
      if (expsign==0) {
        // Integer
        return this.Mantissa;
      } else if (expsign > 0) {
        // Integer with trailing zeros
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero)
          return bigmantissa;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -bigmantissa;
        while (curexp.Sign > 0 && !bigmantissa.IsZero) {
          int shift = 4096;
          if (curexp.CompareTo((BigInteger)shift) < 0) {
            shift = (int)curexp;
          }
          bigmantissa <<= shift;
          curexp -= (BigInteger)shift;
        }
        if (neg) bigmantissa = -bigmantissa;
        return bigmantissa;
      } else {
        // Has fractional parts,
        // shift right without rounding
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero)
          return bigmantissa;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -bigmantissa;
        while (curexp.Sign < 0 && !bigmantissa.IsZero) {
          int shift = 4096;
          if (curexp.CompareTo((BigInteger)(-4096)) > 0) {
            shift = -((int)curexp);
          }
          bigmantissa >>= shift;
          curexp += (BigInteger)shift;
        }
        if (neg) bigmantissa = -bigmantissa;
        return bigmantissa;
      }
    }

    private static BigInteger OneShift23 = BigInteger.One << 23;
    private static BigInteger OneShift52 = BigInteger.One << 52;
    private static BigInteger OneShift62 = BigInteger.One << 62;

    /// <summary> Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used. </summary>
    /// <returns> The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      BigInteger bigmant = BigInteger.Abs(this.mantissa);
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.IsZero) {
        return 0.0f;
      }
      int smallmant = 0;
      FastInteger fastSmallMant;
      if (bigmant.CompareTo(OneShift23) < 0) {
        smallmant = (int)bigmant;
        int exponentchange = 0;
        while (smallmant < (1 << 23)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.SubtractInt(exponentchange);
        fastSmallMant=new FastInteger(smallmant);
      } else {
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant,0,0);
        accum.ShiftToDigitsInt(24);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        fastSmallMant = accum.ShiftedIntFast;
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              !fastSmallMant.IsEvenNumber)) {
        fastSmallMant.AddInt(1);
        if (fastSmallMant.CompareToInt(1 << 24)==0) {
          fastSmallMant=new FastInteger(1<<23);
          bigexponent.AddInt(1);
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareToInt(104) > 0) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Single.NegativeInfinity :
          Single.PositiveInfinity;
      } else if (bigexponent.CompareToInt(-149) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum = BitShiftAccumulator.FromInt32(fastSmallMant.AsInt32());
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        fastSmallMant = accum.ShiftedIntFast;
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                                !fastSmallMant.IsEvenNumber)) {
          fastSmallMant.AddInt(1);
          if (fastSmallMant.CompareToInt(1 << 24)==0) {
            fastSmallMant=new FastInteger(1<<23);
            bigexponent.AddInt(1);
          }
        }
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0) :
          BitConverter.ToSingle(BitConverter.GetBytes((int)0), 0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent = smallexponent + 150;
        int smallmantissa = ((int)fastSmallMant.AsInt32()) & 0x7FFFFF;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 23);
        }
        if (this.mantissa.Sign < 0) smallmantissa |= (1 << 31);
        return BitConverter.ToSingle(BitConverter.GetBytes((int)smallmantissa), 0);
      }
    }
    
    

    /// <summary> Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used. </summary>
    /// <returns> The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      BigInteger bigmant = BigInteger.Abs(this.mantissa);
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.IsZero) {
        return 0.0d;
      }
      int[] mantissaBits;
      if (bigmant.CompareTo(OneShift52) < 0) {
        mantissaBits=FastInteger.GetLastWords(bigmant,2);
        // This will be an infinite loop if both elements
        // of the bits array are 0, but the check for
        // 0 was already done above
        while (!DecimalFraction.HasBitSet(mantissaBits,52)) {
          DecimalFraction.ShiftLeftOne(mantissaBits);
          bigexponent.SubtractInt(1);
        }
      } else {
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant,0,0);
        accum.ShiftToDigitsInt(53);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits=FastInteger.GetLastWords(accum.ShiftedInt,2);
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              !DecimalFraction.HasBitSet(mantissaBits,0))) {
        // Add 1 to the bits
        mantissaBits[0]=unchecked((int)(mantissaBits[0]+1));
        if(mantissaBits[0]==0)
          mantissaBits[1]=unchecked((int)(mantissaBits[1]+1));
        if (mantissaBits[0]==0 &&
            mantissaBits[1]==(1<<21)) { // if mantissa is now 2^53
          mantissaBits[1]>>=1; // change it to 2^52
          bigexponent.AddInt(1);
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareToInt(971) > 0) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Double.NegativeInfinity :
          Double.PositiveInfinity;
      } else if (bigexponent.CompareToInt(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum = new BitShiftAccumulator(
          FastInteger.WordsToBigInteger(mantissaBits),0,0);
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits=FastInteger.GetLastWords(accum.ShiftedInt,2);
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                                !DecimalFraction.HasBitSet(mantissaBits,0))) {
          // Add 1 to the bits
          mantissaBits[0]=unchecked((int)(mantissaBits[0]+1));
          if(mantissaBits[0]==0)
            mantissaBits[1]=unchecked((int)(mantissaBits[1]+1));
          if (mantissaBits[0]==0 &&
              mantissaBits[1]==(1<<21)) { // if mantissa is now 2^53
            mantissaBits[1]>>=1; // change it to 2^52
            bigexponent.AddInt(1);
          }
        }
      }
      if (bigexponent.CompareToInt(-1074) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          Extras.IntegersToDouble(new int[]{0,unchecked((int)0x80000000)}) :
          0.0d;
      } else {
        bigexponent.AddInt(1075);
        // Clear the high bits where the exponent and sign are
        mantissaBits[1]&=0xFFFFF;
        if (!subnormal) {
          int smallexponent=bigexponent.AsInt32()<<20;
          mantissaBits[1]|=smallexponent;
        }
        if (this.mantissa.Sign < 0){
          mantissaBits[1]|= unchecked((int)(1 << 31));
        }
        return Extras.IntegersToDouble(mantissaBits);
      }
    }
    /// <summary> Converts this value to a string.The format of the return
    /// value is exactly the same as that of the java.math.BigDecimal.toString()
    /// method. </summary>
    /// <returns> A string representation of this object.</returns>
    public override string ToString() {
      return DecimalFraction.FromBigFloat(this).ToString();
    }
    /// <summary> Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows the
    /// format of the java.math.BigDecimal.toEngineeringString() method.
    /// </summary>
    /// <returns></returns>
    public string ToEngineeringString() {
      return DecimalFraction.FromBigFloat(this).ToEngineeringString();
    }
    /// <summary> Converts this value to a string, but without an exponent
    /// part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method. </summary>
    /// <returns></returns>
    public string ToPlainString() {
      return DecimalFraction.FromBigFloat(this).ToPlainString();
    }

    private sealed class BinaryMathHelper : IRadixMathHelper<BigFloat> {

    /// <summary> </summary>
    /// <returns></returns>
      public int GetRadix() {
        return 2;
      }

    /// <summary> </summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns></returns>
      public int GetSign(BigFloat value) {
        return value.Sign;
      }

    /// <summary> </summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns></returns>
      public BigInteger GetMantissa(BigFloat value) {
        return value.mantissa;
      }

    /// <summary> </summary>
    /// <param name='value'>A BigFloat object.</param>
    /// <returns></returns>
      public BigInteger GetExponent(BigFloat value) {
        return value.exponent;
      }

    /// <summary> </summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='e1'>A BigInteger object.</param>
    /// <param name='e2'>A BigInteger object.</param>
    /// <returns></returns>
      public BigInteger RescaleByExponentDiff(BigInteger mantissa, BigInteger e1, BigInteger e2) {
        bool negative = (mantissa.Sign < 0);
        if (negative) mantissa = -mantissa;
        BigInteger diff = BigInteger.Abs(e1 - (BigInteger)e2);
        mantissa = ShiftLeft(mantissa, diff);
        if (negative) mantissa = -mantissa;
        return mantissa;
      }

    /// <summary> </summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <returns></returns>
      public BigFloat CreateNew(BigInteger mantissa, BigInteger exponent) {
        return new BigFloat(mantissa, exponent);
      }

    /// <summary> </summary>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer.</param>
    /// <returns></returns>
    /// <param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary> </summary>
    /// <returns></returns>
    /// <param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint,0,0);
      }

    /// <summary> </summary>
    /// <param name='num'>A BigInteger object.</param>
    /// <param name='den'>A BigInteger object.</param>
    /// <returns></returns>
      public bool HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(num, den);
        if (gcd.IsZero) return false;
        den /= gcd;
        while (den.IsEven) {
          den >>= 1;
        }
        return den.Equals(BigInteger.One);
      }

    /// <summary> </summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns></returns>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.Sign <= 0) return bigint;
        if (power.CanFitInInt32()) {
          return ShiftLeftInt(bigint, power.AsInt32());
        } else {
          return ShiftLeft(bigint, power.AsBigInteger());
        }
      }
    }

    /// <summary> Gets this value's sign: -1 if negative; 1 if positive; 0
    /// if zero. </summary>
    public int Sign {
      get {
        return mantissa.Sign;
      }
    }
    /// <summary> Gets whether this object's value equals 0. </summary>
    public bool IsZero {
      get {
        return mantissa.IsZero;
      }
    }
    /// <summary> Gets the absolute value of this object. </summary>
    /// <returns></returns>
    public BigFloat Abs() {
      if (this.Sign < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /// <summary> Gets an object with the same value as this one, but with the
    /// sign reversed. </summary>
    /// <returns></returns>
    public BigFloat Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new BigFloat(neg, this.exponent);
    }

    /// <summary> Divides this object by another bigfloat and returns the
    /// exact result. </summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result would have
    /// a nonterminating decimal expansion.</exception>
    public BigFloat Divide(BigFloat divisor) {
      return Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /// <summary> Divides this object by another bigfloat and returns a result
    /// with the same exponent as this object (the dividend). </summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToSameExponent(BigFloat divisor, Rounding rounding) {
      return DivideToExponent(divisor, this.exponent,  PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two BigFloat objects, and returns the integer
    /// part of the result, rounded down, with the preferred exponent set
    /// to this value's exponent minus the divisor's exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor
     ) {
      return DivideToIntegerNaturalScale(divisor, PrecisionContext.ForRounding(Rounding.Down));
    }

    /// <summary> </summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <returns></returns>
    public BigFloat RemainderNaturalScale(
      BigFloat divisor
     ) {
      return RemainderNaturalScale(divisor,null);
    }

    /// <summary> </summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <returns></returns>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public BigFloat RemainderNaturalScale(
      BigFloat divisor,
      PrecisionContext ctx
     ) {
      return Subtract(this.DivideToIntegerNaturalScale(divisor,null)
                      .Multiply(divisor,null),ctx);
    }
    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode. The precision and exponent range settings of this context are
    /// ignored. If HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx
     ) {
      return DivideToExponent(divisor, ((BigInteger)desiredExponentSmall), ctx);
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to fit the desired exponent.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      long desiredExponentSmall,
      Rounding rounding
     ) {
      return DivideToExponent(divisor, ((BigInteger)desiredExponentSmall),
                    PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two BigFloat objects, and gives a particular exponent
    /// to the result.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to fit the desired exponent.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public BigFloat DivideToExponent(
      BigFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding
     ) {
      return DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary> </summary>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat Abs(PrecisionContext context) {
      return Abs().RoundToPrecision(context);
    }

    /// <summary> </summary>
    /// <param name='context'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat Negate(PrecisionContext context) {
      return Negate().RoundToPrecision(context);
    }

    /// <summary> </summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <returns></returns>
    public BigFloat Add(BigFloat decfrac) {
      return Add(decfrac, PrecisionContext.Unlimited);
    }

    /// <summary>Subtracts a BigFloat object from this instance.</summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <returns>The difference of the two objects.</returns>
    public BigFloat Subtract(BigFloat decfrac) {
      return Subtract(decfrac,null);
    }

    /// <summary>Subtracts a BigFloat object from another BigFloat object.</summary>
    /// <param name='decfrac'>A BigFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The difference of the two objects.</returns>
    public BigFloat Subtract(BigFloat decfrac, PrecisionContext ctx) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      return Add(decfrac.Negate(), ctx);
    }
    /// <summary> Multiplies two bigfloats. The resulting scale will be
    /// the sum of the scales of the two bigfloats. </summary>
    /// <param name='decfrac'> Another bigfloat.</param>
    /// <returns> The product of the two bigfloats. If a precision context
    /// is given, returns null if the result of rounding would cause an overflow.</returns>
    public BigFloat Multiply(BigFloat decfrac) {
      return Multiply(decfrac, PrecisionContext.Unlimited);
    }

    /// <summary> Multiplies by one bigfloat, and then adds another bigfloat.
    /// </summary>
    /// <param name='multiplicand'> The value to multiply.</param>
    /// <param name='augend'> The value to add.</param>
    /// <returns> The result this * multiplicand + augend.</returns>
    public BigFloat MultiplyAndAdd(BigFloat multiplicand,
                                   BigFloat augend) {
      return MultiplyAndAdd(multiplicand,augend,null);
    }
    //----------------------------------------------------------------

    private static RadixMath<BigFloat> math = new RadixMath<BigFloat>(
      new BinaryMathHelper());

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value's exponent minus the divisor's exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision,
    /// rounding, and exponent range of the integer part of the result. Flags
    /// will be set on the given context only if the context's HasFlags is true
    /// and the integer part of the result doesn't fit the precision and exponent
    /// range without rounding.</param>
    /// <returns>The integer part of the quotient of the two objects. Returns
    /// null if the return value would overflow the exponent range. A caller
    /// can handle a null return value by treating it as positive infinity
    /// if both operands have the same sign or as negative infinity if both
    /// operands have different signs.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the integer part of the result is not exact.</exception>
    public BigFloat DivideToIntegerNaturalScale(
      BigFloat divisor, PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the exponent set to 0.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// No flags will be set from this operation even if HasFlags of the context
    /// is true. Can be null.</param>
    /// <returns>The integer part of the quotient of the two objects. The
    /// exponent will be set to 0.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result doesn't fit
    /// the given precision.</exception>
    public BigFloat DivideToIntegerZeroScale(
      BigFloat divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two BigFloat
    /// objects.</summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public BigFloat Remainder(
      BigFloat divisor, PrecisionContext ctx) {
      return math.Remainder(this, divisor, ctx);
    }

    /// <summary> </summary>
    /// <param name='divisor'>A BigFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat RemainderNear(
      BigFloat divisor, PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /// <summary>Divides this BigFloat object by another BigFloat object.
    /// The preferred exponent for the result is this object's exponent minus
    /// the divisor's exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The quotient of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>Either ctx is null or ctx's
    /// precision is 0, and the result would have a nonterminating decimal
    /// expansion; or, the rounding mode is Rounding.Unnecessary and the
    /// result is not exact.</exception>
    public BigFloat Divide(
      BigFloat divisor,
      PrecisionContext ctx
     ) {
      return math.Divide(this, divisor, ctx);
    }

    /// <summary>Divides this object by another object, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode. The precision and exponent range settings of this context are
    /// ignored. If HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    /// <param name='exponent'>A BigInteger object.</param>
    public BigFloat DivideToExponent(
      BigFloat divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <summary> Gets the greater value between two bigfloats. </summary>
    /// <returns> The larger value of the two objects.</returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object.</param>
    public static BigFloat Max(
      BigFloat first, BigFloat second) {
      return math.Max(first, second);
    }

    /// <summary> Gets the lesser value between two bigfloats. </summary>
    /// <returns> The smaller value of the two objects.</returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object.</param>
    public static BigFloat Min(
      BigFloat first, BigFloat second) {
      return math.Min(first, second);
    }
    /// <summary> Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.
    /// </summary>
    /// <returns></returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object.</param>
    public static BigFloat MaxMagnitude(
      BigFloat first, BigFloat second) {
      return math.MaxMagnitude(first, second);
    }
    /// <summary> Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.
    /// </summary>
    /// <returns></returns>
    /// <param name='first'>A BigFloat object.</param>
    /// <param name='second'>A BigFloat object.</param>
    public static BigFloat MinMagnitude(
      BigFloat first, BigFloat second) {
      return math.MinMagnitude(first, second);
    }
    /// <summary> Compares the mathematical values of this object and another
    /// object. <para> This method is not consistent with the Equals method
    /// because two different bigfloats with the same mathematical value,
    /// but different exponents, will compare as equal.</para>
    /// </summary>
    /// <returns> Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if "other" is null, or 0 if both values are equal.</returns>
    /// <param name='other'>A BigFloat object to compare with.</param>
    public int CompareTo(
      BigFloat other) {
      return math.CompareTo(this, other);
    }
    /// <summary> Finds the sum of this object and another object. The result's
    /// exponent is set to the lower of the exponents of the two operands. </summary>
    /// <param name='decfrac'> The number to add to.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The sum of thisValue and the other object. Returns null
    /// if the result would overflow the exponent range.</returns>
    public BigFloat Add(
      BigFloat decfrac, PrecisionContext ctx) {
      return math.Add(this, decfrac, ctx);
    }

    /// <summary> Returns a bigfloat with the same value but a new exponent.
    /// </summary>
    /// <param name='desiredExponent'>The desired exponent of the result.</param>
    /// <param name='ctx'> A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A bigfloat with the same value as this object but with the
    /// exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding</exception>
    /// <exception cref='System.ArgumentException'>The exponent is
    /// outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    public BigFloat Quantize(
      BigInteger desiredExponent, PrecisionContext ctx) {
      return Quantize(new BigFloat(BigInteger.One,desiredExponent), ctx);
    }

    /// <summary> Returns a bigfloat with the same value but a new exponent.
    /// </summary>
    /// <param name='ctx'> A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A bigfloat with the same value as this object but with the
    /// exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding</exception>
    /// <exception cref='System.ArgumentException'>The exponent is
    /// outside of the valid range of the precision context, if it defines
    /// an exponent range.</exception>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    public BigFloat Quantize(
      int desiredExponentSmall, PrecisionContext ctx) {
      return Quantize( 
        new BigFloat(BigInteger.One,(BigInteger)desiredExponentSmall), ctx);
    }

    /// <summary> Returns a bigfloat with the same value as this object but
    /// with the same exponent as another bigfloat. </summary>
    /// <param name='otherValue'>A bigfloat containing the desired exponent
    /// of the result. The mantissa is ignored.</param>
    /// <param name='ctx'> A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A bigfloat with the same value as this object but with the
    /// exponent changed.</returns>
    /// <exception cref='ArithmeticException'>An overflow error occurred,
    /// or the result can't fit the given precision without rounding.</exception>
    public BigFloat Quantize(
      BigFloat otherValue, PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }    /// <summary> </summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.Zero, ctx);
    }
    /// <summary> </summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.Zero, ctx);
    }
    /// <summary> Removes trailing zeros from this object's mantissa. For
    /// example, 1.000 becomes 1.</summary>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// value has a very high exponent and the context says to clamp high exponents,
    /// there may still be some trailing zeros in the mantissa. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow. The caller can handle a null return value by treating
    /// it as positive or negative infinity depending on the sign of this object's
    /// value.</returns>
    public BigFloat Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }
    
    
    /// <summary> Gets the largest value that's smaller than the given value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the largest value that's smaller than the given
    /// value. Returns null if the result is negative infinity.</returns>
    /// <exception cref='System.ArgumentException'>"ctx" is null, the
    /// precision is 0, or "ctx" has an unlimited exponent range.</exception>
    public BigFloat NextMinus(
      PrecisionContext ctx
     ){
      return math.NextMinus(this,ctx);
    }

    /// <summary> Gets the smallest value that's greater than the given value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value. Returns null if the result is positive infinity.</returns>
    /// <exception cref='System.ArgumentException'>"ctx" is null, the
    /// precision is 0, or "ctx" has an unlimited exponent range.</exception>
    public BigFloat NextPlus(
      PrecisionContext ctx
     ){
      return math.NextPlus(this,ctx);
    }

    /// <summary> </summary>
    /// <param name='otherValue'>A BigFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    public BigFloat NextToward(
      BigFloat otherValue,
      PrecisionContext ctx
     ){
      return math.NextToward(this,otherValue,ctx);
    }

    /// <summary>Multiplies two BigFloat objects.</summary>
    /// <param name='op'>A BigFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The product of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    public BigFloat Multiply(
      BigFloat op, PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /// <summary> Multiplies by one value, and then adds another value. </summary>
    /// <param name='multiplicand'> The value to multiply.</param>
    /// <param name='augend'> The value to add.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The result thisValue * multiplicand + augend. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow.</returns>
    public BigFloat MultiplyAndAdd(
      BigFloat multiplicand, BigFloat augend, PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, multiplicand, augend, ctx);
    }
    /// <summary> Rounds this object's value to a given precision, using
    /// the given rounding mode and range of exponent. </summary>
    /// <param name='ctx'> A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns> The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if "context"
    /// is null or the precision and exponent range are unlimited. Returns
    /// null if the result of the rounding would cause an overflow. The caller
    /// can handle a null return value by treating it as positive or negative
    /// infinity depending on the sign of this object's value.</returns>
    public BigFloat RoundToPrecision(
      PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

    /// <summary> Rounds this object's value to a given maximum bit length,
    /// using the given rounding mode and range of exponent. </summary>
    /// <param name='ctx'> A context for controlling the precision, rounding
    /// mode, and exponent range. The precision is interpreted as the maximum
    /// bit length of the mantissa. Can be null.</param>
    /// <returns> The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if "context"
    /// is null or the precision and exponent range are unlimited. Returns
    /// null if the result of the rounding would cause an overflow. The caller
    /// can handle a null return value by treating it as positive or negative
    /// infinity depending on the sign of this object's value.</returns>
    public BigFloat RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }
 
    /// <summary> Represents the number 1. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104", 
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigFloat One = new BigFloat(BigInteger.One,BigInteger.Zero);

    /// <summary> Represents the number 0. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104", 
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigFloat Zero = new BigFloat(BigInteger.Zero,BigInteger.Zero);
    /// <summary> Represents the number 10. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104", 
      Justification="BigInteger is immutable")]
    #endif
    public static readonly BigFloat Ten = FromInt64((long)10);

    
  }

}