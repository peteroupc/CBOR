package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

    /**
     * Description of BigNumberFlags.
     */
  final class BigNumberFlags {
private BigNumberFlags() {
}
    static final int FlagNegative = 1;
    static final int FlagQuietNaN = 4;
    static final int FlagSignalingNaN = 8;
    static final int FlagInfinity = 2;
    static final int FlagSpecial = FlagQuietNaN | FlagSignalingNaN | FlagInfinity;
    static final int FlagNaN = FlagQuietNaN | FlagSignalingNaN;

    static final int FiniteOnly = 0;
    static final int FiniteAndNonFinite = 1;
    static final int X3Dot274 = 2;
  }
