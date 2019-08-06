using System;

namespace PeterO.Cbor {
  public sealed partial class CBORNumber {
    /// <summary>Returns whether one object's value is less than
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is less than another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator <(CBORNumber a, CBORNumber b) {
      return a == null ? b != null : a.CompareTo(b) < 0;
    }

    /// <summary>Returns whether one object's value is up to
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is up to another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator <=(CBORNumber a, CBORNumber b) {
      return a == null || a.CompareTo(b) <= 0;
    }

    /// <summary>Returns whether one object's value is greater than
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is greater than
    /// another's; otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator >(CBORNumber a, CBORNumber b) {
      return a != null && a.CompareTo(b) > 0;
    }

    /// <summary>Returns whether one object's value is at least
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if one object's value is at least another's;
    /// otherwise, <c>false</c>.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='a'/> is null.</exception>
    public static bool operator >=(CBORNumber a, CBORNumber b) {
      return a == null ? b == null : a.CompareTo(b) >= 0;
    }
  }
}
