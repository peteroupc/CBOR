/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Text;

namespace PeterO {
    /// <summary>Represents an arbitrary-precision decimal floating-point
    /// number. Consists of an integer mantissa and an integer exponent,
    /// both arbitrary-precision. The value of the number equals mantissa *
    /// 10^exponent.
    /// <para>The mantissa is the value of the digits that make up a
    /// number, ignoring the decimal point and exponent. For example, in
    /// the number 2356.78, the mantissa is 235678. The exponent is where
    /// the "floating" decimal point of the number is located. A positive
    /// exponent means "move it to the right", and a negative exponent
    /// means "move it to the left." In the example 2, 356.78, the exponent
    /// is -2, since it has 2 decimal places and the decimal point is
    /// "moved to the left by 2." Therefore, in the ExtendedDecimal
    /// representation, this number would be stored as 235678 *
    /// 10^-2.</para>
    /// <para>The mantissa and exponent format preserves trailing zeros in
    /// the number's value. This may give rise to multiple ways to store
    /// the same value. For example, 1.00 and 1 would be stored
    /// differently, even though they have the same value. In the first
    /// case, 100 * 10^-2 (100 with decimal point moved left by 2), and in
    /// the second case, 1 * 10^0 (1 with decimal point moved 0).</para>
    /// <para>This class also supports values for negative zero,
    /// not-a-number (NaN) values, and infinity. <b>Negative zero</b> is
    /// generally used when a negative number is rounded to 0; it has the
    /// same mathematical value as positive zero. <b>Infinity</b> is
    /// generally used when a non-zero number is divided by zero, or when a
    /// very high number can't be represented in a given exponent range.
    /// <b>Not-a-number</b> is generally used to signal errors.</para>
    /// <para>This class implements the General Decimal Arithmetic
    /// Specification version 1.70:
    /// <c>http://speleotrove.com/decimal/decarith.html</c></para>
    /// <para>Passing a signaling NaN to any arithmetic operation shown
    /// here will signal the flag FlagInvalid and return a quiet NaN, even
    /// if another operand to that operation is a quiet NaN, unless noted
    /// otherwise.</para>
    /// <para>Passing a quiet NaN to any arithmetic operation shown here
    /// will return a quiet NaN, unless noted otherwise. Invalid operations
    /// will also return a quiet NaN, as stated in the individual
    /// methods.</para>
    /// <para>Unless noted otherwise, passing a null ExtendedDecimal
    /// argument to any method here will throw an exception.</para>
    /// <para>When an arithmetic operation signals the flag FlagInvalid,
    /// FlagOverflow, or FlagDivideByZero, it will not throw an exception
    /// too, unless the flag's trap is enabled in the precision context
    /// (see PrecisionContext's Traps property).</para>
    /// <para>An ExtendedDecimal value can be serialized in one of the
    /// following ways:</para>
    /// <list>
    /// <item>By calling the toString() method, which will always return
    /// distinct strings for distinct ExtendedDecimal values.</item>
    /// <item>By calling the UnsignedMantissa, Exponent, and IsNegative
    /// properties, and calling the IsInfinity, IsQuietNaN, and
    /// IsSignalingNaN methods. The return values combined will uniquely
    /// identify a particular ExtendedDecimal
    /// value.</item></list></summary>
  public sealed class ExtendedDecimal : IComparable<ExtendedDecimal>,
  IEquatable<ExtendedDecimal> {
    private const int MaxSafeInt = 214748363;

    private readonly BigInteger exponent;
    private readonly BigInteger unsignedMantissa;
    private readonly int flags;

    /// <summary>Gets this object&#x27;s exponent. This object&#x27;s value
    /// will be an integer if the exponent is positive or zero.</summary>
    /// <value>This object&apos;s exponent. This object&apos;s value will
    /// be an integer if the exponent is positive or zero.</value>
    public BigInteger Exponent {
      get {
        return this.exponent;
      }
    }

    /// <summary>Gets the absolute value of this object&#x27;s un-scaled
    /// value.</summary>
    /// <value>The absolute value of this object&apos;s un-scaled
    /// value.</value>
    public BigInteger UnsignedMantissa {
      get {
        return this.unsignedMantissa;
      }
    }

    /// <summary>Gets this object&#x27;s un-scaled value.</summary>
    /// <value>This object&apos;s un-scaled value. Will be negative if this
    /// object&apos;s value is negative (including a negative NaN).</value>
    public BigInteger Mantissa {
      get {
        return this.IsNegative ? (-(BigInteger)this.unsignedMantissa) :
          this.unsignedMantissa;
      }
    }

    #region Equals and GetHashCode implementation
    private bool EqualsInternal(ExtendedDecimal otherValue) {
      return (otherValue != null) && (this.flags == otherValue.flags &&
                    this.unsignedMantissa.Equals(otherValue.unsignedMantissa) &&
                this.exponent.Equals(otherValue.exponent));
    }

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object.</summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>True if this object's mantissa and exponent are equal to
    /// those of another object; otherwise, false.</returns>
    public bool Equals(ExtendedDecimal other) {
      return this.EqualsInternal(other);
    }

    /// <summary>Determines whether this object&#x27;s mantissa and
    /// exponent are equal to those of another object and that other object
    /// is a decimal fraction.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object obj) {
      return this.EqualsInternal(obj as ExtendedDecimal);
    }

    /// <summary>Calculates this object&#x27;s hash code.</summary>
    /// <returns>This object's hash code.</returns>
    public override int GetHashCode() {
      int hashCode = 964453631;
      unchecked {
        hashCode += 964453723 * this.exponent.GetHashCode();
        hashCode += 964453939 * this.unsignedMantissa.GetHashCode();
        hashCode += 964453967 * this.flags;
      }
      return hashCode;
    }
    #endregion

