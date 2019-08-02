/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

namespace PeterO {
    /// <summary>A precision context.</summary>
  [Obsolete("Use EContext from PeterO.Numbers/com.upokecenter.numbers.")]
  public class PrecisionContext {
    private readonly EContext ec;

    internal EContext Ec {
      get {
        return this.ec;
      }
    }

    internal PrecisionContext(EContext ec) {
      this.ec = ec;
    }

    /// <summary>Initializes a new instance of the <see cref='PeterO.PrecisionContext'/> class. HasFlags will be set to false.</summary><param name='precision'>The maximum number of digits a number can have, or 0 for an unlimited
    /// number of digits.
    /// </param><param name='rounding'>The rounding mode to use when a number can't fit the given precision.
    /// </param><param name='exponentMinSmall'>The minimum exponent.
    /// </param><param name='exponentMaxSmall'>The maximum exponent.
    /// </param><param name='clampNormalExponents'>Whether to clamp a number's significand to the given maximum precision (if
    /// it isn't zero) while remaining within the exponent range.
    /// </param>
    public PrecisionContext(
      int precision,
      Rounding rounding,
      int exponentMinSmall,
      int exponentMaxSmall,
      bool clampNormalExponents) {
      throw new NotSupportedException("This class is now obsolete.");
    }

    /// <summary>Gets a string representation of this object. Note that the format is not
    /// intended to be parsed and may change at any time.</summary><returns>A string representation of this object.
    /// </returns>
    public override string ToString() {
      return String.Empty;
    }
  }
}
