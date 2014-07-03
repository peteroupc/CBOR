package com.upokecenter.cbor;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  interface ICBORNumber {

    boolean IsPositiveInfinity(Object obj);

    boolean IsInfinity(Object obj);

    boolean IsNegativeInfinity(Object obj);

    boolean IsNaN(Object obj);

    double AsDouble(Object obj);

    Object Negate(Object obj);

    Object Abs(Object obj);

    ExtendedDecimal AsExtendedDecimal(Object obj);

    ExtendedFloat AsExtendedFloat(Object obj);

    ExtendedRational AsExtendedRational(Object obj);

    float AsSingle(Object obj);

    BigInteger AsBigInteger(Object obj);

    long AsInt64(Object obj);

    boolean CanFitInSingle(Object obj);

    boolean CanFitInDouble(Object obj);

    boolean CanFitInInt32(Object obj);

    boolean CanFitInInt64(Object obj);

    boolean CanTruncatedIntFitInInt64(Object obj);

    boolean CanTruncatedIntFitInInt32(Object obj);

    int AsInt32(Object obj, int minValue, int maxValue);

    boolean IsZero(Object obj);

    int Sign(Object obj);

    boolean IsIntegral(Object obj);
  }
