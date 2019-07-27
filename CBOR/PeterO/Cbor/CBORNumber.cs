using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal sealed class CBORNumber {
    internal enum Kind {
      Integer,
      Binary64,
      EInteger,
      EDecimal,
      EFloat,
      ERational,
    }

    private static readonly ICBORNumber[] NumberInterfaces = {
      new CBORInteger(),
      new CBORDouble(),
      new CBOREInteger(),
      new CBORExtendedDecimal(),
      new CBORExtendedFloat(),
      new CBORExtendedRational(),
    };

    private readonly Kind kind;
    private readonly object value;
    public CBORNumber(Kind kind, object value) {
      this.kind = kind;
      this.value = value;
    }

    internal ICBORNumber GetNumberInterface() {
      return GetNumberInterface(this.kind);
    }

    internal static ICBORNumber GetNumberInterface(CBORObject obj) {
      CBORNumber num = CBORNumber.FromCBORObject(obj);
      return (num == null) ? null : num.GetNumberInterface();
    }

    internal object GetValue() {
      return this.value;
    }
    internal static ICBORNumber GetNumberInterface(Kind kind) {
      switch (kind) {
        case Kind.Integer:
          return NumberInterfaces[0];
        case Kind.Binary64:
          return NumberInterfaces[1];
        case Kind.EInteger:
          return NumberInterfaces[2];
        case Kind.EDecimal:
          return NumberInterfaces[3];
        case Kind.EFloat:
          return NumberInterfaces[4];
        case Kind.ERational:
          return NumberInterfaces[5];
        default: return null;
      }
    }

    public CBORObject ToCBORObject() {
      return CBORObject.FromObject(this.value);
    }

    internal static bool IsNumber(CBORObject o) {
      if (IsUntaggedInteger(o)) {
        return true;
      } else if (!o.IsTagged && o.Type == CBORType.FloatingPoint) {
        return true;
      } else if (o.HasOneTag(2) || o.HasOneTag(3)) {
        return o.Type == CBORType.ByteString;
      } else if (o.HasOneTag(4) ||
   o.HasOneTag(5) ||
   o.HasOneTag(264) ||
   o.HasOneTag(265) ||
   o.HasOneTag(268) ||
   o.HasOneTag(269)) {
        return CheckBigFracToNumber(o,
           o.MostOuterTag.ToInt32Checked());
         } else if (o.HasOneTag(30) ||
           o.HasOneTag(270)) {
        return CheckRationalToNumber(o,
              o.MostOuterTag.ToInt32Checked());
            } else {
        return false;
      }
    }

    public static CBORNumber FromCBORObject(CBORObject o) {
      if (IsUntaggedInteger(o)) {
        if (o.CanValueFitInInt64()) {
          return new CBORNumber(Kind.Integer, o.AsInt64Value());
        } else {
          return new CBORNumber(Kind.EInteger, o.AsEIntegerValue());
        }
      } else if (!o.IsTagged && o.Type == CBORType.FloatingPoint) {
        return CBORNumber.FromObject(o.AsDoubleValue());
      }
      if (o.HasOneTag(2) || o.HasOneTag(3)) {
        return BignumToNumber(o);
      } else if (o.HasOneTag(4) ||
   o.HasOneTag(5) ||
   o.HasOneTag(264) ||
   o.HasOneTag(265) ||
   o.HasOneTag(268) ||
   o.HasOneTag(269)) {
        return BigFracToNumber(o,
           o.MostOuterTag.ToInt32Checked());
         } else if (o.HasOneTag(30) ||
           o.HasOneTag(270)) {
        return RationalToNumber(o,
              o.MostOuterTag.ToInt32Checked());
            } else {
        return null;
      }
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
        throw new CBORException("Big fraction must be an array");
      }
      if (tagName == 270) {
        if (o.Count != 3) {
          throw new CBORException("Extended big fraction requires exactly 3" +
  "\u0020items");
        }
        if (!IsUntaggedInteger(o[2])) {
          throw new CBORException("Third item must be an integer");
        }
      } else {
        if (o.Count != 2) {
          throw new CBORException("Big fraction requires exactly 2 items");
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[0])) {
        throw new CBORException("Numerator is not an integer or bignum");
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        throw new CBORException("Denominator is not an integer or bignum");
      }
      EInteger numerator = IntegerOrBignum(o[0]);
      EInteger denominator = IntegerOrBignum(o[1]);
      if (denominator.Sign <= 0) {
        throw new CBORException("Denominator may not be negative or zero");
      }
      ERational erat = ERational.Create(numerator, denominator);
      if (tagName == 270) {
        if (numerator.Sign < 0) {
          throw new CBORException("Numerator may not be negative");
        }
        if (!o[2].CanValueFitInInt32()) {
          throw new CBORException("Invalid options");
        }
        int options = o[2].AsInt32Value();
        switch (options) {
          case 0:
            break;
          case 1:
            erat = erat.Negate();
            break;
          case 2:
            if (!numerator.IsZero || denominator.CompareTo(1) != 0) {
              throw new CBORException("invalid values");
            }
            erat = ERational.PositiveInfinity;
            break;
          case 3:
            if (!numerator.IsZero || denominator.CompareTo(1) != 0) {
              throw new CBORException("invalid values");
            }
            erat = ERational.NegativeInfinity;
            break;
          case 4:
          case 5:
          case 6:
          case 7:
            if (denominator.CompareTo(1) != 0) {
              throw new CBORException("invalid values");
            }
            erat = ERational.CreateNaN(
               numerator,
               options >= 6,
               options == 5 || options == 7);
            break;
          default: throw new CBORException("Invalid options");
        }
      }
      return CBORNumber.FromObject(erat);
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
      EInteger numerator = IntegerOrBignum(o[0]);
      EInteger denominator = IntegerOrBignum(o[1]);
      if (denominator.Sign <= 0) {
        return false;
      }
      if (tagName == 270) {
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
          default: return false;
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
          default: return false;
        }
      }
      return true;
    }

    private static CBORNumber BigFracToNumber(
      CBORObject o,
      int tagName) {
      if (o.Type != CBORType.Array) {
        throw new CBORException("Big fraction must be an array");
      }
      if (tagName == 268 || tagName == 269) {
        if (o.Count != 3) {
          throw new CBORException("Extended big fraction requires exactly 3" +
  "\u0020items");
        }
        if (!IsUntaggedInteger(o[2])) {
          throw new CBORException("Third item must be an integer");
        }
      } else {
        if (o.Count != 2) {
          throw new CBORException("Big fraction requires exactly 2 items");
        }
      }
      if (tagName == 4 || tagName == 5) {
        if (!IsUntaggedInteger(o[0])) {
          throw new CBORException("Exponent is not an integer");
        }
      } else {
        if (!IsUntaggedIntegerOrBignum(o[0])) {
          throw new CBORException("Exponent is not an integer or bignum");
        }
      }
      if (!IsUntaggedIntegerOrBignum(o[1])) {
        throw new CBORException("Mantissa is not an integer or bignum");
      }
      EInteger exponent = IntegerOrBignum(o[0]);
      EInteger mantissa = IntegerOrBignum(o[1]);
      bool isdec = tagName == 4 || tagName == 264 || tagName == 268;
      EDecimal edec = isdec ? EDecimal.Create(mantissa, exponent) : null;
      EFloat efloat = !isdec ? EFloat.Create(mantissa, exponent) : null;
      if (tagName == 268 || tagName == 269) {
        if (mantissa.Sign < 0) {
          throw new CBORException("Mantissa may not be negative");
        }
        if (!o[2].CanValueFitInInt32()) {
          throw new CBORException("Invalid options");
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
              throw new CBORException("invalid values");
            }
            if (isdec) {
              edec = EDecimal.PositiveInfinity;
            } else {
              efloat = EFloat.PositiveInfinity;
            }
            break;
          case 3:
            if (!exponent.IsZero || !mantissa.IsZero) {
              throw new CBORException("invalid values");
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
              throw new CBORException("invalid values");
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
          default: throw new CBORException("Invalid options");
        }
      }
      if (isdec) {
        return CBORNumber.FromObject(edec);
      } else {
        return CBORNumber.FromObject(efloat);
      }
    }

    private static CBORNumber BignumToNumber(CBORObject o) {
      if (o.Type != CBORType.ByteString) {
        throw new CBORException("Byte array expected");
      }
      bool negative = o.HasMostInnerTag(3);
      byte[] data = o.GetByteString();
      if (data.Length <= 7) {
        long x = 0;
        for (var i = 0; i < data.Length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xff;
        }
        if (negative) {
          x = -x;
          --x;
        }
        return new CBORNumber(Kind.Integer, x);
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
      bytes = new byte[neededLength];
      for (var i = 0; i < data.Length; ++i) {
        bytes[i] = data[data.Length - 1 - i];
        if (negative) {
          bytes[i] = (byte)((~((int)bytes[i])) & 0xff);
        }
      }
      if (extended) {
        bytes[bytes.Length - 1] = negative ? (byte)0xff : (byte)0;
      }
      bi = EInteger.FromBytes(bytes, true);
      if (bi.CanFitInInt64()) {
        return new CBORNumber(Kind.Integer, bi.ToInt64Checked());
      } else {
        return new CBORNumber(Kind.EInteger, bi);
      }
    }

    internal string ToJSONString() {
      switch (this.kind) {
        case Kind.Binary64: {
            var f = (double)this.value;
            if (Double.IsNegativeInfinity(f) ||
Double.IsPositiveInfinity(f) ||
Double.IsNaN(f)) {
              return "null";
            }
            string dblString = CBORUtilities.DoubleToString(f);
            return CBORUtilities.TrimDotZero(dblString);
          }
        case Kind.Integer: {
            var longItem = (long)this.value;
            return CBORUtilities.LongToString(longItem);
          }
        case Kind.EInteger: {
            return ((EInteger)this.value).ToString();
          }
        case Kind.EDecimal: {
            var dec = (EDecimal)this.value;
            if (dec.IsInfinity() || dec.IsNaN()) {
              return "null";
            } else {
              return dec.ToString();
            }
          }
        case Kind.EFloat: {
            var flo = (EFloat)this.value;
            if (flo.IsInfinity() || flo.IsNaN()) {
              return "null";
            }
            if (flo.IsFinite &&
                flo.Exponent.Abs().CompareTo((EInteger)2500) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              double f = flo.ToDouble();
              if (Double.IsNegativeInfinity(f) ||
Double.IsPositiveInfinity(f) ||
Double.IsNaN(f)) {
                return "null";
              }
              string dblString = CBORUtilities.DoubleToString(f);
              return CBORUtilities.TrimDotZero(dblString);
            }
            return flo.ToString();
          }
        case Kind.ERational: {
            var dec = (ERational)this.value;
            EDecimal f = dec.ToEDecimalExactIfPossible(
              EContext.Decimal128.WithUnlimitedExponents());
            if (!f.IsFinite) {
              return "null";
            } else {
              return f.ToString();
            }
          }
        default: throw new InvalidOperationException();
      }
    }

    public static CBORNumber FromObject(int value) {
      return new CBORNumber(Kind.Integer, (long)value);
    }
    public static CBORNumber FromObject(long value) {
      return new CBORNumber(Kind.Integer, value);
    }
    public static CBORNumber FromObject(double value) {
      return new CBORNumber(Kind.Binary64, value);
    }
    public static CBORNumber FromObject(EInteger value) {
      return new CBORNumber(Kind.EInteger, value);
    }
    public static CBORNumber FromObject(EFloat value) {
      return new CBORNumber(Kind.EFloat, value);
    }
    public static CBORNumber FromObject(EDecimal value) {
      return new CBORNumber(Kind.EDecimal, value);
    }
    public static CBORNumber FromObject(ERational value) {
      return new CBORNumber(Kind.ERational, value);
    }

    public CBORNumber Negate() {
      return new CBORNumber(this.kind,
         this.GetNumberInterface().Negate(this.GetValue()));
    }

    public CBORNumber Add(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      Kind typeA = a.kind;
      Kind typeB = b.kind;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueA < 0 && valueB < Int64.MinValue - valueA) ||
                (valueA > 0 && valueB > Int64.MaxValue - valueA)) {
          // would overflow, convert to EInteger
          return CBORNumber.FromObject((EInteger)valueB);
        }
        return new CBORNumber(Kind.Integer, valueA + valueB);
      }
      if (typeA == Kind.ERational ||
             typeB == Kind.ERational) {
        ERational e1 =
        GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return new CBORNumber(Kind.ERational, e1.Add(e2));
      }
      if (typeA == Kind.EDecimal ||
             typeB == Kind.EDecimal) {
        EDecimal e1 =
        GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return new CBORNumber(Kind.EDecimal, e1.Add(e2));
      }
      if (typeA == Kind.EFloat || typeB == Kind.EFloat ||
             typeA == Kind.Binary64 || typeB == Kind.Binary64) {
        EFloat e1 = GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        return new CBORNumber(Kind.EFloat, e1.Add(e2));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return new CBORNumber(Kind.EInteger, b1 + (EInteger)b2);
      }
    }

    public CBORNumber Subtract(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      Kind typeA = a.kind;
      Kind typeB = b.kind;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if ((valueB < 0 && Int64.MaxValue + valueB < valueA) ||
                (valueB > 0 && Int64.MinValue + valueB > valueA)) {
          // would overflow, convert to EInteger
          return CBORNumber.FromObject((EInteger)valueB);
        }
        return new CBORNumber(Kind.Integer, valueA - valueB);
      }
      if (typeA == Kind.ERational || typeB == Kind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return new CBORNumber(Kind.ERational, e1.Subtract(e2));
      }
      if (typeA == Kind.EDecimal || typeB == Kind.EDecimal) {
        EDecimal e1 = GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return new CBORNumber(Kind.EDecimal, e1.Subtract(e2));
      }
      if (typeA == Kind.EFloat || typeB == Kind.EFloat ||
               typeA == Kind.Binary64 || typeB == Kind.Binary64) {
        EFloat e1 = GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        return new CBORNumber(Kind.EFloat, e1.Subtract(e2));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return new CBORNumber(Kind.EInteger, b1 - (EInteger)b2);
      }
    }

    public CBORNumber Multiply(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      Kind typeA = a.kind;
      Kind typeB = b.kind;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
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
          return CBORNumber.FromObject(bvalueA * (EInteger)bvalueB);
        }
        return CBORNumber.FromObject(valueA * valueB);
      }
      if (typeA == Kind.ERational ||
             typeB == Kind.ERational) {
        ERational e1 =
        GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORNumber.FromObject(e1.Multiply(e2));
      }
      if (typeA == Kind.EDecimal ||
             typeB == Kind.EDecimal) {
        EDecimal e1 =
        GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORNumber.FromObject(e1.Multiply(e2));
      }
      if (typeA == Kind.EFloat || typeB ==
      Kind.EFloat || typeA == Kind.Binary64 || typeB ==
               Kind.Binary64) {
        EFloat e1 =
        GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        return new CBORNumber(Kind.EFloat, e1.Multiply(e2));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return new CBORNumber(Kind.EInteger, b1 * (EInteger)b2);
      }
    }

    public CBORNumber Divide(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      CBORNumber a = this;
      object objA = a.value;
      object objB = b.value;
      Kind typeA = a.kind;
      Kind typeB = b.kind;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        if (valueB == 0) {
          return (valueA == 0) ? CBORNumber.FromObject(EDecimal.NaN) :
((valueA < 0) ?
            CBORNumber.FromObject(EDecimal.NegativeInfinity) :
CBORNumber.FromObject(EDecimal.PositiveInfinity));
        }
        if (valueA == Int64.MinValue && valueB == -1) {
          return new CBORNumber(Kind.Integer, valueA).Negate();
        }
        long quo = valueA / valueB;
        long rem = valueA - (quo * valueB);
        return (rem == 0) ? new CBORNumber(Kind.Integer, quo) :
        new CBORNumber(Kind.ERational,
  ERational.Create(
  (EInteger)valueA,
  (EInteger)valueB));
      }
      if (typeA == Kind.ERational || typeB == Kind.ERational) {
        ERational e1 = GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return new CBORNumber(Kind.ERational, e1.Divide(e2));
      }
      if (typeA == Kind.EDecimal ||
             typeB == Kind.EDecimal) {
        EDecimal e1 =
        GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        if (e1.IsZero && e2.IsZero) {
          return new CBORNumber(Kind.EDecimal, EDecimal.NaN);
        }
        EDecimal eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return new CBORNumber(Kind.EDecimal, eret);
        }
        ERational er1 = GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational er2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return new CBORNumber(Kind.ERational, er1.Divide(er2));
      }
      if (typeA == Kind.EFloat || typeB ==
      Kind.EFloat || typeA == Kind.Binary64 || typeB ==
               Kind.Binary64) {
        EFloat e1 =
        GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        if (e1.IsZero && e2.IsZero) {
          return CBORNumber.FromObject(EDecimal.NaN);
        }
        EFloat eret = e1.Divide(e2, null);
        // If either operand is infinity or NaN, the result
        // is already exact. Likewise if the result is a finite number.
        if (!e1.IsFinite || !e2.IsFinite || eret.IsFinite) {
          return CBORNumber.FromObject(eret);
        }
        ERational er1 = GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational er2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return new CBORNumber(Kind.ERational, er1.Divide(er2));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        if (b2.IsZero) {
          return b1.IsZero ? CBORNumber.FromObject(EDecimal.NaN) : ((b1.Sign <
0) ?
            CBORNumber.FromObject(EDecimal.NegativeInfinity) :
CBORNumber.FromObject(EDecimal.PositiveInfinity));
        }
        EInteger bigrem;
        EInteger bigquo;
        {
          EInteger[] divrem = b1.DivRem(b2);
          bigquo = divrem[0];
          bigrem = divrem[1];
        }
        return bigrem.IsZero ? CBORNumber.FromObject(bigquo) :
           new CBORNumber(Kind.ERational, ERational.Create(b1, b2));
      }
    }

    public CBORNumber Remainder(CBORNumber b) {
      if (b == null) {
        throw new ArgumentNullException(nameof(b));
      }
      object objA = this.value;
      object objB = b.value;
      Kind typeA = this.kind;
      Kind typeB = b.kind;
      if (typeA == Kind.Integer && typeB == Kind.Integer) {
        var valueA = (long)objA;
        var valueB = (long)objB;
        return (valueA == Int64.MinValue && valueB == -1) ?
        CBORNumber.FromObject(0) : CBORNumber.FromObject(valueA % valueB);
      }
      if (typeA == Kind.ERational ||
             typeB == Kind.ERational) {
        ERational e1 =
        GetNumberInterface(typeA).AsExtendedRational(objA);
        ERational e2 = GetNumberInterface(typeB).AsExtendedRational(objB);
        return CBORNumber.FromObject(e1.Remainder(e2));
      }
      if (typeA == Kind.EDecimal ||
             typeB == Kind.EDecimal) {
        EDecimal e1 =
        GetNumberInterface(typeA).AsExtendedDecimal(objA);
        EDecimal e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
        return CBORNumber.FromObject(e1.Remainder(e2, null));
      }
      if (typeA == Kind.EFloat ||
               typeB == Kind.EFloat || typeA == Kind.Binary64 || typeB ==
               Kind.Binary64) {
        EFloat e1 =
        GetNumberInterface(typeA).AsExtendedFloat(objA);
        EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
        return CBORNumber.FromObject(e1.Remainder(e2, null));
      } else {
        EInteger b1 = GetNumberInterface(typeA).AsEInteger(objA);
        EInteger b2 = GetNumberInterface(typeB).AsEInteger(objB);
        return CBORNumber.FromObject(b1 % (EInteger)b2);
      }
    }

    public int CompareTo(CBORNumber other) {
      // <summary>Compares two CBOR numbers. In this implementation, the two
      // numbers' mathematical values are compared. Here, NaN (not-a-number)
      // is considered greater than any number. This method is not
      // consistent with the Equals method.</summary>
      // <param name='other'>A value to compare with.</param>
      // <returns>Less than 0, if this value is less than the other object;
      // or 0, if both values are equal; or greater than 0, if this value is
      // less than the other object or if the other object is
      // null.</returns>
      // <exception cref="ArgumentException">An internal error
      // occurred.</exception>
      if (other == null) {
        return 1;
      }
      if (this == other) {
        return 0;
      }
      var cmp = 0;
      Kind typeA = this.kind;
      Kind typeB = other.kind;
      object objA = this.value;
      object objB = other.value;
      if (typeA == typeB) {
        switch (typeA) {
          case Kind.Integer: {
              var a = (long)objA;
              var b = (long)objB;
              cmp = (a == b) ? 0 : ((a < b) ? -1 : 1);
              break;
            }
          case Kind.EInteger: {
              var bigintA = (EInteger)objA;
              var bigintB = (EInteger)objB;
              cmp = bigintA.CompareTo(bigintB);
              break;
            }
          case Kind.Binary64: {
              var a = (double)objA;
              var b = (double)objB;
              // Treat NaN as greater than all other numbers
              cmp = Double.IsNaN(a) ? (Double.IsNaN(b) ? 0 : 1) :
                (Double.IsNaN(b) ? (-1) : ((a == b) ? 0 : ((a < b) ? -1 :
                    1)));
              break;
            }
          case Kind.EDecimal: {
              cmp = ((EDecimal)objA).CompareTo((EDecimal)objB);
              break;
            }
          case Kind.EFloat: {
              cmp = ((EFloat)objA).CompareTo(
                (EFloat)objB);
              break;
            }
          case Kind.ERational: {
              cmp = ((ERational)objA).CompareTo(
                (ERational)objB);
              break;
            }
          default: throw new ArgumentException("Unexpected data type");
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
          if (typeA == Kind.ERational) {
            ERational e1 =
GetNumberInterface(typeA).AsExtendedRational(objA);
            if (typeB == Kind.EDecimal) {
              EDecimal e2 =
GetNumberInterface(typeB).AsExtendedDecimal(objB);
              cmp = e1.CompareToDecimal(e2);
            } else {
              EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
              cmp = e1.CompareToBinary(e2);
            }
          } else if (typeB == Kind.ERational) {
            ERational e2 =
              GetNumberInterface(typeB).AsExtendedRational(objB);
            if (typeA == Kind.EDecimal) {
              EDecimal e1 =
              GetNumberInterface(typeA).AsExtendedDecimal(objA);
              cmp = e2.CompareToDecimal(e1);
              cmp = -cmp;
            } else {
              EFloat e1 =
              GetNumberInterface(typeA).AsExtendedFloat(objA);
              cmp = e2.CompareToBinary(e1);
              cmp = -cmp;
            }
          } else if (typeA == Kind.EDecimal ||
                    typeB == Kind.EDecimal) {
            EDecimal e1 = null;
            EDecimal e2 = null;
            if (typeA == Kind.EFloat) {
              var ef1 = (EFloat)objA;
              e2 = (EDecimal)objB;
              cmp = e2.CompareToBinary(ef1);
              cmp = -cmp;
            } else if (typeB == Kind.EFloat) {
              var ef1 = (EFloat)objB;
              e2 = (EDecimal)objA;
              cmp = e2.CompareToBinary(ef1);
            } else {
              e1 = GetNumberInterface(typeA).AsExtendedDecimal(objA);
              e2 = GetNumberInterface(typeB).AsExtendedDecimal(objB);
              cmp = e1.CompareTo(e2);
            }
          } else if (typeA == Kind.EFloat || typeB ==
                Kind.EFloat || typeA == Kind.Binary64 || typeB ==
                Kind.Binary64) {
            EFloat e1 =
            GetNumberInterface(typeA).AsExtendedFloat(objA);
            EFloat e2 = GetNumberInterface(typeB).AsExtendedFloat(objB);
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
