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
    /// <summary>Arbitrary-precision rational number.</summary>
  public class ExtendedRational
  {
    private BigInteger numerator;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public BigInteger Numerator {
      get {
        return this.numerator;
      }
    }

    private BigInteger denominator;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public BigInteger Denominator {
      get {
        return this.denominator;
      }
    }
    
    #region Equals and GetHashCode implementation
    public override bool Equals(object obj)
  {
    ExtendedRational other = obj as ExtendedRational;
    if (other == null)
      return false;
    return object.Equals(this.numerator, other.numerator) && object.Equals(this.denominator, other.denominator);
  }
    
  public override int GetHashCode()
  {
    int hashCode = 0;
    unchecked {
      if (numerator != null)
        hashCode += 1000000007 * numerator.GetHashCode();
      if (denominator != null)
        hashCode += 1000000009 * denominator.GetHashCode();
    }
    return hashCode;
  }
    #endregion


    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
      if (numerator == null) {
        throw new ArgumentNullException("numerator");
      }
      if (denominator == null) {
        throw new ArgumentNullException("denominator");
      }
      if (denominator.IsZero) {
        throw new ArgumentException("denominator is zero");
      }
      bool numNegative = numerator.Sign < 0;
      bool denNegative = denominator.Sign < 0;
      if (numNegative == denNegative) {
        if (numNegative) {
          numerator = -numerator;
        }
      } else {
        if (!numNegative) {
          numerator = -numerator;
        }
      }
      if (denNegative) {
        denominator = -denominator;
      }
      this.numerator = numerator;
      this.denominator = denominator;
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      return "(" + this.numerator + "/" + this.denominator + ")";
    }

    public static ExtendedRational FromBigInteger(BigInteger bigint) {
      return new ExtendedRational(bigint, BigInteger.One);
    }

    /// <summary>Converts this rational number to a decimal number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// decimal expansion.</returns>
    public ExtendedDecimal ToExtendedDecimal() {
      return this.ToExtendedDecimal(null);
    }

    /// <summary>Converts this rational number to a decimal number and rounds
    /// the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimal(PrecisionContext ctx) {
      return ExtendedDecimal.FromBigInteger(this.numerator).Divide(ExtendedDecimal.FromBigInteger(this.denominator), ctx);
    }

    /// <summary>Converts this rational number to a decimal number, but
    /// if the result would be a nonterminating decimal expansion, rounds
    /// that result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal ToExtendedDecimalExactIfPossible(PrecisionContext ctx) {
      ExtendedDecimal valueEdNum = ExtendedDecimal.FromBigInteger(this.numerator);
      ExtendedDecimal valueEdDen = ExtendedDecimal.FromBigInteger(this.denominator);
      ExtendedDecimal ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Converts this rational number to a binary number.</summary>
    /// <returns>The exact value of the rational number, or not-a-number
    /// (NaN) if the result can't be exact because it has a nonterminating
    /// binary expansion.</returns>
    public ExtendedFloat ToExtendedFloat() {
      return this.ToExtendedFloat(null);
    }

    /// <summary>Converts this rational number to a binary number and rounds
    /// the result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloat(PrecisionContext ctx) {
      return ExtendedFloat.FromBigInteger(this.numerator).Divide(ExtendedFloat.FromBigInteger(this.denominator), ctx);
    }

    /// <summary>Converts this rational number to a binary number, but if
    /// the result would be a nonterminating binary expansion, rounds that
    /// result to the given precision.</summary>
    /// <param name='ctx'>A PrecisionContext object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat ToExtendedFloatExactIfPossible(PrecisionContext ctx) {
      ExtendedFloat valueEdNum = ExtendedFloat.FromBigInteger(this.numerator);
      ExtendedFloat valueEdDen = ExtendedFloat.FromBigInteger(this.denominator);
      ExtendedFloat ed = valueEdNum.Divide(valueEdDen, null);
      if (ed.IsNaN()) {
        // Result would be inexact, try again using the precision context
        ed = valueEdNum.Divide(valueEdDen, ctx);
      }
      return ed;
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
public bool IsFinite {
      get {
        return true;
      }
    }

    /// <summary>Converts this value to an arbitrary-precision integer.
    /// Any fractional part in this value will be discarded when converting
    /// to a big integer.</summary>
    /// <returns>A BigInteger object.</returns>
    public BigInteger ToBigInteger() {
      return this.numerator / (BigInteger)this.denominator;
    }

    public static ExtendedRational FromInt32(int smallint) {
      return new ExtendedRational((BigInteger)smallint, BigInteger.One);
    }

    public static readonly ExtendedRational Zero = FromBigInteger(BigInteger.Zero);
    public static readonly ExtendedRational One = FromBigInteger(BigInteger.One);
    public static readonly ExtendedRational Ten = FromBigInteger((BigInteger)10);

    /// <summary>Converts this value to a 64-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 64-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 64-bit floating point number.</returns>
    public double ToDouble() {
      return this.ToExtendedFloat(PrecisionContext.Binary64).ToDouble();
    }

    /// <summary>Converts this value to a 32-bit floating-point number.
    /// The half-even rounding mode is used.</summary>
    /// <returns>The closest 32-bit floating-point number to this value.
    /// The return value can be positive infinity or negative infinity if
    /// this value exceeds the range of a 32-bit floating point number.</returns>
     public float ToSingle() {
      return this.ToExtendedFloat(PrecisionContext.Binary32).ToSingle();
    }
  }
}
