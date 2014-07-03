package com.upokecenter.util;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Specifies the mode to use when &quot;shortening&quot; numbers that otherwise
     * can&apos;t fit a given number of digits, so that the shortened number
     * has about the same value. This &quot;shortening&quot; is known as
     * rounding.
     */
  public enum Rounding {
    /**
     * If there is a fractional part, the number is rounded to the closest
     * representable number away from zero.
     */
    Up,

    /**
     * The fractional part is discarded (the number is truncated).
     */
    Down,

    /**
     * If there is a fractional part, the number is rounded to the highest
     * representable number that&apos;s closest to it.
     */
    Ceiling,

    /**
     * If there is a fractional part, the number is rounded to the lowest
     * representable number that&apos;s closest to it.
     */
    Floor,

    /**
     * Rounded to the nearest number; if the fractional part is exactly half, the
     * number is rounded to the closest representable number away from zero.
     * This is the most familiar rounding mode for many people.
     */
    HalfUp,

    /**
     * Rounded to the nearest number; if the fractional part is exactly half, it is
     * discarded.
     */
    HalfDown,

    /**
     * Rounded to the nearest number; if the fractional part is exactly half, the
     * number is rounded to the closest representable number that is even.
     * This is sometimes also known as &quot;banker&apos;s rounding&quot;.
     */
    HalfEven,

    /**
     * Indicates that rounding will not be used. If rounding is required, the
     * rounding operation will report an error.
     */
    Unnecessary,

    /**
     * If there is a fractional part and if the last digit before rounding is 0 or
     * half the radix, the number is rounded to the closest representable
     * number away from zero; otherwise the fractional part is discarded. In
     * overflow, the fractional part is always discarded. This rounding mode
     * is useful for rounding intermediate results at a slightly higher
     * precision than the final precision.
     */
    ZeroFiveUp
  }
