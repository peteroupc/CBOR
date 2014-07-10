package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Represents an arbitrary-precision binary floating-point number. Consists of
     * an integer mantissa and an integer exponent, both
     * arbitrary-precision. The value of the number equals mantissa *
     * 2^exponent. This class also supports values for negative zero,
     * not-a-number (NaN) values, and infinity. <p>Passing a signaling NaN
     * to any arithmetic operation shown here will signal the flag
     * FlagInvalid and return a quiet NaN, even if another operand to that
     * operation is a quiet NaN, unless noted otherwise.</p> <p>Passing a
     * quiet NaN to any arithmetic operation shown here will return a quiet
     * NaN, unless noted otherwise.</p> <p>Unless noted otherwise, passing a
     * null ExtendedFloat argument to any method here will throw an
     * exception.</p> <p>When an arithmetic operation signals the flag
     * FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an
     * exception too, unless the operation's trap is enabled in the
     * precision context (see PrecisionContext's Traps property).</p> <p>An
     * ExtendedFloat value can be serialized in one of the following
     * ways:</p> <ul><li>By calling the toString() method. However, not all
     * strings can be converted back to an ExtendedFloat without loss,
     * especially if the string has a fractional part.</li> <li>By calling
     * the UnsignedMantissa, Exponent, and IsNegative properties, and
     * calling the IsInfinity, IsQuietNaN, and IsSignalingNaN methods. The
     * return values combined will uniquely identify a particular
     * ExtendedFloat value.</li> </ul>
     */
  public final class ExtendedFloat implements Comparable<ExtendedFloat> {
    private final BigInteger exponent;
    private final BigInteger unsignedMantissa;
    private final int flags;

    /**
     * Gets this object&apos;s exponent. This object&apos;s value will be an
     * integer if the exponent is positive or zero.
     * @return This object's exponent. This object's value will be an integer if
     * the exponent is positive or zero.
     */
    public final BigInteger getExponent() {
        return this.exponent;
      }

    /**
     * Gets the absolute value of this object&apos;s un-scaled value.
     * @return The absolute value of this object's un-scaled value.
     */
    public final BigInteger getUnsignedMantissa() {
        return this.unsignedMantissa;
      }

    /**
     * Gets this object&apos;s un-scaled value.
     * @return This object's un-scaled value. Will be negative if this object's
     * value is negative (including a negative NaN).
     */
    public final BigInteger getMantissa() {
        return this.isNegative() ? ((this.unsignedMantissa).negate()) :
          this.unsignedMantissa;
      }

    /**
     * Determines whether this object&apos;s mantissa and exponent are equal to
     * those of another object.
     * @param otherValue An ExtendedFloat object.
     * @return True if this object's mantissa and exponent are equal to those of
     * another object; otherwise, false.
     */
    public boolean EqualsInternal(ExtendedFloat otherValue) {
      if (otherValue == null) {
        return false;
      }
      return this.exponent.equals(otherValue.exponent) &&
        this.unsignedMantissa.equals(otherValue.unsignedMantissa) &&
        this.flags == otherValue.flags;
    }

    /**
     * Determines whether this object&apos;s mantissa and exponent are equal to
     * those of another object.
     * @param other An ExtendedFloat object.
     * @return True if this object's mantissa and exponent are equal to those of
     * another object; otherwise, false.
     */
    public boolean equals(ExtendedFloat other) {
      return this.EqualsInternal(other);
    }

    /**
     * Determines whether this object&apos;s mantissa and exponent are equal to
     * those of another object and that other object is a decimal fraction.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
     */
    @Override public boolean equals(Object obj) {
      return this.EqualsInternal(((obj instanceof ExtendedFloat) ? (ExtendedFloat)obj : null));
    }

    /**
     * Calculates this object&apos;s hash code.
     * @return This object's hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 403796923;
      {
        valueHashCode += 403797019 * this.exponent.hashCode();
        valueHashCode += 403797059 * this.unsignedMantissa.hashCode();
        valueHashCode += 403797127 * this.flags;
      }
      return valueHashCode;
    }

    /**
     * Creates a not-a-number ExtendedFloat object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @return A quiet not-a-number object.
     * @throws NullPointerException The parameter {@code diag} is null.
     * @throws IllegalArgumentException The parameter {@code diag} is less than 0.
     */
    public static ExtendedFloat CreateNaN(BigInteger diag) {
      return CreateNaN(diag, false, false, null);
    }

    /**
     * Creates a not-a-number ExtendedFloat object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @param signaling Whether the return value will be signaling (true) or quiet
     * (false).
     * @param negative Whether the return value is negative.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code diag} is null.
     * @throws IllegalArgumentException The parameter {@code diag} is less than 0.
     */
    public static ExtendedFloat CreateNaN(
      BigInteger diag,
      boolean signaling,
      boolean negative,
      PrecisionContext ctx) {
      if (diag == null) {
        throw new NullPointerException("diag");
      }
      if (diag.signum() < 0) {
        throw new
  IllegalArgumentException("Diagnostic information must be 0 or greater, was: " +
                            diag);
      }
      if (diag.signum() == 0 && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      int flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      if (ctx != null && ctx.getHasMaxPrecision()) {
        flags |= BigNumberFlags.FlagQuietNaN;
        ExtendedFloat ef = CreateWithFlags(
          diag,
          BigInteger.ZERO,
          flags).RoundToPrecision(ctx);
        int newFlags = ef.flags;
        newFlags &= ~BigNumberFlags.FlagQuietNaN;
        newFlags |= signaling ? BigNumberFlags.FlagSignalingNaN :
          BigNumberFlags.FlagQuietNaN;
        return new ExtendedFloat(ef.unsignedMantissa, ef.exponent, newFlags);
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      return CreateWithFlags(diag, BigInteger.ZERO, flags);
    }

    /**
     * Creates a number with the value exponent*2^mantissa.
     * @param mantissaSmall The un-scaled value.
     * @param exponentSmall The binary exponent.
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat Create(int mantissaSmall, int exponentSmall) {
      return Create(BigInteger.valueOf(mantissaSmall), BigInteger.valueOf(exponentSmall));
    }

    /**
     * Creates a number with the value exponent*2^mantissa.
     * @param mantissa The un-scaled value.
     * @param exponent The binary exponent.
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code mantissa} or {@code
     * exponent} is null.
     */
    public static ExtendedFloat Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new NullPointerException("mantissa");
      }
      if (exponent == null) {
        throw new NullPointerException("exponent");
      }
      int sign = mantissa.signum();
      return new ExtendedFloat(
        sign < 0 ? ((mantissa).negate()) : mantissa,
        exponent,
        (sign < 0) ? BigNumberFlags.FlagNegative : 0);
    }

    private ExtendedFloat(
      BigInteger unsignedMantissa,
      BigInteger exponent,
      int flags) {
      this.unsignedMantissa = unsignedMantissa;
      this.exponent = exponent;
      this.flags = flags;
    }

    static ExtendedFloat CreateWithFlags(
      BigInteger mantissa,
      BigInteger exponent,
      int flags) {
      if (mantissa == null) {
        throw new NullPointerException("mantissa");
      }
      if (exponent == null) {
        throw new NullPointerException("exponent");
      }
      int sign = mantissa == null ? 0 : mantissa.signum();
      return new ExtendedFloat(
        sign < 0 ? ((mantissa).negate()) : mantissa,
        exponent,
        flags);
    }

    /**
     * Creates a binary float from a string that represents a number. Note that if
     * the string contains a negative exponent, the resulting value might
     * not be exact. However, the resulting binary float will contain enough
     * precision to accurately convert it to a 32-bit or 64-bit floating
     * point number (float or double). <p>The format of the string generally
     * consists of: <ul><li>An optional '-' or '+' character (if '-' , the
     * value is negative.)</li> <li>One or more digits, with a single
     * optional decimal point after the first digit and before the last
     * digit.</li> <li>Optionally, E+ (positive exponent) or E- (negative
     * exponent) plus one or more digits specifying the exponent.</li> </ul>
     * </p> <p>The string can also be "-INF", "-Infinity", "Infinity",
     * "INF", quiet NaN ("qNaN") followed by any number of digits, or
     * signaling NaN ("sNaN") followed by any number of digits, all in any
     * combination of upper and lower case.</p> <p>The format generally
     * follows the definition in java.math.BigDecimal(), except that the
     * digits must be ASCII digits ('0' through '9').</p>
     * @param str A string object.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static ExtendedFloat FromString(
      String str,
      int offset,
      int length,
      PrecisionContext ctx) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return ExtendedDecimal.FromString(
        str,
        offset,
        length,
        ctx).ToExtendedFloat();
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static ExtendedFloat FromString(String str) {
      return FromString(str, 0, str == null ? 0 : str.length(), null);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static ExtendedFloat FromString(String str, PrecisionContext ctx) {
      return FromString(str, 0, str == null ? 0 : str.length(), ctx);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @return An ExtendedFloat object.
     * @throws NullPointerException The parameter {@code str} is null.
     */
    public static ExtendedFloat FromString(String str, int offset, int length) {
      return FromString(str, offset, length, null);
    }

    private static final class BinaryMathHelper implements IRadixMathHelper<ExtendedFloat> {
    /**
     * This is an internal method.
     * @return A 32-bit signed integer.
     */
      public int GetRadix() {
        return 2;
      }

    /**
     * This is an internal method.
     * @param value An ExtendedFloat object.
     * @return A 32-bit signed integer.
     */
      public int GetSign(ExtendedFloat value) {
        return value.signum();
      }

    /**
     * This is an internal method.
     * @param value An ExtendedFloat object.
     * @return A BigInteger object.
     */
      public BigInteger GetMantissa(ExtendedFloat value) {
        return value.getMantissa();
      }

    /**
     * This is an internal method.
     * @param value An ExtendedFloat object.
     * @return A BigInteger object.
     */
      public BigInteger GetExponent(ExtendedFloat value) {
        return value.exponent;
      }

    /**
     * This is an internal method.
     * @param bigint A BigInteger object.
     * @param lastDigit A 32-bit signed integer.
     * @param olderDigits A 32-bit signed integer. (2).
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(
        BigInteger bigint,
        int lastDigit,
        int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * This is an internal method.
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new BitShiftAccumulator(bigint, 0, 0);
      }

    /**
     * This is an internal method.
     * @param num A BigInteger object.
     * @param den A BigInteger object. (2).
     * @return A Boolean object.
     */
      public boolean HasTerminatingRadixExpansion(BigInteger num, BigInteger den) {
        BigInteger gcd = num.gcd(den);
        if (gcd.signum() == 0) {
          return false;
        }
        den = den.divide(gcd);
        while (den.testBit(0) == false) {
          den = den.shiftRight(1);
        }
        return den.equals(BigInteger.ONE);
      }

    /**
     * This is an internal method.
     * @param bigint A BigInteger object. (2).
     * @param power A FastInteger object.
     * @return A BigInteger object.
     */
      public BigInteger MultiplyByRadixPower(
        BigInteger bigint,
        FastInteger power) {
        BigInteger tmpbigint = bigint;
        if (power.signum() <= 0) {
          return tmpbigint;
        }
        if (tmpbigint.signum() < 0) {
          tmpbigint = tmpbigint.negate();
          if (power.CanFitInInt32()) {
            tmpbigint = DecimalUtility.ShiftLeftInt(tmpbigint, power.AsInt32());
            tmpbigint = tmpbigint.negate();
          } else {
            tmpbigint = DecimalUtility.ShiftLeft(
              tmpbigint,
              power.AsBigInteger());
            tmpbigint = tmpbigint.negate();
          }
          return tmpbigint;
        }
        return power.CanFitInInt32() ? DecimalUtility.ShiftLeftInt(
          tmpbigint,
          power.AsInt32()) : DecimalUtility.ShiftLeft(
          tmpbigint,
          power.AsBigInteger());
      }

    /**
     * This is an internal method.
     * @param value An ExtendedFloat object.
     * @return A 32-bit signed integer.
     */
      public int GetFlags(ExtendedFloat value) {
        return value.flags;
      }

    /**
     * This is an internal method.
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object. (2).
     * @param flags A 32-bit signed integer.
     * @return An ExtendedFloat object.
     */
      public ExtendedFloat CreateNewWithFlags(
        BigInteger mantissa,
        BigInteger exponent,
        int flags) {
        return ExtendedFloat.CreateWithFlags(mantissa, exponent, flags);
      }

    /**
     * This is an internal method.
     * @return A 32-bit signed integer.
     */
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteAndNonFinite;
      }

    /**
     * This is an internal method.
     * @param val A 32-bit signed integer.
     * @return An ExtendedFloat object.
     */
      public ExtendedFloat ValueOf(int val) {
        return FromInt64(val);
      }
    }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional part
     * of this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     * @throws ArithmeticException This object's value is infinity or NaN.
     */
    public BigInteger ToBigInteger() {
      return this.ToBigIntegerInternal(false);
    }

    /**
     * Converts this value to an arbitrary-precision integer, checking whether the
     * value contains a fractional part.
     * @return A BigInteger object.
     * @throws ArithmeticException This object's value is infinity or NaN.
     * @throws ArithmeticException This object's value is not an exact integer.
     */
    public BigInteger ToBigIntegerExact() {
      return this.ToBigIntegerInternal(true);
    }

    private BigInteger ToBigIntegerInternal(boolean exact) {
      if (!this.isFinite()) {
        throw new ArithmeticException("Value is infinity or NaN");
      }
      if (this.signum() == 0) {
        return BigInteger.ZERO;
      }
      int expsign = this.getExponent().signum();
      if (expsign == 0) {
        // Integer
        return this.getMantissa();
      }
      if (expsign > 0) {
        // Integer with trailing zeros
        BigInteger curexp = this.getExponent();
        BigInteger bigmantissa = this.getMantissa();
        if (bigmantissa.signum() == 0) {
          return bigmantissa;
        }
        boolean neg = bigmantissa.signum() < 0;
        if (neg) {
          bigmantissa = bigmantissa.negate();
        }
        bigmantissa = DecimalUtility.ShiftLeft(bigmantissa, curexp);
        if (neg) {
          bigmantissa = bigmantissa.negate();
        }
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.getMantissa();
        FastInteger bigexponent = FastInteger.FromBig(this.getExponent()).Negate();
        bigmantissa = (bigmantissa).abs();
        BitShiftAccumulator acc = new BitShiftAccumulator(bigmantissa, 0, 0);
        acc.ShiftRight(bigexponent);
        if (exact && (acc.getLastDiscardedDigit() != 0 || acc.getOlderDiscardedDigits() !=
                      0)) {
          // Some digits were discarded
          throw new ArithmeticException("Not an exact integer");
        }
        bigmantissa = acc.getShiftedInt();
        if (this.isNegative()) {
          bigmantissa = bigmantissa.negate();
        }
        return bigmantissa;
      }
    }

    private static BigInteger valueOneShift23 = BigInteger.ONE.shiftLeft(23);
    private static BigInteger valueOneShift52 = BigInteger.ONE.shiftLeft(52);

    /**
     * Converts this value to a 32-bit floating-point number. The half-even
     * rounding mode is used. <p>If this value is a NaN, sets the high bit
     * of the 32-bit floating point number's mantissa for a quiet NaN, and
     * clears it for a signaling NaN. Then the next highest bit of the
     * mantissa is cleared for a quiet NaN, and set for a signaling NaN.
     * Then the other bits of the mantissa are set to the lowest bits of
     * this object's unsigned mantissa.</p>
     * @return The closest 32-bit floating-point number to this value. The return
     * value can be positive infinity or negative infinity if this value
     * exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      if (this.IsPositiveInfinity()) {
        return Float.POSITIVE_INFINITY;
      }
      if (this.IsNegativeInfinity()) {
        return Float.NEGATIVE_INFINITY;
      }
      if (this.IsNaN()) {
        int nan = 0x7f800000;
        if (this.isNegative()) {
          nan |= ((int)(1 << 31));
        }
        if (this.IsQuietNaN()) {
          // the quiet bit for X86 at least
          nan |= 0x400000;
        } else {
          // not really the signaling bit, but done to keep
          // the mantissa from being zero
          nan |= 0x200000;
        }
        if (this.getUnsignedMantissa().signum() != 0) {
          // Transfer diagnostic information
          BigInteger bigdata = this.getUnsignedMantissa().remainder(BigInteger.valueOf(0x200000));
          nan |= bigdata.intValue();
        }
        return Float.intBitsToFloat(nan);
      }
      if (this.isNegative() && this.signum() == 0) {
        return Float.intBitsToFloat(1 << 31);
      }
      BigInteger bigmant = (this.unsignedMantissa).abs();
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.signum() == 0) {
        return 0.0f;
      }
      int smallmant = 0;
      FastInteger fastSmallMant;
      if (bigmant.compareTo(valueOneShift23) < 0) {
        smallmant = bigmant.intValue();
        int exponentchange = 0;
        while (smallmant < (1 << 23)) {
          smallmant <<= 1;
          ++exponentchange;
        }
        bigexponent.SubtractInt(exponentchange);
        fastSmallMant = new FastInteger(smallmant);
      } else {
        BitShiftAccumulator accum = new BitShiftAccumulator(bigmant, 0, 0);
        accum.ShiftToDigitsInt(24);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
        fastSmallMant = accum.getShiftedIntFast();
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              !fastSmallMant.isEvenNumber())) {
        fastSmallMant.Increment();
        if (fastSmallMant.CompareToInt(1 << 24) == 0) {
          fastSmallMant = new FastInteger(1 << 23);
          bigexponent.Increment();
        }
      }
      boolean subnormal = false;
      if (bigexponent.CompareToInt(104) > 0) {
        // exponent too big
        return this.isNegative() ?
          Float.NEGATIVE_INFINITY :
          Float.POSITIVE_INFINITY;
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum =
          BitShiftAccumulator.FromInt32(fastSmallMant.AsInt32());
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
        fastSmallMant = accum.getShiftedIntFast();
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                                !fastSmallMant.isEvenNumber())) {
          fastSmallMant.Increment();
          if (fastSmallMant.CompareToInt(1 << 24) == 0) {
            fastSmallMant = new FastInteger(1 << 23);
            bigexponent.Increment();
          }
        }
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // exponent too small, so return zero
        return this.isNegative() ?
          Float.intBitsToFloat(1 << 31) :
          Float.intBitsToFloat(0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent += 150;
        int smallmantissa = ((int)fastSmallMant.AsInt32()) & 0x7fffff;
        if (!subnormal) {
          smallmantissa |= smallexponent << 23;
        }
        if (this.isNegative()) {
          smallmantissa |= 1 << 31;
        }
        return Float.intBitsToFloat(smallmantissa);
      }
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-even
     * rounding mode is used. <p>If this value is a NaN, sets the high bit
     * of the 64-bit floating point number's mantissa for a quiet NaN, and
     * clears it for a signaling NaN. Then the next highest bit of the
     * mantissa is cleared for a quiet NaN, and set for a signaling NaN.
     * Then the other bits of the mantissa are set to the lowest bits of
     * this object's unsigned mantissa.</p>
     * @return The closest 64-bit floating-point number to this value. The return
     * value can be positive infinity or negative infinity if this value
     * exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      if (this.IsPositiveInfinity()) {
        return Double.POSITIVE_INFINITY;
      }
      if (this.IsNegativeInfinity()) {
        return Double.NEGATIVE_INFINITY;
      }
      if (this.IsNaN()) {
        int[] nan = { 0, 0x7ff00000 };
        if (this.isNegative()) {
          nan[1] |= ((int)(1 << 31));
        }
        // 0x40000 is not really the signaling bit, but done to keep
        // the mantissa from being zero
        if (this.IsQuietNaN()) {
          nan[1] |= 0x80000;
        } else {
          nan[1] |= 0x40000;
        }
        if (this.getUnsignedMantissa().signum() != 0) {
          // Copy diagnostic information
          int[] words = FastInteger.GetLastWords(this.getUnsignedMantissa(), 2);
          nan[0] = words[0];
          nan[1] = words[1] & 0x3ffff;
        }
        return Extras.IntegersToDouble(nan);
      }
      if (this.isNegative() && this.signum() == 0) {
        return Extras.IntegersToDouble(new int[] { ((int)(1 << 31)), 0 });
      }
      BigInteger bigmant = (this.unsignedMantissa).abs();
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.signum() == 0) {
        return 0.0d;
      }
      int[] mantissaBits;
      if (bigmant.compareTo(valueOneShift52) < 0) {
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
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
        mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                              DecimalUtility.HasBitSet(mantissaBits, 0))) {
        // Add 1 to the bits
        mantissaBits[0] = ((int)(mantissaBits[0] + 1));
        if (mantissaBits[0] == 0) {
          mantissaBits[1] = ((int)(mantissaBits[1] + 1));
        }
        if (mantissaBits[0] == 0 &&
            mantissaBits[1] == (1 << 21)) {  // if mantissa is now 2^53
          mantissaBits[1] >>= 1;  // change it to 2^52
          bigexponent.Increment();
        }
      }
      boolean subnormal = false;
      if (bigexponent.CompareToInt(971) > 0) {
        // exponent too big
        return this.isNegative() ?
          Double.NEGATIVE_INFINITY :
          Double.POSITIVE_INFINITY;
      }
      if (bigexponent.CompareToInt(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum = new BitShiftAccumulator(
          FastInteger.WordsToBigInteger(mantissaBits),
          0,
          0);
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.getOlderDiscardedDigits();
        bitLeftmost = accum.getLastDiscardedDigit();
        bigexponent.Add(accum.getDiscardedDigitCount());
        mantissaBits = FastInteger.GetLastWords(accum.getShiftedInt(), 2);
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                                DecimalUtility.HasBitSet(mantissaBits, 0))) {
          // Add 1 to the bits
          mantissaBits[0] = ((int)(mantissaBits[0] + 1));
          if (mantissaBits[0] == 0) {
            mantissaBits[1] = ((int)(mantissaBits[1] + 1));
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
        return this.isNegative() ?
          Extras.IntegersToDouble(new int[] { 0, ((int)0x80000000) }) :
          0.0d;
      }
      bigexponent.AddInt(1075);
      // Clear the high bits where the exponent and sign are
      mantissaBits[1] &= 0xfffff;
      if (!subnormal) {
        int smallexponent = bigexponent.AsInt32() << 20;
        mantissaBits[1] |= smallexponent;
      }
      if (this.isNegative()) {
        mantissaBits[1] |= ((int)(1 << 31));
      }
      return Extras.IntegersToDouble(mantissaBits);
    }

    /**
     * Creates a binary float from a 32-bit floating-point number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param flt A 32-bit floating-point number.
     * @return A binary float with the same value as {@code flt} .
     */
    public static ExtendedFloat FromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      boolean neg = (value >> 31) != 0;
      int floatExponent = (int)((value >> 23) & 0xff);
      int valueFpMantissa = value & 0x7fffff;
      BigInteger bigmant;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        boolean quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1fffff;
        bigmant = BigInteger.valueOf(valueFpMantissa);
        value = (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ?
  BigNumberFlags.FlagQuietNaN :
  BigNumberFlags.FlagSignalingNaN);
        if (bigmant.signum() == 0) {
          return quiet ? NaN : SignalingNaN;
        }
        return CreateWithFlags(
          bigmant,
          BigInteger.ZERO,
          value);
      }
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        valueFpMantissa |= 1 << 23;
      }
      if (valueFpMantissa == 0) {
        return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      while ((valueFpMantissa & 1) == 0) {
        ++floatExponent;
        valueFpMantissa >>= 1;
      }
      if (neg) {
        valueFpMantissa = -valueFpMantissa;
      }
      bigmant = BigInteger.valueOf(valueFpMantissa);
      return ExtendedFloat.Create(
        bigmant,
        BigInteger.valueOf(floatExponent - 150));
    }

    /**
     * Not documented yet.
     * @param bigint A BigInteger object.
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat FromBigInteger(BigInteger bigint) {
      return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Not documented yet.
     * @param valueSmall A 64-bit signed integer.
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat FromInt64(long valueSmall) {
      BigInteger bigint = BigInteger.valueOf(valueSmall);
      return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Creates a binary float from a 32-bit signed integer.
     * @param valueSmaller A 32-bit signed integer.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedFloat FromInt32(int valueSmaller) {
      BigInteger bigint = BigInteger.valueOf(valueSmaller);
      return ExtendedFloat.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Creates a binary float from a 64-bit floating-point number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param dbl A 64-bit floating-point number.
     * @return A binary float with the same value as {@code dbl} .
     */
    public static ExtendedFloat FromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      int floatExponent = (int)((value[1] >> 20) & 0x7ff);
      boolean neg = (value[1] >> 31) != 0;
      if (floatExponent == 2047) {
        if ((value[1] & 0xfffff) == 0 && value[0] == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        boolean quiet = (value[1] & 0x80000) != 0;
        value[1] &= 0x3ffff;
        BigInteger info = FastInteger.WordsToBigInteger(value);
        if (info.signum() == 0) {
          return quiet ? NaN : SignalingNaN;
        }
        value[0] = (neg ? BigNumberFlags.FlagNegative : 0) |
          (quiet ? BigNumberFlags.FlagQuietNaN :
           BigNumberFlags.FlagSignalingNaN);
        return CreateWithFlags(
          info,
          BigInteger.ZERO,
          value[0]);
      }
      value[1] &= 0xfffff;  // Mask out the exponent and sign
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        value[1] |= 0x100000;
      }
      if ((value[1] | value[0]) != 0) {
        floatExponent +=
          DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      } else {
        return neg ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      return CreateWithFlags(
        FastInteger.WordsToBigInteger(value),
        BigInteger.valueOf(floatExponent - 1075),
        neg ? BigNumberFlags.FlagNegative : 0);
    }

    /**
     * Not documented yet.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal ToExtendedDecimal() {
      return ExtendedDecimal.FromExtendedFloat(this);
    }

    /**
     * Converts this value to a string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return ExtendedDecimal.FromExtendedFloat(this).toString();
    }

    /**
     * Same as toString(), except that when an exponent is used it will be a
     * multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     * @return A string object.
     */
    public String ToEngineeringString() {
      return this.ToExtendedDecimal().ToEngineeringString();
    }

    /**
     * Converts this value to a string, but without an exponent part. The format of
     * the return value follows the format of the
     * java.math.BigDecimal.toPlainString() method.
     * @return A string object.
     */
    public String ToPlainString() {
      return this.ToExtendedDecimal().ToPlainString();
    }

    /**
     * Represents the number 1.
     */

    public static final ExtendedFloat One =
      ExtendedFloat.Create(BigInteger.ONE, BigInteger.ZERO);

    /**
     * Represents the number 0.
     */

    public static final ExtendedFloat Zero =
      ExtendedFloat.Create(BigInteger.ZERO, BigInteger.ZERO);

    /**
     * Represents the number negative zero.
     */

    public static final ExtendedFloat NegativeZero = CreateWithFlags(
      BigInteger.ZERO,
      BigInteger.ZERO,
      BigNumberFlags.FlagNegative);

    /**
     * Represents the number 10.
     */

    public static final ExtendedFloat Ten =
      ExtendedFloat.Create(BigInteger.TEN, BigInteger.ZERO);

    //----------------------------------------------------------------

    /**
     * A not-a-number value.
     */
    public static final ExtendedFloat NaN = CreateWithFlags(
      BigInteger.ZERO,
      BigInteger.ZERO,
      BigNumberFlags.FlagQuietNaN);

    /**
     * A not-a-number value that signals an invalid operation flag when it&apos;s
     * passed as an argument to any arithmetic operation in ExtendedFloat.
     */
    public static final ExtendedFloat SignalingNaN = CreateWithFlags(
      BigInteger.ZERO,
      BigInteger.ZERO,
      BigNumberFlags.FlagSignalingNaN);

    /**
     * Positive infinity, greater than any other number.
     */
    public static final ExtendedFloat PositiveInfinity = CreateWithFlags(
      BigInteger.ZERO,
      BigInteger.ZERO,
      BigNumberFlags.FlagInfinity);

    /**
     * Negative infinity, less than any other number.
     */
    public static final ExtendedFloat NegativeInfinity = CreateWithFlags(
      BigInteger.ZERO,
      BigInteger.ZERO,
      BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    /**
     * Returns whether this object is negative infinity.
     * @return True if this object is negative infinity; otherwise, false.
     */
    public boolean IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                            BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /**
     * Returns whether this object is positive infinity.
     * @return True if this object is positive infinity; otherwise, false.
     */
    public boolean IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                            BigNumberFlags.FlagNegative)) ==
        BigNumberFlags.FlagInfinity;
    }

    /**
     * Returns whether this object is a not-a-number value.
     * @return True if this object is a not-a-number value; otherwise, false.
     */
    public boolean IsNaN() {
      return (this.flags & (BigNumberFlags.FlagQuietNaN |
                            BigNumberFlags.FlagSignalingNaN)) != 0;
    }

    /**
     * Gets a value indicating whether this object is positive or negative
     * infinity.
     * @return True if this object is positive or negative infinity; otherwise,
     * false.
     */
    public boolean IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /**
     * Gets a value indicating whether this object is finite (not infinity or NaN).
     * @return True if this object is finite (not infinity or NaN); otherwise,
     * false.
     */
    public final boolean isFinite() {
        return (this.flags & (BigNumberFlags.FlagInfinity |
                              BigNumberFlags.FlagNaN)) == 0;
      }

    /**
     * Gets a value indicating whether this object is negative, including negative
     * zero.
     * @return True if this object is negative, including negative zero; otherwise,
     * false.
     */
    public final boolean isNegative() {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }

    /**
     * Gets a value indicating whether this object is a quiet not-a-number value.
     * @return True if this object is a quiet not-a-number value; otherwise, false.
     */
    public boolean IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /**
     * Gets a value indicating whether this object is a signaling not-a-number
     * value.
     * @return True if this object is a signaling not-a-number value; otherwise,
     * false.
     */
    public boolean IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /**
     * Gets this value&apos;s sign: -1 if negative; 1 if positive; 0 if zero.
     * @return This value's sign: -1 if negative; 1 if positive; 0 if zero.
     */
    public final int signum() {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
                this.unsignedMantissa.signum() == 0) ? 0 :
          (((this.flags & BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
      }

    /**
     * Gets a value indicating whether this object&apos;s value equals 0.
     * @return True if this object's value equals 0; otherwise, false.
     */
    public final boolean isZero() {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
          this.unsignedMantissa.signum() == 0;
      }

    /**
     * Gets the absolute value of this object.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat Abs() {
      return this.Abs(null);
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat Negate() {
      return this.Negate(null);
    }

    /**
     * Divides this object by another binary float and returns the result. When
     * possible, the result will be exact.
     * @param divisor The divisor.
     * @return The quotient of the two numbers. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0.
     * @throws ArithmeticException The result can't be exact because it would have
     * a nonterminating binary expansion.
     */
    public ExtendedFloat Divide(ExtendedFloat divisor) {
      return this.Divide(
        divisor,
        PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /**
     * Divides this object by another binary float and returns a result with the
     * same exponent as this object (the dividend).
     * @param divisor The divisor.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two numbers. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the result is not exact.
     */
    public ExtendedFloat DivideToSameExponent(
      ExtendedFloat divisor,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        this.exponent,
        PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two ExtendedFloat objects, and returns the integer part of the
     * result, rounded down, with the preferred exponent set to this
     * value&apos;s exponent minus the divisor&apos;s exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects. Signals
     * FlagDivideByZero and returns infinity if the divisor is 0 and the
     * dividend is nonzero. Signals FlagInvalid and returns NaN if the
     * divisor and the dividend are 0.
     */
    public ExtendedFloat DivideToIntegerNaturalScale(
      ExtendedFloat divisor) {
      return this.DivideToIntegerNaturalScale(
        divisor,
        PrecisionContext.ForRounding(Rounding.Down));
    }

    /**
     * Removes trailing zeros from this object&apos;s mantissa. For example, 1.000
     * becomes 1. <p>If this object's value is 0, changes the exponent to 0.
     * (This is unlike the behavior in Java's BigDecimal method
     * "stripTrailingZeros" in Java 7 and earlier.)</p>
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return This value with trailing zeros removed. Note that if the result has
     * a very high exponent and the context says to clamp high exponents,
     * there may still be some trailing zeros in the mantissa.
     */
    public ExtendedFloat Reduce(
      PrecisionContext ctx) {
      return MathValue.Reduce(this, ctx);
    }

    /**
     * Not documented yet.
     * @param divisor An ExtendedFloat object. (2).
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat RemainderNaturalScale(
      ExtendedFloat divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /**
     * Calculates the remainder of a number by the formula this - ((this / divisor)
     * * divisor). This is meant to be similar to the remainder operation in
     * Java's BigDecimal.
     * @param divisor An ExtendedFloat object. (2).
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the integer part of the result. This context
     * will be used only in the division portion of the remainder
     * calculation. Flags will be set on the given context only if the
     * context&apos;s HasFlags is true and the integer part of the division
     * result doesn&apos;t fit the precision and exponent range without
     * rounding.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat RemainderNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, ctx).Multiply(divisor, null),
        null);
    }

    /**
     * Divides two ExtendedFloat objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedFloat object.
     * @param desiredExponentSmall The desired exponent. A negative number places
     * the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal
     * point.
     * @param ctx A precision context object to control the rounding mode to use if
     * the result must be scaled down to have the same exponent as this
     * value. If the precision given in the context is other than 0, calls
     * the Quantize method with both arguments equal to the result of the
     * operation (and can signal FlagInvalid and return NaN if the result
     * doesn&apos;t fit the given precision). If HasFlags of the context is
     * true, will also store the flags resulting from the operation (the
     * flags are in addition to the pre-existing flags). Can be null, in
     * which case the default rounding mode is HalfEven.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0. Signals FlagInvalid and returns NaN if the context defines an
     * exponent range and the desired exponent is outside that range.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the result is not exact.
     */
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      return this.DivideToExponent(
        divisor,
        BigInteger.valueOf(desiredExponentSmall),
        ctx);
    }

    /**
     * Divides this ExtendedFloat object by another ExtendedFloat object. The
     * preferred exponent for the result is this object&apos;s exponent
     * minus the divisor&apos;s exponent.
     * @param divisor The divisor.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0.
     * @throws ArithmeticException Either {@code ctx} is null or {@code ctx} 's
     * precision is 0, and the result would have a nonterminating binary
     * expansion; or, the rounding mode is Rounding.Unnecessary and the
     * result is not exact.
     */
    public ExtendedFloat Divide(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return MathValue.Divide(this, divisor, ctx);
    }

    /**
     * Divides two ExtendedFloat objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedFloat object.
     * @param desiredExponentSmall The desired exponent. A negative number places
     * the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal
     * point.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the result is not exact.
     */
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        BigInteger.valueOf(desiredExponentSmall),
        PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two ExtendedFloat objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedFloat object.
     * @param exponent The desired exponent. A negative number places the cutoff
     * point to the right of the usual decimal point. A positive number
     * places the cutoff point to the left of the usual decimal point.
     * @param ctx A precision context object to control the rounding mode to use if
     * the result must be scaled down to have the same exponent as this
     * value. If the precision given in the context is other than 0, calls
     * the Quantize method with both arguments equal to the result of the
     * operation (and can signal FlagInvalid and return NaN if the result
     * doesn&apos;t fit the given precision). If HasFlags of the context is
     * true, will also store the flags resulting from the operation (the
     * flags are in addition to the pre-existing flags). Can be null, in
     * which case the default rounding mode is HalfEven.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0. Signals FlagInvalid and returns NaN if the context defines an
     * exponent range and the desired exponent is outside that range.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the result is not exact.
     */
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.DivideToExponent(this, divisor, exponent, ctx);
    }

    /**
     * Divides two ExtendedFloat objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedFloat object.
     * @param desiredExponent The desired exponent. A negative number places the
     * cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal
     * point.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the result is not exact.
     */
    public ExtendedFloat DivideToExponent(
      ExtendedFloat divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        desiredExponent,
        PrecisionContext.ForRounding(rounding));
    }

    /**
     * Finds the absolute value of this object (if it&apos;s negative, it becomes
     * positive).
     * @param context A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags are
     * in addition to the pre-existing flags). Can be null.
     * @return The absolute value of this object.
     */
    public ExtendedFloat Abs(PrecisionContext context) {
      return MathValue.Abs(this, context);
    }

    /**
     * Returns a binary float with the same value as this object but with the sign
     * reversed.
     * @param context A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags are
     * in addition to the pre-existing flags). Can be null.
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat Negate(PrecisionContext context) {
      return MathValue.Negate(this, context);
    }

    /**
     * Adds this object and another binary float and returns the result.
     * @param otherValue An ExtendedFloat object.
     * @return The sum of the two objects.
     */
    public ExtendedFloat Add(ExtendedFloat otherValue) {
      return this.Add(otherValue, PrecisionContext.Unlimited);
    }

    /**
     * Subtracts an ExtendedFloat object from this instance and returns the
     * result..
     * @param otherValue An ExtendedFloat object.
     * @return The difference of the two objects.
     */
    public ExtendedFloat Subtract(ExtendedFloat otherValue) {
      return this.Subtract(otherValue, null);
    }

    /**
     * Subtracts an ExtendedFloat object from this instance.
     * @param otherValue An ExtendedFloat object.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The difference of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedFloat Subtract(
      ExtendedFloat otherValue,
      PrecisionContext ctx) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
      }
      ExtendedFloat negated = otherValue;
      if ((otherValue.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = otherValue.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          otherValue.unsignedMantissa,
          otherValue.exponent,
          newflags);
      }
      return this.Add(negated, ctx);
    }

    /**
     * Multiplies two binary floats. The resulting exponent will be the sum of the
     * exponents of the two binary floats.
     * @param otherValue Another binary float.
     * @return The product of the two binary floats.
     */
    public ExtendedFloat Multiply(ExtendedFloat otherValue) {
      return this.Multiply(otherValue, PrecisionContext.Unlimited);
    }

    /**
     * Multiplies by one binary float, and then adds another binary float.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @return The result this * multiplicand + augend.
     */
    public ExtendedFloat MultiplyAndAdd(
      ExtendedFloat multiplicand,
      ExtendedFloat augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static final IRadixMath<ExtendedFloat> MathValue = new
      TrappableRadixMath<ExtendedFloat>(
        new ExtendedOrSimpleRadixMath<ExtendedFloat>(new BinaryMathHelper()));

    /**
     * Divides this object by another object, and returns the integer part of the
     * result, with the preferred exponent set to this value&apos;s exponent
     * minus the divisor&apos;s exponent.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the integer part of the result. Flags will be
     * set on the given context only if the context&apos;s HasFlags is true
     * and the integer part of the result doesn&apos;t fit the precision and
     * exponent range without rounding.
     * @return The integer part of the quotient of the two objects. Returns null if
     * the return value would overflow the exponent range. A caller can
     * handle a null return value by treating it as positive infinity if
     * both operands have the same sign or as negative infinity if both
     * operands have different signs. Signals FlagDivideByZero and returns
     * infinity if the divisor is 0 and the dividend is nonzero. Signals
     * FlagInvalid and returns NaN if the divisor and the dividend are 0.
     * @throws ArithmeticException The rounding mode is Rounding.Unnecessary and
     * the integer part of the result is not exact.
     */
    public ExtendedFloat DivideToIntegerNaturalScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return MathValue.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /**
     * Divides this object by another object, and returns the integer part of the
     * result, with the exponent set to 0.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The rounding
     * and exponent range settings of this context are ignored. If HasFlags
     * of the context is true, will also store the flags resulting from the
     * operation (the flags are in addition to the pre-existing flags). Can
     * be null.
     * @return The integer part of the quotient of the two objects. The exponent
     * will be set to 0. Signals FlagDivideByZero and returns infinity if
     * the divisor is 0 and the dividend is nonzero. Signals FlagInvalid and
     * returns NaN if the divisor and the dividend are 0, or if the result
     * doesn't fit the given precision.
     */
    public ExtendedFloat DivideToIntegerZeroScale(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return MathValue.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /**
     * Finds the remainder that results when dividing two ExtendedFloat objects.
     * @param divisor An ExtendedFloat object.
     * @param ctx A PrecisionContext object.
     * @return The remainder of the two objects.
     */
    public ExtendedFloat Remainder(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return MathValue.Remainder(this, divisor, ctx);
    }

    /**
     * Finds the distance to the closest multiple of the given divisor, based on
     * the result of dividing this object&apos;s value by another
     * object&apos;s value. <ul><li>If this and the other object divide
     * evenly, the result is 0.</li> <li>If the remainder's absolute value
     * is less than half of the divisor's absolute value, the result has the
     * same sign as this object and will be the distance to the closest
     * multiple.</li> <li>If the remainder's absolute value is more than
     * half of the divisor' s absolute value, the result has the opposite
     * sign of this object and will be the distance to the closest
     * multiple.</li> <li>If the remainder's absolute value is exactly half
     * of the divisor's absolute value, the result has the opposite sign of
     * this object if the quotient, rounded down, is odd, and has the same
     * sign as this object if the quotient, rounded down, is even, and the
     * result's absolute value is half of the divisor's absolute value.</li>
     * </ul> This function is also known as the "IEEE Remainder" function.
     * @param divisor The divisor.
     * @param ctx A precision context object to control the precision. The rounding
     * and exponent range settings of this context are ignored (the rounding
     * mode is always ((treated instanceof HalfEven) ? (HalfEven)treated :
     * null)). If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null.
     * @return The distance of the closest multiple. Signals FlagInvalid and
     * returns NaN if the divisor is 0, or either the result of integer
     * division (the quotient) or the remainder wouldn't fit the given
     * precision.
     */
    public ExtendedFloat RemainderNear(
      ExtendedFloat divisor,
      PrecisionContext ctx) {
      return MathValue.RemainderNear(this, divisor, ctx);
    }

    /**
     * Finds the largest value that&apos;s smaller than the given value.
     * @param ctx A precision context object to control the precision and exponent
     * range of the result. The rounding mode from this context is ignored.
     * If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags).
     * @return Returns the largest value that's less than the given value. Returns
     * negative infinity if the result is negative infinity.
     * @throws IllegalArgumentException The parameter {@code ctx} is null, the precision
     * is 0, or {@code ctx} has an unlimited exponent range.
     */
    public ExtendedFloat NextMinus(
      PrecisionContext ctx) {
      return MathValue.NextMinus(this, ctx);
    }

    /**
     * Finds the smallest value that&apos;s greater than the given value.
     * @param ctx A precision context object to control the precision and exponent
     * range of the result. The rounding mode from this context is ignored.
     * If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags).
     * @return Returns the smallest value that's greater than the given value.
     * @throws IllegalArgumentException The parameter {@code ctx} is null, the precision
     * is 0, or {@code ctx} has an unlimited exponent range.
     */
    public ExtendedFloat NextPlus(
      PrecisionContext ctx) {
      return MathValue.NextPlus(this, ctx);
    }

    /**
     * Finds the next value that is closer to the other object&apos;s value than
     * this object&apos;s value.
     * @param otherValue An ExtendedFloat object.
     * @param ctx A precision context object to control the precision and exponent
     * range of the result. The rounding mode from this context is ignored.
     * If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags).
     * @return Returns the next value that is closer to the other object' s value
     * than this object's value.
     * @throws IllegalArgumentException The parameter {@code ctx} is null, the precision
     * is 0, or {@code ctx} has an unlimited exponent range.
     */
    public ExtendedFloat NextToward(
      ExtendedFloat otherValue,
      PrecisionContext ctx) {
      return MathValue.NextToward(this, otherValue, ctx);
    }

    /**
     * Gets the greater value between two binary floats.
     * @param first An ExtendedFloat object.
     * @param second An ExtendedFloat object. (2).
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The larger value of the two objects.
     */
    public static ExtendedFloat Max(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return MathValue.Max(first, second, ctx);
    }

    /**
     * Gets the lesser value between two binary floats.
     * @param first An ExtendedFloat object.
     * @param second An ExtendedFloat object. (2).
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The smaller value of the two objects.
     */
    public static ExtendedFloat Min(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return MathValue.Min(first, second, ctx);
    }

    /**
     * Gets the greater value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Max.
     * @param first An ExtendedFloat object. (2).
     * @param second An ExtendedFloat object. (3).
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat MaxMagnitude(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return MathValue.MaxMagnitude(first, second, ctx);
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Min.
     * @param first An ExtendedFloat object. (2).
     * @param second An ExtendedFloat object. (3).
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat MinMagnitude(
      ExtendedFloat first,
      ExtendedFloat second,
      PrecisionContext ctx) {
      return MathValue.MinMagnitude(first, second, ctx);
    }

    /**
     * Gets the greater value between two binary floats.
     * @param first An ExtendedFloat object.
     * @param second An ExtendedFloat object. (2).
     * @return The larger value of the two objects.
     */
    public static ExtendedFloat Max(
      ExtendedFloat first,
      ExtendedFloat second) {
      return Max(first, second, null);
    }

    /**
     * Gets the lesser value between two binary floats.
     * @param first An ExtendedFloat object.
     * @param second An ExtendedFloat object. (2).
     * @return The smaller value of the two objects.
     */
    public static ExtendedFloat Min(
      ExtendedFloat first,
      ExtendedFloat second) {
      return Min(first, second, null);
    }

    /**
     * Gets the greater value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Max.
     * @param first An ExtendedFloat object. (2).
     * @param second An ExtendedFloat object. (3).
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat MaxMagnitude(
      ExtendedFloat first,
      ExtendedFloat second) {
      return MaxMagnitude(first, second, null);
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Min.
     * @param first An ExtendedFloat object. (2).
     * @param second An ExtendedFloat object. (3).
     * @return An ExtendedFloat object.
     */
    public static ExtendedFloat MinMagnitude(
      ExtendedFloat first,
      ExtendedFloat second) {
      return MinMagnitude(first, second, null);
    }

    /**
     * Compares the mathematical values of this object and another object,
     * accepting NaN values. <p>This method is not consistent with the
     * Equals method because two different numbers with the same
     * mathematical value, but different exponents, will compare as
     * equal.</p> <p>In this method, negative zero and positive zero are
     * considered equal.</p> <p>If this object or the other object is a
     * quiet NaN or signaling NaN, this method will not trigger an error.
     * Instead, NaN will compare greater than any other number, including
     * infinity. Two different NaN values will be considered equal.</p>
     * @param other An ExtendedFloat object.
     * @return Less than 0 if this object's value is less than the other value, or
     * greater than 0 if this object's value is greater than the other value
     * or if {@code other} is null, or 0 if both values are equal.
     */
    public int compareTo(
      ExtendedFloat other) {
      return MathValue.compareTo(this, other);
    }

    /**
     * Compares the mathematical values of this object and another object. <p>In
     * this method, negative zero and positive zero are considered
     * equal.</p> <p>If this object or the other object is a quiet NaN or
     * signaling NaN, this method returns a quiet NaN, and will signal a
     * FlagInvalid flag if either is a signaling NaN.</p>
     * @param other An ExtendedFloat object.
     * @param ctx A precision context. The precision, rounding, and exponent range
     * are ignored. If HasFlags of the context is true, will store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null.
     * @return Quiet NaN if this object or the other object is NaN, or 0 if both
     * objects have the same value, or -1 if this object is less than the
     * other value, or 1 if this object is greater.
     */
    public ExtendedFloat CompareToWithContext(
      ExtendedFloat other,
      PrecisionContext ctx) {
      return MathValue.CompareToWithContext(this, other, false, ctx);
    }

    /**
     * Compares the mathematical values of this object and another object, treating
     * quiet NaN as signaling. <p>In this method, negative zero and positive
     * zero are considered equal.</p> <p>If this object or the other object
     * is a quiet NaN or signaling NaN, this method will return a quiet NaN
     * and will signal a FlagInvalid flag.</p>
     * @param other An ExtendedFloat object.
     * @param ctx A precision context. The precision, rounding, and exponent range
     * are ignored. If HasFlags of the context is true, will store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null.
     * @return Quiet NaN if this object or the other object is NaN, or 0 if both
     * objects have the same value, or -1 if this object is less than the
     * other value, or 1 if this object is greater.
     */
    public ExtendedFloat CompareToSignal(
      ExtendedFloat other,
      PrecisionContext ctx) {
      return MathValue.CompareToWithContext(this, other, true, ctx);
    }

    /**
     * Finds the sum of this object and another object. The result&apos;s exponent
     * is set to the lower of the exponents of the two operands.
     * @param otherValue The number to add to.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The sum of thisValue and the other object.
     */
    public ExtendedFloat Add(
      ExtendedFloat otherValue,
      PrecisionContext ctx) {
      return MathValue.Add(this, otherValue, ctx);
    }

    /**
     * Returns a binary float with the same value but a new exponent.
     * @param desiredExponent A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A binary float with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the rounded result can't fit the given precision,
     * or if the context defines an exponent range and the given exponent is
     * outside that range.
     */
    public ExtendedFloat Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(
        ExtendedFloat.Create(BigInteger.ONE, desiredExponent),
        ctx);
    }

    /**
     * Returns a binary float with the same value but a new exponent.
     * @param desiredExponentSmall A 32-bit signed integer.
     * @param ctx A PrecisionContext object.
     * @return A binary float with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the rounded result can't fit the given precision,
     * or if the context defines an exponent range and the given exponent is
     * outside that range.
     */
    public ExtendedFloat Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(
        ExtendedFloat.Create(BigInteger.ONE, BigInteger.valueOf(desiredExponentSmall)),
        ctx);
    }

    /**
     * Returns a binary float with the same value as this object but with the same
     * exponent as another binary float.
     * @param otherValue A binary float containing the desired exponent of the
     * result. The mantissa is ignored. The exponent is the number of
     * fractional digits in the result, expressed as a negative number. Can
     * also be positive, which eliminates lower-order places from the
     * number. For example, -3 means round to the thousandth (10^-3,
     * 0.0001), and 3 means round to the thousand (10^3, 1000). A value of 0
     * rounds the number to an integer.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A binary float with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent is
     * outside of the valid range of the precision context, if it defines an
     * exponent range.
     */
    public ExtendedFloat Quantize(
      ExtendedFloat otherValue,
      PrecisionContext ctx) {
      return MathValue.Quantize(this, otherValue, ctx);
    }

    /**
     * Returns a binary float with the same value as this object but rounded to an
     * integer.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A binary float with the same value as this object but rounded to an
     * integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent
     * must be changed to 0 when rounding and 0 is outside of the valid
     * range of the precision context, if it defines an exponent range.
     */
    public ExtendedFloat RoundToIntegralExact(
      PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    }

    /**
     * Returns a binary float with the same value as this object but rounded to an
     * integer, without adding the FlagInexact or FlagRounded flags.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags), except that this function will never add the
     * FlagRounded and FlagInexact flags (the only difference between this
     * and RoundToExponentExact). Can be null, in which case the default
     * rounding mode is HalfEven.
     * @return A binary float with the same value as this object but rounded to an
     * integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent
     * must be changed to 0 when rounding and 0 is outside of the valid
     * range of the precision context, if it defines an exponent range.
     */
    public ExtendedFloat RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return MathValue.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    }

    /**
     * Returns a binary float with the same value as this object but rounded to an
     * integer.
     * @param exponent A BigInteger object.
     * @param ctx A PrecisionContext object.
     * @return A binary float with the same value as this object but rounded to an
     * integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent is
     * outside of the valid range of the precision context, if it defines an
     * exponent range.
     */
    public ExtendedFloat RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, exponent, ctx);
    }

    /**
     * Returns a binary float with the same value as this object, and rounds it to
     * a new exponent if necessary.
     * @param exponent The minimum exponent the result can have. This is the
     * maximum number of fractional digits in the result, expressed as a
     * negative number. Can also be positive, which eliminates lower-order
     * places from the number. For example, -3 means round to the thousandth
     * (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
     * value of 0 rounds the number to an integer.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null, in which case the
     * default rounding mode is HalfEven.
     * @return A binary float rounded to the closest value representable in the
     * given precision, meaning if the result can't fit the precision,
     * additional digits are discarded to make it fit. Signals FlagInvalid
     * and returns NaN if the new exponent must be changed when rounding and
     * the new exponent is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public ExtendedFloat RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentSimple(this, exponent, ctx);
    }

    /**
     * Multiplies two binary floats. The resulting scale will be the sum of the
     * scales of the two binary floats. The result&apos;s sign is positive
     * if both operands have the same sign, and negative if they have
     * different signs.
     * @param op Another binary float.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The product of the two binary floats.
     */
    public ExtendedFloat Multiply(
      ExtendedFloat op,
      PrecisionContext ctx) {
      return MathValue.Multiply(this, op, ctx);
    }

    /**
     * Multiplies by one value, and then adds another value.
     * @param op The value to multiply.
     * @param augend The value to add.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The result thisValue * multiplicand + augend.
     */
    public ExtendedFloat MultiplyAndAdd(
      ExtendedFloat op,
      ExtendedFloat augend,
      PrecisionContext ctx) {
      return MathValue.MultiplyAndAdd(this, op, augend, ctx);
    }

    /**
     * Multiplies by one value, and then subtracts another value.
     * @param op The value to multiply.
     * @param subtrahend The value to subtract.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The result thisValue * multiplicand - subtrahend.
     * @throws NullPointerException The parameter "otherValue" is null.
     */
    public ExtendedFloat MultiplyAndSubtract(
      ExtendedFloat op,
      ExtendedFloat subtrahend,
      PrecisionContext ctx) {
      if (subtrahend == null) {
        throw new NullPointerException("subtrahend");
      }
      ExtendedFloat negated = subtrahend;
      if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          subtrahend.unsignedMantissa,
          subtrahend.exponent,
          newflags);
      }
      return MathValue.MultiplyAndAdd(this, op, negated, ctx);
    }

    /**
     * Rounds this object&apos;s value to a given precision, using the given
     * rounding mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode, and
     * exponent range. Can be null.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if {@code ctx} is
     * null or the precision and exponent range are unlimited.
     */
    public ExtendedFloat RoundToPrecision(
      PrecisionContext ctx) {
      return MathValue.RoundToPrecision(this, ctx);
    }

    /**
     * Rounds this object&apos;s value to a given precision, using the given
     * rounding mode and range of exponent, and also converts negative zero
     * to positive zero.
     * @param ctx A context for controlling the precision, rounding mode, and
     * exponent range. Can be null.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if {@code ctx} is
     * null or the precision and exponent range are unlimited.
     */
    public ExtendedFloat Plus(
      PrecisionContext ctx) {
      return MathValue.Plus(this, ctx);
    }

    /**
     * Rounds this object&apos;s value to a given maximum bit length, using the
     * given rounding mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode, and
     * exponent range. The precision is interpreted as the maximum bit
     * length of the mantissa. Can be null.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if {@code ctx} is
     * null or the precision and exponent range are unlimited.
     * @deprecated Instead of this method use RoundToPrecision and pass a precision context
* with the IsPrecisionInBits property set.
 */
