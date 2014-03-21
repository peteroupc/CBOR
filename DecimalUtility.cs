/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;

namespace PeterO
{
    /// <summary>Description of DecimalUtility.</summary>
  internal static class DecimalUtility
  {
    private static BigInteger[] valueBigIntPowersOfTen = new BigInteger[] {
      BigInteger.One, (BigInteger)10, (BigInteger)100, (BigInteger)1000, (BigInteger)10000, (BigInteger)100000, (BigInteger)1000000, (BigInteger)10000000, (BigInteger)100000000, (BigInteger)1000000000,
      (BigInteger)10000000000L, (BigInteger)100000000000L, (BigInteger)1000000000000L, (BigInteger)10000000000000L,
      (BigInteger)100000000000000L, (BigInteger)1000000000000000L, (BigInteger)10000000000000000L,
      (BigInteger)100000000000000000L, (BigInteger)1000000000000000000L
    };

    private static BigInteger[] valueBigIntPowersOfFive = new BigInteger[] {
      BigInteger.One, (BigInteger)5, (BigInteger)25, (BigInteger)125, (BigInteger)625, (BigInteger)3125, (BigInteger)15625, (BigInteger)78125, (BigInteger)390625,
      (BigInteger)1953125, (BigInteger)9765625, (BigInteger)48828125, (BigInteger)244140625, (BigInteger)1220703125,
      (BigInteger)6103515625L, (BigInteger)30517578125L, (BigInteger)152587890625L, (BigInteger)762939453125L,
      (BigInteger)3814697265625L, (BigInteger)19073486328125L, (BigInteger)95367431640625L,
      (BigInteger)476837158203125L, (BigInteger)2384185791015625L, (BigInteger)11920928955078125L,
      (BigInteger)59604644775390625L, (BigInteger)298023223876953125L, (BigInteger)1490116119384765625L,
      (BigInteger)7450580596923828125L
    };

    internal static int ShiftLeftOne(int[] arr) {
      unchecked {
        int carry = 0;
        for (int i = 0; i < arr.Length; ++i) {
          int item = arr[i];
          arr[i] = (int)(arr[i] << 1) | (int)carry;
          carry = ((item >> 31) != 0) ? 1 : 0;
        }
        return carry;
      }
    }

