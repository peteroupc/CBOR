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

    /// <summary>Converts this number's value to an 8-bit signed integer if
    /// it can fit in an 8-bit signed integer after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to an 8-bit signed
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than -128 or greater than
    /// 127.</exception>
    [CLSCompliant(false)]
    public sbyte ToSByteChecked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToSByteChecked();
    }

    /// <summary>Converts this number's value to a CLR decimal.</summary>
    /// <returns>This number's value, converted to a decimal as though by
    /// <c>(decimal)this.ToEDecimal()</c>.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number.</exception>
    public decimal ToDecimal() {
      if (!this.IsFinite()) {
        throw new OverflowException("Value is infinity or NaN");
      }
      return (decimal)this.ToEDecimal();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as an 8-bit signed integer.</summary>
    /// <returns>This number, converted to an 8-bit signed integer. Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    [CLSCompliant(false)]
    public sbyte ToSByteUnchecked() {
      return this.IsFinite() ? this.ToEInteger().ToSByteUnchecked() : (sbyte)0;
    }

    /// <summary>Converts this number's value to an 8-bit signed integer if
    /// it can fit in an 8-bit signed integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as an 8-bit signed integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than -128 or
    /// greater than 127.</exception>
    [CLSCompliant(false)]
    public sbyte ToSByteIfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? ((sbyte)0) :
this.ToEIntegerIfExact().ToSByteChecked();
    }

    /// <summary>Converts this number's value to a 16-bit unsigned integer
    /// if it can fit in a 16-bit unsigned integer after converting it to
    /// an integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 16-bit unsigned
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than 0 or greater than
    /// 65535.</exception>
    [CLSCompliant(false)]
    public ushort ToUInt16Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToUInt16Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 16-bit unsigned integer.</summary>
    /// <returns>This number, converted to a 16-bit unsigned integer.
    /// Returns 0 if this value is infinity or not-a-number.</returns>
    [CLSCompliant(false)]
    public ushort ToUInt16Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToUInt16Unchecked() :
(ushort)0;
    }

    /// <summary>Converts this number's value to a 16-bit unsigned integer
    /// if it can fit in a 16-bit unsigned integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 16-bit unsigned
    /// integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than 0 or greater
    /// than 65535.</exception>
    [CLSCompliant(false)]
    public ushort ToUInt16IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? (ushort)0 : this.IsNegative() ? throw new
OverflowException("Value out of range") :
this.ToEIntegerIfExact().ToUInt16Checked();
    }

    /// <summary>Converts this number's value to a 32-bit signed integer if
    /// it can fit in a 32-bit signed integer after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 32-bit signed
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than 0 or greater than
    /// 4294967295.</exception>
    [CLSCompliant(false)]
    public uint ToUInt32Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToUInt32Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 32-bit signed integer.</summary>
    /// <returns>This number, converted to a 32-bit signed integer. Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    [CLSCompliant(false)]
    public uint ToUInt32Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToUInt32Unchecked() : 0U;
    }

    /// <summary>Converts this number's value to a 32-bit signed integer if
    /// it can fit in a 32-bit signed integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 32-bit signed integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than 0 or greater
    /// than 4294967295.</exception>
    [CLSCompliant(false)]
    public uint ToUInt32IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? 0U : this.IsNegative() ? throw new
OverflowException("Value out of range") :
this.ToEIntegerIfExact().ToUInt32Checked();
    }

    /// <summary>Converts this number's value to a 64-bit unsigned integer
    /// if it can fit in a 64-bit unsigned integer after converting it to
    /// an integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 64-bit unsigned
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than 0 or greater than
    /// 18446744073709551615.</exception>
    [CLSCompliant(false)]
    public ulong ToUInt64Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToUInt64Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 64-bit unsigned integer.</summary>
    /// <returns>This number, converted to a 64-bit unsigned integer.
    /// Returns 0 if this value is infinity or not-a-number.</returns>
    [CLSCompliant(false)]
    public ulong ToUInt64Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToUInt64Unchecked() : 0UL;
    }

    /// <summary>Converts this number's value to a 64-bit unsigned integer
    /// if it can fit in a 64-bit unsigned integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 64-bit unsigned
    /// integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than 0 or greater
    /// than 18446744073709551615.</exception>
    [CLSCompliant(false)]
    public ulong ToUInt64IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? 0UL : this.IsNegative() ? throw new
OverflowException("Value out of range") :
this.ToEIntegerIfExact().ToUInt64Checked();
    }
  }
}