    /// <summary>Creates a number with the value
    /// exponent*10^mantissa.</summary>
    /// <param name='mantissaSmall'>The un-scaled value.</param>
    /// <param name='exponentSmall'>The decimal exponent.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal Create(int mantissaSmall, int exponentSmall) {
      return Create((BigInteger)mantissaSmall, (BigInteger)exponentSmall);
    }

    /// <summary>Creates a number with the value
    /// exponent*10^mantissa.</summary>
    /// <param name='mantissa'>The un-scaled value.</param>
    /// <param name='exponent'>The decimal exponent.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='mantissa'/> or <paramref name='exponent'/> is
    /// null.</exception>
    public static ExtendedDecimal Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      int sign = mantissa.Sign;
      return new ExtendedDecimal(
        sign < 0 ? (-(BigInteger)mantissa) : mantissa,
        exponent,
        (sign < 0) ? BigNumberFlags.FlagNegative : 0);
    }

    private ExtendedDecimal(
      BigInteger unsignedMantissa,
      BigInteger exponent,
      int flags) {
#if DEBUG
      if (unsignedMantissa == null) {
        throw new ArgumentNullException("unsignedMantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      if (unsignedMantissa.Sign < 0) {
        throw new ArgumentException("unsignedMantissa is less than 0.");
      }
#endif
      this.unsignedMantissa = unsignedMantissa;
      this.exponent = exponent;
      this.flags = flags;
    }

    internal static ExtendedDecimal CreateWithFlags(
      BigInteger mantissa,
      BigInteger exponent,
      int flags) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      int sign = mantissa == null ? 0 : mantissa.Sign;
      return new ExtendedDecimal(
        sign < 0 ? (-(BigInteger)mantissa) : mantissa,
        exponent,
        flags);
    }

    /// <summary>Creates a not-a-number ExtendedDecimal object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <returns>A quiet not-a-number object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null or is less than 0.</exception>
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      return CreateNaN(diag, false, false, null);
    }

    /// <summary>Creates a not-a-number ExtendedDecimal object.</summary>
    /// <param name='diag'>A number to use as diagnostic information
    /// associated with this object. If none is needed, should be
    /// zero.</param>
    /// <param name='signaling'>Whether the return value will be signaling
    /// (true) or quiet (false).</param>
    /// <param name='negative'>Whether the return value is
    /// negative.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='diag'/> is null or is less than 0.</exception>
    public static ExtendedDecimal CreateNaN(
      BigInteger diag,
      bool signaling,
      bool negative,
      PrecisionContext ctx) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      if (diag.Sign < 0) {
        throw new
       ArgumentException("Diagnostic information must be 0 or greater, was: " +
                    diag);
      }
      if (diag.IsZero && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      int flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      if (ctx != null && ctx.HasMaxPrecision) {
        flags |= BigNumberFlags.FlagQuietNaN;
        ExtendedDecimal ef = CreateWithFlags(
          diag,
          BigInteger.Zero,
          flags).RoundToPrecision(ctx);
        int newFlags = ef.flags;
        newFlags &= ~BigNumberFlags.FlagQuietNaN;
        newFlags |= signaling ? BigNumberFlags.FlagSignalingNaN :
          BigNumberFlags.FlagQuietNaN;
        return new ExtendedDecimal(ef.unsignedMantissa, ef.exponent, newFlags);
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      return CreateWithFlags(diag, BigInteger.Zero, flags);
    }

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(String str) {
      return FromString(str, 0, str == null ? 0 : str.Length, null);
    }

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(String str, PrecisionContext ctx) {
      return FromString(str, 0, str == null ? 0 : str.Length, ctx);
    }

    /// <summary>Creates a decimal number from a string that represents a
    /// number. See <c>FromString(String, int, int, PrecisionContext)</c>
    /// for more information.</summary>
    /// <param name='str'>A string that represents a number.</param>
    /// <param name='offset'>A 32-bit signed integer.</param>
    /// <param name='length'>A 32-bit signed integer. (2).</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(
      String str,
      int offset,
      int length) {
      return FromString(str, offset, length, null);
    }

    /// <summary>
    /// <para>Creates a decimal number from a string that represents a
    /// number.</para>
    /// <para>The format of the string generally consists of:</para>
    /// <list type=''>
    /// <item>An optional '-' or '+' character (if '-' , the value is
    /// negative.)</item>
    /// <item>One or more digits, with a single optional decimal point
    /// after the first digit and before the last digit.</item>
    /// <item>Optionally, E+ (positive exponent) or E- (negative exponent)
    /// plus one or more digits specifying the exponent.</item></list>
    /// <para>The string can also be "-INF", "-Infinity" , "Infinity",
    /// "INF" , quiet NaN ("qNaN" /"-qNaN") followed by any number of
    /// digits, or signaling NaN ("sNaN" /"-sNaN") followed by any number
    /// of digits, all in any combination of upper and lower case.</para>
    /// <para>The format generally follows the definition in
    /// java.math.BigDecimal(), except that the digits must be ASCII digits
    /// ('0' through '9').</para></summary>
    /// <param name='str'>A string object, a portion of which represents a
    /// number.</param>
    /// <param name='offset'>A zero-based index that identifies the start
    /// of the number.</param>
    /// <param name='length'>The length of the number within the
    /// string.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An arbitrary-precision decimal number with the same value
    /// as the given string.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='FormatException'>The parameter <paramref
    /// name='str'/> is not a correctly formatted number
    /// string.</exception>
    public static ExtendedDecimal FromString(
      String str,
      int offset,
      int length,
      PrecisionContext ctx) {
      int tmpoffset = offset;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (tmpoffset < 0) {
        throw new FormatException("offset (" + tmpoffset + ") is less than " +
                    "0");
      }
      if (tmpoffset > str.Length) {
        throw new FormatException("offset (" + tmpoffset + ") is more than " +
                    str.Length);
      }
      if (length < 0) {
        throw new FormatException("length (" + length + ") is less than " +
                    "0");
      }
      if (length > str.Length) {
        throw new FormatException("length (" + length + ") is more than " +
                    str.Length);
      }
      if (str.Length - tmpoffset < length) {
        throw new FormatException("str's length minus " + tmpoffset + " (" +
                    (str.Length - tmpoffset) + ") is less than " + length);
      }
      if (length == 0) {
        throw new FormatException();
      }
      bool negative = false;
      int endStr = tmpoffset + length;
      if (str[0] == '+' || str[0] == '-') {
        negative = str[0] == '-';
        ++tmpoffset;
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
      int i = tmpoffset;
      if (i + 8 == endStr) {
        if ((str[i] == 'I' || str[i] == 'i') &&
            (str[i + 1] == 'N' || str[i + 1] == 'n') &&
            (str[i + 2] == 'F' || str[i + 2] == 'f') &&
            (str[i + 3] == 'I' || str[i + 3] == 'i') && (str[i + 4] == 'N' ||
                    str[i + 4] == 'n') && (str[i + 5] ==
                    'I' || str[i + 5] == 'i') &&
            (str[i + 6] == 'T' || str[i + 6] == 't') && (str[i + 7] == 'Y' ||
                    str[i + 7] == 'y')) {
          if (ctx != null && ctx.IsSimplified && i < endStr) {
            throw new FormatException("Infinity not allowed");
          }
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 == endStr) {
        if ((str[i] == 'I' || str[i] == 'i') &&
            (str[i + 1] == 'N' || str[i + 1] == 'n') && (str[i + 2] == 'F' ||
                    str[i + 2] == 'f')) {
          if (ctx != null && ctx.IsSimplified && i < endStr) {
            throw new FormatException("Infinity not allowed");
          }
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 <= endStr) {
        // Quiet NaN
        if ((str[i] == 'N' || str[i] == 'n') && (str[i + 1] == 'A' || str[i +
                1] == 'a') && (str[i + 2] == 'N' || str[i + 2] == 'n')) {
          if (ctx != null && ctx.IsSimplified && i < endStr) {
            throw new FormatException("NaN not allowed");
          }
          int flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagQuietNaN;
          if (i + 3 == endStr) {
            return (!negative) ? NaN : CreateWithFlags(
              BigInteger.Zero,
              BigInteger.Zero,
              flags2);
          }
          i += 3;
          var digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && ctx.HasMaxPrecision) {
            maxDigits = FastInteger.FromBig(ctx.Precision);
            if (ctx.ClampNormalExponents) {
              maxDigits.Decrement();
            }
          }
          for (; i < endStr; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              var thisdigit = (int)(str[i] - '0');
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
          BigInteger bigmant = (mant == null) ? ((BigInteger)mantInt) :
            mant.AsBigInteger();
          flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagQuietNaN;
          return CreateWithFlags(
            bigmant,
            BigInteger.Zero,
            flags2);
        }
      }
      if (i + 4 <= endStr) {
        // Signaling NaN
        if ((str[i] == 'S' || str[i] == 's') && (str[i + 1] == 'N' || str[i +
                    1] == 'n') && (str[i + 2] == 'A' || str[i + 2] == 'a') &&
                (str[i + 3] == 'N' || str[i + 3] == 'n')) {
          if (ctx != null && ctx.IsSimplified && i < endStr) {
            throw new FormatException("NaN not allowed");
          }
          if (i + 4 == endStr) {
            int flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
              BigNumberFlags.FlagSignalingNaN;
            return (!negative) ? SignalingNaN :
              CreateWithFlags(
                BigInteger.Zero,
                BigInteger.Zero,
                flags2);
          }
          i += 4;
          var digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && ctx.HasMaxPrecision) {
            maxDigits = FastInteger.FromBig(ctx.Precision);
            if (ctx.ClampNormalExponents) {
              maxDigits.Decrement();
            }
          }
          for (; i < endStr; ++i) {
            if (str[i] >= '0' && str[i] <= '9') {
              var thisdigit = (int)(str[i] - '0');
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
          int flags3 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagSignalingNaN;
          BigInteger bigmant = (mant == null) ? ((BigInteger)mantInt) :
            mant.AsBigInteger();
          return CreateWithFlags(
            bigmant,
            BigInteger.Zero,
            flags3);
        }
      }
      // Ordinary number
      for (; i < endStr; ++i) {
        if (str[i] >= '0' && str[i] <= '9') {
          var thisdigit = (int)(str[i] - '0');
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
              newScale = newScale ?? (new FastInteger(newScaleInt));
              newScale.Decrement();
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
        tmpoffset = 1;
        haveDigits = false;
        if (i == endStr) {
          throw new FormatException();
        }
        if (str[i] == '+' || str[i] == '-') {
          if (str[i] == '-') {
            tmpoffset = -1;
          }
          ++i;
        }
        for (; i < endStr; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
            var thisdigit = (int)(str[i] - '0');
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
        if (tmpoffset >= 0 && newScaleInt == 0 && newScale == null && exp ==
            null) {
          newScaleInt = expInt;
        } else if (exp == null) {
          newScale = newScale ?? (new FastInteger(newScaleInt));
          if (tmpoffset < 0) {
            newScale.SubtractInt(expInt);
          } else if (expInt != 0) {
            newScale.AddInt(expInt);
          }
        } else {
          newScale = newScale ?? (new FastInteger(newScaleInt));
          if (tmpoffset < 0) {
            newScale.Subtract(exp);
          } else {
            newScale.Add(exp);
          }
        }
      }
      if (i != endStr) {
        throw new FormatException();
      }
      BigInteger bigNewScale = (newScale == null) ? ((BigInteger)newScaleInt) :
        newScale.AsBigInteger();
      var ret = new ExtendedDecimal(
        (mant == null) ? ((BigInteger)mantInt) : mant.AsBigInteger(),
        bigNewScale,
        negative ? BigNumberFlags.FlagNegative : 0);
      if (ctx != null) {
        ret = MathValue.RoundAfterConversion(ret, ctx);
      }
      return ret;
    }

    private sealed class DecimalMathHelper : IRadixMathHelper<ExtendedDecimal> {
    /// <summary>This is an internal method.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetRadix() {
        return 10;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetSign(ExtendedDecimal value) {
        return value.Sign;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetMantissa(ExtendedDecimal value) {
        return value.unsignedMantissa;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetExponent(ExtendedDecimal value) {
        return value.exponent;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer. (2).</param>
    /// <returns>An IShiftAccumulator object.</returns>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(
        BigInteger bigint,
        int lastDigit,
        int olderDigits) {
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>An IShiftAccumulator object.</returns>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint, 0, 0);
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>Another BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
      public bool HasTerminatingRadixExpansion(
        BigInteger numerator,
        BigInteger denominator) {
        // Simplify denominator based on numerator
        BigInteger gcd = BigInteger.GreatestCommonDivisor(
          numerator,
          denominator);
        BigInteger tmpden = denominator;
        tmpden /= gcd;
        if (tmpden.IsZero) {
          return false;
        }
        // Eliminate factors of 2
        while (tmpden.IsEven) {
          tmpden >>= 1;
        }
        // Eliminate factors of 5
        while (true) {
          BigInteger bigrem;
          BigInteger bigquo = BigInteger.DivRem(
            tmpden,
            (BigInteger)5,
            out bigrem);
          if (!bigrem.IsZero) {
            break;
          }
          tmpden = bigquo;
        }
        return tmpden.CompareTo(BigInteger.One) == 0;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='bigint'>Another BigInteger object.</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger MultiplyByRadixPower(
        BigInteger bigint,
        FastInteger power) {
        BigInteger tmpbigint = bigint;
        if (power.Sign <= 0) {
          return tmpbigint;
        }
        if (tmpbigint.IsZero) {
          return tmpbigint;
        }
        BigInteger bigtmp = null;
        if (tmpbigint.CompareTo(BigInteger.One) != 0) {
          if (power.CanFitInInt32()) {
            bigtmp = DecimalUtility.FindPowerOfTen(power.AsInt32());
            tmpbigint *= (BigInteger)bigtmp;
          } else {
            bigtmp = DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
            tmpbigint *= (BigInteger)bigtmp;
          }
          return tmpbigint;
        }
        return power.CanFitInInt32() ?
          DecimalUtility.FindPowerOfTen(power.AsInt32()) :
          DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='value'>An ExtendedDecimal object.</param>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetFlags(ExtendedDecimal value) {
        return value.flags;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>Another BigInteger object.</param>
    /// <param name='flags'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
      public ExtendedDecimal CreateNewWithFlags(
        BigInteger mantissa,
        BigInteger exponent,
        int flags) {
        return CreateWithFlags(mantissa, exponent, flags);
      }

    /// <summary>This is an internal method.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteAndNonFinite;
      }

    /// <summary>This is an internal method.</summary>
    /// <param name='val'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
      public ExtendedDecimal ValueOf(int val) {
        return (val == 0) ? Zero : ((val == 1) ? One : FromInt64(val));
      }
    }

    private static bool AppendString(
      StringBuilder builder,
      char c,
      FastInteger count) {
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
        return this.unsignedMantissa.IsZero ? (negative ? "-sNaN" : "sNaN") :
          (negative ? "-sNaN" + BigInteger.Abs(this.unsignedMantissa) :
           "sNaN" + BigInteger.Abs(this.unsignedMantissa));
      }
      if ((this.flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.unsignedMantissa.IsZero ? (negative ? "-NaN" : "NaN") :
          (negative ? "-NaN" + BigInteger.Abs(this.unsignedMantissa) : "NaN" +
           BigInteger.Abs(this.unsignedMantissa));
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
      var builderLength = new FastInteger(mantissaString.Length);
      FastInteger adjustedExponent = FastInteger.FromBig(this.exponent);
      FastInteger thisExponent = FastInteger.Copy(adjustedExponent);
      adjustedExponent.Add(builderLength).Decrement();
      var decimalPointAdjust = new FastInteger(1);
      var threshold = new FastInteger(-6);
      if (mode == 1) {
        // engineering string adjustments
        FastInteger newExponent = FastInteger.Copy(adjustedExponent);
        bool adjExponentNegative = adjustedExponent.Sign < 0;
        int intphase =
          FastInteger.Copy(adjustedExponent).Abs().Remainder(3).AsInt32();
        if (iszero && (adjustedExponent.CompareTo(threshold) < 0 || scaleSign <
                    0)) {
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
              newExponent.Decrement();
            } else {
              decimalPointAdjust.AddInt(2);
              newExponent.AddInt(-2);
            }
          } else if (intphase == 2) {
            if (adjExponentNegative) {
              decimalPointAdjust.Increment();
              newExponent.Decrement();
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
          FastInteger decimalPoint =
            FastInteger.Copy(thisExponent).Add(builderLength);
          int cmp = decimalPoint.CompareToInt(0);
          StringBuilder builder = null;
          if (cmp < 0) {
            var tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Int32.MaxValue) >
                    0 ? Int32.MaxValue : tmpFast.AsInt32());
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
            var tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Int32.MaxValue) >
                    0 ? Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append("0.");
            builder.Append(
              mantissaString,
              tmpInt,
              mantissaString.Length - tmpInt);
          } else if (decimalPoint.CompareToInt(mantissaString.Length) > 0) {
            FastInteger insertionPoint = builderLength;
            if (!insertionPoint.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            var tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Int32.MaxValue) >
                    0 ? Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            AppendString(
              builder,
              '0',
              FastInteger.Copy(decimalPoint).SubtractInt(builder.Length));
            builder.Append('.');
            builder.Append(
              mantissaString,
              tmpInt,
              mantissaString.Length - tmpInt);
          } else {
            if (!decimalPoint.CanFitInInt32()) {
              throw new NotSupportedException();
            }
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            var tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Int32.MaxValue) >
                    0 ? Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(
              mantissaString,
              tmpInt,
              mantissaString.Length - tmpInt);
          }
          return builder.ToString();
        }
        if (mode == 2 && scaleSign < 0) {
          FastInteger negscale = FastInteger.Copy(thisExponent);
          var builder = new StringBuilder();
          if (negative) {
            builder.Append('-');
          }
          builder.Append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.ToString();
        }
        return (!negative) ? mantissaString : ("-" + mantissaString);
      } else {
        StringBuilder builder = null;
        if (mode == 1 && iszero && decimalPointAdjust.CompareToInt(1) > 0) {
          builder = new StringBuilder();
          if (negative) {
            builder.Append('-');
          }
          builder.Append(mantissaString);
          builder.Append('.');
          AppendString(
            builder,
            '0',
            FastInteger.Copy(decimalPointAdjust).Decrement());
        } else {
          FastInteger tmp = FastInteger.Copy(decimalPointAdjust);
          int cmp = tmp.CompareToInt(mantissaString.Length);
          if (cmp > 0) {
            tmp.SubtractInt(mantissaString.Length);
            builder = new StringBuilder();
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
            var tmpFast = new FastInteger(mantissaString.Length).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Int32.MaxValue) >
                    0 ? Int32.MaxValue : tmpFast.AsInt32());
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append('.');
            builder.Append(
              mantissaString,
              tmpInt,
              mantissaString.Length - tmpInt);
          } else if (adjustedExponent.Sign == 0 && !negative) {
            return mantissaString;
          } else if (adjustedExponent.Sign == 0 && negative) {
            return "-" + mantissaString;
          } else {
            builder = new StringBuilder();
            if (negative) {
              builder.Append('-');
            }
            builder.Append(mantissaString);
          }
        }
        if (adjustedExponent.Sign != 0) {
          builder.Append(adjustedExponent.Sign < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          var builderReversed = new StringBuilder();
          while (adjustedExponent.Sign != 0) {
            int digit =
              FastInteger.Copy(adjustedExponent).Remainder(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.Append((char)('0' + digit));
            adjustedExponent.Divide(10);
          }
          int count = builderReversed.Length;
          for (var i = 0; i < count; ++i) {
            builder.Append(builderReversed[count - 1 - i]);
          }
        }
        return builder.ToString();
      }
    }

    /// <summary>Compares a ExtendedFloat object with this
    /// instance.</summary>
    /// <param name='other'>An ExtendedFloat object.</param>
    /// <returns>Zero if the values are equal; a negative number if this
    /// instance is less, or a positive number if this instance is greater.
    /// Returns 0 if both values are NaN (even signaling NaN) and 1 if this
    /// value is NaN (even signaling NaN) and the other isn't.</returns>
    public int CompareToBinary(ExtendedFloat other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
      }
      int signA = this.Sign;
      int signB = other.Sign;
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      if (this.IsInfinity()) {
        if (other.IsInfinity()) {
          // if we get here, this only means that
          // both are positive infinity or both
          // are negative infinity
          return 0;
        }
        return this.IsNegative ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.IsNegative ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign
#if DEBUG
      if (!this.IsFinite) {
        throw new ArgumentException("doesn't satisfy this.IsFinite");
      }
      if (!other.IsFinite) {
        throw new ArgumentException("doesn't satisfy other.IsFinite");
      }
#endif
      if (other.Exponent.CompareTo((BigInteger)(-1000)) < 0) {
        // For very low exponents, the conversion to decimal can take
        // very long, so try this approach
        if (other.Abs(null).CompareTo(ExtendedFloat.One) < 0) {
          // Abs less than 1
          if (this.Abs(null).CompareTo(ExtendedDecimal.One) >= 0) {
            // Abs 1 or more
            return (signA > 0) ? 1 : -1;
          }
        }
      }
      if (other.Exponent.CompareTo((BigInteger)1000) > 0) {
        // Very high exponents
        BigInteger bignum = BigInteger.One << 999;
        if (this.Abs(null).CompareTo(ExtendedDecimal.FromBigInteger(bignum)) <=
            0) {
          // this object's absolute value is less
          return (signA > 0) ? -1 : 1;
        }
        // NOTE: The following check assumes that both
        // operands are nonzero
        BigInteger thisAdjExp = this.GetAdjustedExponent();
        BigInteger otherAdjExp = GetAdjustedExponentBinary(other);
        if (thisAdjExp.Sign > 0 && thisAdjExp.CompareTo(otherAdjExp) >= 0) {
          // This object's adjusted exponent is greater and is positive;
          // so this object's absolute value is greater, since exponents
          // have a greater value in decimal than in binary
          return (signA > 0) ? 1 : -1;
        }
      if (thisAdjExp.Sign > 0 && thisAdjExp.CompareTo((BigInteger)1000) >= 0 &&
              otherAdjExp.CompareTo((BigInteger)1000) >= 0) {
          thisAdjExp += BigInteger.One;
          otherAdjExp += BigInteger.One;
          BigInteger ratio = otherAdjExp / thisAdjExp;
          // Check the ratio of the binary exponent to the decimal exponent.
          // If the ratio is less than 3, the decimal's absolute value is
          // greater. If it's 4 or greater, the binary' s absolute value is
          // greater.
          // (If the two absolute values are equal, the ratio will approach
          // ln(10)/ln(2), or about 3.322, as the exponents get higher and
          // higher.) This check assumes that both exponents are 1000 or
          // greater,
          // when the ratio between exponents of equal values is close to
          // ln(10)/ln(2).
          if (ratio.CompareTo((BigInteger)3) < 0) {
            // Decimal abs. value is greater
            return (signA > 0) ? 1 : -1;
          }
          if (ratio.CompareTo((BigInteger)4) >= 0) {
            return (signA > 0) ? -1 : 1;
          }
        }
      }
      ExtendedDecimal otherDec = ExtendedDecimal.FromExtendedFloat(other);
      return this.CompareTo(otherDec);
    }

    /// <summary>Converts this value to an arbitrary-precision integer. Any
    /// fractional part in this value will be discarded when converting to
    /// a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    public BigInteger ToBigInteger() {
      return this.ToBigIntegerInternal(false);
    }

    /// <summary>Converts this value to an arbitrary-precision integer,
    /// checking whether the fractional part of the integer would be
    /// lost.</summary>
    /// <returns>A BigInteger object.</returns>
    /// <exception cref='OverflowException'>This object's value is infinity
    /// or NaN.</exception>
    /// <exception cref='ArithmeticException'>This object's value is not an
    /// exact integer.</exception>
    public BigInteger ToBigIntegerExact() {
      return this.ToBigIntegerInternal(true);
    }

    private BigInteger ToBigIntegerInternal(bool exact) {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      int sign = this.Exponent.Sign;
      if (this.IsZero) {
        return BigInteger.Zero;
      }
      if (sign == 0) {
        BigInteger bigmantissa = this.Mantissa;
        return bigmantissa;
      }
      if (sign > 0) {
        BigInteger bigmantissa = this.Mantissa;
        BigInteger bigexponent =
          DecimalUtility.FindPowerOfTenFromBig(this.Exponent);
        bigmantissa *= (BigInteger)bigexponent;
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.Mantissa;
        FastInteger bigexponent = FastInteger.FromBig(this.Exponent).Negate();
        bigmantissa = BigInteger.Abs(bigmantissa);
        var acc = new DigitShiftAccumulator(bigmantissa, 0, 0);
        acc.ShiftRight(bigexponent);
        if (exact && (acc.LastDiscardedDigit != 0 || acc.OlderDiscardedDigits !=
                    0)) {
          // Some digits were discarded
          throw new ArithmeticException("Not an exact integer");
        }
        bigmantissa = acc.ShiftedInt;
        if (this.IsNegative) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      }
    }

    private static BigInteger valueOneShift62 = BigInteger.One << 62;

    /// <summary>Creates a binary floating-point number from this
    /// object&#x27;s value. Note that if the binary floating-point number
    /// contains a negative exponent, the resulting value might not be
    /// exact. However, the resulting binary float will contain enough
    /// precision to accurately convert it to a 32-bit or 64-bit floating
    /// point number (float or double).</summary>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloat() {
      return this.ToExtendedFloatInternal(false);
    }

    private ExtendedFloat ToExtendedFloatInternal(bool oddRounding) {
      if (this.IsNaN() || this.IsInfinity()) {
        return ExtendedFloat.CreateWithFlags(
          this.unsignedMantissa,
          this.exponent,
          this.flags);
      }
      BigInteger bigintExp = this.Exponent;
      BigInteger bigintMant = this.Mantissa;
      if (bigintMant.IsZero) {
        return this.IsNegative ? ExtendedFloat.NegativeZero :
          ExtendedFloat.Zero;
      }
      if (bigintExp.IsZero) {
        // Integer
        return ExtendedFloat.FromBigInteger(bigintMant);
      }
      if (bigintExp.Sign > 0) {
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
        BigInteger divisor =
          DecimalUtility.FindPowerOfFiveFromBig(negscale.AsBigInteger());
        while (true) {
          BigInteger quotient = BigInteger.DivRem(
            bigmantissa,
            divisor,
            out remainder);
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (!remainder.IsZero && quotient.CompareTo(valueOneShift62) < 0) {
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
        if (oddRounding) {
          // Round to odd to avoid the double-rounding problem
          if (!remainder.IsZero && bigmantissa.IsEven) {
            bigmantissa += BigInteger.One;
          }
        } else {
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
        }
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedFloat.Create(bigmantissa, scale.AsBigInteger());
      }
    }

    /// <summary>Converts this value to a 32-bit floating-point number. The
    /// half-even rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 32-bit
    /// floating point number's mantissa for a quiet NaN, and clears it for
    /// a signaling NaN. Then the next highest bit of the mantissa is
    /// cleared for a quiet NaN, and set for a signaling NaN. Then the
    /// other bits of the mantissa are set to the lowest bits of this
    /// object's unsigned mantissa.</para></summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point
    /// number.</returns>
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
      if (this.IsZero) {
        return 0.0f;
      }
      BigInteger adjExp = this.GetAdjustedExponent();
      if (adjExp.CompareTo((BigInteger)(-47)) < 0) {
        // Very low exponent, treat as 0
        return this.IsNegative ?
          BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0) :
          0.0f;
      }
      if (adjExp.CompareTo((BigInteger)39) > 0) {
        // Very high exponent, treat as infinity
        return this.IsNegative ? Single.NegativeInfinity :
          Single.PositiveInfinity;
      }
      return this.ToExtendedFloatInternal(true).ToSingle();
    }

    private BigInteger GetAdjustedExponent() {
      if (!this.IsFinite) {
        return BigInteger.Zero;
      }
      if (this.IsZero) {
        return BigInteger.Zero;
      }
      BigInteger ret = this.Exponent;
      int smallPrecision = this.UnsignedMantissa.getDigitCount();
      --smallPrecision;
      ret += (BigInteger)smallPrecision;
      return ret;
    }

    private static BigInteger GetAdjustedExponentBinary(ExtendedFloat ef) {
      if (!ef.IsFinite) {
        return BigInteger.Zero;
      }
      if (ef.IsZero) {
        return BigInteger.Zero;
      }
      BigInteger ret = ef.Exponent;
      int smallPrecision = ef.UnsignedMantissa.bitLength();
      --smallPrecision;
      ret += (BigInteger)smallPrecision;
      return ret;
    }

    /// <summary>Converts this value to a 64-bit floating-point number. The
    /// half-even rounding mode is used.
    /// <para>If this value is a NaN, sets the high bit of the 64-bit
    /// floating point number's mantissa for a quiet NaN, and clears it for
    /// a signaling NaN. Then the next highest bit of the mantissa is
    /// cleared for a quiet NaN, and set for a signaling NaN. Then the
    /// other bits of the mantissa are set to the lowest bits of this
    /// object's unsigned mantissa.</para></summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point
    /// number.</returns>
    public double ToDouble() {
      if (this.IsPositiveInfinity()) {
        return Double.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return Double.NegativeInfinity;
      }
      if (this.IsNegative && this.IsZero) {
        return Extras.IntegersToDouble(new[] { unchecked((int)(1 << 31)),
                    0 });
      }
      if (this.IsZero) {
        return 0.0;
      }
      BigInteger adjExp = this.GetAdjustedExponent();
      if (adjExp.CompareTo((BigInteger)(-326)) < 0) {
        // Very low exponent, treat as 0
        return this.IsNegative ?
          Extras.IntegersToDouble(new[] { unchecked((int)(1 << 31)),
                    0 }) : 0.0;
      }
      if (adjExp.CompareTo((BigInteger)309) > 0) {
        // Very high exponent, treat as infinity
        return this.IsNegative ? Double.NegativeInfinity :
          Double.PositiveInfinity;
      }
      return this.ToExtendedFloatInternal(true).ToDouble();
    }

    /// <summary>Creates a decimal number from a 32-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first. Remember, though, that
    /// the exact value of a 32-bit floating-point number is not always the
    /// value you get when you pass a literal decimal number (for example,
    /// calling <c>ExtendedDecimal.FromSingle(0.1f)</c> ), since not all
    /// decimal numbers can be converted to exact binary numbers (in the
    /// example given, the resulting ExtendedDecimal will be the the value
    /// of the closest "float" to 0.1, not 0.1 exactly). To create an
    /// ExtendedDecimal number from a decimal number, use FromString
    /// instead in most cases (for example:
    /// <c>ExtendedDecimal.FromString("0.1")</c> ).</summary>
    /// <param name='flt'>A 32-bit floating-point number.</param>
    /// <returns>A decimal number with the same value as <paramref
    /// name='flt'/>.</returns>
    public static ExtendedDecimal FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      bool neg = (value >> 31) != 0;
      var floatExponent = (int)((value >> 23) & 0xff);
      int valueFpMantissa = value & 0x7fffff;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1fffff;
        var info = (BigInteger)valueFpMantissa;
        value = (neg ? BigNumberFlags.FlagNegative : 0) |
       (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN);
        return info.IsZero ? (quiet ? NaN : SignalingNaN) :
          CreateWithFlags(
            info,
            BigInteger.Zero,
            value);
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
      }
      if (floatExponent > 0) {
        // Value is an integer
        var bigmantissa = (BigInteger)valueFpMantissa;
        bigmantissa <<= floatExponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        var bigmantissa = (BigInteger)valueFpMantissa;
        BigInteger bigexponent = DecimalUtility.FindPowerOfFive(-floatExponent);
        bigmantissa *= (BigInteger)bigexponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.Create(bigmantissa, (BigInteger)floatExponent);
      }
    }

    /// <summary>Converts a big integer to an arbitrary precision
    /// decimal.</summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object with the exponent set to
    /// 0.</returns>
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 64-bit signed
    /// integer.</summary>
    /// <param name='valueSmall'>A 64-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object with the exponent set to
    /// 0.</returns>
    public static ExtendedDecimal FromInt64(long valueSmall) {
      var bigint = (BigInteger)valueSmall;
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 32-bit signed
    /// integer.</summary>
    /// <param name='valueSmaller'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal FromInt32(int valueSmaller) {
      var bigint = (BigInteger)valueSmaller;
      return ExtendedDecimal.Create(bigint, BigInteger.Zero);
    }

    /// <summary>Creates a decimal number from a 64-bit floating-point
    /// number. This method computes the exact value of the floating point
    /// number, not an approximation, as is often the case by converting
    /// the floating point number to a string first. Remember, though, that
    /// the exact value of a 64-bit floating-point number is not always the
    /// value you get when you pass a literal decimal number (for example,
    /// calling <c>ExtendedDecimal.FromDouble(0.1f)</c> ), since not all
    /// decimal numbers can be converted to exact binary numbers (in the
    /// example given, the resulting ExtendedDecimal will be the value of
    /// the closest "double" to 0.1, not 0.1 exactly). To create an
    /// ExtendedDecimal number from a decimal number, use FromString
    /// instead in most cases (for example:
    /// <c>ExtendedDecimal.FromString("0.1")</c> ).</summary>
    /// <param name='dbl'>A 64-bit floating-point number.</param>
    /// <returns>A decimal number with the same value as <paramref
    /// name='dbl'/>.</returns>
    public static ExtendedDecimal FromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      var floatExponent = (int)((value[1] >> 20) & 0x7ff);
      bool neg = (value[1] >> 31) != 0;
      if (floatExponent == 2047) {
        if ((value[1] & 0xfffff) == 0 && value[0] == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (value[1] & 0x80000) != 0;
        value[1] &= 0x3ffff;
        BigInteger info = FastInteger.WordsToBigInteger(value);
        value[0] = (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ?
                BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN);
        return info.IsZero ? (quiet ? NaN : SignalingNaN) :
          CreateWithFlags(
            info,
            BigInteger.Zero,
            value[0]);
      }
      value[1] &= 0xfffff;

      // Mask out the exponent and sign
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
      }
      if (floatExponent > 0) {
        // Value is an integer
        var bigmantissa = (BigInteger)valueFpMantissaBig;
        bigmantissa <<= floatExponent;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        var bigmantissa = (BigInteger)valueFpMantissaBig;
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
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigfloat'/> is null.</exception>
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
        return bigfloat.IsNegative ? ExtendedDecimal.NegativeZero :
          ExtendedDecimal.Zero;
      }
      if (bigintExp.IsZero) {
        // Integer
        return ExtendedDecimal.FromBigInteger(bigintMant);
      }
      if (bigintExp.Sign > 0) {
        // Scaled integer
        FastInteger intcurexp = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        bool neg = bigmantissa.Sign < 0;
        if (neg) {
          bigmantissa = -(BigInteger)bigmantissa;
        }
        while (intcurexp.Sign > 0) {
          int shift = 1000000;
          if (intcurexp.CompareToInt(1000000) < 0) {
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

    /// <summary>Converts this value to a string. Returns a value
    /// compatible with this class's FromString method.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return this.ToStringInternal(0);
    }

    /// <summary>Same as toString(), except that when an exponent is used
    /// it will be a multiple of 3. The format of the return value follows
    /// the format of the java.math.BigDecimal.toEngineeringString()
    /// method.</summary>
    /// <returns>A string object.</returns>
    public string ToEngineeringString() {
      return this.ToStringInternal(1);
    }

    /// <summary>Converts this value to a string, but without using
    /// exponential notation. The format of the return value follows the
    /// format of the java.math.BigDecimal.toPlainString()
    /// method.</summary>
    /// <returns>A string object.</returns>
    public string ToPlainString() {
      return this.ToStringInternal(2);
    }

    /// <summary>Represents the number 1.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.One, BigInteger.Zero);

    /// <summary>Represents the number 0.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.Zero, BigInteger.Zero);

    /// <summary>Represents the number negative zero.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif
    public static readonly ExtendedDecimal NegativeZero =
      CreateWithFlags(
        BigInteger.Zero,
        BigInteger.Zero,
        BigNumberFlags.FlagNegative);

    /// <summary>Represents the number 10.</summary>
#if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security",
                "CA2104", Justification = "ExtendedDecimal is immutable")]
#endif

    public static readonly ExtendedDecimal Ten =
      ExtendedDecimal.Create((BigInteger)10, BigInteger.Zero);

    //----------------------------------------------------------------

    /// <summary>A not-a-number value.</summary>
    public static readonly ExtendedDecimal NaN =
      CreateWithFlags(
        BigInteger.Zero,
        BigInteger.Zero,
        BigNumberFlags.FlagQuietNaN);

    /// <summary>A not-a-number value that signals an invalid operation
    /// flag when it&#x27;s passed as an argument to any arithmetic
    /// operation in ExtendedDecimal.</summary>
    public static readonly ExtendedDecimal SignalingNaN =
      CreateWithFlags(
        BigInteger.Zero,
        BigInteger.Zero,
        BigNumberFlags.FlagSignalingNaN);

    /// <summary>Positive infinity, greater than any other
    /// number.</summary>
    public static readonly ExtendedDecimal PositiveInfinity =
      CreateWithFlags(
        BigInteger.Zero,
        BigInteger.Zero,
        BigNumberFlags.FlagInfinity);

    /// <summary>Negative infinity, less than any other number.</summary>
    public static readonly ExtendedDecimal NegativeInfinity =
      CreateWithFlags(
        BigInteger.Zero,
        BigInteger.Zero,
        BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    /// <summary>Returns whether this object is negative
    /// infinity.</summary>
    /// <returns>True if this object is negative infinity; otherwise,
    /// false.</returns>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                BigNumberFlags.FlagNegative)) == (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative);
    }

    /// <summary>Returns whether this object is positive
    /// infinity.</summary>
    /// <returns>True if this object is positive infinity; otherwise,
    /// false.</returns>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                  BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    }

    /// <summary>Gets a value indicating whether this object is not a
    /// number (NaN).</summary>
    /// <returns>True if this object is not a number (NaN); otherwise,
    /// false.</returns>
    public bool IsNaN() {
      return (this.flags & (BigNumberFlags.FlagQuietNaN |
                    BigNumberFlags.FlagSignalingNaN)) != 0;
    }

    /// <summary>Gets a value indicating whether this object is positive or
    /// negative infinity.</summary>
    /// <returns>True if this object is positive or negative infinity;
    /// otherwise, false.</returns>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <summary>Gets a value indicating whether this object is finite (not
    /// infinity or NaN).</summary>
    /// <value>True if this object is finite (not infinity or NaN);
    /// otherwise, false.</value>
    public bool IsFinite {
      get {
        return (this.flags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNaN)) == 0;
      }
    }

    /// <summary>Gets a value indicating whether this object is negative,
    /// including negative zero.</summary>
    /// <value>True if this object is negative, including negative zero;
    /// otherwise, false.</value>
    public bool IsNegative {
      get {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }
    }

    /// <summary>Gets a value indicating whether this object is a quiet
    /// not-a-number value.</summary>
    /// <returns>True if this object is a quiet not-a-number value;
    /// otherwise, false.</returns>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <summary>Gets a value indicating whether this object is a signaling
    /// not-a-number value.</summary>
    /// <returns>True if this object is a signaling not-a-number value;
    /// otherwise, false.</returns>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <summary>Gets this value&#x27;s sign: -1 if negative; 1 if
    /// positive; 0 if zero.</summary>
    /// <value>This value&apos;s sign: -1 if negative; 1 if positive; 0 if
    /// zero.</value>
    public int Sign {
      get {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
                this.unsignedMantissa.IsZero) ? 0 : (((this.flags &
                    BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
      }
    }

    /// <summary>Gets a value indicating whether this object&#x27;s value
    /// equals 0.</summary>
    /// <value>True if this object&apos;s value equals 0; otherwise,
    /// false.</value>
    public bool IsZero {
      get {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
          this.unsignedMantissa.IsZero;
      }
    }

    /// <summary>Gets the absolute value of this object.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Abs() {
      return this.Abs(null);
    }

    /// <summary>Gets an object with the same value as this one, but with
    /// the sign reversed.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Negate() {
      return this.Negate(null);
    }

    /// <summary>Divides this object by another decimal number and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Returns NaN if the divisor and the dividend are 0. Returns
    /// NaN if the result can't be exact because it would have a
    /// nonterminating decimal expansion.</returns>
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
      return this.Divide(
        divisor,
        PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /// <summary>Divides this object by another decimal number and returns
    /// a result with the same exponent as this object (the
    /// dividend).</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the rounding
    /// mode is Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToSameExponent(
      ExtendedDecimal divisor,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        this.exponent,
        PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedDecimal objects, and returns the
    /// integer part of the result, rounded down, with the preferred
    /// exponent set to this value&#x27;s exponent minus the divisor&#x27;s
    /// exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Signals FlagDivideByZero and returns infinity if the divisor is 0
    /// and the dividend is nonzero. Signals FlagInvalid and returns NaN if
    /// the divisor and the dividend are 0.</returns>
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                    divisor) {
      return this.DivideToIntegerNaturalScale(
        divisor,
        PrecisionContext.ForRounding(Rounding.Down));
    }

    /// <summary>Removes trailing zeros from this object&#x27;s mantissa.
    /// For example, 1.000 becomes 1.
    /// <para>If this object's value is 0, changes the exponent to 0. (This
    /// is unlike the behavior in Java's BigDecimal method
    /// "stripTrailingZeros" in Java 7 and earlier.)</para></summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// result has a very high exponent and the context says to clamp high
    /// exponents, there may still be some trailing zeros in the
    /// mantissa.</returns>
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
      return MathValue.Reduce(this, ctx);
    }

    /// <summary>Calculates the remainder of a number by the formula "this"
    /// - (("this" / "divisor") * "divisor"). This is meant to be similar
    /// to the remainder operation in Java's BigDecimal.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /// <summary>Calculates the remainder of a number by the formula "this"
    /// - (("this" / "divisor") * "divisor"). This is meant to be similar
    /// to the remainder operation in Java's BigDecimal.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the result. This context
    /// will be used only in the division portion of the remainder
    /// calculation; as a result, it&apos;s possible for the return value
    /// to have a higher precision than given in this context. Flags will
    /// be set on the given context only if the context&apos;s HasFlags is
    /// true and the integer part of the division result doesn&apos;t fit
    /// the precision and exponent range without rounding.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal RemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, ctx).Multiply(divisor, null),
        null);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='ctx'>A precision context object to control the
    /// rounding mode to use if the result must be scaled down to have the
    /// same exponent as this value. If the precision given in the context
    /// is other than 0, calls the Quantize method with both arguments
    /// equal to the result of the operation (and can signal FlagInvalid
    /// and return NaN if the result doesn&apos;t fit the given precision).
    /// If HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the context
    /// defines an exponent range and the desired exponent is outside that
    /// range. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      return this.DivideToExponent(
        divisor,
        (BigInteger)desiredExponentSmall,
        ctx);
    }

    /// <summary>Divides this ExtendedDecimal object by another
    /// ExtendedDecimal object. The preferred exponent for the result is
    /// this object&#x27;s exponent minus the divisor&#x27;s
    /// exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0; or, either <paramref name='ctx'/> is null or
    /// <paramref name='ctx'/> 's precision is 0, and the result would have
    /// a nonterminating decimal expansion; or, the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal Divide(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.Divide(this, divisor, ctx);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the rounding
    /// mode is Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        (BigInteger)desiredExponentSmall,
        PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A
    /// positive number places the cutoff point to the left of the usual
    /// decimal point.</param>
    /// <param name='ctx'>A precision context object to control the
    /// rounding mode to use if the result must be scaled down to have the
    /// same exponent as this value. If the precision given in the context
    /// is other than 0, calls the Quantize method with both arguments
    /// equal to the result of the operation (and can signal FlagInvalid
    /// and return NaN if the result doesn&apos;t fit the given precision).
    /// If HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null, in which case the default
    /// rounding mode is HalfEven.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Signals FlagInvalid and returns NaN if the divisor and the
    /// dividend are 0. Signals FlagInvalid and returns NaN if the context
    /// defines an exponent range and the desired exponent is outside that
    /// range. Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <summary>Divides two ExtendedDecimal objects, and gives a
    /// particular exponent to the result.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal
    /// point. A positive number places the cutoff point to the left of the
    /// usual decimal point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two objects. Signals FlagDivideByZero
    /// and returns infinity if the divisor is 0 and the dividend is
    /// nonzero. Returns NaN if the divisor and the dividend are 0. Returns
    /// NaN if the rounding mode is Rounding.Unnecessary and the result is
    /// not exact.</returns>
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      BigInteger desiredExponent,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        desiredExponent,
        PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Finds the absolute value of this object (if it&#x27;s
    /// negative, it becomes positive).</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    public ExtendedDecimal Abs(PrecisionContext context) {
      return MathValue.Abs(this, context);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but with the sign reversed.</summary>
    /// <param name='context'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Negate(PrecisionContext context) {
      return MathValue.Negate(this, context);
    }

    /// <summary>Adds this object and another decimal number and returns
    /// the result.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <returns>The sum of the two objects.</returns>
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
      return this.Add(otherValue, PrecisionContext.Unlimited);
    }

    /// <summary>Subtracts an ExtendedDecimal object from this instance and
    /// returns the result.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <returns>The difference of the two objects.</returns>
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
      return this.Subtract(otherValue, null);
    }

    /// <summary>Subtracts an ExtendedDecimal object from this
    /// instance.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='otherValue'/> is null.</exception>
    public ExtendedDecimal Subtract(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      ExtendedDecimal negated = otherValue;
      if ((otherValue.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = otherValue.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          otherValue.unsignedMantissa,
          otherValue.exponent,
          newflags);
      }
      return this.Add(negated, ctx);
    }

    /// <summary>Multiplies two decimal numbers. The resulting exponent
    /// will be the sum of the exponents of the two decimal
    /// numbers.</summary>
    /// <param name='otherValue'>Another decimal number.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
      return this.Multiply(otherValue, PrecisionContext.Unlimited);
    }

    /// <summary>Multiplies by one decimal number, and then adds another
    /// decimal number.</summary>
    /// <param name='multiplicand'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <returns>The result this * <paramref name='multiplicand'/> +
    /// <paramref name='augend'/>.</returns>
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static readonly IRadixMath<ExtendedDecimal> MathValue = new
      TrappableRadixMath<ExtendedDecimal>(
        new ExtendedOrSimpleRadixMath<ExtendedDecimal>(new
                    DecimalMathHelper()));

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to this
    /// value&#x27;s exponent minus the divisor&#x27;s exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the integer part of the
    /// result. Flags will be set on the given context only if the
    /// context&apos;s HasFlags is true and the integer part of the result
    /// doesn&apos;t fit the precision and exponent range without
    /// rounding.</param>
    /// <returns>The integer part of the quotient of the two objects.
    /// Signals FlagInvalid and returns NaN if the return value would
    /// overflow the exponent range. Signals FlagDivideByZero and returns
    /// infinity if the divisor is 0 and the dividend is nonzero. Signals
    /// FlagInvalid and returns NaN if the divisor and the dividend are 0.
    /// Signals FlagInvalid and returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal DivideToIntegerNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the exponent set to 0.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored. If HasFlags of the context is true, will also store
    /// the flags resulting from the operation (the flags are in addition
    /// to the pre-existing flags). Can be null.</param>
    /// <returns>The integer part of the quotient of the two objects. The
    /// exponent will be set to 0. Signals FlagDivideByZero and returns
    /// infinity if the divisor is 0 and the dividend is nonzero. Signals
    /// FlagInvalid and returns NaN if the divisor and the dividend are 0,
    /// or if the result doesn't fit the given precision.</returns>
    public ExtendedDecimal DivideToIntegerZeroScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <summary>Finds the remainder that results when dividing two
    /// ExtendedDecimal objects.</summary>
    /// <param name='divisor'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>The remainder of the two objects.</returns>
    public ExtendedDecimal Remainder(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.Remainder(this, divisor, ctx);
    }

    /// <summary>Finds the distance to the closest multiple of the given
    /// divisor, based on the result of dividing this object&#x27;s value
    /// by another object&#x27;s value.
    /// <list type=''>
    /// <item>If this and the other object divide evenly, the result is
    /// 0.</item>
    /// <item>If the remainder's absolute value is less than half of the
    /// divisor's absolute value, the result has the same sign as this
    /// object and will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is more than half of the
    /// divisor' s absolute value, the result has the opposite sign of this
    /// object and will be the distance to the closest multiple.</item>
    /// <item>If the remainder's absolute value is exactly half of the
    /// divisor's absolute value, the result has the opposite sign of this
    /// object if the quotient, rounded down, is odd, and has the same sign
    /// as this object if the quotient, rounded down, is even, and the
    /// result's absolute value is half of the divisor's absolute
    /// value.</item></list> This function is also known as the "IEEE
    /// Remainder" function.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision. The rounding and exponent range settings of this context
    /// are ignored (the rounding mode is always treated as HalfEven). If
    /// HasFlags of the context is true, will also store the flags
    /// resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null.</param>
    /// <returns>The distance of the closest multiple. Signals FlagInvalid
    /// and returns NaN if the divisor is 0, or either the result of
    /// integer division (the quotient) or the remainder wouldn't fit the
    /// given precision.</returns>
    public ExtendedDecimal RemainderNear(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.RemainderNear(this, divisor, ctx);
    }

    /// <summary>Finds the largest value that&#x27;s smaller than the given
    /// value.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the largest value that's less than the given
    /// value. Returns negative infinity if the result is negative
    /// infinity. Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null, the precision is 0, or <paramref
    /// name='ctx'/> has an unlimited exponent range.</returns>
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
      return MathValue.NextMinus(this, ctx);
    }

    /// <summary>Finds the smallest value that&#x27;s greater than the
    /// given value.</summary>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value.Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null, the precision is 0, or <paramref
    /// name='ctx'/> has an unlimited exponent range.</returns>
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
      return MathValue.NextPlus(this, ctx);
    }

    /// <summary>Finds the next value that is closer to the other
    /// object&#x27;s value than this object&#x27;s value. Returns a copy
    /// of this value with the same sign as the other value if both values
    /// are equal.</summary>
    /// <param name='otherValue'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision and exponent range of the result. The rounding mode from
    /// this context is ignored. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags).</param>
    /// <returns>Returns the next value that is closer to the other object'
    /// s value than this object's value. Signals FlagInvalid and returns
    /// NaN if the parameter <paramref name='ctx'/> is null, the precision
    /// is 0, or <paramref name='ctx'/> has an unlimited exponent
    /// range.</returns>
    public ExtendedDecimal NextToward(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.NextToward(this, otherValue, ctx);
    }

    /// <summary>Gets the greater value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The larger value of the two objects.</returns>
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.Max(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The smaller value of the two objects.</returns>
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.Min(first, second, ctx);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.MaxMagnitude(first, second, ctx);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.MinMagnitude(first, second, ctx);
    }

    /// <summary>Gets the greater value between two decimal
    /// numbers.</summary>
    /// <param name='first'>An ExtendedDecimal object.</param>
    /// <param name='second'>Another ExtendedDecimal object.</param>
    /// <returns>The larger value of the two objects.</returns>
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Max(first, second, null);
    }

    /// <summary>Gets the lesser value between two decimal
    /// numbers.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>The smaller value of the two objects.</returns>
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Min(first, second, null);
    }

    /// <summary>Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Max.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return MaxMagnitude(first, second, null);
    }

    /// <summary>Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as
    /// Min.</summary>
    /// <param name='first'>The first value to compare.</param>
    /// <param name='second'>The second value to compare.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return MinMagnitude(first, second, null);
    }

    /// <summary>Compares the mathematical values of this object and
    /// another object, accepting NaN values.
    /// <para>This method is not consistent with the Equals method because
    /// two different numbers with the same mathematical value, but
    /// different exponents, will compare as equal.</para>
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method will not trigger an error. Instead, NaN
    /// will compare greater than any other number, including infinity. Two
    /// different NaN values will be considered equal.</para></summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <returns>Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if <paramref name='other'/> is null, or 0 if both
    /// values are equal.</returns>
    public int CompareTo(ExtendedDecimal other) {
      return MathValue.CompareTo(this, other);
    }

    /// <summary>Compares the mathematical values of this object and
    /// another object.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method returns a quiet NaN, and will signal a
    /// FlagInvalid flag if either is a signaling NaN.</para></summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding, and
    /// exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    public ExtendedDecimal CompareToWithContext(
      ExtendedDecimal other,
      PrecisionContext ctx) {
      return MathValue.CompareToWithContext(this, other, false, ctx);
    }

    /// <summary>Compares the mathematical values of this object and
    /// another object, treating quiet NaN as signaling.
    /// <para>In this method, negative zero and positive zero are
    /// considered equal.</para>
    /// <para>If this object or the other object is a quiet NaN or
    /// signaling NaN, this method will return a quiet NaN and will signal
    /// a FlagInvalid flag.</para></summary>
    /// <param name='other'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context. The precision, rounding, and
    /// exponent range are ignored. If HasFlags of the context is true,
    /// will store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null.</param>
    /// <returns>Quiet NaN if this object or the other object is NaN, or 0
    /// if both objects have the same value, or -1 if this object is less
    /// than the other value, or 1 if this object is greater.</returns>
    public ExtendedDecimal CompareToSignal(
      ExtendedDecimal other,
      PrecisionContext ctx) {
      return MathValue.CompareToWithContext(this, other, true, ctx);
    }

    /// <summary>Finds the sum of this object and another object. The
    /// result&#x27;s exponent is set to the lower of the exponents of the
    /// two operands.</summary>
    /// <param name='otherValue'>The number to add to.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The sum of thisValue and the other object.</returns>
    public ExtendedDecimal Add(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.Add(this, otherValue, ctx);
    }

    /// <summary>Returns a decimal number with the same value but a new
    /// exponent.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='desiredExponent'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the rounded result can't fit the given precision, or if the context
    /// defines an exponent range and the given exponent is outside that
    /// range.</returns>
    public ExtendedDecimal Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(
        ExtendedDecimal.Create(BigInteger.One, desiredExponent),
        ctx);
    }

    /// <summary>Returns a decimal number with the same value as this one
    /// but a new exponent.</summary>
    /// <param name='desiredExponentSmall'>A 32-bit signed integer.</param>
    /// <param name='rounding'>A Rounding object.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Returns NaN if the rounding mode is
    /// Rounding.Unnecessary and the result is not exact.</returns>
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      Rounding rounding) {
      return this.Quantize(
      ExtendedDecimal.Create(BigInteger.One, (BigInteger)desiredExponentSmall),
      PrecisionContext.ForRounding(rounding));
    }

    /// <summary>Returns a decimal number with the same value but a new
    /// exponent.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='desiredExponentSmall'>The desired exponent for the
    /// result. The exponent is the number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the rounded result can't fit the given precision, or if the context
    /// defines an exponent range and the given exponent is outside that
    /// range.</returns>
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(
      ExtendedDecimal.Create(BigInteger.One, (BigInteger)desiredExponentSmall),
      ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but with the same exponent as another decimal number.
    /// <para>Note that this is not always the same as rounding to a given
    /// number of decimal places, since it can fail if the difference
    /// between this value's exponent and the desired exponent is too big,
    /// depending on the maximum precision. If rounding to a number of
    /// decimal places is desired, it's better to use the RoundToExponent
    /// and RoundToIntegral methods instead.</para></summary>
    /// <param name='otherValue'>A decimal number containing the desired
    /// exponent of the result. The mantissa is ignored. The exponent is
    /// the number of fractional digits in the result, expressed as a
    /// negative number. Can also be positive, which eliminates lower-order
    /// places from the number. For example, -3 means round to the
    /// thousandth (10^-3, 0.0001), and 3 means round to the thousand
    /// (10^3, 1000). A value of 0 rounds the number to an integer.</param>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number with the same value as this object but
    /// with the exponent changed. Signals FlagInvalid and returns NaN if
    /// the result can't fit the given precision without rounding, or if
    /// the precision context defines an exponent range and the given
    /// exponent is outside that range.</returns>
    public ExtendedDecimal Quantize(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.Quantize(this, otherValue, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags). Can be null, in which case the
    /// default rounding mode is HalfEven.</param>
    /// <returns>A decimal number rounded to the closest integer
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to 0 when rounding, and 0 is outside of the valid range of the
    /// precision context.</returns>
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, without adding the FlagInexact or
    /// FlagRounded flags.</summary>
    /// <param name='ctx'>A precision context to control precision and
    /// rounding of the result. If HasFlags of the context is true, will
    /// also store the flags resulting from the operation (the flags are in
    /// addition to the pre-existing flags), except that this function will
    /// never add the FlagRounded and FlagInexact flags (the only
    /// difference between this and RoundToExponentExact). Can be null, in
    /// which case the default rounding mode is HalfEven.</param>
    /// <returns>A decimal number rounded to the closest integer
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to 0
    /// when rounding, and 0 is outside of the valid range of the precision
    /// context.</returns>
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      return MathValue.RoundToExponentNoRoundedFlag(this, BigInteger.Zero, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to the given exponent when rounding, and the given exponent is
    /// outside of the valid range of the precision context.</returns>
    public ExtendedDecimal RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, exponent, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponent'>The minimum exponent the result can have.
    /// This is the maximum number of fractional digits in the result,
    /// expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to the
    /// given exponent when rounding, and the given exponent is outside of
    /// the valid range of the precision context.</returns>
    public ExtendedDecimal RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentSimple(this, exponent, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object but rounded to an integer, and signals an invalid operation
    /// if the result would be inexact.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision. Signals FlagInvalid and
    /// returns NaN if the result can't fit the given precision without
    /// rounding. Signals FlagInvalid and returns NaN if the precision
    /// context defines an exponent range, the new exponent must be changed
    /// to the given exponent when rounding, and the given exponent is
    /// outside of the valid range of the precision context.</returns>
    public ExtendedDecimal RoundToExponentExact(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponentExact((BigInteger)exponentSmall, ctx);
    }

    /// <summary>Returns a decimal number with the same value as this
    /// object, and rounds it to a new exponent if necessary.</summary>
    /// <param name='exponentSmall'>The minimum exponent the result can
    /// have. This is the maximum number of fractional digits in the
    /// result, expressed as a negative number. Can also be positive, which
    /// eliminates lower-order places from the number. For example, -3
    /// means round to the thousandth (10^-3, 0.0001), and 3 means round to
    /// the thousand (10^3, 1000). A value of 0 rounds the number to an
    /// integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null, in which case the default rounding mode is
    /// HalfEven.</param>
    /// <returns>A decimal number rounded to the closest value
    /// representable in the given precision, meaning if the result can't
    /// fit the precision, additional digits are discarded to make it fit.
    /// Signals FlagInvalid and returns NaN if the precision context
    /// defines an exponent range, the new exponent must be changed to the
    /// given exponent when rounding, and the given exponent is outside of
    /// the valid range of the precision context.</returns>
    public ExtendedDecimal RoundToExponent(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponent((BigInteger)exponentSmall, ctx);
    }

    /// <summary>Multiplies two decimal numbers. The resulting scale will
    /// be the sum of the scales of the two decimal numbers. The
    /// result&#x27;s sign is positive if both operands have the same sign,
    /// and negative if they have different signs.</summary>
    /// <param name='op'>Another decimal number.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The product of the two decimal numbers.</returns>
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
      return MathValue.Multiply(this, op, ctx);
    }

    /// <summary>Multiplies by one value, and then adds another
    /// value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='augend'>The value to add.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The result thisValue * multiplicand + augend.</returns>
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal op,
      ExtendedDecimal augend,
      PrecisionContext ctx) {
      return MathValue.MultiplyAndAdd(this, op, augend, ctx);
    }

    /// <summary>Multiplies by one value, and then subtracts another
    /// value.</summary>
    /// <param name='op'>The value to multiply.</param>
    /// <param name='subtrahend'>The value to subtract.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>The result thisValue * multiplicand -
    /// subtrahend.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='op'/> or <paramref name='subtrahend'/> is null.</exception>
    public ExtendedDecimal MultiplyAndSubtract(
      ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
      if (op == null) {
        throw new ArgumentNullException("op");
      }
      if (subtrahend == null) {
        throw new ArgumentNullException("subtrahend");
      }
      ExtendedDecimal negated = subtrahend;
      if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          subtrahend.unsignedMantissa,
          subtrahend.exponent,
          newflags);
      }
      return MathValue.MultiplyAndAdd(this, op, negated, ctx);
    }

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
      return MathValue.RoundToPrecision(this, ctx);
    }

    /// <summary>Rounds this object&#x27;s value to a given precision,
    /// using the given rounding mode and range of exponent, and also
    /// converts negative zero to positive zero.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    public ExtendedDecimal Plus(PrecisionContext ctx) {
      return MathValue.Plus(this, ctx);
    }

    /// <summary>Rounds this object&#x27;s value to a given maximum bit
    /// length, using the given rounding mode and range of
    /// exponent.</summary>
    /// <param name='ctx'>A context for controlling the precision, rounding
    /// mode, and exponent range. The precision is interpreted as the
    /// maximum bit length of the mantissa. Can be null.</param>
    /// <returns>The closest value to this object's value, rounded to the
    /// specified precision. Returns the same value as this object if
    /// <paramref name='ctx'/> is null or the precision and exponent range
    /// are unlimited.</returns>
    [Obsolete(
      "Instead of this method use RoundToPrecision and pass a " + "precision context with the IsPrecisionInBits property set.")]
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
      if (ctx == null) {
        return this;
      }
      PrecisionContext ctx2 = ctx.Copy().WithPrecisionInBits(true);
      ExtendedDecimal ret = MathValue.RoundToPrecision(this, ctx2);
      if (ctx2.HasFlags) {
        ctx.Flags = ctx2.Flags;
      }
      return ret;
    }

    /// <summary>Finds the square root of this object&#x27;s
    /// value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the square root function&apos;s
    /// results are generally not exact for many inputs.--.</param>
    /// <returns>The square root. Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the square root would be a
    /// complex number, but the return value is still NaN). Signals
    /// FlagInvalid and returns NaN if the parameter <paramref name='ctx'/>
    /// is null or the precision is unlimited (the context's Precision
    /// property is 0).</returns>
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
      return MathValue.SquareRoot(this, ctx);
    }

    /// <summary>Finds e (the base of natural logarithms) raised to the
    /// power of this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the exponential function&apos;s
    /// results are generally not exact.--.</param>
    /// <returns>Exponential of this object. If this object's value is 1,
    /// returns an approximation to " e" within the given precision.
    /// Signals FlagInvalid and returns NaN if the parameter <paramref
    /// name='ctx'/> is null or the precision is unlimited (the context's
    /// Precision property is 0).</returns>
    public ExtendedDecimal Exp(PrecisionContext ctx) {
      return MathValue.Exp(this, ctx);
    }

    /// <summary>Finds the natural logarithm of this object, that is, the
    /// power (exponent) that e (the base of natural logarithms) must be
    /// raised to in order to equal this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the ln function&apos;s results
    /// are generally not exact.--.</param>
    /// <returns>Ln(this object). Signals the flag FlagInvalid and returns
    /// NaN if this object is less than 0 (the result would be a complex
    /// number with a real part equal to Ln of this object's absolute value
    /// and an imaginary part equal to pi, but the return value is still
    /// NaN.). Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null or the precision is unlimited (the
    /// context's Precision property is 0). Signals no flags and returns
    /// negative infinity if this object's value is 0.</returns>
    public ExtendedDecimal Log(PrecisionContext ctx) {
      return MathValue.Ln(this, ctx);
    }

    /// <summary>Finds the base-10 logarithm of this object, that is, the
    /// power (exponent) that the number 10 must be raised to in order to
    /// equal this object&#x27;s value.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as the ln function&apos;s results
    /// are generally not exact.--.</param>
    /// <returns>Ln(this object)/Ln(10). Signals the flag FlagInvalid and
    /// returns NaN if this object is less than 0. Signals FlagInvalid and
    /// returns NaN if the parameter <paramref name='ctx'/> is null or the
    /// precision is unlimited (the context's Precision property is
    /// 0).</returns>
    public ExtendedDecimal Log10(PrecisionContext ctx) {
      return MathValue.Log10(this, ctx);
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponent'>An ExtendedDecimal object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing
    /// flags).</param>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0; or if this value is
    /// less than 0 and the exponent either has a fractional part or is
    /// infinity. Signals FlagInvalid and returns NaN if the parameter
    /// <paramref name='ctx'/> is null or the precision is unlimited (the
    /// context's Precision property is 0), and the exponent has a
    /// fractional part.</returns>
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
      return MathValue.Power(this, exponent, ctx);
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing
    /// flags).</param>
    /// <returns>This^exponent. Signals the flag FlagInvalid and returns
    /// NaN if this object and exponent are both 0.</returns>
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), ctx);
    }

    /// <summary>Raises this object&#x27;s value to the given
    /// exponent.</summary>
    /// <param name='exponentSmall'>A 32-bit signed integer.</param>
    /// <returns>This^exponent. Returns NaN if this object and exponent are
    /// both 0.</returns>
    public ExtendedDecimal Pow(int exponentSmall) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), null);
    }

    /// <summary>Finds the constant pi.</summary>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// --This parameter cannot be null, as pi can never be represented
    /// exactly.--.</param>
    /// <returns>Pi rounded to the given precision. Signals FlagInvalid and
    /// returns NaN if the parameter <paramref name='ctx'/> is null or the
    /// precision is unlimited (the context's Precision property is
    /// 0).</returns>
    public static ExtendedDecimal PI(PrecisionContext ctx) {
      return MathValue.Pi(ctx);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointLeft(int places) {
      return this.MovePointLeft((BigInteger)places, null);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointLeft(int places, PrecisionContext ctx) {
      return this.MovePointLeft((BigInteger)places, ctx);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointLeft(BigInteger bigPlaces) {
      return this.MovePointLeft(bigPlaces, null);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the left.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointLeft(
BigInteger bigPlaces,
PrecisionContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      return (!this.IsFinite) ? this.RoundToPrecision(ctx) :
        this.MovePointRight(-(BigInteger)bigPlaces, ctx);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointRight(int places) {
      return this.MovePointRight((BigInteger)places, null);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointRight(int places, PrecisionContext ctx) {
      return this.MovePointRight((BigInteger)places, ctx);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal MovePointRight(BigInteger bigPlaces) {
      return this.MovePointRight(bigPlaces, null);
    }

    /// <summary>Returns a number similar to this number but with the
    /// decimal point moved to the right.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A number whose scale is increased by <paramref
    /// name='bigPlaces'/>, but not to more than 0.</returns>
    public ExtendedDecimal MovePointRight(
BigInteger bigPlaces,
PrecisionContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      if (!this.IsFinite) {
        return this.RoundToPrecision(ctx);
      }
      BigInteger bigExp = this.Exponent;
      bigExp += bigPlaces;
      if (bigExp.Sign > 0) {
        BigInteger mant = this.unsignedMantissa;
        BigInteger bigPower = DecimalUtility.FindPowerOfTenFromBig(bigExp);
        mant *= bigPower;
        return CreateWithFlags(
mant,
BigInteger.Zero,
this.flags).RoundToPrecision(ctx);
      }
      return CreateWithFlags(
        this.unsignedMantissa,
        bigExp,
        this.flags).RoundToPrecision(ctx);
    }

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ScaleByPowerOfTen(int places) {
      return this.ScaleByPowerOfTen((BigInteger)places, null);
    }

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='places'>A 32-bit signed integer.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ScaleByPowerOfTen(int places, PrecisionContext ctx) {
      return this.ScaleByPowerOfTen((BigInteger)places, ctx);
    }

    /// <summary>Returns a number similar to this number but with the scale
    /// adjusted.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ScaleByPowerOfTen(BigInteger bigPlaces) {
      return this.ScaleByPowerOfTen(bigPlaces, null);
    }

    /// <summary>Returns a number similar to this number but with its scale
    /// adjusted.</summary>
    /// <param name='bigPlaces'>A BigInteger object.</param>
    /// <param name='ctx'>A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the
    /// context is true, will also store the flags resulting from the
    /// operation (the flags are in addition to the pre-existing flags).
    /// Can be null.</param>
    /// <returns>A number whose scale is increased by <paramref
    /// name='bigPlaces'/>.</returns>
    public ExtendedDecimal ScaleByPowerOfTen(
BigInteger bigPlaces,
PrecisionContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      if (!this.IsFinite) {
        return this.RoundToPrecision(ctx);
      }
      BigInteger bigExp = this.Exponent;
      bigExp += bigPlaces;
      return CreateWithFlags(
        this.unsignedMantissa,
        bigExp,
        this.flags).RoundToPrecision(ctx);
    }

    /// <summary>Finds the number of digits in this number's mantissa.
    /// Returns 1 if this value is 0, and 0 if this value is infinity or
    /// NaN.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger Precision() {
      if (!this.IsFinite) {
 return BigInteger.Zero;
}
      if (this.IsZero) {
 return BigInteger.One;
}
      int digcount = this.unsignedMantissa.getDigitCount();
      return (BigInteger)digcount;
    }

    /// <summary>Returns the unit in the last place. The mantissa will be 1
    /// and the exponent will be this number's exponent. Returns 1 with an
    /// exponent of 0 if this number is infinity or NaN.</summary>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal Ulp() {
      return (!this.IsFinite) ? ExtendedDecimal.One :
        ExtendedDecimal.Create(BigInteger.One, this.exponent);
    }

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale. This is meant to be similar to the
    /// divideAndRemainder method in Java's BigDecimal.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(ExtendedDecimal
      divisor) {
      return this.DivideAndRemainderNaturalScale(divisor, null);
    }

    /// <summary>Calculates the quotient and remainder using the
    /// DivideToIntegerNaturalScale and the formula in
    /// RemainderNaturalScale. This is meant to be similar to the
    /// divideAndRemainder method in Java's BigDecimal.</summary>
    /// <param name='divisor'>The number to divide by.</param>
    /// <param name='ctx'>A precision context object to control the
    /// precision, rounding, and exponent range of the result. This context
    /// will be used only in the division portion of the remainder
    /// calculation; as a result, it&apos;s possible for the remainder to
    /// have a higher precision than given in this context. Flags will be
    /// set on the given context only if the context&apos;s HasFlags is
    /// true and the integer part of the division result doesn&apos;t fit
    /// the precision and exponent range without rounding.</param>
    /// <returns>A 2 element array consisting of the quotient and remainder
    /// in that order.</returns>
    public ExtendedDecimal[] DivideAndRemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      var result = new ExtendedDecimal[2];
      result[0] = this.DivideToIntegerNaturalScale(divisor, ctx);
      result[1] = this.Subtract(
        result[0].Multiply(divisor, null),
        null);
      return result;
    }
  }
}
