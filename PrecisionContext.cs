/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */
using System;
//using System.Numerics;
namespace PeterO {
    /// <summary> Contains parameters for controlling the precision, rounding,
    /// and exponent range of arbitrary-precision numbers. </summary>
  public class PrecisionContext {
    BigInteger exponentMax;
    /// <summary> Gets the highest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the decimal
    /// point. For example, with a precision of 3 and an EMax of 100, the maximum
    /// value possible is 9.99E+100. (This is not the same as the highest possible
    /// Exponent property.) If HasExponentRange is false, this value will
    /// be 0.</summary>
    public BigInteger EMax {
      get { return hasExponentRange ? exponentMax : BigInteger.Zero; }
    }
    BigInteger exponentMin;

    bool hasExponentRange;
    /// <summary> Gets whether this context defines a minimum and maximum
    /// exponent. If false, converted exponents can have any exponent. </summary>
    public bool HasExponentRange {
      get { return hasExponentRange; }
    }

    /// <summary> Gets the lowest exponent possible when a converted number
    /// is expressed in scientific notation with one digit before the decimal
    /// point. For example, with a precision of 3 and an EMin of -100, the next
    /// value that comes after 0 is 0.001E-100. (This is not the same as the
    /// lowest possible Exponent property.) If HasExponentRange is false,
    /// this value will be 0.</summary>
    public BigInteger EMin {
      get { return hasExponentRange ? exponentMin : BigInteger.Zero; }
    }
    BigInteger bigintPrecision;

    /// <summary> Gets the maximum length of a converted number in digits,
    /// ignoring the decimal point and exponent. For example, if precision
    /// is 3, a converted number's mantissa can range from 0 to 999 (up to three
    /// digits long). If 0, converted numbers can have any precision. </summary>
    public BigInteger Precision {
      get { return bigintPrecision; }
    }
    Rounding rounding;

    bool clampNormalExponents;

    /// <summary> If true, a converted number's Exponent property will not
    /// be higher than EMax + 1 - Precision. If a number's exponent is higher
    /// than that value, but not high enough to cause overflow, the exponent
    /// is clamped to that value and enough zeros are added to the number's
    /// mantissa to account for the adjustment. If HasExponentRange is false,
    /// this value is always false.</summary>
    public bool ClampNormalExponents {
      get { return hasExponentRange ? clampNormalExponents : false; }
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
    public const int FlagInexact = 1;
    /// <summary> Signals that the result was rounded to fit the precision;
    /// either the value or the exponent may have changed from the original.
    /// </summary>
    public const int FlagRounded = 2;
    /// <summary> Signals that the result is non-zero and the exponent is
    /// lower than the lowest exponent allowed. </summary>
    public const int FlagSubnormal = 4;
    /// <summary> Signals that the result is non-zero, the exponent is lower
    /// than the lowest exponent allowed, and the result was rounded to a different
    /// mathematical value, but as close as possible to the original. </summary>
    public const int FlagUnderflow = 8;
    /// <summary> Signals that the result is non-zero and the exponent is
    /// higher than the highest exponent allowed. </summary>
    public const int FlagOverflow = 16;
    /// <summary> Signals that the exponent was adjusted to fit the exponent
    /// range. </summary>
    public const int FlagClamped = 32;
    /// <summary> </summary>
    public const int FlagInvalid = 64;
    /// <summary> </summary>
    public const int FlagDivideByZero = 128;
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

    /// <summary> </summary>
    /// <param name='exponent'>A BigInteger object.</param>
    /// <returns>A Boolean object.</returns>
    public bool ExponentWithinRange(BigInteger exponent){
      if((exponent)==null)throw new ArgumentNullException("exponent");
      if(!this.HasExponentRange)
        return true;
      if(bigintPrecision.IsZero){
        // Only check EMax, since with an unlimited
        // precision, any exponent less than EMin will exceed EMin if
        // the mantissa is the right size
        return exponent.CompareTo(this.EMax)<=0;
      } else {
        BigInteger bigint=exponent;
        bigint+=(BigInteger)bigintPrecision;
        bigint-=BigInteger.One;
        if(bigint.CompareTo(this.EMin)<0)
          return false;
        if(exponent.CompareTo(this.EMax)>0)
          return false;
        return true;
      }
    }

    /// <summary> Copies this PrecisionContext with the specified rounding
    /// mode. </summary>
    /// <returns>A PrecisionContext object.</returns>
    /// <param name='rounding'>A Rounding object.</param>
    public PrecisionContext WithRounding(Rounding rounding) {
      PrecisionContext pc = this.Copy();
      pc.rounding = rounding;
      return pc;
    }

    /// <summary> Copies this PrecisionContext with HasFlags set to true
    /// and a Flags value of 0. </summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithBlankFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = true;
      pc.flags = 0;
      return pc;
    }

    /// <summary> Copies this precision context and sets the copy's "ClampNormalExponents"
    /// flag to the given value.</summary>
    /// <param name='clamp'>A Boolean object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithExponentClamp(bool clamp) {
      PrecisionContext pc = this.Copy();
      pc.clampNormalExponents=clamp;
      return pc;
    }

