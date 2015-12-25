/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
  internal interface IRadixMathHelper<T> {
    int GetRadix();

    int GetArithmeticSupport();

    int GetSign(T value);

    int GetFlags(T value);

    EInteger GetMantissa(T value);

    EInteger GetExponent(T value);

    T ValueOf(int val);

    T CreateNewWithFlags(EInteger mantissa, EInteger exponent, int flags);

    IShiftAccumulator CreateShiftAccumulatorWithDigits(
EInteger value,
int lastDigit,
int olderDigits);

    IShiftAccumulator CreateShiftAccumulator(EInteger value);

    bool HasTerminatingRadixExpansion(EInteger num, EInteger den);

    EInteger MultiplyByRadixPower(EInteger value, FastInteger power);
  }
}
