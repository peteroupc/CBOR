/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/21/2014
 * Time: 6:30 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO
{
    /// <summary>Description of ExtendedRational.</summary>
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

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
public override string ToString() {
      return "(" + this.numerator+"/"+this.denominator+")";
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
public double ToDouble() {
      return ExtendedFloat.FromBigInteger(this.numerator).Divide(ExtendedFloat.FromBigInteger(this.denominator), PrecisionContext.Binary64).ToDouble();
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 64-bit floating-point number.</returns>
public double ToSingle() {
      return ExtendedFloat.FromBigInteger(this.numerator).Divide(ExtendedFloat.FromBigInteger(this.denominator), PrecisionContext.Binary32).ToSingle();
    }

    public ExtendedRational(BigInteger numerator, BigInteger denominator) {
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
  }
}
