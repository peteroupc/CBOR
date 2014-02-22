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
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsInfinity(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNegativeInfinity(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsNaN(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit floating-point number.</returns>
    public double AsDouble(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedDecimal object.</returns>
    public ExtendedDecimal AsExtendedDecimal(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>An ExtendedFloat object.</returns>
    public ExtendedFloat AsExtendedFloat(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit floating-point number.</returns>
    public float AsSingle(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A BigInteger object.</returns>
    public BigInteger AsBigInteger(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 64-bit signed integer.</returns>
    public long AsInt64(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInSingle(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInDouble(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt32(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInInt64(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt64(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanTruncatedIntFitInInt32(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsZero(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int Sign(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool IsIntegral(object obj) {
      throw new NotImplementedException();  // TODO: Implement
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <returns>A Boolean object.</returns>
    public bool CanFitInTypeZeroOrOne(object obj) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object.</param>
    /// <param name='minValue'>A 32-bit signed integer. (2).</param>
    /// <param name='maxValue'>A 32-bit signed integer. (3).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int AsInt32(object obj, int minValue, int maxValue) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Negate(object obj) {
      throw new NotImplementedException();
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>An arbitrary object. (2).</param>
    /// <returns>An arbitrary object.</returns>
    public object Abs(object obj) {
      throw new NotImplementedException();
    }
  }
}
