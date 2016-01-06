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
    /// path='docs/doc[@name="T:PeterO.Numbers.RadixMath`1"]/*'/>
  internal class RadixMath<T> : IRadixMath<T> {
    private const int IntegerModeFixedScale = 1;
    private const int IntegerModeRegular = 0;
    private static readonly EInteger ValueMinusOne = EInteger.Zero -
      EInteger.One;

    private readonly IRadixMathHelper<T> helper;
    private readonly int support;
    private readonly int thisRadix;

    public RadixMath(IRadixMathHelper<T> helper) {
      this.helper = helper;
      this.support = helper.GetArithmeticSupport();
      this.thisRadix = helper.GetRadix();
    }

    public T Abs(T value, EContext ctx) {
      int flags = this.helper.GetFlags(value);
      return ((flags & BigNumberFlags.FlagSignalingNaN) != 0) ?
        this.SignalingNaNInvalid(value, ctx) : (
          ((flags & BigNumberFlags.FlagQuietNaN) != 0) ?
          this.ReturnQuietNaN(
value,
ctx) : (((flags &
             BigNumberFlags.FlagNegative) != 0) ? this.RoundToPrecision(
             this.helper.CreateNewWithFlags(this.helper.GetMantissa(value), this.helper.GetExponent(value), flags & ~BigNumberFlags.FlagNegative),
             ctx) : this.RoundToPrecision(
value,
ctx)));
    }

    public T Add(T thisValue, T other, EContext ctx) {
      if ((object)thisValue == null) {
        throw new ArgumentNullException("thisValue");
      }
      if ((object)other == null) {
        throw new ArgumentNullException("other");
      }
      return this.AddEx(thisValue, other, ctx, false);
    }

    private T AddExDiffExp(
T thisValue,
T other,
int thisFlags,
int otherFlags,
EContext ctx,
int expcmp,
bool roundToOperandPrecision) {
      T retval = default(T);
      // choose the minimum exponent
      T op1 = thisValue;
      T op2 = other;
      EInteger op1MantAbs = this.helper.GetMantissa(thisValue);
      EInteger op2MantAbs = this.helper.GetMantissa(other);
      EInteger op1Exponent = this.helper.GetExponent(op1);
      EInteger op2Exponent = this.helper.GetExponent(op2);
      EInteger resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
      if (ctx != null && ctx.HasMaxPrecision && ctx.Precision.Sign > 0) {
        FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
        FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);
        FastInteger expdiff =
          FastInteger.Copy(fastOp1Exp).Subtract(fastOp2Exp).Abs();
        // Check if exponent difference is too big for
        // radix-power calculation to work quickly
        FastInteger fastPrecision = FastInteger.FromBig(ctx.Precision);
        // If exponent difference is greater than the precision
        if (expdiff.CompareTo(fastPrecision) > 0) {
          int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if (!op2MantAbs.IsZero) {
              // first operand's exponent is less
              // and second operand isn't zero
              // second mantissa will be shifted by the exponent
              // difference
              // _________________________111111111111|_
              // ___222222222222222|____________________
              FastInteger digitLength1 =
                this.helper.CreateShiftAccumulator(op1MantAbs)
                .GetDigitLength();
              if
                (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2)
              .CompareTo(fastOp2Exp) < 0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp2Exp).SubtractInt(4)
                  .Subtract(digitLength1).SubtractBig(ctx.Precision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  // Can be treated as almost zero
                  bool sameSign = this.helper.GetSign(thisValue) ==
                  this.helper.GetSign(other);
                  bool oneOpIsZero = op1MantAbs.IsZero;
                  FastInteger digitLength2 =
                  this.helper.CreateShiftAccumulator(op2MantAbs)
                  .GetDigitLength();
                  if (digitLength2.CompareTo(fastPrecision) < 0) {
                    // Second operand's precision too short
                    FastInteger precisionDiff =
                    FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                    if (!oneOpIsZero && !sameSign) {
                    precisionDiff.AddInt(2);
                    }
                    op2MantAbs = this.TryMultiplyByRadixPower(
                    op2MantAbs,
                    precisionDiff);
                    if (op2MantAbs == null) {
                    return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                    }
                    EInteger bigintTemp = precisionDiff.AsBigInteger();
                    op2Exponent -= (EInteger)bigintTemp;
                    if (!oneOpIsZero && !sameSign) {
                    op2MantAbs -= EInteger.One;
                    }
                    other = this.helper.CreateNewWithFlags(
          op2MantAbs,
          op2Exponent,
          this.helper.GetFlags(other));
                    FastInteger shift =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    if (oneOpIsZero && ctx != null && ctx.HasFlags) {
                    ctx.Flags |= EContext.FlagRounded;
                    }
                    // DebugUtility.Log("Second op's prec too short:
                    // op2MantAbs=" + op2MantAbs + " precdiff= " +
                    // (precisionDiff));
                    return this.RoundToPrecisionInternal(
          other,
           (oneOpIsZero || sameSign) ? 0 : 1,
           (oneOpIsZero && !sameSign) ? 0 : 1,
           shift,
           false,
                    ctx);
                  }
                  if (!oneOpIsZero && !sameSign) {
                    op2MantAbs = this.TryMultiplyByRadixPower(
                    op2MantAbs,
                    new FastInteger(2));
                    if (op2MantAbs == null) {
                    return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                    }
                    op2Exponent -= (EInteger)2;
                    op2MantAbs -= EInteger.One;
                    other = this.helper.CreateNewWithFlags(
                    op2MantAbs,
                    op2Exponent,
                    this.helper.GetFlags(other));
                    FastInteger shift =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    return this.RoundToPrecisionInternal(
          other,
          0,
          0,
          shift,
          false,
          ctx);
                  } else {
                    FastInteger shift2 =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    if (!sameSign && ctx != null && ctx.HasFlags) {
                    ctx.Flags |= EContext.FlagRounded;
                    }
                    return this.RoundToPrecisionInternal(
                    other,
                    0,
                    sameSign ? 1 : 0,
                    shift2,
                    false,
                    ctx);
                  }
                }
              }
            }
          } else if (expcmp2 > 0) {
            if (!op1MantAbs.IsZero) {
              // first operand's exponent is greater
              // and first operand isn't zero
              // first mantissa will be shifted by the exponent
              // difference
              // __111111111111|
              // ____________________222222222222222|
              FastInteger digitLength2 =
                this.helper.CreateShiftAccumulator(op2MantAbs)
                .GetDigitLength();
              if
                (FastInteger.Copy(fastOp2Exp).Add(digitLength2).AddInt(2)
              .CompareTo(fastOp1Exp) < 0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp1Exp).SubtractInt(4)
                .Subtract(digitLength2).SubtractBig(ctx.Precision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  // Can be treated as almost zero
                  bool sameSign = this.helper.GetSign(thisValue) ==
                  this.helper.GetSign(other);
                  bool oneOpIsZero = op2MantAbs.IsZero;
                  digitLength2 = this.helper.CreateShiftAccumulator(op1MantAbs)
                    .GetDigitLength();
                  if (digitLength2.CompareTo(fastPrecision) < 0) {
                    // Second operand's precision too short
                    FastInteger precisionDiff =
                    FastInteger.Copy(fastPrecision).Subtract(digitLength2);
                    if (!oneOpIsZero && !sameSign) {
                    precisionDiff.AddInt(2);
                    }
                    op1MantAbs = this.TryMultiplyByRadixPower(
                    op1MantAbs,
                    precisionDiff);
                    if (op1MantAbs == null) {
                    return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                    }
                    EInteger bigintTemp = precisionDiff.AsBigInteger();
                    op1Exponent -= (EInteger)bigintTemp;
                    if (!oneOpIsZero && !sameSign) {
                    op1MantAbs -= EInteger.One;
                    }
                    thisValue = this.helper.CreateNewWithFlags(
                    op1MantAbs,
                    op1Exponent,
                    this.helper.GetFlags(thisValue));
                    FastInteger shift =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    if (oneOpIsZero && ctx != null && ctx.HasFlags) {
                    ctx.Flags |= EContext.FlagRounded;
                    }
                    return this.RoundToPrecisionInternal(
                    thisValue,
     (oneOpIsZero || sameSign) ? 0 : 1,
     (oneOpIsZero && !sameSign) ? 0 : 1,
     shift,
     false,
                    ctx);
                  }
                  if (!oneOpIsZero && !sameSign) {
                    op1MantAbs = this.TryMultiplyByRadixPower(
                    op1MantAbs,
                    new FastInteger(2));
                    if (op1MantAbs == null) {
                    return this.SignalInvalidWithMessage(
                    ctx,
                    "Result requires too much memory");
                    }
                    op1Exponent -= (EInteger)2;
                    op1MantAbs -= EInteger.One;
                    thisValue = this.helper.CreateNewWithFlags(
                    op1MantAbs,
                    op1Exponent,
                    this.helper.GetFlags(thisValue));
                    FastInteger shift =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    return this.RoundToPrecisionInternal(
          thisValue,
          0,
          0,
          shift,
          false,
          ctx);
                  } else {
                    FastInteger shift2 =
                    FastInteger.Copy(digitLength2).Subtract(fastPrecision);
                    if (!sameSign && ctx != null && ctx.HasFlags) {
                    ctx.Flags |= EContext.FlagRounded;
                    }
                    return this.RoundToPrecisionInternal(
                    thisValue,
                    0,
                    sameSign ? 1 : 0,
                    shift2,
                    false,
                    ctx);
                  }
                }
              }
            }
          }
          expcmp = op1Exponent.CompareTo((EInteger)op2Exponent);
          resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
        }
      }
      if (expcmp > 0) {
        // DebugUtility.Log("" + op1MantAbs + " " + op2MantAbs);
        op1MantAbs = this.RescaleByExponentDiff(
op1MantAbs,
op1Exponent,
op2Exponent);
        if (op1MantAbs == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        retval = this.AddCore(
          op1MantAbs,
          op2MantAbs,
          resultExponent,
          thisFlags,
          otherFlags,
          ctx);
      } else {
        op2MantAbs = this.RescaleByExponentDiff(
op2MantAbs,
op1Exponent,
op2Exponent);
        if (op2MantAbs == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        retval = this.AddCore(
op1MantAbs,
op2MantAbs,
resultExponent,
thisFlags,
otherFlags,
ctx);
      }
      if (roundToOperandPrecision && ctx != null && ctx.HasMaxPrecision) {
        FastInteger digitLength1 =
          this.helper.CreateShiftAccumulator(op1MantAbs)
          .GetDigitLength();
        FastInteger digitLength2 =
          this.helper.CreateShiftAccumulator(op2MantAbs)
          .GetDigitLength();
        FastInteger maxDigitLength =
          (digitLength1.CompareTo(digitLength2) > 0) ? digitLength1 :
          digitLength2;
        maxDigitLength.SubtractBig(ctx.Precision);
        // DebugUtility.Log("retval= " + retval + " maxdl=" +
        // maxDigitLength + " prec= " + (ctx.Precision));
        return (maxDigitLength.Sign > 0) ? this.RoundToPrecisionInternal(
            retval,
            0,
            0,
            maxDigitLength,
            false,
            ctx) : this.RoundToPrecision(retval, ctx);
        // DebugUtility.Log("retval now " + retval);
      } else {
        return (IsNullOrSimpleContext(ctx)) ? (retval) :
          (this.RoundToPrecision(retval, ctx));
      }
    }

    public T AddEx(
T thisValue,
T other,
EContext ctx,
bool roundToOperandPrecision) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
            if ((thisFlags & BigNumberFlags.FlagNegative) != (otherFlags &
  BigNumberFlags.FlagNegative)) {
              return this.SignalInvalid(ctx);
            }
          }
          return thisValue;
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return other;
        }
      }
      EInteger op1Exponent = this.helper.GetExponent(thisValue);
      EInteger op2Exponent = this.helper.GetExponent(other);
      EInteger op1Mantissa = this.helper.GetMantissa(thisValue);
      EInteger op2Mantissa = this.helper.GetMantissa(other);
      int expcmp = op1Exponent.CompareTo(op2Exponent);
      EInteger resultExponent = expcmp < 0 ? op1Exponent : op2Exponent;
      T retval = default(T);
      if ((thisFlags & BigNumberFlags.FlagNegative) == 0 &&
        (otherFlags & BigNumberFlags.FlagNegative) == 0) {
       if (expcmp<0 && op2Mantissa.IsZero) {
         return IsNullOrSimpleContext(ctx) ?
          thisValue : this.RoundToPrecision(thisValue, ctx);
       } else if (expcmp >= 0 && op1Mantissa.IsZero) {
         return IsNullOrSimpleContext(ctx) ?
          other : this.RoundToPrecision(other, ctx);
       }
      }
      if ((expcmp == 0 || (op1Exponent.CanFitInInt32() &&
        op2Exponent.CanFitInInt32())) &&
        op1Mantissa.CanFitInInt32() && op2Mantissa.CanFitInInt32() &&
        (thisFlags & BigNumberFlags.FlagNegative) == 0 &&
        (otherFlags & BigNumberFlags.FlagNegative) == 0) {
        var e1int = 0;
        var e2int = 0;
        if (expcmp != 0) {
          e1int = op1Exponent.AsInt32Unchecked();
          e2int = op2Exponent.AsInt32Unchecked();
        }
        int m1, m2;
        var haveRetval = false;
        if (expcmp == 0 || (e1int >= -100 && e1int < 100 && e2int >= -100 &&
          e2int < 100)) {
          int ediff = (expcmp == 0) ? 0 : Math.Abs(e1int - e2int);
          int radix = this.thisRadix;
          if (expcmp == 0) {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if (m2 <= Int32.MaxValue - m1) {
              m1 += m2;
              retval = this.helper.CreateNewWithFlags(
                EInteger.FromInt32(m1),
                resultExponent,
                0);
              haveRetval = true;
            }
          } else if (ediff <= 9 && radix == 10) {
            int power = ValueTenPowers[ediff];
            int maxoverflow = OverflowMaxes[ediff];
            if (expcmp > 0) {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m1 == 0) {
                retval = other;
              } else if (m1 <= maxoverflow) {
                m1 *= power;
                if (m2 <= Int32.MaxValue - m1) {
                  m1 += m2;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m1),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            } else {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m2 == 0) {
                retval = thisValue;
              } if (m2 <= maxoverflow) {
                m2 *= power;
                if (m1 <= Int32.MaxValue - m2) {
                  m2 += m1;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m2),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            }
          } else if (ediff <= 30 && radix == 2) {
            int mask = BitMasks[ediff];
            if (expcmp > 0) {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m1 == 0) {
                retval = other;
              } else if ((m1 & mask) == m1) {
                m1 <<= ediff;
                if (m2 <= Int32.MaxValue - m1) {
                  m1 += m2;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m1),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            } else {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m2 == 0) {
                retval = thisValue;
              } else if ((m2 & mask) == m2) {
                m2 <<= ediff;
                if (m1 <= Int32.MaxValue - m2) {
                  m2 += m1;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m2),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            }
          }
        }
        if (haveRetval) {
          if (!IsNullOrSimpleContext(ctx)) {
            retval = this.RoundToPrecision(retval, ctx);
          }
          return retval;
        }
      }
      if ((expcmp == 0 || (op1Exponent.CanFitInInt32() &&
        op2Exponent.CanFitInInt32())) &&
        op1Mantissa.CanFitInInt32() && op2Mantissa.CanFitInInt32() &&
        (thisFlags & BigNumberFlags.FlagNegative) == 0 &&
        (otherFlags & BigNumberFlags.FlagNegative) != 0 &&
        !(op2Mantissa.IsZero)) {
        var e1int = 0;
        var e2int = 0;
        if (expcmp != 0) {
          e1int = op1Exponent.AsInt32Unchecked();
          e2int = op2Exponent.AsInt32Unchecked();
        }
        int m1, m2;
        var haveRetval = false;
        if (expcmp == 0 || (e1int >= -100 && e1int < 100 && e2int >= -100 &&
          e2int < 100)) {
          int ediff = (expcmp == 0) ? 0 : Math.Abs(e1int - e2int);
          int radix = this.thisRadix;
          if (expcmp == 0) {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if (Int32.MinValue + m2 <= m1) {
              m1 -= m2;
              retval = this.helper.CreateNewWithFlags(
                EInteger.FromInt32(m1),
                resultExponent,
                0);
              haveRetval = true;
            }
          } else if (ediff <= 9 && radix == 10) {
            int power = ValueTenPowers[ediff];
            int maxoverflow = OverflowMaxes[ediff];
            if (expcmp > 0) {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m1 == 0) {
                retval = other;
              } else if (m1 <= maxoverflow) {
                m1 *= power;
                if (Int32.MinValue + m2 <= m1) {
                  m1 -= m2;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m1),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            } else {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m2 == 0) {
                retval = thisValue;
              } if (m2 <= maxoverflow) {
                m2 *= power;
                if (Int32.MinValue + m1 <= m2) {
                  m2 -= m1;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m2),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            }
          } else if (ediff <= 30 && radix == 2) {
            int mask = BitMasks[ediff];
            if (expcmp > 0) {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m1 == 0) {
                retval = other;
              } else if ((m1 & mask) == m1) {
                m1 <<= ediff;
                if (Int32.MinValue + m2 <= m1) {
                  m1 -= m2;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m1),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            } else {
              m1 = op1Mantissa.AsInt32Unchecked();
              m2 = op2Mantissa.AsInt32Unchecked();
              if (m2 == 0) {
                retval = thisValue;
              } else if ((m2 & mask) == m2) {
                m2 <<= ediff;
                if (Int32.MinValue + m1 <= m2) {
                  m2 -= m1;
                  retval = this.helper.CreateNewWithFlags(
                    EInteger.FromInt32(m2),
                    resultExponent,
                    0);
                  haveRetval = true;
                }
              }
            }
          }
        }
        if (haveRetval) {
          if (!IsNullOrSimpleContext(ctx)) {
            retval = this.RoundToPrecision(retval, ctx);
          }
          return retval;
        }
      }

      //DebugUtility.Log("exp=" + (// expcmp) + "/" + 0 + ",res=" + 0 + ",["
      // + op1Mantissa + "," + op1Exponent + "],[" + (//op2Mantissa) + "," +
      // op2Exponent + "]");
      if (expcmp == 0) {
        retval = this.AddCore(
          op1Mantissa,
          op2Mantissa,
          op1Exponent,
          thisFlags,
          otherFlags,
          ctx);
        if (!IsNullOrSimpleContext(ctx)) {
          retval = this.RoundToPrecision(retval, ctx);
        }
      } else {
        retval = this.AddExDiffExp(
thisValue,
other,
thisFlags,
otherFlags,
ctx,
expcmp,
roundToOperandPrecision);
      }
      return retval;
    }

    public int CompareTo(T thisValue, T otherValue) {
      return this.CompareToInternal(thisValue, otherValue, true);
    }

    public T CompareToWithContext(
T thisValue,
T otherValue,
bool treatQuietNansAsSignaling,
EContext ctx) {
      if (otherValue == null) {
        return this.SignalInvalid(ctx);
      }
      T result = this.CompareToHandleSpecial(
thisValue,
otherValue,
treatQuietNansAsSignaling,
ctx);
      if ((object)result != (object)default(T)) {
        return result;
      }
      int cmp = this.CompareToInternal(thisValue, otherValue, false);
      return (cmp == -2) ? this.SignalInvalidWithMessage(
        ctx,
        "Out of memory ") :
        this.ValueOf(this.CompareTo(thisValue, otherValue), null);
    }

    public T Divide(T thisValue, T divisor, EContext ctx) {
      return this.DivideInternal(
thisValue,
divisor,
ctx,
IntegerModeRegular,
EInteger.Zero);
    }

    public T DivideToExponent(
T thisValue,
T divisor,
EInteger desiredExponent,
EContext ctx) {
      if (ctx != null && !ctx.ExponentWithinRange(desiredExponent)) {
        return this.SignalInvalidWithMessage(
ctx,
"Exponent not within exponent range: " + desiredExponent);
      }
      EContext ctx2 = (ctx == null) ?
        EContext.ForRounding(ERounding.HalfDown) :
        ctx.WithUnlimitedExponents().WithPrecision(0);
      T ret = this.DivideInternal(
thisValue,
divisor,
ctx2,
IntegerModeFixedScale,
desiredExponent);
      if (!ctx2.HasMaxPrecision && this.IsFinite(ret)) {
        // If a precision is given, call Quantize to ensure
        // that the value fits the precision
        ret = this.Quantize(ret, ret, ctx2);
        if ((ctx2.Flags & EContext.FlagInvalid) != 0) {
          ctx2.Flags = EContext.FlagInvalid;
        }
      }
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= ctx2.Flags;
      }
      return ret;
    }

    public T DivideToIntegerNaturalScale(
T thisValue,
T divisor,
EContext ctx) {
      FastInteger desiredScale =
        FastInteger.FromBig(this.helper.GetExponent(thisValue))
        .SubtractBig(this.helper.GetExponent(divisor));
      EContext ctx2 =
        EContext.ForRounding(ERounding.Down).WithBigPrecision(ctx ==
                    null ?
  EInteger.Zero :
ctx.Precision).WithBlankFlags();
      T ret = this.DivideInternal(
thisValue,
divisor,
ctx2,
IntegerModeFixedScale,
EInteger.Zero);
      if ((ctx2.Flags & (EContext.FlagInvalid |
                    EContext.FlagDivideByZero)) != 0) {
        if (ctx != null && ctx.HasFlags) {
          ctx.Flags |= EContext.FlagInvalid | EContext.FlagDivideByZero;
        }
        return ret;
      }
      bool neg = (this.helper.GetSign(thisValue) < 0) ^
        (this.helper.GetSign(divisor) < 0);
      // Now the exponent's sign can only be 0 or positive
      if (this.helper.GetMantissa(ret).IsZero) {
        // Value is 0, so just change the exponent
        // to the preferred one
        EInteger dividendExp = this.helper.GetExponent(thisValue);
        EInteger divisorExp = this.helper.GetExponent(divisor);
        ret = this.helper.CreateNewWithFlags(
EInteger.Zero,
dividendExp - (EInteger)divisorExp,
this.helper.GetFlags(ret));
      } else {
        if (desiredScale.Sign < 0) {
          // Desired scale is negative, shift left
          desiredScale.Negate();
          EInteger bigmantissa = this.helper.GetMantissa(ret);
          bigmantissa = this.TryMultiplyByRadixPower(bigmantissa, desiredScale);
          if (bigmantissa == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          EInteger exponentDivisor = this.helper.GetExponent(divisor);
          ret = this.helper.CreateNewWithFlags(
bigmantissa,
this.helper.GetExponent(thisValue) - (EInteger)exponentDivisor,
this.helper.GetFlags(ret));
        } else if (desiredScale.Sign > 0) {
          // Desired scale is positive, shift away zeros
          // but not after scale is reached
          EInteger bigmantissa = this.helper.GetMantissa(ret);
          FastInteger fastexponent =
            FastInteger.FromBig(this.helper.GetExponent(ret));
          var bigradix = (EInteger)this.thisRadix;
          while (true) {
            if (desiredScale.CompareTo(fastexponent) == 0) {
              break;
            }
            EInteger bigrem;
            EInteger bigquo;
            {
              EInteger[] divrem = bigmantissa.DivRem(bigradix);
              bigquo = divrem[0];
              bigrem = divrem[1];
            }
            if (!bigrem.IsZero) {
              break;
            }
            bigmantissa = bigquo;
            fastexponent.Increment();
          }
          ret = this.helper.CreateNewWithFlags(
            bigmantissa,
            fastexponent.AsBigInteger(),
            this.helper.GetFlags(ret));
        }
      }
      if (ctx != null) {
        ret = this.RoundToPrecision(ret, ctx);
      }
      ret = this.EnsureSign(ret, neg);
      return ret;
    }

    public T DivideToIntegerZeroScale(
T thisValue,
T divisor,
EContext ctx) {
      EContext ctx2 = EContext.ForRounding(ERounding.Down)
        .WithBigPrecision(ctx == null ? EInteger.Zero :
                    ctx.Precision).WithBlankFlags();
      T ret = this.DivideInternal(
        thisValue,
        divisor,
        ctx2,
        IntegerModeFixedScale,
        EInteger.Zero);
      if ((ctx2.Flags & (EContext.FlagInvalid |
                    EContext.FlagDivideByZero)) != 0) {
        if (ctx.HasFlags) {
          ctx.Flags |= ctx2.Flags & (EContext.FlagInvalid |
                    EContext.FlagDivideByZero);
        }
        return ret;
      }
      if (ctx != null) {
        ctx2 = ctx.WithBlankFlags().WithUnlimitedExponents();
        ret = this.RoundToPrecision(ret, ctx2);
        if ((ctx2.Flags & EContext.FlagRounded) != 0) {
          return this.SignalInvalid(ctx);
        }
      }
      return ret;
    }

    public T Exp(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      EContext ctxCopy = ctx.WithBlankFlags();
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          T retval = this.helper.CreateNewWithFlags(
            EInteger.Zero,
            EInteger.Zero,
            0);
          retval = this.RoundToPrecision(
            retval,
            ctxCopy);
          if (ctx.HasFlags) {
            ctx.Flags |= ctxCopy.Flags;
          }
          return retval;
        }
        return thisValue;
      }
      int sign = this.helper.GetSign(thisValue);
      T one = this.helper.ValueOf(1);
      EInteger guardDigits = this.thisRadix == 2 ? ctx.Precision +
        (EInteger)10 : (EInteger)10;
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.Precision + guardDigits)
        .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
      if (sign == 0) {
        thisValue = this.RoundToPrecision(one, ctxCopy);
      } else if (sign > 0 && this.CompareTo(thisValue, one) <= 0) {
        thisValue = this.ExpInternal(thisValue, ctxdiv.Precision, ctxCopy);
        if (ctx.HasFlags) {
          ctx.Flags |= EContext.FlagInexact |
            EContext.FlagRounded;
        }
      } else if (sign < 0) {
        // exp(x) = 1/exp(-x) where x<0
        T val = this.Exp(this.NegateRaw(thisValue), ctxdiv);
        if ((ctxdiv.Flags & EContext.FlagOverflow) != 0 ||
            !this.IsFinite(val)) {
          // Overflow, try again with expanded exponent range
          EInteger newMax;
          ctxdiv.Flags = 0;
          newMax = ctx.EMax;
          EInteger expdiff = ctx.EMin;
          expdiff = newMax - (EInteger)expdiff;
          newMax += (EInteger)expdiff;
          ctxdiv = ctxdiv.WithBigExponentRange(ctxdiv.EMin, newMax);
          thisValue = this.Exp(this.NegateRaw(thisValue), ctxdiv);
          if ((ctxdiv.Flags & EContext.FlagOverflow) != 0) {
            // Still overflowed
            if (ctx.HasFlags) {
              ctx.Flags |= BigNumberFlags.UnderflowFlags;
            }
            // Return a "subnormal" zero, with fake extra digits to stimulate
            // rounding
            EInteger ctxdivPrec = ctxdiv.Precision;
            newMax = ctx.EMin;
            if (ctx.AdjustExponent) {
              newMax -= (EInteger)ctxdivPrec;
              newMax += EInteger.One;
            }
            thisValue = this.helper.CreateNewWithFlags(
              EInteger.Zero,
              newMax,
              0);
            return this.RoundToPrecisionInternal(
              thisValue,
              0,
              1,
              null,
              false,
              ctx);
          }
        } else {
          thisValue = val;
        }
        thisValue = this.Divide(one, thisValue, ctxCopy);
        // DebugUtility.Log("end= " + thisValue);
        // DebugUtility.Log("endbit "+this.BitMantissa(thisValue));
        if (ctx.HasFlags) {
          ctx.Flags |= EContext.FlagInexact |
            EContext.FlagRounded;
        }
      } else {
        T intpart = this.Quantize(
          thisValue,
          one,
          EContext.ForRounding(ERounding.Down));
        if (!this.GetHelper().GetExponent(intpart).IsZero) {
          throw new ArgumentException("integer part not zero, as expected");
        }
        if (this.CompareTo(thisValue, this.helper.ValueOf(50000)) > 0 &&
            ctx.HasExponentRange) {
          // Try to check for overflow quickly
          // Do a trial powering using a lower number than e,
          // and a power of 50000
          this.PowerIntegral(
this.helper.ValueOf(2),
(EInteger)50000,
ctxCopy);
          if ((ctxCopy.Flags & EContext.FlagOverflow) != 0) {
            // The trial powering caused overflow, so exp will
            // cause overflow as well
            return this.SignalOverflow2(ctx, false);
          }
          ctxCopy.Flags = 0;
          // Now do the same using the integer part of the operand
          // as the power
          this.PowerIntegral(
this.helper.ValueOf(2),
this.helper.GetMantissa(intpart),
ctxCopy);
          if ((ctxCopy.Flags & EContext.FlagOverflow) != 0) {
            // The trial powering caused overflow, so exp will
            // cause overflow as well
            return this.SignalOverflow2(ctx, false);
          }
          ctxCopy.Flags = 0;
        }
        T fracpart = this.Add(thisValue, this.NegateRaw(intpart), null);
        ctxdiv = SetPrecisionIfLimited(ctxdiv, ctxdiv.Precision + guardDigits)
           .WithBlankFlags();
        fracpart = this.Add(one, this.Divide(fracpart, intpart, ctxdiv), null);
        ctxdiv.Flags = 0;
        //DebugUtility.Log("fracpart=" + fracpart);
        EInteger workingPrec = ctxdiv.Precision;
        workingPrec += (EInteger)17;
        thisValue = this.ExpInternal(fracpart, workingPrec, ctxdiv);
        //DebugUtility.Log("thisValue=" + thisValue);
        if ((ctxdiv.Flags & EContext.FlagUnderflow) != 0) {
          if (ctx.HasFlags) {
            ctx.Flags |= ctxdiv.Flags;
          }
        }
        if (ctx.HasFlags) {
          ctx.Flags |= EContext.FlagInexact |
            EContext.FlagRounded;
        }
        thisValue = this.PowerIntegral(
thisValue,
this.helper.GetMantissa(intpart),
ctxCopy);
      }
      if (ctx.HasFlags) {
        ctx.Flags |= ctxCopy.Flags;
      }
      return thisValue;
    }

    public IRadixMathHelper<T> GetHelper() {
      return this.helper;
    }

    public T Ln(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      int sign = this.helper.GetSign(thisValue);
      if (sign < 0) {
        return this.SignalInvalid(ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        return thisValue;
      }
      EContext ctxCopy = ctx.WithBlankFlags();
      T one = this.helper.ValueOf(1);
      if (sign == 0) {
        return this.helper.CreateNewWithFlags(
EInteger.Zero,
EInteger.Zero,
BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity);
      } else {
        int cmpOne = this.CompareTo(thisValue, one);
        EContext ctxdiv = null;
        if (cmpOne == 0) {
          // Equal to 1
          thisValue = this.RoundToPrecision(
           this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
           ctxCopy);
        } else if (cmpOne < 0) {
          // Less than 1
          var error = new FastInteger(10);
          EInteger bigError = error.AsBigInteger();
          ctxdiv = SetPrecisionIfLimited(ctx, ctx.Precision + bigError)
            .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
          T quarter = this.Divide(one, this.helper.ValueOf(4), ctxCopy);
          if (this.CompareTo(thisValue, quarter) <= 0) {
            // One quarter or less
            T half = this.Multiply(quarter, this.helper.ValueOf(2), null);
            var roots = new FastInteger(0);
            // Take square root until this value
            // is one half or more
            while (this.CompareTo(thisValue, half) < 0) {
              thisValue = this.SquareRoot(
thisValue,
ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            thisValue = this.LnInternal(thisValue, ctxdiv.Precision, ctxdiv);
            EInteger bigintRoots = PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue = this.Multiply(
thisValue,
this.helper.CreateNewWithFlags(bigintRoots, EInteger.Zero, 0),
ctxCopy);
          } else {
            T smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
            T closeToOne = this.Add(one, this.NegateRaw(smallfrac), null);
            if (this.CompareTo(thisValue, closeToOne) >= 0) {
              // This value is close to 1, so use a higher working precision
              error =

  this.helper.CreateShiftAccumulator(this.helper.GetMantissa(thisValue))
                .GetDigitLength();
              error.AddInt(6);
              error.AddBig(ctx.Precision);
              bigError = error.AsBigInteger();
              thisValue = this.LnInternal(
thisValue,
error.AsBigInteger(),
ctxCopy);
            } else {
              thisValue = this.LnInternal(thisValue, ctxdiv.Precision, ctxCopy);
            }
          }
          if (ctx.HasFlags) {
            ctxCopy.Flags |= EContext.FlagInexact;
            ctxCopy.Flags |= EContext.FlagRounded;
          }
        } else {
          // Greater than 1
          T two = this.helper.ValueOf(2);
          if (this.CompareTo(thisValue, two) >= 0) {
            var roots = new FastInteger(0);
            FastInteger error;
            EInteger bigError;
            error = new FastInteger(10);
            bigError = error.AsBigInteger();
            ctxdiv = SetPrecisionIfLimited(ctx, ctx.Precision + bigError)
              .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
            T smallfrac = this.Divide(one, this.helper.ValueOf(10), ctxdiv);
            T closeToOne = this.Add(one, smallfrac, null);
            // Take square root until this value
            // is close to 1
            while (this.CompareTo(thisValue, closeToOne) >= 0) {
              thisValue = this.SquareRoot(
thisValue,
ctxdiv.WithUnlimitedExponents());
              roots.Increment();
            }
            // Find -Ln(1/thisValue)
            thisValue = this.Divide(one, thisValue, ctxdiv);
            thisValue = this.LnInternal(thisValue, ctxdiv.Precision, ctxdiv);
            thisValue = this.NegateRaw(thisValue);
            EInteger bigintRoots = PowerOfTwo(roots);
            // Multiply back 2^X, where X is the number
            // of square root calls
            thisValue = this.Multiply(
thisValue,
this.helper.CreateNewWithFlags(bigintRoots, EInteger.Zero, 0),
ctxCopy);
          } else {
            FastInteger error;
            EInteger bigError;
            error = new FastInteger(10);
            bigError = error.AsBigInteger();
            ctxdiv = SetPrecisionIfLimited(ctx, ctx.Precision + bigError)
              .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
            T smallfrac = this.Divide(one, this.helper.ValueOf(16), ctxdiv);
            T closeToOne = this.Add(one, smallfrac, null);
            if (this.CompareTo(thisValue, closeToOne) < 0) {
              error =

  this.helper.CreateShiftAccumulator(this.helper.GetMantissa(thisValue))
                .GetDigitLength();
              error.AddInt(6);
              error.AddBig(ctx.Precision);
              bigError = error.AsBigInteger();
              // Greater than 1 and close to 1, will require a higher working
              // precision
              thisValue = this.LnInternal(
thisValue,
error.AsBigInteger(),
ctxCopy);
            } else {
              // Find -Ln(1/thisValue)
              thisValue = this.Divide(one, thisValue, ctxdiv);
              thisValue = this.LnInternal(thisValue, ctxdiv.Precision, ctxCopy);
              thisValue = this.NegateRaw(thisValue);
            }
          }
          if (ctx.HasFlags) {
            ctxCopy.Flags |= EContext.FlagInexact;
            ctxCopy.Flags |= EContext.FlagRounded;
          }
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= ctxCopy.Flags;
      }
      return thisValue;
    }

    public T Log10(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        // NOTE: Returning a signaling NaN is independent of
        // rounding mode
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        // NOTE: Returning a quiet NaN is independent of
        // rounding mode
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      int sign = this.helper.GetSign(thisValue);
      if (sign < 0) {
        return this.SignalInvalid(ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        return thisValue;
      }
      EContext ctxCopy = ctx.WithBlankFlags();
      T one = this.helper.ValueOf(1);
      // DebugUtility.Log("input " + thisValue);
      if (sign == 0) {
        // Result is negative infinity if input is 0
        thisValue = this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, BigNumberFlags.FlagNegative | BigNumberFlags.FlagInfinity),
            ctxCopy);
      } else if (this.CompareTo(thisValue, one) == 0) {
        // Result is 0 if input is 1
        thisValue = this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
            ctxCopy);
      } else {
        EInteger exp = this.helper.GetExponent(thisValue);
        EInteger mant = this.helper.GetMantissa(thisValue);
        if (mant.Equals(EInteger.One) && this.thisRadix == 10) {
          // Value is 1 and radix is 10, so the result is the exponent
          thisValue = this.helper.CreateNewWithFlags(
            exp,
            EInteger.Zero,
            exp.Sign < 0 ? BigNumberFlags.FlagNegative : 0);
          thisValue = this.RoundToPrecision(
thisValue,
ctxCopy);
        } else {
          EInteger mantissa = this.helper.GetMantissa(thisValue);
          FastInteger expTmp = FastInteger.FromBig(exp);
          var tenBig = (EInteger)10;
          while (true) {
            EInteger bigrem;
            EInteger bigquo;
            {
              EInteger[] divrem = mantissa.DivRem(tenBig);
              bigquo = divrem[0];
              bigrem = divrem[1];
            }
            if (!bigrem.IsZero) {
              break;
            }
            mantissa = bigquo;
            expTmp.Increment();
          }
          if (mantissa.CompareTo(EInteger.One) == 0 &&
              (this.thisRadix == 10 || expTmp.Sign == 0 || exp.IsZero)) {
            // Value is an integer power of 10
            thisValue = this.helper.CreateNewWithFlags(
              expTmp.AsBigInteger(),
              EInteger.Zero,
              expTmp.Sign < 0 ? BigNumberFlags.FlagNegative : 0);
            thisValue = thisValue = this.RoundToPrecision(
                thisValue,
                ctxCopy);
          } else {
            EContext ctxdiv = SetPrecisionIfLimited(
              ctx,
              ctx.Precision + (EInteger)10)
              .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
            T logNatural = this.Ln(thisValue, ctxdiv);
            T logTen = this.LnTenConstant(ctxdiv);
            thisValue = this.Divide(logNatural, logTen, ctx);
            // Treat result as inexact
            if (ctx.HasFlags) {
              ctx.Flags |= EContext.FlagInexact |
                EContext.FlagRounded;
            }
          }
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= ctxCopy.Flags;
      }
      return thisValue;
    }

    public T Max(T a, T b, EContext ctx) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, false, false);
      if ((object)result != (object)default(T)) {
        return result;
      }
      int cmp = this.CompareTo(a, b);
      if (cmp != 0) {
        return cmp < 0 ? this.RoundToPrecision(b, ctx) :
          this.RoundToPrecision(a, ctx);
      }
      int flagNegA = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
      return (flagNegA != (this.helper.GetFlags(b) &
                    BigNumberFlags.FlagNegative)) ? ((flagNegA != 0) ?
                this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx)) :
        ((flagNegA == 0) ?
         (this.helper.GetExponent(a).CompareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx)) :
         (this.helper.GetExponent(a).CompareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx)));
    }

    public T MaxMagnitude(T a, T b, EContext ctx) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, false, true);
      if ((object)result != (object)default(T)) {
        return result;
      }
      int cmp = this.CompareTo(this.AbsRaw(a), this.AbsRaw(b));
      return (cmp == 0) ? this.Max(a, b, ctx) : ((cmp > 0) ?
                this.RoundToPrecision(
a,
ctx) : this.RoundToPrecision(
b,
ctx));
    }

    public T Min(T a, T b, EContext ctx) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, true, false);
      if ((object)result != (object)default(T)) {
        return result;
      }
      int cmp = this.CompareTo(a, b);
      if (cmp != 0) {
        return cmp > 0 ? this.RoundToPrecision(b, ctx) :
          this.RoundToPrecision(a, ctx);
      }
      int signANeg = this.helper.GetFlags(a) & BigNumberFlags.FlagNegative;
      return (signANeg != (this.helper.GetFlags(b) &
                    BigNumberFlags.FlagNegative)) ? ((signANeg != 0) ?
                this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx)) :
        ((signANeg == 0) ?
         (this.helper.GetExponent(a).CompareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(b, ctx) : this.RoundToPrecision(a, ctx)) :
         (this.helper.GetExponent(a).CompareTo(this.helper.GetExponent(b)) > 0 ?
          this.RoundToPrecision(a, ctx) : this.RoundToPrecision(b, ctx)));
    }

    public T MinMagnitude(T a, T b, EContext ctx) {
      if (a == null) {
        throw new ArgumentNullException("a");
      }
      if (b == null) {
        throw new ArgumentNullException("b");
      }
      // Handle infinity and NaN
      T result = this.MinMaxHandleSpecial(a, b, ctx, true, true);
      if ((object)result != (object)default(T)) {
        return result;
      }
      int cmp = this.CompareTo(this.AbsRaw(a), this.AbsRaw(b));
      return (cmp == 0) ? this.Min(a, b, ctx) : ((cmp < 0) ?
                this.RoundToPrecision(
a,
ctx) : this.RoundToPrecision(
b,
ctx));
    }

    public T Multiply(T thisValue, T other, EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          bool negflag = ((thisFlags & BigNumberFlags.FlagNegative) != 0) ^
            ((otherFlags & BigNumberFlags.FlagNegative) != 0);
          return ((otherFlags & BigNumberFlags.FlagSpecial) == 0 &&
             this.helper.GetMantissa(other).IsZero) ? this.SignalInvalid(ctx) :
            this.EnsureSign(
thisValue,
negflag);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to multiply infinity by 0
          bool negflag = ((thisFlags & BigNumberFlags.FlagNegative) != 0) ^
            ((otherFlags & BigNumberFlags.FlagNegative) != 0);
          return ((thisFlags & BigNumberFlags.FlagSpecial) == 0 &&
                  this.helper.GetMantissa(thisValue).IsZero) ?
            this.SignalInvalid(ctx) : this.EnsureSign(other, negflag);
        }
      }
      EInteger bigintOp2 = this.helper.GetExponent(other);
      EInteger newexp = this.helper.GetExponent(thisValue) +
          (EInteger)bigintOp2;
      EInteger mantissaOp2 = this.helper.GetMantissa(other);
      // DebugUtility.Log("" + (this.helper.GetMantissa(thisValue)) + "," +
      // (this.helper.GetExponent(thisValue)) + " -> " + mantissaOp2 +", " +
      // (bigintOp2));
      thisFlags = (thisFlags & BigNumberFlags.FlagNegative) ^ (otherFlags &
  BigNumberFlags.FlagNegative);
      T ret =
        this.helper.CreateNewWithFlags(
          this.helper.GetMantissa(thisValue) * (EInteger)mantissaOp2,
          newexp,
          thisFlags);
      if (ctx != null && ctx != EContext.Unlimited) {
        ret = this.RoundToPrecision(ret, ctx);
      }
      return ret;
    }

    private static bool IsNullOrSimpleContext(EContext ctx) {
      return ctx == null || ctx == EContext.Unlimited ||
       (!ctx.HasExponentRange && !ctx.HasMaxPrecision && ctx.Traps == 0 &&
        !ctx.HasFlags);
    }

    private static bool IsSimpleContext(EContext ctx) {
      return ctx != null && (ctx == EContext.Unlimited ||
       (!ctx.HasExponentRange && !ctx.HasMaxPrecision && ctx.Traps == 0 &&
        !ctx.HasFlags));
    }

    public T MultiplyAndAdd(
T thisValue,
T multiplicand,
T augend,
EContext ctx) {
      if (multiplicand == null) {
        throw new ArgumentNullException("multiplicand");
      }
      if (augend == null) {
        throw new ArgumentNullException("augend");
      }
      EContext ctx2 = EContext.Unlimited.WithBlankFlags();
      T ret = this.MultiplyAddHandleSpecial(
thisValue,
multiplicand,
augend,
ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      ret = this.Add(this.Multiply(thisValue, multiplicand, ctx2), augend, ctx);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= ctx2.Flags;
      }
      return ret;
    }

    public T Negate(T value, EContext ctx) {
      int flags = this.helper.GetFlags(value);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(value, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(value, ctx);
      }
      EInteger mant = this.helper.GetMantissa(value);
      T zero;
      if ((flags & BigNumberFlags.FlagInfinity) == 0 && mant.IsZero) {
        if ((flags & BigNumberFlags.FlagNegative) == 0) {
          // positive 0 minus positive 0 is always positive 0
          zero = this.helper.CreateNewWithFlags(
mant,
this.helper.GetExponent(value),
flags & ~BigNumberFlags.FlagNegative);
          return this.RoundToPrecision(zero, ctx);
        }
        zero = ctx != null && ctx.Rounding == ERounding.Floor ?
          this.helper.CreateNewWithFlags(
mant,
this.helper.GetExponent(value),
flags | BigNumberFlags.FlagNegative) : this.helper.CreateNewWithFlags(
mant,
this.helper.GetExponent(value),
flags & ~BigNumberFlags.FlagNegative);
        return this.RoundToPrecision(zero, ctx);
      }
      flags ^= BigNumberFlags.FlagNegative;
      return this.RoundToPrecision(
   this.helper.CreateNewWithFlags(mant, this.helper.GetExponent(value), flags),
   ctx);
    }

    public T NextMinus(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      if (!ctx.HasExponentRange) {
        return this.SignalInvalidWithMessage(
ctx,
"doesn't satisfy ctx.HasExponentRange");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          return thisValue;
        } else {
          EInteger bigexp2 = ctx.EMax;
          EInteger bigprec = ctx.Precision;
          if (ctx.AdjustExponent) {
            bigexp2 += EInteger.One;
            bigexp2 -= (EInteger)bigprec;
          }
          EInteger overflowMant = this.TryMultiplyByRadixPower(
              EInteger.One,
              FastInteger.FromBig(ctx.Precision));
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant -= EInteger.One;
          return this.helper.CreateNewWithFlags(overflowMant, bigexp2, 0);
        }
      }
      FastInteger minexp = FastInteger.FromBig(ctx.EMin);
      if (ctx.AdjustExponent) {
        minexp.SubtractBig(ctx.Precision).Increment();
      }
      FastInteger bigexp =
        FastInteger.FromBig(this.helper.GetExponent(thisValue));
      if (bigexp.CompareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = this.helper.CreateNewWithFlags(
        EInteger.One,
        minexp.AsBigInteger(),
        BigNumberFlags.FlagNegative);
      EContext ctx2;
      ctx2 = ctx.WithRounding(ERounding.Floor);
      return this.Add(thisValue, quantum, ctx2);
    }

    public T NextPlus(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      if (!ctx.HasExponentRange) {
        return this.SignalInvalidWithMessage(
ctx,
"doesn't satisfy ctx.HasExponentRange");
      }
      int flags = this.helper.GetFlags(thisValue);
      if ((flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(thisValue, ctx);
      }
      if ((flags & BigNumberFlags.FlagInfinity) != 0) {
        if ((flags & BigNumberFlags.FlagNegative) != 0) {
          EInteger bigexp2 = ctx.EMax;
          EInteger bigprec = ctx.Precision;
          if (ctx.AdjustExponent) {
            bigexp2 += EInteger.One;
            bigexp2 -= (EInteger)bigprec;
          }
          EInteger overflowMant = this.TryMultiplyByRadixPower(
              EInteger.One,
              FastInteger.FromBig(ctx.Precision));
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant -= EInteger.One;
          return this.helper.CreateNewWithFlags(
overflowMant,
bigexp2,
BigNumberFlags.FlagNegative);
        }
        return thisValue;
      }
      FastInteger minexp = FastInteger.FromBig(ctx.EMin);
      if (ctx.AdjustExponent) {
        minexp.SubtractBig(ctx.Precision).Increment();
      }
      FastInteger bigexp =
        FastInteger.FromBig(this.helper.GetExponent(thisValue));
      if (bigexp.CompareTo(minexp) <= 0) {
        // Use a smaller exponent if the input exponent is already
        // very small
        minexp = FastInteger.Copy(bigexp).SubtractInt(2);
      }
      T quantum = this.helper.CreateNewWithFlags(
        EInteger.One,
        minexp.AsBigInteger(),
        0);
      EContext ctx2;
      T val = thisValue;
      ctx2 = ctx.WithRounding(ERounding.Ceiling);
      return this.Add(val, quantum, ctx2);
    }

    public T NextToward(T thisValue, T otherValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      if (!ctx.HasExponentRange) {
        return this.SignalInvalidWithMessage(
ctx,
"doesn't satisfy ctx.HasExponentRange");
      }
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, otherValue, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
      }
      EContext ctx2;
      int cmp = this.CompareTo(thisValue, otherValue);
      if (cmp == 0) {
        return this.RoundToPrecision(
   this.EnsureSign(thisValue, (otherFlags & BigNumberFlags.FlagNegative) != 0),
   ctx.WithNoFlags());
      } else {
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if ((thisFlags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) == (otherFlags &
                    (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) {
            // both values are the same infinity
            return thisValue;
          } else {
            EInteger bigexp2 = ctx.EMax;
            EInteger bigprec = ctx.Precision;
            if (ctx.AdjustExponent) {
              bigexp2 += EInteger.One;
              bigexp2 -= (EInteger)bigprec;
            }
            EInteger overflowMant = this.TryMultiplyByRadixPower(
                EInteger.One,
                FastInteger.FromBig(ctx.Precision));
            if (overflowMant == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            overflowMant -= EInteger.One;
            return this.helper.CreateNewWithFlags(
overflowMant,
bigexp2,
thisFlags & BigNumberFlags.FlagNegative);
          }
        }
        FastInteger minexp = FastInteger.FromBig(ctx.EMin);
        if (ctx.AdjustExponent) {
          minexp.SubtractBig(ctx.Precision).Increment();
        }
        FastInteger bigexp =
          FastInteger.FromBig(this.helper.GetExponent(thisValue));
        if (bigexp.CompareTo(minexp) < 0) {
          // Use a smaller exponent if the input exponent is already
          // very small
          minexp = FastInteger.Copy(bigexp).SubtractInt(2);
        } else {
          // Ensure the exponent is lower than the exponent range
          // (necessary to flag underflow correctly)
          minexp.SubtractInt(2);
        }
        T quantum = this.helper.CreateNewWithFlags(
          EInteger.One,
          minexp.AsBigInteger(),
          (cmp > 0) ? BigNumberFlags.FlagNegative : 0);
        T val = thisValue;
        ctx2 = ctx.WithRounding((cmp > 0) ? ERounding.Floor :
                    ERounding.Ceiling).WithBlankFlags();
        val = this.Add(val, quantum, ctx2);
        if ((ctx2.Flags & (EContext.FlagOverflow |
                    EContext.FlagUnderflow)) == 0) {
          // Don't set flags except on overflow or underflow,
          // in accordance with the DecTest test cases
          ctx2.Flags = 0;
        }
        if ((ctx2.Flags & EContext.FlagUnderflow) != 0) {
          EInteger bigmant = this.helper.GetMantissa(val);
          EInteger maxmant = this.TryMultiplyByRadixPower(
            EInteger.One,
            FastInteger.FromBig(ctx.Precision).Decrement());
          if (maxmant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          if (bigmant.CompareTo(maxmant) >= 0 ||
              ctx.Precision.CompareTo(EInteger.One) == 0) {
            // don't treat max-precision results as having underflowed
            ctx2.Flags = 0;
          }
        }
        if (ctx.HasFlags) {
          ctx.Flags |= ctx2.Flags;
        }
        return val;
      }
    }

    public T Pi(EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      // Gauss-Legendre algorithm
      T a = this.helper.ValueOf(1);
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.Precision + (EInteger)10)
        .WithRounding(ERounding.OddOrZeroFiveUp);
      T two = this.helper.ValueOf(2);
      T b = this.Divide(a, this.SquareRoot(two, ctxdiv), ctxdiv);
      T four = this.helper.ValueOf(4);
      T half = ((this.thisRadix & 1) == 0) ?
        this.helper.CreateNewWithFlags(
(EInteger)(this.thisRadix / 2),
ValueMinusOne,
0) : default(T);
      T t = this.Divide(a, four, ctxdiv);
      var more = true;
      var lastCompare = 0;
      var vacillations = 0;
      T lastGuess = default(T);
      T guess = default(T);
      EInteger powerTwo = EInteger.One;
      while (more) {
        lastGuess = guess;
        T aplusB = this.Add(a, b, null);
        T newA = (half == null) ? this.Divide(aplusB, two, ctxdiv) :
          this.Multiply(aplusB, half, null);
        T valueAMinusNewA = this.Add(a, this.NegateRaw(newA), null);
        if (!a.Equals(b)) {
          T atimesB = this.Multiply(a, b, ctxdiv);
          b = this.SquareRoot(atimesB, ctxdiv);
        }
        a = newA;
        guess = this.Multiply(aplusB, aplusB, null);
        guess = this.Divide(guess, this.Multiply(t, four, null), ctxdiv);
        T newGuess = guess;
        if ((object)lastGuess != (object)default(T)) {
          int guessCmp = this.CompareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                    guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        if (more) {
          T tmpT = this.Multiply(valueAMinusNewA, valueAMinusNewA, null);
          tmpT = this.Multiply(
tmpT,
this.helper.CreateNewWithFlags(powerTwo, EInteger.Zero, 0),
null);
          t = this.Add(t, this.NegateRaw(tmpT), ctxdiv);
          powerTwo <<= 1;
        }
        guess = newGuess;
      }
      return this.RoundToPrecision(guess, ctx);
    }

    public T Plus(T thisValue, EContext context) {
      return this.RoundToPrecisionInternal(
thisValue,
0,
0,
null,
true,
context);
    }

    public T Power(T thisValue, T pow, EContext ctx) {
      T ret = this.HandleNotANumber(thisValue, pow, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      int thisSign = this.helper.GetSign(thisValue);
      int powSign = this.helper.GetSign(pow);
      int thisFlags = this.helper.GetFlags(thisValue);
      int powFlags = this.helper.GetFlags(pow);
      if (thisSign == 0 && powSign == 0) {
        // Both operands are zero: invalid
        return this.SignalInvalid(ctx);
      }
      if (thisSign < 0 && (powFlags & BigNumberFlags.FlagInfinity) != 0) {
        // This value is negative and power is infinity: invalid
        return this.SignalInvalid(ctx);
      }
      if (thisSign > 0 && (thisFlags & BigNumberFlags.FlagInfinity) == 0 &&
          (powFlags & BigNumberFlags.FlagInfinity) != 0) {
        // Power is infinity and this value is greater than
        // zero and not infinity
        int cmp = this.CompareTo(thisValue, this.helper.ValueOf(1));
        if (cmp < 0) {
          // Value is less than 1
          if (powSign < 0) {
            // Power is negative infinity, return positive infinity
            return this.helper.CreateNewWithFlags(
EInteger.Zero,
EInteger.Zero,
BigNumberFlags.FlagInfinity);
          }
          // Power is positive infinity, return 0
          return this.RoundToPrecision(
           this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
           ctx);
        }
        if (cmp == 0) {
          // Extend the precision of the mantissa as much as possible,
          // in the special case that this value is 1
          return this.ExtendPrecision(this.helper.ValueOf(1), ctx);
        }
        // Value is greater than 1
        if (powSign > 0) {
          // Power is positive infinity, return positive infinity
          return pow;
        }
        // Power is negative infinity, return 0
        return this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
            ctx);
      }
      EInteger powExponent = this.helper.GetExponent(pow);
      bool isPowIntegral = powExponent.Sign > 0;
      var isPowOdd = false;
      T powInt = default(T);
      if (!isPowIntegral) {
        powInt = this.Quantize(
      pow,
      this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
      EContext.ForRounding(ERounding.Down));
        isPowIntegral = this.CompareTo(powInt, pow) == 0;
        isPowOdd = !this.helper.GetMantissa(powInt).IsEven;
      } else {
        if (powExponent.Equals(EInteger.Zero)) {
          isPowOdd = !this.helper.GetMantissa(powInt).IsEven;
        } else if (this.thisRadix % 2 == 0) {
          // Never odd for even radixes
          isPowOdd = false;
        } else {
          powInt = this.Quantize(
pow,
this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
EContext.ForRounding(ERounding.Down));
          isPowOdd = !this.helper.GetMantissa(powInt).IsEven;
        }
      }
      // DebugUtility.Log("pow=" + pow + " powint=" + powInt);
      bool isResultNegative = (thisFlags & BigNumberFlags.FlagNegative) != 0 &&
        (powFlags & BigNumberFlags.FlagInfinity) == 0 && isPowIntegral &&
        isPowOdd;
      if (thisSign == 0 && powSign != 0) {
        int infinityFlags = (powSign < 0) ? BigNumberFlags.FlagInfinity : 0;
        if (isResultNegative) {
          infinityFlags |= BigNumberFlags.FlagNegative;
        }
        thisValue = this.helper.CreateNewWithFlags(
          EInteger.Zero,
          EInteger.Zero,
          infinityFlags);
        if ((infinityFlags & BigNumberFlags.FlagInfinity) == 0) {
          thisValue = this.RoundToPrecision(thisValue, ctx);
        }
        return thisValue;
      }
      if ((!isPowIntegral || powSign < 0) && (ctx == null ||
                    !ctx.HasMaxPrecision)) {
        const string ValueOutputMessage =
            "ctx is null or has unlimited precision, " +
            "and pow's exponent is not an integer or is negative";
        return this.SignalInvalidWithMessage(
ctx,
ValueOutputMessage);
      }
      if (thisSign < 0 && !isPowIntegral) {
        return this.SignalInvalid(ctx);
      }
      if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
        // This value is infinity
        int negflag = isResultNegative ? BigNumberFlags.FlagNegative : 0;
        return (powSign > 0) ? this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, negflag | BigNumberFlags.FlagInfinity),
            ctx) : ((powSign < 0) ? this.RoundToPrecision(
     this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, negflag),
     ctx) : this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.One, EInteger.Zero, 0),
            ctx));
      }
      if (powSign == 0) {
        return
          this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.One, EInteger.Zero, 0),
            ctx);
      }
      if (isPowIntegral) {
        // Special case for 1
        if (this.CompareTo(thisValue, this.helper.ValueOf(1)) == 0) {
          return (!this.IsWithinExponentRangeForPow(pow, ctx)) ?
            this.SignalInvalid(ctx) : this.helper.ValueOf(1);
        }
        if ((object)powInt == (object)default(T)) {
          powInt = this.Quantize(
pow,
this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, 0),
EContext.ForRounding(ERounding.Down));
        }
        EInteger signedMant = this.helper.GetMantissa(powInt);
        if (powSign < 0) {
          signedMant = -signedMant;
        }
        // DebugUtility.Log("tv=" + thisValue + " mant=" + signedMant);
        return this.PowerIntegral(thisValue, signedMant, ctx);
      }
      // Special case for 1
      if (this.CompareTo(thisValue, this.helper.ValueOf(1)) == 0 && powSign >
          0) {
        return (!this.IsWithinExponentRangeForPow(pow, ctx)) ?
          this.SignalInvalid(ctx) :
          this.ExtendPrecision(this.helper.ValueOf(1), ctx);
      }
