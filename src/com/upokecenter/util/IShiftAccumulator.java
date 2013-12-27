package com.upokecenter.util;

//import java.math.*;

    /**
     * Common interface for classes that shift a number of digits and record
     * information on whether a non-zero digit was discarded this way.
     */
  interface IShiftAccumulator {
    BigInteger getShiftedInt();
    FastInteger GetDigitLength();
    int getOlderDiscardedDigits();
    int getLastDiscardedDigit();
    FastInteger getShiftedIntFast();
    FastInteger getDiscardedDigitCount();
    void ShiftRight(FastInteger bits);
    void ShiftRightInt(int bits);
    void ShiftToDigits(FastInteger bits);
  }

