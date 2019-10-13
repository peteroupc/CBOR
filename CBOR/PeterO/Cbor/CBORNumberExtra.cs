using System;

namespace PeterO.Cbor {
  public sealed partial class CBORNumber {
    /* The "==" and "!=" operators are not overridden in the .NET version to be
      consistent with Equals, for the following reason: Objects with this
    type can have arbitrary size (e.g., they can
      be arbitrary-precision integers), and
    comparing
      two of them for equality can be much more complicated and take much
      more time than the default behavior of reference equality.
    */

    /// <summary>Returns whether one object's value is less than
    /// another's.</summary>
    /// <param name='a'>The left-hand side of the comparison.</param>
    /// <param name='b'>The right-hand side of the comparison.</param>
    /// <returns><c>true</c> if the first object's value is less than the
    /// other's; otherwise, <c>false</c>.</returns>
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
