/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>Not documented yet.</summary>
  internal interface ICBORNumber {
  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsPositiveInfinity(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsInfinity(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsNegativeInfinity(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsNaN(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsNegative(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    double AsDouble(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    object Negate(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    object Abs(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    EDecimal AsEDecimal(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    EFloat AsEFloat(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    ERational AsERational(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    float AsSingle(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    EInteger AsEInteger(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    long AsInt64(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanFitInSingle(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanFitInDouble(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanFitInInt32(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanFitInInt64(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanTruncatedIntFitInInt64(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool CanTruncatedIntFitInInt32(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <returns>The return value is not documented yet.</returns>
    int AsInt32(Object obj, int minValue, int maxValue);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsNumberZero(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    int Sign(Object obj);

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is not documented yet.</returns>
    bool IsIntegral(Object obj);
  }
}
