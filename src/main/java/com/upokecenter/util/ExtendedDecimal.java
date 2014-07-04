package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Represents an arbitrary-precision decimal floating-point number. Consists of
     * an integer mantissa and an integer exponent, both
     * arbitrary-precision. The value of the number equals mantissa *
     * 10^exponent. <p>The mantissa is the value of the digits that make up
     * a number, ignoring the decimal point and exponent. For example, in
     * the number 2356.78, the mantissa is 235678. The exponent is where the
     * "floating" decimal point of the number is located. A positive
     * exponent means "move it to the right" , and a negative exponent means
     * "move it to the left." In the example 2, 356.78, the exponent is -2,
     * since it has 2 decimal places and the decimal point is "moved to the
     * left by 2." Therefore, in the ExtendedDecimal representation, this
     * number would be stored as 235678 * 10^-2.</p> <p>The mantissa and
     * exponent format preserves trailing zeros in the number's value. This
     * may give rise to multiple ways to store the same value. For example,
     * 1.00 and 1 would be stored differently, even though they have the
     * same value. In the first case, 100 * 10^-2 (100 with decimal point
     * moved left by 2), and in the second case, 1 * 10^0 (1 with decimal
     * point moved 0).</p> <p>This class also supports values for negative
     * zero, not-a-number (NaN) values, and infinity. <b>Negative zero</b>
     * is generally used when a negative number is rounded to 0; it has the
     * same mathematical value as positive zero. <b>Infinity</b> is
     * generally used when a non-zero number is divided by zero, or when a
     * very high number can't be represented in a given exponent range.
     * <b>Not-a-number</b> is generally used to signal errors.</p> <p>This
     * class implements the General Decimal Arithmetic Specification version
     * 1.70: <code>http://speleotrove.com/decimal/decarith.html</code> </p>
     * <p>Passing a signaling NaN to any arithmetic operation shown here
     * will signal the flag FlagInvalid and return a quiet NaN, even if
     * another operand to that operation is a quiet NaN, unless noted
     * otherwise.</p> <p>Passing a quiet NaN to any arithmetic operation
     * shown here will return a quiet NaN, unless noted otherwise. Invalid
     * operations will also return a quiet NaN, as stated in the individual
     * methods.</p> <p>Unless noted otherwise, passing a null
     * ExtendedDecimal argument to any method here will throw an
     * exception.</p> <p>When an arithmetic operation signals the flag
     * FlagInvalid, FlagOverflow, or FlagDivideByZero, it will not throw an
     * exception too, unless the flag's trap is enabled in the precision
     * context (see PrecisionContext's Traps property).</p> <p>An
     * ExtendedDecimal value can be serialized in one of the following
     * ways:</p> <ul><li>By calling the toString() method, which will always
     * return distinct strings for distinct ExtendedDecimal values.</li>
     * <li>By calling the UnsignedMantissa, Exponent, and IsNegative
     * properties, and calling the IsInfinity, IsQuietNaN, and
     * IsSignalingNaN methods. The return values combined will uniquely
     * identify a particular ExtendedDecimal value.</li> </ul>
     */
  public final class ExtendedDecimal implements Comparable<ExtendedDecimal> {
    private static final int MaxSafeInt = 214748363;

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

    private boolean EqualsInternal(ExtendedDecimal otherValue) {
      return (otherValue != null) && (this.flags == otherValue.flags &&
  this.unsignedMantissa.equals(otherValue.unsignedMantissa) &&

  this.exponent.equals(otherValue.exponent));
    }

    /**
     * Determines whether this object&apos;s mantissa and exponent are equal to
     * those of another object.
     * @param other An ExtendedDecimal object.
     * @return True if this object's mantissa and exponent are equal to those of
     * another object; otherwise, false.
     */
    public boolean equals(ExtendedDecimal other) {
      return this.EqualsInternal(other);
    }

    /**
     * Determines whether this object&apos;s mantissa and exponent are equal to
     * those of another object and that other object is a decimal fraction.
     * @param obj An arbitrary object.
     * @return True if the objects are equal; otherwise, false.
     */
    @Override public boolean equals(Object obj) {
      return this.EqualsInternal(((obj instanceof ExtendedDecimal) ? (ExtendedDecimal)obj : null));
    }

    /**
     * Calculates this object&apos;s hash code.
     * @return This object's hash code.
     */
    @Override public int hashCode() {
      int valueHashCode = 964453631;
      {
        valueHashCode += 964453723 * this.exponent.hashCode();
        valueHashCode += 964453939 * this.unsignedMantissa.hashCode();
        valueHashCode += 964453967 * this.flags;
      }
      return valueHashCode;
    }

    /**
     * Creates a number with the value exponent*10^mantissa.
     * @param mantissaSmall The un-scaled value.
     * @param exponentSmall The decimal exponent.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal Create(int mantissaSmall, int exponentSmall) {
      return Create(BigInteger.valueOf(mantissaSmall), BigInteger.valueOf(exponentSmall));
    }

    /**
     * Creates a number with the value exponent*10^mantissa.
     * @param mantissa The un-scaled value.
     * @param exponent The decimal exponent.
     * @return An ExtendedDecimal object.
     * @throws NullPointerException The parameter {@code mantissa} or {@code
     * exponent} is null.
     */
    public static ExtendedDecimal Create(
      BigInteger mantissa,
      BigInteger exponent) {
      if (mantissa == null) {
        throw new NullPointerException("mantissa");
      }
      if (exponent == null) {
        throw new NullPointerException("exponent");
      }
      int sign = mantissa.signum();
      return new ExtendedDecimal(
        sign < 0 ? ((mantissa).negate()) : mantissa,
        exponent,
        (sign < 0) ? BigNumberFlags.FlagNegative : 0);
    }

    private ExtendedDecimal(
      BigInteger unsignedMantissa,
      BigInteger exponent,
      int flags) {
      this.unsignedMantissa = unsignedMantissa;
      this.exponent = exponent;
      this.flags = flags;
    }

    static ExtendedDecimal CreateWithFlags(
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
      return new ExtendedDecimal(
        sign < 0 ? ((mantissa).negate()) : mantissa,
        exponent,
        flags);
    }

    /**
     * Creates a not-a-number ExtendedDecimal object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @return A quiet not-a-number object.
     * @throws NullPointerException The parameter {@code diag} is null or is less
     * than 0.
     */
    public static ExtendedDecimal CreateNaN(BigInteger diag) {
      return CreateNaN(diag, false, false, null);
    }

    /**
     * Creates a not-a-number ExtendedDecimal object.
     * @param diag A number to use as diagnostic information associated with this
     * object. If none is needed, should be zero.
     * @param signaling Whether the return value will be signaling (true) or quiet
     * (false).
     * @param negative Whether the return value is negative.
     * @param ctx A PrecisionContext object.
     * @return An ExtendedDecimal object.
     * @throws NullPointerException The parameter {@code diag} is null or is less
     * than 0.
     */
    public static ExtendedDecimal CreateNaN(
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
        ExtendedDecimal ef = CreateWithFlags(
          diag,
          BigInteger.ZERO,
          flags).RoundToPrecision(ctx);
        int newFlags = ef.flags;
        newFlags &= ~BigNumberFlags.FlagQuietNaN;
        newFlags |= signaling ? BigNumberFlags.FlagSignalingNaN :
          BigNumberFlags.FlagQuietNaN;
        return new ExtendedDecimal(ef.unsignedMantissa, ef.exponent, newFlags);
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      return CreateWithFlags(diag, BigInteger.ZERO, flags);
    }

    /**
     * Creates a decimal number from a string that represents a number. See
     * <code>FromString(string, int, int, PrecisionContext)</code> for more
     * information.
     * @param str A string that represents a number.
     * @return An arbitrary-precision decimal number with the same value as the
     * given string.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws NumberFormatException The parameter {@code str} is not a correctly
     * formatted number string.
     */
    public static ExtendedDecimal FromString(String str) {
      return FromString(str, 0, str == null ? 0 : str.length(), null);
    }

    /**
     * Creates a decimal number from a string that represents a number. See
     * <code>FromString(string, int, int, PrecisionContext)</code> for more
     * information.
     * @param str A string that represents a number.
     * @param ctx A PrecisionContext object.
     * @return An arbitrary-precision decimal number with the same value as the
     * given string.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws NumberFormatException The parameter {@code str} is not a correctly
     * formatted number string.
     */
    public static ExtendedDecimal FromString(String str, PrecisionContext ctx) {
      return FromString(str, 0, str == null ? 0 : str.length(), ctx);
    }

    /**
     * Creates a decimal number from a string that represents a number. See
     * <code>FromString(string, int, int, PrecisionContext)</code> for more
     * information.
     * @param str A string that represents a number.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @return An arbitrary-precision decimal number with the same value as the
     * given string.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws NumberFormatException The parameter {@code str} is not a correctly
     * formatted number string.
     */
    public static ExtendedDecimal FromString(
      String str,
      int offset,
      int length) {
      return FromString(str, offset, length, null);
    }

    /**
     * Creates a decimal number from a string that represents a number. <p>The
     * format of the string generally consists of: <ul><li>An optional '-'
     * or '+' character (if '-', the value is negative.)</li> <li>One or
     * more digits, with a single optional decimal point after the first
     * digit and before the last digit.</li> <li>Optionally, E+ (positive
     * exponent) or E- (negative exponent) plus one or more digits
     * specifying the exponent.</li> </ul> </p> <p>The string can also be
     * "-INF", "-Infinity" , "Infinity" , "INF" , quiet NaN ("qNaN"
     * /"-qNaN") followed by any number of digits, or signaling NaN ("sNaN"
     * /"-sNaN") followed by any number of digits, all in any combination of
     * upper and lower case.</p> <p>The format generally follows the
     * definition in java.math.BigDecimal(), except that the digits must be
     * ASCII digits ('0' through '9').</p>
     * @param str A string object.
     * @param offset A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @param ctx A PrecisionContext object.
     * @return An arbitrary-precision decimal number with the same value as the
     * given string.
     * @throws NullPointerException The parameter {@code str} is null.
     * @throws NumberFormatException The parameter {@code str} is not a correctly
     * formatted number string.
     */
    public static ExtendedDecimal FromString(
      String str,
      int offset,
      int length,
      PrecisionContext ctx) {
      int tmpoffset = offset;
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (tmpoffset < 0) {
        throw new NumberFormatException("offset (" + tmpoffset + ") is less than " +
                                  "0");
      }
      if (tmpoffset > str.length()) {
        throw new NumberFormatException("offset (" + tmpoffset + ") is more than " +
                                  str.length());
      }
      if (length < 0) {
        throw new NumberFormatException("length (" + length + ") is less than " +
                                  "0");
      }
      if (length > str.length()) {
        throw new NumberFormatException("length (" + length + ") is more than " +
                                  str.length());
      }
      if (str.length() - tmpoffset < length) {
        throw new NumberFormatException("str's length minus " + tmpoffset + " (" +
                                  (str.length() - tmpoffset) +
                                  ") is less than " + length);
      }
      if (length == 0) {
        throw new NumberFormatException();
      }
      boolean negative = false;
      int endStr = tmpoffset + length;
      if (str.charAt(0) == '+' || str.charAt(0) == '-') {
        negative = str.charAt(0) == '-';
        ++tmpoffset;
      }
      int mantInt = 0;
      FastInteger mant = null;
      int mantBuffer = 0;
      int mantBufferMult = 1;
      int expBuffer = 0;
      int expBufferMult = 1;
      boolean haveDecimalPoint = false;
      boolean haveDigits = false;
      boolean haveExponent = false;
      int newScaleInt = 0;
      FastInteger newScale = null;
      int i = tmpoffset;
      if (i + 8 == endStr) {
        if ((str.charAt(i) == 'I' || str.charAt(i) == 'i') &&
            (str.charAt(i + 1) == 'N' || str.charAt(i + 1) == 'n') &&
            (str.charAt(i + 2) == 'F' || str.charAt(i + 2) == 'f') &&
            (str.charAt(i + 3) == 'I' || str.charAt(i + 3) == 'i') && (str.charAt(i + 4) == 'N' ||
                                           str.charAt(i + 4) == 'n') && (str.charAt(i +
                                                           5) ==
                                   'I' ||
                                   str.charAt(i + 5) == 'i') &&
            (str.charAt(i + 6) == 'T' || str.charAt(i + 6) == 't') && (str.charAt(i + 7) == 'Y' ||
                                                         str.charAt(i + 7) == 'y')) {
          if (ctx != null && ctx.isSimplified() && i < endStr) {
            throw new NumberFormatException("Infinity not allowed");
          }
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 == endStr) {
        if ((str.charAt(i) == 'I' || str.charAt(i) == 'i') &&
            (str.charAt(i + 1) == 'N' || str.charAt(i +
                                      1) == 'n') && (str.charAt(i + 2) == 'F' ||
                           str.charAt(i + 2) == 'f')) {
          if (ctx != null && ctx.isSimplified() && i < endStr) {
            throw new NumberFormatException("Infinity not allowed");
          }
          return negative ? NegativeInfinity : PositiveInfinity;
        }
      }
      if (i + 3 <= endStr) {
        // Quiet NaN
        if ((str.charAt(i) == 'N' || str.charAt(i) == 'n') && (str.charAt(i + 1) == 'A' || str.charAt(i +
                                                   1) == 'a') && (str.charAt(i +
  2) ==
                           'N' ||
                           str.charAt(i +
                               2) ==
                           'n')) {
          if (ctx != null && ctx.isSimplified() && i < endStr) {
            throw new NumberFormatException("NaN not allowed");
          }
          int flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagQuietNaN;
          if (i + 3 == endStr) {
            return (!negative) ? NaN : CreateWithFlags(
              BigInteger.ZERO,
              BigInteger.ZERO,
              flags2);
          }
          i += 3;
          FastInteger digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && ctx.getHasMaxPrecision()) {
            maxDigits = FastInteger.FromBig(ctx.getPrecision());
            if (ctx.getClampNormalExponents()) {
              maxDigits.Decrement();
            }
          }
          for (; i < endStr; ++i) {
            if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
              int thisdigit = (int)(str.charAt(i) - '0');
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
                if (digitCount.compareTo(maxDigits) > 0) {
                  // NaN contains too many digits
                  throw new NumberFormatException();
                }
              }
            } else {
              throw new NumberFormatException();
            }
          }
          if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
          }
          BigInteger bigmant = (mant == null) ? (BigInteger.valueOf(mantInt)) :
            mant.AsBigInteger();
          flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagQuietNaN;
          return CreateWithFlags(
            bigmant,
            BigInteger.ZERO,
            flags2);
        }
      }
      if (i + 4 <= endStr) {
        // Signaling NaN
        if ((str.charAt(i) == 'S' || str.charAt(i) == 's') && (str.charAt(i + 1) == 'N' || str.charAt(i +
                             1) == 'n') && (str.charAt(i + 2) ==
                           'A' || str.charAt(i + 2) == 'a') &&
            (str.charAt(i + 3) == 'N' || str.charAt(i + 3) == 'n')) {
          if (ctx != null && ctx.isSimplified() && i < endStr) {
            throw new NumberFormatException("NaN not allowed");
          }
          if (i + 4 == endStr) {
            int flags2 = (negative ? BigNumberFlags.FlagNegative : 0) |
              BigNumberFlags.FlagSignalingNaN;
            return (!negative) ? SignalingNaN :
              CreateWithFlags(
                BigInteger.ZERO,
                BigInteger.ZERO,
                flags2);
          }
          i += 4;
          FastInteger digitCount = new FastInteger(0);
          FastInteger maxDigits = null;
          haveDigits = false;
          if (ctx != null && ctx.getHasMaxPrecision()) {
            maxDigits = FastInteger.FromBig(ctx.getPrecision());
            if (ctx.getClampNormalExponents()) {
              maxDigits.Decrement();
            }
          }
          for (; i < endStr; ++i) {
            if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
              int thisdigit = (int)(str.charAt(i) - '0');
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
                if (digitCount.compareTo(maxDigits) > 0) {
                  // NaN contains too many digits
                  throw new NumberFormatException();
                }
              }
            } else {
              throw new NumberFormatException();
            }
          }
          if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
            mant.Multiply(mantBufferMult).AddInt(mantBuffer);
          }
          int flags3 = (negative ? BigNumberFlags.FlagNegative : 0) |
            BigNumberFlags.FlagSignalingNaN;
          BigInteger bigmant = (mant == null) ? (BigInteger.valueOf(mantInt)) :
            mant.AsBigInteger();
          return CreateWithFlags(
            bigmant,
            BigInteger.ZERO,
            flags3);
        }
      }
      // Ordinary number
      for (; i < endStr; ++i) {
        if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
          int thisdigit = (int)(str.charAt(i) - '0');
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
            if (newScaleInt == Integer.MIN_VALUE) {
              newScale = (newScale == null) ? ((new FastInteger(newScaleInt))) : newScale;
              newScale.Decrement();
            } else {
              --newScaleInt;
            }
          }
        } else if (str.charAt(i) == '.') {
          if (haveDecimalPoint) {
            throw new NumberFormatException();
          }
          haveDecimalPoint = true;
        } else if (str.charAt(i) == 'E' || str.charAt(i) == 'e') {
          haveExponent = true;
          ++i;
          break;
        } else {
          throw new NumberFormatException();
        }
      }
      if (!haveDigits) {
        throw new NumberFormatException();
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
          throw new NumberFormatException();
        }
        if (str.charAt(i) == '+' || str.charAt(i) == '-') {
          if (str.charAt(i) == '-') {
            tmpoffset = -1;
          }
          ++i;
        }
        for (; i < endStr; ++i) {
          if (str.charAt(i) >= '0' && str.charAt(i) <= '9') {
            haveDigits = true;
            int thisdigit = (int)(str.charAt(i) - '0');
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
            throw new NumberFormatException();
          }
        }
        if (!haveDigits) {
          throw new NumberFormatException();
        }
        if (exp != null && (expBufferMult != 1 || expBuffer != 0)) {
          exp.Multiply(expBufferMult).AddInt(expBuffer);
        }
        if (tmpoffset >= 0 && newScaleInt == 0 && newScale == null && exp ==
            null) {
          newScaleInt = expInt;
        } else if (exp == null) {
          newScale = (newScale == null) ? ((new FastInteger(newScaleInt))) : newScale;
          if (tmpoffset < 0) {
            newScale.SubtractInt(expInt);
          } else if (expInt != 0) {
            newScale.AddInt(expInt);
          }
        } else {
          newScale = (newScale == null) ? ((new FastInteger(newScaleInt))) : newScale;
          if (tmpoffset < 0) {
            newScale.Subtract(exp);
          } else {
            newScale.Add(exp);
          }
        }
      }
      if (i != endStr) {
        throw new NumberFormatException();
      }
      BigInteger bigNewScale = (newScale == null) ? (BigInteger.valueOf(newScaleInt)) :
        newScale.AsBigInteger();
      ExtendedDecimal ret = new ExtendedDecimal(
        (mant == null) ? (BigInteger.valueOf(mantInt)) : mant.AsBigInteger(),
        bigNewScale,
        negative ? BigNumberFlags.FlagNegative : 0);
      if (ctx != null) {
        ret = MathValue.RoundAfterConversion(ret, ctx);
      }
      return ret;
    }

    private static final class DecimalMathHelper implements IRadixMathHelper<ExtendedDecimal> {
    /**
     * This is an internal method.
     * @return A 32-bit signed integer.
     */
      public int GetRadix() {
        return 10;
      }

    /**
     * This is an internal method.
     * @param value An ExtendedDecimal object.
     * @return A 32-bit signed integer.
     */
      public int GetSign(ExtendedDecimal value) {
        return value.signum();
      }

    /**
     * This is an internal method.
     * @param value An ExtendedDecimal object.
     * @return A BigInteger object.
     */
      public BigInteger GetMantissa(ExtendedDecimal value) {
        return value.unsignedMantissa;
      }

    /**
     * This is an internal method.
     * @param value An ExtendedDecimal object.
     * @return A BigInteger object.
     */
      public BigInteger GetExponent(ExtendedDecimal value) {
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
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /**
     * This is an internal method.
     * @param bigint A BigInteger object.
     * @return An IShiftAccumulator object.
     */
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint, 0, 0);
      }

    /**
     * This is an internal method.
     * @param numerator A BigInteger object.
     * @param denominator A BigInteger object. (2).
     * @return A Boolean object.
     */
      public boolean HasTerminatingRadixExpansion(
        BigInteger numerator,
        BigInteger denominator) {
        // Simplify denominator based on numerator
        BigInteger gcd =
          numerator.gcd(denominator);
        BigInteger tmpden = denominator;
        tmpden = tmpden.divide(gcd);
        if (tmpden.signum() == 0) {
          return false;
        }
        // Eliminate factors of 2
        while (tmpden.testBit(0) == false) {
          tmpden = tmpden.shiftRight(1);
        }
        // Eliminate factors of 5
        while (true) {
          BigInteger bigrem;
          BigInteger bigquo;
{
BigInteger[] divrem = (tmpden).divideAndRemainder(BigInteger.valueOf(5));
bigquo = divrem[0];
bigrem = divrem[1]; }
          if (bigrem.signum() != 0) {
            break;
          }
          tmpden = bigquo;
        }
        return tmpden.compareTo(BigInteger.ONE) == 0;
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
        if (tmpbigint.signum() == 0) {
          return tmpbigint;
        }
        BigInteger bigtmp = null;
        if (tmpbigint.compareTo(BigInteger.ONE) != 0) {
          if (power.CanFitInInt32()) {
            bigtmp = DecimalUtility.FindPowerOfTen(power.AsInt32());
            tmpbigint = tmpbigint.multiply(bigtmp);
          } else {
            bigtmp = DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
            tmpbigint = tmpbigint.multiply(bigtmp);
          }
          return tmpbigint;
        }
        return power.CanFitInInt32() ?
          DecimalUtility.FindPowerOfTen(power.AsInt32()) :
          DecimalUtility.FindPowerOfTenFromBig(power.AsBigInteger());
      }

    /**
     * This is an internal method.
     * @param value An ExtendedDecimal object.
     * @return A 32-bit signed integer.
     */
      public int GetFlags(ExtendedDecimal value) {
        return value.flags;
      }

    /**
     * This is an internal method.
     * @param mantissa A BigInteger object.
     * @param exponent A BigInteger object. (2).
     * @param flags A 32-bit signed integer.
     * @return An ExtendedDecimal object.
     */
      public ExtendedDecimal CreateNewWithFlags(
        BigInteger mantissa,
        BigInteger exponent,
        int flags) {
        return CreateWithFlags(mantissa, exponent, flags);
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
     * @return An ExtendedDecimal object.
     */
      public ExtendedDecimal ValueOf(int val) {
        return (val == 0) ? Zero : ((val == 1) ? One : FromInt64(val));
      }
    }

    private static boolean AppendString(
      StringBuilder builder,
      char c,
      FastInteger count) {
      if (count.CompareToInt(Integer.MAX_VALUE) > 0 || count.signum() < 0) {
        throw new UnsupportedOperationException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; --i) {
        builder.append(c);
      }
      return true;
    }

    private String ToStringInternal(int mode) {
      // Using Java's rules for converting ExtendedDecimal
      // values to a String
      boolean negative = (this.flags & BigNumberFlags.FlagNegative) != 0;
      if ((this.flags & BigNumberFlags.FlagInfinity) != 0) {
        return negative ? "-Infinity" : "Infinity";
      }
      if ((this.flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.unsignedMantissa.signum() == 0 ? (negative ? "-sNaN" : "sNaN") :
          (negative ? "-sNaN" + (this.unsignedMantissa).abs() :
           "sNaN" + (this.unsignedMantissa).abs());
      }
      if ((this.flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.unsignedMantissa.signum() == 0 ? (negative ? "-NaN" : "NaN") :
          (negative ? "-NaN" + (this.unsignedMantissa).abs() : "NaN" +
           (this.unsignedMantissa).abs());
      }
      String mantissaString = (this.unsignedMantissa).abs().toString();
      int scaleSign = -this.exponent.signum();
      if (scaleSign == 0) {
        return negative ? "-" + mantissaString : mantissaString;
      }
      boolean iszero = this.unsignedMantissa.signum() == 0;
      if (mode == 2 && iszero && scaleSign < 0) {
        // special case for zero in plain
        return negative ? "-" + mantissaString : mantissaString;
      }
      FastInteger builderLength = new FastInteger(mantissaString.length());
      FastInteger adjustedExponent = FastInteger.FromBig(this.exponent);
      FastInteger thisExponent = FastInteger.Copy(adjustedExponent);
      adjustedExponent.Add(builderLength).Decrement();
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) {
        // engineering String adjustments
        FastInteger newExponent = FastInteger.Copy(adjustedExponent);
        boolean adjExponentNegative = adjustedExponent.signum() < 0;
        int intphase =
          FastInteger.Copy(adjustedExponent).Abs().Remainder(3).AsInt32();
        if (iszero && (adjustedExponent.compareTo(threshold) < 0 || scaleSign <
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
      if (mode == 2 || (adjustedExponent.compareTo(threshold) >= 0 &&
                        scaleSign >= 0)) {
        if (scaleSign > 0) {
          FastInteger decimalPoint =
            FastInteger.Copy(thisExponent).Add(builderLength);
          int cmp = decimalPoint.CompareToInt(0);
          StringBuilder builder = null;
          if (cmp < 0) {
            FastInteger tmpFast = new FastInteger(mantissaString.length()).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Integer.MAX_VALUE) >
                                        0 ? Integer.MAX_VALUE : tmpFast.AsInt32());
            if (negative) {
              builder.append('-');
            }
            builder.append("0.");
            AppendString(builder, '0', FastInteger.Copy(decimalPoint).Negate());
            builder.append(mantissaString);
          } else if (cmp == 0) {
            if (!decimalPoint.CanFitInInt32()) {
              throw new UnsupportedOperationException();
            }
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.length()).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Integer.MAX_VALUE) >
                                        0 ? Integer.MAX_VALUE : tmpFast.AsInt32());
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString, 0, (0) + (tmpInt));
            builder.append("0.");
            builder.append(
              mantissaString, tmpInt, (tmpInt) + (mantissaString.length() - tmpInt));
          } else if (decimalPoint.CompareToInt(mantissaString.length()) > 0) {
            FastInteger insertionPoint = builderLength;
            if (!insertionPoint.CanFitInInt32()) {
              throw new UnsupportedOperationException();
            }
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.length()).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Integer.MAX_VALUE) >
                                        0 ? Integer.MAX_VALUE : tmpFast.AsInt32());
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString, 0, (0) + (tmpInt));
            AppendString(
              builder,
              '0',
              FastInteger.Copy(decimalPoint).SubtractInt(builder.length()));
            builder.append('.');
            builder.append(
              mantissaString, tmpInt, (tmpInt) + (mantissaString.length() - tmpInt));
          } else {
            if (!decimalPoint.CanFitInInt32()) {
              throw new UnsupportedOperationException();
            }
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.length()).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Integer.MAX_VALUE) >
                                        0 ? Integer.MAX_VALUE : tmpFast.AsInt32());
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString, 0, (0) + (tmpInt));
            builder.append('.');
            builder.append(
              mantissaString, tmpInt, (tmpInt) + (mantissaString.length() - tmpInt));
          }
          return builder.toString();
        }
        if (mode == 2 && scaleSign < 0) {
          FastInteger negscale = FastInteger.Copy(thisExponent);
          StringBuilder builder = new StringBuilder();
          if (negative) {
            builder.append('-');
          }
          builder.append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.toString();
        }
        return (!negative) ? mantissaString : ("-" + mantissaString);
      } else {
        StringBuilder builder = null;
        if (mode == 1 && iszero && decimalPointAdjust.CompareToInt(1) > 0) {
          builder = new StringBuilder();
          if (negative) {
            builder.append('-');
          }
          builder.append(mantissaString);
          builder.append('.');
          AppendString(
            builder,
            '0',
            FastInteger.Copy(decimalPointAdjust).Decrement());
        } else {
          FastInteger tmp = FastInteger.Copy(decimalPointAdjust);
          int cmp = tmp.CompareToInt(mantissaString.length());
          if (cmp > 0) {
            tmp.SubtractInt(mantissaString.length());
            builder = new StringBuilder();
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            // Insert a decimal point at the right place
            if (!tmp.CanFitInInt32()) {
              throw new UnsupportedOperationException();
            }
            int tmpInt = tmp.AsInt32();
            if (tmp.signum() < 0) {
              tmpInt = 0;
            }
            FastInteger tmpFast = new FastInteger(mantissaString.length()).AddInt(6);
            builder = new StringBuilder(tmpFast.CompareToInt(Integer.MAX_VALUE) >
                                        0 ? Integer.MAX_VALUE : tmpFast.AsInt32());
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString, 0, (0) + (tmpInt));
            builder.append('.');
            builder.append(
              mantissaString, tmpInt, (tmpInt) + (mantissaString.length() - tmpInt));
          } else if (adjustedExponent.signum() == 0 && !negative) {
            return mantissaString;
          } else if (adjustedExponent.signum() == 0 && negative) {
            return "-" + mantissaString;
          } else {
            builder = new StringBuilder();
            if (negative) {
              builder.append('-');
            }
            builder.append(mantissaString);
          }
        }
        if (adjustedExponent.signum() != 0) {
          builder.append(adjustedExponent.signum() < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          StringBuilder builderReversed = new StringBuilder();
          while (adjustedExponent.signum() != 0) {
            int digit =
              FastInteger.Copy(adjustedExponent).Remainder(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.append((char)('0' + digit));
            adjustedExponent.Divide(10);
          }
          int count = builderReversed.length();
          for (int i = 0; i < count; ++i) {
            builder.append(builderReversed.charAt(count - 1 - i));
          }
        }
        return builder.toString();
      }
    }

    /**
     * Compares a ExtendedFloat object with this instance.
     * @param other An ExtendedFloat object.
     * @return Zero if the values are equal; a negative number if this instance is
     * less, or a positive number if this instance is greater.
     */
    public int CompareToBinary(ExtendedFloat other) {
      if (other == null) {
        return 1;
      }
      if (this.IsNaN()) {
        return other.IsNaN() ? 0 : 1;
      }
      int signA = this.signum();
      int signB = other.signum();
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
        return this.isNegative() ? -1 : 1;
      }
      if (other.IsInfinity()) {
        return other.isNegative() ? 1 : -1;
      }
      // At this point, both numbers are finite and
      // have the same sign

      if (other.getExponent().compareTo(BigInteger.valueOf(-1000)) < 0) {
        // For very low exponents, the conversion to decimal can take
        // very long, so try this approach
        if (other.Abs(null).compareTo(ExtendedFloat.One) < 0) {
          // Abs less than 1
          if (this.Abs(null).compareTo(ExtendedDecimal.One) >= 0) {
            // Abs 1 or more
            return (signA > 0) ? 1 : -1;
          }
        }
      }
      if (other.getExponent().compareTo(BigInteger.valueOf(1000)) > 0) {
        // Very high exponents
        BigInteger bignum = BigInteger.ONE.shiftLeft(999);
        if (this.Abs(null).compareTo(ExtendedDecimal.FromBigInteger(bignum)) <=
            0) {
          // this object's absolute value is less
          return (signA > 0) ? -1 : 1;
        }
        // NOTE: The following check assumes that both
        // operands are nonzero
        BigInteger thisAdjExp = this.GetAdjustedExponent();
        BigInteger otherAdjExp = GetAdjustedExponentBinary(other);
        if (thisAdjExp.signum() > 0 && thisAdjExp.compareTo(otherAdjExp) >= 0) {
          // This Object's adjusted exponent is greater and is positive;
          // so this object's absolute value is greater, since exponents
          // have a greater value in decimal than in binary
          return (signA > 0) ? 1 : -1;
        }
        if (thisAdjExp.signum() > 0 &&
            thisAdjExp.compareTo(BigInteger.valueOf(1000)) >= 0 &&
            otherAdjExp.compareTo(BigInteger.valueOf(1000)) >= 0) {
          thisAdjExp = thisAdjExp.add(BigInteger.ONE);
          otherAdjExp = otherAdjExp.add(BigInteger.ONE);
          BigInteger ratio = otherAdjExp.divide(thisAdjExp);
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
          if (ratio.compareTo(BigInteger.valueOf(3)) < 0) {
            // Decimal abs. value is greater
            return (signA > 0) ? 1 : -1;
          }
          if (ratio.compareTo(BigInteger.valueOf(4)) >= 0) {
            return (signA > 0) ? -1 : 1;
          }
        }
      }
      ExtendedDecimal otherDec = ExtendedDecimal.FromExtendedFloat(other);
      return this.compareTo(otherDec);
    }

    /**
     * Converts this value to an arbitrary-precision integer. Any fractional part
     * in this value will be discarded when converting to a big integer.
     * @return A BigInteger object.
     * @throws ArithmeticException This object's value is infinity or NaN.
     */
    public BigInteger ToBigInteger() {
      return this.ToBigIntegerInternal(false);
    }

    /**
     * Converts this value to an arbitrary-precision integer, checking whether the
     * fractional part of the integer would be lost.
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
      int sign = this.getExponent().signum();
      if (this.signum() == 0) {
        return BigInteger.ZERO;
      }
      if (sign == 0) {
        BigInteger bigmantissa = this.getMantissa();
        return bigmantissa;
      }
      if (sign > 0) {
        BigInteger bigmantissa = this.getMantissa();
        BigInteger bigexponent =
          DecimalUtility.FindPowerOfTenFromBig(this.getExponent());
        bigmantissa = bigmantissa.multiply(bigexponent);
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.getMantissa();
        FastInteger bigexponent = FastInteger.FromBig(this.getExponent()).Negate();
        bigmantissa = (bigmantissa).abs();
        DigitShiftAccumulator acc = new DigitShiftAccumulator(bigmantissa, 0, 0);
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

    private static BigInteger valueOneShift62 = BigInteger.ONE.shiftLeft(62);

    /**
     * Creates a binary floating-point number from this object&apos;s value. Note
     * that if the binary floating-point number contains a negative
     * exponent, the resulting value might not be exact. However, the
     * resulting binary float will contain enough precision to accurately
     * convert it to a 32-bit or 64-bit floating point number (float or
     * double).
     * @return An ExtendedFloat object.
     */
    public ExtendedFloat ToExtendedFloat() {
      if (this.IsNaN() || this.IsInfinity()) {
        return ExtendedFloat.CreateWithFlags(
          this.unsignedMantissa,
          this.exponent,
          this.flags);
      }
      BigInteger bigintExp = this.getExponent();
      BigInteger bigintMant = this.getMantissa();
      if (bigintMant.signum() == 0) {
        return this.isNegative() ? ExtendedFloat.NegativeZero :
          ExtendedFloat.Zero;
      }
      if (bigintExp.signum() == 0) {
        // Integer
        return ExtendedFloat.FromBigInteger(bigintMant);
      }
      if (bigintExp.signum() > 0) {
        // Scaled integer
        BigInteger bigmantissa = bigintMant;
        bigintExp = DecimalUtility.FindPowerOfTenFromBig(bigintExp);
        bigmantissa = bigmantissa.multiply(bigintExp);
        return ExtendedFloat.FromBigInteger(bigmantissa);
      } else {
        // Fractional number
        FastInteger scale = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = bigmantissa.signum() < 0;
        BigInteger remainder;
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        FastInteger negscale = FastInteger.Copy(scale).Negate();
        BigInteger divisor =
          DecimalUtility.FindPowerOfFiveFromBig(negscale.AsBigInteger());
        while (true) {
          BigInteger quotient;
{
BigInteger[] divrem = (bigmantissa).divideAndRemainder(divisor);
quotient = divrem[0];
remainder = divrem[1]; }
          // Ensure that the quotient has enough precision
          // to be converted accurately to a single or double
          if (remainder.signum() != 0 && quotient.compareTo(valueOneShift62) < 0) {
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
            bigmantissa = bigmantissa.shiftLeft(shift);
          } else {
            bigmantissa = quotient;
            break;
          }
        }
        // Round half-even
        BigInteger halfDivisor = divisor;
        halfDivisor = halfDivisor.shiftRight(1);
        int cmp = remainder.compareTo(halfDivisor);
        // No need to check for exactly half since all powers
        // of five are odd
        if (cmp > 0) {
          // Greater than half
          bigmantissa = bigmantissa.add(BigInteger.ONE);
        }
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedFloat.Create(bigmantissa, scale.AsBigInteger());
      }
    }

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
      if (this.isNegative() && this.signum() == 0) {
        return Float.intBitsToFloat(1 << 31);
      }
      if (this.signum() == 0) {
        return 0.0f;
      }
      BigInteger adjExp = this.GetAdjustedExponent();
      if (adjExp.compareTo(BigInteger.valueOf(-47)) < 0) {
        // Very low exponent, treat as 0
        return this.isNegative() ?
          Float.intBitsToFloat(1 << 31) :
          0.0f;
      }
      if (adjExp.compareTo(BigInteger.valueOf(39)) > 0) {
        // Very high exponent, treat as infinity
        return this.isNegative() ?
          Float.NEGATIVE_INFINITY : Float.POSITIVE_INFINITY;
      }
      return this.ToExtendedFloat().ToSingle();
    }

    private BigInteger GetAdjustedExponent() {
      if (!this.isFinite()) {
        return BigInteger.ZERO;
      }
      if (this.signum() == 0) {
        return BigInteger.ZERO;
      }
      BigInteger ret = this.getExponent();
      int smallPrecision = this.getUnsignedMantissa().getDigitCount();
      --smallPrecision;
      ret = ret.add(BigInteger.valueOf(smallPrecision));
      return ret;
    }

    private static BigInteger GetAdjustedExponentBinary(ExtendedFloat ef) {
      if (!ef.isFinite()) {
        return BigInteger.ZERO;
      }
      if (ef.signum() == 0) {
        return BigInteger.ZERO;
      }
      BigInteger ret = ef.getExponent();
      int smallPrecision = ef.getUnsignedMantissa().bitLength();
      --smallPrecision;
      ret = ret.add(BigInteger.valueOf(smallPrecision));
      return ret;
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
      if (this.isNegative() && this.signum() == 0) {
        return Extras.IntegersToDouble(new int[] { ((int)(1 << 31)),
                                         0 });
      }
      if (this.signum() == 0) {
        return 0.0;
      }
      BigInteger adjExp = this.GetAdjustedExponent();
      if (adjExp.compareTo(BigInteger.valueOf(-326)) < 0) {
        // Very low exponent, treat as 0
        return this.isNegative() ?
          Extras.IntegersToDouble(new int[] { ((int)(1 << 31)),
                                    0 }) :
          0.0;
      }
      if (adjExp.compareTo(BigInteger.valueOf(309)) > 0) {
        // Very high exponent, treat as infinity
        return this.isNegative() ?
          Double.NEGATIVE_INFINITY : Double.POSITIVE_INFINITY;
      }
      return this.ToExtendedFloat().ToDouble();
    }

    /**
     * Creates a decimal number from a 32-bit floating-point number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param flt A 32-bit floating-point number.
     * @return A decimal number with the same value as {@code flt} .
     */
    public static ExtendedDecimal FromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      boolean neg = (value >> 31) != 0;
      int floatExponent = (int)((value >> 23) & 0xff);
      int valueFpMantissa = value & 0x7fffff;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        boolean quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1fffff;
        BigInteger info = BigInteger.valueOf(valueFpMantissa);
        value = (neg ? BigNumberFlags.FlagNegative : 0) |
          (quiet ?
           BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN);
        return info.signum() == 0 ? (quiet ? NaN : SignalingNaN) :
          CreateWithFlags(
            info,
            BigInteger.ZERO,
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
        BigInteger bigmantissa = BigInteger.valueOf(valueFpMantissa);
        bigmantissa = bigmantissa.shiftLeft(floatExponent);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = BigInteger.valueOf(valueFpMantissa);
        BigInteger bigexponent = DecimalUtility.FindPowerOfFive(-floatExponent);
        bigmantissa = bigmantissa.multiply(bigexponent);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedDecimal.Create(bigmantissa, BigInteger.valueOf(floatExponent));
      }
    }

    /**
     * Converts a big integer to an arbitrary precision decimal.
     * @param bigint A BigInteger object.
     * @return An ExtendedDecimal object with the exponent set to 0.
     */
    public static ExtendedDecimal FromBigInteger(BigInteger bigint) {
      return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Creates a decimal number from a 64-bit signed integer.
     * @param valueSmall A 64-bit signed integer.
     * @return An ExtendedDecimal object with the exponent set to 0.
     */
    public static ExtendedDecimal FromInt64(long valueSmall) {
      BigInteger bigint = BigInteger.valueOf(valueSmall);
      return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Creates a decimal number from a 32-bit signed integer.
     * @param valueSmaller A 32-bit signed integer.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal FromInt32(int valueSmaller) {
      BigInteger bigint = BigInteger.valueOf(valueSmaller);
      return ExtendedDecimal.Create(bigint, BigInteger.ZERO);
    }

    /**
     * Creates a decimal number from a 64-bit floating-point number. This method
     * computes the exact value of the floating point number, not an
     * approximation, as is often the case by converting the number to a
     * string.
     * @param dbl A 64-bit floating-point number.
     * @return A decimal number with the same value as {@code dbl} .
     */
    public static ExtendedDecimal FromDouble(double dbl) {
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
        value[0] = (neg ?
                    BigNumberFlags.FlagNegative : 0) | (quiet ?
                                            BigNumberFlags.FlagQuietNaN :
                                            BigNumberFlags.FlagSignalingNaN);
        return info.signum() == 0 ? (quiet ? NaN : SignalingNaN) :
          CreateWithFlags(
            info,
            BigInteger.ZERO,
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
        floatExponent +=
          DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      } else {
        return neg ? ExtendedDecimal.NegativeZero : ExtendedDecimal.Zero;
      }
      floatExponent -= 1075;
      BigInteger valueFpMantissaBig = FastInteger.WordsToBigInteger(value);
      if (floatExponent == 0) {
        if (neg) {
          valueFpMantissaBig = valueFpMantissaBig.negate();
        }
        return ExtendedDecimal.FromBigInteger(valueFpMantissaBig);
      }
      if (floatExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = valueFpMantissaBig;
        bigmantissa = bigmantissa.shiftLeft(floatExponent);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = valueFpMantissaBig;
        BigInteger exp = DecimalUtility.FindPowerOfFive(-floatExponent);
        bigmantissa = bigmantissa.multiply(exp);
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedDecimal.Create(bigmantissa, BigInteger.valueOf(floatExponent));
      }
    }

    /**
     * Creates a decimal number from an arbitrary-precision binary floating-point
     * number.
     * @param bigfloat A big floating-point number.
     * @return An ExtendedDecimal object.
     * @throws NullPointerException The parameter {@code bigfloat} is null.
     */
    public static ExtendedDecimal FromExtendedFloat(ExtendedFloat bigfloat) {
      if (bigfloat == null) {
        throw new NullPointerException("bigfloat");
      }
      if (bigfloat.IsNaN() || bigfloat.IsInfinity()) {
        int flags = (bigfloat.isNegative() ? BigNumberFlags.FlagNegative : 0) |
          (bigfloat.IsInfinity() ? BigNumberFlags.FlagInfinity : 0) |
          (bigfloat.IsQuietNaN() ? BigNumberFlags.FlagQuietNaN : 0) |
          (bigfloat.IsSignalingNaN() ? BigNumberFlags.FlagSignalingNaN : 0);
        return CreateWithFlags(
          bigfloat.getUnsignedMantissa(),
          bigfloat.getExponent(),
          flags);
      }
      BigInteger bigintExp = bigfloat.getExponent();
      BigInteger bigintMant = bigfloat.getMantissa();
      if (bigintMant.signum() == 0) {
        return bigfloat.isNegative() ? ExtendedDecimal.NegativeZero :
          ExtendedDecimal.Zero;
      }
      if (bigintExp.signum() == 0) {
        // Integer
        return ExtendedDecimal.FromBigInteger(bigintMant);
      }
      if (bigintExp.signum() > 0) {
        // Scaled integer
        FastInteger intcurexp = FastInteger.FromBig(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = bigmantissa.signum() < 0;
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        while (intcurexp.signum() > 0) {
          int shift = 1000000;
          if (intcurexp.CompareToInt(1000000) < 0) {
            shift = intcurexp.AsInt32();
          }
          bigmantissa = bigmantissa.shiftLeft(shift);
          intcurexp.AddInt(-shift);
        }
        if (neg) {
          bigmantissa = (bigmantissa).negate();
        }
        return ExtendedDecimal.FromBigInteger(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger negbigintExp = (bigintExp).negate();
        negbigintExp = DecimalUtility.FindPowerOfFiveFromBig(negbigintExp);
        bigmantissa = bigmantissa.multiply(negbigintExp);
        return ExtendedDecimal.Create(bigmantissa, bigintExp);
      }
    }

    /**
     * Converts this value to a string. Returns a value compatible with this
     * class's FromString method.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      return this.ToStringInternal(0);
    }

    /**
     * Same as toString(), except that when an exponent is used it will be a
     * multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     * @return A string object.
     */
    public String ToEngineeringString() {
      return this.ToStringInternal(1);
    }

    /**
     * Converts this value to a string, but without an exponent part. The format of
     * the return value follows the format of the
     * java.math.BigDecimal.toPlainString() method.
     * @return A string object.
     */
    public String ToPlainString() {
      return this.ToStringInternal(2);
    }

    /**
     * Represents the number 1.
     */

    public static final ExtendedDecimal One =
      ExtendedDecimal.Create(BigInteger.ONE, BigInteger.ZERO);

    /**
     * Represents the number 0.
     */

    public static final ExtendedDecimal Zero =
      ExtendedDecimal.Create(BigInteger.ZERO, BigInteger.ZERO);

    /**
     * Represents the number negative zero.
     */

    public static final ExtendedDecimal NegativeZero =
      CreateWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagNegative);

    /**
     * Represents the number 10.
     */

    public static final ExtendedDecimal Ten =
      ExtendedDecimal.Create(BigInteger.TEN, BigInteger.ZERO);

    //----------------------------------------------------------------

    /**
     * A not-a-number value.
     */
    public static final ExtendedDecimal NaN =
      CreateWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagQuietNaN);

    /**
     * A not-a-number value that signals an invalid operation flag when it&apos;s
     * passed as an argument to any arithmetic operation in ExtendedDecimal.
     */
    public static final ExtendedDecimal SignalingNaN =
      CreateWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagSignalingNaN);

    /**
     * Positive infinity, greater than any other number.
     */
    public static final ExtendedDecimal PositiveInfinity =
      CreateWithFlags(
        BigInteger.ZERO,
        BigInteger.ZERO,
        BigNumberFlags.FlagInfinity);

    /**
     * Negative infinity, less than any other number.
     */
    public static final ExtendedDecimal NegativeInfinity =
      CreateWithFlags(
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
        (BigNumberFlags.FlagInfinity |
         BigNumberFlags.FlagNegative);
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
     * Gets a value indicating whether this object is not a number (NaN).
     * @return True if this object is not a number (NaN); otherwise, false.
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
                this.unsignedMantissa.signum() == 0) ? 0 : (((this.flags &
                                               BigNumberFlags.FlagNegative) !=
                                              0) ?
                                             -1 :
                                             1);
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
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal Abs() {
      return this.Abs(null);
    }

    /**
     * Gets an object with the same value as this one, but with the sign reversed.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal Negate() {
      return this.Negate(null);
    }

    /**
     * Divides this object by another decimal number and returns the result. When
     * possible, the result will be exact.
     * @param divisor The divisor.
     * @return The quotient of the two numbers. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Returns NaN if the divisor and the dividend are 0. Returns NaN if the
     * result can't be exact because it would have a nonterminating decimal
     * expansion.
     */
    public ExtendedDecimal Divide(ExtendedDecimal divisor) {
      return this.Divide(
        divisor,
        PrecisionContext.ForRounding(Rounding.Unnecessary));
    }

    /**
     * Divides this object by another decimal number and returns a result with the
     * same exponent as this object (the dividend).
     * @param divisor The divisor.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two numbers. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0. Signals FlagInvalid and returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToSameExponent(
      ExtendedDecimal divisor,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        this.exponent,
        PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two ExtendedDecimal objects, and returns the integer part of the
     * result, rounded down, with the preferred exponent set to this
     * value&apos;s exponent minus the divisor&apos;s exponent.
     * @param divisor The divisor.
     * @return The integer part of the quotient of the two objects. Signals
     * FlagDivideByZero and returns infinity if the divisor is 0 and the
     * dividend is nonzero. Signals FlagInvalid and returns NaN if the
     * divisor and the dividend are 0.
     */
    public ExtendedDecimal DivideToIntegerNaturalScale(ExtendedDecimal
                                                       divisor) {
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
    public ExtendedDecimal Reduce(PrecisionContext ctx) {
      return MathValue.Reduce(this, ctx);
    }

    /**
     * Calculates the remainder of a number by the formula "this" - (("this" /
     * "divisor") * "divisor"). This is meant to be similar to the remainder
     * operation in Java's BigDecimal.
     * @param divisor The number to divide by.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal RemainderNaturalScale(ExtendedDecimal divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /**
     * Calculates the remainder of a number by the formula "this" - (("this" /
     * "divisor") * "divisor"). This is meant to be similar to the remainder
     * operation in Java's BigDecimal.
     * @param divisor The number to divide by.
     * @param ctx A precision context object to control the precision, rounding,
     * and exponent range of the result. This context will be used only in
     * the division portion of the remainder calculation; as a result,
     * it&apos;s possible for the return value to have a higher precision
     * than given in this context. Flags will be set on the given context
     * only if the context&apos;s HasFlags is true and the integer part of
     * the division result doesn&apos;t fit the precision and exponent range
     * without rounding.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal RemainderNaturalScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, ctx).Multiply(divisor, null),
        null);
    }

    /**
     * Divides two ExtendedDecimal objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedDecimal object.
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
     * Signals FlagInvalid and returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      PrecisionContext ctx) {
      return this.DivideToExponent(
        divisor,
        BigInteger.valueOf(desiredExponentSmall),
        ctx);
    }

    /**
     * Divides this ExtendedDecimal object by another ExtendedDecimal object. The
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
     * are 0; or, either {@code ctx} is null or {@code ctx} 's precision is
     * 0, and the result would have a nonterminating decimal expansion; or,
     * the rounding mode is Rounding.Unnecessary and the result is not
     * exact.
     */
    public ExtendedDecimal Divide(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.Divide(this, divisor, ctx);
    }

    /**
     * Divides two ExtendedDecimal objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedDecimal object.
     * @param desiredExponentSmall The desired exponent. A negative number places
     * the cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal
     * point.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Signals FlagInvalid and returns NaN if the divisor and the dividend
     * are 0. Signals FlagInvalid and returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      long desiredExponentSmall,
      Rounding rounding) {
      return this.DivideToExponent(
        divisor,
        BigInteger.valueOf(desiredExponentSmall),
        PrecisionContext.ForRounding(rounding));
    }

    /**
     * Divides two ExtendedDecimal objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedDecimal object.
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
     * Signals FlagInvalid and returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.DivideToExponent(this, divisor, exponent, ctx);
    }

    /**
     * Divides two ExtendedDecimal objects, and gives a particular exponent to the
     * result.
     * @param divisor An ExtendedDecimal object.
     * @param desiredExponent The desired exponent. A negative number places the
     * cutoff point to the right of the usual decimal point. A positive
     * number places the cutoff point to the left of the usual decimal
     * point.
     * @param rounding The rounding mode to use if the result must be scaled down
     * to have the same exponent as this value.
     * @return The quotient of the two objects. Signals FlagDivideByZero and
     * returns infinity if the divisor is 0 and the dividend is nonzero.
     * Returns NaN if the divisor and the dividend are 0. Returns NaN if the
     * rounding mode is Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToExponent(
      ExtendedDecimal divisor,
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
    public ExtendedDecimal Abs(PrecisionContext context) {
      return MathValue.Abs(this, context);
    }

    /**
     * Returns a decimal number with the same value as this object but with the
     * sign reversed.
     * @param context A precision context to control precision, rounding, and
     * exponent range of the result. If HasFlags of the context is true,
     * will also store the flags resulting from the operation (the flags are
     * in addition to the pre-existing flags). Can be null.
     * @return An ExtendedDecimal object.
     */
    public ExtendedDecimal Negate(PrecisionContext context) {
      return MathValue.Negate(this, context);
    }

    /**
     * Adds this object and another decimal number and returns the result.
     * @param otherValue An ExtendedDecimal object.
     * @return The sum of the two objects.
     */
    public ExtendedDecimal Add(ExtendedDecimal otherValue) {
      return this.Add(otherValue, PrecisionContext.Unlimited);
    }

    /**
     * Subtracts an ExtendedDecimal object from this instance and returns the
     * result.
     * @param otherValue An ExtendedDecimal object.
     * @return The difference of the two objects.
     */
    public ExtendedDecimal Subtract(ExtendedDecimal otherValue) {
      return this.Subtract(otherValue, null);
    }

    /**
     * Subtracts an ExtendedDecimal object from this instance.
     * @param otherValue An ExtendedDecimal object.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The difference of the two objects.
     * @throws NullPointerException The parameter {@code otherValue} is null.
     */
    public ExtendedDecimal Subtract(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      if (otherValue == null) {
        throw new NullPointerException("otherValue");
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

    /**
     * Multiplies two decimal numbers. The resulting exponent will be the sum of
     * the exponents of the two decimal numbers.
     * @param otherValue Another decimal number.
     * @return The product of the two decimal numbers.
     */
    public ExtendedDecimal Multiply(ExtendedDecimal otherValue) {
      return this.Multiply(otherValue, PrecisionContext.Unlimited);
    }

    /**
     * Multiplies by one decimal number, and then adds another decimal number.
     * @param multiplicand The value to multiply.
     * @param augend The value to add.
     * @return The result this * {@code multiplicand} + {@code augend} .
     */
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal multiplicand,
      ExtendedDecimal augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static final IRadixMath<ExtendedDecimal> MathValue = new
      TrappableRadixMath<ExtendedDecimal>(
        new ExtendedOrSimpleRadixMath<ExtendedDecimal>(new
                                                       DecimalMathHelper()));

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
     * @return The integer part of the quotient of the two objects. Signals
     * FlagInvalid and returns NaN if the return value would overflow the
     * exponent range. Signals FlagDivideByZero and returns infinity if the
     * divisor is 0 and the dividend is nonzero. Signals FlagInvalid and
     * returns NaN if the divisor and the dividend are 0. Signals
     * FlagInvalid and returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal DivideToIntegerNaturalScale(
      ExtendedDecimal divisor,
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
    public ExtendedDecimal DivideToIntegerZeroScale(
      ExtendedDecimal divisor,
      PrecisionContext ctx) {
      return MathValue.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /**
     * Finds the remainder that results when dividing two ExtendedDecimal objects.
     * @param divisor An ExtendedDecimal object.
     * @param ctx A PrecisionContext object.
     * @return The remainder of the two objects.
     */
    public ExtendedDecimal Remainder(
      ExtendedDecimal divisor,
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
    public ExtendedDecimal RemainderNear(
      ExtendedDecimal divisor,
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
     * negative infinity if the result is negative infinity. Signals
     * FlagInvalid and returns NaN if the parameter {@code ctx} is null, the
     * precision is 0, or {@code ctx} has an unlimited exponent range.
     */
    public ExtendedDecimal NextMinus(PrecisionContext ctx) {
      return MathValue.NextMinus(this, ctx);
    }

    /**
     * Finds the smallest value that&apos;s greater than the given value.
     * @param ctx A precision context object to control the precision and exponent
     * range of the result. The rounding mode from this context is ignored.
     * If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags).
     * @return Returns the smallest value that's greater than the given
     * value.Signals FlagInvalid and returns NaN if the parameter {@code
     * ctx} is null, the precision is 0, or {@code ctx} has an unlimited
     * exponent range.
     */
    public ExtendedDecimal NextPlus(PrecisionContext ctx) {
      return MathValue.NextPlus(this, ctx);
    }

    /**
     * Finds the next value that is closer to the other object&apos;s value than
     * this object&apos;s value.
     * @param otherValue An ExtendedDecimal object.
     * @param ctx A precision context object to control the precision and exponent
     * range of the result. The rounding mode from this context is ignored.
     * If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags).
     * @return Returns the next value that is closer to the other object' s value
     * than this object's value. Signals FlagInvalid and returns NaN if the
     * parameter {@code ctx} is null, the precision is 0, or {@code ctx} has
     * an unlimited exponent range.
     */
    public ExtendedDecimal NextToward(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.NextToward(this, otherValue, ctx);
    }

    /**
     * Gets the greater value between two decimal numbers.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The larger value of the two objects.
     */
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.Max(first, second, ctx);
    }

    /**
     * Gets the lesser value between two decimal numbers.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The smaller value of the two objects.
     */
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.Min(first, second, ctx);
    }

    /**
     * Gets the greater value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Max.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.MaxMagnitude(first, second, ctx);
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Min.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second,
      PrecisionContext ctx) {
      return MathValue.MinMagnitude(first, second, ctx);
    }

    /**
     * Gets the greater value between two decimal numbers.
     * @param first An ExtendedDecimal object.
     * @param second An ExtendedDecimal object. (2).
     * @return The larger value of the two objects.
     */
    public static ExtendedDecimal Max(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Max(first, second, null);
    }

    /**
     * Gets the lesser value between two decimal numbers.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @return The smaller value of the two objects.
     */
    public static ExtendedDecimal Min(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return Min(first, second, null);
    }

    /**
     * Gets the greater value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Max.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal MaxMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
      return MaxMagnitude(first, second, null);
    }

    /**
     * Gets the lesser value between two values, ignoring their signs. If the
     * absolute values are equal, has the same effect as Min.
     * @param first The first value to compare.
     * @param second The second value to compare.
     * @return An ExtendedDecimal object.
     */
    public static ExtendedDecimal MinMagnitude(
      ExtendedDecimal first,
      ExtendedDecimal second) {
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
     * @param other An ExtendedDecimal object.
     * @return Less than 0 if this object's value is less than the other value, or
     * greater than 0 if this object's value is greater than the other value
     * or if {@code other} is null, or 0 if both values are equal.
     */
    public int compareTo(ExtendedDecimal other) {
      return MathValue.compareTo(this, other);
    }

    /**
     * Compares the mathematical values of this object and another object. <p>In
     * this method, negative zero and positive zero are considered
     * equal.</p> <p>If this object or the other object is a quiet NaN or
     * signaling NaN, this method returns a quiet NaN, and will signal a
     * FlagInvalid flag if either is a signaling NaN.</p>
     * @param other An ExtendedDecimal object.
     * @param ctx A precision context. The precision, rounding, and exponent range
     * are ignored. If HasFlags of the context is true, will store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null.
     * @return Quiet NaN if this object or the other object is NaN, or 0 if both
     * objects have the same value, or -1 if this object is less than the
     * other value, or 1 if this object is greater.
     */
    public ExtendedDecimal CompareToWithContext(
      ExtendedDecimal other,
      PrecisionContext ctx) {
      return MathValue.CompareToWithContext(this, other, false, ctx);
    }

    /**
     * Compares the mathematical values of this object and another object, treating
     * quiet NaN as signaling. <p>In this method, negative zero and positive
     * zero are considered equal.</p> <p>If this object or the other object
     * is a quiet NaN or signaling NaN, this method will return a quiet NaN
     * and will signal a FlagInvalid flag.</p>
     * @param other An ExtendedDecimal object.
     * @param ctx A precision context. The precision, rounding, and exponent range
     * are ignored. If HasFlags of the context is true, will store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null.
     * @return Quiet NaN if this object or the other object is NaN, or 0 if both
     * objects have the same value, or -1 if this object is less than the
     * other value, or 1 if this object is greater.
     */
    public ExtendedDecimal CompareToSignal(
      ExtendedDecimal other,
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
    public ExtendedDecimal Add(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.Add(this, otherValue, ctx);
    }

    /**
     * Returns a decimal number with the same value but a new exponent. <p>Note
     * that this is not always the same as rounding to a given number of
     * decimal places, since it can fail if the difference between this
     * value's exponent and the desired exponent is too big, depending on
     * the maximum precision. If rounding to a number of decimal places is
     * desired, it's better to use the RoundToExponent and RoundToIntegral
     * methods instead.</p>
     * @param desiredExponent A BigInteger object.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal number with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the rounded result can't fit the given precision,
     * or if the context defines an exponent range and the given exponent is
     * outside that range.
     */
    public ExtendedDecimal Quantize(
      BigInteger desiredExponent,
      PrecisionContext ctx) {
      return this.Quantize(
        ExtendedDecimal.Create(BigInteger.ONE, desiredExponent),
        ctx);
    }

    /**
     * Returns a decimal number with the same value as this one but a new exponent.
     * @param desiredExponentSmall A 32-bit signed integer.
     * @param rounding A Rounding object.
     * @return A decimal number with the same value as this object but with the
     * exponent changed. Returns NaN if the rounding mode is
     * Rounding.Unnecessary and the result is not exact.
     */
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      Rounding rounding) {
      return this.Quantize(
      ExtendedDecimal.Create(BigInteger.ONE, BigInteger.valueOf(desiredExponentSmall)),
      PrecisionContext.ForRounding(rounding));
    }

    /**
     * Returns a decimal number with the same value but a new exponent. <p>Note
     * that this is not always the same as rounding to a given number of
     * decimal places, since it can fail if the difference between this
     * value's exponent and the desired exponent is too big, depending on
     * the maximum precision. If rounding to a number of decimal places is
     * desired, it's better to use the RoundToExponent and RoundToIntegral
     * methods instead.</p>
     * @param desiredExponentSmall The desired exponent for the result. The
     * exponent is the number of fractional digits in the result, expressed
     * as a negative number. Can also be positive, which eliminates
     * lower-order places from the number. For example, -3 means round to
     * the thousandth (10^-3, 0.0001), and 3 means round to the thousand
     * (10^3, 1000). A value of 0 rounds the number to an integer.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal number with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the rounded result can't fit the given precision,
     * or if the context defines an exponent range and the given exponent is
     * outside that range.
     */
    public ExtendedDecimal Quantize(
      int desiredExponentSmall,
      PrecisionContext ctx) {
      return this.Quantize(
      ExtendedDecimal.Create(BigInteger.ONE, BigInteger.valueOf(desiredExponentSmall)),
      ctx);
    }

    /**
     * Returns a decimal number with the same value as this object but with the
     * same exponent as another decimal number. <p>Note that this is not
     * always the same as rounding to a given number of decimal places,
     * since it can fail if the difference between this value's exponent and
     * the desired exponent is too big, depending on the maximum precision.
     * If rounding to a number of decimal places is desired, it's better to
     * use the RoundToExponent and RoundToIntegral methods instead.</p>
     * @param otherValue A decimal number containing the desired exponent of the
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
     * @return A decimal number with the same value as this object but with the
     * exponent changed. Signals FlagInvalid and returns NaN if an overflow
     * error occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent is
     * outside of the valid range of the precision context, if it defines an
     * exponent range.
     */
    public ExtendedDecimal Quantize(
      ExtendedDecimal otherValue,
      PrecisionContext ctx) {
      return MathValue.Quantize(this, otherValue, ctx);
    }

    /**
     * Returns a decimal number with the same value as this object but rounded to
     * an integer, and signals an invalid operation if the result would be
     * inexact.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags). Can be null, in which case the default rounding
     * mode is HalfEven.
     * @return A decimal number with the same value as this object but rounded to
     * an integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent
     * must be changed to 0 when rounding and 0 is outside of the valid
     * range of the precision context, if it defines an exponent range.
     */
    public ExtendedDecimal RoundToIntegralExact(PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, BigInteger.ZERO, ctx);
    }

    /**
     * Returns a decimal number with the same value as this object but rounded to
     * an integer, without adding the FlagInexact or FlagRounded flags.
     * @param ctx A precision context to control precision and rounding of the
     * result. If HasFlags of the context is true, will also store the flags
     * resulting from the operation (the flags are in addition to the
     * pre-existing flags), except that this function will never add the
     * FlagRounded and FlagInexact flags (the only difference between this
     * and RoundToExponentExact). Can be null, in which case the default
     * rounding mode is HalfEven.
     * @return A decimal number with the same value as this object but rounded to
     * an integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent
     * must be changed to 0 when rounding and 0 is outside of the valid
     * range of the precision context, if it defines an exponent range.
     */
    public ExtendedDecimal RoundToIntegralNoRoundedFlag(PrecisionContext ctx) {
      return MathValue.RoundToExponentNoRoundedFlag(this, BigInteger.ZERO, ctx);
    }

    /**
     * Returns a decimal number with the same value as this object but rounded to
     * an integer, and signals an invalid operation if the result would be
     * inexact.
     * @param exponent The minimum exponent the result can have. This is the
     * maximum number of fractional digits in the result, expressed as a
     * negative number. Can also be positive, which eliminates lower-order
     * places from the number. For example, -3 means round to the thousandth
     * (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
     * value of 0 rounds the number to an integer.
     * @param ctx A PrecisionContext object.
     * @return A decimal number with the same value as this object but rounded to
     * an integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent is
     * outside of the valid range of the precision context, if it defines an
     * exponent range.
     */
    public ExtendedDecimal RoundToExponentExact(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentExact(this, exponent, ctx);
    }

    /**
     * Returns a decimal number with the same value as this object, and rounds it
     * to a new exponent if necessary.
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
     * @return A decimal number rounded to the closest value representable in the
     * given precision, meaning if the result can't fit the precision,
     * additional digits are discarded to make it fit. Signals FlagInvalid
     * and returns NaN if the new exponent must be changed when rounding and
     * the new exponent is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public ExtendedDecimal RoundToExponent(
      BigInteger exponent,
      PrecisionContext ctx) {
      return MathValue.RoundToExponentSimple(this, exponent, ctx);
    }

    /**
     * Returns a decimal number with the same value as this object but rounded to
     * an integer, and signals an invalid operation if the result would be
     * inexact.
     * @param exponentSmall The minimum exponent the result can have. This is the
     * maximum number of fractional digits in the result, expressed as a
     * negative number. Can also be positive, which eliminates lower-order
     * places from the number. For example, -3 means round to the thousandth
     * (10^-3, 0.0001), and 3 means round to the thousand (10^3, 1000). A
     * value of 0 rounds the number to an integer.
     * @param ctx A PrecisionContext object.
     * @return A decimal number with the same value as this object but rounded to
     * an integer. Signals FlagInvalid and returns NaN if an overflow error
     * occurred, or the result can't fit the given precision without
     * rounding. Signals FlagInvalid and returns NaN if the new exponent is
     * outside of the valid range of the precision context, if it defines an
     * exponent range.
     */
    public ExtendedDecimal RoundToExponentExact(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponentExact(BigInteger.valueOf(exponentSmall), ctx);
    }

    /**
     * Returns a decimal number with the same value as this object, and rounds it
     * to a new exponent if necessary.
     * @param exponentSmall The minimum exponent the result can have. This is the
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
     * @return A decimal number rounded to the closest value representable in the
     * given precision, meaning if the result can't fit the precision,
     * additional digits are discarded to make it fit. Signals FlagInvalid
     * and returns NaN if the new exponent must be changed when rounding and
     * the new exponent is outside of the valid range of the precision
     * context, if it defines an exponent range.
     */
    public ExtendedDecimal RoundToExponent(
      int exponentSmall,
      PrecisionContext ctx) {
      return this.RoundToExponent(BigInteger.valueOf(exponentSmall), ctx);
    }

    /**
     * Multiplies two decimal numbers. The resulting scale will be the sum of the
     * scales of the two decimal numbers. The result&apos;s sign is positive
     * if both operands have the same sign, and negative if they have
     * different signs.
     * @param op Another decimal number.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). Can be null.
     * @return The product of the two decimal numbers.
     */
    public ExtendedDecimal Multiply(ExtendedDecimal op, PrecisionContext ctx) {
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
    public ExtendedDecimal MultiplyAndAdd(
      ExtendedDecimal op,
      ExtendedDecimal augend,
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
    public ExtendedDecimal MultiplyAndSubtract(
      ExtendedDecimal op,
      ExtendedDecimal subtrahend,
      PrecisionContext ctx) {
      if (subtrahend == null) {
        throw new NullPointerException("subtrahend");
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

    /**
     * Rounds this object&apos;s value to a given precision, using the given
     * rounding mode and range of exponent.
     * @param ctx A context for controlling the precision, rounding mode, and
     * exponent range. Can be null.
     * @return The closest value to this object's value, rounded to the specified
     * precision. Returns the same value as this object if {@code ctx} is
     * null or the precision and exponent range are unlimited.
     */
    public ExtendedDecimal RoundToPrecision(PrecisionContext ctx) {
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
    public ExtendedDecimal Plus(PrecisionContext ctx) {
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
    public ExtendedDecimal RoundToBinaryPrecision(PrecisionContext ctx) {
      if (ctx == null) {
        return this;
      }
      PrecisionContext ctx2 = ctx.Copy().WithPrecisionInBits(true);
      ExtendedDecimal ret = MathValue.RoundToPrecision(this, ctx2);
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
     * number, but the return value is still NaN). Signals FlagInvalid and
     * returns NaN if the parameter {@code ctx} is null or the precision is
     * unlimited (the context's Precision property is 0).
     */
    public ExtendedDecimal SquareRoot(PrecisionContext ctx) {
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
     * approximation to " e" within the given precision. Signals FlagInvalid
     * and returns NaN if the parameter {@code ctx} is null or the precision
     * is unlimited (the context's Precision property is 0).
     */
    public ExtendedDecimal Exp(PrecisionContext ctx) {
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
     * Signals FlagInvalid and returns NaN if the parameter {@code ctx} is
     * null or the precision is unlimited (the context's Precision property
     * is 0). Signals no flags and returns negative infinity if this
     * object's value is 0.
     */
    public ExtendedDecimal Log(PrecisionContext ctx) {
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
    public ExtendedDecimal Log10(PrecisionContext ctx) {
      return MathValue.Log10(this, ctx);
    }

    /**
     * Raises this object&apos;s value to the given exponent.
     * @param exponent An ExtendedDecimal object.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags).
     * @return This^exponent. Signals the flag FlagInvalid and returns NaN if this
     * object and exponent are both 0; or if this value is less than 0 and
     * the exponent either has a fractional part or is infinity. Signals
     * FlagInvalid and returns NaN if the parameter {@code ctx} is null or
     * the precision is unlimited (the context's Precision property is 0),
     * and the exponent has a fractional part.
     */
    public ExtendedDecimal Pow(ExtendedDecimal exponent, PrecisionContext ctx) {
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
    public ExtendedDecimal Pow(int exponentSmall, PrecisionContext ctx) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), ctx);
    }

    /**
     * Raises this object&apos;s value to the given exponent.
     * @param exponentSmall A 32-bit signed integer.
     * @return This^exponent. Returns NaN if this object and exponent are both 0.
     */
    public ExtendedDecimal Pow(int exponentSmall) {
      return this.Pow(ExtendedDecimal.FromInt64(exponentSmall), null);
    }

    /**
     * Finds the constant pi.
     * @param ctx A precision context to control precision, rounding, and exponent
     * range of the result. If HasFlags of the context is true, will also
     * store the flags resulting from the operation (the flags are in
     * addition to the pre-existing flags). --This parameter cannot be null,
     * as pi can never be represented exactly.--.
     * @return Pi rounded to the given precision. Signals FlagInvalid and returns
     * NaN if the parameter {@code ctx} is null or the precision is
     * unlimited (the context's Precision property is 0).
     */
    public static ExtendedDecimal PI(PrecisionContext ctx) {
      return MathValue.Pi(ctx);
    }
  }
