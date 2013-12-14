package com.upokecenter.util;

//import java.math.*;


  interface IShiftAccumulator {
    BigInteger getShiftedInt();
    FastInteger GetDigitLength();
    int getOlderDiscardedDigits();
    int getLastDiscardedDigit();
    FastInteger getShiftedIntFast();
    FastInteger getDiscardedDigitCount();
    void ShiftRight(FastInteger bits);
    void ShiftRight(int bits);
    void ShiftToDigits(FastInteger bits);
  }
