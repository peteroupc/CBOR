/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Numerics;
namespace PeterO {
    /// <summary> Contains parameters for controlling the precision, rounding,
    /// and exponent range of arbitrary-precision numbers. </summary>
  public class PrecisionContext {
    BigInteger eMax;
    /// <summary> Gets the highest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the decimal
    /// point. For example, with a precision of 3 and an EMax of 100, the maximum
    /// value possible is 9.99E+100. (This is not the same as the highest possible
    /// Exponent property.) </summary>
    public BigInteger EMax {
      get { return eMax; }
    }
    BigInteger eMin;

    /// <summary> Gets the lowest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the decimal
    /// point. For example, with a precision of 3 and an EMin of -100, the next
    /// value that comes after 0 is 0.001E-100. (This is not the same as the
    /// lowest possible Exponent property.) </summary>
    public BigInteger EMin {
      get { return eMin; }
    }
    long precision;

    /// <summary> Gets the maximum length of a converted number in digits,
    /// ignoring the decimal point and exponent. For example, if precision
    /// is 3, a converted number's mantissa can range from 0 to 999 (up to three
    /// digits long). If 0, converted numbers can have any precision. </summary>
    public long Precision {
      get { return precision; }
    }
    Rounding rounding;

    bool clampNormalExponents;

    /// <summary> If true, a converted number's Exponent property will not
    /// be higher than EMax + 1 - Precision. If a number's exponent is higher
    /// than that value, but not high enough to cause overflow, the exponent
    /// is clamped to that value and enough zeros are added to the number's
    /// mantissa to account for the adjustment. </summary>
    public bool ClampNormalExponents {
      get { return clampNormalExponents; }
    }

    /// <summary> Gets the desired rounding mode when converting numbers
    /// that can't be represented in the given precision and exponent range.
    /// </summary>
    public Rounding Rounding {
      get { return rounding; }
    }

    int flags;
    bool hasFlags;

    /// <summary> Returns whether this context has a mutable Flags field.
    /// </summary>
    public bool HasFlags {
      get { return hasFlags; }
    }

    /// <summary> Signals that the result was rounded to a different mathematical
    /// value, but as close as possible to the original. </summary>
    public const int SignalInexact = 1;
    /// <summary> Signals that the result was rounded to fit the precision;
    /// either the value or the exponent may have changed from the original.
    /// </summary>
    public const int SignalRounded = 2;
    /// <summary> Signals that the result is non-zero and the exponent is
    /// lower than the lowest exponent allowed. </summary>
    public const int SignalSubnormal = 4;
    /// <summary> Signals that the result is non-zero, the exponent is lower
    /// than the lowest exponent allowed, and the result was rounded to a different
    /// mathematical value, but as close as possible to the original. </summary>
    public const int SignalUnderflow = 8;
    /// <summary> Signals that the result is non-zero and the exponent is
    /// higher than the highest exponent allowed. </summary>
    public const int SignalOverflow = 16;
    /// <summary> Signals that the exponent was adjusted to fit the exponent
    /// range. </summary>
    public const int SignalClamped = 32;
    /// <summary> Gets the flags that are set from converting numbers according
    /// to this precision context. If HasFlags is false, this value will be
    /// 0. </summary>
    public int Flags {
      get { return flags; }
    /// <summary> Sets the flags that occur from converting numbers according
    /// to this precision context. </summary>
    /// <exception cref='InvalidOperationException'> HasFlags is false.</exception>
      set {
        if (!this.HasFlags)
          throw new InvalidOperationException("Can't set flags");
        flags = value;
      }
    }

    /// <summary> Copies this PrecisionContext with HasFlags set to true
    /// and a Flags value of 0. </summary>
    /// <returns></returns>
    public PrecisionContext WithBlankFlags() {
      PrecisionContext pc = new PrecisionContext(this);
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }
    /// <summary> Copies this PrecisionContext with HasFlags set to false
    /// and a Flags value of 0. </summary>
    /// <returns></returns>
    public PrecisionContext WithNoFlags() {
      PrecisionContext pc = new PrecisionContext(this);
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }
    /// <summary> Initializes a new PrecisionContext that is a copy of another
    /// PrecisionContext. </summary>
    public PrecisionContext(PrecisionContext pc) {
      if ((pc) == null) throw new ArgumentNullException("pc");
      this.hasFlags = pc.hasFlags;
      this.flags = pc.flags;
      this.eMax = pc.eMax;
      this.eMin = pc.eMin;
      this.precision = pc.precision;
      this.rounding = pc.rounding;
      this.clampNormalExponents = pc.clampNormalExponents;
    }
    /// <summary> Initializes a new PrecisionContext. HasFlags will be
    /// set to false. </summary>
    public PrecisionContext(long precision, Rounding rounding, long eMinLong, long eMaxLong) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(precision)) + ")");
      if ((eMinLong) > eMaxLong) throw new ArgumentOutOfRangeException("eMinLong" + " not less or equal to " + Convert.ToString((long)(eMaxLong)) + " (" + Convert.ToString((long)(eMinLong)) + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = false;
      eMax = (BigInteger)eMaxLong;
      eMin = (BigInteger)eMinLong;
    }
    /// <summary> Initializes a new PrecisionContext. HasFlags will be
    /// set to false. </summary>
    public PrecisionContext(long precision, Rounding rounding, long eMinLong, long eMaxLong,
                            bool clampNormalExponents) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(precision)) + ")");
      if ((eMinLong) > eMaxLong) throw new ArgumentOutOfRangeException("eMinLong" + " not less or equal to " + Convert.ToString((long)(eMaxLong)) + " (" + Convert.ToString((long)(eMinLong)) + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = clampNormalExponents;
      eMax = (BigInteger)eMaxLong;
      eMin = (BigInteger)eMinLong;
    }
    /// <summary> Initializes a new PrecisionContext. HasFlags will be
    /// set to false. </summary>
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(precision)) + ")");
      if (eMin.CompareTo(eMax) > 0) throw new ArgumentOutOfRangeException("eMin" + " not less or equal to " + eMax + " (" + eMin + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = false;
      this.eMax = eMax;
      this.eMin = eMin;
    }
    /// <summary> Initializes a new PrecisionContext. HasFlags will be
    /// set to false. </summary>
    public PrecisionContext(long precision, Rounding rounding, BigInteger eMin, BigInteger eMax,
                            bool clampNormalExponents) {
      if ((precision) < 0) throw new ArgumentOutOfRangeException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((long)(precision)) + ")");
      if (eMin.CompareTo(eMax) > 0) throw new ArgumentOutOfRangeException("eMin" + " not less or equal to " + eMax + " (" + eMin + ")");
      this.precision = precision;
      this.rounding = rounding;
      this.hasFlags = false;
      this.clampNormalExponents = clampNormalExponents;
      this.eMax = eMax;
      this.eMin = eMin;
    }

    /// <summary> Precision context for the IEEE-754-2008 decimal32 format.
    /// </summary>
    public static readonly PrecisionContext Decimal32 =
      new PrecisionContext(7, Rounding.HalfEven, -95, 96, true);
    /// <summary> Precision context for the IEEE-754-2008 decimal64 format.
    /// </summary>
    public static readonly PrecisionContext Decimal64 =
      new PrecisionContext(16, Rounding.HalfEven, -383, 384, true);
    /// <summary> Precision context for the IEEE-754-2008 decimal128 format.
    /// </summary>
    public static readonly PrecisionContext Decimal128 =
      new PrecisionContext(34, Rounding.HalfEven, -6143, 6144, true);
  }
}