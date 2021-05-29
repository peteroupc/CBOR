/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>This is an internal API.</summary>
  internal interface ICBORNumber {
  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsPositiveInfinity(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsInfinity(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsNegativeInfinity(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsNaN(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsNegative(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    double AsDouble(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    object Negate(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    object Abs(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    EDecimal AsEDecimal(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    EFloat AsEFloat(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    ERational AsERational(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    float AsSingle(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    EInteger AsEInteger(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    long AsInt64(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanFitInSingle(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanFitInDouble(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanFitInInt32(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanFitInInt64(Object obj);

    bool CanFitInUInt64(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInInt64(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInUInt64(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInInt32(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <returns>The return value is an internal value.</returns>
    int AsInt32(Object obj, int minValue, int maxValue);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsNumberZero(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    int Sign(Object obj);

  /// <summary>This is an internal API.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is an
  /// arbitrary object.</param>
  /// <returns>The return value is an internal value.</returns>
    bool IsIntegral(Object obj);
  }
}
