/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */

using System;

namespace PeterO {
    /// <summary><para><b>This class is obsolete. It will be replaced by a new version of this
    /// class in a different namespace/package and library, called
    /// <c>PeterO.Numbers.ERounding
    /// </c>
    /// in the
    /// <c>PeterO.ERounding
    /// </c>
    /// library (in .NET), or
    /// <c>com.upokecenter.numbers.EFloat
    /// </c>
    /// in the
    /// <a href='https://github.com/peteroupc/numbers-java'>
    /// <c>com.github.peteroupc/numbers
    /// </c>
    /// </a>
    /// artifact (in Java).
    /// </b>
    /// </para>
    /// Specifies the mode to use when "shortening" numbers that otherwise can't
    /// fit a given number of digits, so that the shortened number has about the
    /// same value. This "shortening" is known as rounding.</summary>
  [Obsolete("Use ERounding from PeterO.Numbers/com.upokecenter.numbers.")]
  public enum Rounding {
    /// <summary>If there is a fractional part, the number is rounded to the closest
    /// representable number away from zero.</summary>
    Up,

    /// <summary>The fractional part is discarded (the number is truncated).</summary>
    Down,

    /// <summary>If there is a fractional part, the number is rounded to the highest
    /// representable number that's closest to it.</summary>
    Ceiling,

    /// <summary>If there is a fractional part, the number is rounded to the lowest
    /// representable number that's closest to it.</summary>
    Floor,

    /// <summary>Rounded to the nearest number; if the fractional part is exactly half, the
    /// number is rounded to the closest representable number away from zero. This
    /// is the most familiar rounding mode for many people.</summary>
    HalfUp,

    /// <summary>Rounded to the nearest number; if the fractional part is exactly half, it
    /// is discarded.</summary>
    HalfDown,

    /// <summary>Rounded to the nearest number; if the fractional part is exactly half, the
    /// number is rounded to the closest representable number that is even. This
    /// is sometimes also known as "banker's rounding".</summary>
    HalfEven,

    /// <summary>Indicates that rounding will not be used. If rounding is required, the
    /// rounding operation will report an error.</summary>
    Unnecessary,

    /// <summary>If there is a fractional part and if the last digit before rounding is 0
    /// or half the radix, the number is rounded to the closest representable
    /// number away from zero; otherwise the fractional part is discarded. In
    /// overflow, the fractional part is always discarded.</summary>
    ZeroFiveUp,

    /// <summary>If there is a fractional part and the whole number part is even, the
    /// number is rounded to the closest representable odd number away from zero.</summary>
    Odd,

    /// <summary>For binary floating point numbers, this is the same as Odd. For other
    /// bases (including decimal numbers), this is the same as ZeroFiveUp. This
    /// rounding mode is useful for rounding intermediate results at a slightly
    /// higher precision (at least 2 bits more for binary) than the final
    /// precision.</summary>
    OddOrZeroFiveUp,
  }
}
