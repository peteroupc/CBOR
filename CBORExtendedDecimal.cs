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
  internal class CBORExtendedDecimal : ICBORNumber
  {
    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsPositiveInfinity(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsPositiveInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNegativeInfinity();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsNaN();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double AsDouble(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToDouble();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal AsExtendedDecimal(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat AsExtendedFloat(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToExtendedFloat();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit floating-point number.</returns>
    public float AsSingle(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToSingle();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger AsBigInteger(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.ToBigInteger();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long AsInt64(object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedDecimal.FromSingle(ef.ToSingle())) == 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInDouble(object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.IsFinite) {
        return true;
      }
      return ef.CompareTo(ExtendedDecimal.FromDouble(ef.ToDouble())) == 0;
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
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.IsZero;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Sign(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      if (ed.IsNaN()) {
        return 2;
      }
      return ed.Sign;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsIntegral(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      if (!ed.IsFinite) {
        return false;
      }
      if (ed.Exponent.Sign >= 0) {
        return true;
      }
      return ed.CompareTo(ExtendedDecimal.FromBigInteger(ed.ToBigInteger())) == 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInTypeZeroOrOne(object obj) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
      if (!ef.IsFinite) {
        return false;
      }
      return ef.CompareTo(ExtendedDecimal.FromBigInteger(CBORObject.LowestMajorType1)) >= 0 &&
        ef.CompareTo(ExtendedDecimal.FromBigInteger(CBORObject.UInt64MaxValue)) <= 0;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <param name='minValue'>A 32-bit signed integer. (2).</param>
    /// <param name='maxValue'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32(object obj, int minValue, int maxValue) {
      ExtendedDecimal ef = (ExtendedDecimal)obj;
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
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Negate();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Abs(object obj) {
      ExtendedDecimal ed = (ExtendedDecimal)obj;
      return ed.Abs();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedRational object.</returns>
public ExtendedRational AsExtendedRational(object obj) {
      return ExtendedRational.FromExtendedDecimal((ExtendedDecimal)obj);
    }
  }
}
