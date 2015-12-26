/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.IShiftAccumulator"]'/>
  internal interface IShiftAccumulator {
    EInteger ShiftedInt { get; }

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
