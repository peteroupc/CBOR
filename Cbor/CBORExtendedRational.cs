/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

using System;
using PeterO;

namespace PeterO.Cbor {
  internal class CBORExtendedRational : ICBORNumber
  {
    public bool IsPositiveInfinity(object obj) {
      return ((ExtendedRational)obj).IsPositiveInfinity();
    }

    public bool IsInfinity(object obj) {
      return ((ExtendedRational)obj).IsInfinity();
    }

    public bool IsNegativeInfinity(object obj) {
      return ((ExtendedRational)obj).IsNegativeInfinity();
    }

    public bool IsNaN(object obj) {
      return ((ExtendedRational)obj).IsNaN();
    }

    public double AsDouble(object obj) {
      var er = (ExtendedRational)obj;
      return er.ToDouble();
    }

    public ExtendedDecimal AsExtendedDecimal(object obj) {
      var er = (ExtendedRational)obj;
      return er.ToExtendedDecimalExactIfPossible(PrecisionContext.Decimal128.WithUnlimitedExponents());
    }

    public ExtendedFloat AsExtendedFloat(object obj) {
      var er = (ExtendedRational)obj;
      return er.ToExtendedFloatExactIfPossible(PrecisionContext.Binary128.WithUnlimitedExponents());
    }

    public float AsSingle(object obj) {
      var er = (ExtendedRational)obj;
      return er.ToSingle();
    }

    public BigInteger AsBigInteger(object obj) {
      var er = (ExtendedRational)obj;
      return er.ToBigInteger();
    }

    public long AsInt64(object obj) {
      var ef = (ExtendedRational)obj;
      if (ef.IsFinite) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.bitLength() <= 63) {
          return (long)bi;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public bool CanFitInSingle(object obj) {
      var ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedRational.FromSingle(ef.ToSingle())) == 0;
    }

    public bool CanFitInDouble(object obj) {
      var ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedRational.FromDouble(ef.ToDouble())) == 0;
    }

    public bool CanFitInInt32(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    public bool CanFitInInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    public bool CanTruncatedIntFitInInt64(object obj) {
      var ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    public bool CanTruncatedIntFitInInt32(object obj) {
      var ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    public bool IsZero(object obj) {
      var ef = (ExtendedRational)obj;
      return ef.IsZero;
    }

    public int Sign(object obj) {
      var ef = (ExtendedRational)obj;
      return ef.Sign;
    }

    public bool IsIntegral(object obj) {
      var ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      if (ef.Denominator.Equals(BigInteger.One)) {
        return true;
      }
      // A rational number is integral if the remainder
      // of the numerator divided by the denominator is 0
      BigInteger denom = ef.Denominator;
      BigInteger rem = ef.Numerator % (BigInteger)denom;
      return rem.IsZero;
    }

    public int AsInt32(object obj, int minValue, int maxValue) {
      var ef = (ExtendedRational)obj;
      if (ef.IsFinite) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.canFitInInt()) {
          var ret = (int)bi;
          if (ret >= minValue && ret <= maxValue) {
            return ret;
          }
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    public object Negate(object obj) {
      var ed = (ExtendedRational)obj;
      return ed.Negate();
    }

    public object Abs(object obj) {
      var ed = (ExtendedRational)obj;
      return ed.Abs();
    }

    public ExtendedRational AsExtendedRational(object obj) {
      return (ExtendedRational)obj;
    }
  }
}