@Deprecated
    public ExtendedFloat RoundToBinaryPrecision(
      PrecisionContext ctx) {
      if (ctx == null) {
        return this;
      }
      PrecisionContext ctx2 = ctx.Copy().WithPrecisionInBits(true);
      ExtendedFloat ret = MathValue.RoundToPrecision(this, ctx2);
      if (ctx2.getHasFlags()) {
        ctx.setFlags(ctx2.getFlags());
      }
      return ret;
    }

    /**
     * Finds the square root of this object&apos;s value.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as the square root function&apos;s results are generally not exact
     * for many inputs.--.
     * @return The square root. Signals the flag FlagInvalid and returns NaN if
     * this object is less than 0 (the square root would be a complex
     * number, but the return value is still NaN).
     * @throws IllegalArgumentException The parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0).
     */
    public ExtendedFloat SquareRoot(PrecisionContext ctx) {
      return MathValue.SquareRoot(this, ctx);
    }

    /**
     * Finds e (the base of natural logarithms) raised to the power of this
     * object&apos;s value.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as the exponential function&apos;s results are generally not
     * exact.--.
     * @return Exponential of this object. If this object's value is 1, returns an
     * approximation to " e" within the given precision.
     * @throws IllegalArgumentException The parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0).
     */
    public ExtendedFloat Exp(PrecisionContext ctx) {
      return MathValue.Exp(this, ctx);
    }

    /**
     * Finds the natural logarithm of this object, that is, the power (exponent)
     * that e (the base of natural logarithms) must be raised to in order to
     * equal this object&apos;s value.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as the ln function&apos;s results are generally not exact.--.
     * @return Ln(this object). Signals the flag FlagInvalid and returns NaN if
     * this object is less than 0 (the result would be a complex number with
     * a real part equal to Ln of this object's absolute value and an
     * imaginary part equal to pi, but the return value is still NaN.).
     * @throws IllegalArgumentException The parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0).
     */
    public ExtendedFloat Log(PrecisionContext ctx) {
      return MathValue.Ln(this, ctx);
    }

    /**
     * Finds the base-10 logarithm of this object, that is, the power (exponent)
     * that the number 10 must be raised to in order to equal this
     * object&apos;s value.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as the ln function&apos;s results are generally not exact.--.
     * @return Ln(this object)/Ln(10). Signals the flag FlagInvalid and returns NaN
     * if this object is less than 0. Signals FlagInvalid and returns NaN if
     * the parameter {@code ctx} is null or the precision is unlimited (the
     * context's Precision property is 0).
     */
    public ExtendedFloat Log10(PrecisionContext ctx) {
      return MathValue.Log10(this, ctx);
    }

    /**
     * Raises this object&apos;s value to the given exponent.
     * @param exponent An ExtendedFloat object.
     * @param ctx A PrecisionContext object.
     * @return This^exponent. Signals the flag FlagInvalid and returns NaN if this
     * object and exponent are both 0; or if this value is less than 0 and
     * the exponent either has a fractional part or is infinity.
     * @throws IllegalArgumentException The parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0), and the
     * exponent has a fractional part.
     */
    public ExtendedFloat Pow(ExtendedFloat exponent, PrecisionContext ctx) {
      return MathValue.Power(this, exponent, ctx);
    }

    /**
     * Raises this object&apos;s value to the given exponent.
     * @param exponentSmall A 32-bit signed integer.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags).
     * @return This^exponent. Signals the flag FlagInvalid and returns NaN if this
     * object and exponent are both 0.
     */
    public ExtendedFloat Pow(int exponentSmall, PrecisionContext ctx) {
      return this.Pow(ExtendedFloat.FromInt64(exponentSmall), ctx);
    }

    /**
     * Raises this object&apos;s value to the given exponent.
     * @param exponentSmall A 32-bit signed integer.
     * @return This^exponent. Returns NaN if this object and exponent are both 0.
     */
    public ExtendedFloat Pow(int exponentSmall) {
      return this.Pow(ExtendedFloat.FromInt64(exponentSmall), null);
    }

    /**
     * Finds the constant pi.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as pi can never be represented exactly.--.
     * @return Pi rounded to the given precision.
     * @throws IllegalArgumentException The parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0).
     */
    public static ExtendedFloat PI(PrecisionContext ctx) {
      return MathValue.Pi(ctx);
    }
  }