    internal static int CountTrailingZeros(int numberValue) {
      if (numberValue == 0) {
        return 32;
      }
      int i = 0;
      unchecked {
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

    internal static int BitPrecisionInt(int numberValue) {
      if (numberValue == 0) {
        return 0;
      }
      int i = 32;
      unchecked {
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

    internal static int ShiftAwayTrailingZerosTwoElements(int[] arr) {
      int a0 = arr[0];
      int a1 = arr[1];
      int tz = CountTrailingZeros(a0);
      if (tz == 0) {
        return 0;
      }
      unchecked {
        if (tz < 32) {
          int carry = a1 << (32 - tz);
          arr[0] = (int)((a0 >> tz) & (0x7fffffff >> (tz - 1))) | (int)carry;
          arr[1] = (a1 >> tz) & (0x7fffffff >> (tz - 1));
          return tz;
        } else {
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
    }

    private static BigInteger valueBigShiftIteration = (BigInteger)1000000;

    internal static BigInteger ShiftLeft(BigInteger val, BigInteger bigShift) {
      #if DEBUG
      if (val == null) {
        throw new ArgumentNullException("val");
      }
      if (bigShift == null) {
        throw new ArgumentNullException("bigShift");
      }
      #endif

      if (val.IsZero) {
        return val;
      }
      while (bigShift.CompareTo(valueBigShiftIteration) > 0) {
        val <<= 1000000;
        bigShift -= (BigInteger)valueBigShiftIteration;
      }
      int lastshift = (int)bigShift;
      val <<= lastshift;
      return val;
    }

    internal static BigInteger ShiftLeftInt(BigInteger val, int shift) {
      #if DEBUG
      if (val == null) {
        throw new ArgumentNullException("val");
      }
      #endif

      if (val.IsZero) {
        return val;
      }
      while (shift > 1000000) {
        val <<= 1000000;
        shift -= 1000000;
      }
      int lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }

    internal static bool HasBitSet(int[] arr, int bit) {
      return (bit >> 5) < arr.Length && (arr[bit >> 5] & (1 << (bit & 31))) != 0;
    }

    private sealed class PowerCache {
      private const int MaxSize = 64;
      private BigInteger[] outputs;
      private BigInteger[] inputs;
      private int[] inputsInts;

      public PowerCache() {
        this.outputs = new BigInteger[MaxSize];
        this.inputs = new BigInteger[MaxSize];
        this.inputsInts = new int[MaxSize];
      }

      private int size;

      public BigInteger[] FindCachedPowerOrSmaller(BigInteger bi) {
        BigInteger[] ret = null;
        BigInteger minValue = null;
        lock (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (this.inputs[i].CompareTo(bi) <= 0 && (minValue == null || this.inputs[i].CompareTo(minValue) >= 0)) {
              // Console.WriteLine("Have cached power (" + inputs[i] + ", " + bi + ")");
              ret = new BigInteger[] { this.inputs[i], this.outputs[i] };
              minValue = this.inputs[i];
            }
          }
        }
        return ret;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='bi'>A BigInteger object. (2).</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetCachedPower(BigInteger bi) {
        lock (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (bi.Equals(this.inputs[i])) {
              if (i != 0) {
                BigInteger tmp;
                // Move to head of cache if it isn't already
                tmp = this.inputs[i]; this.inputs[i] = this.inputs[0]; this.inputs[0] = tmp;
                int tmpi = this.inputsInts[i]; this.inputsInts[i] = this.inputsInts[0]; this.inputsInts[0] = tmpi;
                tmp = this.outputs[i]; this.outputs[i] = this.outputs[0]; this.outputs[0] = tmp;
                // Move formerly newest to next newest
                if (i != 1) {
                  tmp = this.inputs[i]; this.inputs[i] = this.inputs[1]; this.inputs[1] = tmp;
                  tmpi = this.inputsInts[i]; this.inputsInts[i] = this.inputsInts[1]; this.inputsInts[1] = tmpi;
                  tmp = this.outputs[i]; this.outputs[i] = this.outputs[1]; this.outputs[1] = tmp;
                }
              }
              return this.outputs[0];
            }
          }
        }
        return null;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='bi'>A 32-bit signed integer.</param>
    /// <returns>A BigInteger object.</returns>
      public BigInteger GetCachedPowerInt(int bi) {
        lock (this.outputs) {
          for (int i = 0; i < this.size; ++i) {
            if (this.inputsInts[i] >= 0 && this.inputsInts[i] == bi) {
              if (i != 0) {
                BigInteger tmp;
                // Move to head of cache if it isn't already
                tmp = this.inputs[i]; this.inputs[i] = this.inputs[0]; this.inputs[0] = tmp;
                int tmpi = this.inputsInts[i]; this.inputsInts[i] = this.inputsInts[0]; this.inputsInts[0] = tmpi;
                tmp = this.outputs[i]; this.outputs[i] = this.outputs[0]; this.outputs[0] = tmp;
                // Move formerly newest to next newest
                if (i != 1) {
                  tmp = this.inputs[i]; this.inputs[i] = this.inputs[1]; this.inputs[1] = tmp;
                  tmpi = this.inputsInts[i]; this.inputsInts[i] = this.inputsInts[1]; this.inputsInts[1] = tmpi;
                  tmp = this.outputs[i]; this.outputs[i] = this.outputs[1]; this.outputs[1] = tmp;
                }
              }
              return this.outputs[0];
            }
          }
        }
        return null;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='input'>A BigInteger object.</param>
    /// <param name='output'>A BigInteger object. (2).</param>
      public void AddPower(BigInteger input, BigInteger output) {
        lock (this.outputs) {
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

    private static PowerCache powerOfFiveCache = new DecimalUtility.PowerCache();
    private static PowerCache powerOfTenCache = new DecimalUtility.PowerCache();

    internal static BigInteger FindPowerOfFiveFromBig(BigInteger diff) {
      int sign = diff.Sign;
      if (sign < 0) {
        return BigInteger.Zero;
      }
      if (sign == 0) {
        return BigInteger.One;
      }
      FastInteger intcurexp = FastInteger.FromBig(diff);
      if (intcurexp.CompareToInt(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      BigInteger mantissa = BigInteger.One;
      BigInteger bigpow;
      BigInteger origdiff = diff;
      bigpow = powerOfFiveCache.GetCachedPower(origdiff);
      if (bigpow != null) {
        return bigpow;
      }
      BigInteger[] otherPower = powerOfFiveCache.FindCachedPowerOrSmaller(origdiff);
      if (otherPower != null) {
        intcurexp.SubtractBig(otherPower[0]);
        bigpow = otherPower[1];
        mantissa = bigpow;
      } else {
        bigpow = BigInteger.Zero;
      }
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareToInt(9999999) <= 0) {
          bigpow = BigInteger.Pow(FindPowerOfFive(1), intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else {
          if (bigpow.IsZero) {
            bigpow = BigInteger.Pow(FindPowerOfFive(1), 9999999);
          }
          mantissa *= bigpow;
          intcurexp.AddInt(-9999999);
        }
      }
      powerOfFiveCache.AddPower(origdiff, mantissa);
      return mantissa;
    }

    private static BigInteger valueBigInt36 = (BigInteger)36;

    internal static BigInteger FindPowerOfTenFromBig(BigInteger bigintExponent) {
      int sign = bigintExponent.Sign;
      if (sign < 0) {
        return BigInteger.Zero;
      }
      if (sign == 0) {
        return BigInteger.One;
      }
      if (bigintExponent.CompareTo(valueBigInt36) <= 0) {
        return FindPowerOfTen((int)bigintExponent);
      }
      FastInteger intcurexp = FastInteger.FromBig(bigintExponent);
      BigInteger mantissa = BigInteger.One;
      BigInteger bigpow = BigInteger.Zero;
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa *= (BigInteger)bigpow;
          break;
        } else if (intcurexp.CompareToInt(9999999) <= 0) {
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
          intcurexp.AddInt(-9999999);
        }
      }
      return mantissa;
    }

    private static BigInteger valueFivePower40 = ((BigInteger)95367431640625L) * (BigInteger)95367431640625L;

    internal static BigInteger FindPowerOfFive(int precision) {
      if (precision < 0) {
        return BigInteger.Zero;
      }
      if (precision == 0) {
        return BigInteger.One;
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
      BigInteger origPrecision = (BigInteger)precision;
      if (precision <= 54) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfFive[(int)(precision >> 1)];
          ret *= (BigInteger)ret;
          powerOfFiveCache.AddPower(origPrecision, ret);
          return ret;
        } else {
          ret = valueBigIntPowersOfFive[27];
          bigpow = valueBigIntPowersOfFive[((int)precision) - 27];
          ret *= (BigInteger)bigpow;
          powerOfFiveCache.AddPower(origPrecision, ret);
          return ret;
        }
      }
      if (precision > 40 && precision <= 94) {
        ret = valueFivePower40;
        bigpow = FindPowerOfFive(precision - 40);
        ret *= (BigInteger)bigpow;
        powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
      }
      BigInteger[] otherPower;
      bool first = true;
      bigpow = BigInteger.Zero;
      while (true) {
        otherPower = powerOfFiveCache.FindCachedPowerOrSmaller((BigInteger)precision);
        if (otherPower != null) {
          BigInteger otherPower0 = otherPower[0];
          BigInteger otherPower1 = otherPower[1];
          precision -= (int)otherPower0;
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow *= (BigInteger)otherPower1;
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : BigInteger.One;
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = valueBigIntPowersOfFive[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          break;
        } else if (precision <= 9999999) {
          // Console.WriteLine("calcing pow for "+precision);
          bigpow = BigInteger.Pow(valueBigIntPowersOfFive[1], precision);
          if (precision != startPrecision) {
            BigInteger bigprec = (BigInteger)precision;
            powerOfFiveCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          break;
        } else {
          if (bigpow.IsZero) {
            bigpow = FindPowerOfFive(9999999);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          precision -= 9999999;
        }
      }
      powerOfFiveCache.AddPower(origPrecision, ret);
      return ret;
    }

    internal static BigInteger FindPowerOfTen(int precision) {
      if (precision < 0) {
        return BigInteger.Zero;
      }
      if (precision == 0) {
        return BigInteger.One;
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
      BigInteger origPrecision = (BigInteger)precision;
      if (precision <= 27) {
        int prec = (int)precision;
        ret = valueBigIntPowersOfFive[prec];
        ret <<= prec;
        powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
      }
      if (precision <= 36) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfTen[(int)(precision >> 1)];
          ret *= (BigInteger)ret;
          powerOfTenCache.AddPower(origPrecision, ret);
          return ret;
        } else {
          ret = valueBigIntPowersOfTen[18];
          bigpow = valueBigIntPowersOfTen[((int)precision) - 18];
          ret *= (BigInteger)bigpow;
          powerOfTenCache.AddPower(origPrecision, ret);
          return ret;
        }
      }
      BigInteger[] otherPower;
      bool first = true;
      bigpow = BigInteger.Zero;
      while (true) {
        otherPower = powerOfTenCache.FindCachedPowerOrSmaller((BigInteger)precision);
        if (otherPower != null) {
          BigInteger otherPower0 = otherPower[0];
          BigInteger otherPower1 = otherPower[1];
          precision -= (int)otherPower0;
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow *= (BigInteger)otherPower1;
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : BigInteger.One;
      while (precision > 0) {
        if (precision <= 18) {
          bigpow = valueBigIntPowersOfTen[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          break;
        } else if (precision <= 9999999) {
          // Console.WriteLine("calcing pow for "+precision);
          bigpow = FindPowerOfFive(precision);
          bigpow <<= precision;
          if (precision != startPrecision) {
            BigInteger bigprec = (BigInteger)precision;
            powerOfTenCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          break;
        } else {
          if (bigpow.IsZero) {
            bigpow = FindPowerOfTen(9999999);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (BigInteger)bigpow;
          }
          first = false;
          precision -= 9999999;
        }
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
      #if DEBUG
      if (precision != null && digits == null) {
        throw new ArgumentException("doesn't satisfy precision==null || digits!=null");
      }
      #endif
      if (bigmant.IsZero) {
        exponentMutable.SetInt(0);
        return bigmant;
      }
      BigInteger bigradix = (BigInteger)radix;
      int bitToTest = 0;
      FastInteger bitsToShift = new FastInteger(0);
      while (!bigmant.IsZero) {
        if (precision != null && digits.CompareTo(precision) == 0) {
          break;
        }
        if (idealExp != null && exponentMutable.CompareTo(idealExp) == 0) {
          break;
        }
        if (radix == 2) {
          if (bitToTest < Int32.MaxValue) {
            if (bigmant.testBit(bitToTest)) {
              break;
            }
            ++bitToTest;
            bitsToShift.Increment();
          } else {
            if (!bigmant.IsEven) {
              break;
            }
            bigmant >>= 1;
          }
        } else {
          BigInteger bigrem;
          BigInteger bigquo = BigInteger.DivRem(bigmant, bigradix, out bigrem);
          if (!bigrem.IsZero) {
            break;
          }
          bigmant = bigquo;
        }
        exponentMutable.Increment();
        if (digits != null) {
          digits.Decrement();
        }
      }
      if (radix == 2 && !bitsToShift.IsValueZero) {
        while (bitsToShift.CompareToInt(1000000) > 0) {
          bigmant >>= 1000000;
          bitsToShift.SubtractInt(1000000);
        }
        int tmpshift = bitsToShift.AsInt32();
        bigmant >>= tmpshift;
      }
      return bigmant;
    }
  }
}