    /// <summary> </summary>
    /// <param name='exponentMin'>A BigInteger object.</param>
    /// <param name='exponentMax'>A BigInteger object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithExponentRange(BigInteger exponentMin, BigInteger exponentMax) {
      if((exponentMin)==null)throw new ArgumentNullException("exponentMin");
      if(exponentMin.CompareTo(exponentMax)>0)
        throw new ArgumentException("exponentMin greater than exponentMax");
      PrecisionContext pc = this.Copy();
      pc.hasExponentRange=true;
      pc.exponentMin=exponentMin;
      pc.exponentMax=exponentMax;
      return pc;
    }

    /// <summary> Copies this PrecisionContext with HasFlags set to false
    /// and a Flags value of 0. </summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithNoFlags() {
      PrecisionContext pc = this.Copy();
      pc.hasFlags = false;
      pc.flags = 0;
      return pc;
    }
    /// <summary> Copies this PrecisionContext with an unlimited exponent
    /// range. </summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithUnlimitedExponents() {
      PrecisionContext pc = this.Copy();
      pc.hasExponentRange = false;
      return pc;
    }
    /// <summary> Copies this PrecisionContext and gives it a particular
    /// precision value.</summary>
    /// <returns>A PrecisionContext object.</returns>
    /// <param name='precision'>Desired precision. 0 means unlimited
    /// precision.</param>
    public PrecisionContext WithPrecision(int precision) {
      if (precision < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((precision),System.Globalization.CultureInfo.InvariantCulture) + ")");
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = (BigInteger)precision;
      return pc;
    }

    /// <summary> </summary>
    /// <param name='bigintPrecision'>A BigInteger object.</param>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext WithBigPrecision(BigInteger bigintPrecision) {
      if((bigintPrecision)==null)throw new ArgumentNullException("bigintPrecision");
      if (bigintPrecision.Sign < 0) throw new ArgumentException(
        "precision" + " not greater or equal to " + "0" + " (" +
        bigintPrecision + ")");
      PrecisionContext pc = this.Copy();
      pc.bigintPrecision = bigintPrecision;
      return pc;
    }

    /// <summary> Initializes a new PrecisionContext that is a copy of another
    /// PrecisionContext. </summary>
    /// <returns>A PrecisionContext object.</returns>
    public PrecisionContext Copy() {
      PrecisionContext pcnew=new PrecisionContext(0,this.rounding,0,0,this.clampNormalExponents);
      pcnew.hasFlags = this.hasFlags;
      pcnew.flags = this.flags;
      pcnew.exponentMax = this.exponentMax;
      pcnew.exponentMin = this.exponentMin;
      pcnew.hasExponentRange = this.hasExponentRange;
      pcnew.bigintPrecision = this.bigintPrecision;
      pcnew.rounding = this.rounding;
      pcnew.clampNormalExponents = this.clampNormalExponents;
      return pcnew;
    }

    public static PrecisionContext ForPrecision(int precision){
      return new PrecisionContext(precision,Rounding.HalfUp,0,0,false).WithUnlimitedExponents();
    }
    public static PrecisionContext ForRounding(Rounding rounding){
      return new PrecisionContext(0,rounding,0,0,false).WithUnlimitedExponents();
    }
    public static PrecisionContext ForPrecisionAndRounding(int precision, Rounding rounding){
      return new PrecisionContext(precision,rounding,0,0,false).WithUnlimitedExponents();
    }
    /// <summary> Initializes a new PrecisionContext. HasFlags will be
    /// set to false. </summary>
    public PrecisionContext(int precision, Rounding rounding, int exponentMinSmall, int exponentMaxSmall,
                            bool clampNormalExponents) {
      if ((precision) < 0) throw new ArgumentException("precision" + " not greater or equal to " + "0" + " (" + Convert.ToString((precision),System.Globalization.CultureInfo.InvariantCulture) + ")");
      if ((exponentMinSmall) > exponentMaxSmall) throw new ArgumentException("exponentMinSmall" + " not less or equal to " + Convert.ToString((exponentMaxSmall),System.Globalization.CultureInfo.InvariantCulture) + " (" + Convert.ToString((exponentMinSmall),System.Globalization.CultureInfo.InvariantCulture) + ")");
      this.bigintPrecision = (BigInteger)precision;
      this.rounding = rounding;
      this.clampNormalExponents = clampNormalExponents;
      this.hasExponentRange=true;
      exponentMax = (BigInteger)exponentMaxSmall;
      exponentMin = (BigInteger)exponentMinSmall;
    }
    /// <summary> Unlimited precision context. Rounding mode HalfUp.</summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="This PrecisionContext is immutable")]
    #endif
    public static readonly PrecisionContext Unlimited = PrecisionContext.ForPrecision(0);
    /// <summary> Precision context for the IEEE-754-2008 decimal32 format.
    /// </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="This PrecisionContext is immutable")]
    #endif
    public static readonly PrecisionContext Decimal32 =
      new PrecisionContext(7, Rounding.HalfEven, -95, 96,true);
    /// <summary> Precision context for the IEEE-754-2008 decimal64 format.
    /// </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="This PrecisionContext is immutable")]
    #endif
    public static readonly PrecisionContext Decimal64 =
      new PrecisionContext(16, Rounding.HalfEven, -383, 384,true);
    /// <summary> Precision context for the IEEE-754-2008 decimal128 format.
    /// </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="This PrecisionContext is immutable")]
    #endif
    public static readonly PrecisionContext Decimal128 =
      new PrecisionContext(34, Rounding.HalfEven, -6143, 6144,true);
    /// <summary> Precision context for the Common Language Infrastructure
    /// (.NET Framework) decimal format, 96 bits precision. Use RoundToBinaryPrecision
    /// to round a decimal fraction to this format. </summary>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security","CA2104",
      Justification="This PrecisionContext is immutable")]
    #endif
    public static readonly PrecisionContext CliDecimal =
      new PrecisionContext(96,Rounding.HalfEven,0,28,true);

  }
}