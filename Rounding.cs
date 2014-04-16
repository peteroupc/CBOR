/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
namespace PeterO {
    /// <summary>Specifies the mode to use when &quot;shortening&quot;
    /// numbers that otherwise can&apos;t fit a given number of digits, so
    /// that the shortened number has about the same value. This &quot;shortening&quot;
    /// is known as rounding.</summary>
  public enum Rounding {
    /// <summary>If there is a fractional part, the number is rounded to the
    /// closest representable number away from zero.</summary>
    Up,

    /// <summary>The fractional part is discarded (the number is truncated).</summary>
    Down,

    /// <summary>If there is a fractional part, the number is rounded to the
    /// highest representable number that&apos;s closest to it.</summary>
    Ceiling,

    /// <summary>If there is a fractional part, the number is rounded to the
    /// lowest representable number that&apos;s closest to it.</summary>
    Floor,

    /// <summary>Rounded to the nearest number; if the fractional part is
    /// exactly half, the number is rounded to the closest representable
    /// number away from zero. This is the most familiar rounding mode for
    /// many people.</summary>
    HalfUp,

    /// <summary>Rounded to the nearest number; if the fractional part is
    /// exactly half, it is discarded.</summary>
    HalfDown,

    /// <summary>Rounded to the nearest number; if the fractional part is
    /// exactly half, the number is rounded to the closest representable
    /// number that is even. This is sometimes also known as &quot;banker&apos;s
    /// rounding&quot;.</summary>
    HalfEven,

    /// <summary>Indicates that rounding will not be used. If rounding is
    /// required, the rounding operation will report an error.</summary>
    Unnecessary,

    /// <summary>If there is a fractional part and if the last digit before
    /// rounding is 0 or half the radix, the number is rounded to the closest
    /// representable number away from zero; otherwise the fractional part
    /// is discarded. In overflow, the fractional part is always discarded.
    /// This rounding mode is useful for rounding intermediate results at
    /// a slightly higher precision than the final precision.</summary>
    ZeroFiveUp
  }
}
