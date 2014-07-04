package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  final class DecimalUtility {
private DecimalUtility() {
}
    private static BigInteger[] valueBigIntPowersOfTen = {
      BigInteger.ONE, BigInteger.TEN, BigInteger.valueOf(100), BigInteger.valueOf(1000),
      BigInteger.valueOf(10000), BigInteger.valueOf(100000), BigInteger.valueOf(1000000),
      BigInteger.valueOf(10000000), BigInteger.valueOf(100000000), BigInteger.valueOf(1000000000),
      BigInteger.valueOf(10000000000L), BigInteger.valueOf(100000000000L),
      BigInteger.valueOf(1000000000000L), BigInteger.valueOf(10000000000000L),
      BigInteger.valueOf(100000000000000L), BigInteger.valueOf(1000000000000000L),
      BigInteger.valueOf(10000000000000000L),
      BigInteger.valueOf(100000000000000000L), BigInteger.valueOf(1000000000000000000L)
    };

    private static BigInteger[] valueBigIntPowersOfFive = {
      BigInteger.ONE, BigInteger.valueOf(5), BigInteger.valueOf(25), BigInteger.valueOf(125),
      BigInteger.valueOf(625), BigInteger.valueOf(3125), BigInteger.valueOf(15625),
      BigInteger.valueOf(78125), BigInteger.valueOf(390625),
      BigInteger.valueOf(1953125), BigInteger.valueOf(9765625), BigInteger.valueOf(48828125),
      BigInteger.valueOf(244140625), BigInteger.valueOf(1220703125),
      BigInteger.valueOf(6103515625L), BigInteger.valueOf(30517578125L),
      BigInteger.valueOf(152587890625L), BigInteger.valueOf(762939453125L),
      BigInteger.valueOf(3814697265625L), BigInteger.valueOf(19073486328125L),
      BigInteger.valueOf(95367431640625L),
      BigInteger.valueOf(476837158203125L), BigInteger.valueOf(2384185791015625L),
      BigInteger.valueOf(11920928955078125L),
      BigInteger.valueOf(59604644775390625L), BigInteger.valueOf(298023223876953125L),
      BigInteger.valueOf(1490116119384765625L),
      BigInteger.valueOf(7450580596923828125L)
    };

    static int ShiftLeftOne(int[] arr) {
      {
        int carry = 0;
        for (int i = 0; i < arr.length; ++i) {
          int item = arr[i];
          arr[i] = (int)(arr[i] << 1) | (int)carry;
          carry = ((item >> 31) != 0) ? 1 : 0;
        }
        return carry;
      }
    }

    private static int CountTrailingZeros(int numberValue) {
      if (numberValue == 0) {
        return 32;
      }
      int i = 0;
      {
        if ((numberValue << 16) == 0) {
          numberValue >>= 16;
          i += 16;
        }
        if ((numberValue << 24) == 0) {
          numberValue >>= 8;
          i += 8;
        }
        if ((numberValue << 28) == 0) {
          numberValue >>= 4;
          i += 4;
        }
        if ((numberValue << 30) == 0) {
          numberValue >>= 2;
          i += 2;
        }
        if ((numberValue << 31) == 0) {
          ++i;
        }
      }
      return i;
    }

    static int BitPrecisionInt(int numberValue) {
      if (numberValue == 0) {
        return 0;
      }
      int i = 32;
      {
        if ((numberValue >> 16) == 0) {
          numberValue <<= 16;
          i -= 8;
        }
        if ((numberValue >> 24) == 0) {
          numberValue <<= 8;
          i -= 8;
        }
        if ((numberValue >> 28) == 0) {
          numberValue <<= 4;
          i -= 4;
        }
        if ((numberValue >> 30) == 0) {
          numberValue <<= 2;
          i -= 2;
        }
        if ((numberValue >> 31) == 0) {
          --i;
        }
      }
      return i;
    }

    static int ShiftAwayTrailingZerosTwoElements(int[] arr) {
      int a0 = arr[0];
      int a1 = arr[1];
      int tz = CountTrailingZeros(a0);
      if (tz == 0) {
        return 0;
      }
      {
        if (tz < 32) {
          int carry = a1 << (32 - tz);
          arr[0] = (int)((a0 >> tz) & (0x7fffffff >> (tz - 1))) | (int)carry;
          arr[1] = (a1 >> tz) & (0x7fffffff >> (tz - 1));
          return tz;
        }
        tz = CountTrailingZeros(a1);
        if (tz == 32) {
          arr[0] = 0;
        } else if (tz > 0) {
          arr[0] = (a1 >> tz) & (0x7fffffff >> (tz - 1));
        } else {
          arr[0] = a1;
        }
        arr[1] = 0;
        return 32 + tz;
      }
    }

    private static BigInteger valueBigShiftIteration = BigInteger.valueOf(1000000);

    static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      if (val.signum() == 0) {
        return val;
      }
      while (bigShift.compareTo(valueBigShiftIteration) > 0) {
        val = val.shiftLeft(1000000);
        bigShift = bigShift.subtract(valueBigShiftIteration);
      }
      int lastshift = bigShift.intValue();
      val = val.shiftLeft(lastshift);
      return val;
    }

    static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      if (val.signum() == 0) {
        return val;
      }
      while (shift > 1000000) {
        val = val.shiftLeft(1000000);
        shift -= 1000000;
      }
      int lastshift = (int)shift;
      val = val.shiftLeft(lastshift);
      return val;
    }

    static boolean HasBitSet(int[] arr, int bit) {
      return (bit >> 5) < arr.length && (arr[bit >> 5] & (1 << (bit & 31))) !=
      0;
    }

    private static final class PowerCache {
      private static final int MaxSize = 64;
      private BigInteger[] outputs;
      private BigInteger[] inputs;
      private int[] inputsInts;

      public PowerCache () {
        this.outputs = new BigInteger[MaxSize];
        this.inputs = new BigInteger[MaxSize];
        this.inputsInts = new int[MaxSize];
      }

      private int size;

      public BigInteger[] FindCachedPowerOrSmaller(BigInteger bi) {
        BigInteger[] ret = null;
        BigInteger minValue = null;
        synchronized (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (this.inputs[i].compareTo(bi) <= 0 && (minValue == null ||
            this.inputs[i].compareTo(minValue) >= 0)) {
   // System.out.println("Have cached power (" + inputs[i] + "," + bi + ") ");
              ret = new BigInteger[2];
              ret[0] = this.inputs[i];
              ret[1] = this.outputs[i];
              minValue = this.inputs[i];
            }
          }
        }
        return ret;
      }

      public BigInteger GetCachedPower(BigInteger bi) {
        synchronized (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (bi.equals(this.inputs[i])) {
              if (i != 0) {
                BigInteger tmp;
                // Move to head of cache if it isn't already
                tmp = this.inputs[i]; this.inputs[i] = this.inputs[0];
                this.inputs[0] = tmp;
                int tmpi = this.inputsInts[i]; this.inputsInts[i] =
                this.inputsInts[0]; this.inputsInts[0] = tmpi;
                tmp = this.outputs[i]; this.outputs[i] = this.outputs[0];
                this.outputs[0] = tmp;
                // Move formerly newest to next newest
                if (i != 1) {
                  tmp = this.inputs[i]; this.inputs[i] = this.inputs[1];
                  this.inputs[1] = tmp;
                  tmpi = this.inputsInts[i]; this.inputsInts[i] =
                  this.inputsInts[1]; this.inputsInts[1] = tmpi;
                  tmp = this.outputs[i]; this.outputs[i] = this.outputs[1];
                  this.outputs[1] = tmp;
                }
              }
              return this.outputs[0];
            }
          }
        }
        return null;
      }

      public BigInteger GetCachedPowerInt(int bi) {
        synchronized (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (this.inputsInts[i] >= 0 && this.inputsInts[i] == bi) {
              if (i != 0) {
                BigInteger tmp;
                // Move to head of cache if it isn't already
                tmp = this.inputs[i]; this.inputs[i] = this.inputs[0];
                this.inputs[0] = tmp;
                int tmpi = this.inputsInts[i]; this.inputsInts[i] =
                this.inputsInts[0]; this.inputsInts[0] = tmpi;
                tmp = this.outputs[i]; this.outputs[i] = this.outputs[0];
                this.outputs[0] = tmp;
                // Move formerly newest to next newest
                if (i != 1) {
                  tmp = this.inputs[i]; this.inputs[i] = this.inputs[1];
                  this.inputs[1] = tmp;
                  tmpi = this.inputsInts[i]; this.inputsInts[i] =
                  this.inputsInts[1]; this.inputsInts[1] = tmpi;
                  tmp = this.outputs[i]; this.outputs[i] = this.outputs[1];
                  this.outputs[1] = tmp;
                }
              }
              return this.outputs[0];
            }
          }
        }
        return null;
      }

      public void AddPower(BigInteger input, BigInteger output) {
        synchronized (this.outputs) {
          if (this.size < MaxSize) {
            // Shift newer entries down
            for (int i = this.size; i > 0; --i) {
              this.inputs[i] = this.inputs[i - 1];
              this.inputsInts[i] = this.inputsInts[i - 1];
              this.outputs[i] = this.outputs[i - 1];
            }
            this.inputs[0] = input;
            this.inputsInts[0] = input.canFitInInt() ? input.intValue() : -1;
            this.outputs[0] = output;
            ++this.size;
          } else {
            // Shift newer entries down
            for (int i = MaxSize - 1; i > 0; --i) {
              this.inputs[i] = this.inputs[i - 1];
              this.inputsInts[i] = this.inputsInts[i - 1];
              this.outputs[i] = this.outputs[i - 1];
            }
            this.inputs[0] = input;
            this.inputsInts[0] = input.canFitInInt() ? input.intValue() : -1;
            this.outputs[0] = output;
          }
        }
      }
    }

    private static PowerCache powerOfFiveCache = new
    DecimalUtility.PowerCache();

    private static PowerCache powerOfTenCache = new DecimalUtility.PowerCache();

    static BigInteger FindPowerOfFiveFromBig(BigInteger diff) {
      int sign = diff.signum();
      if (sign < 0) {
        return BigInteger.ZERO;
      }
      if (sign == 0) {
        return BigInteger.ONE;
      }
      FastInteger intcurexp = FastInteger.FromBig(diff);
      if (intcurexp.CompareToInt(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa = BigInteger.ONE;
      BigInteger bigpow;
      BigInteger origdiff = diff;
      bigpow = powerOfFiveCache.GetCachedPower(origdiff);
      if (bigpow != null) {
        return bigpow;
      }
      BigInteger[] otherPower =
      powerOfFiveCache.FindCachedPowerOrSmaller(origdiff);
      if (otherPower != null) {
        intcurexp.SubtractBig(otherPower[0]);
        bigpow = otherPower[1];
        mantissa = bigpow;
      } else {
        bigpow = BigInteger.ZERO;
      }
      while (intcurexp.signum() > 0) {
        if (intcurexp.CompareToInt(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa = mantissa.multiply(bigpow);
          break;
        }
        if (intcurexp.CompareToInt(9999999) <= 0) {
          bigpow = (FindPowerOfFive(1)).pow(intcurexp.AsInt32());
          mantissa = mantissa.multiply(bigpow);
          break;
        }
        if (bigpow.signum() == 0) {
          bigpow = (FindPowerOfFive(1)).pow(9999999);
        }
        mantissa = mantissa.multiply(bigpow);
        intcurexp.AddInt(-9999999);
      }
      powerOfFiveCache.AddPower(origdiff, mantissa);
      return mantissa;
    }

    private static BigInteger valueBigInt36 = BigInteger.valueOf(36);

    static BigInteger FindPowerOfTenFromBig(BigInteger
    bigintExponent) {
      int sign = bigintExponent.signum();
      if (sign < 0) {
        return BigInteger.ZERO;
      }
      if (sign == 0) {
        return BigInteger.ONE;
      }
      if (bigintExponent.compareTo(valueBigInt36) <= 0) {
        return FindPowerOfTen(bigintExponent.intValue());
      }
      FastInteger intcurexp = FastInteger.FromBig(bigintExponent);
      BigInteger mantissa = BigInteger.ONE;
      BigInteger bigpow = BigInteger.ZERO;
      while (intcurexp.signum() > 0) {
        if (intcurexp.CompareToInt(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa = mantissa.multiply(bigpow);
          break;
        }
        if (intcurexp.CompareToInt(9999999) <= 0) {
          int val = intcurexp.AsInt32();
          bigpow = FindPowerOfFive(val);
          bigpow = bigpow.shiftLeft(val);
          mantissa = mantissa.multiply(bigpow);
          break;
        }
        if (bigpow.signum() == 0) {
          bigpow = FindPowerOfFive(9999999);
          bigpow = bigpow.shiftLeft(9999999);
        }
        mantissa = mantissa.multiply(bigpow);
        intcurexp.AddInt(-9999999);
      }
      return mantissa;
    }

    private static BigInteger valueFivePower40 =
    (BigInteger.valueOf(95367431640625L)).multiply(BigInteger.valueOf(95367431640625L));

    static BigInteger FindPowerOfFive(int precision) {
      if (precision < 0) {
        return BigInteger.ZERO;
      }
      if (precision == 0) {
        return BigInteger.ONE;
      }
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 27) {
        return valueBigIntPowersOfFive[(int)precision];
      }
      if (precision == 40) {
        return valueFivePower40;
      }
      int startPrecision = precision;
      bigpow = powerOfFiveCache.GetCachedPowerInt(precision);
      if (bigpow != null) {
        return bigpow;
      }
      BigInteger origPrecision = BigInteger.valueOf(precision);
      if (precision <= 54) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfFive[(int)(precision >> 1)];
          ret = ret.multiply(ret);
          powerOfFiveCache.AddPower(origPrecision, ret);
          return ret;
        }
        ret = valueBigIntPowersOfFive[27];
        bigpow = valueBigIntPowersOfFive[((int)precision) - 27];
        ret = ret.multiply(bigpow);
        powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
      }
      if (precision > 40 && precision <= 94) {
        ret = valueFivePower40;
        bigpow = FindPowerOfFive(precision - 40);
        ret = ret.multiply(bigpow);
        powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
      }
      BigInteger[] otherPower;
      boolean first = true;
      bigpow = BigInteger.ZERO;
      while (true) {
        otherPower =
        powerOfFiveCache.FindCachedPowerOrSmaller(BigInteger.valueOf(precision));
        if (otherPower != null) {
          BigInteger otherPower0 = otherPower[0];
          BigInteger otherPower1 = otherPower[1];
          precision -= otherPower0.intValue();
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow = bigpow.multiply(otherPower1);
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : BigInteger.ONE;
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = valueBigIntPowersOfFive[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret = ret.multiply(bigpow);
          }
          first = false;
          break;
        }
        if (precision <= 9999999) {
          // System.out.println("calcing pow for "+precision);
          bigpow = (valueBigIntPowersOfFive[1]).pow(precision);
          if (precision != startPrecision) {
            BigInteger bigprec = BigInteger.valueOf(precision);
            powerOfFiveCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret = ret.multiply(bigpow);
          }
          first = false;
          break;
        }
        if (bigpow.signum() == 0) {
          bigpow = FindPowerOfFive(9999999);
        }
        if (first) {
          ret = bigpow;
        } else {
          ret = ret.multiply(bigpow);
        }
        first = false;
        precision -= 9999999;
      }
      powerOfFiveCache.AddPower(origPrecision, ret);
      return ret;
    }

    static BigInteger FindPowerOfTen(int precision) {
      if (precision < 0) {
        return BigInteger.ZERO;
      }
      if (precision == 0) {
        return BigInteger.ONE;
      }
      BigInteger bigpow;
      BigInteger ret;
      if (precision <= 18) {
        return valueBigIntPowersOfTen[(int)precision];
      }
      int startPrecision = precision;
      bigpow = powerOfTenCache.GetCachedPowerInt(precision);
      if (bigpow != null) {
        return bigpow;
      }
      BigInteger origPrecision = BigInteger.valueOf(precision);
      if (precision <= 27) {
        int prec = (int)precision;
        ret = valueBigIntPowersOfFive[prec];
        ret = ret.shiftLeft(prec);
        powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
      }
      if (precision <= 36) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfTen[(int)(precision >> 1)];
          ret = ret.multiply(ret);
          powerOfTenCache.AddPower(origPrecision, ret);
          return ret;
        }
        ret = valueBigIntPowersOfTen[18];
        bigpow = valueBigIntPowersOfTen[((int)precision) - 18];
        ret = ret.multiply(bigpow);
        powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
      }
      BigInteger[] otherPower;
      boolean first = true;
      bigpow = BigInteger.ZERO;
      while (true) {
        otherPower =
        powerOfTenCache.FindCachedPowerOrSmaller(BigInteger.valueOf(precision));
        if (otherPower != null) {
          BigInteger otherPower0 = otherPower[0];
          BigInteger otherPower1 = otherPower[1];
          precision -= otherPower0.intValue();
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow = bigpow.multiply(otherPower1);
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : BigInteger.ONE;
      while (precision > 0) {
        if (precision <= 18) {
          bigpow = valueBigIntPowersOfTen[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret = ret.multiply(bigpow);
          }
          first = false;
          break;
        }
        if (precision <= 9999999) {
          // System.out.println("calcing pow for "+precision);
          bigpow = FindPowerOfFive(precision);
          bigpow = bigpow.shiftLeft(precision);
          if (precision != startPrecision) {
            BigInteger bigprec = BigInteger.valueOf(precision);
            powerOfTenCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret = ret.multiply(bigpow);
          }
          first = false;
          break;
        }
        if (bigpow.signum() == 0) {
          bigpow = FindPowerOfTen(9999999);
        }
        if (first) {
          ret = bigpow;
        } else {
          ret = ret.multiply(bigpow);
        }
        first = false;
        precision -= 9999999;
      }
      powerOfTenCache.AddPower(origPrecision, ret);
      return ret;
    }

    public static BigInteger ReduceTrailingZeros(
      BigInteger bigmant,
      FastInteger exponentMutable,
      int radix,
      FastInteger digits,
      FastInteger precision,
      FastInteger idealExp) {
      if (bigmant.signum() == 0) {
        exponentMutable.SetInt(0);
        return bigmant;
      }
      BigInteger bigradix = BigInteger.valueOf(radix);
      int bitToTest = 0;
      FastInteger bitsToShift = new FastInteger(0);
      while (bigmant.signum() != 0) {
        if (precision != null && digits.compareTo(precision) == 0) {
          break;
        }
        if (idealExp != null && exponentMutable.compareTo(idealExp) == 0) {
          break;
        }
        if (radix == 2) {
          if (bitToTest < Integer.MAX_VALUE) {
            if (bigmant.testBit(bitToTest)) {
              break;
            }
            ++bitToTest;
            bitsToShift.Increment();
          } else {
            if (bigmant.testBit(0)) {
              break;
            }
            bigmant = bigmant.shiftRight(1);
          }
        } else {
          BigInteger bigrem;
          BigInteger bigquo;
{
BigInteger[] divrem = (bigmant).divideAndRemainder(bigradix);
bigquo = divrem[0];
bigrem = divrem[1]; }
          if (bigrem.signum() != 0) {
            break;
          }
          bigmant = bigquo;
        }
        exponentMutable.Increment();
        if (digits != null) {
          digits.Decrement();
        }
      }
      if (radix == 2 && !bitsToShift.isValueZero()) {
        while (bitsToShift.CompareToInt(1000000) > 0) {
          bigmant = bigmant.shiftRight(1000000);
          bitsToShift.SubtractInt(1000000);
        }
        int tmpshift = bitsToShift.AsInt32();
        bigmant = bigmant.shiftRight(tmpshift);
      }
      return bigmant;
    }
  }
