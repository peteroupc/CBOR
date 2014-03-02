/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/CBOR/
 */

using System;

namespace PeterO
{
  internal class CBORExtendedRational : ICBORNumber
  {
    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity(object obj) {
      return ((ExtendedRational)obj).IsPositiveInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity(object obj) {
      return ((ExtendedRational)obj).IsInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity(object obj) {
      return ((ExtendedRational)obj).IsNegativeInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN(object obj) {
      return ((ExtendedRational)obj).IsNaN();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double AsDouble(object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToDouble();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal AsExtendedDecimal(object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToExtendedDecimalExactIfPossible(PrecisionContext.Decimal128.WithUnlimitedExponents());
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat AsExtendedFloat(object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToExtendedFloatExactIfPossible(PrecisionContext.Binary128.WithUnlimitedExponents());
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit floating-point number.</returns>
    public float AsSingle(object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToSingle();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger AsBigInteger(object obj) {
      ExtendedRational er = (ExtendedRational)obj;
      return er.ToBigInteger();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long AsInt64(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (ef.IsFinite) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.bitLength() <= 63) {
          return (long)bi;
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInSingle(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedRational.FromSingle(ef.ToSingle())) == 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInDouble(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedRational.FromDouble(ef.ToDouble())) == 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt32(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt32(obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt64(object obj) {
      return this.IsIntegral(obj) && this.CanTruncatedIntFitInInt64(obj);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt64(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.bitLength() <= 63;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt32(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (!ef.IsFinite) {
        return false;
      }
      BigInteger bi = ef.ToBigInteger();
      return bi.canFitInInt();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsZero(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.IsZero;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Sign(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
      return ef.Sign;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsIntegral(object obj) {
      ExtendedRational ef = (ExtendedRational)obj;
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

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <param name='minValue'>A 32-bit signed integer. (2).</param>
    /// <param name='maxValue'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32(object obj, int minValue, int maxValue) {
      ExtendedRational ef = (ExtendedRational)obj;
      if (ef.IsFinite) {
        BigInteger bi = ef.ToBigInteger();
        if (bi.canFitInInt()) {
          int ret = (int)bi;
          if (ret >= minValue && ret <= maxValue) {
            return ret;
          }
        }
      }
      throw new OverflowException("This object's value is out of range");
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Negate(object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Negate();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Abs(object obj) {
      ExtendedRational ed = (ExtendedRational)obj;
      return ed.Abs();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedRational object.</returns>
    public ExtendedRational AsExtendedRational(object obj) {
      return (ExtendedRational)obj;
    }
  }
}
