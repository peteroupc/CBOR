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

    /// <summary> Represents an arbitrary-precision decimal floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number is equal to mantissa
    /// * 10^exponent. </summary>
  public sealed class DecimalFraction : IComparable<DecimalFraction>, IEquatable<DecimalFraction> {
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
    /// <param name='obj'> A DecimalFraction object.</param>
    public bool Equals(DecimalFraction obj) {
      DecimalFraction other = obj as DecimalFraction;
      if (other == null)
        return false;
      return this.exponent.Equals(other.exponent) &&
        this.mantissa.Equals(other.mantissa);
    }
    /// <summary> Determines whether this object's mantissa and exponent
    /// are equal to those of another object and that other object is a decimal
    /// fraction. </summary>
    /// <returns> True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'> A Object object.</param>
    public override bool Equals(object obj) {
      return Equals(obj as DecimalFraction);
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
    /// <summary> Creates a decimal fraction with the value exponent*10^mantissa.
    /// </summary>
    /// <param name='mantissa'> The unscaled value.</param>
    /// <param name='exponent'> The decimal exponent.</param>
    public DecimalFraction(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }
    /// <summary> Creates a decimal fraction with the value exponentLong*10^mantissa.
    /// </summary>
    /// <param name='mantissa'> The unscaled value.</param>
    /// <param name='exponentLong'> The decimal exponent.</param>
    public DecimalFraction(BigInteger mantissa, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = mantissa;
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissa'> The desired value of the bigfloat</param>
    public DecimalFraction(BigInteger mantissa) {
      this.exponent = BigInteger.Zero;
      this.mantissa = mantissa;
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissaLong'> The desired value of the bigfloat</param>
    public DecimalFraction(long mantissaLong) {
      this.exponent = BigInteger.Zero;
      this.mantissa = (BigInteger)mantissaLong;
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissaLong'> The unscaled value.</param>
    /// <param name='exponentLong'> The decimal exponent.</param>
    public DecimalFraction(long mantissaLong, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = (BigInteger)mantissaLong;
    }

    /// <summary> Creates a decimal fraction from a string that represents
    /// a number. <para> The format of the string generally consists of:<list
    /// type=''> <item> An optional '-' or '+' character (if '-', the value
    /// is negative.)</item>
    /// <item> One or more digits, with a single optional decimal point after
    /// the first digit and before the last digit.</item>
    /// <item> Optionally, E+ (positive exponent) or E- (negative exponent)
    /// plus one or more digits specifying the exponent.</item>
    /// </list>
    /// </para>
    /// <para> The format generally follows the definition in java.math.BigDecimal(),
    /// except that the digits must be ASCII digits ('0' through '9').</para>
    /// </summary>
    /// <param name='s'> A string that represents a number.</param>
    /// <returns></returns>
    public static DecimalFraction FromString(String s) {
      if (s == null)
        throw new ArgumentNullException("s");
      if (s.Length == 0)
        throw new FormatException();
      int offset = 0;
      bool negative = false;
      if (s[0] == '+' || s[0] == '-') {
        negative = (s[0] == '-');
        offset++;
      }
      FastInteger2 mant = new FastInteger2();
      bool haveDecimalPoint = false;
      bool haveDigits = false;
      bool haveExponent = false;
      FastInteger newScale = new FastInteger();
      int i = offset;
      for (; i < s.Length; i++) {
        if (s[i] >= '0' && s[i] <= '9') {
          int thisdigit = (int)(s[i] - '0');
          mant.MultiplyByTen();
          mant.Add(thisdigit);
          haveDigits = true;
          if (haveDecimalPoint) {
            newScale.Add(-1);
          }
        } else if (s[i] == '.') {
          if (haveDecimalPoint)
            throw new FormatException();
          haveDecimalPoint = true;
        } else if (s[i] == 'E' || s[i] == 'e') {
          haveExponent = true;
          i++;
          break;
        } else {
          throw new FormatException();
        }
      }
      if (!haveDigits)
        throw new FormatException();
      if (haveExponent) {
        FastInteger2 exp = new FastInteger2();
        offset = 1;
        haveDigits = false;
        if (i == s.Length) throw new FormatException();
        if (s[i] == '+' || s[i] == '-') {
          if (s[i] == '-') offset = -1;
          i++;
        }
        for (; i < s.Length; i++) {
          if (s[i] >= '0' && s[i] <= '9') {
            haveDigits = true;
            int thisdigit = (int)(s[i] - '0');
            exp.MultiplyByTen();
            exp.Add(thisdigit);
          } else {
            throw new FormatException();
          }
        }
        if (!haveDigits)
          throw new FormatException();
        if (offset < 0)
          exp.SubtractThisFrom(newScale);
        else
          exp.AddThisTo(newScale);
      } else if (i != s.Length) {
        throw new FormatException();
      }
      return new DecimalFraction(
        (negative) ? mant.AsNegatedBigInteger() :
        mant.AsBigInteger(),
        newScale.AsBigInteger());
    }
    private static BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      bool negative = (mantissa.Sign < 0);
      if (mantissa.Sign == 0) return BigInteger.Zero;
      if (negative) mantissa = -mantissa;
      FastInteger diff = new FastInteger(e1).Subtract(e2).Abs();
      if (diff.CanFitInInt64()) {
        mantissa *= (BigInteger)(FindPowerOfTen(diff.AsInt64()));
      } else {
        mantissa *= (BigInteger)(FindPowerOfTenFromBig(diff.AsBigInteger()));
      }
      if (negative) mantissa = -mantissa;
      return mantissa;
    }
    /// <summary> Gets an object with the same value as this one, but with the
    /// sign reversed. </summary>
    /// <returns></returns>
    public DecimalFraction Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new DecimalFraction(neg, this.exponent);
    }
    /// <summary> Gets the absolute value of this object. </summary>
    /// <returns></returns>
    public DecimalFraction Abs() {
      if (this.Sign < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /// <summary> Gets the lesser value between two DecimalFraction values.
    /// If both values are equal, returns "a". </summary>
    /// <returns> The smaller value of the two objects.</returns>
    /// <param name='a'> A DecimalFraction object.</param>
    /// <param name='b'> A DecimalFraction object.</param>
    public static DecimalFraction Min(DecimalFraction a, DecimalFraction b) {
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
    /// <param name='a'> A DecimalFraction object.</param>
    /// <param name='b'> A DecimalFraction object.</param>
    public static DecimalFraction MinMagnitude(DecimalFraction a, DecimalFraction b) {
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
    /// <param name='a'> A DecimalFraction object.</param>
    /// <param name='b'> A DecimalFraction object.</param>
    public static DecimalFraction MaxMagnitude(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      int cmp = a.Abs().CompareTo(b.Abs());
      if (cmp == 0) return Max(a, b);
      return (cmp > 0) ? a : b;
    }
    /// <summary> Gets the greater value between two DecimalFraction values.
    /// If both values are equal, returns "a". </summary>
    /// <returns> The larger value of the two objects.</returns>
    /// <param name='a'> A DecimalFraction object.</param>
    /// <param name='b'> A DecimalFraction object.</param>
    public static DecimalFraction Max(DecimalFraction a, DecimalFraction b) {
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
    internal static BigInteger FindPowerOfFiveFromBig(BigInteger diff) {
      if (diff.Sign <= 0) return BigInteger.One;
      BigInteger bigpow = BigInteger.Zero;
      FastInteger intcurexp = new FastInteger(diff);
      if (intcurexp.CompareTo(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa = BigInteger.One;
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareTo(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareTo(9999999) <= 0) {
          bigpow = BigInteger.Pow(FindPowerOfFive(1), intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = BigInteger.Pow(FindPowerOfFive(1), 9999999);
          mantissa *= bigpow;
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }

    private static BigInteger BigInt36 = (BigInteger)36;

    internal static BigInteger FindPowerOfTenFromBig(BigInteger bigintExponent) {
      if (bigintExponent.Sign <= 0) return BigInteger.One;
      if (bigintExponent.CompareTo(BigInt36) <= 0) {
        return FindPowerOfTen((int)bigintExponent);
      }
      FastInteger intcurexp = new FastInteger(bigintExponent);
      BigInteger mantissa = BigInteger.One;
      BigInteger bigpow = BigInteger.Zero;
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareTo(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareTo(9999999) <= 0) {
          int val = intcurexp.AsInt32();
          bigpow = FindPowerOfFive(val);
          bigpow <<= val;
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if (bigpow.IsZero) {
            bigpow = FindPowerOfFive(9999999);
            bigpow <<= 9999999;
          }
          mantissa *= bigpow;
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }
    internal static BigInteger FindPowerOfFive(long precision) {
      if (precision <= 0) return BigInteger.One;
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 27)
        return BigIntPowersOfFive[(int)precision];
      if (precision <= 54) {
        ret = BigIntPowersOfFive[27];
        bigpow = BigIntPowersOfFive[((int)precision) - 27];
        ret *= (BigInteger)bigpow;
        return ret;
      }
      ret = BigInteger.One;
      bool first = true;
      bigpow = BigInteger.Zero;
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = BigIntPowersOfFive[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else if (precision <= 9999999) {
          bigpow = BigInteger.Pow(BigIntPowersOfFive[1], (int)precision);
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = BigInteger.Pow(BigIntPowersOfFive[1], 9999999);
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          precision -= 9999999;
        }
      }
      return ret;
    }

    internal static BigInteger FindPowerOfTen(long precision) {
      if (precision <= 0) return BigInteger.One;
      BigInteger ret;
      BigInteger bigpow;
      if (precision <= 18)
        return BigIntPowersOfTen[(int)precision];
      if (precision <= 27) {
        int prec = (int)precision;
        ret = BigIntPowersOfFive[prec];
        ret <<= prec;
        return ret;
      }
      if (precision <= 36) {
        ret = BigIntPowersOfTen[18];
        bigpow = BigIntPowersOfTen[((int)precision) - 18];
        ret *= (BigInteger)bigpow;
        return ret;
      }
      ret = BigInteger.One;
      bool first = true;
      bigpow = BigInteger.Zero;
      while (precision > 0) {
        if (precision <= 18) {
          bigpow = BigIntPowersOfTen[(int)precision];
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else if (precision <= 9999999) {
          int prec = (int)precision;
          bigpow = FindPowerOfFive(prec);
          bigpow <<= prec;
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          break;
        } else {
          if (bigpow.IsZero)
            bigpow = BigInteger.Pow(BigIntPowersOfTen[1], 9999999);
          if (first)
            ret = bigpow;
          else
            ret *= (BigInteger)bigpow;
          first = false;
          precision -= 9999999;
        }
      }
      return ret;
    }

    private static bool Round(DigitShiftAccumulator accum, Rounding rounding,
                              bool neg, BigInteger bigval) {
      bool incremented = false;
      if (rounding == Rounding.HalfUp) {
        if (accum.BitLeftmost >= 5) {
          incremented = true;
        }
      } else if (rounding == Rounding.HalfEven) {
        if (accum.BitLeftmost >= 5) {
          if ((accum.BitLeftmost > 5 || accum.BitsAfterLeftmost != 0)) {
            incremented = true;
          } else if (!bigval.IsEven) {
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
        if (accum.BitLeftmost > 5 || (accum.BitLeftmost == 5 && accum.BitsAfterLeftmost != 0)) {
          incremented = true;
        }
      } else if (rounding == Rounding.Up) {
        if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          incremented = true;
        }
      } else if (rounding == Rounding.ZeroFiveUp) {
        if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          BigInteger bigdigit = bigval % (BigInteger)10;
          int lastDigit = (int)bigdigit;
          if (lastDigit == 0 || lastDigit == 5) {
            incremented = true;
          }
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
    public DecimalFraction RoundToPrecision(
      PrecisionContext context
     ) {
      if ((context) == null) throw new ArgumentNullException("context");
      FastInteger fastExp = new FastInteger(this.exponent);
      FastInteger fastEMin = new FastInteger(context.EMin);
      FastInteger fastEMax = new FastInteger(context.EMax);
      if (context.Precision > 0 && context.Precision <= 18) {
        // Check if rounding is necessary at all
        // for small precisions
        BigInteger mantabs = BigInteger.Abs(this.mantissa);
        if (mantabs.CompareTo(FindPowerOfTen(context.Precision)) < 0) {
          FastInteger fastAdjustedExp = new FastInteger(fastExp)
            .Add(context.Precision).Subtract(1);
          FastInteger fastNormalMin = new FastInteger(fastEMin)
            .Add(context.Precision).Subtract(1);
          if (fastAdjustedExp.CompareTo(new FastInteger(fastEMax)) <= 0 &&
             fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
            return this;
          }
        }
      }
      int[] signals = new int[1];
      DecimalFraction dfrac = RoundToPrecision(
        context.Precision,
        context.Rounding, fastEMin, fastEMax, signals);
      if (context.ClampNormalExponents && dfrac != null) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        FastInteger clamp = new FastInteger(fastEMax).Add(1).Subtract(context.Precision);
        fastExp = new FastInteger(dfrac.Exponent);
        if (fastExp.CompareTo(clamp) > 0) {
          BigInteger bigmantissa = dfrac.mantissa;
          int sign = bigmantissa.Sign;
          if (sign != 0) {
            if (sign < 0) bigmantissa = -bigmantissa;
            FastInteger expdiff = new FastInteger(fastExp).Subtract(clamp);
            if (expdiff.CanFitInInt64()) {
              bigmantissa *= (BigInteger)(FindPowerOfTen(expdiff.AsInt64()));
            } else {
              bigmantissa *= (BigInteger)(FindPowerOfTenFromBig(expdiff.AsBigInteger()));
            }
            if (sign < 0) bigmantissa = -bigmantissa;
          }
          if (signals != null)
            signals[0] |= PrecisionContext.SignalClamped;
          dfrac = new DecimalFraction(bigmantissa, clamp.AsBigInteger());
        }
      }
      if (context.HasFlags) {
        context.Flags |= signals[0];
      }
      return dfrac;
    }

    private DecimalFraction RoundToPrecision(
      long precision,
      Rounding rounding,
      FastInteger fastEMin,
      FastInteger fastEMax,
      int[] signals
     ) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(precision)) + ")");
      bool neg = this.mantissa.Sign < 0;
      BigInteger bigmantissa = this.mantissa;
      if (neg) bigmantissa = -bigmantissa;
      // save mantissa in case result is subnormal
      // and must be rounded again
      BigInteger oldmantissa = bigmantissa;
      FastInteger exp = new FastInteger(this.exponent);
      int flags = 0;
      DigitShiftAccumulator accum = new DigitShiftAccumulator(bigmantissa);
      bool unlimitedPrec = (precision == 0);
      if (precision > 0) {
        accum.ShiftToBits(precision);
      } else {
        precision = accum.KnownBitLength;
      }
      FastInteger discardedBits = new FastInteger(accum.DiscardedBitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = new FastInteger(exp)
        .Add(accum.KnownBitLength).Subtract(1);
      FastInteger clamp = null;
      if (adjExponent.CompareTo(fastEMax) > 0) {
        if (oldmantissa.IsZero) {
          flags |= PrecisionContext.SignalClamped;
          if (signals != null) signals[0] = flags;
          return new DecimalFraction(oldmantissa, fastEMax.AsBigInteger());
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
          BigInteger overflowMant = FindPowerOfTen(precision);
          overflowMant -= BigInteger.One;
          if (neg) overflowMant = -overflowMant;
          if (signals != null) signals[0] = flags;
          clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
          return new DecimalFraction(overflowMant, clamp.AsBigInteger());
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
          accum = new DigitShiftAccumulator(oldmantissa);
          accum.ShiftRight(expdiff);
          BigInteger newmantissa = accum.ShiftedInt;
          if ((accum.DiscardedBitCount).Sign != 0) {
            if (!oldmantissa.IsZero)
              flags |= PrecisionContext.SignalRounded;
            if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
              flags |= PrecisionContext.SignalInexact;
            }
            if (Round(accum, rounding, neg, newmantissa)) {
              newmantissa += BigInteger.One;
            }
          }
          if (newmantissa.IsZero)
            flags |= PrecisionContext.SignalClamped;
          if ((flags & (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact)) == (PrecisionContext.SignalSubnormal | PrecisionContext.SignalInexact))
            flags |= PrecisionContext.SignalUnderflow | PrecisionContext.SignalRounded;
          if (signals != null) signals[0] = flags;
          if (neg) newmantissa = -newmantissa;
          return new DecimalFraction(newmantissa, fastETiny.AsBigInteger());
        }
      }
      bool expChanged = false;
      if ((accum.DiscardedBitCount).Sign != 0) {
        if (!bigmantissa.IsZero)
          flags |= PrecisionContext.SignalRounded;
        bigmantissa = accum.ShiftedInt;
        if ((accum.BitLeftmost | accum.BitsAfterLeftmost) != 0) {
          flags |= PrecisionContext.SignalInexact;
        }
        if (Round(accum, rounding, neg, bigmantissa)) {
          bigmantissa += BigInteger.One;
          if (bigmantissa.IsEven) {
            accum = new DigitShiftAccumulator(bigmantissa);
            accum.ShiftToBits(precision);
            if ((accum.DiscardedBitCount).Sign != 0) {
              exp.Add(accum.DiscardedBitCount);
              discardedBits.Add(accum.DiscardedBitCount);
              bigmantissa = accum.ShiftedInt;
              expChanged = true;
            }
          }
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
            BigInteger overflowMant = FindPowerOfTen(precision);
            overflowMant -= BigInteger.One;
            if (neg) overflowMant = -overflowMant;
            if (signals != null) signals[0] = flags;
            clamp = new FastInteger(fastEMax).Add(1).Subtract(precision);
            return new DecimalFraction(overflowMant, clamp.AsBigInteger());
          }
          if (signals != null) signals[0] = flags;
          return null;
        }
      }
      if (signals != null) signals[0] = flags;
      if (neg) bigmantissa = -bigmantissa;
      return new DecimalFraction(bigmantissa, exp.AsBigInteger());
    }

    /// <summary> Finds the sum of this object and another decimal fraction.
    /// The result's exponent is set to the lower of the exponents of the two
    /// operands. </summary>
    /// <param name='decfrac'> The number to add to.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The sum of this and the other object.</returns>
    public DecimalFraction Add(DecimalFraction decfrac, PrecisionContext ctx) {
      int expcmp = this.exponent.CompareTo((BigInteger)decfrac.exponent);
      DecimalFraction retval = null;
      if (expcmp == 0) {
        retval = new DecimalFraction(
          this.mantissa + (BigInteger)decfrac.mantissa, this.exponent);
      } else {
        // choose the minimum exponent
        BigInteger resultExponent = (expcmp < 0 ? this.exponent : decfrac.exponent);
        DecimalFraction op1 = this;
        DecimalFraction op2 = decfrac;
        BigInteger expdiff = BigInteger.Abs(op1.exponent - (BigInteger)op2.exponent);
        if (ctx != null && ctx.Precision > 0) {
          // Check if exponent difference is too big for
          // power-of-ten calculation to work quickly
          if (expdiff.CompareTo((BigInteger)100) >= 0) {
            FastInteger fastint = new FastInteger(expdiff).Add(3);
            // If exponent difference plus 3 is greater than the precision
            if (fastint.CompareTo(ctx.Precision) > 0) {
              int expcmp2 = op1.exponent.CompareTo(op2.exponent);
              if (expcmp2 < 0 && !(op2.mantissa.IsZero)) {
                // first operand's exponent is less
                // and second operand isn't zero
                // the 8 digits at the end are guard digits
                op1 = new DecimalFraction(
                  op1.mantissa, new FastInteger(op2.exponent).Subtract(ctx.Precision).Subtract(8)
                  .AsBigInteger());
              } else if (expcmp2 > 0 && !(op1.mantissa.IsZero)) {
                // first operand's exponent is greater
                // and first operand isn't zero
                // the 8 digits at the end are guard digits
                op2 = new DecimalFraction(
                  op2.mantissa, new FastInteger(op1.exponent).Subtract(ctx.Precision).Subtract(8)
                  .AsBigInteger());
              }
              expcmp = op1.exponent.CompareTo((BigInteger)op2.exponent);
              resultExponent = (expcmp < 0 ? op1.exponent : op2.exponent);
            }
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = RescaleByExponentDiff(
            op1.mantissa, op1.exponent, op2.exponent);
          retval = new DecimalFraction(
            newmant + (BigInteger)op2.mantissa, resultExponent);
        } else {
          BigInteger newmant = RescaleByExponentDiff(
            op2.mantissa, op1.exponent, op2.exponent);
          retval = new DecimalFraction(
            newmant + (BigInteger)op1.mantissa, resultExponent);
        }
      }
      if (ctx != null) {
        retval = retval.RoundToPrecision(ctx);
      }
      return retval;
    }

    /// <summary> Finds the sum of this object and another decimal fraction.
    /// The result's exponent is set to the lower of the exponents of the two
    /// operands. </summary>
    /// <returns></returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
    public DecimalFraction Add(DecimalFraction decfrac) {
      return Add(decfrac, null);
    }
    /// <summary> Finds the difference between this object and another decimal
    /// fraction. The result's exponent is set to the lower of the exponents
    /// of the two operands. </summary>
    /// <returns> The difference of the two objects.</returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      return Add(decfrac.Negate());
    }
    /// <summary> Finds the difference between this object and another decimal
    /// fraction. The result's exponent is set to the lower of the exponents
    /// of the two operands. </summary>
    /// <returns> The difference of the two objects.</returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public DecimalFraction Subtract(DecimalFraction decfrac, PrecisionContext ctx) {
      return Add(decfrac.Negate(), ctx);
    }
    /// <summary> Multiplies two decimal fractions. The resulting scale
    /// will be the sum of the scales of the two decimal fractions. </summary>
    /// <param name='decfrac'> Another decimal fraction.</param>
    /// <returns> The product of the two decimal fractions.</returns>
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      return Multiply(decfrac, null);
    }
    /// <summary> Multiplies two decimal fractions. The resulting scale
    /// will be the sum of the scales of the two decimal fractions. </summary>
    /// <param name='decfrac'> Another decimal fraction.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The product of the two decimal fractions.</returns>
    public DecimalFraction Multiply(DecimalFraction decfrac, PrecisionContext ctx) {
      BigInteger newexp = (this.exponent + (BigInteger)decfrac.exponent);
      DecimalFraction ret = new DecimalFraction(mantissa * (BigInteger)decfrac.mantissa, newexp);
      if (ctx != null) {
        ret = ret.RoundToPrecision(ctx);
      }
      return ret;
    }
    /// <summary> Multiplies by one decimal fraction, and then adds another
    /// decimal fraction. </summary>
    /// <param name='multiplicand'> The value to multiply.</param>
    /// <param name='augend'> The value to add.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The result this * multiplicand + augend.</returns>
    public DecimalFraction MultiplyAndAdd(DecimalFraction multiplicand,
                                          DecimalFraction augend,
                                          PrecisionContext ctx) {
      BigInteger newexp = (this.exponent + (BigInteger)multiplicand.exponent);
      DecimalFraction addend = new DecimalFraction(mantissa * (BigInteger)multiplicand.mantissa,
                                                 newexp);
      return addend.Add(augend, ctx);
    }

    /// <summary> Multiplies by one decimal fraction, and then adds another
    /// decimal fraction. </summary>
    /// <param name='multiplicand'> The value to multiply.</param>
    /// <param name='augend'> The value to add.</param>
    /// <returns> The result this * multiplicand + augend.</returns>
    public DecimalFraction MultiplyAndAdd(DecimalFraction multiplicand,
                                          DecimalFraction augend) {
      return this.Multiply(multiplicand).Add(augend);
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
    /// <summary> Compares the mathematical values of two decimal fractions.
    /// <para> This method is not consistent with the Equals method because
    /// two different decimal fractions with the same mathematical value,
    /// but different exponents, will compare as equal.</para>
    /// </summary>
    /// <returns> Less than 0 if this value is less than the other value, or
    /// greater than 0 if this value is greater than the other value or if "other"
    /// is null, or 0 if both values are equal.</returns>
    /// <param name='decfrac'> A DecimalFraction object.</param>
    public int CompareTo(DecimalFraction decfrac) {
      if (decfrac == null) return 1;
      int s = this.Sign;
      int ds = decfrac.Sign;
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      int mantcmp = mantissa.CompareTo((BigInteger)decfrac.mantissa);
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return s == 0 ? 0 : expcmp * s;
      }
      if (ds == 0) {
        // Special case: Second operand is zero
        return s;
      }
      if (s == 0) {
        // Special case: First operand is zero
        return -ds;
      }
      if (expcmp == 0) {
        return mantcmp;
      } else {
        BigInteger op1Exponent = this.exponent;
        BigInteger op2Exponent = decfrac.exponent;
        BigInteger expdiff = BigInteger.Abs(op1Exponent - (BigInteger)op2Exponent);
        // Check if exponent difference is too big for
        // power-of-ten calculation to work quickly
        if (expdiff.CompareTo((BigInteger)100) >= 0) {
          FastInteger fastint = new FastInteger(expdiff).Add(3);
          long precision1 = new DigitShiftAccumulator(
            BigInteger.Abs(this.mantissa)).KnownBitLength;
          long precision2 = new DigitShiftAccumulator(
            BigInteger.Abs(decfrac.mantissa)).KnownBitLength;
          long maxPrecision = Math.Max(precision1, precision2);
          // If exponent difference plus 3 is greater than the
          // maximum precision of the two operands
          if (fastint.CompareTo(maxPrecision) > 0) {
            int expcmp2 = op1Exponent.CompareTo(op2Exponent);
            if (expcmp2 < 0) {
              // first operand's exponent is less
              // (second operand won't be zero at this point)
              // the 8 digits at the end are guard digits
              op1Exponent = (new FastInteger(op2Exponent).Subtract(maxPrecision).Subtract(8)
                           .AsBigInteger());
            } else if (expcmp2 > 0) {
              // first operand's exponent is greater
              // (first operand won't be zero at this point)
              // the 8 digits at the end are guard digits
              op2Exponent = (new FastInteger(op1Exponent).Subtract(maxPrecision).Subtract(8)
                           .AsBigInteger());
            }
            expcmp = op1Exponent.CompareTo((BigInteger)op2Exponent);
          }
        }
        if (expcmp > 0) {
          BigInteger newmant = RescaleByExponentDiff(
            mantissa, op1Exponent, op2Exponent);
          return newmant.CompareTo((BigInteger)decfrac.mantissa);
        } else {
          BigInteger newmant = RescaleByExponentDiff(
            decfrac.mantissa, op1Exponent, op2Exponent);
          return this.mantissa.CompareTo((BigInteger)newmant);
        }
      }
    }
    private static bool AppendString(StringBuilder builder, char c, FastInteger count) {
      if (count.CompareTo(Int32.MaxValue) > 0 || count.Sign < 0) {
        throw new NotSupportedException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; i--) {
        builder.Append(c);
      }
      return true;
    }
    private string ToStringInternal(int mode) {
      // Using Java's rules for converting DecimalFraction
      // values to a string
      String mantissaString = this.mantissa.ToString(
        System.Globalization.CultureInfo.InvariantCulture);
      int scaleSign = -this.exponent.Sign;
      if (scaleSign == 0)
        return mantissaString;
      bool iszero = (this.mantissa.IsZero);
      if (mode == 2 && iszero && scaleSign < 0) {
        // special case for zero in plain
        return mantissaString;
      }
      FastInteger sbLength = new FastInteger(mantissaString.Length);
      int negaPos = 0;
      if (mantissaString[0] == '-') {
        sbLength.Add(-1);
        negaPos = 1;
      }
      FastInteger adjustedExponent = new FastInteger(this.exponent);
      FastInteger thisExponent = new FastInteger(adjustedExponent);
      adjustedExponent.Add(sbLength).Add(-1);
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) { // engineering string adjustments
        FastInteger newExponent = new FastInteger(adjustedExponent);
        bool adjExponentNegative = (adjustedExponent.Sign < 0);
        int intphase = new FastInteger(adjustedExponent).Abs().Mod(3).AsInt32();
        if (iszero && (adjustedExponent.CompareTo(threshold) < 0 ||
                       scaleSign < 0)) {
          if (intphase == 1) {
            if (adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(2);
            }
          } else if (intphase == 2) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(2);
            }
          }
          threshold.Add(1);
        } else {
          if (intphase == 1) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(-1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(-2);
            }
          } else if (intphase == 2) {
            if (adjExponentNegative) {
              decimalPointAdjust.Add(1);
              newExponent.Add(-1);
            } else {
              decimalPointAdjust.Add(2);
              newExponent.Add(-2);
            }
          }
        }
        adjustedExponent = newExponent;
      }
      if (mode == 2 || ((adjustedExponent.CompareTo(threshold) >= 0 &&
                         scaleSign >= 0))) {
        if (scaleSign > 0) {
          FastInteger decimalPoint = new FastInteger(thisExponent).Add(negaPos).Add(sbLength);
          int cmp = decimalPoint.CompareTo(negaPos);
          System.Text.StringBuilder builder = null;
          if (cmp < 0) {
            builder = new System.Text.StringBuilder((int)Math.Min(Int32.MaxValue, (long)mantissaString.Length + 6));
            builder.Append(mantissaString, 0, negaPos);
            builder.Append("0.");
            AppendString(builder, '0', new FastInteger(negaPos).Subtract(decimalPoint));
            builder.Append(mantissaString, negaPos, mantissaString.Length - negaPos);
          } else if (cmp == 0) {
            if (!decimalPoint.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new System.Text.StringBuilder((int)Math.Min(Int32.MaxValue, (long)mantissaString.Length + 6));
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append("0.");
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else if (decimalPoint.CompareTo(new FastInteger(negaPos).Add(mantissaString.Length)) > 0) {
            FastInteger insertionPoint = new FastInteger(negaPos).Add(sbLength);
            if (!insertionPoint.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new System.Text.StringBuilder((int)Math.Min(Int32.MaxValue, (long)mantissaString.Length + 6));
            builder.Append(mantissaString, 0, tmpInt);
            AppendString(builder, '0',
                         new FastInteger(decimalPoint).Subtract(builder.Length));
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else {
            if (!decimalPoint.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            builder = new System.Text.StringBuilder((int)Math.Min(Int32.MaxValue, (long)mantissaString.Length + 6));
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          }
          return builder.ToString();
        } else if (mode == 2 && scaleSign < 0) {
          FastInteger negscale = new FastInteger(thisExponent);
          System.Text.StringBuilder builder = new System.Text.StringBuilder();
          builder.Append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.ToString();
        } else {
          return mantissaString;
        }
      } else {
        System.Text.StringBuilder builder = null;
        if (mode == 1 && iszero && decimalPointAdjust.CompareTo(1) > 0) {
          builder = new System.Text.StringBuilder();
          builder.Append(mantissaString);
          builder.Append('.');
          AppendString(builder, '0', new FastInteger(decimalPointAdjust).Add(-1));
        } else {
          FastInteger tmp = new FastInteger(negaPos).Add(decimalPointAdjust);
          int cmp = tmp.CompareTo(mantissaString.Length);
          if (cmp > 0) {
            tmp.Subtract(mantissaString.Length);
            builder = new System.Text.StringBuilder();
            builder.Append(mantissaString);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            // Insert a decimal point at the right place
            if (!tmp.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = tmp.AsInt32();
            if (tmp.Sign < 0) tmpInt = 0;
            builder = new System.Text.StringBuilder(
              (int)Math.Min(Int32.MaxValue, (long)mantissaString.Length + 6));
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else if (adjustedExponent.Sign == 0) {
            return mantissaString;
          } else {
            builder = new System.Text.StringBuilder();
            builder.Append(mantissaString);
          }
        }
        if (adjustedExponent.Sign != 0) {
          builder.Append(adjustedExponent.Sign < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          StringBuilder builderReversed = new StringBuilder();
          while (adjustedExponent.Sign != 0) {
            int digit = new FastInteger(adjustedExponent).Mod(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.Append((char)('0' + digit));
            adjustedExponent.Divide(10);
          }
          int count = builderReversed.Length;
          for (int i = 0; i < count; i++) {
            builder.Append(builderReversed[count - 1 - i]);
          }
        }
        return builder.ToString();
      }
    }

    private static BigInteger[] BigIntPowersOfTen = new BigInteger[]{
      (BigInteger)1, (BigInteger)10, (BigInteger)100, (BigInteger)1000, (BigInteger)10000, (BigInteger)100000, (BigInteger)1000000, (BigInteger)10000000, (BigInteger)100000000, (BigInteger)1000000000,
      (BigInteger)10000000000L, (BigInteger)100000000000L, (BigInteger)1000000000000L, (BigInteger)10000000000000L,
      (BigInteger)100000000000000L, (BigInteger)1000000000000000L, (BigInteger)10000000000000000L,
      (BigInteger)100000000000000000L, (BigInteger)1000000000000000000L
    };

    private static BigInteger[] BigIntPowersOfFive = new BigInteger[]{
      (BigInteger)1, (BigInteger)5, (BigInteger)25, (BigInteger)125, (BigInteger)625, (BigInteger)3125, (BigInteger)15625, (BigInteger)78125, (BigInteger)390625,
      (BigInteger)1953125, (BigInteger)9765625, (BigInteger)48828125, (BigInteger)244140625, (BigInteger)1220703125,
      (BigInteger)6103515625L, (BigInteger)30517578125L, (BigInteger)152587890625L, (BigInteger)762939453125L,
      (BigInteger)3814697265625L, (BigInteger)19073486328125L, (BigInteger)95367431640625L,
      (BigInteger)476837158203125L, (BigInteger)2384185791015625L, (BigInteger)11920928955078125L,
      (BigInteger)59604644775390625L, (BigInteger)298023223876953125L, (BigInteger)1490116119384765625L,
      (BigInteger)7450580596923828125L
    };

    /// <summary> Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer. </summary>
    /// <returns></returns>
    public BigInteger ToBigInteger() {
      int sign = this.Exponent.Sign;
      if (sign == 0) {
        return this.Mantissa;
      } else if (sign > 0) {
        BigInteger bigmantissa = this.Mantissa;
        bigmantissa *= (BigInteger)(FindPowerOfTenFromBig(this.Exponent));
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.Mantissa;
        BigInteger bigexponent = this.Exponent;
        bigexponent = -bigexponent;
        bigmantissa /= (BigInteger)(FindPowerOfTenFromBig(bigexponent));
        return bigmantissa;
      }
    }
    /// <summary> Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used. </summary>
    /// <returns> The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
    public float ToSingle() {
      return BigFloat.FromDecimalFraction(this).ToSingle();
    }
    /// <summary> Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used. </summary>
    /// <returns> The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      return BigFloat.FromDecimalFraction(this).ToDouble();
    }
    /// <summary> Creates a decimal fraction from a 32-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string. </summary>
    /// <param name='dbl'> A 32-bit floating-point number.</param>
    /// <returns> A decimal fraction with the same value as "flt".</returns>
    /// <exception cref='OverflowException'> "flt" is infinity or not-a-number.</exception>
    /// <param name='flt'> A 32-bit floating-point number.</param>
    public static DecimalFraction FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      int fpExponent = (int)((value >> 23) & 0xFF);
      if (fpExponent == 255)
        throw new OverflowException("Value is infinity or NaN");
      int fpMantissa = value & 0x7FFFFF;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1 << 23);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 150;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      bool neg = ((value >> 31) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = (BigInteger)fpMantissa;
        bigmantissa <<= fpExponent;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = (BigInteger)fpMantissa;
        bigmantissa *= (BigInteger)(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }
    /// <summary> Creates a decimal fraction from a 64-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string. </summary>
    /// <param name='dbl'> A 64-bit floating-point number.</param>
    /// <returns> A decimal fraction with the same value as "dbl"</returns>
    /// <exception cref='OverflowException'> "dbl" is infinity or not-a-number.</exception>
    public static DecimalFraction FromDouble(double dbl) {
      long value = BitConverter.ToInt64(BitConverter.GetBytes((double)dbl), 0);
      int fpExponent = (int)((value >> 52) & 0x7ffL);
      if (fpExponent == 2047)
        throw new OverflowException("Value is infinity or NaN");
      long fpMantissa = value & 0xFFFFFFFFFFFFFL;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1L << 52);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 1075;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      bool neg = ((value >> 63) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = (BigInteger)fpMantissa;
        bigmantissa <<= fpExponent;
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = (BigInteger)fpMantissa;
        bigmantissa *= (BigInteger)(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }

    /// <summary> Creates a decimal fraction from an arbitrary-precision
    /// binary floating-point number. </summary>
    /// <param name='bigfloat'> A bigfloat.</param>
    /// <returns></returns>
    public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
      BigInteger bigintExp = bigfloat.Exponent;
      BigInteger bigintMant = bigfloat.Mantissa;
      if (bigintExp.IsZero) {
        // Integer
        return new DecimalFraction(bigintMant);
      } else if (bigintExp > 0) {
        // Scaled integer
        FastInteger intcurexp = new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = (bigmantissa.Sign < 0);
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        while (intcurexp.Sign > 0) {
          int shift = 512;
          if (intcurexp.CompareTo(512) < 0) {
            shift = intcurexp.AsInt32();
          }
          bigmantissa <<= shift;
          intcurexp.Add(-shift);
        }
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger negbigintExp = -(BigInteger)bigintExp;
        bigmantissa *= (BigInteger)(FindPowerOfFiveFromBig(negbigintExp));
        return new DecimalFraction(bigmantissa, bigintExp);
      }
    }
    /// <summary> Converts this value to a string.The format of the return
    /// value is exactly the same as that of the java.math.BigDecimal.toString()
    /// method. </summary>
    /// <returns> A string representation of this object.</returns>
    public override string ToString() {
      return ToStringInternal(0);
    }
    /// <summary> Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows the
    /// format of the java.math.BigDecimal.toEngineeringString() method.
    /// </summary>
    /// <returns></returns>
    public string ToEngineeringString() {
      return ToStringInternal(1);
    }
    /// <summary> Converts this value to a string, but without an exponent
    /// part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method. </summary>
    /// <returns></returns>
    public string ToPlainString() {
      return ToStringInternal(2);
    }
  }
}