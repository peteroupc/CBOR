package com.upokecenter.util;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  interface IRadixMathHelper<T> {
    int GetRadix();

    int GetArithmeticSupport();

    int GetSign(T value);

    int GetFlags(T value);

    BigInteger GetMantissa(T value);

    BigInteger GetExponent(T value);

    T ValueOf(int val);

    T CreateNewWithFlags(BigInteger mantissa, BigInteger exponent, int flags);

    IShiftAccumulator CreateShiftAccumulatorWithDigits(
BigInteger value,
int lastDigit,
int olderDigits);

    IShiftAccumulator CreateShiftAccumulator(BigInteger value);

    boolean HasTerminatingRadixExpansion(BigInteger num, BigInteger den);

    BigInteger MultiplyByRadixPower(BigInteger value, FastInteger power);
  }
