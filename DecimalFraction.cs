/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Text;
//using System.Numerics;
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
    /// <param name='other'> A DecimalFraction object.</param>
    public bool Equals(DecimalFraction other) {
      DecimalFraction otherValue = other as DecimalFraction;
      if (otherValue == null)
        return false;
      return this.exponent.Equals(otherValue.exponent) &&
        this.mantissa.Equals(otherValue.mantissa);
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
    /// <summary> Creates a decimal fraction with the value exponentSmall*10^mantissa.
    /// </summary>
    /// <param name='mantissa'> The unscaled value.</param>
    /// <param name='exponentSmall'> The decimal exponent.</param>
    public DecimalFraction(BigInteger mantissa, long exponentSmall) :
      this(mantissa,(BigInteger)exponentSmall) {
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissa'> The desired value of the bigfloat</param>
    public DecimalFraction(BigInteger mantissa) :
      this(mantissa,BigInteger.Zero){
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissaSmall'> The desired value of the bigfloat</param>
    public DecimalFraction(long mantissaSmall) {
      this.exponent = BigInteger.Zero;
      this.mantissa = (BigInteger)mantissaSmall;
    }
    /// <summary> Creates a decimal fraction with the given mantissa and
    /// an exponent of 0. </summary>
    /// <param name='mantissaSmall'> The unscaled value.</param>
    /// <param name='exponentSmall'> The decimal exponent.</param>
    public DecimalFraction(long mantissaSmall, long exponentSmall) {
      this.exponent = (BigInteger)exponentSmall;
      this.mantissa = (BigInteger)mantissaSmall;
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
    /// <param name='str'> A string that represents a number.</param>
    /// <returns></returns>
    public static DecimalFraction FromString(String str) {
      if (str == null)
        throw new ArgumentNullException("str");
      if (str.Length == 0)
        throw new FormatException();
      int offset = 0;
      bool negative = false;
      if (str[0] == '+' || str[0] == '-') {
        negative = (str[0] == '-');
        offset++;
      }
      FastInteger2 mant = new FastInteger2();
      bool haveDecimalPoint = false;
      bool haveDigits = false;
      bool haveExponent = false;
      FastInteger newScale = new FastInteger();
      int i = offset;
      for (; i < str.Length; i++) {
        if (str[i] >= '0' && str[i] <= '9') {
          int thisdigit = (int)(str[i] - '0');
          mant.MultiplyByTen();
          mant.Add(thisdigit);
          haveDigits = true;
          if (haveDecimalPoint) {
            newScale.Add(-1);
          }
        } else if (str[i] == '.') {
          if (haveDecimalPoint)
            throw new FormatException();
          haveDecimalPoint = true;
        } else if (str[i] == 'E' || str[i] == 'e') {
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
        if (i == str.Length) throw new FormatException();
        if (str[i] == '+' || str[i] == '-') {
          if (str[i] == '-') offset = -1;
          i++;
        }
        for (; i < str.Length; i++) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
            int thisdigit = (int)(str[i] - '0');
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
      } else if (i != str.Length) {
        throw new FormatException();
      }
      return new DecimalFraction(
        (negative) ? mant.AsNegatedBigInteger() :
        mant.AsBigInteger(),
        newScale.AsBigInteger());
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
    
    private static BigInteger FivePower40=((BigInteger)95367431640625L)*(BigInteger)(95367431640625L);

    internal static BigInteger FindPowerOfFive(int precision) {
      if (precision <= 0) return BigInteger.One;
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 27)
        return BigIntPowersOfFive[(int)precision];
      if(precision==40)
        return FivePower40;
      if (precision <= 54) {
        if((precision&1)==0){
          ret = BigIntPowersOfFive[(int)(precision>>1)];
          ret *= (BigInteger)ret;
          return ret;
        } else {
          ret = BigIntPowersOfFive[27];
          bigpow = BigIntPowersOfFive[((int)precision) - 27];
          ret *= (BigInteger)bigpow;
          return ret;
        }
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

    internal static BigInteger FindPowerOfTen(int precision) {
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
        if((precision&1)==0){
          ret = BigIntPowersOfTen[(int)(precision>>1)];
          ret *= (BigInteger)ret;
          return ret;
        } else {
          ret = BigIntPowersOfTen[18];
          bigpow = BigIntPowersOfTen[((int)precision) - 18];
          ret *= (BigInteger)bigpow;
          return ret;
        }
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

    private sealed class DecimalMathHelper : IRadixMathHelper<DecimalFraction> {

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/>
      public int GetRadix() {
        return 10;
      }

    /// <summary> </summary>
    /// <param name='value'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
      public DecimalFraction Abs(DecimalFraction value) {
        return value.Abs();
      }

    /// <summary> </summary>
    /// <param name='value'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
      public int GetSign(DecimalFraction value) {
        return value.Sign;
      }

    /// <summary> </summary>
    /// <param name='value'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger GetMantissa(DecimalFraction value) {
        return value.mantissa;
      }

    /// <summary> </summary>
    /// <param name='value'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger GetExponent(DecimalFraction value) {
        return value.exponent;
      }

    /// <summary> </summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='e1'>A BigInteger object.</param>
    /// <param name='e2'>A BigInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger RescaleByExponentDiff(BigInteger mantissa, BigInteger e1, BigInteger e2) {
        bool negative = (mantissa.Sign < 0);
        if (mantissa.Sign == 0) return BigInteger.Zero;
        if (negative) mantissa = -mantissa;
        FastInteger diff = new FastInteger(e1).Subtract(e2).Abs();
        if (diff.CanFitInInt32()) {
          mantissa *= (BigInteger)(FindPowerOfTen(diff.AsInt32()));
        } else {
          mantissa *= (BigInteger)(FindPowerOfTenFromBig(diff.AsBigInteger()));
        }
        if (negative) mantissa = -mantissa;
        return mantissa;
      }

    /// <summary> </summary>
    /// <param name='mantissa'>A BigInteger object.</param>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
      public DecimalFraction CreateNew(BigInteger mantissa, BigInteger exponent) {
        return new DecimalFraction(mantissa, exponent);
      }

    /// <summary> </summary>
    /// <param name='lastDigit'>A 32-bit signed integer.</param>
    /// <param name='olderDigits'>A 32-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/><param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint, int lastDigit, int olderDigits) {
        return new DigitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <summary> </summary>
    /// <returns></returns>
    /// <remarks/><param name='bigint'>A BigInteger object.</param>
      public IShiftAccumulator CreateShiftAccumulator(BigInteger bigint) {
        return new DigitShiftAccumulator(bigint);
      }

    /// <summary> </summary>
    /// <param name='numerator'>A BigInteger object.</param>
    /// <param name='denominator'>A BigInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
      public bool HasTerminatingRadixExpansion(BigInteger numerator, BigInteger denominator) {
        // Simplify denominator based on numerator
        BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
        denominator /= gcd;
        if (denominator.IsZero)
          return false;
        // Eliminate factors of 2
        while (denominator.IsEven) {
          denominator >>= 1;
        }
        // Eliminate factors of 5
        while(true){
          BigInteger bigrem;
          BigInteger bigquo=BigInteger.DivRem(denominator,(BigInteger)5,out bigrem);
          if(!bigrem.IsZero)
            break;
          denominator=bigquo;
        }
        return denominator.CompareTo(BigInteger.One) == 0;
      }

    /// <summary> </summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='power'>A 64-bit signed integer.</param>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, long power) {
        if (power <= 0) return bigint;
        if(power<=Int32.MaxValue){
          if (bigint.Equals(BigInteger.One)) {
            bigint = (BigInteger)(FindPowerOfTen((int)power));
          } else if (bigint.IsZero) {
            return bigint;
          } else {
            bigint *= (BigInteger)(FindPowerOfTen((int)power));
          }
          return bigint;
        } else {
          return MultiplyByRadixPower(bigint,new FastInteger(power));
        }
      }

    /// <summary> </summary>
    /// <param name='bigint'>A BigInteger object.</param>
    /// <param name='power'>A FastInteger object.</param>
    /// <returns></returns>
    /// <remarks/>
      public BigInteger MultiplyByRadixPower(BigInteger bigint, FastInteger power) {
        if (power.Sign <= 0) return bigint;
        if (bigint.IsZero) return bigint;
        if (power.CanFitInInt32()) {
          bigint *= (BigInteger)(FindPowerOfTen(power.AsInt32()));
        } else {
          bigint *= (BigInteger)(FindPowerOfTenFromBig(power.AsBigInteger()));
        }
        return bigint;
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
      String mantissaString = this.mantissa.ToString();
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
            FastInteger tmpFast=new FastInteger(mantissaString.Length).Add(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareTo(Int32.MaxValue)>0 ? 
              Int32.MaxValue : tmpFast.AsInt32());
            builder.Append(mantissaString, 0, negaPos);
            builder.Append("0.");
            AppendString(builder, '0', new FastInteger(negaPos).Subtract(decimalPoint));
            builder.Append(mantissaString, negaPos, mantissaString.Length - negaPos);
          } else if (cmp == 0) {
            if (!decimalPoint.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = decimalPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            FastInteger tmpFast=new FastInteger(mantissaString.Length).Add(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareTo(Int32.MaxValue)>0 ? 
              Int32.MaxValue : tmpFast.AsInt32());
            builder.Append(mantissaString, 0, tmpInt);
            builder.Append("0.");
            builder.Append(mantissaString, tmpInt, mantissaString.Length - tmpInt);
          } else if (decimalPoint.CompareTo(new FastInteger(negaPos).Add(mantissaString.Length)) > 0) {
            FastInteger insertionPoint = new FastInteger(negaPos).Add(sbLength);
            if (!insertionPoint.CanFitInInt32())
              throw new NotSupportedException();
            int tmpInt = insertionPoint.AsInt32();
            if (tmpInt < 0) tmpInt = 0;
            FastInteger tmpFast=new FastInteger(mantissaString.Length).Add(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareTo(Int32.MaxValue)>0 ? 
              Int32.MaxValue : tmpFast.AsInt32());
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
            FastInteger tmpFast=new FastInteger(mantissaString.Length).Add(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareTo(Int32.MaxValue)>0 ? 
              Int32.MaxValue : tmpFast.AsInt32());
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
            FastInteger tmpFast=new FastInteger(mantissaString.Length).Add(6);
            builder = new System.Text.StringBuilder(
              tmpFast.CompareTo(Int32.MaxValue)>0 ? 
              Int32.MaxValue : tmpFast.AsInt32());
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
      if((bigfloat)==null)throw new ArgumentNullException("bigfloat");
      BigInteger bigintExp = bigfloat.Exponent;
      BigInteger bigintMant = bigfloat.Mantissa;
      if (bigintExp.IsZero) {
        // Integer
        return new DecimalFraction(bigintMant);
      } else if (bigintExp.Sign > 0) {
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

    /// <summary> Represents the number 1. </summary>
    public static readonly DecimalFraction One = new DecimalFraction(1);

    /// <summary> Represents the number 0. </summary>
    public static readonly DecimalFraction Zero = new DecimalFraction(0);
    /// <summary> Represents the number 10. </summary>
    public static readonly DecimalFraction Ten = new DecimalFraction(10);

    //----------------------------------------------------------------

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
    public DecimalFraction Abs() {
      if (this.Sign < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /// <summary> Gets an object with the same value as this one, but with the
    /// sign reversed. </summary>
    /// <returns></returns>
    public DecimalFraction Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new DecimalFraction(neg, this.exponent);
    }

    /// <summary> Divides this object by another decimal fraction and returns
    /// the result. When possible, the result will be exact.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The result would have
    /// a nonterminating decimal expansion.</exception>
    public DecimalFraction Divide(DecimalFraction divisor) {
      return Divide(divisor, new PrecisionContext(Rounding.Unnecessary));
    }

    /// <summary> Divides this object by another decimal fraction and returns
    /// a result with the same exponent as this object (the dividend). </summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <returns>The quotient of the two numbers.</returns>
    /// <exception cref='DivideByZeroException'>Attempted to divide
    /// by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public DecimalFraction Divide(DecimalFraction divisor, Rounding rounding) {
      return Divide(divisor, this.exponent, rounding);
    }

    /// <summary>Divides two DecimalFraction objects, and returns the
    /// integer part of the result, rounded down, with the preferred exponent
    /// set to this value's exponent minus the divisor's exponent.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <returns>The integer part of the quotient of the two objects.</returns>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    public DecimalFraction DivideToIntegerNaturalScale(
      DecimalFraction divisor
     ) {
      return DivideToIntegerNaturalScale(divisor, new PrecisionContext(Rounding.Down));
    }


    /// <summary> Removes trailing zeros from this object's mantissa. For
    /// example, 1.000 becomes 1.</summary>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>This value with trailing zeros removed. Note that if the
    /// result has a very high exponent and the context says to clamp high exponents,
    /// there may still be some trailing zeros in the mantissa. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow. The caller can handle a null return value by treating
    /// it as positive or negative infinity depending on the sign of this object's
    /// value.</returns>
    public DecimalFraction Reduce(
      PrecisionContext ctx) {
      return math.Reduce(this, ctx);
    }
    /// <summary> </summary>
    /// <param name='divisor'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction RemainderNaturalScale(
      DecimalFraction divisor
     ) {
      return Subtract(this.DivideToIntegerNaturalScale(divisor).Multiply(divisor));
    }


    /// <summary>Divides two DecimalFraction objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>A DecimalFraction object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='ctx'>A precision context object to control the rounding
    /// mode to use if the result must be scaled down to have the same exponent
    /// as this value. The precision and exponent range settings of this context
    /// are ignored. If HasFlags of the context is true, will also store the
    /// flags resulting from the operation (the flags are in addition to the
    /// pre-existing flags). Can be null, in which case the default rounding
    /// mode is HalfEven.</param>
    /// <returns>The quotient of the two objects.</returns>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public DecimalFraction Divide(
      DecimalFraction divisor,
      long desiredExponentSmall,
      PrecisionContext ctx
     ) {
      BigInteger desexp=(BigInteger)desiredExponentSmall;
      return Divide(divisor, desexp, ctx);
    }









    /// <summary>Divides two DecimalFraction objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>A DecimalFraction object.</param>
    /// <param name='desiredExponentSmall'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use if the result must
    /// be scaled down to have the same exponent as this value.</param>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    /// <returns>The quotient of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    public DecimalFraction Divide(
      DecimalFraction divisor,
      long desiredExponentSmall,
      Rounding rounding
     ) {
      BigInteger desexp=(BigInteger)desiredExponentSmall;
      return Divide(divisor, desexp, new PrecisionContext(rounding));
    }






    /// <summary>Divides two DecimalFraction objects and gives the result
    /// a particular exponent.</summary>
    /// <param name='divisor'>A DecimalFraction object.</param>
    /// <param name='exponent'>The desired exponent. A negative number
    /// places the cutoff point to the right of the usual decimal point. A positive
    /// number places the cutoff point to the left of the usual decimal point.</param>
    /// <param name='ctx'> A precision context to control precision and
    /// rounding of the result. The exponent range of this context is ignored.
    /// If HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null.</param>
    /// <returns>The quotient of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public DecimalFraction Divide(
      DecimalFraction divisor, BigInteger exponent, PrecisionContext ctx) {
      return math.Divide(this, divisor, exponent, ctx);
    }



    /// <summary>Divides two DecimalFraction objects, and gives a particular
    /// exponent to the result.</summary>
    /// <param name='divisor'>A DecimalFraction object.</param>
    /// <param name='desiredExponent'>The desired exponent. A negative
    /// number places the cutoff point to the right of the usual decimal point.
    /// A positive number places the cutoff point to the left of the usual decimal
    /// point.</param>
    /// <param name='rounding'>The rounding mode to use when converting
    /// the value to use the specified exponent.</param>
    /// <returns>The quotient of the two objects. Returns null if the return
    /// value would overflow the exponent range. A caller can handle a null
    /// return value by treating it as positive infinity if both operands
    /// have the same sign or as negative infinity if both operands have different
    /// signs.</returns>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the result is not exact.</exception>
    public DecimalFraction Divide(
      DecimalFraction divisor,
      BigInteger desiredExponent,
      Rounding rounding
     ) {
      return Divide(divisor, desiredExponent, new PrecisionContext(rounding));
    }


    /// <summary> </summary>
    /// <param name='context'> A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The absolute value of this object.</returns>
    /// <remarks/>
    public DecimalFraction Abs(PrecisionContext context) {
      return Abs().RoundToPrecision(context);
    }










    /// <summary> Returns a decimal fraction with the same value as this object
    /// but with the sign reversed.</summary>
    /// <param name='context'> A precision context to control precision,
    /// rounding, and exponent range of the result. If HasFlags of the context
    /// is true, will also store the flags resulting from the operation (the
    /// flags are in addition to the pre-existing flags). Can be null.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction Negate(PrecisionContext context) {
      return Negate().RoundToPrecision(context);
    }







    /// <summary> </summary>
    /// <param name='decfrac'>A DecimalFraction object.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction Add(DecimalFraction decfrac) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      return Add(decfrac, PrecisionContext.Unlimited);
    }






    /// <summary>Subtracts a DecimalFraction object from this instance.</summary>
    /// <param name='decfrac'>A DecimalFraction object.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <remarks/>
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      return Add(decfrac.Negate());
    }


    /// <summary>Subtracts a DecimalFraction object from this instance.</summary>
    /// <param name='decfrac'>A DecimalFraction object.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns>The difference of the two objects.</returns>
    /// <remarks/>
    public DecimalFraction Subtract(DecimalFraction decfrac, PrecisionContext ctx) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      return Add(decfrac.Negate(), ctx);
    }
    /// <summary> Multiplies two decimal fractions. The resulting scale
    /// will be the sum of the scales of the two decimal fractions. </summary>
    /// <param name='decfrac'> Another decimal fraction.</param>
    /// <returns> The product of the two decimal fractions. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow.</returns>
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      if((decfrac)==null)throw new ArgumentNullException("decfrac");
      return Multiply(decfrac, PrecisionContext.Unlimited);
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
    //----------------------------------------------------------------

    private static RadixMath<DecimalFraction> math = new RadixMath<DecimalFraction>(
      new DecimalMathHelper());

    /// <summary>Divides this object by another object, and returns the
    /// integer part of the result, with the preferred exponent set to thisValue
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
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The rounding mode is Rounding.Unnecessary
    /// and the integer part of the result is not exact.</exception>
    public DecimalFraction DivideToIntegerNaturalScale(
      DecimalFraction divisor, PrecisionContext ctx) {
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
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The result doesn't fit
    /// the given precision.</exception>
    public DecimalFraction DivideToIntegerZeroScale(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.DivideToIntegerZeroScale(this, divisor, ctx);
    }
    
    /// <summary>Finds the remainder that results when dividing two DecimalFraction
    /// objects. The remainder is the value that remains when the absolute
    /// value of this object is divided by the absolute value of the other object;
    /// the remainder has the same sign (positive or negative) as this object.</summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// No flags will be set from this operation even if HasFlags of the context
    /// is true. Can be null.</param>
    /// <returns>The remainder of the two objects.</returns>
    /// <remarks/><remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The result of integer
    /// division (the quotient, not the remainder) wouldn't fit the given
    /// precision.</exception>
    public DecimalFraction Remainder(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.Remainder(this, divisor, ctx);
    }
    /// <summary>Finds the distance to the closest multiple of the given
    /// divisor. <list type=''> <item>If this and the other object divide
    /// evenly, the result is 0.</item>
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
    /// </summary>
    /// <param name='divisor'>The divisor.</param>
    /// <param name='ctx'>A precision context object to control the precision.
    /// The rounding and exponent range settings of this context are ignored.
    /// No flags will be set from this operation even if HasFlags of the context
    /// is true. Can be null.</param>
    /// <returns>The distance of the closest multiple.</returns>
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>The result of integer
    /// division (the quotient, not the remainder) wouldn't fit the given
    /// precision.</exception>
    public DecimalFraction RemainderNear(
      DecimalFraction divisor, PrecisionContext ctx) {
      return math.RemainderNear(this, divisor, ctx);
    }

    /// <summary> Finds the largest value that's smaller than the given value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the largest value that's less than the given value.
    /// Returns null if the result is negative infinity.</returns>
    /// <remarks/><exception cref='System.ArgumentException'>"ctx"
    /// is null, the precision is 0, or "ctx" has an unlimited exponent range.</exception>
    public DecimalFraction NextMinus(
      PrecisionContext ctx
     ){
      return math.NextMinus(this,ctx);
    }


    /// <summary> Finds the smallest value that's greater than the given
    /// value.</summary>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns>Returns the smallest value that's greater than the given
    /// value. Returns null if the result is positive infinity.</returns>
    /// <remarks/><exception cref='System.ArgumentException'>"ctx"
    /// is null, the precision is 0, or "ctx" has an unlimited exponent range.</exception>
    public DecimalFraction NextPlus(
      PrecisionContext ctx
     ){
      return math.NextPlus(this,ctx);
    }
    
    /// <summary> </summary>
    /// <param name='otherValue'>A DecimalFraction object.</param>
    /// <param name='ctx'>A precision context object to control the precision
    /// and exponent range of the result. The rounding mode from this context
    /// is ignored. No flags will be set from this operation even if HasFlags
    /// of the context is true.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction NextToward(
      DecimalFraction otherValue,
      PrecisionContext ctx
     ){
      return math.NextToward(this,otherValue,ctx);
    }

    
    /// <summary>Divides this DecimalFraction object by another DecimalFraction
    /// object. The preferred exponent for the result is this object's exponent
    /// minus the divisor's exponent.</summary>
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
    /// <remarks/><exception cref='DivideByZeroException'>Attempted
    /// to divide by zero.</exception>
    /// <exception cref='ArithmeticException'>Either ctx is null or ctx's
    /// precision is 0, and the result would have a nonterminating decimal
    /// expansion; or, the rounding mode is Rounding.Unnecessary and the
    /// result is not exact.</exception>
    public DecimalFraction Divide(
      DecimalFraction divisor,
      PrecisionContext ctx
     ) {
      return math.Divide(this, divisor, ctx);
    }


    /// <summary> Gets the greater value between two decimal fractions.
    /// </summary>
    /// <returns> The larger value of the two objects.</returns>
    /// <param name='a'>A DecimalFraction object.</param>
    /// <param name='b'>A DecimalFraction object.</param>
    public static DecimalFraction Max(
      DecimalFraction a, DecimalFraction b) {
      return math.Max(a, b);
    }

    /// <summary> Gets the lesser value between two decimal fractions. </summary>
    /// <returns> The smaller value of the two objects.</returns>
    /// <param name='a'>A DecimalFraction object.</param>
    /// <param name='b'>A DecimalFraction object.</param>
    public static DecimalFraction Min(
      DecimalFraction a, DecimalFraction b) {
      return math.Min(a, b);
    }
    /// <summary> Gets the greater value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Max.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'>A DecimalFraction object.</param>
    /// <param name='b'>A DecimalFraction object.</param>
    public static DecimalFraction MaxMagnitude(
      DecimalFraction a, DecimalFraction b) {
      return math.MaxMagnitude(a, b);
    }
    
    /// <summary> Gets the lesser value between two values, ignoring their
    /// signs. If the absolute values are equal, has the same effect as Min.
    /// </summary>
    /// <returns></returns>
    /// <param name='a'>A DecimalFraction object.</param>
    /// <param name='b'>A DecimalFraction object.</param>
    public static DecimalFraction MinMagnitude(
      DecimalFraction a, DecimalFraction b) {
      return math.MinMagnitude(a, b);
    }
    /// <summary> Compares the mathematical values of this object and another
    /// object. <para> This method is not consistent with the Equals method
    /// because two different decimal fractions with the same mathematical
    /// value, but different exponents, will compare as equal.</para>
    /// </summary>
    /// <returns> Less than 0 if this object's value is less than the other
    /// value, or greater than 0 if this object's value is greater than the
    /// other value or if "other" is null, or 0 if both values are equal.</returns>
    /// <param name='other'>A DecimalFraction object.</param>
    public int CompareTo(
      DecimalFraction other) {
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
    public DecimalFraction Add(
      DecimalFraction decfrac, PrecisionContext ctx) {
      return math.Add(this, decfrac, ctx);
    }
    /// <summary> Returns a decimal fraction with the same value but a new
    /// exponent. </summary>
    /// <param name='otherValue'>A decimal fraction containing the desired
    /// exponent of the result. The mantissa is ignored.</param>
    /// <param name='ctx'> A precision context to control precision and
    /// rounding of the result. The exponent range of this context is ignored.
    /// If HasFlags of the context is true, will also store the flags resulting
    /// from the operation (the flags are in addition to the pre-existing
    /// flags). Can be null, in which case the default rounding mode is HalfEven.</param>
    /// <returns>A decimal fraction with the same value as this object but
    /// with the exponent changed.</returns>
    /// <remarks/><exception cref='ArithmeticException'>An overflow
    /// error occurred, or the result can't fit the given precision without
    /// rounding.</exception>
    public DecimalFraction Quantize(
      DecimalFraction otherValue, PrecisionContext ctx) {
      return math.Quantize(this, otherValue, ctx);
    }
    /// <summary> </summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction RoundToIntegralExact(
      PrecisionContext ctx) {
      return math.RoundToIntegralExact(this, ctx);
    }
    /// <summary> </summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns></returns>
    /// <remarks/>
    public DecimalFraction RoundToIntegralValue(
      PrecisionContext ctx) {
      return math.RoundToIntegralValue(this, ctx);
    }


    /// <summary> Multiplies two decimal fractions. The resulting scale
    /// will be the sum of the scales of the two decimal fractions. </summary>
    /// <param name='op'> Another decimal fraction.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The product of the two decimal fractions. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow. The caller can handle a null return value by treating
    /// it as negative infinity if this value and the other value have different
    /// signs, or as positive infinity if this value and the other value have
    /// the same sign.</returns>
    public DecimalFraction Multiply(
      DecimalFraction op, PrecisionContext ctx) {
      return math.Multiply(this, op, ctx);
    }

    /// <summary> Multiplies by one value, and then adds another value. </summary>
    /// <param name='op'> The value to multiply.</param>
    /// <param name='augend'> The value to add.</param>
    /// <param name='ctx'> A precision context to control precision, rounding,
    /// and exponent range of the result. If HasFlags of the context is true,
    /// will also store the flags resulting from the operation (the flags
    /// are in addition to the pre-existing flags). Can be null.</param>
    /// <returns> The result thisValue * multiplicand + augend. If a precision
    /// context is given, returns null if the result of rounding would cause
    /// an overflow. The caller can handle a null return value by treating
    /// it as negative infinity if this value and the other value have different
    /// signs, or as positive infinity if this value and the other value have
    /// the same sign.</returns>
    public DecimalFraction MultiplyAndAdd(
      DecimalFraction op, DecimalFraction augend, PrecisionContext ctx) {
      return math.MultiplyAndAdd(this, op, augend, ctx);
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
    public DecimalFraction RoundToPrecision(
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
    public DecimalFraction RoundToBinaryPrecision(
      PrecisionContext ctx) {
      return math.RoundToBinaryPrecision(this, ctx);
    }

  }
}