using System;
//using System.Numerics;

namespace PeterO {
  interface IShiftAccumulator {
    BigInteger ShiftedInt { get; }
    FastInteger GetDigitLength();
    int OlderDiscardedDigits { get; }
    int LastDiscardedDigit { get; }
    FastInteger ShiftedIntFast { get; }
    FastInteger DiscardedDigitCount { get; }
    void ShiftRight(FastInteger bits);
    void ShiftRight(int bits);
    void ShiftToDigits(FastInteger bits);
  }
}