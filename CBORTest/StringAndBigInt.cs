using System;
using System.Text;
using PeterO;

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

    private BigInteger bigintValue;

    public BigInteger BigIntValue {
      get {
        return this.bigintValue;
      }
    }

    public static StringAndBigInt Generate(FastRandom rand, int radix) {
      BigInteger bv = BigInteger.Zero;
      var sabi = new StringAndBigInt();
      int numDigits = 1 + rand.NextValue(100);
      var negative = false;
      var builder = new StringBuilder();
      if (rand.NextValue(2) == 0) {
        builder.Append('-');
        negative = true;
      }
      for (int i = 0; i < numDigits; ++i) {
        int digit = rand.NextValue(radix);
        if (rand.NextValue(2) == 0) {
          builder.Append(ValueDigits[digit]);
        } else {
          builder.Append(ValueDigitsLower[digit]);
        }
        //var bigintTmp = (BigInteger)radix;
        //bv *= bigintTmp;
        //bigintTmp = (BigInteger)digit;
        //bv += bigintTmp;
      }
      //if (negative) {
      //  bv = -bv;
      //}
      bv = BigInteger.fromRadixString(builder.ToString(),radix);
      sabi.bigintValue = bv;
      sabi.stringValue = builder.ToString();
      return sabi;
    }
  }
}
