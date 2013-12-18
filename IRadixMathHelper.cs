using System;
using System.Text;
//using System.Numerics;

namespace PeterO {
  interface IRadixMathHelper<T> {
    int GetRadix();
    int GetSign(T value);
    BigInteger GetMantissa(T value);
    BigInteger GetExponent(T value);
    BigInteger RescaleByExponentDiff(BigInteger value, BigInteger exp1, BigInteger exp2);
    T CreateNew(BigInteger mantissa, BigInteger exponent);
    IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger value, int lastDigit, int olderDigits);
    IShiftAccumulator CreateShiftAccumulator(BigInteger value);
    bool HasTerminatingRadixExpansion(BigInteger num, BigInteger den);
    BigInteger MultiplyByRadixPower(BigInteger value, FastInteger power);
  }
}