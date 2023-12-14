using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>An instance of a number that CBOR or certain CBOR tags can
  /// represent. For this purpose, infinities and not-a-number or NaN
  /// values are considered numbers. Currently, this class can store one
  /// of the following kinds of numbers: 64-bit signed integers or binary
  /// floating-point numbers; or arbitrary-precision integers, decimal
  /// numbers, binary numbers, or rational numbers.</summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
      "Microsoft.Design",
      "CA1036",
      Justification = "Arbitrary size.")]
  public sealed partial class CBORNumber : IComparable<CBORNumber> {
    /// <summary>Specifies the underlying form of this CBOR number
    /// object.</summary>
    public enum NumberKind {
      /// <summary>A 64-bit signed integer.</summary>
      Integer,

      /// <summary>A 64-bit binary floating-point number.</summary>
      Double,

      /// <summary>An arbitrary-precision integer.</summary>
      EInteger,

      /// <summary>An arbitrary-precision decimal number.</summary>
      EDecimal,

      /// <summary>An arbitrary-precision binary number.</summary>
      EFloat,

      /// <summary>An arbitrary-precision rational number.</summary>
      ERational,
    }

    private static readonly ICBORNumber[] NumberInterfaces = {
      new CBORInteger(),
      new CBORDoubleBits(),
      new CBOREInteger(),
      new CBORExtendedDecimal(),
      new CBORExtendedFloat(),
      new CBORExtendedRational(),
    };
    private readonly object value;
    private CBORNumber(NumberKind kind, object value) {
      this.Kind = kind;
      this.value = value;
    }

    internal ICBORNumber GetNumberInterface() {
      return GetNumberInterface(this.Kind);
    }

    internal static ICBORNumber GetNumberInterface(CBORObject obj) {
      var num = CBORNumber.FromCBORObject(obj);
      return num?.GetNumberInterface();
    }

    internal object GetValue() {
      return this.value;
    }

    internal static ICBORNumber GetNumberInterface(NumberKind kind) {
      switch (kind) {
        case NumberKind.Integer:
          return NumberInterfaces[0];
        case NumberKind.Double:
          return NumberInterfaces[1];
        case NumberKind.EInteger:
          return NumberInterfaces[2];
        case NumberKind.EDecimal:
          return NumberInterfaces[3];
        case NumberKind.EFloat:
          return NumberInterfaces[4];
        case NumberKind.ERational:
          return NumberInterfaces[5];
        default:
          throw new InvalidOperationException();
      }
    }

    /// <summary>Converts this object's value to a CBOR object.</summary>
    /// <returns>A CBOR object that stores this object's value.</returns>
    public CBORObject ToCBORObject() {
      object obj = this.value;
      if (obj is long l) {
        return CBORObject.FromInt64(l);
      }
      if (obj is EInteger eif) {
        return CBORObject.FromEInteger(eif);
      }
      if (obj is EDecimal edf) {
        return CBORObject.FromEDecimal(edf);
      }
      if (obj is EFloat eff) {
        return CBORObject.FromEFloat(eff);
      }
      if (obj is ERational erf) {
        return CBORObject.FromERational(erf);
      }
      throw new InvalidOperationException("Unexpected type: " + obj.GetType());
    }

    /// <summary>Gets this value's sign: -1 if nonzero and negative; 1 if
    /// nonzero and positive; 0 if zero. Not-a-number (NaN) values are
    /// positive or negative depending on what sign is stored in their
    /// underlying forms.</summary>
    /// <value>This value's sign.</value>
    public int Sign => this.IsNaN() ? (this.IsNegative() ? -1 : 1) :
          this.GetNumberInterface().Sign(this.value);

    internal static bool IsNumber(CBORObject o) {
      if (IsUntaggedInteger(o)) {
        return true;
      } else {
        bool isByteString = o.Type == CBORType.ByteString;
        bool isFloatingPoint = o.Type == CBORType.FloatingPoint;
        return (!o.IsTagged && isFloatingPoint) || (
                o.HasOneTag(2) || o.HasOneTag(3) ?
                isByteString : (o.HasOneTag(4) ||
                o.HasOneTag(5) || o.HasOneTag(264) ||
                o.HasOneTag(265) || o.HasOneTag(268) ||
                o.HasOneTag(269)) ? CheckBigFracToNumber(o,
                o.MostOuterTag.ToInt32Checked()) : ((o.HasOneTag(30) ||
                o.HasOneTag(270)) && CheckRationalToNumber(o,
                                            o.MostOuterTag.ToInt32Checked())));
      }
    }

    /// <summary>Creates a CBOR number object from a CBOR object
    /// representing a number (that is, one for which the IsNumber property
    /// in.NET or the isNumber() method in Java returns true).</summary>
    /// <param name='o'>The parameter is a CBOR object representing a
    /// number.</param>
    /// <returns>A CBOR number object, or null if the given CBOR object is
    /// null or does not represent a number.</returns>
    public static CBORNumber FromCBORObject(CBORObject o) {
      if (o == null) {
        return null;
      }
      if (IsUntaggedInteger(o)) {
        return o.CanValueFitInInt64() ?
          new CBORNumber(NumberKind.Integer, o.AsInt64Value()) :
          new CBORNumber(NumberKind.EInteger, o.AsEIntegerValue());
      } else if (!o.IsTagged && o.Type == CBORType.FloatingPoint) {
        return CBORNumber.FromDoubleBits(o.AsDoubleBits());
      }
      return o.HasOneTag(2) || o.HasOneTag(3) ?
        BignumToNumber(o) : o.HasOneTag(4) ||
           o.HasOneTag(5) || o.HasOneTag(264) ||
           o.HasOneTag(265) || o.HasOneTag(268) ||
           o.HasOneTag(269) ? BigFracToNumber(o,
                o.MostOuterTag.ToInt32Checked()) : o.HasOneTag(30) ||
                o.HasOneTag(270) ? RationalToNumber(o,
                o.MostOuterTag.ToInt32Checked()) : null;
    }

    private static bool IsUntaggedInteger(CBORObject o) {
      return !o.IsTagged && o.Type == CBORType.Integer;
    }

    private static bool IsUntaggedIntegerOrBignum(CBORObject o) {
      return IsUntaggedInteger(o) || ((o.HasOneTag(2) || o.HasOneTag(3)) &&
          o.Type == CBORType.ByteString);
    }

    private static EInteger IntegerOrBignum(CBORObject o) {
      if (IsUntaggedInteger(o)) {
        return o.AsEIntegerValue();
      } else {
        CBORNumber n = BignumToNumber(o);
        return n.GetNumberInterface().AsEInteger(n.GetValue());
      }
    }

    private static CBORNumber RationalToNumber(
      CBORObject o,
      int tagName) {
      if (o.Type != CBORType.Array) {
        return null; // "Big fraction must be an array";
      }
      if (tagName == 270) {
        if (o.Count != 3) {
          return null; // "Extended big fraction requires exactly 3 items";
        }
        if (!IsUntaggedInteger(o[2])) {
          return null; // "Third item must be an integer";
        }
      } else {
        if (o.Count != 2) {
          return null; // "Big fraction requires exactly 2 items";
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[0])) {
        return null; // "Numerator is not an integer or bignum";
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        return null; // "Denominator is not an integer or bignum");
      }
      EInteger numerator = IntegerOrBignum(o[0]);
      EInteger denominator = IntegerOrBignum(o[1]);
      if (denominator.Sign <= 0) {
        return null; // "Denominator may not be negative or zero");
      }
      if (tagName == 270) {
        if (numerator.Sign < 0) {
          return null; // "Numerator may not be negative");
        }
        if (!o[2].CanValueFitInInt32()) {
          return null; // "Invalid options";
        }
        int options = o[2].AsInt32Value();
        ERational erat = null;
        switch (options) {
          case 0:
            erat = ERational.Create(numerator, denominator);
            break;
          case 1:
            erat = ERational.Create(numerator, denominator).Negate();
            break;
          case 2:
            if (!numerator.IsZero || denominator.CompareTo(1) != 0) {
              return null; // "invalid values");
            }
            erat = ERational.PositiveInfinity;
            break;
          case 3:
            if (!numerator.IsZero || denominator.CompareTo(1) != 0) {
              return null; // "invalid values");
            }
            erat = ERational.NegativeInfinity;
            break;
          case 4:
          case 5:
          case 6:
          case 7:
            if (denominator.CompareTo(1) != 0) {
              return null; // "invalid values");
            }
            erat = ERational.CreateNaN(
                numerator,
                options >= 6,
                options == 5 || options == 7);
            break;
          default: return null; // "Invalid options");
        }
        return CBORNumber.FromERational(erat);
      } else {
        return CBORNumber.FromERational(ERational.Create(numerator,
  denominator));
      }
    }

    private static bool CheckRationalToNumber(
      CBORObject o,
      int tagName) {
      if (o.Type != CBORType.Array) {
        return false;
      }
      if (tagName == 270) {
        if (o.Count != 3) {
          return false;
        }
        if (!IsUntaggedInteger(o[2])) {
          return false;
        }
      } else {
        if (o.Count != 2) {
          return false;
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[0])) {
        return false;
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        return false;
      }
      EInteger denominator = IntegerOrBignum(o[1]);
      if (denominator.Sign <= 0) {
        return false;
      }
      if (tagName == 270) {
        EInteger numerator = IntegerOrBignum(o[0]);
        if (numerator.Sign < 0 || !o[2].CanValueFitInInt32()) {
          return false;
        }
        int options = o[2].AsInt32Value();
        switch (options) {
          case 0:
          case 1:
            return true;
          case 2:
          case 3:
            return numerator.IsZero && denominator.CompareTo(1) == 0;
          case 4:
          case 5:
          case 6:
          case 7:
            return denominator.CompareTo(1) == 0;
          default:
            return false;
        }
      }
      return true;
    }

    private static bool CheckBigFracToNumber(
      CBORObject o,
      int tagName) {
      if (o.Type != CBORType.Array) {
        return false;
      }
      if (tagName == 268 || tagName == 269) {
        if (o.Count != 3) {
          return false;
        }
        if (!IsUntaggedInteger(o[2])) {
          return false;
        }
      } else {
        if (o.Count != 2) {
          return false;
        }
      }
      if (tagName == 4 || tagName == 5) {
        if (!IsUntaggedInteger(o[0])) {
          return false;
        }
      } else {
        if (!IsUntaggedIntegerOrBignum(o[0])) {
          return false;
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        return false;
      }
      if (tagName == 268 || tagName == 269) {
        EInteger exponent = IntegerOrBignum(o[0]);
        EInteger mantissa = IntegerOrBignum(o[1]);
        if (mantissa.Sign < 0 || !o[2].CanValueFitInInt32()) {
          return false;
        }
        int options = o[2].AsInt32Value();
        switch (options) {
          case 0:
          case 1:
            return true;
          case 2:
          case 3:
            return exponent.IsZero && mantissa.IsZero;
          case 4:
          case 5:
          case 6:
          case 7:
            return exponent.IsZero;
          default:
            return false;
        }
      }
      return true;
    }

    private static CBORNumber BigFracToNumber(
      CBORObject o,
      int tagName) {
      if (o.Type != CBORType.Array) {
        return null; // "Big fraction must be an array");
      }
      if (tagName == 268 || tagName == 269) {
        if (o.Count != 3) {
          return null; // "Extended big fraction requires exactly 3 items");
        }
        if (!IsUntaggedInteger(o[2])) {
          return null; // "Third item must be an integer");
        }
      } else {
        if (o.Count != 2) {
          return null; // "Big fraction requires exactly 2 items");
        }
      }
      if (tagName == 4 || tagName == 5) {
        if (!IsUntaggedInteger(o[0])) {
          return null; // "Exponent is not an integer");
        }
      } else {
        if (!IsUntaggedIntegerOrBignum(o[0])) {
          return null; // "Exponent is not an integer or bignum");
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        return null; // "Mantissa is not an integer or bignum");
      }
      EInteger exponent = IntegerOrBignum(o[0]);
      EInteger mantissa = IntegerOrBignum(o[1]);
      bool isdec = tagName == 4 || tagName == 264 || tagName == 268;
      EDecimal edec = isdec ? EDecimal.Create(mantissa, exponent) : null;
      EFloat efloat = !isdec ? EFloat.Create(mantissa, exponent) : null;
      if (tagName == 268 || tagName == 269) {
        if (mantissa.Sign < 0) {
          return null; // "Mantissa may not be negative");
        }
        if (!o[2].CanValueFitInInt32()) {
          return null; // "Invalid options");
        }
        int options = o[2].AsInt32Value();
        switch (options) {
          case 0:
            break;
          case 1:
            if (isdec) {
              edec = edec.Negate();
            } else {
              efloat = efloat.Negate();
            }
            break;
          case 2:
            if (!exponent.IsZero || !mantissa.IsZero) {
              return null; // "invalid values");
            }
            if (isdec) {
              edec = EDecimal.PositiveInfinity;
            } else {
              efloat = EFloat.PositiveInfinity;
            }
            break;
          case 3:
            if (!exponent.IsZero || !mantissa.IsZero) {
              return null; // "invalid values");
            }
            if (isdec) {
              edec = EDecimal.NegativeInfinity;
            } else {
              efloat = EFloat.NegativeInfinity;
            }
            break;
          case 4:
          case 5:
          case 6:
          case 7:
            if (!exponent.IsZero) {
              return null; // "invalid values");
            }
            if (isdec) {
              edec = EDecimal.CreateNaN(
                  mantissa,
                  options >= 6,
                  options == 5 || options == 7,
                  null);
            } else {
              efloat = EFloat.CreateNaN(
                  mantissa,
                  options >= 6,
                  options == 5 || options == 7,
                  null);
            }
            break;
          default: return null; // "Invalid options");
        }
      }
      return isdec ? FromEDecimal(edec) : FromEFloat(efloat);
    }

    /// <summary>Gets the underlying form of this CBOR number
    /// object.</summary>
    /// <value>The underlying form of this CBOR number object.</value>
    public NumberKind Kind { get; }

    /// <summary>Returns whether this object's value, converted to an
    /// integer by discarding its fractional part, would be -(2^31) or
    /// greater, and less than 2^31.</summary>
    /// <returns><c>true</c> if this object's value, converted to an
    /// integer by discarding its fractional part, would be -(2^31) or
    /// greater, and less than 2^31; otherwise, <c>false</c>.</returns>
    public bool CanTruncatedIntFitInInt32() {
      return
        this.GetNumberInterface().CanTruncatedIntFitInInt32(this.GetValue());
    }

    /// <summary>Returns whether this object's value, converted to an
    /// integer by discarding its fractional part, would be -(2^63) or
    /// greater, and less than 2^63.</summary>
    /// <returns><c>true</c> if this object's value, converted to an
    /// integer by discarding its fractional part, would be -(2^63) or
    /// greater, and less than 2^63; otherwise, <c>false</c>.</returns>
    public bool CanTruncatedIntFitInInt64() {
      switch (this.Kind) {
        case NumberKind.Integer:
          return true;
        default:
          return

            this.GetNumberInterface()
            .CanTruncatedIntFitInInt64(this.GetValue());
      }
    }

    /// <summary>Returns whether this object's value, converted to an
    /// integer by discarding its fractional part, would be 0 or greater,
    /// and less than 2^64.</summary>
    /// <returns><c>true</c> if this object's value, converted to an
    /// integer by discarding its fractional part, would be 0 or greater,
    /// and less than 2^64; otherwise, <c>false</c>.</returns>
    public bool CanTruncatedIntFitInUInt64() {
      return this.GetNumberInterface()
        .CanTruncatedIntFitInUInt64(this.GetValue());
    }

    /// <summary>Returns whether this object's value can be converted to a
    /// 32-bit floating point number without its value being rounded to
    /// another numerical value.</summary>
    /// <returns><c>true</c> if this object's value can be converted to a
    /// 32-bit floating point number without its value being rounded to
    /// another numerical value, or if this is a not-a-number value, even
    /// if the value's diagnostic information can' t fit in a 32-bit
    /// floating point number; otherwise, <c>false</c>.</returns>
    public bool CanFitInSingle() {
      return this.GetNumberInterface().CanFitInSingle(this.GetValue());
    }

    /// <summary>Returns whether this object's value can be converted to a
    /// 64-bit floating point number without its value being rounded to
    /// another numerical value.</summary>
    /// <returns><c>true</c> if this object's value can be converted to a
    /// 64-bit floating point number without its value being rounded to
    /// another numerical value, or if this is a not-a-number value, even
    /// if the value's diagnostic information can't fit in a 64-bit
    /// floating point number; otherwise, <c>false</c>.</returns>
    public bool CanFitInDouble() {
      return this.GetNumberInterface().CanFitInDouble(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this CBOR object
    /// represents a finite number.</summary>
    /// <returns><c>true</c> if this CBOR object represents a finite
    /// number; otherwise, <c>false</c>.</returns>
    public bool IsFinite() {
      switch (this.Kind) {
        case NumberKind.Integer:
        case NumberKind.EInteger:
          return true;
        default: return !this.IsInfinity() && !this.IsNaN();
      }
    }

    /// <summary>Gets a value indicating whether this object represents an
    /// integer number, that is, a number without a fractional part.
    /// Infinity and not-a-number are not considered integers.</summary>
    /// <returns><c>true</c> if this object represents an integer number,
    /// that is, a number without a fractional part; otherwise,
    /// <c>false</c>.</returns>
    public bool IsInteger() {
      switch (this.Kind) {
        case NumberKind.Integer:
        case NumberKind.EInteger:
          return true;
        default: return this.GetNumberInterface().IsIntegral(this.GetValue());
      }
    }

    /// <summary>Gets a value indicating whether this object is a negative
    /// number.</summary>
    /// <returns><c>true</c> if this object is a negative number;
    /// otherwise, <c>false</c>.</returns>
    public bool IsNegative() {
      return this.GetNumberInterface().IsNegative(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this object's value equals
    /// 0.</summary>
    /// <returns><c>true</c> if this object's value equals 0; otherwise,
    /// <c>false</c>.</returns>
    public bool IsZero() {
      switch (this.Kind) {
        case NumberKind.Integer:
          {
            var thisValue = (long)this.value;
            return thisValue == 0;
          }
        default: return this.GetNumberInterface().IsNumberZero(this.GetValue());
      }
    }

    /// <summary>Converts this object to an arbitrary-precision integer.
    /// See the ToObject overload taking a type for more
    /// information.</summary>
    /// <returns>The closest arbitrary-precision integer to this
    /// object.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number.</exception>
    public EInteger ToEInteger() {
      return this.GetNumberInterface().AsEInteger(this.GetValue());
    }

    /// <summary>Converts this object to an arbitrary-precision integer if
    /// its value is an integer.</summary>
    /// <returns>The arbitrary-precision integer given by object.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number or is not an exact integer.</exception>
    public EInteger ToEIntegerIfExact() {
      return !this.IsInteger() ? throw new ArithmeticException("Not an" +
"\u0020integer") : this.ToEInteger();
    }

    // Begin integer conversions

    /// <summary>Converts this number's value to a byte (from 0 to 255) if
    /// it can fit in a byte (from 0 to 255) after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a byte (from 0 to
    /// 255).</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than 0 or greater than
    /// 255.</exception>
    public byte ToByteChecked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToByteChecked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a byte (from 0 to 255).</summary>
    /// <returns>This number, converted to a byte (from 0 to 255). Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    public byte ToByteUnchecked() {
      return this.IsFinite() ? this.ToEInteger().ToByteUnchecked() : (byte)0;
    }

    /// <summary>Converts this number's value to a byte (from 0 to 255) if
    /// it can fit in a byte (from 0 to 255) without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a byte (from 0 to 255).</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than 0 or greater
    /// than 255.</exception>
    public byte ToByteIfExact() {
      if (!this.IsFinite()) {
        throw new OverflowException("Value is infinity or NaN");
      }
      if (this.IsZero()) {
        return (byte)0;
      }
      return this.IsNegative() ? throw new
        OverflowException("Value out of range") :
        this.ToEIntegerIfExact().ToByteChecked();
    }

    /// <summary>Converts a byte (from 0 to 255) to an arbitrary-precision
    /// decimal number.</summary>
    /// <param name='inputByte'>The number to convert as a byte (from 0 to
    /// 255).</param>
    /// <returns>This number's value as an arbitrary-precision decimal
    /// number.</returns>
    public static CBORNumber FromByte(byte inputByte) {
      int val = inputByte & 0xff;
      return FromInt64((long)val);
    }

    /// <summary>Converts this number's value to a 16-bit signed integer if
    /// it can fit in a 16-bit signed integer after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 16-bit signed
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than -32768 or greater than
    /// 32767.</exception>
    public short ToInt16Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToInt16Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 16-bit signed integer.</summary>
    /// <returns>This number, converted to a 16-bit signed integer. Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    public short ToInt16Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToInt16Unchecked() : (short)0;
    }

    /// <summary>Converts this number's value to a 16-bit signed integer if
    /// it can fit in a 16-bit signed integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 16-bit signed integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than -32768 or
    /// greater than 32767.</exception>
    public short ToInt16IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? ((short)0) : this.ToEIntegerIfExact().ToInt16Checked();
    }

    /// <summary>Converts a 16-bit signed integer to an arbitrary-precision
    /// decimal number.</summary>
    /// <param name='inputInt16'>The number to convert as a 16-bit signed
    /// integer.</param>
    /// <returns>This number's value as an arbitrary-precision decimal
    /// number.</returns>
    public static CBORNumber FromInt16(short inputInt16) {
      int val = inputInt16;
      return FromInt64((long)val);
    }

    /// <summary>Converts this number's value to a 32-bit signed integer if
    /// it can fit in a 32-bit signed integer after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 32-bit signed
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than -2147483648 or greater
    /// than 2147483647.</exception>
    public int ToInt32Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToInt32Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 32-bit signed integer.</summary>
    /// <returns>This number, converted to a 32-bit signed integer. Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    public int ToInt32Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToInt32Unchecked() : 0;
    }

    /// <summary>Converts this number's value to a 32-bit signed integer if
    /// it can fit in a 32-bit signed integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 32-bit signed integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than -2147483648
    /// or greater than 2147483647.</exception>
    public int ToInt32IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? 0 : this.ToEIntegerIfExact().ToInt32Checked();
    }

    /// <summary>Converts this number's value to a 64-bit signed integer if
    /// it can fit in a 64-bit signed integer after converting it to an
    /// integer by discarding its fractional part.</summary>
    /// <returns>This number's value, truncated to a 64-bit signed
    /// integer.</returns>
    /// <exception cref='OverflowException'>This value is infinity or
    /// not-a-number, or the number, once converted to an integer by
    /// discarding its fractional part, is less than -9223372036854775808
    /// or greater than 9223372036854775807.</exception>
    public long ToInt64Checked() {
      return !this.IsFinite() ? throw new OverflowException("Value is" +
"\u0020infinity or NaN") : this.ToEInteger().ToInt64Checked();
    }

    /// <summary>Converts this number's value to an integer by discarding
    /// its fractional part, and returns the least-significant bits of its
    /// two's-complement form as a 64-bit signed integer.</summary>
    /// <returns>This number, converted to a 64-bit signed integer. Returns
    /// 0 if this value is infinity or not-a-number.</returns>
    public long ToInt64Unchecked() {
      return this.IsFinite() ? this.ToEInteger().ToInt64Unchecked() : 0L;
    }

    /// <summary>Converts this number's value to a 64-bit signed integer if
    /// it can fit in a 64-bit signed integer without rounding to a
    /// different numerical value.</summary>
    /// <returns>This number's value as a 64-bit signed integer.</returns>
    /// <exception cref='ArithmeticException'>This value is infinity or
    /// not-a-number, is not an exact integer, or is less than
    /// -9223372036854775808 or greater than
    /// 9223372036854775807.</exception>
    public long ToInt64IfExact() {
      return !this.IsFinite() ?
        throw new OverflowException("Value is infinity or NaN") :
        this.IsZero() ? 0L : this.ToEIntegerIfExact().ToInt64Checked();
    }
    // End integer conversions
    private static CBORNumber BignumToNumber(CBORObject o) {
      if (o.Type != CBORType.ByteString) {
        return null; // "Byte array expected");
      }
      bool negative = o.HasMostInnerTag(3);
      byte[] data = o.GetByteString();
      if (data.Length <= 7) {
        long x = 0;
        for (int i = 0; i < data.Length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xff;
        }
        if (negative) {
          x = -x;
          --x;
        }
        return new CBORNumber(NumberKind.Integer, x);
      }
      int neededLength = data.Length;
      byte[] bytes;
      EInteger bi;
      var extended = false;
      if (((data[0] >> 7) & 1) != 0) {
        // Increase the needed length
        // if the highest bit is set, to
        // distinguish negative and positive
        // values
        ++neededLength;
        extended = true;
      }
      if (extended || negative) {
        bytes = new byte[neededLength];
        Array.Copy(data, 0, bytes, neededLength - data.Length, data.Length);
        if (negative) {
          int i;
          for (i = neededLength - data.Length; i < neededLength; ++i) {
            bytes[i] ^= 0xff;
          }
        }
        if (extended) {
          bytes[0] = negative ? (byte)0xff : (byte)0;
        }
        bi = EInteger.FromBytes(bytes, false);
      } else {
        // No data conversion needed
        bi = EInteger.FromBytes(data, false);
      }
      return bi.CanFitInInt64() ? new CBORNumber(NumberKind.Integer,
  bi.ToInt64Checked()) : new CBORNumber(NumberKind.EInteger, bi);
    }

    /// <summary>Returns the value of this object in text form.</summary>
    /// <returns>A text string representing the value of this
    /// object.</returns>
    public override string ToString() {
      switch (this.Kind) {
        case NumberKind.Integer:
          {
            var longItem = (long)this.value;
            return CBORUtilities.LongToString(longItem);
          }
        case NumberKind.Double:
          {
            var longItem = (long)this.value;
            return CBORUtilities.DoubleBitsToString(longItem);
          }
        default:
          return (this.value == null) ? String.Empty :
            this.value.ToString();
      }
    }

    internal string ToJSONString() {
      switch (this.Kind) {
        case NumberKind.Double:
          {
            var f = (long)this.value;
            if (!CBORUtilities.DoubleBitsFinite(f)) {
              return "null";
            }
            string dblString = CBORUtilities.DoubleBitsToString(f);
            return CBORUtilities.TrimDotZero(dblString);
          }
        case NumberKind.Integer:
          {
            var longItem = (long)this.value;
            return CBORUtilities.LongToString(longItem);
          }
        case NumberKind.EInteger:
          {
            object eiobj = this.value;
            return ((EInteger)eiobj).ToString();
          }
        case NumberKind.EDecimal:
          {
            var dec = (EDecimal)this.value;
            return dec.IsInfinity() || dec.IsNaN() ? "null" : dec.ToString();
          }
        case NumberKind.EFloat:
          {
            var flo = (EFloat)this.value;
            if (flo.IsInfinity() || flo.IsNaN()) {
              return "null";
            }
            if (flo.IsFinite &&
              flo.Exponent.Abs().CompareTo((EInteger)2500) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              long f = flo.ToDoubleBits();
              if (!CBORUtilities.DoubleBitsFinite(f)) {
                return "null";
              }
              string dblString = CBORUtilities.DoubleBitsToString(f);
              return CBORUtilities.TrimDotZero(dblString);
            }
            return flo.ToString();
          }
        case NumberKind.ERational:
          {
            var dec = (ERational)this.value;
            EDecimal f = dec.ToEDecimalExactIfPossible(
                EContext.Decimal128.WithUnlimitedExponents());
            // DebugUtility.Log(
            // " end="+DateTime.UtcNow);
            return !f.IsFinite ? "null" : f.ToString();
          }
        default: throw new InvalidOperationException();
      }
    }

    internal static CBORNumber FromInt(int intValue) {
      return new CBORNumber(NumberKind.Integer, (long)intValue);
    }
    internal static CBORNumber FromInt64(long longValue) {
      return new CBORNumber(NumberKind.Integer, longValue);
    }
    internal static CBORNumber FromDoubleBits(long doubleBits) {
      return new CBORNumber(NumberKind.Double, doubleBits);
    }
    internal static CBORNumber FromEInteger(EInteger eivalue) {
      return new CBORNumber(NumberKind.EInteger, eivalue);
    }
    internal static CBORNumber FromEFloat(EFloat value) {
      return new CBORNumber(NumberKind.EFloat, value);
    }
    internal static CBORNumber FromEDecimal(EDecimal value) {
      return new CBORNumber(NumberKind.EDecimal, value);
    }
    internal static CBORNumber FromERational(ERational value) {
      return new CBORNumber(NumberKind.ERational, value);
    }

    /// <summary>Returns whether this object's numerical value is an
    /// integer, is -(2^31) or greater, and is less than 2^31.</summary>
    /// <returns><c>true</c> if this object's numerical value is an
    /// integer, is -(2^31) or greater, and is less than 2^31; otherwise,
    /// <c>false</c>.</returns>
    public bool CanFitInInt32() {
      ICBORNumber icn = this.GetNumberInterface();
      object gv = this.GetValue();
      if (!icn.CanFitInInt64(gv)) {
        return false;
      }
      long v = icn.AsInt64(gv);
      return v >= Int32.MinValue && v <= Int32.MaxValue;
    }

    /// <summary>Returns whether this object's numerical value is an
    /// integer, is -(2^63) or greater, and is less than 2^63.</summary>
    /// <returns><c>true</c> if this object's numerical value is an
    /// integer, is -(2^63) or greater, and is less than 2^63; otherwise,
    /// <c>false</c>.</returns>
    public bool CanFitInInt64() {
      return this.GetNumberInterface().CanFitInInt64(this.GetValue());
    }

    /// <summary>Returns whether this object's numerical value is an
    /// integer, is 0 or greater, and is less than 2^64.</summary>
    /// <returns><c>true</c> if this object's numerical value is an
    /// integer, is 0 or greater, and is less than 2^64; otherwise,
    /// <c>false</c>.</returns>
    public bool CanFitInUInt64() {
      return this.GetNumberInterface().CanFitInUInt64(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this object represents
    /// infinity.</summary>
    /// <returns><c>true</c> if this object represents infinity; otherwise,
    /// <c>false</c>.</returns>
    public bool IsInfinity() {
      return this.GetNumberInterface().IsInfinity(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this object represents
    /// positive infinity.</summary>
    /// <returns><c>true</c> if this object represents positive infinity;
    /// otherwise, <c>false</c>.</returns>
    public bool IsPositiveInfinity() {
      return this.GetNumberInterface().IsPositiveInfinity(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this object represents
    /// negative infinity.</summary>
    /// <returns><c>true</c> if this object represents negative infinity;
    /// otherwise, <c>false</c>.</returns>
    public bool IsNegativeInfinity() {
      return this.GetNumberInterface().IsNegativeInfinity(this.GetValue());
    }

    /// <summary>Gets a value indicating whether this object represents a
    /// not-a-number value.</summary>
    /// <returns><c>true</c> if this object represents a not-a-number
    /// value; otherwise, <c>false</c>.</returns>
    public bool IsNaN() {
      return this.GetNumberInterface().IsNaN(this.GetValue());
    }

    /// <summary>Converts this object to a decimal number.</summary>
    /// <returns>A decimal number for this object's value.</returns>
    public EDecimal ToEDecimal() {
      return this.GetNumberInterface().AsEDecimal(this.GetValue());
    }

    /// <summary>Converts this object to an arbitrary-precision binary
    /// floating point number. See the ToObject overload taking a type for
    /// more information.</summary>
    /// <returns>An arbitrary-precision binary floating-point number for
    /// this object's value.</returns>
    public EFloat ToEFloat() {
      return this.GetNumberInterface().AsEFloat(this.GetValue());
    }

    /// <summary>Converts this object to a rational number. See the
    /// ToObject overload taking a type for more information.</summary>
    /// <returns>A rational number for this object's value.</returns>
    public ERational ToERational() {
      return this.GetNumberInterface().AsERational(this.GetValue());
    }

    /// <summary>Returns the absolute value of this CBOR number.</summary>
    /// <returns>This object's absolute value without its negative
    /// sign.</returns>
    public CBORNumber Abs() {
      switch (this.Kind) {
        case NumberKind.Integer:
          {
            var longValue = (long)this.value;
            return longValue == Int64.MinValue ?
              FromEInteger(EInteger.FromInt64(longValue).Negate()) :
              longValue >= 0 ? this : new CBORNumber(
                  this.Kind,
                  Math.Abs(longValue));
          }
        case NumberKind.EInteger:
          {
            var eivalue = (EInteger)this.value;
            return eivalue.Sign >= 0 ? this : FromEInteger(eivalue.Abs());
          }
        default:
          return new CBORNumber(this.Kind,
              this.GetNumberInterface().Abs(this.GetValue()));
      }
    }

    /// <summary>Returns a CBOR number with the same value as this one but
    /// with the sign reversed.</summary>
    /// <returns>A CBOR number with the same value as this one but with the
    /// sign reversed.</returns>
    public CBORNumber Negate() {
      switch (this.Kind) {
        case NumberKind.Integer:
          {
            var longValue = (long)this.value;
            return longValue == 0 ? FromEDecimal(EDecimal.NegativeZero) :
              longValue == Int64.MinValue ?
              FromEInteger(EInteger.FromInt64(longValue).Negate()) : new
              CBORNumber(this.Kind, -longValue);
          }
        case NumberKind.EInteger:
          {
            var eiValue = (EInteger)this.value;
            return eiValue.IsZero ?
              FromEDecimal(EDecimal.NegativeZero) :
              FromEInteger(eiValue.Negate());
          }
        default:
          return new CBORNumber(this.Kind,
              this.GetNumberInterface().Negate(this.GetValue()));
      }
    }

    private static ERational CheckOverflow(
      ERational e1,
      ERational e2,
      ERational eresult) {
      return e1.IsFinite && e2.IsFinite && eresult.IsNaN() ?
        throw new OutOfMemoryException("Result might be too big to fit in" +
          "\u0020memory") : eresult;
    }

    private static EDecimal CheckOverflow(EDecimal e1, EDecimal e2, EDecimal
      eresult) {
      // DebugUtility.Log("ED e1.Exp="+e1.Exponent);
      // DebugUtility.Log("ED e2.Exp="+e2.Exponent);
      return e1.IsFinite && e2.IsFinite && eresult.IsNaN() ?
        throw new OutOfMemoryException("Result might be too big to fit in" +
          "\u0020memory") : eresult;
    }

    private static EFloat CheckOverflow(EFloat e1, EFloat e2, EFloat eresult) {
      return e1.IsFinite && e2.IsFinite && eresult.IsNaN() ?
        throw new OutOfMemoryException("Result might be too big to fit in" +
          "\u0020memory") : eresult;
    }

    private static NumberKind GetConvertKind(CBORNumber a, CBORNumber b) {
      NumberKind typeA = a.Kind;
      NumberKind typeB = b.Kind;
      NumberKind convertKind = !a.IsFinite() ?
        (typeB == NumberKind.Integer || typeB ==
            NumberKind.EInteger) ? ((typeA == NumberKind.Double) ?
NumberKind.EFloat :
            typeA) : ((typeB == NumberKind.Double) ? NumberKind.EFloat :
typeB) :
        !b.IsFinite() ? (typeA == NumberKind.Integer || typeA ==
                    NumberKind.EInteger) ? ((typeB == NumberKind.Double) ?
        NumberKind.EFloat : typeB) : ((typeA == NumberKind.Double) ?
NumberKind.EFloat : typeA) :
          typeA == NumberKind.ERational || typeB ==
                NumberKind.ERational ? NumberKind.ERational :
                  typeA == NumberKind.EDecimal || typeB == NumberKind.EDecimal ?
                NumberKind.EDecimal : (typeA == NumberKind.EFloat || typeB ==
                NumberKind.EFloat || typeA == NumberKind.Double || typeB ==
NumberKind.Double) ?
                                    NumberKind.EFloat : NumberKind.EInteger;
      return convertKind;
    }

    /// <summary>Returns the sum of this number and another
    /// number.</summary>
    /// <param name='b'>The number to add with this one.</param>
    /// <returns>The sum of this number and another number.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='b'/> is null.</exception>
    /// <exception cref='OutOfMemoryException'>The exact result of the
    /// operation might be too big to fit in memory (or might require more
    /// than 2 gigabytes of memory to store).</exception>
    public CBORNumber Add(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      NumberKind typeA = a.Kind;
      NumberKind typeB = b.Kind;
      if (typeA == NumberKind.Integer && typeB == NumberKind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueA < 0 && valueB < Int64.MinValue - valueA) ||
          (valueA > 0 && valueB > Int64.MaxValue - valueA)) {
          // would overflow, convert to EInteger
          return CBORNumber.FromEInteger(
              EInteger.FromInt64(valueA).Add(EInteger.FromInt64(valueB)));
        }
        return new CBORNumber(NumberKind.Integer, valueA + valueB);
      }
      NumberKind convertKind = GetConvertKind(a, b);
      if (convertKind == NumberKind.ERational) {
        // DebugUtility.Log("Rational/Rational");
        ERational e1 = GetNumberInterface(typeA).AsERational(objA);
        ERational e2 = GetNumberInterface(typeB).AsERational(objB);
        // DebugUtility.Log("conv Rational/Rational");
        return new CBORNumber(NumberKind.ERational,
            CheckOverflow(
              e1,
              e2,
              e1.Add(e2)));
      }
      if (convertKind == NumberKind.EDecimal) {
        // DebugUtility.Log("Decimal/Decimal");
        EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
        // DebugUtility.Log("ED e1.Exp="+e1.Exponent);
        // DebugUtility.Log("ED e2.Exp="+e2.Exponent);
        return new CBORNumber(NumberKind.EDecimal,
            CheckOverflow(
              e1,
              e2,
              e1.Add(e2)));
      }
      if (convertKind == NumberKind.EFloat) {
        // DebugUtility.Log("Float/Float");
        EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
        // DebugUtility.Log("EF e1.Exp="+e1.Exponent);
        // DebugUtility.Log("EF e2.Exp="+e2.Exponent);
        return new CBORNumber(NumberKind.EFloat,
            CheckOverflow(
              e1,
              e2,
              e1.Add(e2)));
      } else {
        // DebugUtility.Log("type=" + typeA + "/" + typeB + " finite=" +
        // (// this.IsFinite()) + "/" + (b.IsFinite()));
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return new CBORNumber(NumberKind.EInteger, b1 + b2);
      }
    }

    /// <summary>Returns a number that expresses this number minus
    /// another.</summary>
    /// <param name='b'>The second operand to the subtraction.</param>
    /// <returns>A CBOR number that expresses this number minus the given
    /// number.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='b'/> is null.</exception>
    /// <exception cref='OutOfMemoryException'>The exact result of the
    /// operation might be too big to fit in memory (or might require more
    /// than 2 gigabytes of memory to store).</exception>
    public CBORNumber Subtract(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      NumberKind typeA = a.Kind;
      NumberKind typeB = b.Kind;
      if (typeA == NumberKind.Integer && typeB == NumberKind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueB < 0 && Int64.MaxValue + valueB < valueA) ||
          (valueB > 0 && Int64.MinValue + valueB > valueA)) {
          // would overflow, convert to EInteger
          return CBORNumber.FromEInteger(
              EInteger.FromInt64(valueA).Subtract(EInteger.FromInt64(
                  valueB)));
        }
        return new CBORNumber(NumberKind.Integer, valueA - valueB);
      }
      NumberKind convertKind = GetConvertKind(a, b);
      if (convertKind == NumberKind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsERational(objA);
        ERational e2 = GetNumberInterface(typeB).AsERational(objB);
        return new CBORNumber(NumberKind.ERational,
            CheckOverflow(
              e1,
              e2,
              e1.Subtract(e2)));
      }
      if (convertKind == NumberKind.EDecimal) {
        EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
        return new CBORNumber(NumberKind.EDecimal,
            CheckOverflow(
              e1,
              e2,
              e1.Subtract(e2)));
      }
      if (convertKind == NumberKind.EFloat) {
        EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
        return new CBORNumber(NumberKind.EFloat,
            CheckOverflow(
              e1,
              e2,
              e1.Subtract(e2)));
      } else {
        // DebugUtility.Log("type=" + typeA + "/" + typeB + " finite=" +
        // (// this.IsFinite()) + "/" + (b.IsFinite()));
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA)
           .Subtract(GetNumberInterface(typeB).AsEInteger(objB));
        return new CBORNumber(NumberKind.EInteger, b1);
      }
    }

    /// <summary>Returns a CBOR number expressing the product of this
    /// number and the given number.</summary>
    /// <param name='b'>The second operand to the multiplication
    /// operation.</param>
    /// <returns>A number expressing the product of this number and the
    /// given number.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='b'/> is null.</exception>
    /// <exception cref='OutOfMemoryException'>The exact result of the
    /// operation might be too big to fit in memory (or might require more
    /// than 2 gigabytes of memory to store).</exception>
    public CBORNumber Multiply(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      NumberKind typeA = a.Kind;
      NumberKind typeB = b.Kind;
      if (typeA == NumberKind.Integer && typeB == NumberKind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        bool apos = valueA > 0L;
        bool bpos = valueB > 0L;
        if (
          (apos && ((!bpos && (Int64.MinValue / valueA) > valueB) ||
              (bpos && valueA > (Int64.MaxValue / valueB)))) ||
          (!apos && ((!bpos && valueA != 0L &&
                (Int64.MaxValue / valueA) > valueB) ||
              (bpos && valueA < (Int64.MinValue / valueB))))) {
          // would overflow, convert to EInteger
          var bvalueA = (EInteger)valueA;
          var bvalueB = (EInteger)valueB;
          return CBORNumber.FromEInteger(bvalueA * bvalueB);
        }
        return CBORNumber.FromInt64(valueA * valueB);
      }
      NumberKind convertKind = GetConvertKind(a, b);
      if (convertKind == NumberKind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsERational(objA);
        ERational e2 = GetNumberInterface(typeB).AsERational(objB);
        return CBORNumber.FromERational(CheckOverflow(e1, e2, e1.Multiply(e2)));
      }
      if (convertKind == NumberKind.EDecimal) {
        EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
        return CBORNumber.FromEDecimal(CheckOverflow(e1, e2, e1.Multiply(e2)));
      }
      if (convertKind == NumberKind.EFloat) {
        EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
        return new CBORNumber(NumberKind.EFloat,
            CheckOverflow(
              e1,
              e2,
              e1.Multiply(e2)));
      } else {
        // DebugUtility.Log("type=" + typeA + "/" + typeB + " finite=" +
        // (// this.IsFinite()) + "/" + (b.IsFinite()));
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return new CBORNumber(NumberKind.EInteger, b1 * b2);
      }
    }

    /// <summary>Returns the quotient of this number and another
    /// number.</summary>
    /// <param name='b'>The right-hand side (divisor) to the division
    /// operation.</param>
    /// <returns>The quotient of this number and another one.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='b'/> is null.</exception>
    /// <exception cref='OutOfMemoryException'>The exact result of the
    /// operation might be too big to fit in memory (or might require more
    /// than 2 gigabytes of memory to store).</exception>
    public CBORNumber Divide(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      NumberKind typeA = a.Kind;
      NumberKind typeB = b.Kind;
      if (typeA == NumberKind.Integer && typeB == NumberKind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if (valueB == 0) {
          return (valueA == 0) ? FromEDecimal(EDecimal.NaN) :
            ((valueA < 0) ? FromEDecimal(EDecimal.NegativeInfinity) :

              FromEDecimal(EDecimal.PositiveInfinity));
        }
        if (valueA == Int64.MinValue && valueB == -1) {
          return new CBORNumber(NumberKind.Integer, valueA).Negate();
        }
        long quo = valueA / valueB;
        long rem = valueA - (quo * valueB);
        return (rem == 0) ? new CBORNumber(NumberKind.Integer, quo) :
          new CBORNumber(NumberKind.ERational,
            ERational.Create(
              (EInteger)valueA,
              (EInteger)valueB));
      }
      NumberKind convertKind = GetConvertKind(a, b);
      if (convertKind == NumberKind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsERational(objA);
        ERational e2 = GetNumberInterface(typeB).AsERational(objB);
        return new CBORNumber(NumberKind.ERational,
            CheckOverflow(
              e1,
              e2,
              e1.Divide(e2)));
      }
      if (convertKind == NumberKind.EDecimal) {
        EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
        if (e1.IsZero && e2.IsZero) {
          return new CBORNumber(NumberKind.EDecimal, EDecimal.NaN);
        }
        EDecimal eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return new CBORNumber(NumberKind.EDecimal, eret);
        }
        ERational er1 = GetNumberInterface(typeA).AsERational(objA);
        ERational er2 = GetNumberInterface(typeB).AsERational(objB);
        return new CBORNumber(NumberKind.ERational,
            CheckOverflow(
              er1,
              er2,
              er1.Divide(er2)));
      }
      if (convertKind == NumberKind.EFloat) {
        EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
        if (e1.IsZero && e2.IsZero) {
          return CBORNumber.FromEDecimal(EDecimal.NaN);
        }
        EFloat eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return CBORNumber.FromEFloat(eret);
        }
        ERational er1 = GetNumberInterface(typeA).AsERational(objA);
        ERational er2 = GetNumberInterface(typeB).AsERational(objB);
        return new CBORNumber(NumberKind.ERational,
            CheckOverflow(
              er1,
              er2,
              er1.Divide(er2)));
      } else {
        // DebugUtility.Log("type=" + typeA + "/" + typeB + " finite=" +
        // (// this.IsFinite()) + "/" + (b.IsFinite()));
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        if (b2.IsZero) {
          return b1.IsZero ? CBORNumber.FromEDecimal(EDecimal.NaN) : ((b1.Sign <
                0) ? CBORNumber.FromEDecimal(EDecimal.NegativeInfinity) :
              CBORNumber.FromEDecimal(EDecimal.PositiveInfinity));
        }
        EInteger bigrem;
        EInteger bigquo;
        {
          EInteger[] divrem = b1.DivRem(b2);
          bigquo = divrem[0];
          bigrem = divrem[1];
        }
        return bigrem.IsZero ? CBORNumber.FromEInteger(bigquo) :
          new CBORNumber(NumberKind.ERational, ERational.Create(b1, b2));
      }
    }

    /// <summary>Returns the remainder when this number is divided by
    /// another number.</summary>
    /// <param name='b'>The right-hand side (dividend) of the remainder
    /// operation.</param>
    /// <returns>The remainder when this number is divided by the other
    /// number.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='b'/> is null.</exception>
    /// <exception cref='OutOfMemoryException'>The exact result of the
    /// operation might be too big to fit in memory (or might require more
    /// than 2 gigabytes of memory to store).</exception>
    public CBORNumber Remainder(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      object objA = this.value;
      object objB = b.value;
      NumberKind typeA = this.Kind;
      NumberKind typeB = b.Kind;
      if (typeA == NumberKind.Integer && typeB == NumberKind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        return (valueA == Int64.MinValue && valueB == -1) ?
          CBORNumber.FromInt(0) : CBORNumber.FromInt64(valueA % valueB);
      }
      NumberKind convertKind = GetConvertKind(this, b);
      if (convertKind == NumberKind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsERational(objA);
        ERational e2 = GetNumberInterface(typeB).AsERational(objB);
        return CBORNumber.FromERational(CheckOverflow(
          e1,
          e2,
          e1.Remainder(e2)));
      }
      if (convertKind == NumberKind.EDecimal) {
        EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
        return CBORNumber.FromEDecimal(CheckOverflow(e1, e2, e1.Remainder(e2,
                null)));
      }
      if (convertKind == NumberKind.EFloat) {
        EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
        return CBORNumber.FromEFloat(CheckOverflow(e1, e2, e1.Remainder(e2,
                null)));
      } else {
        // DebugUtility.Log("type=" + typeA + "/" + typeB + " finite=" +
        // (// this.IsFinite()) + "/" + (b.IsFinite()));
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return CBORNumber.FromEInteger(b1 % b2);
      }
    }

    /// <summary>Compares this CBOR number with a 32-bit signed integer. In
    /// this implementation, the two numbers' mathematical values are
    /// compared. Here, NaN (not-a-number) is considered greater than any
    /// number.</summary>
    /// <param name='other'>A value to compare with. Can be null.</param>
    /// <returns>A negative number, if this value is less than the other
    /// object; or 0, if both values are equal; or a positive number, if
    /// this value is less than the other object or if the other object is
    /// null.
    /// <para>This implementation returns a positive number if <paramref
    /// name='other'/> is null, to conform to the.NET definition of
    /// CompareTo. This is the case even in the Java version of this
    /// library, for consistency's sake, even though implementations of
    /// <c>Comparable.compareTo()</c> in Java ought to throw an exception
    /// if they receive a null argument rather than treating null as less
    /// or greater than any object.</para>.</returns>
    public int CompareTo(int other) {
      return this.CompareTo(CBORObject.FromInt32(other).AsNumber());
    }

    /// <summary>Compares this CBOR number with a 64-bit signed integer. In
    /// this implementation, the two numbers' mathematical values are
    /// compared. Here, NaN (not-a-number) is considered greater than any
    /// number.</summary>
    /// <param name='other'>A value to compare with. Can be null.</param>
    /// <returns>A negative number, if this value is less than the other
    /// object; or 0, if both values are equal; or a positive number, if
    /// this value is less than the other object or if the other object is
    /// null.
    /// <para>This implementation returns a positive number if <paramref
    /// name='other'/> is null, to conform to the.NET definition of
    /// CompareTo. This is the case even in the Java version of this
    /// library, for consistency's sake, even though implementations of
    /// <c>Comparable.compareTo()</c> in Java ought to throw an exception
    /// if they receive a null argument rather than treating null as less
    /// or greater than any object.</para>.</returns>
    public int CompareTo(long other) {
      return this.CompareTo(CBORObject.FromInt64(other).AsNumber());
    }

    /// <summary>Compares this CBOR number with another. In this
    /// implementation, the two numbers' mathematical values are compared.
    /// Here, NaN (not-a-number) is considered greater than any
    /// number.</summary>
    /// <param name='other'>A value to compare with. Can be null.</param>
    /// <returns>A negative number, if this value is less than the other
    /// object; or 0, if both values are equal; or a positive number, if
    /// this value is less than the other object or if the other object is
    /// null.
    /// <para>This implementation returns a positive number if <paramref
    /// name='other'/> is null, to conform to the.NET definition of
    /// CompareTo. This is the case even in the Java version of this
    /// library, for consistency's sake, even though implementations of
    /// <c>Comparable.compareTo()</c> in Java ought to throw an exception
    /// if they receive a null argument rather than treating null as less
    /// or greater than any object.</para>.</returns>
    public int CompareTo(CBORNumber other) {
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }

      NumberKind typeA = this.Kind;
      NumberKind typeB = other.Kind;
      object objA = this.value;
      object objB = other.value;
      int cmp;
      if (typeA == typeB) {
        switch (typeA) {
          case NumberKind.Integer:
            {
              var a = (long)objA;
              var b = (long)objB;
              cmp = (a == b) ? 0 : ((a < b) ? -1 : 1);
              break;
            }
          case NumberKind.EInteger:
            {
              var bigintA = (EInteger)objA;
              var bigintB = (EInteger)objB;
              cmp = bigintA.CompareTo(bigintB);
              break;
            }
          case NumberKind.Double:
            {
              var a = (long)objA;
              var b = (long)objB;
              // Treat NaN as greater than all other numbers
              cmp = CBORUtilities.DoubleBitsNaN(a) ?
                (CBORUtilities.DoubleBitsNaN(b) ? 0 : 1) :
                (CBORUtilities.DoubleBitsNaN(b) ?
                  -1 : (((a < 0) != (b < 0)) ? ((a < b) ? -1 : 1) :
                    ((a == b) ? 0 : (((a < b) ^ (a < 0)) ? -1 : 1))));
              break;
            }
          case NumberKind.EDecimal:
            {
              cmp = ((EDecimal)objA).CompareTo((EDecimal)objB);
              break;
            }
          case NumberKind.EFloat:
            {
              cmp = ((EFloat)objA).CompareTo(
                  (EFloat)objB);
              break;
            }
          case NumberKind.ERational:
            {
              cmp = ((ERational)objA).CompareTo(
                  (ERational)objB);
              break;
            }
          default: throw new InvalidOperationException(
              "Unexpected data type");
        }
      } else {
        int s1 = GetNumberInterface(typeA).Sign(objA);
        int s2 = GetNumberInterface(typeB).Sign(objB);
        if (s1 != s2 && s1 != 2 && s2 != 2) {
          // if both types are numbers
          // and their signs are different
          return (s1 < s2) ? -1 : 1;
        }
        if (s1 == 2 && s2 == 2) {
          // both are NaN
          cmp = 0;
        } else if (s1 == 2) {
          // first object is NaN
          return 1;
        } else if (s2 == 2) {
          // second object is NaN
          return -1;
        } else {
          // DebugUtility.Log("a=" + this + " b=" + other);
          if (typeA == NumberKind.ERational) {
            ERational e1 = GetNumberInterface(typeA).AsERational(objA);
            if (typeB == NumberKind.EDecimal) {
              EDecimal e2 = GetNumberInterface(typeB).AsEDecimal(objB);
              cmp = e1.CompareToDecimal(e2);
            } else {
              EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
              cmp = e1.CompareToBinary(e2);
            }
          } else if (typeB == NumberKind.ERational) {
            ERational e2 = GetNumberInterface(typeB).AsERational(objB);
            if (typeA == NumberKind.EDecimal) {
              EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
              cmp = e2.CompareToDecimal(e1);
              cmp = -cmp;
            } else {
              EFloat e1 =
                GetNumberInterface(typeA).AsEFloat(objA);
              cmp = e2.CompareToBinary(e1);
              cmp = -cmp;
            }
          } else if (typeA == NumberKind.EDecimal || typeB ==
NumberKind.EDecimal) {
            EDecimal e2;
            if (typeA == NumberKind.EFloat) {
              var ef1 = (EFloat)objA;
              e2 = (EDecimal)objB;
              cmp = e2.CompareToBinary(ef1);
              cmp = -cmp;
            } else if (typeB == NumberKind.EFloat) {
              var ef1 = (EFloat)objB;
              e2 = (EDecimal)objA;
              cmp = e2.CompareToBinary(ef1);
            } else {
              EDecimal e1 = GetNumberInterface(typeA).AsEDecimal(objA);
              e2 = GetNumberInterface(typeB).AsEDecimal(objB);
              cmp = e1.CompareTo(e2);
            }
          } else if (typeA == NumberKind.EFloat || typeB ==
            NumberKind.EFloat || typeA == NumberKind.Double || typeB ==
            NumberKind.Double) {
            EFloat e1 = GetNumberInterface(typeA).AsEFloat(objA);
            EFloat e2 = GetNumberInterface(typeB).AsEFloat(objB);
            cmp = e1.CompareTo(e2);
          } else {
            EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
            EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
            cmp = b1.CompareTo(b2);
          }
        }
      }
      return cmp;
    }
  }
}
