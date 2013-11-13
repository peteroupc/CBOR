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
  /// Represents an arbitrary-precision decimal floating-point number.
  /// Consists of a integer mantissa and an integer exponent,
  /// both arbitrary-precision.  The value of the number is equal
  /// to mantissa * 10^exponent.
  /// <para>
  /// Note:  This class doesn't yet implement certain operations,
  /// notably division, that require results to be rounded.  That's
  /// because I haven't decided yet how to incorporate rounding into
  /// the API, since the results of some divisions can't be represented
  /// exactly in a decimal fraction (for example, 1/3).  Should I include
  /// precision and rounding mode, as is done in Java's Big Decimal class,
  /// or should I also include minimum and maximum exponent in the
  /// rounding parameters, for better support when converting to other
  /// decimal number formats?  Or is there a better approach to supporting
  /// rounding?
  /// </para>
  /// </summary>
  public sealed class DecimalFraction : IComparable<DecimalFraction>, IEquatable<DecimalFraction> {
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
    public bool Equals(DecimalFraction obj) {
      DecimalFraction other = obj as DecimalFraction;
      if (other == null)
        return false;
      return this.exponent.Equals(other.exponent) &&
        this.mantissa.Equals(other.mantissa);
    }

    /// <summary>
    /// Determines whether this object's mantissa and exponent
    /// are equal to those of another object and that other
    /// object is a decimal fraction.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as DecimalFraction);
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
    /// Creates a decimal fraction with the value exponent*10^mantissa.
    /// </summary>
    /// <param name="mantissa">The unscaled value.</param>
    /// <param name="exponent">The decimal exponent.</param>
    public DecimalFraction(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a decimal fraction with the value exponentLong*10^mantissa.
    /// </summary>
    /// <param name="mantissa">The unscaled value.</param>
    /// <param name="exponentLong">The decimal exponent.</param>
    public DecimalFraction(BigInteger mantissa, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a decimal fraction with the given mantissa and an exponent of 0.
    /// </summary>
    /// <param name="mantissa">The desired value of the bigfloat</param>
    public DecimalFraction(BigInteger mantissa) {
      this.exponent = BigInteger.Zero;
      this.mantissa = mantissa;
    }

    /// <summary>
    /// Creates a decimal fraction with the given mantissa and an exponent of 0.
    /// </summary>
    /// <param name="mantissaLong">The desired value of the bigfloat</param>
    public DecimalFraction(long mantissaLong) {
      this.exponent = BigInteger.Zero;
      this.mantissa = (BigInteger)mantissaLong;
    }

    /// <summary>
    /// Creates a decimal fraction with the given mantissa and an exponent of 0.
    /// </summary>
    /// <param name="mantissaLong">The unscaled value.</param>
    /// <param name="exponentLong">The decimal exponent.</param>
    public DecimalFraction(long mantissaLong, long exponentLong) {
      this.exponent = (BigInteger)exponentLong;
      this.mantissa = (BigInteger)mantissaLong;
    }

    
    ///<summary>
    /// Creates a decimal fraction from a string that represents a number.
    /// <para>
    ///The format of the string generally consists of:<list type=''>
    /// <item>
    ///An optional '-' or '+' character (if '-', the value is negative.)</item>
    /// <item>
    ///One or more digits, with a single optional decimal point after the first digit and before the last digit.</item>
    /// <item>
    ///Optionally, E+ (positive exponent) or E- (negative exponent) plus one or more digits specifying the exponent.</item>
    /// </list>
    ///</para>
    /// <para>The format generally follows the definition in java.math.BigDecimal(),
    /// except that the digits must be ASCII digits ('0' through '9').</para>
    /// </summary>
    ///<param name='s'>
    ///A string that represents a number.</param>
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
      FastInteger mant = new FastInteger();
      bool haveDecimalPoint = false;
      bool haveDigits = false;
      bool haveExponent = false;
      FastInteger newScale = new FastInteger();
      int i = offset;
      for (; i < s.Length; i++) {
        if (s[i] >= '0' && s[i] <= '9') {
          int thisdigit = (int)(s[i] - '0');
          mant.Multiply(10);
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
        FastInteger exp=new FastInteger();
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
            int thisdigit = (int)(s[i] - '0') * offset;
            exp.Multiply(10);
            exp.Add(thisdigit);
          } else {
            throw new FormatException();
          }
        }
        if (!haveDigits)
          throw new FormatException();
        newScale.Add(exp);
      } else if (i != s.Length) {
        throw new FormatException();
      }
      if(negative)mant.Negate();
      return new DecimalFraction(mant.AsBigInteger(), newScale.AsBigInteger());
    }

    private BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      bool negative = (mantissa.Sign < 0);
      if(mantissa.Sign==0)return BigInteger.Zero;
      if (negative) mantissa = -mantissa;
      BigInteger diff=e1-(BigInteger)e2;
      diff=BigInteger.Abs(diff);
      mantissa*=(BigInteger)(FindPowerOfTen(diff));
      if (negative) mantissa = -mantissa;
      return mantissa;
    }

    /// <summary>
    /// Gets an object with the same value as this one, but
    /// with the sign reversed.
    /// </summary>
    public DecimalFraction Negate() {
      BigInteger neg = -(BigInteger)this.mantissa;
      return new DecimalFraction(neg, this.exponent);
    }

    /// <summary>
    /// Gets the absolute value of this object.
    /// </summary>
    public DecimalFraction Abs() {
      if (this.Sign < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /// <summary>
    /// Gets the greater value between two DecimalFraction values.
    /// </summary>
    public DecimalFraction Max(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      return a.CompareTo(b) > 0 ? a : b;
    }

    internal enum RoundingMode {
      Down, Up,
      HalfDown, HalfUp, HalfEven,
      Ceiling, Floor
    }

    internal static BigInteger FindPowerOfFive(BigInteger diff){
      if(diff.Sign<=0)return BigInteger.One;
      BigInteger bigpow=BigInteger.Zero;
      FastInteger intcurexp=new FastInteger(diff);
      if(intcurexp.CompareTo(27)<=0){
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa=BigInteger.One;
      while (intcurexp.Sign>0) {
        if(intcurexp.CompareTo(27)<=0){
          bigpow=FindPowerOfFive(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if(intcurexp.CompareTo(9999999)<=0){
          bigpow=BigInteger.Pow(FindPowerOfFive(1),intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if(bigpow.IsZero)
            bigpow=BigInteger.Pow(FindPowerOfFive(1),9999999);
          mantissa *= bigpow;
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }
    
    internal static BigInteger FindPowerOfTen(BigInteger diff){
      if(diff.Sign<=0)return BigInteger.One;
      BigInteger bigpow=BigInteger.Zero;
      FastInteger intcurexp=new FastInteger(diff);
      if(intcurexp.CompareTo(18)<=0){
        return FindPowerOfTen(intcurexp.AsInt32());
      }
      BigInteger mantissa=BigInteger.One;
      while (intcurexp.Sign>0) {
        if(intcurexp.CompareTo(18)<=0){
          bigpow=FindPowerOfTen(intcurexp.AsInt32());
          mantissa*=(BigInteger)bigpow;
          break;
        } else if(intcurexp.CompareTo(9999999)<=0){
          bigpow=BigInteger.Pow(FindPowerOfTen(1),intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if(bigpow.IsZero)
            bigpow=BigInteger.Pow(FindPowerOfTen(1),9999999);
          mantissa *= bigpow;
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }

    internal static BigInteger FindPowerOfFive(long precision){
      if(precision<=0)return BigInteger.One;
      if(precision<BigIntPowersOfFive.Length)
        return BigIntPowersOfFive[(int)precision];
      BigInteger ret=BigInteger.One;
      BigInteger bigpow=BigInteger.Zero;
      while(precision>0){
        if(precision<=27){
          bigpow=BigIntPowersOfFive[(int)precision];
          ret *= (BigInteger)bigpow;
          break;
        } else if(precision<=9999999){
          bigpow=BigInteger.Pow(BigIntPowersOfFive[1],(int)precision);
          ret *= (BigInteger)bigpow;
          break;
        } else {
          if(bigpow.IsZero)
            bigpow=BigInteger.Pow(BigIntPowersOfFive[1],9999999);
          ret *= (BigInteger)bigpow;
          precision-=9999999;
        }
      }
      return ret;
    }
    
    internal static BigInteger FindPowerOfTen(long precision){
      if(precision<=0)return BigInteger.One;
      if(precision<BigIntPowersOfTen.Length)
        return BigIntPowersOfTen[(int)precision];
      BigInteger ret=BigInteger.One;
      BigInteger bigpow=BigInteger.Zero;
      while(precision>0){
        if(precision<=18){
          bigpow=BigIntPowersOfTen[(int)precision];
          ret *= (BigInteger)bigpow;
          break;
        } else if(precision<=9999999){
          bigpow=BigInteger.Pow(BigIntPowersOfTen[1],(int)precision);
          ret *= (BigInteger)bigpow;
          break;
        } else {
          if(bigpow.IsZero)
            bigpow=BigInteger.Pow(BigIntPowersOfTen[1],9999999);
          ret *= (BigInteger)bigpow;
          precision-=9999999;
        }
      }
      return ret;
    }
    
    
    internal static BigInteger[] DivideWithPrecision(
      BigInteger dividend, // NOTE: Assumes dividend is nonnegative
      BigInteger bigdivisor,
      long minimumPrecision // in decimal digits
    ) {
      if(bigdivisor.IsZero)
        throw new DivideByZeroException("division by zero");
      BigInteger bigquotient = BigInteger.Zero;
      BigInteger bigrem = BigInteger.Zero;
      FastInteger scale = new FastInteger();
      BigInteger minInclusive=FindPowerOfTen(minimumPrecision-1);
      BigInteger bigintTen = FindPowerOfTen(1);
      if(dividend.CompareTo(bigdivisor) < 0){
        while (dividend.CompareTo(bigdivisor) < 0) {
          scale.Add(1);
          dividend *= bigintTen;
        }
      } else {
        BigInteger divisormul;
        divisormul=bigdivisor*(BigInteger)(FindPowerOfTen(5));
        while (dividend.CompareTo(divisormul) >= 0) {
          scale.Add(5);
          bigdivisor=divisormul;
          divisormul=bigdivisor*(BigInteger)(FindPowerOfTen(5));
        }
        divisormul=bigdivisor*(BigInteger)(FindPowerOfTen(1));
        while (dividend.CompareTo(divisormul) >= 0) {
          scale.Add(1);
          bigdivisor=divisormul;
          divisormul=bigdivisor*(BigInteger)(FindPowerOfTen(1));
        }
      }
      while(true) {
        BigInteger newquotient = BigInteger.DivRem(
          (BigInteger)dividend,
          (BigInteger)bigdivisor,
          out bigrem);
        if(bigquotient.IsZero)
          bigquotient=newquotient;
        else
          bigquotient += (BigInteger)newquotient;
        dividend = bigrem;
        if (scale.Sign >= 0 && bigrem.IsZero) {
          break;
        }
        if(bigquotient.CompareTo(minInclusive)>=0){
          break;
        }
        scale.Add(1);
        bigquotient *= bigintTen;
        dividend *= bigintTen;
      }
      if (!dividend.IsZero) {
        // Use half-even rounding
        BigInteger halfbigdivisor = bigdivisor;
        halfbigdivisor >>= 1;
        int cmp=dividend.CompareTo(halfbigdivisor);
        if (cmp > 0) {
          // Greater than half
          bigquotient += BigInteger.One;
        } else if(cmp==0 && bigdivisor.IsEven){
          // Exactly half
          if(!bigquotient.IsEven){
            // Result is odd
            bigquotient+=BigInteger.One;
          }
        }
        
      }
      BigInteger scaleValue=scale.AsBigInteger();
      BigInteger negscale = -(BigInteger)scaleValue;
      return new BigInteger[]{
        bigquotient,dividend,negscale
      };
    }
    
    internal static BigInteger[] Round(
      BigInteger coeff,
      BigInteger exponent,
      long precision,
      RoundingMode mode
    ){
      int sign=coeff.Sign;
      if(sign!=0){
        BigInteger powerForPrecision=FindPowerOfTen(precision);
        if(sign<0)coeff=-coeff;
        int digitsFollowingLeftmost=0; // OR of all digits following the leftmost digit
        int digitLeftmost=0;
        int exponentChange=0;
        while(coeff.CompareTo(powerForPrecision)<0){
          BigInteger digit;
          BigInteger quotient=BigInteger.DivRem(coeff,FindPowerOfTen(1),out digit);
          coeff=quotient;
          int intDigit=(int)digit;
          digitsFollowingLeftmost|=digitLeftmost;
          digitLeftmost=intDigit;
          exponentChange+=1;
          if(exponentChange>=1000){
            exponent+=(BigInteger)exponentChange;
            exponentChange=0;
          }
        }
        if(exponentChange>0){
          exponent+=(BigInteger)exponentChange;
        }
        bool incremented=false;
        if(mode==RoundingMode.HalfUp){
          if(digitLeftmost>=5){
            coeff+=BigInteger.One;
            incremented=true;
          }
        } else if(mode==RoundingMode.Up){
          if((digitLeftmost|digitsFollowingLeftmost)!=0){
            coeff+=BigInteger.One;
            incremented=true;
          }
        } else if(mode==RoundingMode.HalfEven){
          if(digitLeftmost>5 || (digitLeftmost==5 && digitsFollowingLeftmost!=0)){
            coeff+=BigInteger.One;
            incremented=true;
          } else if(digitLeftmost==5 && !coeff.IsEven){
            coeff+=BigInteger.One;
            incremented=true;
          }
        } else if(mode==RoundingMode.HalfDown){
          if(digitLeftmost>5 || (digitLeftmost==5 && digitsFollowingLeftmost!=0)){
            coeff+=BigInteger.One;
            incremented=true;
          }
        } else if(mode==RoundingMode.Ceiling){
          if((digitLeftmost|digitsFollowingLeftmost)!=0 && sign>0){
            coeff+=BigInteger.One;
            incremented=true;
          }
        } else if(mode==RoundingMode.Floor){
          if((digitLeftmost|digitsFollowingLeftmost)!=0 && sign<0){
            coeff+=BigInteger.One;
            incremented=true;
          }
        }
        if(incremented && coeff.CompareTo(powerForPrecision)>=0){
          coeff/=(BigInteger)(FindPowerOfTen(1));
          exponent+=BigInteger.One;
        }
      }
      if(sign<0)coeff=-coeff;
      return new BigInteger[]{ coeff, exponent };
    }
    
    /// <summary>
    /// Gets the lesser value between two DecimalFraction values.
    /// </summary>
    public DecimalFraction Min(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new ArgumentNullException("a");
      if (b == null) throw new ArgumentNullException("b");
      return a.CompareTo(b) > 0 ? b : a;
    }

    /// <summary>
    /// Finds the sum of this object and another decimal fraction.
    /// The result's exponent is set to the lower of the exponents
    /// of the two operands.
    /// </summary>
    public DecimalFraction Add(DecimalFraction decfrac) {
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa + (BigInteger)decfrac.mantissa, exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant + (BigInteger)decfrac.mantissa, decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant + (BigInteger)this.mantissa, exponent);
      }
    }

    /// <summary>
    /// Finds the difference between this object and another decimal fraction.
    /// The result's exponent is set to the lower of the exponents
    /// of the two operands.
    /// </summary>
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa - (BigInteger)decfrac.mantissa, exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant - (BigInteger)decfrac.mantissa, decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          this.mantissa - (BigInteger)newmant, exponent);
      }
    }

    /// <summary>
    /// Multiplies two decimal fractions.  The resulting scale will be the sum
    /// of the scales of the two decimal fractions.
    /// </summary>
    /// <param name="decfrac">Another decimal fraction.</param>
    /// <returns>The product of the two decimal fractions.</returns>
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      BigInteger newexp = (this.exponent + (BigInteger)decfrac.exponent);
      return new DecimalFraction(mantissa * (BigInteger)decfrac.mantissa, newexp);
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
    /// Compares the mathematical values of two decimal fractions.
    /// <para>This method is not consistent with the Equals method
    /// because two different decimal fractions with the same mathematical
    /// value, but different exponents, will compare as equal.</para>
    /// </summary>
    /// <param name="other">Another decimal fraction.</param>
    /// <returns>Less than 0 if this value is less than the other
    /// value, or greater than 0 if this value is greater than the other
    /// value or if "other" is null, or 0 if both values are equal.</returns>
    public int CompareTo(DecimalFraction decfrac) {
      if (decfrac == null) return 1;
      int s = this.Sign;
      int ds = decfrac.Sign;
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.CompareTo((BigInteger)decfrac.exponent);
      if (expcmp == 0) {
        return mantissa.CompareTo((BigInteger)decfrac.mantissa);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return newmant.CompareTo((BigInteger)decfrac.mantissa);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return this.mantissa.CompareTo((BigInteger)newmant);
      }
    }

    private bool InsertString(StringBuilder builder, FastInteger index, char c) {
      if (index.CompareTo(Int32.MaxValue) > 0 || index.Sign<0) {
        throw new NotSupportedException();
      }
      int iindex = index.AsInt32();
      builder.Insert(iindex, c);
      return true;
    }

    private bool InsertString(StringBuilder builder, FastInteger index, char c, FastInteger count) {
      if (count.CompareTo(Int32.MaxValue) > 0 || count.Sign<0) {
        throw new NotSupportedException();
      }
      if (index.CompareTo(Int32.MaxValue) > 0 || index.Sign<0) {
        throw new NotSupportedException();
      }
      int icount = count.AsInt32();
      int iindex = index.AsInt32();
      for (int i = icount - 1; i >= 0; i--) {
        builder.Insert(iindex, c);
      }
      return true;
    }

    private bool AppendString(StringBuilder builder, char c, FastInteger count) {
      if (count.CompareTo(Int32.MaxValue) > 0 || count.Sign<0) {
        throw new NotSupportedException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; i--) {
        builder.Append(c);
      }
      return true;
    }

    private bool InsertString(StringBuilder builder, FastInteger index, string c) {
      if (index.CompareTo(Int32.MaxValue) > 0 || index.Sign<0) {
        throw new NotSupportedException();
      }
      int iindex = index.AsInt32();
      builder.Insert(iindex, c);
      return true;
    }

    private string ToStringInternal(int mode) {
      // Using Java's rules for converting DecimalFraction
      // values to a string
      System.Text.StringBuilder builder = new System.Text.StringBuilder();
      builder.Append(this.mantissa.ToString(
        System.Globalization.CultureInfo.InvariantCulture));
      FastInteger adjustedExponent=new FastInteger(this.exponent);
      FastInteger scale=new FastInteger(this.exponent).Negate();
      FastInteger sbLength=new FastInteger(builder.Length);
      FastInteger negaPos=new FastInteger(0);
      if (builder[0] == '-') {
        sbLength.Add(-1);
        negaPos.Add(1);
      }
      bool iszero = (this.mantissa.IsZero);
      if (mode == 2 && iszero && scale.Sign < 0) {
        // special case for zero in plain
        return builder.ToString();
      }
      adjustedExponent.Add(sbLength).Add(-1);
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) { // engineering string adjustments
        FastInteger newExponent=new FastInteger(adjustedExponent);
        bool adjExponentNegative = (adjustedExponent.Sign < 0);
        int intphase = new FastInteger(adjustedExponent).Abs().Mod(3).AsInt32();
        if (iszero && (adjustedExponent.CompareTo(threshold) < 0 ||
                       scale.Sign < 0)) {
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
                         scale.Sign >= 0))) {
        FastInteger decimalPoint = new FastInteger(scale).Negate().Add(negaPos).Add(sbLength);
        if (scale.Sign > 0) {
          int cmp = decimalPoint.CompareTo(negaPos);
          if (cmp < 0) {
            InsertString(builder, negaPos, '0', new FastInteger(negaPos).Subtract(decimalPoint));
            InsertString(builder, negaPos, "0.");
          } else if (cmp == 0) {
            InsertString(builder, decimalPoint, "0.");
          } else if (decimalPoint.CompareTo(new FastInteger(negaPos).Add(builder.Length)) > 0) {
            InsertString(builder, new FastInteger(negaPos).Add(sbLength), '.');
            InsertString(builder, new FastInteger(negaPos).Add(sbLength), '0',
                         new FastInteger(decimalPoint).Subtract(builder.Length));
          } else {
            InsertString(builder, decimalPoint, '.');
          }
        }
        if (mode == 2 && scale.Sign < 0) {
          FastInteger negscale = new FastInteger(scale).Negate();
          AppendString(builder, '0', negscale);
        }
        return builder.ToString();
      } else {
        if (mode == 1 && iszero && decimalPointAdjust.CompareTo(1) > 0) {
          builder.Append('.');
          AppendString(builder, '0', new FastInteger(decimalPointAdjust).Add(-1));
        } else {
          FastInteger tmp = new FastInteger(negaPos).Add(decimalPointAdjust);
          int cmp = tmp.CompareTo(builder.Length);
          if (cmp > 0) {
            tmp.Subtract(builder.Length);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            InsertString(builder, tmp, '.');
          }
        }
        if (adjustedExponent.Sign!=0) {
          builder.Append('E');
          builder.Append(adjustedExponent.Sign < 0 ? '-' : '+');
          FastInteger sbPos = new FastInteger(builder.Length);
          adjustedExponent.Abs();
          while (adjustedExponent.Sign!=0) {
            int digit = new FastInteger(adjustedExponent).Mod(10).AsInt32();
            InsertString(builder, sbPos, (char)('0' + digit));
            adjustedExponent.Divide(10);
          }
        }
        return builder.ToString();
      }
    }
    
    private static BigInteger[] BigIntPowersOfTen=new BigInteger[]{
      (BigInteger)1, (BigInteger)10, (BigInteger)100, (BigInteger)1000, (BigInteger)10000, (BigInteger)100000, (BigInteger)1000000, (BigInteger)10000000, (BigInteger)100000000, (BigInteger)1000000000,
      (BigInteger)10000000000L, (BigInteger)100000000000L, (BigInteger)1000000000000L, (BigInteger)10000000000000L,
      (BigInteger)100000000000000L, (BigInteger)1000000000000000L, (BigInteger)10000000000000000L,
      (BigInteger)100000000000000000L, (BigInteger)1000000000000000000L
    };
    
    private static BigInteger[] BigIntPowersOfFive=new BigInteger[]{
      (BigInteger)1, (BigInteger)5, (BigInteger)25, (BigInteger)125, (BigInteger)625, (BigInteger)3125, (BigInteger)15625, (BigInteger)78125, (BigInteger)390625,
      (BigInteger)1953125, (BigInteger)9765625, (BigInteger)48828125, (BigInteger)244140625, (BigInteger)1220703125,
      (BigInteger)6103515625L, (BigInteger)30517578125L, (BigInteger)152587890625L, (BigInteger)762939453125L,
      (BigInteger)3814697265625L, (BigInteger)19073486328125L, (BigInteger)95367431640625L,
      (BigInteger)476837158203125L, (BigInteger)2384185791015625L, (BigInteger)11920928955078125L,
      (BigInteger)59604644775390625L, (BigInteger)298023223876953125L, (BigInteger)1490116119384765625L,
      (BigInteger)7450580596923828125L
    };

    
    /// <summary>
    /// Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when
    /// converting to a big integer.
    /// </summary>
    public BigInteger ToBigInteger() {
      int sign=this.Exponent.Sign;
      if (sign==0) {
        return this.Mantissa;
      } else if (sign>0) {
        BigInteger bigmantissa = this.Mantissa;
        bigmantissa*=(BigInteger)(FindPowerOfTen(this.Exponent));
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.Mantissa;
        BigInteger bigexponent=this.Exponent;
        bigexponent=-bigexponent;
        bigmantissa/=(BigInteger)(FindPowerOfTen(bigexponent));
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
      return BigFloat.FromDecimalFraction(this).ToSingle();
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
      return BigFloat.FromDecimalFraction(this).ToDouble();
    }

    /// <summary>
    /// Creates a decimal fraction from a 32-bit floating-point number.
    /// This method computes the exact value of the floating point number,
    /// not an approximation, as is often the case by converting
    /// the number to a string.
    /// </summary>
    /// <param name="dbl">A 32-bit floating-point number.</param>
    /// <returns>A decimal fraction with the same value as "flt".</returns>
    /// <exception cref="OverflowException">"flt" is infinity or not-a-number.</exception>
    public static DecimalFraction FromSingle(float flt) {
      int value = ConverterInternal.SingleToInt32Bits(flt);
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
        bigmantissa*=(BigInteger)(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }

    /// <summary>
    /// Creates a decimal fraction from a 64-bit floating-point number.
    /// This method computes the exact value of the floating point number,
    /// not an approximation, as is often the case by converting
    /// the number to a string.
    /// </summary>
    /// <param name="dbl">A 64-bit floating-point number.</param>
    /// <returns>A decimal fraction with the same value as "dbl"</returns>
    /// <exception cref="OverflowException">"dbl" is infinity or not-a-number.</exception>
    public static DecimalFraction FromDouble(double dbl) {
      long value = ConverterInternal.DoubleToInt64Bits(dbl);
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
        bigmantissa*=(BigInteger)(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa = -(BigInteger)bigmantissa;
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }
    
    /// <summary>
    /// Creates a decimal fraction from an arbitrary-precision
    /// binary floating-point number.
    /// </summary>
    /// <param name="bigfloat">A bigfloat.</param>
    public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
      BigInteger bigintExp = bigfloat.Exponent;
      BigInteger bigintMant = bigfloat.Mantissa;
      if (bigintExp.IsZero) {
        // Integer
        return new DecimalFraction(bigintMant);
      } else if (bigintExp > 0) {
        // Scaled integer
        FastInteger intcurexp=new FastInteger(bigintExp);
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
        BigInteger negbigintExp=-(BigInteger)bigintExp;
        bigmantissa*=(BigInteger)(FindPowerOfFive(negbigintExp));
        return new DecimalFraction(bigmantissa, bigintExp);
      }
    }

    /*
    public DecimalFraction MovePointLeft(BigInteger steps){
      if(steps.IsZero)return this;
      return new DecimalFraction(this.Mantissa,this.Exponent-(BigInteger)steps);
    }
    
    public DecimalFraction MovePointRight(BigInteger steps){
      if(steps.IsZero)return this;
      return new DecimalFraction(this.Mantissa,this.Exponent+(BigInteger)steps);
    }

    internal DecimalFraction Rescale(BigInteger scale)
    {
      throw new NotImplementedException();
    }
 
    internal DecimalFraction RoundToIntegralValue(BigInteger scale)
    {
      return Rescale(BigInteger.Zero);
    }
    internal DecimalFraction Normalize()
    {
      if(this.Mantissa.IsZero)
        return new DecimalFraction(0);
      BigInteger mant=this.Mantissa;
      BigInteger exp=this.Exponent;
      bool changed=false;
      while((mant%(BigInteger)10)==0){
        mant/=(BigInteger)10;
        exp+=BigInteger.One;
        changed=true;
      }
      if(!changed)return this;
      return new DecimalFraction(mant,exp);
    }
     */

    ///<summary>
    /// Converts this value to a string.
    ///The format of the return value is exactly the same as that of the java.math.BigDecimal.toString() method.
    /// </summary>
    public override string ToString() {
      return ToStringInternal(0);
    }


    ///<summary>
    /// Same as toString(), except that when an exponent is used it will be a multiple of 3.
    /// The format of the return value follows the format of the
    /// java.math.BigDecimal.toEngineeringString() method.
    /// </summary>
    public string ToEngineeringString() {
      return ToStringInternal(1);
    }

    ///<summary>
    /// Converts this value to a string, but without an exponent part.
    ///The format of the return value follows the format of the
    /// java.math.BigDecimal.toPlainString()
    /// method.
    /// </summary>
    public string ToPlainString() {
      return ToStringInternal(2);
    }
  }
}