/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Numbers {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Numbers.EFloat"]/*'/>
  internal sealed class EFloat : IComparable<EFloat>,
  IEquatable<EFloat> {
    private readonly EInteger exponent;
    private readonly EInteger unsignedMantissa;
    private readonly int flags;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Exponent"]/*'/>
    public EInteger Exponent {
      get {
        return this.exponent;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.UnsignedMantissa"]/*'/>
    public EInteger UnsignedMantissa {
      get {
        return this.unsignedMantissa;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Mantissa"]/*'/>
    public EInteger Mantissa {
      get {
        return this.IsNegative ? (-(EInteger)this.unsignedMantissa) :
          this.unsignedMantissa;
      }
    }

    #region Equals and GetHashCode implementation
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.EqualsInternal(PeterO.Numbers.EFloat)"]/*'/>
    public bool EqualsInternal(EFloat otherValue) {
      if (otherValue == null) {
        return false;
      }
      return this.exponent.Equals(otherValue.exponent) &&
        this.unsignedMantissa.Equals(otherValue.unsignedMantissa) &&
        this.flags == otherValue.flags;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Equals(PeterO.Numbers.EFloat)"]/*'/>
    public bool Equals(EFloat other) {
      return this.EqualsInternal(other);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Equals(System.Object)"]/*'/>
    public override bool Equals(object obj) {
      return this.EqualsInternal(obj as EFloat);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.GetHashCode"]/*'/>
    public override int GetHashCode() {
      var hashCode = 403796923;
      unchecked {
        hashCode += 403797019 * this.exponent.GetHashCode();
        hashCode += 403797059 * this.unsignedMantissa.GetHashCode();
        hashCode += 403797127 * this.flags;
      }
      return hashCode;
    }
    #endregion

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CreateNaN(PeterO.Numbers.EInteger)"]/*'/>
    public static EFloat CreateNaN(EInteger diag) {
      return CreateNaN(diag, false, false, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CreateNaN(PeterO.Numbers.EInteger,System.Boolean,System.Boolean,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat CreateNaN(
      EInteger diag,
      bool signaling,
      bool negative,
      EContext ctx) {
      if (diag == null) {
        throw new ArgumentNullException("diag");
      }
      if (diag.Sign < 0) {
        throw new
  ArgumentException("Diagnostic information must be 0 or greater, was: " +
                    diag);
      }
      if (diag.IsZero && !negative) {
        return signaling ? SignalingNaN : NaN;
      }
      var flags = 0;
      if (negative) {
        flags |= BigNumberFlags.FlagNegative;
      }
      if (ctx != null && ctx.HasMaxPrecision) {
        flags |= BigNumberFlags.FlagQuietNaN;
        EFloat ef = CreateWithFlags(
          diag,
          EInteger.Zero,
          flags).RoundToPrecision(ctx);
        int newFlags = ef.flags;
        newFlags &= ~BigNumberFlags.FlagQuietNaN;
        newFlags |= signaling ? BigNumberFlags.FlagSignalingNaN :
          BigNumberFlags.FlagQuietNaN;
        return new EFloat(ef.unsignedMantissa, ef.exponent, newFlags);
      }
      flags |= signaling ? BigNumberFlags.FlagSignalingNaN :
        BigNumberFlags.FlagQuietNaN;
      return CreateWithFlags(diag, EInteger.Zero, flags);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Create(System.Int32,System.Int32)"]/*'/>
    public static EFloat Create(int mantissaSmall, int exponentSmall) {
      return Create((EInteger)mantissaSmall, (EInteger)exponentSmall);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Create(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'/>
    public static EFloat Create(
      EInteger mantissa,
      EInteger exponent) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      int sign = mantissa.Sign;
      return new EFloat(
        sign < 0 ? (-(EInteger)mantissa) : mantissa,
        exponent,
        (sign < 0) ? BigNumberFlags.FlagNegative : 0);
    }

    private EFloat(
      EInteger unsignedMantissa,
      EInteger exponent,
      int flags) {
      #if DEBUG
      if (unsignedMantissa == null) {
        throw new ArgumentNullException("unsignedMantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      if (unsignedMantissa.Sign < 0) {
        throw new ArgumentException("unsignedMantissa is less than 0.");
      }
      #endif
      this.unsignedMantissa = unsignedMantissa;
      this.exponent = exponent;
      this.flags = flags;
    }

    internal static EFloat CreateWithFlags(
      EInteger mantissa,
      EInteger exponent,
      int flags) {
      if (mantissa == null) {
        throw new ArgumentNullException("mantissa");
      }
      if (exponent == null) {
        throw new ArgumentNullException("exponent");
      }
      int sign = mantissa == null ? 0 : mantissa.Sign;
      return new EFloat(
        sign < 0 ? (-(EInteger)mantissa) : mantissa,
        exponent,
        flags);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,System.Int32,System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat FromString(
      string str,
      int offset,
      int length,
      EContext ctx) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return EDecimal.FromString(
        str,
        offset,
        length,
        ctx).ToExtendedFloat();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String)"]/*'/>
    public static EFloat FromString(string str) {
      return FromString(str, 0, str == null ? 0 : str.Length, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat FromString(string str, EContext ctx) {
      return FromString(str, 0, str == null ? 0 : str.Length, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromString(System.String,System.Int32,System.Int32)"]/*'/>
    public static EFloat FromString(string str, int offset, int length) {
      return FromString(str, offset, length, null);
    }

    private sealed class BinaryMathHelper : IRadixMathHelper<EFloat> {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetRadix"]/*'/>
      public int GetRadix() {
        return 2;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetSign(PeterO.Numbers.EFloat)"]/*'/>
      public int GetSign(EFloat value) {
        return value.Sign;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetMantissa(PeterO.Numbers.EFloat)"]/*'/>
      public EInteger GetMantissa(EFloat value) {
        return value.unsignedMantissa;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetExponent(PeterO.Numbers.EFloat)"]/*'/>
      public EInteger GetExponent(EFloat value) {
        return value.exponent;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.CreateShiftAccumulatorWithDigits(PeterO.Numbers.EInteger,System.Int32,System.Int32)"]/*'/>
      public IShiftAccumulator CreateShiftAccumulatorWithDigits(
        EInteger bigint,
        int lastDigit,
        int olderDigits) {
        return new BitShiftAccumulator(bigint, lastDigit, olderDigits);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.CreateShiftAccumulator(PeterO.Numbers.EInteger)"]/*'/>
      public IShiftAccumulator CreateShiftAccumulator(EInteger bigint) {
        return new BitShiftAccumulator(bigint, 0, 0);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.HasTerminatingRadixExpansion(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger)"]/*'/>
      public bool HasTerminatingRadixExpansion(EInteger num, EInteger den) {
        EInteger gcd = EInteger.GreatestCommonDivisor(num, den);
        if (gcd.IsZero) {
          return false;
        }
        den /= gcd;
        while (den.IsEven) {
          den >>= 1;
        }
        return den.Equals(EInteger.One);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.MultiplyByRadixPower(PeterO.Numbers.EInteger,PeterO.Numbers.FastInteger)"]/*'/>
      public EInteger MultiplyByRadixPower(
        EInteger bigint,
        FastInteger power) {
        EInteger tmpbigint = bigint;
        if (power.Sign <= 0) {
          return tmpbigint;
        }
        if (tmpbigint.Sign < 0) {
          tmpbigint = -tmpbigint;
          if (power.CanFitInInt32()) {
            tmpbigint = DecimalUtility.ShiftLeftInt(tmpbigint, power.AsInt32());
            tmpbigint = -tmpbigint;
          } else {
            tmpbigint = DecimalUtility.ShiftLeft(
              tmpbigint,
              power.AsBigInteger());
            tmpbigint = -tmpbigint;
          }
          return tmpbigint;
        }
        return power.CanFitInInt32() ? DecimalUtility.ShiftLeftInt(
          tmpbigint,
          power.AsInt32()) : DecimalUtility.ShiftLeft(
          tmpbigint,
          power.AsBigInteger());
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetFlags(PeterO.Numbers.EFloat)"]/*'/>
      public int GetFlags(EFloat value) {
        return value.flags;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.CreateNewWithFlags(PeterO.Numbers.EInteger,PeterO.Numbers.EInteger,System.Int32)"]/*'/>
      public EFloat CreateNewWithFlags(
        EInteger mantissa,
        EInteger exponent,
        int flags) {
        return EFloat.CreateWithFlags(mantissa, exponent, flags);
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.GetArithmeticSupport"]/*'/>
      public int GetArithmeticSupport() {
        return BigNumberFlags.FiniteAndNonFinite;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.BinaryMathHelper.ValueOf(System.Int32)"]/*'/>
      public EFloat ValueOf(int val) {
        return FromInt64(val);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToBigInteger"]/*'/>
    public EInteger ToBigInteger() {
      return this.ToBigIntegerInternal(false);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToBigIntegerExact"]/*'/>
    public EInteger ToBigIntegerExact() {
      return this.ToBigIntegerInternal(true);
    }

    private EInteger ToBigIntegerInternal(bool exact) {
      if (!this.IsFinite) {
        throw new OverflowException("Value is infinity or NaN");
      }
      if (this.IsZero) {
        return EInteger.Zero;
      }
      int expsign = this.Exponent.Sign;
      if (expsign == 0) {
        // Integer
        return this.Mantissa;
      }
      if (expsign > 0) {
        // Integer with trailing zeros
        EInteger curexp = this.Exponent;
        EInteger bigmantissa = this.Mantissa;
        if (bigmantissa.IsZero) {
          return bigmantissa;
        }
        bool neg = bigmantissa.Sign < 0;
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        bigmantissa = DecimalUtility.ShiftLeft(bigmantissa, curexp);
        if (neg) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      } else {
        EInteger bigmantissa = this.Mantissa;
        FastInteger bigexponent = FastInteger.FromBig(this.Exponent).Negate();
        bigmantissa = bigmantissa.Abs();
        var acc = new BitShiftAccumulator(bigmantissa, 0, 0);
        acc.ShiftRight(bigexponent);
        if (exact && (acc.LastDiscardedDigit != 0 || acc.OlderDiscardedDigits !=
                    0)) {
          // Some digits were discarded
          throw new ArithmeticException("Not an exact integer");
        }
        bigmantissa = acc.ShiftedInt;
        if (this.IsNegative) {
          bigmantissa = -bigmantissa;
        }
        return bigmantissa;
      }
    }

    private static readonly EInteger ValueOneShift23 = EInteger.One << 23;
    private static readonly EInteger ValueOneShift52 = EInteger.One << 52;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToSingle"]/*'/>
    public float ToSingle() {
      if (this.IsPositiveInfinity()) {
        return Single.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return Single.NegativeInfinity;
      }
      if (this.IsNaN()) {
        var nan = 0x7f800000;
        if (this.IsNegative) {
          nan |= unchecked((int)(1 << 31));
        }
        // IsQuietNaN(): the quiet bit for X86 at least
        // Not IsQuietNaN(): not really the signaling bit, but done to keep
        // the mantissa from being zero
        nan |= this.IsQuietNaN() ? 0x400000 : 0x200000;
        if (!this.UnsignedMantissa.IsZero) {
          // Transfer diagnostic information
          EInteger bigdata = this.UnsignedMantissa % (EInteger)0x200000;
          nan |= (int)bigdata;
        }
        return BitConverter.ToSingle(BitConverter.GetBytes(nan), 0);
      }
      if (this.IsNegative && this.IsZero) {
        return BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0);
      }
      EInteger bigmant = this.unsignedMantissa.Abs();
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      var bitLeftmost = 0;
      var bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.IsZero) {
        return 0.0f;
      }
      var smallmant = 0;
      FastInteger fastSmallMant;
      if (bigmant.CompareTo(ValueOneShift23) < 0) {
        smallmant = (int)bigmant;
        var exponentchange = 0;
        while (smallmant < (1 << 23)) {
          smallmant <<= 1;
          ++exponentchange;
        }
        bigexponent.SubtractInt(exponentchange);
        fastSmallMant = new FastInteger(smallmant);
      } else {
        var accum = new BitShiftAccumulator(bigmant, 0, 0);
        accum.ShiftToDigitsInt(24);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        fastSmallMant = accum.ShiftedIntFast;
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                    !fastSmallMant.IsEvenNumber)) {
        fastSmallMant.Increment();
        if (fastSmallMant.CompareToInt(1 << 24) == 0) {
          fastSmallMant = new FastInteger(1 << 23);
          bigexponent.Increment();
        }
      }
      var subnormal = false;
      if (bigexponent.CompareToInt(104) > 0) {
        // exponent too big
        return this.IsNegative ? Single.NegativeInfinity :
          Single.PositiveInfinity;
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        BitShiftAccumulator accum =
          BitShiftAccumulator.FromInt32(fastSmallMant.AsInt32());
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-149).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        fastSmallMant = accum.ShiftedIntFast;
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                    !fastSmallMant.IsEvenNumber)) {
          fastSmallMant.Increment();
          if (fastSmallMant.CompareToInt(1 << 24) == 0) {
            fastSmallMant = new FastInteger(1 << 23);
            bigexponent.Increment();
          }
        }
      }
      if (bigexponent.CompareToInt(-149) < 0) {
        // exponent too small, so return zero
        return this.IsNegative ?
          BitConverter.ToSingle(BitConverter.GetBytes((int)1 << 31), 0) :
          BitConverter.ToSingle(BitConverter.GetBytes((int)0), 0);
      } else {
        int smallexponent = bigexponent.AsInt32();
        smallexponent += 150;
        int smallmantissa = ((int)fastSmallMant.AsInt32()) & 0x7fffff;
        if (!subnormal) {
          smallmantissa |= smallexponent << 23;
        }
        if (this.IsNegative) {
          smallmantissa |= 1 << 31;
        }
        return BitConverter.ToSingle(
          BitConverter.GetBytes((int)smallmantissa),
          0);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToDouble"]/*'/>
    public double ToDouble() {
      if (this.IsPositiveInfinity()) {
        return Double.PositiveInfinity;
      }
      if (this.IsNegativeInfinity()) {
        return Double.NegativeInfinity;
      }
      if (this.IsNaN()) {
        int[] nan = { 0, 0x7ff00000 };
        if (this.IsNegative) {
          nan[1] |= unchecked((int)(1 << 31));
        }
        // 0x40000 is not really the signaling bit, but done to keep
        // the mantissa from being zero
        if (this.IsQuietNaN()) {
          nan[1] |= 0x80000;
        } else {
          nan[1] |= 0x40000;
        }
        if (!this.UnsignedMantissa.IsZero) {
          // Copy diagnostic information
          int[] words = FastInteger.GetLastWords(this.UnsignedMantissa, 2);
          nan[0] = words[0];
          nan[1] = words[1] & 0x3ffff;
        }
        return Extras.IntegersToDouble(nan);
      }
      if (this.IsNegative && this.IsZero) {
        return Extras.IntegersToDouble(new[] { 0, unchecked((int)(1 << 31)) });
      }
      EInteger bigmant = this.unsignedMantissa.Abs();
      FastInteger bigexponent = FastInteger.FromBig(this.exponent);
      var bitLeftmost = 0;
      var bitsAfterLeftmost = 0;
      if (this.unsignedMantissa.IsZero) {
        return 0.0d;
      }
      int[] mantissaBits;
      if (bigmant.CompareTo(ValueOneShift52) < 0) {
        mantissaBits = FastInteger.GetLastWords(bigmant, 2);
        // This will be an infinite loop if both elements
        // of the bits array are 0, but the check for
        // 0 was already done above
        while (!DecimalUtility.HasBitSet(mantissaBits, 52)) {
          DecimalUtility.ShiftLeftOne(mantissaBits);
          bigexponent.Decrement();
        }
      } else {
        var accum = new BitShiftAccumulator(bigmant, 0, 0);
        accum.ShiftToDigitsInt(53);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits = FastInteger.GetLastWords(accum.ShiftedInt, 2);
      }
      // Round half-even
      if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                    DecimalUtility.HasBitSet(mantissaBits, 0))) {
        // Add 1 to the bits
        mantissaBits[0] = unchecked((int)(mantissaBits[0] + 1));
        if (mantissaBits[0] == 0) {
          mantissaBits[1] = unchecked((int)(mantissaBits[1] + 1));
        }
        if (mantissaBits[0] == 0 &&
            mantissaBits[1] == (1 << 21)) {  // if mantissa is now 2^53
          mantissaBits[1] >>= 1;  // change it to 2^52
          bigexponent.Increment();
        }
      }
      var subnormal = false;
      if (bigexponent.CompareToInt(971) > 0) {
        // exponent too big
        return this.IsNegative ? Double.NegativeInfinity :
          Double.PositiveInfinity;
      }
      if (bigexponent.CompareToInt(-1074) < 0) {
        // subnormal
        subnormal = true;
        // Shift while number remains subnormal
        var accum = new BitShiftAccumulator(
          FastInteger.WordsToBigInteger(mantissaBits),
          0,
          0);
        FastInteger fi = FastInteger.Copy(bigexponent).SubtractInt(-1074).Abs();
        accum.ShiftRight(fi);
        bitsAfterLeftmost = accum.OlderDiscardedDigits;
        bitLeftmost = accum.LastDiscardedDigit;
        bigexponent.Add(accum.DiscardedDigitCount);
        mantissaBits = FastInteger.GetLastWords(accum.ShiftedInt, 2);
        // Round half-even
        if (bitLeftmost > 0 && (bitsAfterLeftmost > 0 ||
                    DecimalUtility.HasBitSet(mantissaBits, 0))) {
          // Add 1 to the bits
          mantissaBits[0] = unchecked((int)(mantissaBits[0] + 1));
          if (mantissaBits[0] == 0) {
            mantissaBits[1] = unchecked((int)(mantissaBits[1] + 1));
          }
          if (mantissaBits[0] == 0 &&
              mantissaBits[1] == (1 << 21)) {  // if mantissa is now 2^53
            mantissaBits[1] >>= 1;  // change it to 2^52
            bigexponent.Increment();
          }
        }
      }
      if (bigexponent.CompareToInt(-1074) < 0) {
        // exponent too small, so return zero
        return this.IsNegative ?
          Extras.IntegersToDouble(new[] { 0, unchecked((int)0x80000000) }) :
          0.0d;
      }
      bigexponent.AddInt(1075);
      // Clear the high bits where the exponent and sign are
      mantissaBits[1] &= 0xfffff;
      if (!subnormal) {
        int smallexponent = bigexponent.AsInt32() << 20;
        mantissaBits[1] |= smallexponent;
      }
      if (this.IsNegative) {
        mantissaBits[1] |= unchecked((int)(1 << 31));
      }
      return Extras.IntegersToDouble(mantissaBits);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromSingle(System.Single)"]/*'/>
    public static EFloat FromSingle(float flt) {
      int value = BitConverter.ToInt32(BitConverter.GetBytes((float)flt), 0);
      bool neg = (value >> 31) != 0;
      var floatExponent = (int)((value >> 23) & 0xff);
      int valueFpMantissa = value & 0x7fffff;
      EInteger bigmant;
      if (floatExponent == 255) {
        if (valueFpMantissa == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (valueFpMantissa & 0x400000) != 0;
        valueFpMantissa &= 0x1fffff;
        bigmant = (EInteger)valueFpMantissa;
        value = (neg ? BigNumberFlags.FlagNegative : 0) | (quiet ?
                BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN);
        if (bigmant.IsZero) {
          return quiet ? NaN : SignalingNaN;
        }
        return CreateWithFlags(
          bigmant,
          EInteger.Zero,
          value);
      }
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        valueFpMantissa |= 1 << 23;
      }
      if (valueFpMantissa == 0) {
        return neg ? EFloat.NegativeZero : EFloat.Zero;
      }
      while ((valueFpMantissa & 1) == 0) {
        ++floatExponent;
        valueFpMantissa >>= 1;
      }
      if (neg) {
        valueFpMantissa = -valueFpMantissa;
      }
      bigmant = (EInteger)valueFpMantissa;
      return EFloat.Create(
        bigmant,
        (EInteger)(floatExponent - 150));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromBigInteger(PeterO.Numbers.EInteger)"]/*'/>
    public static EFloat FromBigInteger(EInteger bigint) {
      return EFloat.Create(bigint, EInteger.Zero);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromInt64(System.Int64)"]/*'/>
    public static EFloat FromInt64(long valueSmall) {
      var bigint = (EInteger)valueSmall;
      return EFloat.Create(bigint, EInteger.Zero);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromInt32(System.Int32)"]/*'/>
    public static EFloat FromInt32(int valueSmaller) {
      var bigint = (EInteger)valueSmaller;
      return EFloat.Create(bigint, EInteger.Zero);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.FromDouble(System.Double)"]/*'/>
    public static EFloat FromDouble(double dbl) {
      int[] value = Extras.DoubleToIntegers(dbl);
      var floatExponent = (int)((value[1] >> 20) & 0x7ff);
      bool neg = (value[1] >> 31) != 0;
      if (floatExponent == 2047) {
        if ((value[1] & 0xfffff) == 0 && value[0] == 0) {
          return neg ? NegativeInfinity : PositiveInfinity;
        }
        // Treat high bit of mantissa as quiet/signaling bit
        bool quiet = (value[1] & 0x80000) != 0;
        value[1] &= 0x3ffff;
        EInteger info = FastInteger.WordsToBigInteger(value);
        if (info.IsZero) {
          return quiet ? NaN : SignalingNaN;
        }
        value[0] = (neg ? BigNumberFlags.FlagNegative : 0) |
       (quiet ? BigNumberFlags.FlagQuietNaN : BigNumberFlags.FlagSignalingNaN);
        return CreateWithFlags(
          info,
          EInteger.Zero,
          value[0]);
      }
      value[1] &= 0xfffff;  // Mask out the exponent and sign
      if (floatExponent == 0) {
        ++floatExponent;
      } else {
        value[1] |= 0x100000;
      }
      if ((value[1] | value[0]) != 0) {
      floatExponent += DecimalUtility.ShiftAwayTrailingZerosTwoElements(value);
      } else {
        return neg ? EFloat.NegativeZero : EFloat.Zero;
      }
      return CreateWithFlags(
        FastInteger.WordsToBigInteger(value),
        (EInteger)(floatExponent - 1075),
        neg ? BigNumberFlags.FlagNegative : 0);
    }

    public EDecimal ToExtendedDecimal() {
      return EDecimal.FromExtendedFloat(this);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToString"]/*'/>
    public override string ToString() {
      return EDecimal.FromExtendedFloat(this).ToString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToEngineeringString"]/*'/>
    public string ToEngineeringString() {
      return this.ToExtendedDecimal().ToEngineeringString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ToPlainString"]/*'/>
    public string ToPlainString() {
      return this.ToExtendedDecimal().ToPlainString();
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.One"]/*'/>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
    #endif
    public static readonly EFloat One =
      EFloat.Create(EInteger.One, EInteger.Zero);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.Zero"]/*'/>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
    #endif
    public static readonly EFloat Zero =
      EFloat.Create(EInteger.Zero, EInteger.Zero);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NegativeZero"]/*'/>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
    #endif
    public static readonly EFloat NegativeZero = CreateWithFlags(
      EInteger.Zero,
      EInteger.Zero,
      BigNumberFlags.FlagNegative);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.Ten"]/*'/>
    #if CODE_ANALYSIS
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Security",
      "CA2104",
      Justification = "ExtendedFloat is immutable")]
    #endif

    public static readonly EFloat Ten =
      EFloat.Create((EInteger)10, EInteger.Zero);

    //----------------------------------------------------------------

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NaN"]/*'/>
    public static readonly EFloat NaN = CreateWithFlags(
      EInteger.Zero,
      EInteger.Zero,
      BigNumberFlags.FlagQuietNaN);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.SignalingNaN"]/*'/>
    public static readonly EFloat SignalingNaN = CreateWithFlags(
      EInteger.Zero,
      EInteger.Zero,
      BigNumberFlags.FlagSignalingNaN);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.PositiveInfinity"]/*'/>
    public static readonly EFloat PositiveInfinity = CreateWithFlags(
      EInteger.Zero,
      EInteger.Zero,
      BigNumberFlags.FlagInfinity);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Numbers.EFloat.NegativeInfinity"]/*'/>
    public static readonly EFloat NegativeInfinity = CreateWithFlags(
      EInteger.Zero,
      EInteger.Zero,
      BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsNegativeInfinity"]/*'/>
    public bool IsNegativeInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) ==
        (BigNumberFlags.FlagInfinity | BigNumberFlags.FlagNegative);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsPositiveInfinity"]/*'/>
    public bool IsPositiveInfinity() {
      return (this.flags & (BigNumberFlags.FlagInfinity |
                BigNumberFlags.FlagNegative)) == BigNumberFlags.FlagInfinity;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsNaN"]/*'/>
    public bool IsNaN() {
      return (this.flags & (BigNumberFlags.FlagQuietNaN |
                    BigNumberFlags.FlagSignalingNaN)) != 0;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsInfinity"]/*'/>
    public bool IsInfinity() {
      return (this.flags & BigNumberFlags.FlagInfinity) != 0;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsFinite"]/*'/>
    public bool IsFinite {
      get {
        return (this.flags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNaN)) == 0;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsNegative"]/*'/>
    public bool IsNegative {
      get {
        return (this.flags & BigNumberFlags.FlagNegative) != 0;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsQuietNaN"]/*'/>
    public bool IsQuietNaN() {
      return (this.flags & BigNumberFlags.FlagQuietNaN) != 0;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.IsSignalingNaN"]/*'/>
    public bool IsSignalingNaN() {
      return (this.flags & BigNumberFlags.FlagSignalingNaN) != 0;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.Sign"]/*'/>
    public int Sign {
      get {
        return (((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
                this.unsignedMantissa.IsZero) ? 0 :
          (((this.flags & BigNumberFlags.FlagNegative) != 0) ? -1 : 1);
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Numbers.EFloat.IsZero"]/*'/>
    public bool IsZero {
      get {
        return ((this.flags & BigNumberFlags.FlagSpecial) == 0) &&
          this.unsignedMantissa.IsZero;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Abs"]/*'/>
    public EFloat Abs() {
      return this.Abs(null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Negate"]/*'/>
    public EFloat Negate() {
      return this.Negate(null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Divide(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat Divide(EFloat divisor) {
      return this.Divide(
        divisor,
        EContext.ForRounding(ERounding.None));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToSameExponent(PeterO.Numbers.EFloat,PeterO.Numbers.ERounding)"]/*'/>
    public EFloat DivideToSameExponent(
      EFloat divisor,
      ERounding rounding) {
      return this.DivideToExponent(
        divisor,
        this.exponent,
        EContext.ForRounding(rounding));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerNaturalScale(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat DivideToIntegerNaturalScale(
      EFloat divisor) {
      return this.DivideToIntegerNaturalScale(
        divisor,
        EContext.ForRounding(ERounding.Down));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Reduce(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Reduce(EContext ctx) {
      return MathValue.Reduce(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNaturalScale(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat RemainderNaturalScale(
      EFloat divisor) {
      return this.RemainderNaturalScale(divisor, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RemainderNaturalScale(
      EFloat divisor,
      EContext ctx) {
      return this.Subtract(
        this.DivideToIntegerNaturalScale(divisor, ctx).Multiply(divisor, null),
        null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,System.Int64,PeterO.Numbers.EContext)"]/*'/>
    public EFloat DivideToExponent(
      EFloat divisor,
      long desiredExponentSmall,
      EContext ctx) {
      return this.DivideToExponent(
        divisor,
        (EInteger)desiredExponentSmall,
        ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Divide(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Divide(
      EFloat divisor,
      EContext ctx) {
      return MathValue.Divide(this, divisor, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,System.Int64,PeterO.Numbers.ERounding)"]/*'/>
    public EFloat DivideToExponent(
      EFloat divisor,
      long desiredExponentSmall,
      ERounding rounding) {
      return this.DivideToExponent(
        divisor,
        (EInteger)desiredExponentSmall,
        EContext.ForRounding(rounding));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat DivideToExponent(
      EFloat divisor,
      EInteger exponent,
      EContext ctx) {
      return MathValue.DivideToExponent(this, divisor, exponent, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToExponent(PeterO.Numbers.EFloat,PeterO.Numbers.EInteger,PeterO.Numbers.ERounding)"]/*'/>
    public EFloat DivideToExponent(
      EFloat divisor,
      EInteger desiredExponent,
      ERounding rounding) {
      return this.DivideToExponent(
        divisor,
        desiredExponent,
        EContext.ForRounding(rounding));
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Abs(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Abs(EContext context) {
      return MathValue.Abs(this, context);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Negate(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Negate(EContext context) {
      return MathValue.Negate(this, context);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Add(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat Add(EFloat otherValue) {
      return this.Add(otherValue, EContext.Unlimited);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Subtract(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat Subtract(EFloat otherValue) {
      return this.Subtract(otherValue, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Subtract(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Subtract(
      EFloat otherValue,
      EContext ctx) {
      if (otherValue == null) {
        throw new ArgumentNullException("otherValue");
      }
      EFloat negated = otherValue;
      if ((otherValue.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = otherValue.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          otherValue.unsignedMantissa,
          otherValue.exponent,
          newflags);
      }
      return this.Add(negated, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Multiply(PeterO.Numbers.EFloat)"]/*'/>
    public EFloat Multiply(EFloat otherValue) {
      return this.Multiply(otherValue, EContext.Unlimited);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndAdd(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]/*'/>
    public EFloat MultiplyAndAdd(
      EFloat multiplicand,
      EFloat augend) {
      return this.MultiplyAndAdd(multiplicand, augend, null);
    }
    //----------------------------------------------------------------
    private static readonly IRadixMath<EFloat> MathValue = new
      TrappableRadixMath<EFloat>(
        new ExtendedOrSimpleRadixMath<EFloat>(new BinaryMathHelper()));

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat DivideToIntegerNaturalScale(
      EFloat divisor,
      EContext ctx) {
      return MathValue.DivideToIntegerNaturalScale(this, divisor, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideToIntegerZeroScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat DivideToIntegerZeroScale(
      EFloat divisor,
      EContext ctx) {
      return MathValue.DivideToIntegerZeroScale(this, divisor, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Remainder(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Remainder(
      EFloat divisor,
      EContext ctx) {
      return MathValue.Remainder(this, divisor, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RemainderNear(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RemainderNear(
      EFloat divisor,
      EContext ctx) {
      return MathValue.RemainderNear(this, divisor, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextMinus(PeterO.Numbers.EContext)"]/*'/>
    public EFloat NextMinus(EContext ctx) {
      return MathValue.NextMinus(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextPlus(PeterO.Numbers.EContext)"]/*'/>
    public EFloat NextPlus(EContext ctx) {
      return MathValue.NextPlus(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.NextToward(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat NextToward(
      EFloat otherValue,
      EContext ctx) {
      return MathValue.NextToward(this, otherValue, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Max(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat Max(
      EFloat first,
      EFloat second,
      EContext ctx) {
      return MathValue.Max(first, second, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Min(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat Min(
      EFloat first,
      EFloat second,
      EContext ctx) {
      return MathValue.Min(first, second, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MaxMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat MaxMagnitude(
      EFloat first,
      EFloat second,
      EContext ctx) {
      return MathValue.MaxMagnitude(first, second, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MinMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public static EFloat MinMagnitude(
      EFloat first,
      EFloat second,
      EContext ctx) {
      return MathValue.MinMagnitude(first, second, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Max(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]/*'/>
    public static EFloat Max(
      EFloat first,
      EFloat second) {
      return Max(first, second, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Min(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]/*'/>
    public static EFloat Min(
      EFloat first,
      EFloat second) {
      return Min(first, second, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MaxMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]/*'/>
    public static EFloat MaxMagnitude(
      EFloat first,
      EFloat second) {
      return MaxMagnitude(first, second, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MinMagnitude(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat)"]/*'/>
    public static EFloat MinMagnitude(
      EFloat first,
      EFloat second) {
      return MinMagnitude(first, second, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareTo(PeterO.Numbers.EFloat)"]/*'/>
    public int CompareTo(EFloat other) {
      return MathValue.CompareTo(this, other);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareToWithContext(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat CompareToWithContext(
      EFloat other,
      EContext ctx) {
      return MathValue.CompareToWithContext(this, other, false, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.CompareToSignal(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat CompareToSignal(
      EFloat other,
      EContext ctx) {
      return MathValue.CompareToWithContext(this, other, true, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Add(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Add(
      EFloat otherValue,
      EContext ctx) {
      return MathValue.Add(this, otherValue, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Quantize(
      EInteger desiredExponent,
      EContext ctx) {
      return this.Quantize(
        EFloat.Create(EInteger.One, desiredExponent),
        ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Quantize(
      int desiredExponentSmall,
      EContext ctx) {
      return this.Quantize(
        EFloat.Create(EInteger.One, (EInteger)desiredExponentSmall),
        ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Quantize(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Quantize(
      EFloat otherValue,
      EContext ctx) {
      return MathValue.Quantize(this, otherValue, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToIntegralExact(PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToIntegralExact(EContext ctx) {
      return MathValue.RoundToExponentExact(this, EInteger.Zero, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToIntegralNoRoundedFlag(PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToIntegralNoRoundedFlag(EContext ctx) {
      return MathValue.RoundToExponentNoRoundedFlag(this, EInteger.Zero, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponentExact(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToExponentExact(
      EInteger exponent,
      EContext ctx) {
      return MathValue.RoundToExponentExact(this, exponent, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponent(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToExponent(
      EInteger exponent,
      EContext ctx) {
      return MathValue.RoundToExponentSimple(this, exponent, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponentExact(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToExponentExact(
      int exponentSmall,
      EContext ctx) {
      return this.RoundToExponentExact((EInteger)exponentSmall, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToExponent(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToExponent(
      int exponentSmall,
      EContext ctx) {
      return this.RoundToExponent((EInteger)exponentSmall, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Multiply(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Multiply(
      EFloat op,
      EContext ctx) {
      return MathValue.Multiply(this, op, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndAdd(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MultiplyAndAdd(
      EFloat op,
      EFloat augend,
      EContext ctx) {
      return MathValue.MultiplyAndAdd(this, op, augend, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MultiplyAndSubtract(PeterO.Numbers.EFloat,PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MultiplyAndSubtract(
      EFloat op,
      EFloat subtrahend,
      EContext ctx) {
      if (op == null) {
        throw new ArgumentNullException("op");
      }
      if (subtrahend == null) {
        throw new ArgumentNullException("subtrahend");
      }
      EFloat negated = subtrahend;
      if ((subtrahend.flags & BigNumberFlags.FlagNaN) == 0) {
        int newflags = subtrahend.flags ^ BigNumberFlags.FlagNegative;
        negated = CreateWithFlags(
          subtrahend.unsignedMantissa,
          subtrahend.exponent,
          newflags);
      }
      return MathValue.MultiplyAndAdd(this, op, negated, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.RoundToPrecision(PeterO.Numbers.EContext)"]/*'/>
    public EFloat RoundToPrecision(EContext ctx) {
      return MathValue.RoundToPrecision(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Plus(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Plus(EContext ctx) {
      return MathValue.Plus(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.SquareRoot(PeterO.Numbers.EContext)"]/*'/>
    public EFloat SquareRoot(EContext ctx) {
      return MathValue.SquareRoot(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Exp(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Exp(EContext ctx) {
      return MathValue.Exp(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Log(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Log(EContext ctx) {
      return MathValue.Ln(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Log10(PeterO.Numbers.EContext)"]/*'/>
    public EFloat Log10(EContext ctx) {
      return MathValue.Log10(this, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Pow(EFloat exponent, EContext ctx) {
      return MathValue.Power(this, exponent, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat Pow(int exponentSmall, EContext ctx) {
      return this.Pow(EFloat.FromInt64(exponentSmall), ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Pow(System.Int32)"]/*'/>
    public EFloat Pow(int exponentSmall) {
      return this.Pow(EFloat.FromInt64(exponentSmall), null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.PI(PeterO.Numbers.EContext)"]/*'/>
    public static EFloat PI(EContext ctx) {
      return MathValue.Pi(ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(System.Int32)"]/*'/>
    public EFloat MovePointLeft(int places) {
      return this.MovePointLeft((EInteger)places, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MovePointLeft(int places, EContext ctx) {
      return this.MovePointLeft((EInteger)places, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(PeterO.Numbers.EInteger)"]/*'/>
    public EFloat MovePointLeft(EInteger bigPlaces) {
      return this.MovePointLeft(bigPlaces, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointLeft(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MovePointLeft(
EInteger bigPlaces,
EContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      return (!this.IsFinite) ? this.RoundToPrecision(ctx) :
        this.MovePointRight(-(EInteger)bigPlaces, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(System.Int32)"]/*'/>
    public EFloat MovePointRight(int places) {
      return this.MovePointRight((EInteger)places, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MovePointRight(int places, EContext ctx) {
      return this.MovePointRight((EInteger)places, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(PeterO.Numbers.EInteger)"]/*'/>
    public EFloat MovePointRight(EInteger bigPlaces) {
      return this.MovePointRight(bigPlaces, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.MovePointRight(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat MovePointRight(
EInteger bigPlaces,
EContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      if (!this.IsFinite) {
        return this.RoundToPrecision(ctx);
      }
      EInteger bigExp = this.Exponent;
      bigExp += bigPlaces;
      if (bigExp.Sign > 0) {
        EInteger mant = DecimalUtility.ShiftLeft(
          this.unsignedMantissa,
          bigExp);
        return CreateWithFlags(
mant,
EInteger.Zero,
this.flags).RoundToPrecision(ctx);
      }
      return CreateWithFlags(
        this.unsignedMantissa,
        bigExp,
        this.flags).RoundToPrecision(ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(System.Int32)"]/*'/>
    public EFloat ScaleByPowerOfTwo(int places) {
      return this.ScaleByPowerOfTwo((EInteger)places, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(System.Int32,PeterO.Numbers.EContext)"]/*'/>
    public EFloat ScaleByPowerOfTwo(int places, EContext ctx) {
      return this.ScaleByPowerOfTwo((EInteger)places, ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(PeterO.Numbers.EInteger)"]/*'/>
    public EFloat ScaleByPowerOfTwo(EInteger bigPlaces) {
      return this.ScaleByPowerOfTwo(bigPlaces, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.ScaleByPowerOfTwo(PeterO.Numbers.EInteger,PeterO.Numbers.EContext)"]/*'/>
    public EFloat ScaleByPowerOfTwo(
EInteger bigPlaces,
EContext ctx) {
      if (bigPlaces.IsZero) {
        return this.RoundToPrecision(ctx);
      }
      if (!this.IsFinite) {
        return this.RoundToPrecision(ctx);
      }
      EInteger bigExp = this.Exponent;
      bigExp += bigPlaces;
      return CreateWithFlags(
        this.unsignedMantissa,
        bigExp,
        this.flags).RoundToPrecision(ctx);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Precision"]/*'/>
    public EInteger Precision() {
      if (!this.IsFinite) {
 return EInteger.Zero;
}
      if (this.IsZero) {
 return EInteger.One;
}
      int bitlen = this.unsignedMantissa.bitLength();
      return (EInteger)bitlen;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.Ulp"]/*'/>
    public EFloat Ulp() {
      return (!this.IsFinite) ? EFloat.One :
        EFloat.Create(EInteger.One, this.exponent);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideAndRemainderNaturalScale(PeterO.Numbers.EFloat)"]/*'/>
 public EFloat[] DivideAndRemainderNaturalScale(EFloat divisor) {
      return this.DivideAndRemainderNaturalScale(divisor, null);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Numbers.EFloat.DivideAndRemainderNaturalScale(PeterO.Numbers.EFloat,PeterO.Numbers.EContext)"]/*'/>
    public EFloat[] DivideAndRemainderNaturalScale(
      EFloat divisor,
      EContext ctx) {
      var result = new EFloat[2];
      result[0] = this.DivideToIntegerNaturalScale(divisor, ctx);
      result[1] = this.Subtract(
        result[0].Multiply(divisor, null),
        null);
      return result;
    }
  }
}
