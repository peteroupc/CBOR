package com.upokecenter.util;

// import java.math.*;

  internal interface IRadixMathHelper<T> {
    int GetRadix();

    int GetArithmeticSupport();

    int GetSign(T value);

    int GetFlags(T value);

    BigInteger GetMantissa(T value);

    BigInteger GetExponent(T value);

    BigInteger RescaleByExponentDiff(BigInteger value, BigInteger exp1, BigInteger exp2);

    T ValueOf(int val);

    T CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags);

    IShiftAccumulator CreateShiftAccumulatorWithDigits(BigInteger value, int lastDigit, int olderDigits);

    IShiftAccumulator CreateShiftAccumulator(BigInteger value);

    boolean HasTerminatingRadixExpansion(BigInteger num, BigInteger den);

    BigInteger MultiplyByRadixPower(BigInteger value, FastInteger power);
  }

