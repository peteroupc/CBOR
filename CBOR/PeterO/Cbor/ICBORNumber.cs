/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>This is an internal API.</summary>
  internal interface ICBORNumber
  {
    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsPositiveInfinity(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsInfinity(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsNegativeInfinity(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsNaN(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsNegative(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    double AsDouble(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    object Negate(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    object Abs(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    EDecimal AsEDecimal(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    EFloat AsEFloat(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    ERational AsERational(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    float AsSingle(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    EInteger AsEInteger(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    long AsInt64(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanFitInSingle(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanFitInDouble(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanFitInInt32(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanFitInInt64(object obj);

    bool CanFitInUInt64(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInInt64(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInUInt64(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool CanTruncatedIntFitInInt32(object obj);

    /// <summary>This is an internal API.</summary>
    /// <returns>The return value is an internal value.</returns>
    int AsInt32(object obj, int minValue, int maxValue);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsNumberZero(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    int Sign(object obj);

    /// <summary>This is an internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// arbitrary object.</param>
    /// <returns>The return value is an internal value.</returns>
    bool IsIntegral(object obj);
  }
}
