/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace Test {
  /// <summary>Generates random objects of various kinds for purposes of
  /// testing code that uses them. The methods will not necessarily
  /// sample uniformly from all objects of a particular kind.</summary>
  public static class RandomNumerics {
    private const int MaxExclusiveStringLength = 0x2000;
    private const int MaxExclusiveShortStringLength = 50;
    private const int MaxNumberLength = 50000;
    private const int MaxShortNumberLength = 40;

    public static ERational RandomERational(IRandomGenExtended rand) {
      EInteger bigintA = RandomEInteger(rand);
      EInteger bigintB = RandomEInteger(rand);
      if (bigintB.IsZero) {
        bigintB = EInteger.One;
      }
      return ERational.Create(bigintA, bigintB);
    }

    public static EDecimal GenerateEDecimalSmall(IRandomGenExtended wrapper) {
      if (wrapper == null) {
        throw new ArgumentNullException(nameof(wrapper));
      }
      if (wrapper.GetInt32(2) == 0) {
        var eix = EInteger.FromBytes(
            RandomObjects.RandomByteString(wrapper, 1 + wrapper.GetInt32(36)),
            true);
        int exp = wrapper.GetInt32(25) - 12;
        return EDecimal.Create(eix, exp);
      }
      return
EDecimal.FromString(RandomObjects.RandomDecimalStringShort(wrapper, false));
    }

    public static EDecimal RandomEDecimal(IRandomGenExtended r) {
      return RandomEDecimal(r, null);
    }

    public static EDecimal RandomEDecimal(IRandomGenExtended r, string[]
      decimalString) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (r.GetInt32(100) == 0) {
        int x = r.GetInt32(3);
        if (x == 0) {
          if (decimalString != null) {
            decimalString[0] = "Infinity";
          }
          return EDecimal.PositiveInfinity;
        }
        if (x == 1) {
          if (decimalString != null) {
            decimalString[0] = "-Infinity";
          }
          return EDecimal.NegativeInfinity;
        }
        if (x == 2) {
          if (decimalString != null) {
            decimalString[0] = "NaN";
          }
          return EDecimal.NaN;
        }
        // Signaling NaN currently not generated because
        // it doesn't round-trip as well
      }
      if (r.GetInt32(100) < 30) {
        string str = RandomObjects.RandomDecimalString(r);
        if (str.Length < 500) {
          if (decimalString != null) {
            decimalString[0] = str;
          }
          return EDecimal.FromString(str);
        }
      }
      EInteger emant = RandomEInteger(r);
      EInteger eexp;
      if (r.GetInt32(100) < 95) {
        int exp = (r.GetInt32(100) < 80) ? (r.GetInt32(50) - 25) :
          (r.GetInt32(5000) - 2500);
        eexp = EInteger.FromInt32(exp);
      } else {
        eexp = RandomEInteger(r);
      }
      var ed = EDecimal.Create(emant, eexp);
      if (decimalString != null) {
        decimalString[0] = emant.ToString() + "E" + eexp.ToString();
      }
      return ed;
    }

    private static EInteger BitHeavyEInteger(IRandomGenExtended rg, int count) {
      var sb = new StringBuilder();
      int[] oneChances = {
        999, 1, 980, 20, 750, 250, 980,
        20, 980, 20, 980, 20, 750, 250,
      };
      int oneChance = oneChances[rg.GetInt32(oneChances.Length)];
      for (int i = 0; i < count; ++i) {
        _ = sb.Append((rg.GetInt32(1000) >= oneChance) ? '0' : '1');
      }
      return EInteger.FromRadixString(sb.ToString(), 2);
    }

    private static EInteger DigitHeavyEInteger(IRandomGenExtended rg, int
      count) {
      var sb = new StringBuilder();
      int[] oneChances = {
        999, 1, 980, 20, 750, 250, 980,
        20, 980, 20, 980, 20, 750, 250,
      };
      int oneChance = oneChances[rg.GetInt32(oneChances.Length)];
      for (int i = 0; i < count; ++i) {
        _ = sb.Append((rg.GetInt32(1000) >= oneChance) ? '0' : '9');
      }
      return EInteger.FromRadixString(sb.ToString(), 10);
    }

    public static EInteger RandomEInteger(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      int selection = r.GetInt32(100);
      if (selection < 10) {
        int count = r.GetInt32(MaxNumberLength);
        count = (int)((long)count * r.GetInt32(MaxNumberLength) /
          MaxNumberLength);
        count = (int)((long)count * r.GetInt32(MaxNumberLength) /
          MaxNumberLength);
        count = Math.Max(count, 1);
        if (selection is 0 or 1) {
          return BitHeavyEInteger(r, count);
        } else if ((selection == 2 || selection == 3) && count < 500) {
          return DigitHeavyEInteger(r, count);
        }
        byte[] bytes = RandomObjects.RandomByteString(r, count);
        return EInteger.FromBytes(bytes, true);
      } else {
        byte[] bytes = RandomObjects.RandomByteString(
            r,
            r.GetInt32(MaxShortNumberLength) + 1);
        return EInteger.FromBytes(bytes, true);
      }
    }

    public static EInteger RandomEIntegerSmall(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      byte[] bytes = RandomObjects.RandomByteString(
          r,
          r.GetInt32(MaxShortNumberLength) + 1);
      return EInteger.FromBytes(bytes, true);
    }

    private static int IntInRange(IRandomGenExtended rg, int minInc, int
      maxExc) {
      return minInc + rg.GetInt32(maxExc - minInc);
    }

    public static EFloat CloseToPowerOfTwo(IRandomGenExtended rg) {
      if (rg == null) {
        throw new ArgumentNullException(nameof(rg));
      }
      int pwr = (rg.GetInt32(100) < 80) ? IntInRange(rg, -20, 20) :
        IntInRange(rg, -300, 300);
      int pwr2 = pwr - (rg.GetInt32(100) < 80 ? IntInRange(rg, 51, 61) :
        IntInRange(rg, 2, 300));
      EFloat ef = rg.GetInt32(2) == 0 ? EFloat.Create(1,
        pwr).Add(EFloat.Create(1, pwr2)) : EFloat.Create(1,
          pwr).Subtract(EFloat.Create(1, pwr2));
      if (rg.GetInt32(10) == 0) {
        pwr2 = pwr - (rg.GetInt32(100) < 80 ? IntInRange(rg, 51, 61) :
          IntInRange(rg, 2, 300));
        ef = (rg.GetInt32(2) == 0) ? ef.Add(EFloat.Create(1, pwr2)) :
          ef.Subtract(EFloat.Create(1, pwr2));
      }
      return ef;
    }

    public static EFloat RandomEFloat(IRandomGenExtended r) {
      if (r == null) {
        throw new ArgumentNullException(nameof(r));
      }
      if (r.GetInt32(100) == 0) {
        int x = r.GetInt32(3);
        if (x == 0) {
          return EFloat.PositiveInfinity;
        }
        if (x == 1) {
          return EFloat.NegativeInfinity;
        }
        if (x == 2) {
          return EFloat.NaN;
        }
      }
      return r.GetInt32(100) == 3 ?
        CloseToPowerOfTwo(r) : EFloat.Create(
        RandomEInteger(r),
        (EInteger)(r.GetInt32(400) - 200));
    }

    public static EInteger RandomSmallIntegral(IRandomGenExtended r) {
      return EInteger.FromString(RandomObjects.RandomSmallIntegralString(r));
    }
  }
}
