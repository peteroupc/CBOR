/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
using System.Text;

namespace PeterO {
    /// <summary>Represents an arbitrary-precision binary floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number is equal to mantissa
    /// * 2^exponent. This class also supports values for negative zero,
    /// not-a-number (NaN) values, and infinity.<para>Passing a signaling
    /// NaN to any arithmetic operation shown here will signal the flag FlagInvalid
    /// and return a quiet NaN, even if another operand to that operation is
    /// a quiet NaN, unless noted otherwise.</para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here
    /// will return a quiet NaN, unless noted otherwise.</para>
    /// <para>Unless noted otherwise, passing a null ExtendedFloat argument
    /// to any method here will throw an exception.</para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid,
    /// FlagOverflow, or FlagDivideByZero, it will not throw an exception
    /// too, unless the operation's trap is enabled in the precision context
    /// (see PrecisionContext's Traps property).</para>
    /// <para>An ExtendedFloat value can be serialized in one of the following
    /// ways:</para>
    /// <list> <item>By calling the toString() method, which will always
    /// return distinct strings for distinct ExtendedFloat values. However,
    /// not all strings can be converted back to an ExtendedFloat without
    /// loss, especially if the string has a fractional part.</item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative
    /// properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN
    /// methods. The return values combined will uniquely identify a particular
    /// ExtendedFloat value.</item>
    /// </list>
    /// </summary>
  public sealed class ExtendedFloat : IComparable<ExtendedFloat>, IEquatable<ExtendedFloat> {
    private BigInteger exponent;
    private BigInteger unsignedMantissa;
    private int flags;

    /// <summary>Gets this object&apos;s exponent. This object&apos;s
    /// value will be an integer if the exponent is positive or zero.</summary>
    /// <value>This object&apos;s exponent. This object&apos;s value
    /// will be an integer if the exponent is positive or zero.</value>
    public BigInteger Exponent {
      get {
        return this.exponent;
      }
    }

    /// <summary>Gets the absolute value of this object&apos;s un-scaled
    /// value.</summary>
    /// <value>The absolute value of this object&apos;s un-scaled value.</value>
    public BigInteger UnsignedMantissa {
      get {
        return this.unsignedMantissa;
      }
    }

    /// <summary>Gets this object&apos;s un-scaled value.</summary>
    /// <value>This object&apos;s un-scaled value.</value>
    public BigInteger Mantissa {
      get {
        return this.IsNegative ? (-(BigInteger)this.unsignedMantissa) : this.unsignedMantissa;
      }
    }

    #region Equals and GetHashCode implementation
    /// <summary>Determines whether this object&apos;s mantissa and exponent
    /// are equal to those of another object.</summary>
    /// <returns>A Boolean object.</returns>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    public bool EqualsInternal(ExtendedFloat otherValue) {
      if (otherValue == null) {
        return false;
      }
      return this.exponent.Equals(otherValue.exponent) &&
        this.unsignedMantissa.Equals(otherValue.unsignedMantissa) &&
        this.flags == otherValue.flags;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(ExtendedFloat other) {
      return this.EqualsInternal(other);
    }

    /// <summary>Determines whether this object&apos;s mantissa and exponent
    /// are equal to those of another object and that other object is a decimal
    /// fraction.</summary>
    /// <returns>True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public override bool Equals(object obj) {
      return this.EqualsInternal(obj as ExtendedFloat);
    }

