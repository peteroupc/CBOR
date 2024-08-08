/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal static class CBORDataUtilitiesByteArrayString {
    private const long DoubleNegInfinity = unchecked(0xfffL << 52);
    private const long DoublePosInfinity = unchecked(0x7ffL << 52);

    internal static CBORObject ParseJSONNumber(
      byte[] chars,
      int offset,
      int count,
      JSONOptions options,
      int[] endOfNumber) {
      if (chars == null || chars.Length == 0 || count <= 0) {
        return null;
      }
      if (offset < 0 || offset > chars.Length) {
        return null;
      }
      if (count > chars.Length || chars.Length - offset < count) {
        return null;
      }

      _ = options ?? CBORDataUtilities.DefaultOptions;
      bool preserveNegativeZero = options.PreserveNegativeZero;
      JSONOptions.ConversionMode kind = options.NumberConversion;
      int endPos = offset + count;
      int initialOffset = offset;
      var negative = false;
      if (chars[initialOffset] == '-') {
        ++offset;
        negative = true;
      }
      int numOffset = offset;
      var haveDecimalPoint = false;
      var haveDigits = false;
      var haveDigitsAfterDecimal = false;
      var haveNonzeroDigits = false;
      var haveExponent = false;
      int i = offset;
      var decimalPointPos = -1;
      // Check syntax
      int k = i;
      if (endPos - 1 > k && chars[k] == '0' && chars[k + 1] >= '0' &&
        chars[k + 1] <= '9') {
        if (endOfNumber != null) {
          endOfNumber[0] = k + 2;
        }
        return null;
      }
      for (; k < endPos; ++k) {
        byte c = chars[k];
        if (c == '0') {
          haveDigits = true;
          haveDigitsAfterDecimal |= haveDecimalPoint;
        } else if (c >= '1' && c <= '9') {
          haveDigits = true;
          haveDigitsAfterDecimal |= haveDecimalPoint;
          haveNonzeroDigits = true;
        } else if (c == '.') {
          if (!haveDigits || haveDecimalPoint) {
            // no digits before the decimal point,
            // or decimal point already seen
            if (endOfNumber != null) {
              endOfNumber[0] = k;
            }
            return null;
          }
          haveDecimalPoint = true;
          decimalPointPos = k;
        } else if (c == 'E' || c == 'e') {
          ++k;
          haveExponent = true;
          break;
        } else {
          if (endOfNumber != null) {
            endOfNumber[0] = k;
            // Check if character can validly appear after a JSON number
            if (c != ',' && c != ']' && c != '}' &&
              c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
              return null;
            } else {
              endPos = k;
              break;
            }
          }
          return null;
        }
      }
      if (!haveDigits || (haveDecimalPoint && !haveDigitsAfterDecimal)) {
        if (endOfNumber != null) {
          endOfNumber[0] = k;
        }
        return null;
      }
      var exponentPos = -1;
      var negativeExp = false;
      if (haveExponent) {
        haveDigits = false;
        if (k == endPos) {
          if (endOfNumber != null) {
            endOfNumber[0] = k;
          }
          return null;
        }
        byte c = chars[k];
        if (c == '+') {
          ++k;
        } else if (c == '-') {
          negativeExp = true;
          ++k;
        }
        for (; k < endPos; ++k) {
          c = chars[k];
          if (c >= '0' && c <= '9') {
            if (exponentPos < 0 && c != '0') {
              exponentPos = k;
            }
            haveDigits = true;
          } else if (endOfNumber != null) {
            endOfNumber[0] = k;
            // Check if character can validly appear after a JSON number
            if (c != ',' && c != ']' && c != '}' &&
              c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
              return null;
            } else {
              endPos = k;
              break;
            }
          } else {
            return null;
          }
        }
        if (!haveDigits) {
          if (endOfNumber != null) {
            endOfNumber[0] = k;
          }
          return null;
        }
      }
      if (endOfNumber != null) {
        endOfNumber[0] = endPos;
      }
      if (exponentPos >= 0 && endPos - exponentPos > 20) {
        // Exponent too high for precision to overcome (which
        // has a length no bigger than Int32.MaxValue, which is 10 digits
        // long)
        if (!haveNonzeroDigits) {
          // zero
          if (kind == JSONOptions.ConversionMode.Double) {
            return !negative ? CBORObject.FromFloatingPointBits(0, 2) :
CBORObject.FromFloatingPointBits(0x8000, 2);
          } else if (kind == JSONOptions.ConversionMode.IntOrFloatFromDouble ||
            kind == JSONOptions.ConversionMode.IntOrFloat) {
            return CBORObject.FromInt32(0);
          }
        } else if (negativeExp) {
          // underflow
          if (kind == JSONOptions.ConversionMode.Double ||
            kind == JSONOptions.ConversionMode.IntOrFloat) {
            return !negative ? CBORObject.FromFloatingPointBits(0, 2) :
CBORObject.FromFloatingPointBits(0x8000, 2);
          } else if (kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
            return CBORObject.FromInt32(0);
          }
        } else {
          // overflow
          if (kind == JSONOptions.ConversionMode.Double ||
            kind == JSONOptions.ConversionMode.IntOrFloatFromDouble ||
            kind == JSONOptions.ConversionMode.IntOrFloat) {
            return CBORObject.FromFloatingPointBits(
                negative ? DoubleNegInfinity : DoublePosInfinity,
                8);
          } else if (kind == JSONOptions.ConversionMode.Decimal128) {
            return CBORObject.FromEDecimal(negative ?
                EDecimal.NegativeInfinity : EDecimal.PositiveInfinity);
          }
        }
      }
      if (!haveExponent && !haveDecimalPoint &&
        (endPos - numOffset) <= 16) {
        // Very common case of all-digit JSON number strings
        // less than 2^53 (with or without number sign)
        long v = 0L;
        int vi = numOffset;
        for (; vi < endPos; ++vi) {
          v = (v * 10) + (chars[vi] - '0');
        }
        if ((v != 0 || !negative) && v < (1L << 53) - 1) {
          if (negative) {
            v = -v;
          }
          return kind == JSONOptions.ConversionMode.Double
            ?
CBORObject.FromFloatingPointBits(EFloat.FromInt64(v).ToDoubleBits(), 8) :
            kind == JSONOptions.ConversionMode.Decimal128 ?
CBORObject.FromEDecimal(EDecimal.FromInt64(v)) : CBORObject.FromInt64(v);
        }
      }
      if (kind == JSONOptions.ConversionMode.Full) {
        if (!haveDecimalPoint && !haveExponent) {
          EInteger ei = EInteger.FromSubstring(chars, initialOffset, endPos);
          return (preserveNegativeZero && ei.IsZero && negative) ?
CBORObject.FromEDecimal(EDecimal.NegativeZero) :
CBORObject.FromEInteger(ei);
        }
        if (!haveExponent && haveDecimalPoint) {
          // No more than 18 digits plus one decimal point (which
          // should fit a long)
          long lv = 0L;
          int expo = -(endPos - (decimalPointPos + 1));
          int vi = numOffset;
          var digitCount = 0;
          for (; vi < decimalPointPos; ++vi) {
            if (digitCount < 0 || digitCount >= 18) {
              digitCount = -1;
              break;
            } else if (digitCount > 0 || chars[vi] != '0') {
              ++digitCount;
            }
            lv = checked((lv * 10) + (chars[vi] - '0'));
          }
          for (vi = decimalPointPos + 1; vi < endPos; ++vi) {
            if (digitCount < 0 || digitCount >= 18) {
              digitCount = -1;
              break;
            } else if (digitCount > 0 || chars[vi] != '0') {
              ++digitCount;
            }
            lv = checked((lv * 10) + (chars[vi] - '0'));
          }
          if (negative) {
            lv = -lv;
          }
          if (digitCount >= 0 && (!negative || lv != 0)) {
            if (expo == 0) {
              return CBORObject.FromInt64(lv);
            } else {
              var cbor = CBORObject.FromArrayBackedObject(
              new CBORObject[] {
                CBORObject.FromInt32(expo),
                CBORObject.FromInt64(lv),
              });
              return cbor.WithTag(4);
            }
          }
        }
        // DebugUtility.Log("convfull " + chars.Substring(initialOffset, endPos -
        // initialOffset));
        var ed = EDecimal.FromString(
            chars,
            initialOffset,
            endPos - initialOffset);
        if (ed.IsZero && negative) {
          if (ed.Exponent.IsZero) {
            return preserveNegativeZero ?
              CBORObject.FromEDecimal(EDecimal.NegativeZero) :
              CBORObject.FromInt32(0);
          } else {
            return !preserveNegativeZero ?
CBORObject.FromEDecimal(ed.Negate()) : CBORObject.FromEDecimal(ed);
          }
        } else {
          return ed.Exponent.IsZero ? CBORObject.FromEInteger(ed.Mantissa) :
            CBORObject.FromEDecimal(ed);
        }
      } else if (kind == JSONOptions.ConversionMode.Double) {
        var ef = EFloat.FromString(
            chars,
            initialOffset,
            endPos - initialOffset,
            EContext.Binary64);
        long lb = ef.ToDoubleBits();
        if (!preserveNegativeZero && (lb == 1L << 63 || lb == 0L)) {
          lb = 0L;
        }
        return CBORObject.FromFloatingPointBits(lb, 8);
      } else if (kind == JSONOptions.ConversionMode.Decimal128) {
        var ed = EDecimal.FromString(
            chars,
            initialOffset,
            endPos - initialOffset,
            EContext.Decimal128);
        if (!preserveNegativeZero && ed.IsNegative && ed.IsZero) {
          ed = ed.Negate();
        }
        return CBORObject.FromEDecimal(ed);
      } else if (kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
        var ef = EFloat.FromString(
            chars,
            initialOffset,
            endPos - initialOffset,
            EContext.Binary64);
        long lb = ef.ToDoubleBits();
        return (!CBORUtilities.IsBeyondSafeRange(lb) &&
CBORUtilities.IsIntegerValue(lb)) ?
          CBORObject.FromInt64(CBORUtilities.GetIntegerValue(lb)) :
          CBORObject.FromFloatingPointBits(lb, 8);
      } else if (kind == JSONOptions.ConversionMode.IntOrFloat) {
        EContext ctx = EContext.Binary64.WithBlankFlags();
        var ef = EFloat.FromString(
            chars,
            initialOffset,
            endPos - initialOffset,
            ctx);
        long lb = ef.ToDoubleBits();
        if ((ctx.Flags & EContext.FlagInexact) != 0) {
          // Inexact conversion to double, meaning that the string doesn't
          // represent an integer in [-(2^53)+1, 2^53), which is representable
          // exactly as double, so treat as ConversionMode.Double
          if (!preserveNegativeZero && (lb == 1L << 63 || lb == 0L)) {
            lb = 0L;
          }
          return CBORObject.FromFloatingPointBits(lb, 8);
        } else {
          // Exact conversion; treat as ConversionMode.IntToFloatFromDouble
          return (!CBORUtilities.IsBeyondSafeRange(lb) &&
CBORUtilities.IsIntegerValue(lb)) ?
            CBORObject.FromInt64(CBORUtilities.GetIntegerValue(lb)) :
            CBORObject.FromFloatingPointBits(lb, 8);
        }
      } else {
        throw new ArgumentException("Unsupported conversion kind.");
      }
    }
  }
}
