using System;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace Test {
  internal sealed class StringAndBigInt {
    private const string ValueDigits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private const string ValueDigitsLower =
      "0123456789abcdefghijklmnopqrstuvwxyz";

    private String stringValue;

    public String StringValue {
      get {
        return this.stringValue;
      }
    }

    private EInteger bigintValue;

    public EInteger BigIntValue {
      get {
        return this.bigintValue;
      }
    }

    public static StringAndBigInt Generate(IRandomGenExtended rand, int radix) {
      return Generate(rand, radix, 50);
    }

    public static StringAndBigInt Generate(
      IRandomGenExtended rand,
      int radix,
      int maxNumDigits) {
      if (radix < 2) {
        throw new ArgumentException("radix(" + radix +
          ") is less than 2");
      }
      if (radix > 36) {
        throw new ArgumentException("radix(" + radix +
          ") is more than 36");
      }
      EInteger bv = EInteger.Zero;
      var sabi = new StringAndBigInt();
      int numDigits = 1 + rand.GetInt32(maxNumDigits);
      var negative = false;
      var builder = new StringBuilder();
      if (rand.GetInt32(2) == 0) {
        builder.Append('-');
        negative = true;
      }
      int radixpowint = radix * radix * radix * radix;
      EInteger radixpow4 = EInteger.FromInt32(radixpowint);
      EInteger radixpow1 = EInteger.FromInt32(radix);
      var count = 0;
      for (int i = 0; i < numDigits - 4; i += 4) {
        int digitvalues = rand.GetInt32(radixpowint);
        int digit = digitvalues % radix;
        digitvalues /= radix;
        int digit2 = digitvalues % radix;
        digitvalues /= radix;
        int digit3 = digitvalues % radix;
        digitvalues /= radix;
        int digit4 = digitvalues % radix;
        digitvalues /= radix;
        count += 4;
        int bits = rand.GetInt32(16);
        if ((bits & 0x01) == 0) {
          builder.Append(ValueDigits[digit]);
        } else {
          builder.Append(ValueDigitsLower[digit]);
        }
        if ((bits & 0x02) == 0) {
          builder.Append(ValueDigits[digit2]);
        } else {
          builder.Append(ValueDigitsLower[digit2]);
        }
        if ((bits & 0x04) == 0) {
          builder.Append(ValueDigits[digit3]);
        } else {
          builder.Append(ValueDigitsLower[digit3]);
        }
        if ((bits & 0x08) == 0) {
          builder.Append(ValueDigits[digit4]);
        } else {
          builder.Append(ValueDigitsLower[digit4]);
        }
        int digits = (((((digit * radix) + digit2) *
                radix) + digit3) * radix) + digit4;
        bv *= radixpow4;
        var bigintTmp = (EInteger)digits;
        bv += bigintTmp;
      }
      for (int i = count; i < numDigits; ++i) {
        int digit = rand.GetInt32(radix);
        if (rand.GetInt32(2) == 0) {
          builder.Append(ValueDigits[digit]);
        } else {
          builder.Append(ValueDigitsLower[digit]);
        }
        bv *= radixpow1;
        var bigintTmp = (EInteger)digit;
        bv += bigintTmp;
      }
      if (negative) {
        bv = -bv;
      }
      sabi.bigintValue = bv;
      sabi.stringValue = builder.ToString();
      return sabi;
    }
  }
}