    /// <summary>Calculates this object&apos;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode = hashCode + (1000000007 * this.exponent.GetHashCode());
        hashCode = hashCode + (1000000009 * this.unsignedMantissa.GetHashCode());
        hashCode = hashCode + (1000000009 * this.flags);
      }
      return hashCode;
    }
    #endregion

    /// <summary>Creates a number with the value exponent*2^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The binary exponent.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public static ExtendedFloat Create(BigInteger mantissa, BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      ExtendedFloat ex = new ExtendedFloat();
      ex.exponent = exponent;
      int sign = mantissa == null ? 0 : mantissa.Sign;
      ex.unsignedMantissa = sign < 0 ? (-(BigInteger)mantissa) : mantissa;
      ex.flags = (sign < 0) ? BigNumberFlags.FlagNegative : 0;
      return ex;
    }

    private ExtendedFloat() {
    }

    internal static ExtendedFloat CreateWithFlags(
      BigInteger mantissa,
      BigInteger exponent,
      int flags) {
      ExtendedFloat ext = ExtendedFloat.Create(mantissa, exponent);
      ext.flags = flags;
      return ext;
    }

    /// <summary>Creates a binary float from a string that represents a number.
    /// Note that if the string contains a negative exponent, the resulting
    /// value might not be exact. However, the resulting binary float will
    /// contain enough precision to accurately convert it to a 32-bit or 64-bit
    /// floating point number (float or double).<para> The format of the
    /// string generally consists of:<list type=''> <item> An optional
    /// '-' or '+' character (if '-', the value is negative.)</item>
    /// <item> One or more digits, with a single optional decimal point after
    /// the first digit and before the last digit.</item>
    /// <item> Optionally, E+ (positive exponent) or E- (negative exponent)
    /// plus one or more digits specifying the exponent.</item>
    /// </list>
    /// </para>
    /// <para>The string can also be "-INF", "-Infinity", "Infinity", "INF",
    /// quiet NaN ("qNaN") followed by any number of digits, or signaling
    /// NaN ("sNaN") followed by any number of digits, all in any combination
    /// of upper and lower case.</para>
    /// <para> The format generally follows the definition in java.math.BigDecimal(),
    /// except that the digits must be ASCII digits ('0' through '9').</para>
    /// </summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public static ExtendedFloat FromString(String str, PrecisionContext ctx) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return ExtendedDecimal.FromString(str, ctx).ToExtendedFloat();
    }

    public static ExtendedFloat FromString(String str) {
      return FromString(str, null);
    }

    private static BigInteger valueBigShiftIteration = (BigInteger)1000000;
    private static int valueShiftIteration = 1000000;

    private static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      if (val.IsZero) {
        return val;
      }
      while (bigShift.CompareTo(valueBigShiftIteration) > 0) {
        val <<= 1000000;
        bigShift -= (BigInteger)valueBigShiftIteration;
      }
      int lastshift = (int)bigShift;
      val <<= lastshift;
      return val;
    }

    private static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      if (val.IsZero) {
        return val;
      }
      while (shift > valueShiftIteration) {
        val <<= 1000000;
        shift -= valueShiftIteration;
      }
      int lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }

    private sealed class BinaryMathHelper : IRadixMathHelper<ExtendedFloat> {
    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetRadix() {
        return 2;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedFloat object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetSign(ExtendedFloat value) {
        return value.Sign;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedFloat object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetMantissa(ExtendedFloat value) {
        return value.Mantissa;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedFloat object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetExponent(ExtendedFloat value) {
        return value.exponent;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer. (2).</param>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint, 0, 0);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='num'>A BigInteger object.</param>
    /// <param name='den'>A BigInteger object. (2).</param>
    /// <returns>A Boolean object.</returns>
      public bool HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = BigInteger.GreatestCommonDivisor(num, den);
        if (gcd.IsZero) {
          return false;
        }
        den /= gcd;
        while (den.IsEven) {
          den >>= 1;
        }
        return den.Equals(BigInteger.One);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigint'>A BigInteger object. (2).</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.Sign <= 0) {
          return bigint;
        }
        if (bigint.Sign < 0) {
          bigint = -bigint;
          if (power.CanFitInInt32()) {
            bigint = ShiftLeftInt(bigint, power.AsInt32());
            bigint = -bigint;
          } else {
            bigint = ShiftLeft(bigint, power.AsBigInteger());
            bigint = -bigint;
          }
          return bigint;
        } else {
          if (power.CanFitInInt32()) {
            return ShiftLeftInt(bigint, power.AsInt32());
          } else {
            return ShiftLeft(bigint, power.AsBigInteger());
          }
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedFloat object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetFlags(ExtendedFloat value) {
        return value.flags;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>A BigInteger object. (2).</param>
    /// <param name='flags'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedFloat object.</returns>
      public ExtendedFloat CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags) {
        return ExtendedFloat.CreateWithFlags(mantissa, exponent, flags);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteAndNonFinite;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedFloat object.</returns>
      public ExtendedFloat ValueOf(int val) {
        return FromInt64(val);
      }
    }

    /// <summary>Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ToBigInteger() {
      int expsign = this.Exponent.Sign;
      if (expsign == 0) {
        // Integer
        return this.Mantissa;
      } else if (expsign > 0) {
        // Integer with trailing zeros
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero) {
          return bigmantissa;
        }
        bool neg = bigmantissa.Sign < 0;
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        bigmantissa = ShiftLeft(bigmantissa, curexp);
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      } else {
        // Has fractional parts,
        // shift right without rounding
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero) {
          return bigmantissa;
        }
        bool neg = bigmantissa.Sign < 0;
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        while (curexp.Sign < 0 && !bigmantissa.IsZero) {
          int shift = 4096;
          if (curexp.CompareTo((BigInteger)(-4096)) > 0) {
            shift = -((int)curexp);
          }
          bigmantissa >>= shift;
          curexp += (BigInteger)shift;
        }
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      }
    }

    private static BigInteger valueOneShift23 = BigInteger.One << 23;
    private static BigInteger valueOneShift52 = BigInteger.One << 52;

    /// <summary>Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used.<para>If this value is a NaN,
    /// sets the high bit of the 32-bit floating point number's mantissa for
    /// a quiet NaN, and clears it for a signaling NaN. Then the next highest
    /// bit of the mantissa is cleared for a quiet NaN, and set for a signaling
    /// NaN. Then the other bits of the mantissa are set to the lowest bits of
    /// this object's unsigned mantissa. </para>
    /// </summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      if (this.IsPositiveInfinity()) {
        return Single.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return Single.NegativeInfinity;
      }
      if (this.IsNaN()) {
        int nan = 0x7F800001;
        if (this.IsNegative) {
          nan |= unchecked((int)(1 << 31));
        }
        if (this.IsQuietNaN()) {
          // the quiet bit for X86 at least
          nan |= 0x400000;
        } else {
          // not really the signaling bit, but done to keep
          // the mantissa from being zero
          nan |= 0x200000;
        }
        if (!this.UnsignedMantissa.IsZero) {
          // Transfer diagnostic information
          BigInteger bigdata = this.UnsignedMantissa % (BigInteger)0x200000;
          nan |= (int)bigdata;
        }
        return BitConverter.ToSingle(BitConverter.GetBytes(nan), 0);
      }
      if (this.IsNegative && this.IsZero) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0);
      }
      BigInteger bigmant = BigInteger.Abs(this.unsignedMantissa);
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.IsZero) {
        return 0.0f;
      }
      int smallmant = 0;
      FastInteger fastSmallMant;
      if (bigmant.CompareTo(valueOneShift23) < 0) {
        smallmant = (int)bigmant;
        int exponentchange = 0;
        while (smallmant < (1 << 23)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.SubtractInt(exponentchange);
        fastSmallMant = new FastInteger(smallmant);
      } else {
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant, 0, 0);
        accum.ShiftToDigitsInt(24);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        fastSmallMant = accum.ShiftedIntFast;
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              !fastSmallMant.IsEvenNumber)) {
        fastSmallMant.Increment();
        if (fastSmallMant.CompareToInt(1 << 24) == 0) {
          fastSmallMant = new FastInteger(1 << 23);
          bigexponent.Increment();
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareToInt(104) > 0) {
        // exponent too big
        return this.IsNegative ?
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
          fastSmallMant.Increment();
          if (fastSmallMant.CompareToInt(1 << 24) == 0) {
            fastSmallMant = new FastInteger(1 << 23);
            bigexponent.Increment();
          }
        }
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // exponent too small, so return zero
        return this.IsNegative ?
          BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0) :
          BitConverter.ToSingle(BitConverter.GetBytes((int)0), 0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent = smallexponent + 150;
        int smallmantissa = ((int)fastSmallMant.AsInt32()) & 0x7FFFFF;
        if (!subnormal) {
          smallmantissa |= smallexponent << 23;
        }
        if (this.IsNegative) {
          smallmantissa |= 1 << 31;
        }
        return BitConverter.ToSingle(BitConverter.GetBytes((int)smallmantissa), 0);
      }
    }

    /// <summary>Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used.<para>If this value is a NaN,
    /// sets the high bit of the 64-bit floating point number's mantissa for
    /// a quiet NaN, and clears it for a signaling NaN. Then the next highest
    /// bit of the mantissa is cleared for a quiet NaN, and set for a signaling
    /// NaN. Then the other bits of the mantissa are set to the lowest bits of
    /// this object's unsigned mantissa. </para>
    /// </summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      if (this.IsPositiveInfinity()) {
        return Double.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return Double.NegativeInfinity;
      }
      if (this.IsNaN()) {
        int[] nan = new int[] { 1, 0x7FF00000 };
        if (this.IsNegative) {
          nan[1] |= unchecked((int)(1 << 31));
        }
        if (this.IsQuietNaN()) {
          nan[1] |= 0x80000;
        } else {
          // not really the signaling bit, but done to keep
          // the mantissa from being zero
          nan[1] |= 0x40000;
        }
        if (!this.UnsignedMantissa.IsZero) {
          // Copy diagnostic information
          int[] words = FastInteger.GetLastWords(this.UnsignedMantissa, 2);
          nan[0] = words[0];
          nan[1] = words[1] & 0x3FFFF;
        }
        return Extras.IntegersToDouble(nan);
      }
      if (this.IsNegative && this.IsZero) {
        return Extras.IntegersToDouble(new int[] { unchecked((int)(1 << 31)), 0 });
      }
      BigInteger bigmant = BigInteger.Abs(this.unsignedMantissa);
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.IsZero) {
        return 0.0d;
      }
      int[] mantissaBits;
      if (bigmant.CompareTo(valueOneShift52) < 0) {
        mantissaBits = FastInteger.GetLastWords(bigmant, 2);
        // This will be an infinite loop if both elements
        // of the bits array are 0, but the check for
        // 0 was already done above
        while (!DecimalUtility.HasBitSet(mantissaBits, 52)) {
          DecimalUtility.ShiftLeftOne(mantissaBits);
          bigexponent.Decrement();
        }
      } else {
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant, 0, 0);
        accum.ShiftToDigitsInt(53);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits = FastInteger.GetLastWords(accum.ShiftedInt, 2);
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              DecimalUtility.HasBitSet(mantissaBits, 0))) {
        // Add 1 to the bits
        mantissaBits[0] = unchecked((int)(mantissaBits[0] + 1));
        if (mantissaBits[0] == 0) {
          mantissaBits[1] = unchecked((int)(mantissaBits[1] + 1));
        }
        if (mantissaBits[0] == 0 &&
            mantissaBits[1] == (1 << 21)) {  // if mantissa is now 2^53
          mantissaBits[1] >>= 1;  // change it to 2^52
          bigexponent.Increment();
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareToInt(971) > 0) {
        // exponent too big
        return this.IsNegative ?
          Double.NegativeInfinity :
          Double.PositiveInfinity;
      } else if (bigexponent.CompareToInt(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum = new BitShiftAccumulator(
          FastInteger.WordsToBigInteger(mantissaBits),
          0,
          0);
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits = FastInteger.GetLastWords(accum.ShiftedInt, 2);
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                                DecimalUtility.HasBitSet(mantissaBits, 0))) {
          // Add 1 to the bits
          mantissaBits[0] = unchecked((int)(mantissaBits[0] + 1));
          if (mantissaBits[0] == 0) {
            mantissaBits[1] = unchecked((int)(mantissaBits[1] + 1));
          }
          if (mantissaBits[0] == 0 &&
              mantissaBits[1] == (1 << 21)) {  // if mantissa is now 2^53
            mantissaBits[1] >>= 1;  // change it to 2^52
            bigexponent.Increment();
          }
        }
      }
      if (bigexponent.CompareToInt(-1074) < 0) {
        // exponent too small, so return zero
        return this.IsNegative ?
          Extras.IntegersToDouble(new int[] { 0, unchecked((int)0x80000000) }) :
          0.0d;
      } else {
        bigexponent.AddInt(1075);
        // Clear the high bits where the exponent and sign are
        mantissaBits[1] &= 0xFFFFF;
        if (!subnormal) {
          int smallexponent = bigexponent.AsInt32() << 20;
          mantissaBits[1] |= smallexponent;
        }
        if (this.IsNegative) {
          mantissaBits[1] |= unchecked((int)(1 << 31));
        }
        return Extras.IntegersToDouble(mantissaBits);
      }
    }

    /// <summary>Creates a binary float from a 32-bit floating-point number.
    /// This method computes the exact value of the floating point number,
    /// not an approximation, as is often the case by converting the number
    /// to a string.</summary>
    /// <returns>A binary float with the same value as <paramref name='flt'/>.</returns>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    public static ExtendedFloat FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      bool neg = (value >> 31) != 0;
      int floatExponent = (int)((value >> 23) & 0xFF);
      int valueFpMantissa = value & 0x7FFFFF;
      BigInteger bigmant;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1FFFFF;
        bigmant = (BigInteger)valueFpMantissa;
        bigmant -= BigInteger.One;
        if (bigmant.IsZero) {
          return quiet ? NaN : SignalingNaN;
        } else {
          return CreateWithFlags(
            bigmant,
            BigInteger.Zero,
            (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
        }
      }
      if (floatExponent == 0) {
        floatExponent++;
      } else {
        valueFpMantissa |= 1 << 23;
      }
      if (valueFpMantissa == 0) {
        return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      while ((valueFpMantissa & 1) == 0) {
        floatExponent++;
        valueFpMantissa >>= 1;
      }
      if (neg) {
        valueFpMantissa = -valueFpMantissa;
      }
      bigmant = (BigInteger)valueFpMantissa;
      return ExtendedFloat.Create(
        bigmant,
        (BigInteger)(floatExponent - 150));
    }

    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      return ExtendedFloat.Create(bigint, BigInteger.Zero);
    }

    public static ExtendedFloat FromInt64(long valueSmall) {
      BigInteger bigint = (BigInteger)valueSmall;
      return ExtendedFloat.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a binary float from a 64-bit floating-point number.
    /// This method computes the exact value of the floating point number,
    /// not an approximation, as is often the case by converting the number
    /// to a string.</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A binary float with the same value as <paramref name='dbl'/>.</returns>
    public static ExtendedFloat FromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      int floatExponent = (int)((value[1] >> 20) & 0x7ff);
      bool neg = (value[1] >> 31) != 0;
      if (floatExponent == 2047) {
        if ((value[1] & 0xFFFFF) == 0 && value[0] == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (value[1] & 0x80000) != 0;
        value[1] &= 0x3FFFF;
        BigInteger info = FastInteger.WordsToBigInteger(value);
        info -= BigInteger.One;
        if (info.IsZero) {
          return quiet ? NaN : SignalingNaN;
        } else {
          return CreateWithFlags(
            info,
            BigInteger.Zero,
            (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
        }
      }
      value[1] &= 0xFFFFF;  // Mask out the exponent and sign
      if (floatExponent == 0) {
        floatExponent++;
      } else {
        value[1] |= 0x100000;
      }
      if ((value[1] | value[0]) != 0) {
        floatExponent += DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      } else {
        return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      return CreateWithFlags(
        FastInteger.WordsToBigInteger(value),
        (BigInteger)(floatExponent - 1075),
        neg ? BigNumberFlags.FlagNegative : 0);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimal() {
      return ExtendedDecimal.FromExtendedFloat(this);
    }

    /// <summary>Converts this value to a string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return ExtendedDecimal.FromExtendedFloat(this).ToString();
    }

    /// <summary>Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows the
    /// format of the java.math.BigDecimal.toEngineeringString() method.</summary>
    /// <returns>A string object.</returns>
    public string ToEngineeringString() {
      return this.ToExtendedDecimal().ToEngineeringString();
    }

    /// <summary>Converts this value to a string, but without an exponent
    /// part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method.</summary>
    /// <returns>A string object.</returns>
    public string ToPlainString() {
      return this.ToExtendedDecimal().ToPlainString();
    }

    /// <summary>Represents the number 1.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedFloat is immutable")]
    #endif
    public static readonly ExtendedFloat One = ExtendedFloat.Create(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedFloat is immutable")]
    #endif
    public static readonly ExtendedFloat Zero = ExtendedFloat.Create(BigInteger.Zero, BigInteger.Zero);
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedFloat is immutable")]
    #endif
    public static readonly ExtendedFloat NegativeZero = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagNegative);

    /// <summary>Represents the number 10.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedFloat is immutable")]
    #endif

    public static readonly ExtendedFloat Ten = ExtendedFloat.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedFloat NaN = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagQuietNaN);

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it&apos;s passed as an argument to any arithmetic operation
    /// in ExtendedFloat.</summary>
    public static readonly ExtendedFloat SignalingNaN = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagSignalingNaN);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedFloat PositiveInfinity = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedFloat NegativeInfinity = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
        BigNumberFlags.FlagInfinity;
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN() {
      return (this.flags & (BigNumberFlags.FlagQuietNaN | BigNumberFlags.FlagSignalingNaN)) != 0;
    }

    /// <summary>Gets a value indicating whether this object is positive
    /// or negative infinity.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <summary>Gets a value indicating whether this object is negative,
    /// including negative zero.</summary>
    /// <value>Whether this object is negative, including negative zero.</value>
    public bool IsNegative {
      get {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }
    }

    /// <summary>Gets a value indicating whether this object is a quiet not-a-number
    /// value.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <summary>Gets a value indicating whether this object is a signaling
    /// not-a-number value.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <summary>Gets this value&apos;s sign: -1 if negative; 1 if positive;
    /// 0 if zero.</summary>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public int Sign {
      get {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
                this.unsignedMantissa.IsZero) ? 0 :
          (((this.flags & BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
      }
    }

    /// <summary>Gets a value indicating whether this object&apos;s value
    /// equals 0.</summary>
    /// <value>Whether this object&apos;s value equals 0.</value>
    public bool IsZero {
      get {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) && this.unsignedMantissa.IsZero;
      }
    }

    /// <summary>Gets the absolute value of this object.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Abs() {
      return this.Abs(null);
    }

    /// <summary>Gets an object with the same value as this one, but with the
    /// sign reversed.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Negate() {
      return this.Negate(null);
    }

    /// <summary>Divides this object by another binary float and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0.</returns>
    /// <exception cref='ArithmeticException'>The result can't be exact
    /// because it would have a nonterminating binary expansion.</exception>
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      return this.Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /// <summary>Divides this object by another binary float and returns
    /// a result with the same exponent as this object (the dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public ExtendedFloat DivideToSameExponent(ExtendedFloat divisor, Rounding rounding) {
      return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedFloat objects, and returns the integer
    /// part of the result, rounded down, with the preferred exponent set
    /// to this value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects. Signals
    /// FlagDivideByZero and returns infinity if the divisor is 0 and the
    /// dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor
    /// and the dividend are 0.</returns>
    public ExtendedFloat DivideToIntegerNaturalScale(
      ExtendedFloat divisor)
    {
      return this.DivideToIntegerNaturalScale(divisor, PrecisionContext.ForRounding(Rounding.Down));
    }

    /// <summary>Removes trailing zeros from this object&apos;s mantissa.
    /// For example, 1.000 becomes 1.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// result has a very high exponent and the context says to clamp high exponents,
    /// there may still be some trailing zeros in the mantissa.</returns>
    public ExtendedFloat Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>An ExtendedFloat object. (2).</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat RemainderNaturalScale(
      ExtendedFloat divisor)
    {
      return this.RemainderNaturalScale(divisor, null);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>An ExtendedFloat object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat RemainderNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx)
    {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null),
        ctx);
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. The precision setting of this context is ignored. If
    /// HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the context defines
    /// an exponent range and the desired exponent is outside that range.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx)
    {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, ctx);
    }

    /// <summary>Divides this ExtendedFloat object by another ExtendedFloat
    /// object. The preferred exponent for the result is this object&apos;s
    /// exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0.</returns>
    /// <exception cref='ArithmeticException'>Either <paramref name='ctx'/>
    /// is null or <paramref name='ctx'/>'s precision is 0, and the result
    /// would have a nonterminating binary expansion; or, the rounding mode
    /// is Rounding.Unnecessary and the result is not exact.</exception>
    public ExtendedFloat Divide(
      ExtendedFloat divisor,
      PrecisionContext ctx)
    {
      return math.Divide(this, divisor, ctx);
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0.</returns>
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      long desiredExponentSmall,
      Rounding rounding)
    {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A positive
    /// number places the cutoff point to the left of the usual decimal point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. The precision setting of this context is ignored. If
    /// HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the context defines
    /// an exponent range and the desired exponent is outside that range.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <summary>Divides two ExtendedFloat objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding)
    {
      return this.DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Finds the absolute value of this object (if it&apos;s negative,
    /// it becomes positive).</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    public ExtendedFloat Abs(PrecisionContext context) {
      return math.Abs(this, context);
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat Negate(PrecisionContext context) {
      return math.Negate(this, context);
    }

    /// <summary>Adds this object and another binary float and returns the
    /// result.</summary>
    /// <param name='numberObject'>An ExtendedFloat object.</param>
    /// <returns>The sum of the two objects.</returns>
    public ExtendedFloat Add(ExtendedFloat numberObject) {
      return this.Add(numberObject, PrecisionContext.Unlimited);
    }

    /// <summary>Subtracts a ExtendedFloat object from this instance and
    /// returns the result..</summary>
    /// <param name='numberObject'>An ExtendedFloat object.</param>
    /// <returns>The difference of the two objects.</returns>
    public ExtendedFloat Subtract(ExtendedFloat numberObject) {
      return this.Subtract(numberObject, null);
    }

    /// <summary>Subtracts a ExtendedFloat object from this instance.</summary>
    /// <param name='numberObject'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    public ExtendedFloat Subtract(ExtendedFloat numberObject, PrecisionContext ctx) {
      if (numberObject == null) {
        throw new ArgumentNullException("numberObject");
      }
      ExtendedFloat negated = numberObject;
      if ((numberObject.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = numberObject.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(numberObject.unsignedMantissa, numberObject.exponent, newflags);
      }
      return this.Add(negated, ctx);
    }

    /// <summary>Multiplies two binary floats. The resulting exponent
    /// will be the sum of the exponents of the two binary floats.</summary>
    /// <param name='numberObject'>Another binary float.</param>
    /// <returns>The product of the two binary floats.</returns>
    public ExtendedFloat Multiply(ExtendedFloat numberObject) {
      return this.Multiply(numberObject, PrecisionContext.Unlimited);
    }

    /// <summary>Multiplies by one binary float, and then adds another binary
    /// float.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * multiplicand + augend.</returns>
    public ExtendedFloat MultiplyAndAdd(
      ExtendedFloat multiplicand,
      ExtendedFloat augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static IRadixMath<ExtendedFloat> math = new TrappableRadixMath<ExtendedFloat>(
      new RadixMath<ExtendedFloat>(new BinaryMathHelper()));

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision,
    /// rounding, and exponent range of the integer part of the result. Flags
    /// will be set on the given context only if the context&apos;s HasFlags
    /// is true and the integer part of the result doesn&apos;t fit the precision
    /// and exponent range without rounding.</param>
    /// <returns>The integer part of the quotient of the two objects. Returns
    /// null if the return value would overflow the exponent range. A caller
    /// can handle a null return value by treating it as positive infinity
    /// if both operands have the same sign or as negative infinity if both
    /// operands have different signs. Signals FlagDivideByZero and returns
    /// infinity if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid
    /// and returns NaN if the divisor and the dividend are 0.</returns>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the integer part of the result is not exact.</exception>
    public ExtendedFloat DivideToIntegerNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return math.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the exponent set to 0.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// If HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null.</param>
    /// <returns>The integer part of the quotient of the two objects. The
    /// exponent will be set to 0. Signals FlagDivideByZero and returns infinity
    /// if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid
    /// and returns NaN if the divisor and the dividend are 0, or if the result
    /// doesn't fit the given precision.</returns>
    public ExtendedFloat DivideToIntegerZeroScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two ExtendedFloat
    /// objects.</summary>
    /// <param name='divisor'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public ExtendedFloat Remainder(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return math.Remainder(this, divisor, ctx);
    }

    /// <summary>Finds the distance to the closest multiple of the given
    /// divisor, based on the result of dividing this object&apos;s value
    /// by another object&apos;s value.<list type=''> <item> If this and
    /// the other object divide evenly, the result is 0.</item>
    /// <item>If the remainder's absolute value is less than half of the divisor's
    /// absolute value, the result has the same sign as this object and will
    /// be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is more than half of the divisor's
    /// absolute value, the result has the opposite sign of this object and
    /// will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is exactly half of the divisor's
    /// absolute value, the result has the opposite sign of this object if
    /// the quotient, rounded down, is odd, and has the same sign as this object
    /// if the quotient, rounded down, is even, and the result's absolute
    /// value is half of the divisor's absolute value.</item>
    /// </list>
    /// This function is also known as the "IEEE Remainder" function.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored
    /// (the rounding mode is always treated as HalfEven). If HasFlags of
    /// the context is true, will also store the flags resulting from the operation
    /// (the flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The distance of the closest multiple. Signals FlagInvalid
    /// and returns NaN if the divisor is 0, or either the result of integer
    /// division (the quotient) or the remainder wouldn't fit the given precision.</returns>
    public ExtendedFloat RemainderNear(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /// <summary>Finds the largest value that&apos;s smaller than the given
    /// value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. If HasFlags of the context is true, will also store the
    /// flags resulting from the operation (the flags are in addition to the
    /// pre-existing flags).</param>
    /// <returns>Returns the largest value that's less than the given value.
    /// Returns negative infinity if the result is negative infinity.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public ExtendedFloat NextMinus(
      PrecisionContext ctx)
    {
      return math.NextMinus(this, ctx);
    }

    /// <summary>Finds the smallest value that&apos;s greater than the
    /// given value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. If HasFlags of the context is true, will also store the
    /// flags resulting from the operation (the flags are in addition to the
    /// pre-existing flags).</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public ExtendedFloat NextPlus(
      PrecisionContext ctx)
    {
      return math.NextPlus(this, ctx);
    }

    /// <summary>Finds the next value that is closer to the other object&apos;s
    /// value than this object&apos;s value.</summary>
    /// <param name='otherValue'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. If HasFlags of the context is true, will also store the
    /// flags resulting from the operation (the flags are in addition to the
    /// pre-existing flags).</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</exception>
    public ExtendedFloat NextToward(
      ExtendedFloat otherValue,
      PrecisionContext ctx)
    {
      return math.NextToward(this, otherValue, ctx);
    }

    /// <summary>Gets the greater value between two binary floats.</summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (2).</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedFloat Max(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return math.Max(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two binary floats.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (2).</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedFloat Min(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return math.Min(first, second, ctx);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='first'>An ExtendedFloat object. (2).</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedFloat MaxMagnitude(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return math.MaxMagnitude(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='first'>An ExtendedFloat object. (2).</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedFloat MinMagnitude(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return math.MinMagnitude(first, second, ctx);
    }

    /// <summary>Gets the greater value between two binary floats.</summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (2).</param>
    public static ExtendedFloat Max(
      ExtendedFloat first,
      ExtendedFloat second) {
      return Max(first, second, null);
    }

    /// <summary>Gets the lesser value between two binary floats.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='first'>An ExtendedFloat object.</param>
    /// <param name='second'>An ExtendedFloat object. (2).</param>
    public static ExtendedFloat Min(
      ExtendedFloat first,
      ExtendedFloat second) {
      return Min(first, second, null);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='first'>An ExtendedFloat object. (2).</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    public static ExtendedFloat MaxMagnitude(
      ExtendedFloat first,
      ExtendedFloat second) {
      return MaxMagnitude(first, second, null);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='first'>An ExtendedFloat object. (2).</param>
    /// <param name='second'>An ExtendedFloat object. (3).</param>
    public static ExtendedFloat MinMagnitude(
      ExtendedFloat first,
      ExtendedFloat second) {
      return MinMagnitude(first, second, null);
    }

    /// <summary>Compares the mathematical values of this object and another
    /// object, accepting NaN values.<para> This method is not consistent
    /// with the Equals method because two different numbers with the same
    /// mathematical value, but different exponents, will compare as equal.</para>
    /// <para>In this method, negative zero and positive zero are considered
    /// equal.</para>
    /// <para>If this object or the other object is a quiet NaN or signaling
    /// NaN, this method will not trigger an error. Instead, NaN will compare
    /// greater than any other number, including infinity. Two different
    /// NaN values will be considered equal.</para>
    /// </summary>
    /// <returns>Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if <paramref name='other'/> is null, or 0 if both values
    /// are equal.</returns>
    /// <param name='other'>An ExtendedFloat object.</param>
    public int CompareTo(
      ExtendedFloat other) {
      return math.CompareTo(this, other);
    }

    /// <summary>Compares the mathematical values of this object and another
    /// object.<para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or signaling
    /// NaN, this method returns a quiet NaN, and will signal a FlagInvalid
    /// flag if either is a signaling NaN.</para>
    /// </summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding,
    /// and exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0 if
    /// both objects have the same value, or -1 if this object is less than the
    /// other value, or 1 if this object is greater.</returns>
    public ExtendedFloat CompareToWithContext(
      ExtendedFloat other,
      PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, false, ctx);
    }

    /// <summary>Compares the mathematical values of this object and another
    /// object, treating quiet NaN as signaling.<para>In this method, negative
    /// zero and positive zero are considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or signaling
    /// NaN, this method will return a quiet NaN and will signal a FlagInvalid
    /// flag.</para>
    /// </summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding,
    /// and exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0 if
    /// both objects have the same value, or -1 if this object is less than the
    /// other value, or 1 if this object is greater.</returns>
    public ExtendedFloat CompareToSignal(
      ExtendedFloat other,
      PrecisionContext ctx) {
      return math.CompareToWithContext(this, other, true, ctx);
    }

    /// <summary>Finds the sum of this object and another object. The result&apos;s
    /// exponent is set to the lower of the exponents of the two operands.</summary>
    /// <param name='numberObject'>The number to add to.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The sum of thisValue and the other object.</returns>
    public ExtendedFloat Add(
      ExtendedFloat numberObject,
      PrecisionContext ctx) {
      return math.Add(this, numberObject, ctx);
    }

    /// <summary>Returns a binary float with the same value but a new exponent.</summary>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the rounded result can't fit the given precision,
    /// or if the context defines an exponent range and the given exponent
    /// is outside that range.</returns>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedFloat Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(ExtendedFloat.Create(BigInteger.One, desiredExponent), ctx);
    }

    /// <summary>Returns a binary float with the same value but a new exponent.</summary>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the rounded result can't fit the given precision,
    /// or if the context defines an exponent range and the given exponent
    /// is outside that range.</returns>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedFloat Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(ExtendedFloat.Create(BigInteger.One, (BigInteger)desiredExponentSmall), ctx);
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but with the same exponent as another binary float.</summary>
    /// <param name='otherValue'>A binary float containing the desired
    /// exponent of the result. The mantissa is ignored. The exponent is the
    /// number of fractional digits in the result, expressed as a negative
    /// number. Can also be positive, which eliminates lower-order places
    /// from the number. For example, -3 means round to the thousandth (10^-3,
    /// 0.0001), and 3 means round to the thousand (10^3, 1000). A value of
    /// 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A binary float with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</returns>
    public ExtendedFloat Quantize(
      ExtendedFloat otherValue,
      PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but rounded to an integer.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A binary float with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</returns>
    public ExtendedFloat RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but rounded to an integer, without adding the FlagInexact or FlagRounded
    /// flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags), except that this function will never
    /// add the FlagRounded and FlagInexact flags (the only difference between
    /// this and RoundToExponentExact). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A binary float with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</returns>
    public ExtendedFloat RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a binary float with the same value as this object
    /// but rounded to an integer.</summary>
    /// <returns>A binary float with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</returns>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedFloat RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, exponent, ctx);
    }

    /// <summary>Returns a binary float with the same value as this object,
    /// and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result, expressed
    /// as a negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the thousandth
    /// (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
    /// value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null, in which case
    /// the default rounding mode is HalfEven.</param>
    /// <returns>A binary float rounded to the closest value representable
    /// in the given precision, meaning if the result can't fit the precision,
    /// additional digits are discarded to make it fit. Signals FlagInvalid
    /// and returns NaN if the new exponent must be changed when rounding and
    /// the new exponent is outside of the valid range of the precision context,
    /// if it defines an exponent range.</returns>
    public ExtendedFloat RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentSimple(this, exponent, ctx);
    }

    /// <summary>Multiplies two binary floats. The resulting scale will
    /// be the sum of the scales of the two binary floats. The result&apos;s
    /// sign is positive if both operands have the same sign, and negative
    /// if they have different signs.</summary>
    /// <param name='op'>Another binary float.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The product of the two binary floats.</returns>
    public ExtendedFloat Multiply(
      ExtendedFloat op,
      PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /// <summary>Multiplies by one value, and then adds another value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The result thisValue * multiplicand + augend.</returns>
    public ExtendedFloat MultiplyAndAdd(
      ExtendedFloat op,
      ExtendedFloat augend,
      PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
    }

    /// <summary>Multiplies by one value, and then subtracts another value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='subtrahend'>The value to subtract.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The result thisValue * multiplicand - subtrahend.</returns>
    public ExtendedFloat MultiplyAndSubtract(
      ExtendedFloat op,
      ExtendedFloat subtrahend,
      PrecisionContext ctx) {
      if (subtrahend == null) {
        throw new ArgumentNullException("numberObject");
      }
      ExtendedFloat negated = subtrahend;
      if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(subtrahend.unsignedMantissa, subtrahend.exponent, newflags);
      }
      return math.MultiplyAndAdd(this, op, negated, ctx);
    }

    /// <summary>Rounds this object&apos;s value to a given precision,
    /// using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if <paramref
    /// name='ctx'/> is null or the precision and exponent range are unlimited.</returns>
    public ExtendedFloat RoundToPrecision(
      PrecisionContext ctx) {
      return math.RoundToPrecision(this, ctx);
    }

    /// <summary>Rounds this object&apos;s value to a given precision,
    /// using the given rounding mode and range of exponent, and also converts
    /// negative zero to positive zero.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if <paramref
    /// name='ctx'/> is null or the precision and exponent range are unlimited.</returns>
    public ExtendedFloat Plus(
      PrecisionContext ctx) {
      return math.Plus(this, ctx);
    }

    /// <summary>Rounds this object&apos;s value to a given maximum bit
    /// length, using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. The precision is interpreted as the maximum
    /// bit length of the mantissa. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if <paramref
    /// name='ctx'/> is null or the precision and exponent range are unlimited.</returns>
    public ExtendedFloat RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }

    /// <summary>Finds the square root of this object&apos;s value.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as the square root function&apos;s results are generally
    /// not exact for many inputs.--.</param>
    /// <returns>The square root. Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the result would be a complex number
    /// with a real part of 0 and an imaginary part of this object's absolute
    /// value, but the return value is still NaN).</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      return math.SquareRoot(this, ctx);
    }

    /// <summary>Finds e (the base of natural logarithms) raised to the power
    /// of this object&apos;s value.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as the exponential function&apos;s results are generally
    /// not exact.--.</param>
    /// <returns>Exponential of this object. If this object's value is 1,
    /// returns an approximation to " e" within the given precision.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    public ExtendedFloat Exp(PrecisionContext ctx) {
      return math.Exp(this, ctx);
    }

    /// <summary>Finds the natural logarithm of this object, that is, the
    /// exponent that e (the base of natural logarithms) must be raised to
    /// in order to equal this object&apos;s value.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as the ln function&apos;s results are generally not exact.--.</param>
    /// <returns>Ln(this object). Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the result would be a complex number
    /// with a real part equal to Ln of this object's absolute value and an imaginary
    /// part equal to pi, but the return value is still NaN.).</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    public ExtendedFloat Log(PrecisionContext ctx) {
      return math.Ln(this, ctx);
    }

    /// <summary>Finds the base-10 logarithm of this object, that is, the
    /// exponent that the number 10 must be raised to in order to equal this
    /// object&apos;s value.</summary>
    /// <returns>An ExtendedFloat object.</returns>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedFloat Log10(PrecisionContext ctx) {
      return math.Log10(this, ctx);
    }

    /// <summary>Raises this object&apos;s value to the given exponent.</summary>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0; or if this value is less than
    /// 0 and the exponent either has a fractional part or is infinity.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0), and the exponent has a fractional part.</exception>
    /// <param name='exponent'>An ExtendedFloat object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      return math.Power(this, exponent, ctx);
    }

    /// <summary>Finds the constant pi.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as pi can never be represented exactly.--.</param>
    /// <returns>Pi rounded to the given precision.</returns>
    /// <exception cref='System.ArgumentException'>The parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</exception>
    public static ExtendedFloat PI(PrecisionContext ctx) {
      return math.Pi(ctx);
    }
  }
}
