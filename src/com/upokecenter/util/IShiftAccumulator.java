package com.upokecenter.util;

//import java.math.*;


  interface IShiftAccumulator {
    BigInteger getShiftedInt();
    long getDigitLength();
    boolean isSmall();
    int getOlderDiscardedDigits();
    int getLastDiscardedDigit();
    long getShiftedIntSmall();
    FastInteger getDiscardedDigitCount();
    void ShiftRight(FastInteger bits);
    void ShiftRight(int bits);
    void ShiftToDigits(long bits);
  }