#if DEBUG
      if (ctx == null) {
        throw new ArgumentNullException("ctx");
      }
#endif
      // Special case for 0.5
      if (this.thisRadix == 10 || this.thisRadix == 2) {
        T half = (this.thisRadix == 10) ? this.helper.CreateNewWithFlags(
            (EInteger)5,
            ValueMinusOne,
            0) : this.helper.CreateNewWithFlags(
EInteger.One,
ValueMinusOne,
0);
        if (this.CompareTo(pow, half) == 0 &&
            this.IsWithinExponentRangeForPow(pow, ctx) &&
            this.IsWithinExponentRangeForPow(thisValue, ctx)) {
          EContext ctxCopy = ctx.WithBlankFlags();
          thisValue = this.SquareRoot(thisValue, ctxCopy);
          ctxCopy.Flags |= EContext.FlagInexact;
          ctxCopy.Flags |= EContext.FlagRounded;
          if ((ctxCopy.Flags & EContext.FlagSubnormal) != 0) {
            ctxCopy.Flags |= EContext.FlagUnderflow;
          }
          thisValue = this.ExtendPrecision(thisValue, ctxCopy);
          if (ctx.HasFlags) {
            ctx.Flags |= ctxCopy.Flags;
          }
          return thisValue;
        }
      }
      int guardDigitCount = this.thisRadix == 2 ? 32 : 10;
      var guardDigits = (EInteger)guardDigitCount;
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.Precision + guardDigits);
      ctxdiv = ctxdiv.WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
      T lnresult = this.Ln(thisValue, ctxdiv);
      /* DebugUtility.Log("guard= " + guardDigits + " prec=" + ctx.Precision+
        " newprec= " + ctxdiv.Precision);
      DebugUtility.Log("pwrIn " + pow);
      DebugUtility.Log("lnIn " + thisValue);
      DebugUtility.Log("lnOut " + lnresult);
      DebugUtility.Log("lnOut[n] "+this.NextPlus(lnresult,ctxdiv));*/
      lnresult = this.Multiply(lnresult, pow, ctxdiv);
      // DebugUtility.Log("expIn " + lnresult);
      // Now use original precision and rounding mode
      ctxdiv = ctx.WithBlankFlags();
      lnresult = this.Exp(lnresult, ctxdiv);
      if ((ctxdiv.Flags & (EContext.FlagClamped |
                    EContext.FlagOverflow)) != 0) {
        if (!this.IsWithinExponentRangeForPow(thisValue, ctx)) {
          return this.SignalInvalid(ctx);
        }
        if (!this.IsWithinExponentRangeForPow(pow, ctx)) {
          return this.SignalInvalid(ctx);
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= ctxdiv.Flags;
      }
      return lnresult;
    }

    public T Quantize(T thisValue, T otherValue, EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, otherValue, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if (((thisFlags & otherFlags) & BigNumberFlags.FlagInfinity) != 0) {
          return this.RoundToPrecision(thisValue, ctx);
        }
        // At this point, it's only the case that either value
        // is infinity
        return this.SignalInvalid(ctx);
      }
      EInteger expOther = this.helper.GetExponent(otherValue);
      if (ctx != null && !ctx.ExponentWithinRange(expOther)) {
        // DebugUtility.Log("exp not within range");
        return this.SignalInvalidWithMessage(
ctx,
"Exponent not within exponent range: " + expOther);
      }
      EContext tmpctx = (ctx == null ?
  EContext.ForRounding(ERounding.HalfEven) :
                    ctx.Copy()).WithBlankFlags();
      EInteger mantThis = this.helper.GetMantissa(thisValue);
      EInteger expThis = this.helper.GetExponent(thisValue);
      int expcmp = expThis.CompareTo(expOther);
      int negativeFlag = this.helper.GetFlags(thisValue) &
        BigNumberFlags.FlagNegative;
      T ret = default(T);
      if (expcmp == 0) {
        // DebugUtility.Log("exp same");
        ret = this.RoundToPrecision(thisValue, tmpctx);
      } else if (mantThis.IsZero) {
        // DebugUtility.Log("mant is 0");
        ret = this.helper.CreateNewWithFlags(
EInteger.Zero,
expOther,
negativeFlag);
        ret = this.RoundToPrecision(ret, tmpctx);
      } else if (expcmp > 0) {
        // Other exponent is less
        // DebugUtility.Log("other exp less");
        FastInteger radixPower =
               FastInteger.FromBig(expThis).SubtractBig(expOther);
        if (tmpctx.Precision.Sign > 0 &&
            radixPower.CompareTo(FastInteger.FromBig(tmpctx.Precision)
                .AddInt(10)) > 0) {
          // Radix power is much too high for the current precision
          // DebugUtility.Log("result too high for prec:" +
          // tmpctx.Precision + " radixPower= " + radixPower);
          return this.SignalInvalidWithMessage(
ctx,
"Result too high for current precision");
        }
        mantThis = this.TryMultiplyByRadixPower(mantThis, radixPower);
        if (mantThis == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        ret = this.helper.CreateNewWithFlags(mantThis, expOther, negativeFlag);
        ret = this.RoundToPrecision(ret, tmpctx);
      } else {
        // Other exponent is greater
        // DebugUtility.Log("other exp greater");
        FastInteger shift = FastInteger.FromBig(expOther).SubtractBig(expThis);
        ret = this.RoundToPrecisionInternal(
thisValue,
0,
0,
shift,
false,
tmpctx);
      }
      if ((tmpctx.Flags & EContext.FlagOverflow) != 0) {
        // DebugUtility.Log("overflow occurred");
        return this.SignalInvalid(ctx);
      }
      if (ret == null || !this.helper.GetExponent(ret).Equals(expOther)) {
        // DebugUtility.Log("exp not same "+ret);
        return this.SignalInvalid(ctx);
      }
      ret = this.EnsureSign(ret, negativeFlag != 0);
      if (ctx != null && ctx.HasFlags) {
        int flags = tmpctx.Flags;
        flags &= ~EContext.FlagUnderflow;
        ctx.Flags |= flags;
      }
      return ret;
    }

    public T Reduce(T thisValue, EContext ctx) {
      return this.ReduceToPrecisionAndIdealExponent(thisValue, ctx, null, null);
    }

    public T Remainder(T thisValue, T divisor, EContext ctx) {
      EContext ctx2 = ctx == null ? null : ctx.WithBlankFlags();
      T ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((object)ret != (object)default(T)) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = this.DivideToIntegerZeroScale(thisValue, divisor, ctx2);
      if ((ctx2.Flags & EContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      ret = this.Add(
thisValue,
this.NegateRaw(this.Multiply(ret, divisor, null)),
ctx2);
      ret = this.EnsureSign(
    ret,
    (this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      TransferFlags(ctx, ctx2);
      return ret;
    }

    public T RemainderNear(T thisValue, T divisor, EContext ctx) {
      EContext ctx2 = ctx == null ?
        EContext.ForRounding(ERounding.HalfEven).WithBlankFlags() :
        ctx.WithRounding(ERounding.HalfEven).WithBlankFlags();
      T ret = this.RemainderHandleSpecial(thisValue, divisor, ctx2);
      if ((object)ret != (object)default(T)) {
        TransferFlags(ctx, ctx2);
        return ret;
      }
      ret = this.DivideInternal(
thisValue,
divisor,
ctx2,
IntegerModeFixedScale,
EInteger.Zero);
      if ((ctx2.Flags & EContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      ctx2 = ctx2.WithBlankFlags();
      ret = this.RoundToPrecision(ret, ctx2);
      if ((ctx2.Flags & (EContext.FlagRounded |
                    EContext.FlagInvalid)) != 0) {
        return this.SignalInvalid(ctx);
      }
      ctx2 = ctx == null ? EContext.Unlimited.WithBlankFlags() :
        ctx.WithBlankFlags();
      T ret2 = this.Add(
        thisValue,
        this.NegateRaw(this.Multiply(ret, divisor, null)),
        ctx2);
      if ((ctx2.Flags & EContext.FlagInvalid) != 0) {
        return this.SignalInvalid(ctx);
      }
      if (this.helper.GetFlags(ret2) == 0 &&
             this.helper.GetMantissa(ret2).IsZero) {
        ret2 = this.EnsureSign(
ret2,
(this.helper.GetFlags(thisValue) & BigNumberFlags.FlagNegative) != 0);
      }
      TransferFlags(ctx, ctx2);
      return ret2;
    }

    public T RoundAfterConversion(T thisValue, EContext ctx) {
      // DebugUtility.Log("RM RoundAfterConversion");
      return this.RoundToPrecision(thisValue, ctx);
    }

    public T RoundToExponentExact(
T thisValue,
EInteger expOther,
EContext ctx) {
      if (this.helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return this.RoundToPrecision(thisValue, ctx);
      } else {
        EContext pctx = (ctx == null) ? null :
          ctx.WithPrecision(0).WithBlankFlags();
        T ret = this.Quantize(
        thisValue,
        this.helper.CreateNewWithFlags(EInteger.One, expOther, 0),
        pctx);
        if (ctx != null && ctx.HasFlags) {
          ctx.Flags |= pctx.Flags;
        }
        return ret;
      }
    }

    public T RoundToExponentNoRoundedFlag(
T thisValue,
EInteger exponent,
EContext ctx) {
      EContext pctx = (ctx == null) ? null : ctx.WithBlankFlags();
      T ret = this.RoundToExponentExact(thisValue, exponent, pctx);
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= pctx.Flags & ~(EContext.FlagInexact |
                    EContext.FlagRounded);
      }
      return ret;
    }

    public T RoundToExponentSimple(
T thisValue,
EInteger expOther,
EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, thisValue, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      if (this.helper.GetExponent(thisValue).CompareTo(expOther) >= 0) {
        return this.RoundToPrecision(thisValue, ctx);
      } else {
        if (ctx != null && !ctx.ExponentWithinRange(expOther)) {
          return this.SignalInvalidWithMessage(
ctx,
"Exponent not within exponent range: " + expOther);
        }
        FastInteger shift = FastInteger.FromBig(expOther)
          .SubtractBig(this.helper.GetExponent(thisValue));
        if (shift.Sign == 0 && IsSimpleContext(ctx)) {
          return thisValue;
        }
        EInteger bigmantissa = this.helper.GetMantissa(thisValue);
        IShiftAccumulator accum =
             this.helper.CreateShiftAccumulator(bigmantissa);
        if (IsSimpleContext(ctx) && ctx.Rounding == ERounding.Down) {
          accum.TruncateRight(shift);
          return this.helper.CreateNewWithFlags(
             accum.ShiftedInt,
             expOther,
             thisFlags);
        } else {
          accum.ShiftRight(shift);
        }
        bigmantissa = accum.ShiftedInt;
        thisValue = this.helper.CreateNewWithFlags(
          bigmantissa,
          expOther,
          thisFlags);
        return this.RoundToPrecisionInternal(
          thisValue,
          accum.LastDiscardedDigit,
          accum.OlderDiscardedDigits,
          null,
          false,
          ctx);
      }
    }

    public T RoundToPrecision(T thisValue, EContext context) {
      return this.RoundToPrecisionInternal(
thisValue,
0,
0,
null,
false,
context);
    }

    public T SquareRoot(T thisValue, EContext ctx) {
      if (ctx == null) {
        return this.SignalInvalidWithMessage(ctx, "ctx is null");
      }
      if (!ctx.HasMaxPrecision) {
        return this.SignalInvalidWithMessage(
ctx,
"ctx has unlimited precision");
      }
      T ret = this.SquareRootHandleSpecial(thisValue, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      EContext ctxtmp = ctx.WithBlankFlags();
      EInteger currentExp = this.helper.GetExponent(thisValue);
      EInteger origExp = currentExp;
      EInteger idealExp;
      idealExp = currentExp;
      idealExp /= (EInteger)2;
      if (currentExp.Sign < 0 && !currentExp.IsEven) {
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        idealExp -= EInteger.One;
      }
      // DebugUtility.Log("curr=" + currentExp + " ideal=" + idealExp);
      if (this.helper.GetSign(thisValue) == 0) {
        ret = this.RoundToPrecision(
            this.helper.CreateNewWithFlags(EInteger.Zero, idealExp, this.helper.GetFlags(thisValue)),
            ctxtmp);
        if (ctx.HasFlags) {
          ctx.Flags |= ctxtmp.Flags;
        }
        return ret;
      }
      EInteger mantissa = this.helper.GetMantissa(thisValue);
      IShiftAccumulator accum = this.helper.CreateShiftAccumulator(mantissa);
      FastInteger digitCount = accum.GetDigitLength();
      FastInteger targetPrecision = FastInteger.FromBig(ctx.Precision);
      FastInteger precision =
        FastInteger.Copy(targetPrecision).Multiply(2).AddInt(2);
      var rounded = false;
      var inexact = false;
      if (digitCount.CompareTo(precision) < 0) {
        FastInteger diff = FastInteger.Copy(precision).Subtract(digitCount);
        // DebugUtility.Log(diff);
        if ((!diff.IsEvenNumber) ^ (!origExp.IsEven)) {
          diff.Increment();
        }
        EInteger bigdiff = diff.AsBigInteger();
        currentExp -= (EInteger)bigdiff;
        mantissa = this.TryMultiplyByRadixPower(mantissa, diff);
        if (mantissa == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
      }
      EInteger[] sr = mantissa.SqrtRem();
      digitCount = this.helper.CreateShiftAccumulator(sr[0]).GetDigitLength();
      EInteger squareRootRemainder = sr[1];
      // DebugUtility.Log("I " + mantissa + " -> " + sr[0] + " [target="+
      // targetPrecision + "], (zero= " + squareRootRemainder.IsZero +") "
      mantissa = sr[0];
      if (!squareRootRemainder.IsZero) {
        rounded = true;
        inexact = true;
      }
      EInteger oldexp = currentExp;
      currentExp /= (EInteger)2;
      if (oldexp.Sign < 0 && !oldexp.IsEven) {
        // Round towards negative infinity; BigInteger's
        // division operation rounds towards zero
        currentExp -= EInteger.One;
      }
      T retval = this.helper.CreateNewWithFlags(mantissa, currentExp, 0);
      // DebugUtility.Log("idealExp= " + idealExp + ", curr" + currentExp
      // +" guess= " + mantissa);
      retval = this.RoundToPrecisionInternal(
retval,
0,
inexact ? 1 : 0,
null,
false,
ctxtmp);
      currentExp = this.helper.GetExponent(retval);
      // DebugUtility.Log("guess I " + guess + " idealExp=" + idealExp
      // +", curr " + currentExp + " clamped= " +
      // (ctxtmp.Flags&PrecisionContext.FlagClamped));
      if ((ctxtmp.Flags & EContext.FlagUnderflow) == 0) {
        int expcmp = currentExp.CompareTo(idealExp);
        if (expcmp <= 0 || !this.IsFinite(retval)) {
          retval = this.ReduceToPrecisionAndIdealExponent(
            retval,
            ctx.HasExponentRange ? ctxtmp : null,
            inexact ? targetPrecision : null,
            FastInteger.FromBig(idealExp));
        }
      }
      if (ctx.HasFlags) {
        if (ctx.ClampNormalExponents &&
            !this.helper.GetExponent(retval).Equals(idealExp) && (ctxtmp.Flags &
    EContext.FlagInexact) == 0) {
          ctx.Flags |= EContext.FlagClamped;
        }
        rounded |= (ctxtmp.Flags & EContext.FlagOverflow) != 0;
        // DebugUtility.Log("guess II " + guess);
        currentExp = this.helper.GetExponent(retval);
        if (rounded) {
          ctxtmp.Flags |= EContext.FlagRounded;
        } else {
          if (currentExp.CompareTo(idealExp) > 0) {
            // Greater than the ideal, treat as rounded anyway
            ctxtmp.Flags |= EContext.FlagRounded;
          } else {
            // DebugUtility.Log("idealExp= " + idealExp + ", curr" +
            // currentExp + " (II)");
            ctxtmp.Flags &= ~EContext.FlagRounded;
          }
        }
        if (inexact) {
          ctxtmp.Flags |= EContext.FlagRounded;
          ctxtmp.Flags |= EContext.FlagInexact;
        }
        ctx.Flags |= ctxtmp.Flags;
      }
      return retval;
    }

    private static EInteger PowerOfTwo(FastInteger fi) {
      if (fi.Sign <= 0) {
        return EInteger.One;
      }
      if (fi.CanFitInInt32()) {
        int val = fi.AsInt32();
        if (val <= 30) {
          val = 1 << val;
          return (EInteger)val;
        }
        return EInteger.One << val;
      } else {
        EInteger bi = EInteger.One;
        FastInteger fi2 = FastInteger.Copy(fi);
        while (fi2.Sign > 0) {
          var count = 1000000;
          if (fi2.CompareToInt(1000000) < 0) {
            count = (int)bi;
          }
          bi <<= count;
          fi2.SubtractInt(count);
        }
        return bi;
      }
    }

    private static EContext SetPrecisionIfLimited(
      EContext ctx,
      EInteger bigPrecision) {
      return (ctx == null || !ctx.HasMaxPrecision) ? ctx :
        ctx.WithBigPrecision(bigPrecision);
    }

    private static void TransferFlags(
EContext ctxDst,
EContext ctxSrc) {
      if (ctxDst != null && ctxDst.HasFlags) {
        if ((ctxSrc.Flags & (EContext.FlagInvalid |
                    EContext.FlagDivideByZero)) != 0) {
          ctxDst.Flags |= ctxSrc.Flags & (EContext.FlagInvalid |
                    EContext.FlagDivideByZero);
        } else {
          ctxDst.Flags |= ctxSrc.Flags;
        }
      }
    }

    private T AbsRaw(T value) {
      return this.EnsureSign(value, false);
    }
    // mant1 and mant2 are assumed to be nonnegative
    private T AddCore(
EInteger mant1,
EInteger mant2,
EInteger exponent,
int flags1,
int flags2,
EContext ctx) {
#if DEBUG
      if (mant1.Sign < 0) {
        throw new InvalidOperationException();
      }
      if (mant2.Sign < 0) {
        throw new InvalidOperationException();
      }
#endif
      bool neg1 = (flags1 & BigNumberFlags.FlagNegative) != 0;
      bool neg2 = (flags2 & BigNumberFlags.FlagNegative) != 0;
      var negResult = false;
      // DebugUtility.Log("neg1=" + neg1 + " neg2=" + neg2);
      if (neg1 != neg2) {
        // Signs are different, treat as a subtraction
        mant1 -= (EInteger)mant2;
        int mant1Sign = mant1.Sign;
        negResult = neg1 ^ (mant1Sign == 0 ? neg2 : (mant1Sign < 0));
      } else {
        // Signs are same, treat as an addition
        mant1 += (EInteger)mant2;
        negResult = neg1;
      }
      if (negResult && mant1.IsZero) {
        // Result is negative zero
        negResult &= (neg1 && neg2) || ((neg1 ^ neg2) && ctx != null &&
                    ctx.Rounding == ERounding.Floor);
      }
      // DebugUtility.Log("mant1= " + mant1 + " exp= " + exponent +" neg= "+
      // (negResult));
      return this.helper.CreateNewWithFlags(
mant1,
exponent,
negResult ? BigNumberFlags.FlagNegative : 0);
    }

    private T CompareToHandleSpecial(
T thisValue,
T other,
bool treatQuietNansAsSignaling,
EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(other, ctx);
        }
        if (treatQuietNansAsSignaling) {
          if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.SignalingNaNInvalid(thisValue, ctx);
          }
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.SignalingNaNInvalid(other, ctx);
          }
        } else {
          // Check this value then the other value for quiet NaN
          if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(thisValue, ctx);
          }
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            return this.ReturnQuietNaN(other, ctx);
          }
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ? this.ValueOf(0, null) : (((thisFlags &
                BigNumberFlags.FlagNegative) == 0) ? this.ValueOf(
1,
null) : this.ValueOf(-1, null));
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ? this.ValueOf(0, null) : (((otherFlags &
                    BigNumberFlags.FlagNegative) == 0) ?
                this.ValueOf(-1, null) : this.ValueOf(1, null));
        }
      }
      return default(T);
    }

    private int CompareToHandleSpecialReturnInt(T thisValue, T other) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagNaN) != 0) {
          if ((otherFlags & BigNumberFlags.FlagNaN) != 0) {
            return 0;
          }
          // Consider NaN to be greater
          return 1;
        }
        if ((otherFlags & BigNumberFlags.FlagNaN) != 0) {
          // Consider this to be less than NaN
          return -1;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // thisValue is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ? 0 :
            (((thisFlags & BigNumberFlags.FlagNegative) == 0) ? 1 : -1);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // the other value is infinity
          return ((thisFlags & (BigNumberFlags.FlagInfinity |
                    BigNumberFlags.FlagNegative)) == (otherFlags &
  (BigNumberFlags.FlagInfinity |
  BigNumberFlags.FlagNegative))) ? 0 :
            (((otherFlags & BigNumberFlags.FlagNegative) == 0) ? -1 : 1);
        }
      }
      return 2;
    }

    private static readonly int[] ValueTenPowers = {
      1, 10, 100, 1000, 10000, 100000,
      1000000, 10000000, 100000000,
      1000000000
    };
    private static readonly int[] OverflowMaxes = {
     2147483647, 214748364, 21474836,
     2147483, 214748, 21474, 2147, 214, 21, 2
    };
    private static readonly int[] BitMasks = {
      0x7fffffff, 0x3fffffff, 0x1fffffff,
      0xfffffff, 0x7ffffff, 0x3ffffff, 0x1ffffff,
      0xffffff, 0x7fffff, 0x3fffff, 0x1fffff,
      0xfffff, 0x7ffff, 0x3ffff, 0x1ffff,
      0xffff, 0x7fff, 0x3fff, 0x1fff,
      0xfff, 0x7ff, 0x3ff, 0x1ff,
      0xff, 0x7f, 0x3f, 0x1f,
      0xf, 0x7, 0x3, 0x1
    };

    private static int CompareToFast(
  int e1int,
  int e2int,
  int expcmp,
  int signA,
  EInteger op1Mantissa,
  EInteger op2Mantissa,
  int radix) {
      int m1, m2;
      int mantcmp;
      if (e1int >= -100 && e1int < 100 && e2int >= -100 && e2int < 100) {
        int ediff = Math.Abs(e1int - e2int);
        if (ediff <= 9 && radix == 10) {
          int power = ValueTenPowers[ediff];
          int maxoverflow = OverflowMaxes[ediff];
          if (expcmp > 0) {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if (m1 <= maxoverflow) {
              m1 *= power;
              mantcmp = (m1 == m2) ? 0 : ((m1 < m2) ? -1 : 1);
              return (signA < 0) ? -mantcmp : mantcmp;
            }
          } else {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if (m2 <= maxoverflow) {
              m2 *= power;
              mantcmp = (m1 == m2) ? 0 : ((m1 < m2) ? -1 : 1);
              return (signA < 0) ? -mantcmp : mantcmp;
            }
          }
        } else if (ediff <= 30 && radix == 2) {
          int mask = BitMasks[ediff];
          if (expcmp > 0) {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if ((m1 & mask) == m1) {
              m1 <<= ediff;
              mantcmp = (m1 == m2) ? 0 : ((m1 < m2) ? -1 : 1);
              return (signA < 0) ? -mantcmp : mantcmp;
            }
          } else {
            m1 = op1Mantissa.AsInt32Unchecked();
            m2 = op2Mantissa.AsInt32Unchecked();
            if ((m2 & mask) == m2) {
              m2 <<= ediff;
              mantcmp = (m1 == m2) ? 0 : ((m1 < m2) ? -1 : 1);
              return (signA < 0) ? -mantcmp : mantcmp;
            }
          }
        }
      }
      return 2;
    }

    private int CompareToInternal(T thisValue, T otherValue, bool reportOOM) {
      if (otherValue == null) {
        return 1;
      }
      int signA = this.CompareToHandleSpecialReturnInt(thisValue, otherValue);
      if (signA <= 1) {
        return signA;
      }
      signA = this.helper.GetSign(thisValue);
      int signB = this.helper.GetSign(otherValue);
      if (signA != signB) {
        return (signA < signB) ? -1 : 1;
      }
      if (signB == 0 || signA == 0) {
        // Special case: Either operand is zero
        return 0;
      }
      EInteger op1Exponent = this.helper.GetExponent(thisValue);
      EInteger op2Exponent = this.helper.GetExponent(otherValue);
      EInteger op1Mantissa = this.helper.GetMantissa(thisValue);
      EInteger op2Mantissa = this.helper.GetMantissa(otherValue);
      int expcmp = op1Exponent.CompareTo(op2Exponent);
      // At this point, the signs are equal so we can compare
      // their absolute values instead
      int mantcmp = op1Mantissa.CompareTo(op2Mantissa);
      if (mantcmp == 0) {
        // Special case: Mantissas are equal
        return signA < 0 ? -expcmp : expcmp;
      }
      if (signA < 0) {
        mantcmp = -mantcmp;
      }
      if (expcmp == 0) {
        return mantcmp;
      }
      if (op1Exponent.CanFitInInt32() && op2Exponent.CanFitInInt32() &&
          op1Mantissa.CanFitInInt32() && op2Mantissa.CanFitInInt32()) {
        int e1int = op1Exponent.AsInt32Unchecked();
        int e2int = op2Exponent.AsInt32Unchecked();
        int c = CompareToFast(e1int, e2int,
          expcmp, signA, op1Mantissa, op2Mantissa, this.thisRadix);
        if (c <= 1) {
          return c;
        }
      }
      FastInteger fastOp1Exp = FastInteger.FromBig(op1Exponent);
      FastInteger fastOp2Exp = FastInteger.FromBig(op2Exponent);

      FastInteger expdiff = FastInteger.Copy(fastOp1Exp)
             .Subtract(fastOp2Exp).Abs();
      // Check if exponent difference is too big for
      // radix-power calculation to work quickly
      if (expdiff.CompareToInt(100) >= 0) {
        EInteger op1MantAbs =
          this.helper.GetMantissa(thisValue);
        EInteger op2MantAbs = this.helper.GetMantissa(otherValue);
        FastInteger precision1 =
          this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
        FastInteger precision2 =
          this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
        FastInteger maxPrecision = null;
        maxPrecision = (precision1.CompareTo(precision2) > 0) ? precision1 :
          precision2;
        // If exponent difference is greater than the
        // maximum precision of the two operands
        if (FastInteger.Copy(expdiff).CompareTo(maxPrecision) > 0) {
          int expcmp2 = fastOp1Exp.CompareTo(fastOp2Exp);
          if (expcmp2 < 0) {
            if (!op2MantAbs.IsZero) {
              // first operand's exponent is less
              // and second operand isn't zero
              // second mantissa will be shifted by the exponent
              // difference
              FastInteger digitLength1 =
                this.helper.CreateShiftAccumulator(op1MantAbs).GetDigitLength();
              if (FastInteger.Copy(fastOp1Exp).Add(digitLength1).AddInt(2)
                .CompareTo(fastOp2Exp) < 0) {
                // first operand's mantissa can't reach the
                // second operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp2Exp)
                .SubtractInt(8).Subtract(digitLength1).Subtract(maxPrecision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp2Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  // At this point, both operands have the same sign
                  return (signA < 0) ? 1 : -1;
                }
              }
            }
          } else if (expcmp2 > 0) {
            if (!op1MantAbs.IsZero) {
              // first operand's exponent is greater
              // and second operand isn't zero
              // first mantissa will be shifted by the exponent
              // difference
              FastInteger digitLength2 =
                this.helper.CreateShiftAccumulator(op2MantAbs).GetDigitLength();
              if (FastInteger.Copy(fastOp2Exp)
                  .Add(digitLength2).AddInt(2).CompareTo(fastOp1Exp) <
                0) {
                // second operand's mantissa can't reach the
                // first operand's mantissa, so the exponent can be
                // raised without affecting the result
                FastInteger tmp = FastInteger.Copy(fastOp1Exp)
                .SubtractInt(8).Subtract(digitLength2).Subtract(maxPrecision);
                FastInteger newDiff =
                  FastInteger.Copy(tmp).Subtract(fastOp1Exp).Abs();
                if (newDiff.CompareTo(expdiff) < 0) {
                  // At this point, both operands have the same sign
                  return (signA < 0) ? -1 : 1;
                }
              }
            }
          }
          expcmp = op1Exponent.CompareTo((EInteger)op2Exponent);
        }
      }
      if (expcmp > 0) {
        EInteger newmant =
          this.RescaleByExponentDiff(
op1Mantissa,
op1Exponent,
op2Exponent);
        if (newmant == null) {
          if (reportOOM) {
            throw new OutOfMemoryException("Result requires too much memory");
          }
          return -2;
        }
        mantcmp = newmant.CompareTo(op2Mantissa);
        return (signA < 0) ? -mantcmp : mantcmp;
      } else {
        EInteger newmant = this.RescaleByExponentDiff(
            op2Mantissa,
            op1Exponent,
            op2Exponent);
        if (newmant == null) {
          if (reportOOM) {
            throw new OutOfMemoryException("Result requires too much memory");
          }
          return -2;
        }
        mantcmp = op1Mantissa.CompareTo(newmant);
        return (signA < 0) ? -mantcmp : mantcmp;
      }
    }

    private T DivideInternal(
T thisValue,
T divisor,
EContext ctx,
int integerMode,
EInteger desiredExponent) {
      T ret = this.DivisionHandleSpecial(thisValue, divisor, ctx);
      if ((object)ret != (object)default(T)) {
        return ret;
      }
      int signA = this.helper.GetSign(thisValue);
      int signB = this.helper.GetSign(divisor);
      if (signB == 0) {
        if (signA == 0) {
          return this.SignalInvalid(ctx);
        }
        bool flagsNeg = ((this.helper.GetFlags(thisValue) &
           BigNumberFlags.FlagNegative) != 0) ^
                  ((this.helper.GetFlags(divisor) &
            BigNumberFlags.FlagNegative) != 0);
        return this.SignalDivideByZero(ctx, flagsNeg);
      }
      int radix = this.thisRadix;
      if (signA == 0) {
        T retval = default(T);
        if (integerMode == IntegerModeFixedScale) {
          int newflags = (this.helper.GetFlags(thisValue) &
                BigNumberFlags.FlagNegative) ^ (this.helper.GetFlags(divisor) &
             BigNumberFlags.FlagNegative);
          retval = this.helper.CreateNewWithFlags(
            EInteger.Zero,
            desiredExponent,
            newflags);
        } else {
          EInteger dividendExp = this.helper.GetExponent(thisValue);
          EInteger divisorExp = this.helper.GetExponent(divisor);
          int newflags = (this.helper.GetFlags(thisValue) &
                BigNumberFlags.FlagNegative) ^ (this.helper.GetFlags(divisor) &
             BigNumberFlags.FlagNegative);
          retval =
            this.RoundToPrecision(
              this.helper.CreateNewWithFlags(EInteger.Zero, dividendExp - (EInteger)divisorExp, newflags),
              ctx);
        }
        return retval;
      } else {
        EInteger mantissaDividend = this.helper.GetMantissa(thisValue);
        EInteger mantissaDivisor = this.helper.GetMantissa(divisor);
        FastInteger expDividend =
          FastInteger.FromBig(this.helper.GetExponent(thisValue));
        FastInteger expDivisor =
          FastInteger.FromBig(this.helper.GetExponent(divisor));
        FastInteger expdiff =
            FastInteger.Copy(expDividend).Subtract(expDivisor);
        var adjust = new FastInteger(0);
        var result = new FastInteger(0);
        FastInteger naturalExponent = FastInteger.Copy(expdiff);
        bool hasPrecision = ctx != null && ctx.Precision.Sign != 0;
        bool resultNeg = (this.helper.GetFlags(thisValue) &
                BigNumberFlags.FlagNegative) != (this.helper.GetFlags(divisor) &
           BigNumberFlags.FlagNegative);
        FastInteger fastPrecision = (!hasPrecision) ? new FastInteger(0) :
          FastInteger.FromBig(ctx.Precision);
        FastInteger dividendPrecision = null;
        FastInteger divisorPrecision = null;
        if (integerMode == IntegerModeFixedScale) {
          FastInteger shift;
          EInteger rem;
          FastInteger fastDesiredExponent =
              FastInteger.FromBig(desiredExponent);
          if (ctx != null && ctx.HasFlags &&
              fastDesiredExponent.CompareTo(naturalExponent) > 0) {
            // Treat as rounded if the desired exponent is greater
            // than the "ideal" exponent
            ctx.Flags |= EContext.FlagRounded;
          }
          if (expdiff.CompareTo(fastDesiredExponent) <= 0) {
            shift = FastInteger.Copy(fastDesiredExponent).Subtract(expdiff);
            EInteger quo;
            {
              EInteger[] divrem = mantissaDividend.DivRem(mantissaDivisor);
              quo = divrem[0];
              rem = divrem[1];
            }
            return this.RoundToScale(
quo,
rem,
mantissaDivisor,
desiredExponent,
shift,
resultNeg,
ctx);
          }
          if (ctx != null && ctx.Precision.Sign != 0 &&
           FastInteger.Copy(expdiff).SubtractInt(8).CompareTo(fastPrecision) >
              0) {
            // NOTE: 8 guard digits
            // Result would require a too-high precision since
            // exponent difference is much higher
            return this.SignalInvalidWithMessage(
ctx,
"Result can't fit the precision");
          } else {
            shift = FastInteger.Copy(expdiff).Subtract(fastDesiredExponent);
            mantissaDividend =
              this.TryMultiplyByRadixPower(mantissaDividend, shift);
            if (mantissaDividend == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            EInteger quo;
            {
              EInteger[] divrem = mantissaDividend.DivRem(mantissaDivisor);
              quo = divrem[0];
              rem = divrem[1];
            }
            return this.RoundToScale(
              quo,
 rem,
 mantissaDivisor,
 desiredExponent,
 new FastInteger(0),
 resultNeg,
 ctx);
          }
        }
        if (integerMode == IntegerModeRegular) {
          EInteger rem = null;
          EInteger quo = null;
          // DebugUtility.Log("div=" +
          // (mantissaDividend.getUnsignedBitLength()) + " divs= " +
          // (mantissaDivisor.getUnsignedBitLength()));
          {
            EInteger[] divrem = mantissaDividend.DivRem(mantissaDivisor);
            quo = divrem[0];
            rem = divrem[1];
          }
          if (rem.IsZero) {
            // Dividend is divisible by divisor
            if (resultNeg) {
              quo = -quo;
            }
            return this.RoundToPrecision(
              this.helper.CreateNewWithFlags(quo, naturalExponent.AsBigInteger(), resultNeg ? BigNumberFlags.FlagNegative : 0),
              ctx);
          }
          rem = null;
          quo = null;
          if (hasPrecision) {
#if DEBUG
            if (ctx == null) {
              throw new ArgumentNullException("ctx");
            }
#endif
            EInteger divid = mantissaDividend;
            FastInteger shift = FastInteger.FromBig(ctx.Precision);
            dividendPrecision =
                   this.helper.CreateShiftAccumulator(mantissaDividend)
                    .GetDigitLength();
            divisorPrecision =
              this.helper.CreateShiftAccumulator(mantissaDivisor)
              .GetDigitLength();
            if (dividendPrecision.CompareTo(divisorPrecision) <= 0) {
              divisorPrecision.Subtract(dividendPrecision);
              divisorPrecision.Increment();
              shift.Add(divisorPrecision);
              divid = this.TryMultiplyByRadixPower(divid, shift);
              if (divid == null) {
                return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
              }
            } else {
              // Already greater than divisor precision
              dividendPrecision.Subtract(divisorPrecision);
              if (dividendPrecision.CompareTo(shift) <= 0) {
                shift.Subtract(dividendPrecision);
                shift.Increment();
                divid = this.TryMultiplyByRadixPower(divid, shift);
                if (divid == null) {
                  return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
                }
              } else {
                // no need to shift
                shift.SetInt(0);
              }
            }
            dividendPrecision =
              this.helper.CreateShiftAccumulator(divid).GetDigitLength();
            divisorPrecision =
                 this.helper.CreateShiftAccumulator(mantissaDivisor)
                    .GetDigitLength();
            if (shift.Sign != 0 || quo == null) {
              // if shift isn't zero, recalculate the quotient
              // and remainder
              {
                EInteger[] divrem = divid.DivRem(mantissaDivisor);
                quo = divrem[0];
                rem = divrem[1];
              }
            }
            // DebugUtility.Log(String.Format("" + divid + "" +
            // mantissaDivisor + " -> quo= " + quo + " rem= " +
            // (rem)));
            int[] digitStatus = this.RoundToScaleStatus(
              rem,
              mantissaDivisor,
              ctx);
            if (digitStatus == null) {
              return this.SignalInvalidWithMessage(
ctx,
"Rounding was required");
            }
            FastInteger natexp =
                  FastInteger.Copy(naturalExponent).Subtract(shift);
            EContext ctxcopy = ctx.WithBlankFlags();
            T retval2 = this.helper.CreateNewWithFlags(
              quo,
              natexp.AsBigInteger(),
              resultNeg ? BigNumberFlags.FlagNegative : 0);
            retval2 = this.RoundToPrecisionInternal(
              retval2,
              digitStatus[0],
              digitStatus[1],
              null,
              false,
              ctxcopy);
            if ((ctxcopy.Flags & EContext.FlagInexact) != 0) {
              if (ctx.HasFlags) {
                ctx.Flags |= ctxcopy.Flags;
              }
              return retval2;
            }
            if (ctx.HasFlags) {
              ctx.Flags |= ctxcopy.Flags;
              ctx.Flags &= ~EContext.FlagRounded;
            }
            return this.ReduceToPrecisionAndIdealExponent(
              retval2,
              ctx,
              rem.IsZero ? null : fastPrecision,
              expdiff);
          }
        }
        // Rest of method assumes unlimited precision
        // and IntegerModeRegular
        int mantcmp = mantissaDividend.CompareTo(mantissaDivisor);
        if (mantcmp < 0) {
          // dividend mantissa is less than divisor mantissa
          dividendPrecision =
               this.helper.CreateShiftAccumulator(mantissaDividend)
                    .GetDigitLength();
          divisorPrecision =
            this.helper.CreateShiftAccumulator(mantissaDivisor)
            .GetDigitLength();
          divisorPrecision.Subtract(dividendPrecision);
          if (divisorPrecision.IsValueZero) {
            divisorPrecision.Increment();
          }
          // multiply dividend mantissa so precisions are the same
          // (except if they're already the same, in which case multiply
          // by radix)
          mantissaDividend = this.TryMultiplyByRadixPower(
            mantissaDividend,
            divisorPrecision);
          if (mantissaDividend == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          adjust.Add(divisorPrecision);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is still less, multiply once more
            if (radix == 2) {
              mantissaDividend <<= 1;
            } else {
              mantissaDividend *= (EInteger)radix;
            }
            adjust.Increment();
          }
        } else if (mantcmp > 0) {
          // dividend mantissa is greater than divisor mantissa
          dividendPrecision =
               this.helper.CreateShiftAccumulator(mantissaDividend)
                    .GetDigitLength();
          divisorPrecision =
            this.helper.CreateShiftAccumulator(mantissaDivisor)
            .GetDigitLength();
          dividendPrecision.Subtract(divisorPrecision);
          EInteger oldMantissaB = mantissaDivisor;
          mantissaDivisor = this.TryMultiplyByRadixPower(
            mantissaDivisor,
            dividendPrecision);
          if (mantissaDivisor == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          adjust.Subtract(dividendPrecision);
          if (mantissaDividend.CompareTo(mantissaDivisor) < 0) {
            // dividend mantissa is now less, divide by radix power
            if (dividendPrecision.CompareToInt(1) == 0) {
              // no need to divide here, since that would just undo
              // the multiplication
              mantissaDivisor = oldMantissaB;
            } else {
              var bigpow = (EInteger)radix;
              mantissaDivisor /= bigpow;
            }
            adjust.Increment();
          }
        }
        if (mantcmp == 0) {
          result = new FastInteger(1);
          mantissaDividend = EInteger.Zero;
        } else {
          {
            if (!this.helper.HasTerminatingRadixExpansion(
              mantissaDividend,
              mantissaDivisor)) {
              return this.SignalInvalidWithMessage(
ctx,
"Result would have a nonterminating expansion");
            }
            FastInteger divs = FastInteger.FromBig(mantissaDivisor);
            FastInteger divd = FastInteger.FromBig(mantissaDividend);
            bool divisorFits = divs.CanFitInInt32();
            int smallDivisor = divisorFits ? divs.AsInt32() : 0;
            int halfRadix = radix / 2;
            FastInteger divsHalfRadix = null;
            if (radix != 2) {
              divsHalfRadix =
                FastInteger.FromBig(mantissaDivisor).Multiply(halfRadix);
            }
            while (true) {
              var remainderZero = false;
              var count = 0;
              if (divd.CanFitInInt32()) {
                if (divisorFits) {
                  int smallDividend = divd.AsInt32();
                  count = smallDividend / smallDivisor;
                  divd.SetInt(smallDividend % smallDivisor);
                } else {
                  count = 0;
                }
              } else {
                if (divsHalfRadix != null) {
                  count += halfRadix * divd.RepeatedSubtract(divsHalfRadix);
                }
                count += divd.RepeatedSubtract(divs);
              }
              result.AddInt(count);
              remainderZero = divd.IsValueZero;
              if (remainderZero && adjust.Sign >= 0) {
                mantissaDividend = divd.AsBigInteger();
                break;
              }
              adjust.Increment();
              result.Multiply(radix);
              divd.Multiply(radix);
            }
          }
        }
        // mantissaDividend now has the remainder
        FastInteger exp = FastInteger.Copy(expdiff).Subtract(adjust);
        ERounding rounding = (ctx == null) ? ERounding.HalfEven : ctx.Rounding;
        var lastDiscarded = 0;
        var olderDiscarded = 0;
        if (!mantissaDividend.IsZero) {
          if (rounding == ERounding.HalfDown || rounding == ERounding.HalfEven||
              rounding == ERounding.HalfUp) {
            EInteger halfDivisor = mantissaDivisor >> 1;
            int cmpHalf = mantissaDividend.CompareTo(halfDivisor);
            if ((cmpHalf == 0) && mantissaDivisor.IsEven) {
              // remainder is exactly half
              lastDiscarded = radix / 2;
              olderDiscarded = 0;
            } else if (cmpHalf > 0) {
              // remainder is greater than half
              lastDiscarded = radix / 2;
              olderDiscarded = 1;
            } else {
              // remainder is less than half
              lastDiscarded = 0;
              olderDiscarded = 1;
            }
          } else {
            if (rounding == ERounding.None) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
            lastDiscarded = 1;
            olderDiscarded = 1;
          }
        }
        EInteger bigResult = result.AsBigInteger();
        if (ctx != null && ctx.HasFlags && exp.CompareTo(expdiff) > 0) {
          // Treat as rounded if the true exponent is greater
          // than the "ideal" exponent
          ctx.Flags |= EContext.FlagRounded;
        }
        EInteger bigexp = exp.AsBigInteger();
        T retval = this.helper.CreateNewWithFlags(
          bigResult,
          bigexp,
          resultNeg ? BigNumberFlags.FlagNegative : 0);
        return this.RoundToPrecisionInternal(
retval,
lastDiscarded,
olderDiscarded,
null,
false,
ctx);
      }
    }

    private T DivisionHandleSpecial(
T thisValue,
T other,
EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0 && (otherFlags &
  BigNumberFlags.FlagInfinity) != 0) {
          // Attempt to divide infinity by infinity
          return this.SignalInvalid(ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.EnsureSign(
thisValue,
((thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative) != 0);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Divisor is infinity, so result will be epsilon
          if (ctx != null && ctx.HasExponentRange && ctx.Precision.Sign > 0) {
            if (ctx.HasFlags) {
              ctx.Flags |= EContext.FlagClamped;
            }
            EInteger bigexp = ctx.EMin;
            EInteger bigprec = ctx.Precision;
            if (ctx.AdjustExponent) {
              bigexp -= (EInteger)bigprec;
              bigexp += EInteger.One;
            }
            thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
            return this.helper.CreateNewWithFlags(
EInteger.Zero,
bigexp,
thisFlags);
          }
          thisFlags = (thisFlags ^ otherFlags) & BigNumberFlags.FlagNegative;
          return this.RoundToPrecision(
   this.helper.CreateNewWithFlags(EInteger.Zero, EInteger.Zero, thisFlags),
   ctx);
        }
      }
      return default(T);
    }

    private T EnsureSign(T val, bool negative) {
      if (val == null) {
        return val;
      }
      int flags = this.helper.GetFlags(val);
      if ((negative && (flags & BigNumberFlags.FlagNegative) == 0) ||
          (!negative && (flags & BigNumberFlags.FlagNegative) != 0)) {
        flags &= ~BigNumberFlags.FlagNegative;
        flags |= negative ? BigNumberFlags.FlagNegative : 0;
        return this.helper.CreateNewWithFlags(
this.helper.GetMantissa(val),
this.helper.GetExponent(val),
flags);
      }
      return val;
    }

    private T ExpInternal(
T thisValue,
EInteger workingPrecision,
EContext ctx) {
      T one = this.helper.ValueOf(1);
      int precisionAdd = this.thisRadix == 2 ? 18 : 12;
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        workingPrecision + (EInteger)precisionAdd)
        .WithRounding(ERounding.OddOrZeroFiveUp);
      var bigintN = (EInteger)2;
      EInteger facto = EInteger.One;
      // Guess starts with 1 + thisValue
      T guess = this.Add(one, thisValue, null);
      T lastGuess = guess;
      T pow = thisValue;
      var more = true;
      var lastCompare = 0;
      var vacillations = 0;
      while (more) {
        lastGuess = guess;
        // Iterate by:
        // newGuess = guess + (thisValue^n/factorial(n))
        // (n starts at 2 and increases by 1 after
        // each iteration)
        pow = this.Multiply(pow, thisValue, ctxdiv);
        facto *= (EInteger)bigintN;
        T tmp = this.Divide(
          pow,
          this.helper.CreateNewWithFlags(facto, EInteger.Zero, 0),
          ctxdiv);
        T newGuess = this.Add(guess, tmp, ctxdiv);
        //DebugUtility.Log("newguess" +
        // this.helper.GetMantissa(newGuess)+" ctxdiv " +
        // ctxdiv.Precision);
        //DebugUtility.Log("newguess " + newGuess);
        // DebugUtility.Log("newguessN " + NextPlus(newGuess,ctxdiv));
        {
          int guessCmp = this.CompareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                    guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        guess = newGuess;
        if (more) {
          bigintN += EInteger.One;
        }
      }
      return this.RoundToPrecision(guess, ctx);
    }

    private T ExtendPrecision(T thisValue, EContext ctx) {
      if (ctx == null || !ctx.HasMaxPrecision) {
        return this.RoundToPrecision(thisValue, ctx);
      }
      EInteger mant = this.helper.GetMantissa(thisValue);
      FastInteger digits =
        this.helper.CreateShiftAccumulator(mant).GetDigitLength();
      FastInteger fastPrecision = FastInteger.FromBig(ctx.Precision);
      EInteger exponent = this.helper.GetExponent(thisValue);
      if (digits.CompareTo(fastPrecision) < 0) {
        fastPrecision.Subtract(digits);
        mant = this.TryMultiplyByRadixPower(mant, fastPrecision);
        if (mant == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        EInteger bigPrec = fastPrecision.AsBigInteger();
        exponent -= (EInteger)bigPrec;
      }
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= EContext.FlagRounded;
        ctx.Flags |= EContext.FlagInexact;
      }
      return this.RoundToPrecision(
        this.helper.CreateNewWithFlags(mant, exponent, 0),
        ctx);
    }

    private T HandleNotANumber(T thisValue, T other, EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      // Check this value then the other value for signaling NaN
      if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(thisValue, ctx);
      }
      if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(other, ctx);
      }
      // Check this value then the other value for quiet NaN
      return ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) ?
        this.ReturnQuietNaN(thisValue, ctx) : (((otherFlags &
                BigNumberFlags.FlagQuietNaN) != 0) ? this.ReturnQuietNaN(
other,
ctx) : default(T));
    }

    private bool IsFinite(T val) {
      return (this.helper.GetFlags(val) & BigNumberFlags.FlagSpecial) == 0;
    }

    private bool IsNegative(T val) {
      return (this.helper.GetFlags(val) & BigNumberFlags.FlagNegative) != 0;
    }

    private bool IsWithinExponentRangeForPow(
      T thisValue,
      EContext ctx) {
      if (ctx == null || !ctx.HasExponentRange) {
        return true;
      }
      FastInteger digits =

  this.helper.CreateShiftAccumulator(this.helper.GetMantissa(thisValue))
        .GetDigitLength();
      EInteger exp = this.helper.GetExponent(thisValue);
      FastInteger fi = FastInteger.FromBig(exp);
      if (ctx.AdjustExponent) {
        fi.Add(digits);
        fi.Decrement();
      }
      // DebugUtility.Log("" + exp + " -> " + fi);
      if (fi.Sign < 0) {
        fi.Negate().Divide(2).Negate();
        // DebugUtility.Log("" + exp + " II -> " + fi);
      }
      exp = fi.AsBigInteger();
      return exp.CompareTo(ctx.EMin) >= 0 && exp.CompareTo(ctx.EMax) <= 0;
    }

    private T LnInternal(
T thisValue,
EInteger workingPrecision,
EContext ctx) {
      var more = true;
      var lastCompare = 0;
      var vacillations = 0;
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        workingPrecision + (EInteger)6)
        .WithRounding(ERounding.OddOrZeroFiveUp);
      T z = this.Add(this.NegateRaw(thisValue), this.helper.ValueOf(1), null);
      T zpow = this.Multiply(z, z, ctxdiv);
      T guess = this.NegateRaw(z);
      T lastGuess = default(T);
      var denom = (EInteger)2;
      while (more) {
        lastGuess = guess;
        T tmp = this.Divide(
zpow,
this.helper.CreateNewWithFlags(denom, EInteger.Zero, 0),
ctxdiv);
        T newGuess = this.Add(guess, this.NegateRaw(tmp), ctxdiv);
        {
          int guessCmp = this.CompareTo(lastGuess, newGuess);
          if (guessCmp == 0) {
            more = false;
          } else if ((guessCmp > 0 && lastCompare < 0) || (lastCompare > 0 &&
                    guessCmp < 0)) {
            // Guesses are vacillating
            ++vacillations;
            more &= vacillations <= 3 || guessCmp <= 0;
          }
          lastCompare = guessCmp;
        }
        guess = newGuess;
        if (more) {
          zpow = this.Multiply(zpow, z, ctxdiv);
          denom += EInteger.One;
        }
      }
      return this.RoundToPrecision(guess, ctx);
    }

    private T LnTenConstant(EContext ctx) {
#if DEBUG
      if (ctx == null) {
        throw new ArgumentNullException("ctx");
      }
#endif
      T thisValue = this.helper.ValueOf(10);
      FastInteger error;
      EInteger bigError;
      error = new FastInteger(10);
      bigError = error.AsBigInteger();
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.Precision + bigError)
        .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
      for (var i = 0; i < 9; ++i) {
        thisValue = this.SquareRoot(thisValue, ctxdiv.WithUnlimitedExponents());
      }
      // Find -Ln(1/thisValue)
      thisValue = this.Divide(this.helper.ValueOf(1), thisValue, ctxdiv);
      thisValue = this.LnInternal(thisValue, ctxdiv.Precision, ctxdiv);
      thisValue = this.NegateRaw(thisValue);
      thisValue = this.Multiply(thisValue, this.helper.ValueOf(1 << 9), ctx);
      if (ctx.HasFlags) {
        ctx.Flags |= EContext.FlagInexact;
        ctx.Flags |= EContext.FlagRounded;
      }
      return thisValue;
    }

    private T MinMaxHandleSpecial(
T thisValue,
T otherValue,
EContext ctx,
bool isMinOp,
bool compareAbs) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(otherValue);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        // Check this value then the other value for signaling NaN
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(otherValue, ctx);
        }
        // Check this value then the other value for quiet NaN
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
            // both values are quiet NaN
            return this.ReturnQuietNaN(thisValue, ctx);
          }
          // return "other" for being numeric
          return this.RoundToPrecision(otherValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          // At this point, "thisValue" can't be NaN,
          // return "thisValue" for being numeric
          return this.RoundToPrecision(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs && (otherFlags & BigNumberFlags.FlagInfinity) == 0) {
            // treat this as larger
            return isMinOp ? this.RoundToPrecision(otherValue, ctx) : thisValue;
          }
          // This value is infinity
          if (isMinOp) {
            // if negative, will be less than every other number
            return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ?
              thisValue : this.RoundToPrecision(otherValue, ctx);
            // if positive, will be greater
          }
          // if positive, will be greater than every other number
          return ((thisFlags & BigNumberFlags.FlagNegative) == 0) ?
            thisValue : this.RoundToPrecision(otherValue, ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          if (compareAbs) {
            // treat this as larger (the first value
            // won't be infinity at this point
            return isMinOp ? this.RoundToPrecision(thisValue, ctx) : otherValue;
          }
          return isMinOp ? (((otherFlags & BigNumberFlags.FlagNegative) ==
                    0) ? this.RoundToPrecision(thisValue, ctx) :
             otherValue) : (((otherFlags & BigNumberFlags.FlagNegative) !=
                0) ? this.RoundToPrecision(thisValue, ctx) : otherValue);
        }
      }
      return default(T);
    }

    private T MultiplyAddHandleSpecial(
T op1,
T op2,
T op3,
EContext ctx) {
      int op1Flags = this.helper.GetFlags(op1);
      // Check operands in order for signaling NaN
      if ((op1Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op1, ctx);
      }
      int op2Flags = this.helper.GetFlags(op2);
      if ((op2Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op2, ctx);
      }
      int op3Flags = this.helper.GetFlags(op3);
      if ((op3Flags & BigNumberFlags.FlagSignalingNaN) != 0) {
        return this.SignalingNaNInvalid(op3, ctx);
      }
      // Check operands in order for quiet NaN
      if ((op1Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(op1, ctx);
      }
      if ((op2Flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return this.ReturnQuietNaN(op2, ctx);
      }
      // Check multiplying infinity by 0 (important to check
      // now before checking third operand for quiet NaN because
      // this signals invalid operation and the operation starts
      // with multiplying only the first two operands)
      if ((op1Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op2Flags & BigNumberFlags.FlagSpecial) == 0 &&
            this.helper.GetMantissa(op2).IsZero) {
          return this.SignalInvalid(ctx);
        }
      }
      if ((op2Flags & BigNumberFlags.FlagInfinity) != 0) {
        // Attempt to multiply infinity by 0
        if ((op1Flags & BigNumberFlags.FlagSpecial) == 0 &&
            this.helper.GetMantissa(op1).IsZero) {
          return this.SignalInvalid(ctx);
        }
      }
      // Now check third operand for quiet NaN
      return ((op3Flags & BigNumberFlags.FlagQuietNaN) != 0) ?
        this.ReturnQuietNaN(op3, ctx) : default(T);
    }

    private T NegateRaw(T val) {
      if (val == null) {
        return val;
      }
      int sign = this.helper.GetFlags(val) & BigNumberFlags.FlagNegative;
      return this.helper.CreateNewWithFlags(
this.helper.GetMantissa(val),
this.helper.GetExponent(val),
sign == 0 ? BigNumberFlags.FlagNegative : 0);
    }

    private T PowerIntegral(
T thisValue,
EInteger powIntBig,
EContext ctx) {
      int sign = powIntBig.Sign;
      T one = this.helper.ValueOf(1);
      if (sign == 0) {
        // however 0 to the power of 0 is undefined
        return this.RoundToPrecision(one, ctx);
      }
      if (powIntBig.Equals(EInteger.One)) {
        return this.RoundToPrecision(thisValue, ctx);
      }
      if (powIntBig.Equals((EInteger)2)) {
        return this.Multiply(thisValue, thisValue, ctx);
      }
      if (powIntBig.Equals((EInteger)3)) {
        return this.Multiply(
thisValue,
this.Multiply(thisValue, thisValue, null),
ctx);
      }
      bool retvalNeg = this.IsNegative(thisValue) && !powIntBig.IsEven;
      FastInteger error = this.helper.CreateShiftAccumulator(
        powIntBig.Abs()).GetDigitLength();
      error.AddInt(18);
      EInteger bigError = error.AsBigInteger();
      EContext ctxdiv = SetPrecisionIfLimited(
        ctx,
        ctx.Precision + (EInteger)bigError)
        .WithRounding(ERounding.OddOrZeroFiveUp).WithBlankFlags();
      if (sign < 0) {
        // Use the reciprocal for negative powers
        thisValue = this.Divide(one, thisValue, ctxdiv);
        if ((ctxdiv.Flags & EContext.FlagOverflow) != 0) {
          return this.SignalOverflow2(ctx, retvalNeg);
        }
        powIntBig = -powIntBig;
      }
      T r = one;
      while (!powIntBig.IsZero) {
        if (!powIntBig.IsEven) {
          r = this.Multiply(r, thisValue, ctxdiv);
          if ((ctxdiv.Flags & EContext.FlagOverflow) != 0) {
            return this.SignalOverflow2(ctx, retvalNeg);
          }
        }
        powIntBig >>= 1;
        if (!powIntBig.IsZero) {
          ctxdiv.Flags = 0;
          T tmp = this.Multiply(thisValue, thisValue, ctxdiv);
          if ((ctxdiv.Flags & EContext.FlagOverflow) != 0) {
            // Avoid multiplying too huge numbers with
            // limited exponent range
            return this.SignalOverflow2(ctx, retvalNeg);
          }
          thisValue = tmp;
        }
        //DebugUtility.Log("r="+r);
      }
      return this.RoundToPrecision(r, ctx);
    }

    private T ReduceToPrecisionAndIdealExponent(
      T thisValue,
      EContext ctx,
      FastInteger precision,
      FastInteger idealExp) {
      T ret = this.RoundToPrecision(thisValue, ctx);
      if (ret != null && (this.helper.GetFlags(ret) &
                    BigNumberFlags.FlagSpecial) == 0) {
        EInteger bigmant = this.helper.GetMantissa(ret);
        FastInteger exp = FastInteger.FromBig(this.helper.GetExponent(ret));
        int radix = this.thisRadix;
        if (bigmant.IsZero) {
          exp = new FastInteger(0);
        } else {
          FastInteger digits = (precision == null) ? null :
            this.helper.CreateShiftAccumulator(bigmant).GetDigitLength();
          bigmant = DecimalUtility.ReduceTrailingZeros(
            bigmant,
            exp,
            radix,
            digits,
            precision,
            idealExp);
        }
        int flags = this.helper.GetFlags(thisValue);
        ret = this.helper.CreateNewWithFlags(
bigmant,
exp.AsBigInteger(),
flags);
        if (ctx != null && ctx.ClampNormalExponents) {
          EContext ctxtmp = ctx.WithBlankFlags();
          ret = this.RoundToPrecision(ret, ctxtmp);
          if (ctx.HasFlags) {
            ctx.Flags |= ctxtmp.Flags & ~EContext.FlagClamped;
          }
        }
        ret = this.EnsureSign(ret, (flags & BigNumberFlags.FlagNegative) != 0);
      }
      return ret;
    }

    private T RemainderHandleSpecial(
T thisValue,
T other,
EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      int otherFlags = this.helper.GetFlags(other);
      if (((thisFlags | otherFlags) & BigNumberFlags.FlagSpecial) != 0) {
        T result = this.HandleNotANumber(thisValue, other, ctx);
        if ((object)result != (object)default(T)) {
          return result;
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.SignalInvalid(ctx);
        }
        if ((otherFlags & BigNumberFlags.FlagInfinity) != 0) {
          return this.RoundToPrecision(thisValue, ctx);
        }
      }
      return this.helper.GetMantissa(other).IsZero ? this.SignalInvalid(ctx) :
        default(T);
    }

    private EInteger RescaleByExponentDiff(
      EInteger mantissa,
      EInteger e1,
      EInteger e2) {
      if (mantissa.Sign == 0) {
        return EInteger.Zero;
      }
      FastInteger diff = FastInteger.FromBig(e1).SubtractBig(e2).Abs();
      return this.TryMultiplyByRadixPower(mantissa, diff);
    }

    private static EInteger AbsInt(EInteger ei) {
      return ei.Abs();
    }

    private T ReturnQuietNaN(T thisValue, EContext ctx) {
      EInteger mant = this.helper.GetMantissa(thisValue);
      var mantChanged = false;
      if (!mant.IsZero && ctx != null && ctx.HasMaxPrecision) {
        FastInteger compPrecision = FastInteger.FromBig(ctx.Precision);
        if (this.helper.CreateShiftAccumulator(mant).GetDigitLength()
            .CompareTo(compPrecision) >= 0) {
          // Mant's precision is higher than the maximum precision
          EInteger limit = this.TryMultiplyByRadixPower(
            EInteger.One,
            compPrecision);
          if (limit == null) {
            // Limit can't be allocated
            return this.SignalInvalidWithMessage(
ctx,
"Result requires too much memory");
          }
          if (mant.CompareTo(limit) >= 0) {
            mant %= (EInteger)limit;
            mantChanged = true;
          }
        }
      }
      int flags = this.helper.GetFlags(thisValue);
      if (!mantChanged && (flags & BigNumberFlags.FlagQuietNaN) != 0) {
        return thisValue;
      }
      flags &= BigNumberFlags.FlagNegative;
      flags |= BigNumberFlags.FlagQuietNaN;
      return this.helper.CreateNewWithFlags(mant, EInteger.Zero, flags);
    }

    private bool Round(
IShiftAccumulator accum,
ERounding rounding,
bool neg,
FastInteger fastint) {
      var incremented = false;
      if (rounding == ERounding.HalfEven) {
        int radix = this.thisRadix;
        if (accum.LastDiscardedDigit >= (radix / 2)) {
          if (accum.LastDiscardedDigit > (radix / 2) ||
              accum.OlderDiscardedDigits != 0) {
            incremented = true;
          } else {
            incremented |= !fastint.IsEvenNumber;
          }
        }
      } else if (rounding == ERounding.ZeroFiveUp ||
        (rounding == ERounding.OddOrZeroFiveUp && this.thisRadix != 2)) {
        int radix = this.thisRadix;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            int lastDigit =
              FastInteger.Copy(fastint).Remainder(radix).AsInt32();
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      } else if (rounding != ERounding.Down) {
        incremented = this.RoundGivenDigits(
          accum.LastDiscardedDigit,
          accum.OlderDiscardedDigits,
          rounding,
          neg,
          EInteger.Zero);
      }
      return incremented;
    }

    private bool RoundGivenBigInt(
IShiftAccumulator accum,
ERounding rounding,
bool neg,
EInteger bigval) {
      return this.RoundGivenDigits(
accum.LastDiscardedDigit,
accum.OlderDiscardedDigits,
rounding,
neg,
bigval);
    }

    private bool RoundGivenDigits(
int lastDiscarded,
int olderDiscarded,
ERounding rounding,
bool neg,
EInteger bigval) {
      var incremented = false;
      int radix = this.thisRadix;
      if (rounding == ERounding.OddOrZeroFiveUp) {
        rounding = (radix == 2) ? ERounding.Odd : ERounding.ZeroFiveUp;
      }
      if (rounding == ERounding.HalfUp) {
        incremented |= lastDiscarded >= (radix / 2);
      } else if (rounding == ERounding.HalfEven) {
        // DebugUtility.Log("rgd last= " + lastDiscarded + " older=" +
        // olderDiscarded + " even= " + (bigval.IsEven));
        // DebugUtility.Log("--- --- " +
        // (BitMantissa(helper.CreateNewWithFlags(bigval, BigInteger.Zero,
        // 0))));
        if (lastDiscarded >= (radix / 2)) {
          if (lastDiscarded > (radix / 2) || olderDiscarded != 0) {
            incremented = true;
          } else {
            incremented |= !bigval.IsEven;
          }
        }
      } else if (rounding == ERounding.Ceiling) {
        incremented |= !neg && (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == ERounding.Floor) {
        incremented |= neg && (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == ERounding.HalfDown) {
        incremented |= lastDiscarded > (radix / 2) || (lastDiscarded ==
                (radix / 2) && olderDiscarded != 0);
      } else if (rounding == ERounding.Up) {
        incremented |= (lastDiscarded | olderDiscarded) != 0;
      } else if (rounding == ERounding.Odd) {
        incremented |= (lastDiscarded | olderDiscarded) != 0 && bigval.IsEven;
      } else if (rounding == ERounding.ZeroFiveUp) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          if (radix == 2) {
            incremented = true;
          } else {
            EInteger bigdigit = bigval % (EInteger)radix;
            var lastDigit = (int)bigdigit;
            if (lastDigit == 0 || lastDigit == (radix / 2)) {
              incremented = true;
            }
          }
        }
      }
      return incremented;
    }
    // binaryPrec means whether precision is the number of bits and not
    // digits
    private T RoundToPrecisionInternal(
T thisValue,
int lastDiscarded,
int olderDiscarded,
FastInteger shift,
bool adjustNegativeZero,
EContext ctx) {
      // If context has unlimited precision and exponent range,
      // and no discarded digits or shifting
      bool unlimitedPrecisionExp = (ctx == null ||
         (!ctx.HasMaxPrecision && !ctx.HasExponentRange));
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          if (ctx != null && ctx.HasFlags) {
            ctx.Flags |= EContext.FlagInvalid;
          }
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          return thisValue;
        }
      }
      if (unlimitedPrecisionExp &&
         (lastDiscarded | olderDiscarded) == 0 &&
         (shift == null || shift.IsValueZero)) {
        return thisValue;
      }
      if (unlimitedPrecisionExp &&
        (ctx == null || (!ctx.HasFlags && ctx.Traps == 0)) &&
        (shift == null || shift.IsValueZero)) {
        ERounding er = (ctx == null) ? ERounding.HalfDown : ctx.Rounding;
        bool negative = (thisFlags & BigNumberFlags.FlagNegative) != 0;
        bool negzero = adjustNegativeZero && negative &&
        this.helper.GetMantissa(thisValue).IsZero &&
        (er != ERounding.Floor);
        if (!negzero) {
          if (er == ERounding.Down) {
            return thisValue;
          }
          if (this.thisRadix == 10 && (er == ERounding.HalfEven) &&
            lastDiscarded < 5) {
            return thisValue;
          }
          if (this.thisRadix == 10 && (er == ERounding.HalfEven) &&
            (lastDiscarded > 5 || olderDiscarded != 0)) {
            EInteger bm = this.helper.GetMantissa(thisValue);
            bm += EInteger.FromInt64(1);
            return this.helper.CreateNewWithFlags(
              bm,
              this.helper.GetExponent(thisValue),
              thisFlags);
          }
          if (this.thisRadix == 2 && (er == ERounding.HalfEven) &&
            lastDiscarded == 0) {
            return thisValue;
          }
          if (!this.RoundGivenDigits(
lastDiscarded, olderDiscarded,

 er,

 negative,
            this.helper.GetMantissa(thisValue))) {
            return thisValue;
          } else {
            EInteger bm = this.helper.GetMantissa(thisValue);
            bm += EInteger.FromInt64(1);
            return this.helper.CreateNewWithFlags(
              bm,
              this.helper.GetExponent(thisValue),
              thisFlags);
          }
        }
      }
      ctx = ctx ?? EContext.Unlimited.WithRounding(ERounding.HalfEven);
      bool binaryPrec = ctx.IsPrecisionInBits;
      // get the precision
      FastInteger fastPrecision = ctx.Precision.CanFitInInt32() ? new
    FastInteger(ctx.Precision.AsInt32Checked()) :
      FastInteger.FromBig(ctx.Precision);
      // No need to check if precision is less than 0, since the
      // PrecisionContext object should already ensure this
      binaryPrec &= this.thisRadix != 2 && !fastPrecision.IsValueZero;
      IShiftAccumulator accum = null;
      FastInteger fastEMin = null;
      FastInteger fastEMax = null;
      // get the exponent range
      if (ctx != null && ctx.HasExponentRange) {
        fastEMax = ctx.EMax.CanFitInInt32() ? new
       FastInteger(ctx.EMax.AsInt32Checked()) : FastInteger.FromBig(ctx.EMax);
        fastEMin = ctx.EMin.CanFitInInt32() ? new
       FastInteger(ctx.EMin.AsInt32Checked()) : FastInteger.FromBig(ctx.EMin);
      }
      ERounding rounding = (ctx == null) ? ERounding.HalfEven : ctx.Rounding;
      bool unlimitedPrec = fastPrecision.IsValueZero;
      if (!binaryPrec) {
        // Fast path to check if rounding is necessary at all
        if (fastPrecision.Sign > 0 && (shift == null || shift.IsValueZero) &&
            (thisFlags & BigNumberFlags.FlagSpecial) == 0) {
          EInteger mantabs = this.helper.GetMantissa(thisValue);
          if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) !=
              0 && mantabs.IsZero && (ctx.Rounding != ERounding.Floor)) {
            // Change negative zero to positive zero
            // except if the rounding mode is Floor
            thisValue = this.EnsureSign(thisValue, false);
            thisFlags = 0;
          }
          accum = this.helper.CreateShiftAccumulatorWithDigits(
            mantabs,
            lastDiscarded,
            olderDiscarded);
          FastInteger digitCount = accum.GetDigitLength();
          if (digitCount.CompareTo(fastPrecision) <= 0) {
            if (!this.RoundGivenDigits(
lastDiscarded,
              olderDiscarded,
 ctx.Rounding,
 (thisFlags & BigNumberFlags.FlagNegative) != 0,
              mantabs)) {
              if (ctx.HasFlags && (lastDiscarded | olderDiscarded) != 0) {
                ctx.Flags |= EContext.FlagInexact | EContext.FlagRounded;
              }
              if (!ctx.HasExponentRange) {
                return thisValue;
              }
              EInteger bigexp = this.helper.GetExponent(thisValue);
              FastInteger fastExp = bigexp.CanFitInInt32() ? new
           FastInteger(bigexp.AsInt32Checked()) : FastInteger.FromBig(bigexp);
              FastInteger fastAdjustedExp = FastInteger.Copy(fastExp);
              FastInteger fastNormalMin = FastInteger.Copy(fastEMin);
              if (ctx == null || ctx.AdjustExponent) {
                fastAdjustedExp.Add(fastPrecision).Decrement();
                fastNormalMin.Add(fastPrecision).Decrement();
              }
              if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
                  fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
                return thisValue;
              }
            } else {
              if (ctx.HasFlags && (lastDiscarded | olderDiscarded) != 0) {
                ctx.Flags |= EContext.FlagInexact | EContext.FlagRounded;
              }
              var stillWithinPrecision = false;
              mantabs += EInteger.One;
              if (digitCount.CompareTo(fastPrecision) < 0) {
                stillWithinPrecision = true;
              } else {
                EInteger radixPower =
                  this.TryMultiplyByRadixPower(EInteger.One, fastPrecision);
                if (radixPower == null) {
                  return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
                }
                stillWithinPrecision = mantabs.CompareTo(radixPower) < 0;
              }
              if (stillWithinPrecision) {
                EInteger bigexp = this.helper.GetExponent(thisValue);
                if (!ctx.HasExponentRange) {
                  return this.helper.CreateNewWithFlags(
                mantabs,
                bigexp,
                thisFlags);
                }
                FastInteger fastExp = bigexp.CanFitInInt32() ? new
           FastInteger(bigexp.AsInt32Checked()) : FastInteger.FromBig(bigexp);
                FastInteger fastAdjustedExp = FastInteger.Copy(fastExp);
                FastInteger fastNormalMin = FastInteger.Copy(fastEMin);
                if (ctx == null || ctx.AdjustExponent) {
                  fastAdjustedExp.Add(fastPrecision).Decrement();
                  fastNormalMin.Add(fastPrecision).Decrement();
                }
                if (fastAdjustedExp.CompareTo(fastEMax) <= 0 &&
                    fastAdjustedExp.CompareTo(fastNormalMin) >= 0) {
                  return this.helper.CreateNewWithFlags(
  mantabs,
  bigexp,
  thisFlags);
                }
              }
            }
          }
        }
      }
      if (adjustNegativeZero && (thisFlags & BigNumberFlags.FlagNegative) !=
          0 && this.helper.GetMantissa(thisValue).IsZero && (rounding !=
                    ERounding.Floor)) {
        // Change negative zero to positive zero
        // except if the rounding mode is Floor
        thisValue = this.EnsureSign(thisValue, false);
        thisFlags = 0;
      }
      bool neg = (thisFlags & BigNumberFlags.FlagNegative) != 0;
      EInteger bigmantissa = this.helper.GetMantissa(thisValue);
      // save mantissa in case result is subnormal
      // and must be rounded again
      EInteger oldmantissa = bigmantissa;
      bool mantissaWasZero = oldmantissa.IsZero && (lastDiscarded |
                    olderDiscarded) == 0;
      EInteger maxMantissa = EInteger.One;
      FastInteger exp = FastInteger.FromBig(this.helper.GetExponent(thisValue));
      var flags = 0;
      accum = accum ??
        this.helper.CreateShiftAccumulatorWithDigits(
          bigmantissa,
          lastDiscarded,
          olderDiscarded);
      if (binaryPrec) {
        FastInteger prec = FastInteger.Copy(fastPrecision);
        while (prec.Sign > 0) {
          int bitShift = (prec.CompareToInt(1000000) >= 0) ? 1000000 :
            prec.AsInt32();
          maxMantissa <<= bitShift;
          prec.SubtractInt(bitShift);
        }
        maxMantissa -= EInteger.One;
        IShiftAccumulator accumMaxMant =
          this.helper.CreateShiftAccumulator(maxMantissa);
        // Get the digit length of the maximum possible mantissa
        // for the given binary precision, and use that for
        // fastPrecision
        fastPrecision = accumMaxMant.GetDigitLength();
      }
      if (shift != null && shift.Sign != 0) {
        accum.ShiftRight(shift);
      }
      if (!unlimitedPrec) {
        accum.ShiftToDigits(fastPrecision);
      } else {
        fastPrecision = accum.GetDigitLength();
      }
      if (binaryPrec) {
        while (accum.ShiftedInt.CompareTo(maxMantissa) > 0) {
          accum.ShiftRightInt(1);
        }
      }
      FastInteger discardedBits = FastInteger.Copy(accum.DiscardedDigitCount);
      exp.Add(discardedBits);
      FastInteger adjExponent = FastInteger.Copy(exp);
      if (ctx.AdjustExponent) {
        adjExponent = adjExponent.Add(accum.GetDigitLength()).Decrement();
      }
      // DebugUtility.Log("" + bigmantissa + "-> " + accum.ShiftedInt
      // +" digits= " + accum.DiscardedDigitCount + " exp= " + exp
      // +" [curexp= " + (helper.GetExponent(thisValue)) + "] adj=" +
      // adjExponent + ",max= " + fastEMax);
      FastInteger newAdjExponent = adjExponent;
      FastInteger clamp = null;
      EInteger earlyRounded = EInteger.Zero;
      if (binaryPrec && fastEMax != null && adjExponent.CompareTo(fastEMax)
          == 0) {
        // May or may not be an overflow depending on the mantissa
        FastInteger expdiff =
          FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
        EInteger currMantissa = accum.ShiftedInt;
        currMantissa = this.TryMultiplyByRadixPower(currMantissa, expdiff);
        if (currMantissa == null) {
          return this.SignalInvalidWithMessage(
            ctx,
            "Result requires too much memory");
        }
        if (currMantissa.CompareTo(maxMantissa) > 0) {
          // Mantissa too high, treat as overflow
          adjExponent.Increment();
        }
      }
      if (ctx.HasFlags && fastEMin != null &&
          adjExponent.CompareTo(fastEMin) < 0) {
        earlyRounded = accum.ShiftedInt;
        if (this.RoundGivenBigInt(accum, rounding, neg, earlyRounded)) {
          earlyRounded += EInteger.One;
          if (!unlimitedPrec && (earlyRounded.IsEven || (this.thisRadix & 1) !=
                    0)) {
            IShiftAccumulator accum2 =
              this.helper.CreateShiftAccumulator(earlyRounded);
            FastInteger newDigitLength = accum2.GetDigitLength();
            // Ensure newDigitLength doesn't exceed precision
            if (binaryPrec || newDigitLength.CompareTo(fastPrecision) > 0) {
              newDigitLength = FastInteger.Copy(fastPrecision);
            }
            newAdjExponent = FastInteger.Copy(exp);
            if (ctx.AdjustExponent) {
              newAdjExponent = newAdjExponent.Add(newDigitLength).Decrement();
            }
          }
        }
      }
      if (fastEMax != null && adjExponent.CompareTo(fastEMax) > 0) {
        if (mantissaWasZero) {
          if (ctx.HasFlags) {
            ctx.Flags |= flags | EContext.FlagClamped;
          }
          if (ctx.ClampNormalExponents) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && this.thisRadix != 2) {
              fastPrecision =
                this.helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax);
            if (ctx.AdjustExponent) {
              clampExp.Increment().Subtract(fastPrecision);
            }
            if (fastEMax.CompareTo(clampExp) > 0) {
              if (ctx.HasFlags) {
                ctx.Flags |= EContext.FlagClamped;
              }
              fastEMax = clampExp;
            }
          }
          return this.helper.CreateNewWithFlags(
oldmantissa,
fastEMax.AsBigInteger(),
thisFlags);
        }
        // Overflow
        flags |= EContext.FlagOverflow |
          EContext.FlagInexact | EContext.FlagRounded;
        if (rounding == ERounding.None) {
          return this.SignalInvalidWithMessage(ctx, "Rounding was required");
        }
        if (!unlimitedPrec && (rounding == ERounding.Down || rounding ==
                ERounding.ZeroFiveUp ||
              (rounding == ERounding.OddOrZeroFiveUp && this.thisRadix != 2) ||
                (rounding == ERounding.Ceiling && neg) || (rounding ==
                  ERounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          EInteger overflowMant = EInteger.Zero;
          if (binaryPrec) {
            overflowMant = maxMantissa;
          } else {
            overflowMant = this.TryMultiplyByRadixPower(
              EInteger.One,
              fastPrecision);
            if (overflowMant == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
            overflowMant -= EInteger.One;
          }
          if (ctx.HasFlags) {
            ctx.Flags |= flags;
          }
          clamp = FastInteger.Copy(fastEMax);
          if (ctx.AdjustExponent) {
            clamp.Increment().Subtract(fastPrecision);
          }
          return this.helper.CreateNewWithFlags(
            overflowMant,
            clamp.AsBigInteger(),
            neg ? BigNumberFlags.FlagNegative : 0);
        }
        if (ctx.HasFlags) {
          ctx.Flags |= flags;
        }
        return this.SignalOverflow(neg);
      }
      if (fastEMin != null && adjExponent.CompareTo(fastEMin) < 0) {
        // Subnormal
        FastInteger fastETiny = FastInteger.Copy(fastEMin);
        if (ctx.AdjustExponent) {
          fastETiny = fastETiny.Subtract(fastPrecision).Increment();
        }
        if (ctx.HasFlags) {
          if (!earlyRounded.IsZero) {
            if (newAdjExponent.CompareTo(fastEMin) < 0) {
              flags |= EContext.FlagSubnormal;
            }
          }
        }
        // DebugUtility.Log("exp=" + exp + " eTiny=" + fastETiny);
        FastInteger subExp = FastInteger.Copy(exp);
        // DebugUtility.Log("exp=" + subExp + " eTiny=" + fastETiny);
        if (subExp.CompareTo(fastETiny) < 0) {
          // DebugUtility.Log("Less than ETiny");
          FastInteger expdiff = FastInteger.Copy(fastETiny).Subtract(exp);
          expdiff.Add(discardedBits);
          accum = this.helper.CreateShiftAccumulatorWithDigits(
            oldmantissa,
            lastDiscarded,
            olderDiscarded);
          accum.ShiftRight(expdiff);
          FastInteger newmantissa = accum.ShiftedIntFast;
          if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (rounding == ERounding.None) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
          }
          if (accum.DiscardedDigitCount.Sign != 0 ||
              (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            if (ctx.HasFlags) {
              if (!mantissaWasZero) {
                flags |= EContext.FlagRounded;
              }
              if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) !=
                  0) {
                flags |= EContext.FlagInexact |
                  EContext.FlagRounded;
              }
            }
            if (this.Round(accum, rounding, neg, newmantissa)) {
              newmantissa.Increment();
            }
          }
          if (ctx.HasFlags) {
            if (newmantissa.IsValueZero) {
              flags |= EContext.FlagClamped;
            }
            if ((flags & (EContext.FlagSubnormal |
            EContext.FlagInexact)) == (EContext.FlagSubnormal |
                 EContext.FlagInexact)) {
              flags |= EContext.FlagUnderflow |
                EContext.FlagRounded;
            }
            ctx.Flags |= flags;
          }
          bigmantissa = newmantissa.AsBigInteger();
          if (ctx.ClampNormalExponents) {
            // Clamp exponents to eMax + 1 - precision
            // if directed
            if (binaryPrec && this.thisRadix != 2) {
              fastPrecision =
                this.helper.CreateShiftAccumulator(maxMantissa)
                .GetDigitLength();
            }
            FastInteger clampExp = FastInteger.Copy(fastEMax);
            if (ctx.AdjustExponent) {
              clampExp.Increment().Subtract(fastPrecision);
            }
            if (fastETiny.CompareTo(clampExp) > 0) {
              if (!bigmantissa.IsZero) {
                expdiff = FastInteger.Copy(fastETiny).Subtract(clampExp);
                // Change bigmantissa for use
                // in the return value
                bigmantissa = this.TryMultiplyByRadixPower(
                bigmantissa,
                expdiff);
                if (bigmantissa == null) {
                  return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
                }
              }
              if (ctx.HasFlags) {
                ctx.Flags |= EContext.FlagClamped;
              }
              fastETiny = clampExp;
            }
          }
          return this.helper.CreateNewWithFlags(
bigmantissa,
fastETiny.AsBigInteger(),
neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      var recheckOverflow = false;
      if (accum.DiscardedDigitCount.Sign != 0 || (accum.LastDiscardedDigit |
                accum.OlderDiscardedDigits) != 0) {
        if (!bigmantissa.IsZero) {
          flags |= EContext.FlagRounded;
        }
        bigmantissa = accum.ShiftedInt;
        if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
          flags |= EContext.FlagInexact | EContext.FlagRounded;
          if (rounding == ERounding.None) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
        }
        if (this.RoundGivenBigInt(accum, rounding, neg, bigmantissa)) {
          FastInteger oldDigitLength = accum.GetDigitLength();
          bigmantissa += EInteger.One;
          recheckOverflow |= binaryPrec;
          // Check if mantissa's precision is now greater
          // than the one set by the context
          if (!unlimitedPrec && (bigmantissa.IsEven || (this.thisRadix &
                1) != 0) && (binaryPrec ||
                oldDigitLength.CompareTo(fastPrecision) >=
                    0)) {
            accum = this.helper.CreateShiftAccumulator(bigmantissa);
            FastInteger newDigitLength = accum.GetDigitLength();
            if (binaryPrec || newDigitLength.CompareTo(fastPrecision) > 0) {
              FastInteger neededShift =
                FastInteger.Copy(newDigitLength).Subtract(fastPrecision);
              accum.ShiftRight(neededShift);
              if (binaryPrec) {
                while (accum.ShiftedInt.CompareTo(maxMantissa) > 0) {
                  accum.ShiftRightInt(1);
                }
              }
              if (accum.DiscardedDigitCount.Sign != 0) {
                exp.Add(accum.DiscardedDigitCount);
                discardedBits.Add(accum.DiscardedDigitCount);
                bigmantissa = accum.ShiftedInt;
                recheckOverflow |= !binaryPrec;
              }
            }
          }
        }
      }
      if (recheckOverflow && fastEMax != null) {
        // Check for overflow again
        adjExponent = FastInteger.Copy(exp);
        if (ctx.AdjustExponent) {
          adjExponent.Add(accum.GetDigitLength()).Decrement();
        }
        if (binaryPrec && fastEMax != null &&
            adjExponent.CompareTo(fastEMax) == 0) {
          // May or may not be an overflow depending on the mantissa
          // (uses accumulator from previous steps, including the check
          // if the mantissa now exceeded the precision)
          FastInteger expdiff =
            FastInteger.Copy(fastPrecision).Subtract(accum.GetDigitLength());
          EInteger currMantissa = accum.ShiftedInt;
          currMantissa = this.TryMultiplyByRadixPower(currMantissa, expdiff);
          if (currMantissa == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          if (currMantissa.CompareTo(maxMantissa) > 0) {
            // Mantissa too high, treat as overflow
            adjExponent.Increment();
          }
        }
        if (adjExponent.CompareTo(fastEMax) > 0) {
          flags |= EContext.FlagOverflow |
            EContext.FlagInexact | EContext.FlagRounded;
          if (!unlimitedPrec && (rounding == ERounding.Down || rounding ==
                   ERounding.ZeroFiveUp ||
        (rounding == ERounding.OddOrZeroFiveUp || rounding == ERounding.Odd) ||
            (rounding == ERounding.Ceiling &&
                neg) || (rounding == ERounding.Floor && !neg))) {
            // Set to the highest possible value for
            // the given precision
            EInteger overflowMant = EInteger.Zero;
            if (binaryPrec) {
              overflowMant = maxMantissa;
            } else {
              overflowMant = this.TryMultiplyByRadixPower(
                EInteger.One,
                fastPrecision);
              if (overflowMant == null) {
                return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
              }
              overflowMant -= EInteger.One;
            }
            if (ctx.HasFlags) {
              ctx.Flags |= flags;
            }
            clamp = FastInteger.Copy(fastEMax);
            if (ctx.AdjustExponent) {
              clamp.Increment().Subtract(fastPrecision);
            }
            return this.helper.CreateNewWithFlags(
              overflowMant,
              clamp.AsBigInteger(),
              neg ? BigNumberFlags.FlagNegative : 0);
          }
          if (ctx.HasFlags) {
            ctx.Flags |= flags;
          }
          return this.SignalOverflow(neg);
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= flags;
      }
      if (ctx.ClampNormalExponents) {
        // Clamp exponents to eMax + 1 - precision
        // if directed
        if (binaryPrec && this.thisRadix != 2) {
          fastPrecision =
            this.helper.CreateShiftAccumulator(maxMantissa).GetDigitLength();
        }
        FastInteger clampExp = FastInteger.Copy(fastEMax);
        if (ctx.AdjustExponent) {
          clampExp.Increment().Subtract(fastPrecision);
        }
        if (exp.CompareTo(clampExp) > 0) {
          if (!bigmantissa.IsZero) {
            FastInteger expdiff = FastInteger.Copy(exp).Subtract(clampExp);
            bigmantissa = this.TryMultiplyByRadixPower(bigmantissa, expdiff);
            if (bigmantissa == null) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Result requires too much memory");
            }
          }
          if (ctx.HasFlags) {
            ctx.Flags |= EContext.FlagClamped;
          }
          exp = clampExp;
        }
      }
      return this.helper.CreateNewWithFlags(
bigmantissa,
exp.AsBigInteger(),
neg ? BigNumberFlags.FlagNegative : 0);
    }

    private T RoundToScale(
EInteger mantissa,
EInteger remainder,
EInteger divisor,
EInteger desiredExponent,
FastInteger shift,
bool neg,
EContext ctx) {
#if DEBUG
      if (mantissa.Sign < 0) {
        throw new ArgumentException("doesn't satisfy mantissa.Sign>= 0");
      }
      if (remainder.Sign < 0) {
        throw new ArgumentException("doesn't satisfy remainder.Sign>= 0");
      }
      if (divisor.Sign < 0) {
        throw new ArgumentException("doesn't satisfy divisor.Sign>= 0");
      }
#endif
      IShiftAccumulator accum;
      ERounding rounding = (ctx == null) ? ERounding.HalfEven : ctx.Rounding;
      var lastDiscarded = 0;
      var olderDiscarded = 0;
      if (!remainder.IsZero) {
        if (rounding == ERounding.HalfDown || rounding == ERounding.HalfUp ||
            rounding == ERounding.HalfEven) {
          EInteger halfDivisor = divisor >> 1;
          int cmpHalf = remainder.CompareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.IsEven) {
            // remainder is exactly half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == ERounding.None) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      var flags = 0;
      EInteger newmantissa = mantissa;
      if (shift.IsValueZero) {
        if ((lastDiscarded | olderDiscarded) != 0) {
          flags |= EContext.FlagInexact | EContext.FlagRounded;
          if (rounding == ERounding.None) {
            return this.SignalInvalidWithMessage(ctx, "Rounding was required");
          }
          if (
            this.RoundGivenDigits(
lastDiscarded,
olderDiscarded,
rounding,
neg,
newmantissa)) {
            newmantissa += EInteger.One;
          }
        }
      } else {
        accum = this.helper.CreateShiftAccumulatorWithDigits(
          mantissa,
          lastDiscarded,
          olderDiscarded);
        accum.ShiftRight(shift);
        newmantissa = accum.ShiftedInt;
        if (accum.DiscardedDigitCount.Sign != 0 ||
            (accum.LastDiscardedDigit | accum.OlderDiscardedDigits) !=
            0) {
          if (!mantissa.IsZero) {
            flags |= EContext.FlagRounded;
          }
          if ((accum.LastDiscardedDigit | accum.OlderDiscardedDigits) != 0) {
            flags |= EContext.FlagInexact | EContext.FlagRounded;
            if (rounding == ERounding.None) {
              return this.SignalInvalidWithMessage(
                ctx,
                "Rounding was required");
            }
          }
          if (this.RoundGivenBigInt(accum, rounding, neg, newmantissa)) {
            newmantissa += EInteger.One;
          }
        }
      }
      if (ctx.HasFlags) {
        ctx.Flags |= flags;
      }
      return this.helper.CreateNewWithFlags(
        newmantissa,
        desiredExponent,
        neg ? BigNumberFlags.FlagNegative : 0);
    }

    private int[] RoundToScaleStatus(
EInteger remainder,
EInteger divisor,
EContext ctx) {
      ERounding rounding = (ctx == null) ? ERounding.HalfEven : ctx.Rounding;
      var lastDiscarded = 0;
      var olderDiscarded = 0;
      if (!remainder.IsZero) {
        if (rounding == ERounding.HalfDown || rounding == ERounding.HalfUp ||
            rounding == ERounding.HalfEven) {
          EInteger halfDivisor = divisor >> 1;
          int cmpHalf = remainder.CompareTo(halfDivisor);
          if ((cmpHalf == 0) && divisor.IsEven) {
            // remainder is exactly half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 0;
          } else if (cmpHalf > 0) {
            // remainder is greater than half
            lastDiscarded = this.thisRadix / 2;
            olderDiscarded = 1;
          } else {
            // remainder is less than half
            lastDiscarded = 0;
            olderDiscarded = 1;
          }
        } else {
          // Rounding mode doesn't care about
          // whether remainder is exactly half
          if (rounding == ERounding.None) {
            // Rounding was required
            return null;
          }
          lastDiscarded = 1;
          olderDiscarded = 1;
        }
      }
      return new[] { lastDiscarded, olderDiscarded };
    }

    private T SignalDivideByZero(EContext ctx, bool neg) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= EContext.FlagDivideByZero;
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new DivideByZeroException("Division by zero");
      }
      return this.helper.CreateNewWithFlags(
        EInteger.Zero,
        EInteger.Zero,
        BigNumberFlags.FlagInfinity | (neg ? BigNumberFlags.FlagNegative : 0));
    }

    private T SignalingNaNInvalid(T value, EContext ctx) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= EContext.FlagInvalid;
      }
      return this.ReturnQuietNaN(value, ctx);
    }

    private T SignalInvalid(EContext ctx) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= EContext.FlagInvalid;
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException("Invalid operation");
      }
      return this.helper.CreateNewWithFlags(
        EInteger.Zero,
        EInteger.Zero,
        BigNumberFlags.FlagQuietNaN);
    }

    private T SignalInvalidWithMessage(EContext ctx, string str) {
      if (ctx != null && ctx.HasFlags) {
        ctx.Flags |= EContext.FlagInvalid;
      }
      if (this.support == BigNumberFlags.FiniteOnly) {
        throw new ArithmeticException(str);
      }
      return this.helper.CreateNewWithFlags(
EInteger.Zero,
EInteger.Zero,
BigNumberFlags.FlagQuietNaN);
    }

    private T SignalOverflow(bool neg) {
      return this.support == BigNumberFlags.FiniteOnly ? default(T) :
        this.helper.CreateNewWithFlags(
EInteger.Zero,
EInteger.Zero,
(neg ? BigNumberFlags.FlagNegative : 0) | BigNumberFlags.FlagInfinity);
    }

    private T SignalOverflow2(EContext ctx, bool neg) {
      if (ctx != null) {
        ERounding roundingOnOverflow = ctx.Rounding;
        if (ctx.HasFlags) {
          ctx.Flags |= EContext.FlagOverflow |
            EContext.FlagInexact | EContext.FlagRounded;
        }
        if (ctx.HasMaxPrecision && ctx.HasExponentRange &&
            (roundingOnOverflow == ERounding.Down || roundingOnOverflow ==
             ERounding.ZeroFiveUp ||
                (roundingOnOverflow == ERounding.OddOrZeroFiveUp ||
                  roundingOnOverflow == ERounding.Odd) ||
             (roundingOnOverflow == ERounding.Ceiling && neg) ||
             (roundingOnOverflow == ERounding.Floor && !neg))) {
          // Set to the highest possible value for
          // the given precision
          EInteger overflowMant = EInteger.Zero;
          FastInteger fastPrecision = FastInteger.FromBig(ctx.Precision);
          overflowMant = this.TryMultiplyByRadixPower(
            EInteger.One,
            fastPrecision);
          if (overflowMant == null) {
            return this.SignalInvalidWithMessage(
              ctx,
              "Result requires too much memory");
          }
          overflowMant -= EInteger.One;
          FastInteger clamp = FastInteger.FromBig(ctx.EMax);
          if (ctx.AdjustExponent) {
            clamp.Increment().Subtract(fastPrecision);
          }
          return this.helper.CreateNewWithFlags(
            overflowMant,
            clamp.AsBigInteger(),
            neg ? BigNumberFlags.FlagNegative : 0);
        }
      }
      return this.SignalOverflow(neg);
    }

    private T SquareRootHandleSpecial(T thisValue, EContext ctx) {
      int thisFlags = this.helper.GetFlags(thisValue);
      if ((thisFlags & BigNumberFlags.FlagSpecial) != 0) {
        if ((thisFlags & BigNumberFlags.FlagSignalingNaN) != 0) {
          return this.SignalingNaNInvalid(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagQuietNaN) != 0) {
          return this.ReturnQuietNaN(thisValue, ctx);
        }
        if ((thisFlags & BigNumberFlags.FlagInfinity) != 0) {
          // Square root of infinity
          return ((thisFlags & BigNumberFlags.FlagNegative) != 0) ?
            this.SignalInvalid(ctx) : thisValue;
        }
      }
      int sign = this.helper.GetSign(thisValue);
      return (sign < 0) ? this.SignalInvalid(ctx) : default(T);
    }

    // Conservative maximum base-10 radix power for
    // TryMultiplyByRadix Power; derived from
    // Int32.MaxValue*8/3 (8 is the number of bits in a byte;
    // 3 is a conservative estimate of log(10)/log(2).)
    private static EInteger MaxDigits=(EInteger)5726623058L;

    private EInteger TryMultiplyByRadixPower(
      EInteger bi,
      FastInteger radixPower) {
      if (bi.IsZero) {
        return bi;
      }
      if (!radixPower.CanFitInInt32()) {
        // NOTE: For radix 10, each digit fits less than 1 byte; the
        // supported byte length is thus less than the maximum value
        // of a 32-bit integer (2GB).
        FastInteger fastBI = FastInteger.FromBig(MaxDigits);
        if (this.thisRadix != 10 || radixPower.CompareTo(fastBI)>0) {
          return null;
        }
      }
      return this.helper.MultiplyByRadixPower(bi, radixPower);
    }

    private T ValueOf(int value, EContext ctx) {
      return (ctx == null || !ctx.HasExponentRange ||
              ctx.ExponentWithinRange(EInteger.Zero)) ?
        this.helper.ValueOf(value) :
        this.RoundToPrecision(this.helper.ValueOf(value), ctx);
    }
  }
}
