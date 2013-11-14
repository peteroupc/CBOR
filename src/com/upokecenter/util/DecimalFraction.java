package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */


import java.math.*;


  
  /**
   * Represents an arbitrary-precision decimal floating-point number.
   * Consists of a integer mantissa and an integer exponent, both arbitrary-precision.
   * The value of the number is equal to mantissa * 10^exponent. <p> Note:
   * This class doesn't yet implement certain operations, notably division,
   * that require results to be rounded. That's because I haven't decided
   * yet how to incorporate rounding into the API, since the results of
   * some divisions can't be represented exactly in a decimal fraction
   * (for example, 1/3). Should I include precision and rounding mode,
   * as is done in Java's Big Decimal class, or should I also include minimum
   * and maximum exponent in the rounding parameters, for better support
   * when converting to other decimal number formats? Or is there a better
   * approach to supporting rounding? </p>
   */
  public final class DecimalFraction implements Comparable<DecimalFraction> {
    BigInteger exponent;
    BigInteger mantissa;
    /**
     * Gets this object's exponent. This object's value will be an integer
     * if the exponent is positive or zero.
     */
    public BigInteger getExponent() { return exponent; }

    /**
     * Gets this object's unscaled value.
     */
    public BigInteger getMantissa() { return mantissa; }

    
    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object.
     */
    public boolean equals(DecimalFraction obj) {
      DecimalFraction other = ((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null);
      if (other == null)
        return false;
      return this.exponent.equals(other.exponent) &&
        this.mantissa.equals(other.mantissa);
    }

    /**
     * Determines whether this object's mantissa and exponent are equal
     * to those of another object and that other object is a decimal fraction.
     */
    @Override public boolean equals(Object obj) {
      return equals(((obj instanceof DecimalFraction) ? (DecimalFraction)obj : null));
    }

    /**
     * Calculates this object's hash code.
     * @return This object's hash code.
     */
    @Override public int hashCode() {
      int hashCode_ = 0;
      {
        hashCode_ += 1000000007 * exponent.hashCode();
        hashCode_ += 1000000009 * mantissa.hashCode();
      }
      return hashCode_;
    }
    



    /**
     * Creates a decimal fraction with the value exponent*10^mantissa.
     * @param mantissa The unscaled value.
     * @param exponent The decimal exponent.
     */
    public DecimalFraction(BigInteger mantissa, BigInteger exponent) {
      this.exponent = exponent;
      this.mantissa = mantissa;
    }

    /**
     * Creates a decimal fraction with the value exponentLong*10^mantissa.
     * @param mantissa The unscaled value.
     * @param exponentLong The decimal exponent.
     */
    public DecimalFraction(BigInteger mantissa, long exponentLong) {
      this.exponent = BigInteger.valueOf(exponentLong);
      this.mantissa = mantissa;
    }

    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissa The desired value of the bigfloat
     */
    public DecimalFraction(BigInteger mantissa) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = mantissa;
    }

    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissaLong The desired value of the bigfloat
     */
    public DecimalFraction(long mantissaLong) {
      this.exponent = BigInteger.ZERO;
      this.mantissa = BigInteger.valueOf(mantissaLong);
    }

    /**
     * Creates a decimal fraction with the given mantissa and an exponent
     * of 0.
     * @param mantissaLong The unscaled value.
     * @param exponentLong The decimal exponent.
     */
    public DecimalFraction(long mantissaLong, long exponentLong) {
      this.exponent = BigInteger.valueOf(exponentLong);
      this.mantissa = BigInteger.valueOf(mantissaLong);
    }

    
    /**
     * Creates a decimal fraction from a string that represents a number.
     * <p> The format of the string generally consists of:<ul> <li> An optional
     * '-' or '+' character (if '-', the value is negative.)</li> <li> One
     * or more digits, with a single optional decimal point after the first
     * digit and before the last digit.</li> <li> Optionally, E+ (positive
     * exponent) or E- (negative exponent) plus one or more digits specifying
     * the exponent.</li> </ul> </p> <p>The format generally follows the
     * definition in java.math.BigDecimal(), except that the digits must
     * be ASCII digits ('0' through '9').</p>
     * @param s A string that represents a number.
     */
    public static DecimalFraction FromString(String s) {
      if (s == null)
        throw new NullPointerException("s");
      if (s.length() == 0)
        throw new NumberFormatException();
      int offset = 0;
      boolean negative = false;
      if (s.charAt(0) == '+' || s.charAt(0) == '-') {
        negative = (s.charAt(0) == '-');
        offset++;
      }
      FastInteger mant = new FastInteger();
      boolean haveDecimalPoint = false;
      boolean haveDigits = false;
      boolean haveExponent = false;
      FastInteger newScale = new FastInteger();
      int i = offset;
      for (; i < s.length(); i++) {
        if (s.charAt(i) >= '0' && s.charAt(i) <= '9') {
          int thisdigit = (int)(s.charAt(i) - '0');
          mant.Multiply(10);
          mant.Add(thisdigit);
          haveDigits = true;
          if (haveDecimalPoint) {
            newScale.Add(-1);
          }
        } else if (s.charAt(i) == '.') {
          if (haveDecimalPoint)
            throw new NumberFormatException();
          haveDecimalPoint = true;
        } else if (s.charAt(i) == 'E' || s.charAt(i) == 'e') {
          haveExponent = true;
          i++;
          break;
        } else {
          throw new NumberFormatException();
        }
      }
      if (!haveDigits)
        throw new NumberFormatException();
      if (haveExponent) {
        FastInteger exp=new FastInteger();
        offset = 1;
        haveDigits = false;
        if (i == s.length()) throw new NumberFormatException();
        if (s.charAt(i) == '+' || s.charAt(i) == '-') {
          if (s.charAt(i) == '-') offset = -1;
          i++;
        }
        for (; i < s.length(); i++) {
          if (s.charAt(i) >= '0' && s.charAt(i) <= '9') {
            haveDigits = true;
            int thisdigit = (int)(s.charAt(i) - '0') * offset;
            exp.Multiply(10);
            exp.Add(thisdigit);
          } else {
            throw new NumberFormatException();
          }
        }
        if (!haveDigits)
          throw new NumberFormatException();
        newScale.Add(exp);
      } else if (i != s.length()) {
        throw new NumberFormatException();
      }
      if(negative)mant.Negate();
      return new DecimalFraction(mant.AsBigInteger(), newScale.AsBigInteger());
    }

    private BigInteger
      RescaleByExponentDiff(BigInteger mantissa,
                            BigInteger e1,
                            BigInteger e2) {
      boolean negative = (mantissa.signum() < 0);
      if(mantissa.signum()==0)return BigInteger.ZERO;
      if (negative) mantissa=mantissa.negate();
      BigInteger diff=e1.subtract(e2);
      diff=(diff).abs();
      mantissa=mantissa.multiply(FindPowerOfTen(diff));
      if (negative) mantissa=mantissa.negate();
      return mantissa;
    }

    /**
     * Gets an object with the same ((value instanceof this one) ? (this one)value
     * : null), but with the sign reversed.
     */
    public DecimalFraction Negate() {
      BigInteger neg=(this.mantissa).negate();
      return new DecimalFraction(neg, this.exponent);
    }

    /**
     * Gets the absolute value of this object.
     */
    public DecimalFraction Abs() {
      if (this.signum() < 0) {
        return Negate();
      } else {
        return this;
      }
    }

    /**
     * Gets the greater value between two DecimalFraction values.
     */
    public DecimalFraction Max(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? a : b;
    }

    enum RoundingMode {
      Down, Up,
      HalfDown, HalfUp, HalfEven,
      Ceiling, Floor
    }

    static BigInteger FindPowerOfFive(BigInteger diff){
      if(diff.signum()<=0)return BigInteger.ONE;
      BigInteger bigpow=BigInteger.ZERO;
      FastInteger intcurexp=new FastInteger(diff);
      if(intcurexp.compareTo(54)<=0){
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa=BigInteger.ONE;
      while (intcurexp.signum()>0) {
        if(intcurexp.compareTo(27)<=0){
          bigpow=FindPowerOfFive(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else if(intcurexp.compareTo(9999999)<=0){
          bigpow=(FindPowerOfFive(1)).pow(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else {
          if(bigpow.signum()==0)
            bigpow=(FindPowerOfFive(1)).pow(9999999);
          mantissa=mantissa.multiply(bigpow);
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }
    
    static BigInteger FindPowerOfTen(BigInteger diff){
      if(diff.signum()<=0)return BigInteger.ONE;
      BigInteger bigpow=BigInteger.ZERO;
      FastInteger intcurexp=new FastInteger(diff);
      if(intcurexp.compareTo(36)<=0){
        return FindPowerOfTen(intcurexp.AsInt32());
      }
      BigInteger mantissa=BigInteger.ONE;
      while (intcurexp.signum()>0) {
        if(intcurexp.compareTo(18)<=0){
          bigpow=FindPowerOfTen(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else if(intcurexp.compareTo(9999999)<=0){
          bigpow=(FindPowerOfTen(1)).pow(intcurexp.AsInt32());
          mantissa=mantissa.multiply(bigpow);
          break;
        } else {
          if(bigpow.signum()==0)
            bigpow=(FindPowerOfTen(1)).pow(9999999);
          mantissa=mantissa.multiply(bigpow);
          intcurexp.Add(-9999999);
        }
      }
      return mantissa;
    }

    static BigInteger FindPowerOfFive(long precision){
      if(precision<=0)return BigInteger.ONE;
      BigInteger bigpow;
      BigInteger ret;
      if(precision<=27)
        return BigIntPowersOfFive[(int)precision];
      if(precision<=54){
        ret=BigIntPowersOfFive[27];
        bigpow=BigIntPowersOfFive[((int)precision)-27];
        ret=ret.multiply(bigpow);
        return ret;
      }
      ret=BigInteger.ONE;
      bigpow=BigInteger.ZERO;
      while(precision>0){
        if(precision<=27){
          bigpow=BigIntPowersOfFive[(int)precision];
          ret=ret.multiply(bigpow);
          break;
        } else if(precision<=9999999){
          bigpow=(BigIntPowersOfFive[1]).pow((int)precision);
          ret=ret.multiply(bigpow);
          break;
        } else {
          if(bigpow.signum()==0)
            bigpow=(BigIntPowersOfFive[1]).pow(9999999);
          ret=ret.multiply(bigpow);
          precision-=9999999;
        }
      }
      return ret;
    }
    
    static BigInteger FindPowerOfTen(long precision){
      if(precision<=0)return BigInteger.ONE;
      BigInteger ret;
      BigInteger bigpow;
      if(precision<=18)
        return BigIntPowersOfTen[(int)precision];
      if(precision<=36){
        ret=BigIntPowersOfTen[18];
        bigpow=BigIntPowersOfTen[((int)precision)-18];
        ret=ret.multiply(bigpow);
        return ret;
      }
      ret=BigInteger.ONE;
      bigpow=BigInteger.ZERO;
      while(precision>0){
        if(precision<=18){
          bigpow=BigIntPowersOfTen[(int)precision];
          ret=ret.multiply(bigpow);
          break;
        } else if(precision<=9999999){
          bigpow=(BigIntPowersOfTen[1]).pow((int)precision);
          ret=ret.multiply(bigpow);
          break;
        } else {
          if(bigpow.signum()==0)
            bigpow=(BigIntPowersOfTen[1]).pow(9999999);
          ret=ret.multiply(bigpow);
          precision-=9999999;
        }
      }
      return ret;
    }
    
    static BigInteger[] Round(
      BigInteger coeff,
      BigInteger exponent,
      long precision,
      RoundingMode mode
     ){
      int sign=coeff.signum();
      if(sign!=0){
        BigInteger powerForPrecision=FindPowerOfTen(precision);
        if(sign<0)coeff=coeff.negate();
        int digitsFollowingLeftmost=0; // OR of all digits following the leftmost digit
        int digitLeftmost=0;
        int exponentChange=0;
        while(coeff.compareTo(powerForPrecision)<0){
          BigInteger digit;
          BigInteger quotient;
BigInteger[] divrem=(coeff).divideAndRemainder(FindPowerOfTen(1));
quotient=divrem[0];
digit=divrem[1];
          coeff=quotient;
          int intDigit=digit.intValue();
          digitsFollowingLeftmost|=digitLeftmost;
          digitLeftmost=intDigit;
          exponentChange+=1;
          if(exponentChange>=1000){
            exponent=exponent.add(BigInteger.valueOf(exponentChange));
            exponentChange=0;
          }
        }
        if(exponentChange>0){
          exponent=exponent.add(BigInteger.valueOf(exponentChange));
        }
        boolean incremented=false;
        if(mode==RoundingMode.HalfUp){
          if(digitLeftmost>=5){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        } else if(mode==RoundingMode.Up){
          if((digitLeftmost|digitsFollowingLeftmost)!=0){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        } else if(mode==RoundingMode.HalfEven){
          if(digitLeftmost>5 || (digitLeftmost==5 && digitsFollowingLeftmost!=0)){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          } else if(digitLeftmost==5 && coeff.testBit(0)){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        } else if(mode==RoundingMode.HalfDown){
          if(digitLeftmost>5 || (digitLeftmost==5 && digitsFollowingLeftmost!=0)){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        } else if(mode==RoundingMode.Ceiling){
          if((digitLeftmost|digitsFollowingLeftmost)!=0 && sign>0){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        } else if(mode==RoundingMode.Floor){
          if((digitLeftmost|digitsFollowingLeftmost)!=0 && sign<0){
            coeff=coeff.add(BigInteger.ONE);
            incremented=true;
          }
        }
        if(incremented && coeff.compareTo(powerForPrecision)>=0){
          coeff=coeff.divide(FindPowerOfTen(1));
          exponent=exponent.add(BigInteger.ONE);
        }
      }
      if(sign<0)coeff=coeff.negate();
      return new BigInteger[]{ coeff, exponent };
    }
    
    /**
     * Gets the lesser value between two DecimalFraction values.
     */
    public DecimalFraction Min(DecimalFraction a, DecimalFraction b) {
      if (a == null) throw new NullPointerException("a");
      if (b == null) throw new NullPointerException("b");
      return a.compareTo(b) > 0 ? b : a;
    }

    /**
     * Finds the sum of this object and another decimal fraction. The result's
     * exponent is set to the lower of the exponents of the two operands.
     */
    public DecimalFraction Add(DecimalFraction decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa.add(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.add(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.add(this.mantissa), exponent);
      }
    }

    /**
     * Finds the difference between this object and another decimal fraction.
     * The result's exponent is set to the lower of the exponents of the two
     * operands.
     */
    public DecimalFraction Subtract(DecimalFraction decfrac) {
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return new DecimalFraction(
          mantissa.subtract(decfrac.mantissa), exponent);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          newmant.subtract(decfrac.mantissa), decfrac.exponent);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return new DecimalFraction(
          this.mantissa.subtract(newmant), exponent);
      }
    }

    /**
     * Multiplies two decimal fractions. The resulting scale will be the
     * sum of the scales of the two decimal fractions.
     * @param decfrac Another decimal fraction.
     * @return The product of the two decimal fractions.
     */
    public DecimalFraction Multiply(DecimalFraction decfrac) {
      BigInteger newexp = (this.exponent.add(decfrac.exponent));
      return new DecimalFraction(mantissa.multiply(decfrac.mantissa), newexp);
    }

    /**
     * Gets this value's sign: -1 if negative; 1 if positive; 0 if zero.
     */
    public int signum() {
        return mantissa.signum();
      }

    /**
     * Gets whether this object's value equals 0.
     */
    public boolean isZero() {
        return mantissa.signum()==0;
      }

    /**
     * Compares the mathematical values of two decimal fractions. <p>This
     * method is not consistent with the Equals method because two different
     * decimal fractions with the same mathematical value, but different
     * exponents, will compare as equal.</p>
     * @param other Another decimal fraction.
     * @return Less than 0 if this value is less than the other value, or greater
     * than 0 if this value is greater than the other value or if "other" is
     * null, or 0 if both values are equal.
     */
    public int compareTo(DecimalFraction decfrac) {
      if (decfrac == null) return 1;
      int s = this.signum();
      int ds = decfrac.signum();
      if (s != ds) return (s < ds) ? -1 : 1;
      int expcmp = exponent.compareTo(decfrac.exponent);
      if (expcmp == 0) {
        return mantissa.compareTo(decfrac.mantissa);
      } else if (expcmp > 0) {
        BigInteger newmant = RescaleByExponentDiff(
          mantissa, exponent, decfrac.exponent);
        return newmant.compareTo(decfrac.mantissa);
      } else {
        BigInteger newmant = RescaleByExponentDiff(
          decfrac.mantissa, exponent, decfrac.exponent);
        return this.mantissa.compareTo(newmant);
      }
    }

    private boolean AppendString(StringBuilder builder, char c, FastInteger count) {
      if (count.compareTo(Integer.MAX_VALUE) > 0 || count.signum()<0) {
        throw new UnsupportedOperationException();
      }
      int icount = count.AsInt32();
      for (int i = icount - 1; i >= 0; i--) {
        builder.append(c);
      }
      return true;
    }


    private String ToStringInternal(int mode) {
      // Using Java's rules for converting DecimalFraction
      // values to a String
      String mantissaString=this.mantissa.toString();
      FastInteger scale=new FastInteger(this.exponent).Negate();
      boolean iszero = (this.mantissa.signum()==0);
      if (mode == 2 && iszero && scale.signum() < 0) {
        // special case for zero in plain
        return mantissaString;
      }
      FastInteger adjustedExponent=new FastInteger(this.exponent);
      FastInteger sbLength=new FastInteger(mantissaString.length());
      int negaPos=0;
      if (mantissaString.charAt(0) == '-') {
        sbLength.Add(-1);
        negaPos=1;
      }
      adjustedExponent.Add(sbLength).Add(-1);
      FastInteger decimalPointAdjust = new FastInteger(1);
      FastInteger threshold = new FastInteger(-6);
      if (mode == 1) { // engineering String adjustments
        FastInteger newExponent=new FastInteger(adjustedExponent);
        boolean adjExponentNegative = (adjustedExponent.signum() < 0);
        int intphase = new FastInteger(adjustedExponent).Abs().Mod(3).AsInt32();
        if (iszero && (adjustedExponent.compareTo(threshold) < 0 ||
                       scale.signum() < 0)) {
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
      if (mode == 2 || ((adjustedExponent.compareTo(threshold) >= 0 &&
                         scale.signum() >= 0))) {
        if (scale.signum() > 0) {
          FastInteger decimalPoint = new FastInteger(scale).Negate().Add(negaPos).Add(sbLength);
          int cmp = decimalPoint.compareTo(negaPos);
          StringBuilder builder = new StringBuilder();
          if (cmp < 0) {
            builder.append(mantissaString,0,(0)+(negaPos));
            builder.append("0.");
            AppendString(builder, '0', new FastInteger(negaPos).Subtract(decimalPoint));
            builder.append(mantissaString,negaPos,(negaPos)+(mantissaString.length()-negaPos));
          } else if (cmp == 0) {
            if(!decimalPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt=decimalPoint.AsInt32();
            if(tmpInt<0)tmpInt=0;
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append("0.");
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length()-tmpInt));
          } else if (decimalPoint.compareTo(new FastInteger(negaPos).Add(mantissaString.length())) > 0) {
            FastInteger insertionPoint=new FastInteger(negaPos).Add(sbLength);
            if(!insertionPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt=insertionPoint.AsInt32();
            if(tmpInt<0)tmpInt=0;
            builder.append(mantissaString,0,(0)+(tmpInt));
            AppendString(builder, '0',
                         new FastInteger(decimalPoint).Subtract(builder.length()));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length()-tmpInt));
          } else {
            if(!decimalPoint.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt=decimalPoint.AsInt32();
            if(tmpInt<0)tmpInt=0;
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length()-tmpInt));
          }
          return builder.toString();
        } else if (mode == 2 && scale.signum() < 0) {
          FastInteger negscale = new FastInteger(scale).Negate();
          StringBuilder builder = new StringBuilder();
          builder.append(mantissaString);
          AppendString(builder, '0', negscale);
          return builder.toString();
        } else {
          return mantissaString;
        }
      } else {
        StringBuilder builder=null;
        if (mode == 1 && iszero && decimalPointAdjust.compareTo(1) > 0) {
          builder=new StringBuilder();
          builder.append(mantissaString);
          builder.append('.');
          AppendString(builder, '0', new FastInteger(decimalPointAdjust).Add(-1));
        } else {
          FastInteger tmp = new FastInteger(negaPos).Add(decimalPointAdjust);
          int cmp = tmp.compareTo(mantissaString.length());
          if (cmp > 0) {
            tmp.Subtract(mantissaString.length());
            builder=new StringBuilder();
            builder.append(mantissaString);
            AppendString(builder, '0', tmp);
          } else if (cmp < 0) {
            // Insert a decimal point at the right place
            if(!tmp.CanFitInInt32())
              throw new UnsupportedOperationException();
            int tmpInt=tmp.AsInt32();
            if(tmp.signum()<0)tmpInt=0;
            builder=new StringBuilder();
            builder.append(mantissaString,0,(0)+(tmpInt));
            builder.append('.');
            builder.append(mantissaString,tmpInt,(tmpInt)+(mantissaString.length()-tmpInt));
          } else if (adjustedExponent.signum()==0) {
            return mantissaString;
          } else {
            builder=new StringBuilder();
            builder.append(mantissaString);
          }
        }
        if (adjustedExponent.signum()!=0) {
          builder.append(adjustedExponent.signum() < 0 ? "E-" : "E+");
          adjustedExponent.Abs();
          StringBuilder builderReversed=new StringBuilder();
          while (adjustedExponent.signum()!=0) {
            int digit = new FastInteger(adjustedExponent).Mod(10).AsInt32();
            // Each digit is retrieved from right to left
            builderReversed.append((char)('0'+digit));
            adjustedExponent.Divide(10);
          }
          int count=builderReversed.length();
          for(int i=0;i<count;i++){
            builder.append(builderReversed.charAt(count-1-i));
          }
        }
        return builder.toString();
      }
    }
    
    private static BigInteger[] BigIntPowersOfTen=new BigInteger[]{
      BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000), BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000), BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000),
      BigInteger.valueOf(10000000000L), BigInteger.valueOf(100000000000L), BigInteger.valueOf(1000000000000L), BigInteger.valueOf(10000000000000L),
      BigInteger.valueOf(100000000000000L), BigInteger.valueOf(1000000000000000L), BigInteger.valueOf(10000000000000000L),
      BigInteger.valueOf(100000000000000000L), BigInteger.valueOf(1000000000000000000L)
    };
    
    private static BigInteger[] BigIntPowersOfFive=new BigInteger[]{
      BigInteger.ONE, BigInteger.valueOf(5), BigInteger.valueOf(25), BigInteger.valueOf(125), BigInteger.valueOf(625), BigInteger.valueOf(3125), BigInteger.valueOf(15625), BigInteger.valueOf(78125), BigInteger.valueOf(390625),
      BigInteger.valueOf(1953125), BigInteger.valueOf(9765625), BigInteger.valueOf(48828125), BigInteger.valueOf(244140625), BigInteger.valueOf(1220703125),
      BigInteger.valueOf(6103515625L), BigInteger.valueOf(30517578125L), BigInteger.valueOf(152587890625L), BigInteger.valueOf(762939453125L),
      BigInteger.valueOf(3814697265625L), BigInteger.valueOf(19073486328125L), BigInteger.valueOf(95367431640625L),
      BigInteger.valueOf(476837158203125L), BigInteger.valueOf(2384185791015625L), BigInteger.valueOf(11920928955078125L),
      BigInteger.valueOf(59604644775390625L), BigInteger.valueOf(298023223876953125L), BigInteger.valueOf(1490116119384765625L),
      BigInteger.valueOf(7450580596923828125L)
    };

    
    /**
     * Converts this value to an arbitrary-precision integer. Any fractional
     * part in this value will be discarded when converting to a big integer.
     */
    public BigInteger ToBigInteger() {
      int sign=this.getExponent().signum();
      if (sign==0) {
        return this.getMantissa();
      } else if (sign>0) {
        BigInteger bigmantissa = this.getMantissa();
        bigmantissa=bigmantissa.multiply(FindPowerOfTen(this.getExponent()));
        return bigmantissa;
      } else {
        BigInteger bigmantissa = this.getMantissa();
        BigInteger bigexponent=this.getExponent();
        bigexponent=bigexponent.negate();
        bigmantissa=bigmantissa.divide(FindPowerOfTen(bigexponent));
        return bigmantissa;
      }
    }

    /**
     * Converts this value to a 32-bit floating-point number. The half-up
     * rounding mode is used.
     * @return The closest 32-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 32-bit floating point number.
     */
    public float ToSingle() {
      return BigFloat.FromDecimalFraction(this).ToSingle();
    }

    /**
     * Converts this value to a 64-bit floating-point number. The half-up
     * rounding mode is used.
     * @return The closest 64-bit floating-point number to this value.
     * The return value can be positive infinity or negative infinity if
     * this value exceeds the range of a 64-bit floating point number.
     */
    public double ToDouble() {
      return BigFloat.FromDecimalFraction(this).ToDouble();
    }

    /**
     * Creates a decimal fraction from a 32-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param dbl A 32-bit floating-point number.
     * @return A decimal fraction with the same value as "flt".
     * @throws ArithmeticException "flt" is infinity or not-a-number.
     */
    public static DecimalFraction FromSingle(float flt) {
      int value = Float.floatToRawIntBits(flt);
      int fpExponent = (int)((value >> 23) & 0xFF);
      if (fpExponent == 255)
        throw new ArithmeticException("Value is infinity or NaN");
      int fpMantissa = value & 0x7FFFFF;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1 << 23);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 150;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      boolean neg = ((value >> 31) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.shiftLeft(fpExponent);
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.multiply(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }

    /**
     * Creates a decimal fraction from a 64-bit floating-point number.
     * This method computes the exact value of the floating point number,
     * not an approximation, as is often the case by converting the number
     * to a string.
     * @param dbl A 64-bit floating-point number.
     * @return A decimal fraction with the same value as "dbl"
     * @throws ArithmeticException "dbl" is infinity or not-a-number.
     */
    public static DecimalFraction FromDouble(double dbl) {
      long value = Double.doubleToRawLongBits(dbl);
      int fpExponent = (int)((value >> 52) & 0x7ffL);
      if (fpExponent == 2047)
        throw new ArithmeticException("Value is infinity or NaN");
      long fpMantissa = value & 0xFFFFFFFFFFFFFL;
      if (fpExponent == 0) fpExponent++;
      else fpMantissa |= (1L << 52);
      if (fpMantissa == 0) return new DecimalFraction(0);
      fpExponent -= 1075;
      while ((fpMantissa & 1) == 0) {
        fpExponent++;
        fpMantissa >>= 1;
      }
      boolean neg = ((value >> 63) != 0);
      if (fpExponent == 0) {
        if (neg) fpMantissa = -fpMantissa;
        return new DecimalFraction(fpMantissa);
      } else if (fpExponent > 0) {
        // Value is an integer
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.shiftLeft(fpExponent);
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Value has a fractional part
        BigInteger bigmantissa = BigInteger.valueOf(fpMantissa);
        bigmantissa=bigmantissa.multiply(FindPowerOfFive(-fpExponent));
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa, fpExponent);
      }
    }
    
    /**
     * Creates a decimal fraction from an arbitrary-precision binary floating-point
     * number.
     * @param bigfloat A bigfloat.
     */
    public static DecimalFraction FromBigFloat(BigFloat bigfloat) {
      BigInteger bigintExp = bigfloat.getExponent();
      BigInteger bigintMant = bigfloat.getMantissa();
      if (bigintExp.signum()==0) {
        // Integer
        return new DecimalFraction(bigintMant);
      } else if (bigintExp.signum()>0) {
        // Scaled integer
        FastInteger intcurexp=new FastInteger(bigintExp);
        BigInteger bigmantissa = bigintMant;
        boolean neg = (bigmantissa.signum() < 0);
        if (neg) bigmantissa=(bigmantissa).negate();
        while (intcurexp.signum() > 0) {
          int shift = 512;
          if (intcurexp.compareTo(512) < 0) {
            shift = intcurexp.AsInt32();
          }
          bigmantissa=bigmantissa.shiftLeft(shift);
          intcurexp.Add(-shift);
        }
        if (neg) bigmantissa=(bigmantissa).negate();
        return new DecimalFraction(bigmantissa);
      } else {
        // Fractional number
        BigInteger bigmantissa = bigintMant;
        BigInteger negbigintExp=(bigintExp).negate();
        bigmantissa=bigmantissa.multiply(FindPowerOfFive(negbigintExp));
        return new DecimalFraction(bigmantissa, bigintExp);
      }
    }

    /*
    public DecimalFraction MovePointLeft(BigInteger steps) {
      if(steps.signum()==0)return this;
      return new DecimalFraction(this.getMantissa(),this.getExponent().subtract(steps));
    }
    
    public DecimalFraction MovePointRight(BigInteger steps) {
      if(steps.signum()==0)return this;
      return new DecimalFraction(this.getMantissa(),this.getExponent().add(steps));
    }

    internal DecimalFraction Rescale(BigInteger scale)
    {
      throw new UnsupportedOperationException();
    }
 
    internal DecimalFraction RoundToIntegralValue(BigInteger scale)
    {
      return Rescale(BigInteger.ZERO);
    }
    internal DecimalFraction Normalize()
    {
      if(this.getMantissa().signum()==0)
        return new DecimalFraction(0);
      BigInteger mant=this.getMantissa();
      BigInteger exp=this.getExponent();
      boolean changed=false;
      while((mant.remainder(BigInteger.TEN))==0){
        mant=mant.divide(BigInteger.TEN);
        exp=exp.add(BigInteger.ONE);
        changed=true;
      }
      if(!changed)return this;
      return new DecimalFraction(mant,exp);
    }
     */

    /**
     * Converts this value to a string. The format of the return value is exactly
     * the same as that of the java.math.BigDecimal.toString() method.
     */
    @Override public String toString() {
      return ToStringInternal(0);
    }


    /**
     * Same as toString(), except that when an exponent is used it will be
     * a multiple of 3. The format of the return value follows the format of
     * the java.math.BigDecimal.toEngineeringString() method.
     */
    public String ToEngineeringString() {
      return ToStringInternal(1);
    }

    /**
     * Converts this value to a string, but without an exponent part. The
     * format of the return value follows the format of the java.math.BigDecimal.toPlainString()
     * method.
     */
    public String ToPlainString() {
      return ToStringInternal(2);
    }
  }
