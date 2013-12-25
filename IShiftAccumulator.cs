using System;
//using System.Numerics;

namespace PeterO {
    /// <summary> Common interface for classes that shift a number of digits
    /// and record information on whether a non-zero digit was discarded
    /// this way. </summary>
  interface IShiftAccumulator {
    BigInteger ShiftedInt { get; }
    FastInteger GetDigitLength();
    int OlderDiscardedDigits { get; }
    int LastDiscardedDigit { get; }
    FastInteger ShiftedIntFast { get; }
    FastInteger DiscardedDigitCount { get; }
    void ShiftRight(FastInteger bits);
    void ShiftRightInt(int bits);
    void ShiftToDigits(FastInteger bits);
  }
}