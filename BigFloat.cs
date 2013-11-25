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
    /// <returns></returns>
    /// <param name='obj'> A BigFloat object.</param>
    public bool Equals(BigFloat obj) {
      BigFloat other = obj as BigFloat;
      if (other == null)
        return false;
      return this.exponent.Equals(other.exponent) &&
        this.mantissa.Equals(other.mantissa);
    }
    /// <summary> Determines whether this object's mantissa and exponent
    /// are equal to those of another object and that object is a Bigfloat.
    /// </summary>
    /// <returns> True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'> A Object object.</param>
    public override bool Equals(object obj) {
      return Equals(obj as BigFloat);
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
    /// <summary> Creates a bigfloat with the value exponentLong*2^mantissa.
    /// </summary>
    /// <param name='mantissa'> The unscaled value.</param>
    /// <param name='exponentLong'> The binary exponent.</param>
    public BigFloat(BigInteger mantissa, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = mantissa;
    }
    /// <summary> Creates a bigfloat with the given mantissa and an exponent
    /// of 0. </summary>
    /// <param name='mantissa'> The desired value of the bigfloat</param>
    public BigFloat(BigInteger mantissa) {
      this.exponent = BigInteger.Zero;
      this.mantissa = mantissa;
    }
    /// <summary> Creates a bigfloat with the given mantissa and an exponent
    /// of 0. </summary>
    /// <param name='mantissaLong'> The desired value of the bigfloat</param>
    public BigFloat(long mantissaLong) {
      this.exponent = BigInteger.Zero;
      this.mantissa = (BigInteger)mantissaLong;
    }
    private static BigInteger FastMaxExponent = (BigInteger)2000;
    private static BigInteger FastMinExponent = (BigInteger)(-2000);
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
    private static BigInteger ShiftLeft(BigInteger val, long shift) {
      while (shift > ShiftIteration) {
        val <<= 1000000;
        shift -= ShiftIteration;
      }
      int lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }

    private static BigInteger
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
        BigInteger diff = BigInteger.Abs(e1 - (BigInteger)e2);
        mantissa = ShiftLeft(mantissa, diff);
      }
      if (negative) mantissa = -mantissa;
      return mantissa;
    }
    /// <summary> Gets an object with the same value as this one, but with the
    /// sign reversed. </summary>
    /// <returns></returns>
    public BigFloat Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new BigFloat(neg, this.exponent);
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

    private static bool Round(ShiftAccumulator accum, Rounding rounding,
                              bool neg, int lastDigit) {
      bool incremented = false;
      if (rounding == Rounding.HalfUp) {
        if (accum.BitLeftmost != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.BitLeftmost != 0) {
          if (accum.BitsAfterLeftmost != 0 || lastDigit != 0) {
            incremented = true;
          }
        }
      } else if (rounding == Rounding.Ceiling) {
        if (!neg && (accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Floor) {
        if (neg && (accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfDown) {
        if (accum.BitLeftmost != 0 && accum.BitsAfterLeftmost != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up ||
                rounding == Rounding.ZeroFiveUp) {
        // NOTE: For binary floating point, ZeroFiveUp
        // is the same as Up except in overflow situations
        if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          incremented = true;
        }
      }
      return incremented;
    }

    /// <summary> Rounds this object's value to a given precision, using
    /// the given rounding mode and range of exponent. </summary>
    /// <param name='context'> A context for controlling the precision,
    /// rounding mode, and exponent range.</param>
    /// <returns> The closest value to this object's value, rounded to the
    /// specified precision. Returns null if the result of the rounding would
    /// cause an overflow. The caller can handle a null return value by treating
    /// it as positive or negative infinity depending on the sign of this object's
    /// value.</returns>
    /// <exception cref='System.ArgumentException'> "precision" is
    /// null.</exception>
    public BigFloat RoundToPrecision(
      PrecisionContext context
     ) {
      if ((context) == null) throw new ArgumentNullException("context");
      if (context.Precision > 0 && context.Precision <= 18) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = BigInteger.Abs(this.mantissa);
        if (mantabs.CompareTo(ShiftLeft(BigInteger.One, context.Precision)) < 0) {
          FastInteger fastExp = new FastInteger(this.exponent)
            .Add(context.Precision).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(context.EMin)
            .Add(context.Precision).Subtract(1);
          if (fastExp.CompareTo(new FastInteger(context.EMax)) <= 0 &&
             fastExp.CompareTo(fastNormalMin) >= 0) {
            return this;
          }
        }
      }
      int[] signals = new int[1];
      BigFloat dfrac = RoundToPrecision(
        context.Precision,
        context.Rounding,
        context.EMin,
        context.EMax, signals);
      if (context.ClampNormalExponents && dfrac != null) {
        dfrac = dfrac.ClampExponent(context.Precision, context.EMax, signals);
      }
      if (context.HasFlags) {
        context.Flags |= signals[0];
      }
      return dfrac;
    }

    private BigFloat ClampExponent(long precision, BigInteger eMax, int[] signals) {
      if (signals != null && signals.Length == 0)
        throw new ArgumentException("signals has zero length");
      FastInteger exp = new FastInteger(this.exponent);
      FastInteger clamp = new FastInteger(eMax).Add(1).Subtract(precision);
      if (exp.CompareTo(clamp) > 0) {
        BigInteger bigmantissa = this.mantissa;
        int sign = bigmantissa.Sign;
        if (sign != 0) {
          if (sign < 0) bigmantissa = -bigmantissa;
          FastInteger expdiff = new FastInteger(exp).Subtract(clamp);
          if (expdiff.CanFitInInt64()) {
            bigmantissa = (ShiftLeft(bigmantissa, expdiff.AsInt64()));
          } else {
            bigmantissa = (ShiftLeft(bigmantissa, expdiff.AsBigInteger()));
          }
          if (sign < 0) bigmantissa = -bigmantissa;
        }
        if (signals != null)
          signals[0] |= PrecisionContext.SignalClamped;
        return new BigFloat(bigmantissa, clamp.AsBigInteger());
      }
      return this;
    }

    private BigFloat RoundToPrecision(
      long precision,
      Rounding rounding,
      BigInteger eMin,
      BigInteger eMax,
      int[] signals
     ) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to 1 (" + Convert.ToString((long)(precision)) + ")");
      if (eMin.CompareTo(eMax) > 0) throw new ArgumentException("eMin greater than eMax");
      if (signals != null && signals.Length == 0)
        throw new ArgumentException("signals has zero length");
      bool neg = this.mantissa.Sign < 0;
      int lastDigit = 0;
      BigInteger bigmantissa = this.mantissa;
      if (neg) bigmantissa = -bigmantissa;
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      ShiftAccumulator accum = new ShiftAccumulator(bigmantissa);
      FastInteger exp = new FastInteger(this.exponent);
      FastInteger fastEMin = new FastInteger(eMin);
      FastInteger fastEMax = new FastInteger(eMax);
      int flags = 0;
      bool unlimitedPrec = (precision == 0);
      if (precision == 0) {
        precision = accum.KnownBitLength;
      } else {
        accum.ShiftToBits(precision);
      }
      FastInteger discardedBits = new FastInteger(accum.DiscardedBitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp);
      adjExponent.Add(accum.KnownBitLength).Subtract(1);
      FastInteger clamp = null;
      if (adjExponent.CompareTo(fastEMax) > 0) {
        if (oldmantissa.IsZero) {
          flags |= PrecisionContext.SignalClamped;
          if (signals != null) signals[0] = flags;
          return new BigFloat(oldmantissa, fastEMax.AsBigInteger());
        }
        // Overflow
        flags |= PrecisionContext.SignalOverflow | PrecisionContext.SignalInexact | PrecisionContext.SignalRounded;
        if (!unlimitedPrec &&
           (rounding == Rounding.Down ||
           rounding == Rounding.ZeroFiveUp ||
           (rounding == Rounding.Ceiling && neg) ||
           (rounding == Rounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          BigInteger overflowMant = BigInteger.One;
          overflowMant = ShiftLeft(overflowMant, precision);
          overflowMant -= BigInteger.One;
          if (neg) overflowMant = -overflowMant;
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return new BigFloat(overflowMant, clamp.AsBigInteger());
        }
        if (signals != null) signals[0] = flags;
        return null;
      } else if (adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = new FastInteger(fastEMin)
          .Subtract(precision)
          .Add(1);
        if (!oldmantissa.IsZero)
          flags |= PrecisionContext.SignalSubnormal;
        if (exp.CompareTo(fastETiny) < 0) {
          FastInteger expdiff = new FastInteger(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = new ShiftAccumulator(oldmantissa);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.ShiftedInt;
          if ((accum.DiscardedBitCount).Sign != 0) {
            if (!oldmantissa.IsZero)
              flags |= PrecisionContext.SignalRounded;
            lastDigit = 0;
            if (rounding == Rounding.HalfEven ||
               rounding == Rounding.ZeroFiveUp) {
              lastDigit = (int)(newmantissa.IsEven ? 0 : 1);
            }
            if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
              flags |= PrecisionContext.SignalInexact;
            }
            if (Round(accum, rounding, neg, lastDigit)) {
              newmantissa += BigInteger.One;
            }
          }
          if (newmantissa.IsZero)
            flags |= PrecisionContext.SignalClamped;
          if ((flags & (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact)) == (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact))
            flags |= PrecisionContext.SignalUnderflow | PrecisionContext.SignalRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa = -newmantissa;
          return new BigFloat(newmantissa, fastETiny.AsBigInteger());
        }
      }
      bool expChanged = false;
      lastDigit = 0;
      if ((accum.DiscardedBitCount).Sign != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.SignalRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          flags |= PrecisionContext.SignalInexact;
        }
      }
      if (rounding == Rounding.HalfEven ||
         rounding == Rounding.ZeroFiveUp) {
        lastDigit = (int)(bigmantissa.IsEven ? 0 : 1);
      }
      if (Round(accum, rounding, neg, lastDigit)) {
        bigmantissa += BigInteger.One;
        accum = new ShiftAccumulator(bigmantissa);
        accum.ShiftToBits(precision);
        if ((accum.DiscardedBitCount).Sign != 0) {
          exp.Add(accum.DiscardedBitCount);
          discardedBits.Add(accum.DiscardedBitCount);
          bigmantissa = accum.ShiftedInt;
          expChanged = true;
        }
      }
      if (expChanged) {
        // If exponent changed, check for overflow again
        adjExponent = new FastInteger(exp);
        adjExponent.Add(accum.KnownBitLength).Subtract(1);
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= PrecisionContext.SignalOverflow | PrecisionContext.SignalInexact | PrecisionContext.SignalRounded;
          if (!unlimitedPrec &&
             (rounding == Rounding.Down ||
             rounding == Rounding.ZeroFiveUp ||
             (rounding == Rounding.Ceiling && neg) ||
             (rounding == Rounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            BigInteger overflowMant = BigInteger.One;
            while (precision > 0) {
              int tmp = (int)Math.Min(9999999, precision);
              overflowMant <<= tmp;
              precision -= tmp;
            }
            overflowMant -= BigInteger.One;
            if (neg) overflowMant = -overflowMant;
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return new BigFloat(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa = -bigmantissa;
      return new BigFloat(bigmantissa, exp.AsBigInteger());
    }
    /// <summary> Gets the greater value between two BigFloat values. If
    /// both values are equal, returns "a". </summary>
    /// <returns> The larger value of the two objects.</returns>
    /// <param name='a'> A BigFloat object.</param>
    /// <param name='b'> A BigFloat object.</param>
    public static BigFloat Max(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = a.CompareTo(b);
      if (cmp != 0)
        return cmp > 0 ? a : b;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (a.Sign >= 0) {
        return (a.Exponent).CompareTo(b.Exponent) > 0 ? a : b;
      } else {
        return (a.Exponent).CompareTo(b.Exponent) > 0 ? b : a;
      }
    }
    /// <summary> Gets the lesser value between two BigFloat values. If both
    /// values are equal, returns "a". </summary>
    /// <returns> The smaller value of the two objects.</returns>
    /// <param name='a'> A BigFloat object.</param>
    /// <param name='b'> A BigFloat object.</param>
    public static BigFloat Min(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = a.CompareTo(b);
      if (cmp != 0)
        return cmp > 0 ? b : a;
      // Here the signs of both a and b can only be
      // equal (negative zeros are not supported)
      if (a.Sign >= 0) {
        return (a.Exponent).CompareTo(b.Exponent) > 0 ? b : a;
      } else {
        return (a.Exponent).CompareTo(b.Exponent) > 0 ? a : b;
      }
    }

    /// <summary> Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'> A BigFloat object.</param>
    /// <param name='b'> A BigFloat object.</param>
    public static BigFloat MinMagnitude(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = a.Abs().CompareTo(b.Abs());
      if (cmp == 0) return Min(a, b);
      return (cmp < 0) ? a : b;
    }
    /// <summary> Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'> A BigFloat object.</param>
    /// <param name='b'> A BigFloat object.</param>
    public static BigFloat MaxMagnitude(BigFloat a, BigFloat b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = a.Abs().CompareTo(b.Abs());
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /// <summary> Finds the sum of this object and another bigfloat. The result's
    /// exponent is set to the lower of the exponents of the two operands. </summary>
    /// <returns></returns>
    /// <param name='decfrac'> A BigFloat object.</param>
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
    /// <summary> Finds the difference between this object and another bigfloat.
    /// The result's exponent is set to the lower of the exponents of the two
    /// operands. </summary>
    /// <returns> The difference of the two objects.</returns>
    /// <param name='decfrac'> A BigFloat object.</param>
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
    /// <summary> Multiplies two bigfloats. The resulting scale will be
    /// the sum of the scales of the two bigfloats. </summary>
    /// <param name='decfrac'> Another bigfloat.</param>
    /// <returns> The product of the two bigfloats.</returns>
    public BigFloat Multiply(BigFloat decfrac) {
      BigInteger newexp = (this.exponent + (BigInteger)decfrac.exponent);
      return new BigFloat(
        mantissa * (BigInteger)decfrac.mantissa, newexp);
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
    /// <summary> Compares the mathematical values of two bigfloats. <para>
    /// This method is not consistent with the Equals method because two different
    /// bigfloats with the same mathematical value, but different exponents,
    /// will compare as equal.</para>
    /// </summary>
    /// <param name='other'> Another bigfloat.</param>
    /// <returns> Less than 0 if this value is less than the other value, or
    /// greater than 0 if this value is greater than the other value or if "other"
    /// is null, or 0 if both values are equal.</returns>
    public int CompareTo(BigFloat other) {
      if (other == null) return 1;
      int s = this.Sign;
      int ds = other.Sign;
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.CompareTo((BigInteger)other.exponent);
      int mantcmp = mantissa.CompareTo((BigInteger)other.mantissa);
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
      }
      if (expcmp == 0) {
        return mantcmp;
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

    /// <summary> Creates a bigfloat from an arbitrary-precision decimal
    /// fraction. Note that if the decimal fraction contains a negative exponent,
    /// the resulting value might not be exact. </summary>
    /// <returns></returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
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
        bigmantissa *= (BigInteger)(DecimalFraction.FindPowerOfTenFromBig(bigintExp));
        return new BigFloat(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = (bigmantissa.Sign < 0);
        BigInteger remainder;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        FastInteger negscale = new FastInteger(scale).Negate();
        BigInteger divisor = DecimalFraction.FindPowerOfFiveFromBig(
          negscale.AsBigInteger());
        while (true) {
          BigInteger quotient = BigInteger.DivRem(bigmantissa, divisor, out remainder);
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (!remainder.IsZero &&
             quotient.CompareTo(OneShift62) < 0) {
            long smallquot = (long)quotient;
            int shift = 0;
            while (smallquot != 0 && smallquot < (1L << 62)) {
              smallquot <<= 1;
              shift++;
            }
            if (shift == 0) shift = 1;
            bigmantissa <<= shift;
            scale.Add(-shift);
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
      return new BigFloat((BigInteger)((long)fpMantissa), fpExponent - 150);
    }
    /// <summary> Creates a bigfloat from a 64-bit floating-point number.
    /// </summary>
    /// <param name='dbl'> A 64-bit floating-point number.</param>
    /// <returns> A bigfloat with the same value as "dbl"</returns>
    /// <exception cref='OverflowException'> "dbl" is infinity or not-a-number.</exception>
    public static BigFloat FromDouble(double dbl) {
      long value = BitConverter.ToInt64(BitConverter.GetBytes((double)dbl), 0);
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
    /// <summary> Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer. </summary>
    /// <returns></returns>
    public BigInteger ToBigInteger() {
      if (this.Exponent == 0) {
        // Integer
        return this.Mantissa;
      } else if (this.Exponent > 0) {
        // Integer with trailing zeros
        BigInteger curexp = this.Exponent;
        BigInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero)
          return bigmantissa;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -bigmantissa;
        while (curexp > 0 && !bigmantissa.IsZero) {
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
        while (curexp < 0 && !bigmantissa.IsZero) {
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

    private static BigInteger OneShift23 = (BigInteger)(1L << 23);
    private static BigInteger OneShift52 = (BigInteger)(1L << 52);
    private static BigInteger OneShift62 = (BigInteger)(1L << 62);

    /// <summary> Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used. </summary>
    /// <returns> The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      BigInteger bigmant = BigInteger.Abs(this.mantissa);
      FastInteger bigexponent = new FastInteger(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.IsZero) {
        return 0.0f;
      }
      long smallmant = 0;
      if (bigmant.CompareTo(OneShift23) < 0) {
        smallmant = (long)bigmant;
        int exponentchange = 0;
        while (smallmant < (1L << 23)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        ShiftAccumulator accum = new ShiftAccumulator(bigmant);
        accum.ShiftToBits(24);
        bitsAfterLeftmost = accum.BitsAfterLeftmost;
        bitLeftmost = accum.BitLeftmost;
        bigexponent.Add(accum.DiscardedBitCount);
        smallmant = accum.ShiftedIntSmall;
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
        smallmant++;
        if (smallmant == (1 << 24)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareTo(104) > 0) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Single.NegativeInfinity :
          Single.PositiveInfinity;
      } else if (bigexponent.CompareTo(-149) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        ShiftAccumulator accum = new ShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.BitsAfterLeftmost;
        bitLeftmost = accum.BitLeftmost;
        bigexponent.Add(accum.DiscardedBitCount);
        smallmant = accum.ShiftedIntSmall;
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
          smallmant++;
          if (smallmant == (1 << 24)) {
            smallmant >>= 1;
            bigexponent.Add(1);
          }
        }
      }
      if (bigexponent.CompareTo(-149) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0) :
          BitConverter.ToSingle(BitConverter.GetBytes((int)0), 0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent = smallexponent + 150;
        int smallmantissa = ((int)smallmant) & 0x7FFFFF;
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
      FastInteger bigexponent = new FastInteger(this.exponent);
      int bitLeftmost = 0;
      int bitsAfterLeftmost = 0;
      if (this.mantissa.IsZero) {
        return 0.0d;
      }
      long smallmant = 0;
      if (bigmant.CompareTo(OneShift52) < 0) {
        smallmant = (long)bigmant;
        int exponentchange = 0;
        while (smallmant < (1L << 52)) {
          smallmant <<= 1;
          exponentchange++;
        }
        bigexponent.Subtract(exponentchange);
      } else {
        ShiftAccumulator accum = new ShiftAccumulator(bigmant);
        accum.ShiftToBits(53);
        bitsAfterLeftmost = accum.BitsAfterLeftmost;
        bitLeftmost = accum.BitLeftmost;
        bigexponent.Add(accum.DiscardedBitCount);
        smallmant = accum.ShiftedIntSmall;
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
        smallmant++;
        if (smallmant == (1 << 53)) {
          smallmant >>= 1;
          bigexponent.Add(1);
        }
      }
      bool subnormal = false;
      if (bigexponent.CompareTo(971) > 0) {
        // exponent too big
        return (this.mantissa.Sign < 0) ?
          Double.NegativeInfinity :
          Double.PositiveInfinity;
      } else if (bigexponent.CompareTo(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        ShiftAccumulator accum = new ShiftAccumulator(smallmant);
        FastInteger fi = new FastInteger(bigexponent).Subtract(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.BitsAfterLeftmost;
        bitLeftmost = accum.BitLeftmost;
        bigexponent.Add(accum.DiscardedBitCount);
        smallmant = accum.ShiftedIntSmall;
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 || (smallmant & 1) != 0)) {
          smallmant++;
          if (smallmant == (1 << 53)) {
            smallmant >>= 1;
            bigexponent.Add(1);
          }
        }
      }
      if (bigexponent.CompareTo(-1074) < 0) {
        // exponent too small, so return zero
        return (this.mantissa.Sign < 0) ?
          BitConverter.ToDouble(BitConverter.GetBytes((long)1L << 63), 0) :
          BitConverter.ToDouble(BitConverter.GetBytes((long)0), 0);
      } else {
        long smallexponent = bigexponent.AsInt64();
        smallexponent = smallexponent + 1075;
        long smallmantissa = smallmant & 0xFFFFFFFFFFFFFL;
        if (!subnormal) {
          smallmantissa |= (smallexponent << 52);
        }
        if (this.mantissa.Sign < 0) smallmantissa |= (1L << 63);
        return BitConverter.ToDouble(BitConverter.GetBytes((long)smallmantissa), 0);
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
    /// part.The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method. </summary>
    /// <returns></returns>
    public string ToPlainString() {
      return DecimalFraction.FromBigFloat(this).ToPlainString();
    }
  }
}