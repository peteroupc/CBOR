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
    /// <summary>Represents an arbitrary-precision decimal floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number is equal to mantissa
    /// * 10^exponent. <para>The mantissa is the value of the digits that
    /// make up a number, ignoring the decimal point and exponent. For example,
    /// in the number 2356.78, the mantissa is 235678. The exponent is where
    /// the "floating" decimal point of the number is located. A positive
    /// exponent means "move it to the right", and a negative exponent means
    /// "move it to the left." In the example 2,356.78, the exponent is -2,
    /// since it has 2 decimal places and the decimal point is "moved to the
    /// left by 2." Therefore, in the ExtendedDecimal representation, this
    /// number would be stored as 235678 * 10^-2.</para>
    /// <para>The mantissa and exponent format preserves trailing zeros
    /// in the number's value. This may give rise to multiple ways to store
    /// the same value. For example, 1.00 and 1 would be stored differently,
    /// even though they have the same value. In the first case, 100 * 10^-2
    /// (100 with decimal point moved left by 2), and in the second case, 1 *
    /// 10^0 (1 with decimal point moved 0).</para>
    /// <para>This class also supports values for negative zero, not-a-number
    /// (NaN) values, and infinity. <b>Negative zero</b>
    /// is generally used when a negative number is rounded to 0; it has the
    /// same mathematical value as positive zero. <b>Infinity</b>
    /// is generally used when a non-zero number is divided by zero, or when
    /// a very high number can't be represented in a given exponent range.
    /// <b>Not-a-number</b>
    /// is generally used to signal errors. </para>
    /// <para>This class implements the <a href='http://speleotrove.com/decimal/decarith.html'>General
    /// Decimal Arithmetic Specification</a>
    /// version 1.70.</para>
    /// <para>Passing a signaling NaN to any arithmetic operation shown
    /// here will signal the flag FlagInvalid and return a quiet NaN, even
    /// if another operand to that operation is a quiet NaN, unless noted otherwise.</para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here
    /// will return a quiet NaN, unless noted otherwise. Invalid operations
    /// will also return a quiet NaN, as stated in the individual methods.</para>
    /// <para>Unless noted otherwise, passing a null ExtendedDecimal argument
    /// to any method here will throw an exception.</para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid,
    /// FlagOverflow, or FlagDivideByZero, it will not throw an exception
    /// too, unless the flag's trap is enabled in the precision context (see
    /// PrecisionContext's Traps property).</para>
    /// <para>An ExtendedDecimal value can be serialized in one of the following
    /// ways:</para>
    /// <list> <item>By calling the toString() method, which will always
    /// return distinct strings for distinct ExtendedDecimal values.</item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative
    /// properties, and calling the IsInfinity, IsQuietNaN, and IsSignalingNaN
    /// methods. The return values combined will uniquely identify a particular
    /// ExtendedDecimal value.</item>
    /// </list>
    /// </summary>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>, IEquatable<ExtendedDecimal> {
    private const int MaxSafeInt = 214748363;

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
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    public bool EqualsInternal(ExtendedDecimal otherValue) {
      if (otherValue == null) {
        return false;
      }
      return this.flags == otherValue.flags &&
        this.unsignedMantissa.Equals(otherValue.unsignedMantissa) &&
        this.exponent.Equals(otherValue.exponent);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>A Boolean object.</returns>
    public bool Equals(ExtendedDecimal other) {
      return this.EqualsInternal(other);
    }

    /// <summary>Determines whether this object&apos;s mantissa and exponent
    /// are equal to those of another object and that other object is a decimal
    /// fraction.</summary>
    /// <returns>True if the objects are equal; false otherwise.</returns>
    /// <param name='obj'>An arbitrary object.</param>
    public override bool Equals(object obj) {
      return this.EqualsInternal(obj as ExtendedDecimal);
    }

    /// <summary>Calculates this object&apos;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * this.exponent.GetHashCode();
        hashCode += 1000000009 * this.unsignedMantissa.GetHashCode();
        hashCode += 1000000009 * this.flags;
      }
      return hashCode;
    }
    #endregion

    /// <summary>Creates a decimal number with the value exponent*10^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The decimal exponent.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal Create(BigInteger mantissa, BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      ExtendedDecimal ex = new ExtendedDecimal();
      ex.exponent = exponent;
      int sign = mantissa == null ? 0 : mantissa.Sign;
      ex.unsignedMantissa = sign < 0 ? (-(BigInteger)mantissa) : mantissa;
      ex.flags = (sign < 0) ? BigNumberFlags.FlagNegative : 0;
      return ex;
    }

    private ExtendedDecimal() {
    }

    private static ExtendedDecimal CreateWithFlags(
      BigInteger mantissa,
      BigInteger exponent,
      int flags) {
      ExtendedDecimal ext = ExtendedDecimal.Create(mantissa, exponent);
      ext.flags = flags;
      return ext;
    }

    public static ExtendedDecimal FromString(String str) {
      return FromString(str, null);
    }

    /// <summary>Creates a decimal number from a string that represents
    /// a number.<para> The format of the string generally consists of:<list
    /// type=''> <item> An optional '-' or '+' character (if '-', the value
    /// is negative.)</item>
    /// <item> One or more digits, with a single optional decimal point after
    /// the first digit and before the last digit.</item>
    /// <item> Optionally, E+ (positive exponent) or E- (negative exponent)
    /// plus one or more digits specifying the exponent.</item>
    /// </list>
    /// </para>
    /// <para>The string can also be "-INF", "-Infinity", "Infinity", "INF",
    /// quiet NaN ("qNaN"/"-qNaN") followed by any number of digits, or signaling
    /// NaN ("sNaN"/"-sNaN") followed by any number of digits, all in any
    /// combination of upper and lower case.</para>
    /// <para> The format generally follows the definition in java.math.BigDecimal(),
    /// except that the digits must be ASCII digits ('0' through '9').</para>
    /// </summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <exception cref='System.ArgumentNullException'>The parameter
    /// <paramref name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref name='str'/>
    /// is not a correctly formatted number string.</exception>
    public static ExtendedDecimal FromString(String str, PrecisionContext ctx) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (str.Length == 0) {
        throw new FormatException();
      }
      int offset = 0;
      bool negative = false;
      if (str[0] == '+' || str[0] == '-') {
        negative = str[0] == '-';
        ++offset;
      }
      int mantInt = 0;
      FastInteger mant = null;
      int mantBuffer = 0;
      int mantBufferMult = 1;
      int expBuffer = 0;
      int expBufferMult = 1;
      bool haveDecimalPoint = false;
      bool haveDigits = false;
      bool haveExponent = false;
      int newScaleInt = 0;
      FastInteger newScale = null;
      int i = offset;
      if (i + 8 == str.Length) {
        if ((str[i] == 'I' || str[i] == 'i') &&
            (str[i + 1] == 'N' || str[i + 1] == 'n') &&
            (str[i + 2] == 'F' || str[i + 2] == 'f') &&
            (str[i + 3] == 'I' || str[i + 3] == 'i') &&
            (str[i + 4] == 'N' || str[i + 4] == 'n') &&
            (str[i + 5] == 'I' || str[i + 5] == 'i') &&
            (str[i + 6] == 'T' || str[i + 6] == 't') &&
            (str[i + 7] == 'Y' || str[i + 7] == 'y')) {
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 == str.Length) {
        if ((str[i] == 'I' || str[i] == 'i') &&
            (str[i + 1] == 'N' || str[i + 1] == 'n') &&
            (str[i + 2] == 'F' || str[i + 2] == 'f')) {
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 <= str.Length) {
        // Quiet NaN
        if ((str[i] == 'N' || str[i] == 'n') &&
            (str[i + 1] == 'A' || str[i + 1] == 'a') &&
            (str[i + 2] == 'N' || str[i + 2] == 'n')) {
          if (i + 3 == str.Length) {
            if (!negative) {
              return NaN;
            }
            return CreateWithFlags(
              BigInteger.Zero,
              BigInteger.Zero,
              (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagQuietNaN);
          }
          i += 3;
          FastInteger digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && !ctx.Precision.IsZero) {
            maxDigits = FastInteger.FromBig(ctx.Precision);
            if (ctx.ClampNormalExponents) {
              maxDigits.Decrement();
            }
          }
          for (; i < str.Length; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              int thisdigit = (int)(str[i] - '0');
              haveDigits = haveDigits || thisdigit != 0;
              if (mantInt > MaxSafeInt) {
                if (mant == null) {
                  mant = new FastInteger(mantInt);
                  mantBuffer = thisdigit;
                  mantBufferMult = 10;
                } else {
                  if (mantBufferMult >= 1000000000) {
                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                    mantBuffer = thisdigit;
                    mantBufferMult = 10;
                  } else {
                    mantBufferMult *= 10;
                    mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                    mantBuffer += thisdigit;
                  }
                }
              } else {
                mantInt *= 10;
                mantInt += thisdigit;
              }
              if (haveDigits && maxDigits != null) {
                digitCount.Increment();
                if (digitCount.CompareTo(maxDigits) > 0) {
                  // NaN contains too many digits
                  throw new FormatException();
                }
              }
            } else {
              throw new FormatException();
            }
          }
          if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
          }
          BigInteger bigmant = (mant == null) ? ((BigInteger)mantInt) : mant.AsBigInteger();
          return CreateWithFlags(
            bigmant,
            BigInteger.Zero,
            (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagQuietNaN);
        }
      }
      if (i + 4 <= str.Length) {
        // Signaling NaN
        if ((str[i] == 'S' || str[i] == 's') &&
            (str[i + 1] == 'N' || str[i + 1] == 'n') &&
            (str[i + 2] == 'A' || str[i + 2] == 'a') &&
            (str[i + 3] == 'N' || str[i + 3] == 'n')) {
          if (i + 4 == str.Length) {
            if (!negative) {
              return SignalingNaN;
            }
            return CreateWithFlags(
              BigInteger.Zero,
              BigInteger.Zero,
              (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagSignalingNaN);
          }
          i += 4;
          FastInteger digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && !ctx.Precision.IsZero) {
            maxDigits = FastInteger.FromBig(ctx.Precision);
            if (ctx.ClampNormalExponents) {
              maxDigits.Decrement();
            }
          }
          for (; i < str.Length; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              int thisdigit = (int)(str[i] - '0');
              haveDigits = haveDigits || thisdigit != 0;
              if (mantInt > MaxSafeInt) {
                if (mant == null) {
                  mant = new FastInteger(mantInt);
                  mantBuffer = thisdigit;
                  mantBufferMult = 10;
                } else {
                  if (mantBufferMult >= 1000000000) {
                    mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                    mantBuffer = thisdigit;
                    mantBufferMult = 10;
                  } else {
                    mantBufferMult *= 10;
                    mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                    mantBuffer += thisdigit;
                  }
                }
              } else {
                mantInt *= 10;
                mantInt += thisdigit;
              }
              if (haveDigits && maxDigits != null) {
                digitCount.Increment();
                if (digitCount.CompareTo(maxDigits) > 0) {
                  // NaN contains too many digits
                  throw new FormatException();
                }
              }
            } else {
              throw new FormatException();
            }
          }
          if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
          }
          BigInteger bigmant = (mant == null) ? ((BigInteger)mantInt) : mant.AsBigInteger();
          return CreateWithFlags(
            bigmant,
            BigInteger.Zero,
            (negative ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagSignalingNaN);
        }
      }
      // Ordinary number
      for (; i < str.Length; ++i) {
        if (str[i] >= '0' && str[i] <= '9') {
          int thisdigit = (int)(str[i] - '0');
          if (mantInt > MaxSafeInt) {
            if (mant == null) {
              mant = new FastInteger(mantInt);
              mantBuffer = thisdigit;
              mantBufferMult = 10;
            } else {
              if (mantBufferMult >= 1000000000) {
                mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                mantBuffer = thisdigit;
                mantBufferMult = 10;
              } else {
                mantBufferMult *= 10;
                mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                mantBuffer += thisdigit;
              }
            }
          } else {
            mantInt *= 10;
            mantInt += thisdigit;
          }
          haveDigits = true;
          if (haveDecimalPoint) {
            if (newScaleInt == Int32.MinValue) {
              if (newScale == null) {
                newScale = new FastInteger(newScaleInt);
              }
              newScale.AddInt(-1);
            } else {
              --newScaleInt;
            }
          }
        } else if (str[i] == '.') {
          if (haveDecimalPoint) {
            throw new FormatException();
          }
          haveDecimalPoint = true;
        } else if (str[i] == 'E' || str[i] == 'e') {
          haveExponent = true;
          ++i;
          break;
        } else {
          throw new FormatException();
        }
      }
      if (!haveDigits) {
        throw new FormatException();
      }
      if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
        mant.Multiply(mantBufferMult).AddInt(mantBuffer);
      }
      if (haveExponent) {
        FastInteger exp = null;
        int expInt = 0;
        offset = 1;
        haveDigits = false;
        if (i == str.Length) {
          throw new FormatException();
        }
        if (str[i] == '+' || str[i] == '-') {
          if (str[i] == '-') {
            offset = -1;
          }
          ++i;
        }
        for (; i < str.Length; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
            int thisdigit = (int)(str[i] - '0');
            if (expInt > MaxSafeInt) {
              if (exp == null) {
                exp = new FastInteger(expInt);
                expBuffer = thisdigit;
                expBufferMult = 10;
              } else {
                if (expBufferMult >= 1000000000) {
                  exp.Multiply(expBufferMult).AddInt(expBuffer);
                  expBuffer = thisdigit;
                  expBufferMult = 10;
                } else {
                  // multiply expBufferMult and expBuffer each by 10
                  expBufferMult = (expBufferMult << 3) + (expBufferMult << 1);
                  expBuffer = (expBuffer << 3) + (expBuffer << 1);
                  expBuffer += thisdigit;
                }
              }
            } else {
              expInt *= 10;
              expInt += thisdigit;
            }
          } else {
            throw new FormatException();
          }
        }
        if (!haveDigits) {
          throw new FormatException();
        }
        if (exp != null && (expBufferMult != 1 || expBuffer != 0)) {
          exp.Multiply(expBufferMult).AddInt(expBuffer);
        }
        if (offset >= 0 && newScaleInt == 0 && newScale == null && exp == null) {
          newScaleInt = expInt;
        } else if (exp == null) {
          if (newScale == null) {
            newScale = new FastInteger(newScaleInt);
          }
          if (offset < 0) {
            newScale.SubtractInt(expInt);
          } else if (expInt != 0) {
            newScale.AddInt(expInt);
          }
        } else {
          if (newScale == null) {
            newScale = new FastInteger(newScaleInt);
          }
          if (offset < 0) {
            newScale.Subtract(exp);
          } else {
            newScale.Add(exp);
          }
        }
      }
      if (i != str.Length) {
        throw new FormatException();
      }
      ExtendedDecimal ret = new ExtendedDecimal();
      ret.unsignedMantissa = (mant == null) ? ((BigInteger)mantInt) : mant.AsBigInteger();
      ret.exponent = (newScale == null) ? ((BigInteger)newScaleInt) : newScale.AsBigInteger();
      ret.flags = negative ? BigNumberFlags.FlagNegative : 0;
      if (ctx != null) {
        ret = ret.RoundToPrecision(ctx);
      }
      return ret;
    }

    private sealed class DecimalMathHelper : IRadixMathHelper<ExtendedDecimal> {
    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetRadix() {
        return 10;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetSign(ExtendedDecimal value) {
        return value.Sign;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetMantissa(ExtendedDecimal value) {
        return value.unsignedMantissa;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetExponent(ExtendedDecimal value) {
        return value.exponent;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer. (2).</param>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger bigint, int lastDigit, int olderDigits) {
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An IShiftAccumulator object.</returns>
    /// <param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint, 0, 0);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>A BigInteger object. (2).</param>
    /// <returns>A Boolean object.</returns>
      public bool HasTerminatingRadixExpansion(BigInteger numerator, BigInteger denominator) {
        // Simplify denominator based on numerator
        BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
        denominator /= gcd;
        if (denominator.IsZero) {
          return false;
        }
        // Eliminate factors of 2
        while (denominator.IsEven) {
          denominator >>= 1;
        }
        // Eliminate factors of 5
        while (true) {
          BigInteger bigrem;
          BigInteger bigquo = BigInteger.DivRem(denominator, (BigInteger)5, out bigrem);
          if (!bigrem.IsZero) {
            break;
          }
          denominator = bigquo;
        }
        return denominator.CompareTo(BigInteger.One) == 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigint'>A BigInteger object. (2).</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.Sign <= 0) {
          return bigint;
        }
        if (bigint.IsZero) {
          return bigint;
        }
        BigInteger bigtmp = null;
        if (bigint.CompareTo(BigInteger.One) != 0) {
          if (power.CanFitInInt32()) {
            bigtmp = DecimalUtility.FindPowerOfTen(power.AsInt32());
            bigint *= (BigInteger)bigtmp;
          } else {
            bigtmp = DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
            bigint *= (BigInteger)bigtmp;
          }
          return bigint;
        } else {
          if (power.CanFitInInt32()) {
            return DecimalUtility.FindPowerOfTen(power.AsInt32());
          } else {
            return DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
          }
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetFlags(ExtendedDecimal value) {
        return value.flags;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>A BigInteger object. (2).</param>
    /// <param name='flags'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
      public ExtendedDecimal CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags) {
        return CreateWithFlags(mantissa, exponent, flags);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteAndNonFinite;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
      public ExtendedDecimal ValueOf(int val) {
        if (val == 0) {
          return Zero;
        }
        if (val == 1) {
          return One;
        }
        return FromInt64(val);
      }
    }

    private static bool AppendString(StringBuilder builder, char c, FastInteger count) {
      if (count.CompareToInt(Int32.MaxValue) > 0 || count.Sign < 0) {
        throw new NotSupportedException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; --i) {
        builder.Append(c);
      }
      return true;
    }

    private string ToStringInternal(int mode) {
      // Using Java's rules for converting ExtendedDecimal
      // values to a string
      bool negative = (this.flags & BigNumberFlags.FlagNegative) != 0;
      if ((this.flags & BigNumberFlags.FlagInfinity) != 0) {
        return negative ? "-Infinity" : "Infinity";
      }
      if ((this.flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        if (this.unsignedMantissa.IsZero) {
          return negative ? "-sNaN" : "sNaN";
        }
        return negative ?
          "-sNaN" + BigInteger.Abs(this.unsignedMantissa).ToString() :
          "sNaN" + BigInteger.Abs(this.unsignedMantissa).ToString();
      }
      if ((this.flags & BigNumberFlags.FlagQuietNaN) != 0) {
        if (this.unsignedMantissa.IsZero) {
          return negative ? "-NaN" : "NaN";
        }
        return negative ?
          "-NaN" + BigInteger.Abs(this.unsignedMantissa).ToString() :
          "NaN" + BigInteger.Abs(this.unsignedMantissa).ToString();
      }
      String mantissaString = BigInteger.Abs(this.unsignedMantissa).ToString();
      int scaleSign = -this.exponent.Sign;
      if (scaleSign == 0) {
        return negative ? "-" + mantissaString : mantissaString;
      }
      bool iszero = this.unsignedMantissa.IsZero;
      if (mode == 2 && iszero && scaleSign < 0) {
        // special case for zero in plain
        return negative ? "-" + mantissaString : mantissaString;
      }
      FastInteger builderLength = new FastInteger(mantissaString.Length);
      FastInteger adjustedExponent = FastInteger.FromBig(this.exponent);
      FastInteger thisExponent = FastInteger.Copy(adjustedExponent);
      adjustedExponent.Add(builderLength).AddInt(-1);
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) {  // engineering string adjustments
        FastInteger newExponent = FastInteger.Copy(adjustedExponent);
        bool adjExponentNegative = adjustedExponent.Sign < 0;
        int intphase = FastInteger.Copy(adjustedExponent).Abs().Remainder(3).AsInt32();
        if (iszero && (adjustedExponent.CompareTo(threshold) < 0 ||
                       scaleSign < 0)) {
          if (intphase == 1) {
            if (adjExponentNegative) {
              decimalPointAdjust.Increment();
              newExponent.Increment();
            } else {
              decimalPointAdjust.AddInt(2);
              newExponent.AddInt(2);
            }
          } else if (intphase == 2) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Increment();
              newExponent.Increment();
            } else {
              decimalPointAdjust.AddInt(2);
              newExponent.AddInt(2);
            }
          }
          threshold.Increment();
        } else {
          if (intphase == 1) {
            if (!adjExponentNegative) {
              decimalPointAdjust.Increment();
              newExponent.AddInt(-1);
            } else {
              decimalPointAdjust.AddInt(2);
              newExponent.AddInt(-2);
            }
          } else if (intphase == 2) {
            if (adjExponentNegative) {
              decimalPointAdjust.Increment();
              newExponent.AddInt(-1);
            } else {
              decimalPointAdjust.AddInt(2);
              newExponent.AddInt(-2);
            }
          }
        }
        adjustedExponent = newExponent;
      }
      if (mode == 2 || (adjustedExponent.CompareTo(threshold) >= 0 &&
                        scaleSign >= 0)) {
        if (scaleSign > 0) {
          FastInteger decimalPoint = FastInteger.Copy(thisExponent).Add(builderLength);
          int cmp = decimalPoint.CompareToInt(0);
          System.Text.StringBuilder builder = null;
          if (cmp < 0) {
            FastInteger tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareToInt(Int32.MaxValue) > 0 ?
              Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append("0.");
            AppendString(builder, '0', FastInteger.Copy(decimalPoint).Negate());
            builder.Append(mantissaString);
          } else if (cmp == 0) {
            if (!decimalPoint.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareToInt(Int32.MaxValue) > 0 ?
              Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append("0.");
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else if (decimalPoint.CompareToInt(mantissaString.Length) > 0) {
            FastInteger insertionPoint = builderLength;
            if (!insertionPoint.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareToInt(Int32.MaxValue) > 0 ?
              Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            AppendString(
              builder,
              '0',
              FastInteger.Copy(decimalPoint).SubtractInt(builder.Length));
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else {
            if (!decimalPoint.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareToInt(Int32.MaxValue) > 0 ?
              Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          }
          return builder.ToString();
        } else if (mode == 2 && scaleSign < 0) {
          FastInteger negscale = FastInteger.Copy(thisExponent);
          System.Text.StringBuilder builder = new System.Text.StringBuilder();
          if (negative) {
            builder.Append('-');
          }
          builder.Append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.ToString();
        } else if (!negative) {
          return mantissaString;
        } else {
          return "-" + mantissaString;
        }
      } else {
        System.Text.StringBuilder builder = null;
        if (mode == 1 && iszero && decimalPointAdjust.CompareToInt(1) > 0) {
          builder = new System.Text.StringBuilder();
          if (negative) {
            builder.Append('-');
          }
          builder.Append(mantissaString);
          builder.Append('.');
          AppendString(builder, '0', FastInteger.Copy(decimalPointAdjust).AddInt(-1));
        } else {
          FastInteger tmp = FastInteger.Copy(decimalPointAdjust);
          int cmp = tmp.CompareToInt(mantissaString.Length);
          if (cmp > 0) {
            tmp.SubtractInt(mantissaString.Length);
            builder = new System.Text.StringBuilder();
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            // Insert a decimal point at the right place
            if (!tmp.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = tmp.AsInt32();
            if (tmp.Sign < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareToInt(Int32.MaxValue) > 0 ?
              Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else if (adjustedExponent.Sign == 0 && !negative) {
            return mantissaString;
          } else if (adjustedExponent.Sign == 0 && negative) {
            return "-" + mantissaString;
          } else {
            builder = new System.Text.StringBuilder();
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString);
          }
        }
        if (adjustedExponent.Sign != 0) {
          builder.Append(adjustedExponent.Sign < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          StringBuilder builderReversed = new StringBuilder();
          while (adjustedExponent.Sign != 0) {
            int digit = FastInteger.Copy(adjustedExponent).Remainder(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.Append((char)('0' + digit));
            adjustedExponent.Divide(10);
          }
          int count = builderReversed.Length;
          for (int i = 0; i < count; ++i) {
            builder.Append(builderReversed[count - 1 - i]);
          }
        }
        return builder.ToString();
      }
    }

    /// <summary>Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ToBigInteger() {
      int sign = this.Exponent.Sign;
      if (sign == 0) {
        BigInteger bigmantissa = this.Mantissa;
        return bigmantissa;
      } else if (sign > 0) {
        BigInteger bigmantissa = this.Mantissa;
        BigInteger bigexponent = DecimalUtility.FindPowerOfTenFromBig(this.Exponent);
        bigmantissa *= (BigInteger)bigexponent;
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.Mantissa;
        BigInteger bigexponent = this.Exponent;
        bigexponent = -bigexponent;
        bigexponent = DecimalUtility.FindPowerOfTenFromBig(bigexponent);
        bigmantissa /= (BigInteger)bigexponent;
        return bigmantissa;
      }
    }

    private static BigInteger valueOneShift62 = BigInteger.One << 62;

    /// <summary>Creates a binary floating-point number from this object&apos;s
    /// value. Note that if the binary floating-point number contains a negative
    /// exponent, the resulting value might not be exact. However, the resulting
    /// binary float will contain enough precision to accurately convert
    /// it to a 32-bit or 64-bit floating point number (float or double).</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloat() {
      if (this.IsNaN() || this.IsInfinity()) {
        return ExtendedFloat.CreateWithFlags(this.unsignedMantissa, this.exponent, this.flags);
      }
      BigInteger bigintExp = this.Exponent;
      BigInteger bigintMant = this.Mantissa;
      if (bigintMant.IsZero) {
        return this.IsNegative ? ExtendedFloat.NegativeZero : ExtendedFloat.Zero;
      }
      if (bigintExp.IsZero) {
        // Integer
        return ExtendedFloat.FromBigInteger(bigintMant);
      } else if (bigintExp.Sign > 0) {
        // Scaled integer
        BigInteger bigmantissa = bigintMant;
        bigintExp = DecimalUtility.FindPowerOfTenFromBig(bigintExp);
        bigmantissa *= (BigInteger)bigintExp;
        return ExtendedFloat.FromBigInteger(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = bigmantissa.Sign < 0;
        BigInteger remainder;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        FastInteger negscale = FastInteger.Copy(scale).Negate();
        BigInteger divisor = DecimalUtility.FindPowerOfFiveFromBig(
          negscale.AsBigInteger());
        while (true) {
          BigInteger quotient = BigInteger.DivRem(bigmantissa, divisor, out remainder);
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (!remainder.IsZero &&
              quotient.CompareTo(valueOneShift62) < 0) {
            // At this point, the quotient has 62 or fewer bits
            int[] bits = FastInteger.GetLastWords(quotient, 2);
            int shift = 0;
            if ((bits[0] | bits[1]) != 0) {
              // Quotient's integer part is nonzero.
              // Get the number of bits of the quotient
              int bitPrecision = DecimalUtility.BitPrecisionInt(bits[1]);
              if (bitPrecision != 0) {
                bitPrecision += 32;
              } else {
                bitPrecision = DecimalUtility.BitPrecisionInt(bits[0]);
              }
              shift = 63 - bitPrecision;
              scale.SubtractInt(shift);
            } else {
              // Integer part of quotient is 0
              shift = 1;
              scale.SubtractInt(shift);
            }
            // shift by that many bits, but not less than 1
            bigmantissa <<= shift;
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
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedFloat.Create(bigmantissa, scale.AsBigInteger());
      }
    }

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
      if (this.IsNegative && this.IsZero) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0);
      }
      return this.ToExtendedFloat().ToSingle();
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
      if (this.IsNegative && this.IsZero) {
        return Extras.IntegersToDouble(new int[] { unchecked((int)(1 << 31)), 0 });
      }
      return this.ToExtendedFloat().ToDouble();
    }

    /// <summary>Creates a decimal number from a 32-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string.</summary>
    /// <returns>A decimal number with the same value as <paramref name='flt'/>.</returns>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    public static ExtendedDecimal FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      bool neg = (value >> 31) != 0;
      int floatExponent = (int)((value >> 23) & 0xFF);
      int valueFpMantissa = value & 0x7FFFFF;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1FFFFF;
        BigInteger info = (BigInteger)valueFpMantissa;
        if (info.IsZero) {
          return quiet ? NaN : SignalingNaN;
        } else {
          return CreateWithFlags(
            info,
            BigInteger.Zero,
            (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN));
        }
      }
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        valueFpMantissa |= 1 << 23;
      }
      if (valueFpMantissa == 0) {
        return neg ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
      }
      floatExponent -= 150;
      while ((valueFpMantissa & 1) == 0) {
        ++floatExponent;
        valueFpMantissa >>= 1;
      }
      if (floatExponent == 0) {
        if (neg) {
          valueFpMantissa = -valueFpMantissa;
        }
        return ExtendedDecimal.FromInt64(valueFpMantissa);
      } else if (floatExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = (BigInteger)valueFpMantissa;
        bigmantissa <<= floatExponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = (BigInteger)valueFpMantissa;
        BigInteger bigexponent = DecimalUtility.FindPowerOfFive(-floatExponent);
        bigmantissa *= (BigInteger)bigexponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.Create(bigmantissa, (BigInteger)floatExponent);
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 64-bit signed integer.</summary>
    /// <param name='valueSmall'>A 64-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal FromInt64(long valueSmall) {
      BigInteger bigint = (BigInteger)valueSmall;
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 32-bit signed integer.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='valueSmaller'>A 32-bit signed integer.</param>
    public static ExtendedDecimal FromInt32(int valueSmaller) {
      BigInteger bigint = (BigInteger)valueSmaller;
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 64-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting the
    /// number to a string.</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A decimal number with the same value as <paramref name='dbl'/>.</returns>
    public static ExtendedDecimal FromDouble(double dbl) {
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
        ++floatExponent;
      } else {
        value[1] |= 0x100000;
      }
      if ((value[1] | value[0]) != 0) {
        floatExponent += DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      } else {
        return neg ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
      }
      floatExponent -= 1075;
      BigInteger valueFpMantissaBig = FastInteger.WordsToBigInteger(value);
      if (floatExponent == 0) {
        if (neg) {
          valueFpMantissaBig = -valueFpMantissaBig;
        }
        return ExtendedDecimal.FromBigInteger(valueFpMantissaBig);
      } else if (floatExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = (BigInteger)valueFpMantissaBig;
        bigmantissa <<= floatExponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = (BigInteger)valueFpMantissaBig;
        BigInteger exp = DecimalUtility.FindPowerOfFive(-floatExponent);
        bigmantissa *= (BigInteger)exp;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.Create(bigmantissa, (BigInteger)floatExponent);
      }
    }

    /// <summary>Creates a decimal number from an arbitrary-precision
    /// binary floating-point number.</summary>
    /// <param name='bigfloat'>A big floating-point number.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
      if (bigfloat == null) {
        throw new ArgumentNullException("bigfloat");
      }
      if (bigfloat.IsNaN() || bigfloat.IsInfinity()) {
        int flags = (bigfloat.IsNegative ? BigNumberFlags.FlagNegative : 0) |
          (bigfloat.IsInfinity() ? BigNumberFlags.FlagInfinity : 0) |
          (bigfloat.IsQuietNaN() ? BigNumberFlags.FlagQuietNaN : 0) |
          (bigfloat.IsSignalingNaN() ? BigNumberFlags.FlagSignalingNaN : 0);
        return CreateWithFlags(
          bigfloat.UnsignedMantissa,
          bigfloat.Exponent,
          flags);
      }
      BigInteger bigintExp = bigfloat.Exponent;
      BigInteger bigintMant = bigfloat.Mantissa;
      if (bigintMant.IsZero) {
        return bigfloat.IsNegative ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
      }
      if (bigintExp.IsZero) {
        // Integer
        return ExtendedDecimal.FromBigInteger(bigintMant);
      } else if (bigintExp.Sign > 0) {
        // Scaled integer
        FastInteger intcurexp = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = bigmantissa.Sign < 0;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        while (intcurexp.Sign > 0) {
          int shift = 512;
          if (intcurexp.CompareToInt(512) < 0) {
            shift = intcurexp.AsInt32();
          }
          bigmantissa <<= shift;
          intcurexp.AddInt(-shift);
        }
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger negbigintExp = -(BigInteger)bigintExp;
        negbigintExp = DecimalUtility.FindPowerOfFiveFromBig(negbigintExp);
        bigmantissa *= (BigInteger)negbigintExp;
        return ExtendedDecimal.Create(bigmantissa, bigintExp);
      }
    }

    /// <summary>Converts this value to a string. Returns a value compatible
    /// with this class's FromString method.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.ToStringInternal(0);
    }

    /// <summary>Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows the
    /// format of the java.math.BigDecimal.toEngineeringString() method.</summary>
    /// <returns>A string object.</returns>
    public string ToEngineeringString() {
      return this.ToStringInternal(1);
    }

    /// <summary>Converts this value to a string, but without an exponent
    /// part. The format of the return value follows the format of the java.math.BigDecimal.toPlainString()
    /// method.</summary>
    /// <returns>A string object.</returns>
    public string ToPlainString() {
      return this.ToStringInternal(2);
    }

    /// <summary>Represents the number 1.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedDecimal is immutable")]
    #endif
    public static readonly ExtendedDecimal One = ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedDecimal is immutable")]
    #endif
    public static readonly ExtendedDecimal Zero = ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedDecimal is immutable")]
    #endif
    public static readonly ExtendedDecimal NegativeZero = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagNegative);

    /// <summary>Represents the number 10.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification="ExtendedDecimal is immutable")]
    #endif

    public static readonly ExtendedDecimal Ten = ExtendedDecimal.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedDecimal NaN = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagQuietNaN);

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it&apos;s passed as an argument to any arithmetic operation
    /// in ExtendedDecimal.</summary>
    public static readonly ExtendedDecimal SignalingNaN = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagSignalingNaN);

    /// <summary>Positive infinity, greater than any other number.</summary>
    public static readonly ExtendedDecimal PositiveInfinity = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedDecimal NegativeInfinity = CreateWithFlags(
      BigInteger.Zero,
      BigInteger.Zero,
      BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity() {
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

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>Whether this object is finite (not infinity or NaN).</value>
    public bool IsFinite {
      get {
        return (this.flags & (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNaN)) == 0;
      }
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
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Abs() {
      return this.Abs(null);
    }

    /// <summary>Gets an object with the same value as this one, but with the
    /// sign reversed.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Negate() {
      return this.Negate(null);
    }

    /// <summary>Divides this object by another decimal number and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the result can't be exact
    /// because it would have a nonterminating decimal expansion.</returns>
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
      return this.Divide(divisor, PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /// <summary>Divides this object by another decimal number and returns
    /// a result with the same exponent as this object (the dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToSameExponent(ExtendedDecimal divisor, Rounding rounding) {
      return this.DivideToExponent(divisor, this.exponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedDecimal objects, and returns the
    /// integer part of the result, rounded down, with the preferred exponent
    /// set to this value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects. Signals
    /// FlagDivideByZero and returns infinity if the divisor is 0 and the
    /// dividend is nonzero. Signals FlagInvalid and returns NaN if the divisor
    /// and the dividend are 0.</returns>
    public ExtendedDecimal DivideToIntegerNaturalScale(
      ExtendedDecimal divisor) {
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
    public ExtendedDecimal Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>An ExtendedDecimal object. (2).</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal RemainderNaturalScale(
      ExtendedDecimal divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='divisor'>An ExtendedDecimal object. (2).</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal RemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, null).Multiply(divisor, null),
        ctx);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. If the precision given in the context is other than 0,
    /// calls the Quantize method with both arguments equal to the result
    /// of the operation (and can signal FlagInvalid and return NaN if the
    /// result doesn&apos;t fit the given precision). If HasFlags of the
    /// context is true, will also store the flags resulting from the operation
    /// (the flags are in addition to the pre-existing flags). Can be null,
    /// in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the context defines
    /// an exponent range and the desired exponent is outside that range.
    /// Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, ctx);
    }

    /// <summary>Divides this ExtendedDecimal object by another ExtendedDecimal
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
    /// are 0; or, either <paramref name='ctx'/> is null or <paramref name='ctx'/>'s
    /// precision is 0, and the result would have a nonterminating decimal
    /// expansion; or, the rounding mode is Rounding.Unnecessary and the
    /// result is not exact.</returns>
    public ExtendedDecimal Divide(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return math.Divide(this, divisor, ctx);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      return this.DivideToExponent(divisor, (BigInteger)desiredExponentSmall, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A positive
    /// number places the cutoff point to the left of the usual decimal point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. If the precision given in the context is other than 0,
    /// calls the Quantize method with both arguments equal to the result
    /// of the operation (and can signal FlagInvalid and return NaN if the
    /// result doesn&apos;t fit the given precision). If HasFlags of the
    /// context is true, will also store the flags resulting from the operation
    /// (the flags are in addition to the pre-existing flags). Can be null,
    /// in which case the default rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Signals FlagInvalid and returns NaN if the divisor and the dividend
    /// are 0. Signals FlagInvalid and returns NaN if the context defines
    /// an exponent range and the desired exponent is outside that range.
    /// Signals FlagInvalid and returns NaN if the rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is nonzero.
    /// Returns NaN if the divisor and the dividend are 0. Returns NaN if the
    /// rounding mode is Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
      return this.DivideToExponent(divisor, desiredExponent, PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Finds the absolute value of this object (if it&apos;s negative,
    /// it becomes positive).</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    public ExtendedDecimal Abs(PrecisionContext context) {
      return math.Abs(this, context);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Negate(PrecisionContext context) {
      return math.Negate(this, context);
    }

    /// <summary>Adds this object and another decimal number and returns
    /// the result.</summary>
    /// <param name='numberObject'>An ExtendedDecimal object.</param>
    /// <returns>The sum of the two objects.</returns>
    public ExtendedDecimal Add(ExtendedDecimal numberObject) {
      return this.Add(numberObject, PrecisionContext.Unlimited);
    }

    /// <summary>Subtracts an ExtendedDecimal object from this instance
    /// and returns the result.</summary>
    /// <param name='numberObject'>An ExtendedDecimal object.</param>
    /// <returns>The difference of the two objects.</returns>
    public ExtendedDecimal Subtract(ExtendedDecimal numberObject) {
      return this.Subtract(numberObject, null);
    }

    /// <summary>Subtracts an ExtendedDecimal object from this instance.</summary>
    /// <param name='numberObject'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    public ExtendedDecimal Subtract(ExtendedDecimal numberObject, PrecisionContext ctx) {
      if (numberObject == null) {
        throw new ArgumentNullException("numberObject");
      }
      ExtendedDecimal negated = numberObject;
      if ((numberObject.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = numberObject.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(numberObject.unsignedMantissa, numberObject.exponent, newflags);
      }
      return this.Add(negated, ctx);
    }

    /// <summary>Multiplies two decimal numbers. The resulting exponent
    /// will be the sum of the exponents of the two decimal numbers.</summary>
    /// <param name='numberObject'>Another decimal number.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    public ExtendedDecimal Multiply(ExtendedDecimal numberObject) {
      return this.Multiply(numberObject, PrecisionContext.Unlimited);
    }

    /// <summary>Multiplies by one decimal number, and then adds another
    /// decimal number.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * <paramref name='multiplicand'/> + <paramref
    /// name='augend'/>.</returns>
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static IRadixMath<ExtendedDecimal> math = new TrappableRadixMath<ExtendedDecimal>(
      new RadixMath<ExtendedDecimal>(new DecimalMathHelper()));

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&apos;s exponent minus the divisor&apos;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision,
    /// rounding, and exponent range of the integer part of the result. Flags
    /// will be set on the given context only if the context&apos;s HasFlags
    /// is true and the integer part of the result doesn&apos;t fit the precision
    /// and exponent range without rounding.</param>
    /// <returns>The integer part of the quotient of the two objects. Signals
    /// FlagInvalid and returns NaN if the return value would overflow the
    /// exponent range. Signals FlagDivideByZero and returns infinity
    /// if the divisor is 0 and the dividend is nonzero. Signals FlagInvalid
    /// and returns NaN if the divisor and the dividend are 0. Signals FlagInvalid
    /// and returns NaN if the rounding mode is Rounding.Unnecessary and
    /// the result is not exact.</returns>
    public ExtendedDecimal DivideToIntegerNaturalScale(
      ExtendedDecimal divisor,
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
    public ExtendedDecimal DivideToIntegerZeroScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two ExtendedDecimal
    /// objects.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public ExtendedDecimal Remainder(
      ExtendedDecimal divisor,
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
    public ExtendedDecimal RemainderNear(
      ExtendedDecimal divisor,
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
    /// Returns negative infinity if the result is negative infinity. Signals
    /// FlagInvalid and returns NaN if the parameter <paramref name='ctx'/>
    /// is null, the precision is 0, or <paramref name='ctx'/> has an unlimited
    /// exponent range.</returns>
    public ExtendedDecimal NextMinus(
      PrecisionContext ctx) {
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
    /// value.Signals FlagInvalid and returns NaN if the parameter <paramref
    /// name='ctx'/> is null, the precision is 0, or <paramref name='ctx'/>
    /// has an unlimited exponent range.</returns>
    public ExtendedDecimal NextPlus(
      PrecisionContext ctx) {
      return math.NextPlus(this, ctx);
    }

    /// <summary>Finds the next value that is closer to the other object&apos;s
    /// value than this object&apos;s value.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. If HasFlags of the context is true, will also store the
    /// flags resulting from the operation (the flags are in addition to the
    /// pre-existing flags).</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value. Signals FlagInvalid and returns
    /// NaN if the parameter <paramref name='ctx'/> is null, the precision
    /// is 0, or <paramref name='ctx'/> has an unlimited exponent range.</returns>
    public ExtendedDecimal NextToward(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return math.NextToward(this, otherValue, ctx);
    }

    /// <summary>Gets the greater value between two decimal numbers.</summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return math.Max(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two decimal numbers.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return math.Min(first, second, ctx);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return math.MaxMagnitude(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='first'>An ExtendedDecimal object. (2).</param>
    /// <param name='second'>An ExtendedDecimal object. (3).</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return math.MinMagnitude(first, second, ctx);
    }

    /// <summary>Gets the greater value between two decimal numbers.</summary>
    /// <returns>The larger value of the two objects.</returns>
    /// <param name='first'>An ExtendedDecimal object.</param>
    /// <param name='second'>An ExtendedDecimal object. (2).</param>
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Max(first, second, null);
    }

    /// <summary>Gets the lesser value between two decimal numbers.</summary>
    /// <returns>The smaller value of the two objects.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Min(first, second, null);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return MaxMagnitude(first, second, null);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
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
    /// <param name='other'>An ExtendedDecimal object.</param>
    public int CompareTo(
      ExtendedDecimal other) {
      return math.CompareTo(this, other);
    }

    /// <summary>Compares the mathematical values of this object and another
    /// object.<para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or signaling
    /// NaN, this method returns a quiet NaN, and will signal a FlagInvalid
    /// flag if either is a signaling NaN.</para>
    /// </summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding,
    /// and exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0 if
    /// both objects have the same value, or -1 if this object is less than the
    /// other value, or 1 if this object is greater.</returns>
    public ExtendedDecimal CompareToWithContext(
      ExtendedDecimal other,
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
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding,
    /// and exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0 if
    /// both objects have the same value, or -1 if this object is less than the
    /// other value, or 1 if this object is greater.</returns>
    public ExtendedDecimal CompareToSignal(
      ExtendedDecimal other,
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
    public ExtendedDecimal Add(
      ExtendedDecimal numberObject,
      PrecisionContext ctx) {
      return math.Add(this, numberObject, ctx);
    }

    /// <summary>Returns a decimal number with the same value but a new exponent.
    /// <para>Note that this is not always the same as rounding to a given number
    /// of decimal places, since it can fail if the difference between this
    /// value's exponent and the desired exponent is too big, depending on
    /// the maximum precision. If rounding to a number of decimal places is
    /// desired, it's better to use the RoundToExponent and RoundToIntegral
    /// methods instead.</para>
    /// </summary>
    /// <returns>A decimal number with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the rounded result can't fit the given precision,
    /// or if the context defines an exponent range and the given exponent
    /// is outside that range.</returns>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    public ExtendedDecimal Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(ExtendedDecimal.Create(BigInteger.One, desiredExponent), ctx);
    }

    /// <summary>Returns a decimal number with the same value as this one
    /// but a new exponent.</summary>
    /// <returns>A decimal number with the same value as this object but with
    /// the exponent changed. Returns NaN if the rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</returns>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='rounding'>A Rounding object.</param>
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      Rounding rounding) {
      return this.Quantize(
        ExtendedDecimal.Create(BigInteger.One, (BigInteger)desiredExponentSmall),
        PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Returns a decimal number with the same value but a new exponent.<para>Note
    /// that this is not always the same as rounding to a given number of decimal
    /// places, since it can fail if the difference between this value's exponent
    /// and the desired exponent is too big, depending on the maximum precision.
    /// If rounding to a number of decimal places is desired, it's better to
    /// use the RoundToExponent and RoundToIntegral methods instead.</para>
    /// </summary>
    /// <returns>A decimal number with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the rounded result can't fit the given precision,
    /// or if the context defines an exponent range and the given exponent
    /// is outside that range.</returns>
    /// <param name='desiredExponentSmall'>The desired exponent for
    /// the result. The exponent is the number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3 means
    /// round to the thousandth (10^-3, 0.0001), and 3 means round to the thousand
    /// (10^3, 1000). A value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(ExtendedDecimal.Create(BigInteger.One, (BigInteger)desiredExponentSmall), ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but with the same exponent as another decimal number.</summary>
    /// <param name='otherValue'>A decimal number containing the desired
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
    /// <returns>A decimal number with the same value as this object but with
    /// the exponent changed. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</returns>
    public ExtendedDecimal Quantize(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but rounded to an integer.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</returns>
    public ExtendedDecimal RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but rounded to an integer, without adding the FlagInexact or FlagRounded
    /// flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will also
    /// store the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags), except that this function will never
    /// add the FlagRounded and FlagInexact flags (the only difference between
    /// this and RoundToExponentExact). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// must be changed to 0 when rounding and 0 is outside of the valid range
    /// of the precision context, if it defines an exponent range.</returns>
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(
      PrecisionContext ctx) {
      return math.RoundToExponentNoRoundedFlag(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but rounded to an integer, and signals an invalid operation if the
    /// result would be inexact.</summary>
    /// <returns>A decimal number with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</returns>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result, expressed
    /// as a negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the thousandth
    /// (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
    /// value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedDecimal RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentExact(this, exponent, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object,
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
    /// <returns>A decimal number rounded to the closest value representable
    /// in the given precision, meaning if the result can't fit the precision,
    /// additional digits are discarded to make it fit. Signals FlagInvalid
    /// and returns NaN if the new exponent must be changed when rounding and
    /// the new exponent is outside of the valid range of the precision context,
    /// if it defines an exponent range.</returns>
    public ExtendedDecimal RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return math.RoundToExponentSimple(this, exponent, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object
    /// but rounded to an integer, and signals an invalid operation if the
    /// result would be inexact.</summary>
    /// <returns>A decimal number with the same value as this object but rounded
    /// to an integer. Signals FlagInvalid and returns NaN if an overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the new exponent
    /// is outside of the valid range of the precision context, if it defines
    /// an exponent range.</returns>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which eliminates
    /// lower-order places from the number. For example, -3 means round to
    /// the thousandth (10^-3, 0.0001), and 3 means round to the thousand
    /// (10^3, 1000). A value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    public ExtendedDecimal RoundToExponentExact(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponentExact((BigInteger)exponentSmall, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this object,
    /// and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which eliminates
    /// lower-order places from the number. For example, -3 means round to
    /// the thousandth (10^-3, 0.0001), and 3 means round to the thousand
    /// (10^3, 1000). A value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null, in which case
    /// the default rounding mode is HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value representable
    /// in the given precision, meaning if the result can't fit the precision,
    /// additional digits are discarded to make it fit. Signals FlagInvalid
    /// and returns NaN if the new exponent must be changed when rounding and
    /// the new exponent is outside of the valid range of the precision context,
    /// if it defines an exponent range.</returns>
    public ExtendedDecimal RoundToExponent(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponent((BigInteger)exponentSmall, ctx);
    }

    /// <summary>Multiplies two decimal numbers. The resulting scale will
    /// be the sum of the scales of the two decimal numbers. The result&apos;s
    /// sign is positive if both operands have the same sign, and negative
    /// if they have different signs.</summary>
    /// <param name='op'>Another decimal number.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    public ExtendedDecimal Multiply(
      ExtendedDecimal op,
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
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal op,
      ExtendedDecimal augend,
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
    public ExtendedDecimal MultiplyAndSubtract(
      ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
      if (subtrahend == null) {
        throw new ArgumentNullException("numberObject");
      }
      ExtendedDecimal negated = subtrahend;
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
    public ExtendedDecimal RoundToPrecision(
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
    public ExtendedDecimal Plus(
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
    public ExtendedDecimal RoundToBinaryPrecision(
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
    /// value, but the return value is still NaN). Signals FlagInvalid and
    /// returns NaN if the parameter <paramref name='ctx'/> is null or the
    /// precision is unlimited (the context's Precision property is 0).</returns>
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
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
    /// returns an approximation to " e" within the given precision. Signals
    /// FlagInvalid and returns NaN if the parameter <paramref name='ctx'/>
    /// is null or the precision is unlimited (the context's Precision property
    /// is 0).</returns>
    public ExtendedDecimal Exp(PrecisionContext ctx) {
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
    /// part equal to pi, but the return value is still NaN.). Signals FlagInvalid
    /// and returns NaN if the parameter <paramref name='ctx'/> is null or
    /// the precision is unlimited (the context's Precision property is
    /// 0). Signals no flags and returns negative infinity if this object's
    /// value is 0.</returns>
    public ExtendedDecimal Log(PrecisionContext ctx) {
      return math.Ln(this, ctx);
    }

    /// <summary>Finds the base-10 logarithm of this object, that is, the
    /// exponent that the number 10 must be raised to in order to equal this
    /// object&apos;s value.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as the ln function&apos;s results are generally not exact.--.</param>
    /// <returns>Ln(this object)/Ln(10). Signals the flag FlagInvalid
    /// and returns NaN if this object is less than 0. Signals FlagInvalid
    /// and returns NaN if the parameter <paramref name='ctx'/> is null or
    /// the precision is unlimited (the context's Precision property is
    /// 0).</returns>
    public ExtendedDecimal Log10(PrecisionContext ctx) {
      return math.Log10(this, ctx);
    }

    /// <summary>Raises this object&apos;s value to the given exponent.</summary>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0; or if this value is less than
    /// 0 and the exponent either has a fractional part or is infinity. Signals
    /// FlagInvalid and returns NaN if the parameter <paramref name='ctx'/>
    /// is null or the precision is unlimited (the context's Precision property
    /// is 0), and the exponent has a fractional part.</returns>
    /// <param name='exponent'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags).</param>
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
      return math.Power(this, exponent, ctx);
    }

    /// <summary>Raises this object&apos;s value to the given exponent.</summary>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0.</returns>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags).</param>
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), ctx);
    }

    /// <summary>Raises this object&apos;s value to the given exponent.</summary>
    /// <returns>This^exponent. Returns NaN if this object and exponent
    /// are both 0.</returns>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    public ExtendedDecimal Pow(int exponentSmall) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), null);
    }

    /// <summary>Finds the constant pi.</summary>
    /// <param name='ctx'>A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). --This parameter cannot
    /// be null, as pi can never be represented exactly.--.</param>
    /// <returns>Pi rounded to the given precision. Signals FlagInvalid
    /// and returns NaN if the parameter <paramref name='ctx'/> is null or
    /// the precision is unlimited (the context's Precision property is
    /// 0).</returns>
    public static ExtendedDecimal PI(PrecisionContext ctx) {
      return math.Pi(ctx);
    }
  }
}
