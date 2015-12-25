/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
  internal static class DecimalUtility {
    private static readonly EInteger[] valueBigIntPowersOfTen = {
      EInteger.One, (EInteger)10, (EInteger)100, (EInteger)1000,
      (EInteger)10000, (EInteger)100000, (EInteger)1000000,
      (EInteger)10000000, (EInteger)100000000, (EInteger)1000000000,
      (EInteger)10000000000L, (EInteger)100000000000L,
      (EInteger)1000000000000L, (EInteger)10000000000000L,
      (EInteger)100000000000000L, (EInteger)1000000000000000L,
      (EInteger)10000000000000000L,
      (EInteger)100000000000000000L, (EInteger)1000000000000000000L
    };

    private static readonly EInteger[] valueBigIntPowersOfFive = {
      EInteger.One, (EInteger)5, (EInteger)25, (EInteger)125,
      (EInteger)625, (EInteger)3125, (EInteger)15625,
      (EInteger)78125, (EInteger)390625,
      (EInteger)1953125, (EInteger)9765625, (EInteger)48828125,
      (EInteger)244140625, (EInteger)1220703125,
      (EInteger)6103515625L, (EInteger)30517578125L,
      (EInteger)152587890625L, (EInteger)762939453125L,
      (EInteger)3814697265625L, (EInteger)19073486328125L,
      (EInteger)95367431640625L,
      (EInteger)476837158203125L, (EInteger)2384185791015625L,
      (EInteger)11920928955078125L,
      (EInteger)59604644775390625L, (EInteger)298023223876953125L,
      (EInteger)1490116119384765625L, (EInteger)7450580596923828125L
    };

    internal static int ShiftLeftOne(int[] arr) {
      unchecked {
        var carry = 0;
        for (var i = 0; i < arr.Length; ++i) {
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
      var i = 0;
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
      var i = 32;
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

    private static readonly EInteger valueBigShiftIteration =
      (EInteger)1000000;

    internal static EInteger ShiftLeft(EInteger val, EInteger bigShift) {
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
        bigShift -= (EInteger)valueBigShiftIteration;
      }
      var lastshift = (int)bigShift;
      val <<= lastshift;
      return val;
    }

    internal static EInteger ShiftLeftInt(EInteger val, int shift) {
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
      var lastshift = (int)shift;
      val <<= lastshift;
      return val;
    }

    internal static bool HasBitSet(int[] arr, int bit) {
      return (bit >> 5) < arr.Length && (arr[bit >> 5] & (1 << (bit & 31))) !=
      0;
    }

    private sealed class PowerCache {
      private const int MaxSize = 64;
      private readonly EInteger[] outputs;
      private readonly EInteger[] inputs;
      private readonly int[] inputsInts;

      public PowerCache() {
        this.outputs = new EInteger[MaxSize];
        this.inputs = new EInteger[MaxSize];
        this.inputsInts = new int[MaxSize];
      }

      private int size;

      public EInteger[] FindCachedPowerOrSmaller(EInteger bi) {
        EInteger[] ret = null;
        EInteger minValue = null;
        lock (this.outputs) {
          for (var i = 0; i < this.size; ++i) {
            if (this.inputs[i].CompareTo(bi) <= 0 && (minValue == null ||
            this.inputs[i].CompareTo(minValue) >= 0)) {
   // Console.WriteLine("Have cached power (" + inputs[i] + "," + bi + ") ");
              ret = new EInteger[2];
              ret[0] = this.inputs[i];
              ret[1] = this.outputs[i];
              minValue = this.inputs[i];
            }
          }
        }
        return ret;
      }

      public EInteger GetCachedPower(EInteger bi) {
        lock (this.outputs) {
          for (var i = 0; i < this.size; ++i) {
            if (bi.Equals(this.inputs[i])) {
              if (i != 0) {
                EInteger tmp;
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

      public EInteger GetCachedPowerInt(int bi) {
        lock (this.outputs) {
          for (var i = 0; i < this.size; ++i) {
            if (this.inputsInts[i] >= 0 && this.inputsInts[i] == bi) {
              if (i != 0) {
                EInteger tmp;
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

      public void AddPower(EInteger input, EInteger output) {
        lock (this.outputs) {
          if (this.size < MaxSize) {
            // Shift newer entries down
            for (int i = this.size; i > 0; --i) {
              this.inputs[i] = this.inputs[i - 1];
              this.inputsInts[i] = this.inputsInts[i - 1];
              this.outputs[i] = this.outputs[i - 1];
            }
            this.inputs[0] = input;
       this.inputsInts[0] = input.canFitInInt() ? input.intValueChecked() : -1;
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
       this.inputsInts[0] = input.canFitInInt() ? input.intValueChecked() : -1;
            this.outputs[0] = output;
          }
        }
      }
    }

    private static readonly PowerCache powerOfFiveCache = new
    DecimalUtility.PowerCache();

    private static readonly PowerCache powerOfTenCache = new
      DecimalUtility.PowerCache();

    internal static EInteger FindPowerOfFiveFromBig(EInteger diff) {
      int sign = diff.Sign;
      if (sign < 0) {
        return EInteger.Zero;
      }
      if (sign == 0) {
        return EInteger.One;
      }
      FastInteger intcurexp = FastInteger.FromBig(diff);
      if (intcurexp.CompareToInt(54) <= 0) {
        return FindPowerOfFive(intcurexp.AsInt32());
      }
      EInteger mantissa = EInteger.One;
      EInteger bigpow;
      EInteger origdiff = diff;
      bigpow = powerOfFiveCache.GetCachedPower(origdiff);
      if (bigpow != null) {
        return bigpow;
      }
      EInteger[] otherPower =
      powerOfFiveCache.FindCachedPowerOrSmaller(origdiff);
      if (otherPower != null) {
        intcurexp.SubtractBig(otherPower[0]);
        bigpow = otherPower[1];
        mantissa = bigpow;
      } else {
        bigpow = EInteger.Zero;
      }
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(27) <= 0) {
          bigpow = FindPowerOfFive(intcurexp.AsInt32());
          mantissa *= (EInteger)bigpow;
          break;
        }
        if (intcurexp.CompareToInt(9999999) <= 0) {
          bigpow = EInteger.Pow(FindPowerOfFive(1), intcurexp.AsInt32());
          mantissa *= (EInteger)bigpow;
          break;
        }
        if (bigpow.IsZero) {
          bigpow = EInteger.Pow(FindPowerOfFive(1), 9999999);
        }
        mantissa *= bigpow;
        intcurexp.AddInt(-9999999);
      }
      powerOfFiveCache.AddPower(origdiff, mantissa);
      return mantissa;
    }

    private static readonly EInteger valueBigInt36 = (EInteger)36;

    internal static EInteger FindPowerOfTenFromBig(EInteger
    bigintExponent) {
      int sign = bigintExponent.Sign;
      if (sign < 0) {
        return EInteger.Zero;
      }
      if (sign == 0) {
        return EInteger.One;
      }
      if (bigintExponent.CompareTo(valueBigInt36) <= 0) {
        return FindPowerOfTen((int)bigintExponent);
      }
      FastInteger intcurexp = FastInteger.FromBig(bigintExponent);
      EInteger mantissa = EInteger.One;
      EInteger bigpow = EInteger.Zero;
      while (intcurexp.Sign > 0) {
        if (intcurexp.CompareToInt(18) <= 0) {
          bigpow = FindPowerOfTen(intcurexp.AsInt32());
          mantissa *= (EInteger)bigpow;
          break;
        }
        if (intcurexp.CompareToInt(9999999) <= 0) {
          int val = intcurexp.AsInt32();
          bigpow = FindPowerOfFive(val);
          bigpow <<= val;
          mantissa *= (EInteger)bigpow;
          break;
        }
        if (bigpow.IsZero) {
          bigpow = FindPowerOfFive(9999999);
          bigpow <<= 9999999;
        }
        mantissa *= bigpow;
        intcurexp.AddInt(-9999999);
      }
      return mantissa;
    }

    private static readonly EInteger valueFivePower40 =
    ((EInteger)95367431640625L) * (EInteger)95367431640625L;

    internal static EInteger FindPowerOfFive(int precision) {
      if (precision < 0) {
        return EInteger.Zero;
      }
      if (precision == 0) {
        return EInteger.One;
      }
      EInteger bigpow;
      EInteger ret;
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
      var origPrecision = (EInteger)precision;
      if (precision <= 54) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfFive[(int)(precision >> 1)];
          ret *= (EInteger)ret;
          powerOfFiveCache.AddPower(origPrecision, ret);
          return ret;
        }
        ret = valueBigIntPowersOfFive[27];
        bigpow = valueBigIntPowersOfFive[((int)precision) - 27];
        ret *= (EInteger)bigpow;
        powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
      }
      if (precision > 40 && precision <= 94) {
        ret = valueFivePower40;
        bigpow = FindPowerOfFive(precision - 40);
        ret *= (EInteger)bigpow;
        powerOfFiveCache.AddPower(origPrecision, ret);
        return ret;
      }
      EInteger[] otherPower;
      var first = true;
      bigpow = EInteger.Zero;
      while (true) {
        otherPower =
        powerOfFiveCache.FindCachedPowerOrSmaller((EInteger)precision);
        if (otherPower != null) {
          EInteger otherPower0 = otherPower[0];
          EInteger otherPower1 = otherPower[1];
          precision -= (int)otherPower0;
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow *= (EInteger)otherPower1;
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : EInteger.One;
      while (precision > 0) {
        if (precision <= 27) {
          bigpow = valueBigIntPowersOfFive[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret *= (EInteger)bigpow;
          }
          first = false;
          break;
        }
        if (precision <= 9999999) {
          // Console.WriteLine("calcing pow for "+precision);
          bigpow = EInteger.Pow(valueBigIntPowersOfFive[1], precision);
          if (precision != startPrecision) {
            var bigprec = (EInteger)precision;
            powerOfFiveCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (EInteger)bigpow;
          }
          first = false;
          break;
        }
        if (bigpow.IsZero) {
          bigpow = FindPowerOfFive(9999999);
        }
        if (first) {
          ret = bigpow;
        } else {
          ret *= (EInteger)bigpow;
        }
        first = false;
        precision -= 9999999;
      }
      powerOfFiveCache.AddPower(origPrecision, ret);
      return ret;
    }

    internal static EInteger FindPowerOfTen(int precision) {
      if (precision < 0) {
        return EInteger.Zero;
      }
      if (precision == 0) {
        return EInteger.One;
      }
      EInteger bigpow;
      EInteger ret;
      if (precision <= 18) {
        return valueBigIntPowersOfTen[(int)precision];
      }
      int startPrecision = precision;
      bigpow = powerOfTenCache.GetCachedPowerInt(precision);
      if (bigpow != null) {
        return bigpow;
      }
      var origPrecision = (EInteger)precision;
      if (precision <= 27) {
        var prec = (int)precision;
        ret = valueBigIntPowersOfFive[prec];
        ret <<= prec;
        powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
      }
      if (precision <= 36) {
        if ((precision & 1) == 0) {
          ret = valueBigIntPowersOfTen[(int)(precision >> 1)];
          ret *= (EInteger)ret;
          powerOfTenCache.AddPower(origPrecision, ret);
          return ret;
        }
        ret = valueBigIntPowersOfTen[18];
        bigpow = valueBigIntPowersOfTen[((int)precision) - 18];
        ret *= (EInteger)bigpow;
        powerOfTenCache.AddPower(origPrecision, ret);
        return ret;
      }
      EInteger[] otherPower;
      var first = true;
      bigpow = EInteger.Zero;
      while (true) {
        otherPower =
        powerOfTenCache.FindCachedPowerOrSmaller((EInteger)precision);
        if (otherPower != null) {
          EInteger otherPower0 = otherPower[0];
          EInteger otherPower1 = otherPower[1];
          precision -= (int)otherPower0;
          if (first) {
            bigpow = otherPower[1];
          } else {
            bigpow *= (EInteger)otherPower1;
          }
          first = false;
        } else {
          break;
        }
      }
      ret = !first ? bigpow : EInteger.One;
      while (precision > 0) {
        if (precision <= 18) {
          bigpow = valueBigIntPowersOfTen[(int)precision];
          if (first) {
            ret = bigpow;
          } else {
            ret *= (EInteger)bigpow;
          }
          first = false;
          break;
        }
        if (precision <= 9999999) {
          // Console.WriteLine("calcing pow for "+precision);
          bigpow = FindPowerOfFive(precision);
          bigpow <<= precision;
          if (precision != startPrecision) {
            var bigprec = (EInteger)precision;
            powerOfTenCache.AddPower(bigprec, bigpow);
          }
          if (first) {
            ret = bigpow;
          } else {
            ret *= (EInteger)bigpow;
          }
          first = false;
          break;
        }
        if (bigpow.IsZero) {
          bigpow = FindPowerOfTen(9999999);
        }
        if (first) {
          ret = bigpow;
        } else {
          ret *= (EInteger)bigpow;
        }
        first = false;
        precision -= 9999999;
      }
      powerOfTenCache.AddPower(origPrecision, ret);
      return ret;
    }

    public static EInteger ReduceTrailingZeros(
      EInteger bigmant,
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
      var bigradix = (EInteger)radix;
      var bitToTest = 0;
      var bitsToShift = new FastInteger(0);
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
          EInteger bigrem;
          EInteger bigquo = EInteger.DivRem(bigmant, bigradix, out bigrem);
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
