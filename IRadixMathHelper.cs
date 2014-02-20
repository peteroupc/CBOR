using System;
using System.Text;
// using System.Numerics;
namespace PeterO {
  internal interface IRadixMathHelper<T> {
    int GetRadix();

    int GetArithmeticSupport();

    int GetSign(T value);

    int GetFlags(T value);

    BigInteger GetMantissa(T value);

    BigInteger GetExponent(T value);

    T ValueOf(int val);

    T CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags);

    IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger value, int lastDigit, int olderDigits);

    IShiftAccumulator CreateShiftAccumulator(BigInteger value);

    bool HasTerminatingRadixExpansion(BigInteger num, BigInteger den);

    BigInteger MultiplyByRadixPower(BigInteger value, FastInteger power);
  }
}
