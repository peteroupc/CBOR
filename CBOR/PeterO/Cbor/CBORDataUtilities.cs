/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>Contains methods useful for reading and writing data, with
  /// a focus on CBOR.</summary>
  public static class CBORDataUtilities {
    private const string HexAlphabet = "0123456789ABCDEF";

    internal static string ToStringHelper(CBORObject obj, int depth) {
      StringBuilder sb = null;
      string simvalue = null;
      CBORType type = obj.Type;
      CBORObject curobject;
      if (obj.IsTagged) {
        if (sb == null) {
          if (type == CBORType.TextString) {
            // The default capacity of StringBuilder may be too small
            // for many strings, so set a suggested capacity
            // explicitly
            string str = obj.AsString();
            sb = new StringBuilder(Math.Min(str.Length, 4096) + 16);
          } else {
            sb = new StringBuilder();
          }
        }
        // Append opening tags if needed
        curobject = obj;
        while (curobject.IsTagged) {
          EInteger ei = curobject.MostOuterTag;
          sb.Append(ei.ToString());
          sb.Append('(');
          curobject = curobject.UntagOne();
        }
      }
      switch (type) {
        case CBORType.SimpleValue:
          sb = sb ?? new StringBuilder();
          if (obj.IsUndefined) {
            sb.Append("undefined");
          } else if (obj.IsNull) {
            sb.Append("null");
          } else {
            sb.Append("simple(");
            int thisItemInt = obj.SimpleValue;
            char c;
            if (thisItemInt >= 100) {
              // NOTE: '0'-'9' have ASCII code 0x30-0x39
              c = (char)(0x30 + ((thisItemInt / 100) % 10));
              sb.Append(c);
            }
            if (thisItemInt >= 10) {
              c = (char)(0x30 + ((thisItemInt / 10) % 10));
              sb.Append(c);
              c = (char)(0x30 + (thisItemInt % 10));
            } else {
              c = (char)(0x30 + thisItemInt);
            }
            sb.Append(c);
            sb.Append(")");
          }
          break;
        case CBORType.Boolean:
        case CBORType.Integer:
          simvalue = obj.Untag().ToJSONString();
          if (sb == null) {
            return simvalue;
          }
          sb.Append(simvalue);
          break;
        case CBORType.FloatingPoint: {
          double f = obj.AsDoubleValue();
          simvalue = Double.IsNegativeInfinity(f) ? "-Infinity" :
(Double.IsPositiveInfinity(f) ? "Infinity" : (Double.IsNaN(f) ?

                "NaN" : obj.Untag().ToJSONString()));
          if (sb == null) {
            return simvalue;
          }
          sb.Append(simvalue);
          break;
        }
        case CBORType.ByteString: {
          sb = sb ?? new StringBuilder();
          sb.Append("h'");
          byte[] data = obj.GetByteString();
          int length = data.Length;
          for (var i = 0; i < length; ++i) {
            sb.Append(HexAlphabet[(data[i] >> 4) & 15]);
            sb.Append(HexAlphabet[data[i] & 15]);
          }
          sb.Append("'");
          break;
        }
        case CBORType.TextString: {
          if (sb == null) {
            return "\"" + obj.AsString() + "\"";
          }
          sb.Append('\"');
          sb.Append(obj.AsString());
          sb.Append('\"');
          break;
        }
        case CBORType.Array: {
          sb = sb ?? new StringBuilder();
          var first = true;
          sb.Append("[");
          if (depth >= 50) {
            sb.Append("...");
          } else {
            for (var i = 0; i < obj.Count; ++i) {
              if (!first) {
                sb.Append(", ");
              }
              sb.Append(ToStringHelper(obj[i], depth + 1));
              first = false;
            }
          }
          sb.Append("]");
          break;
        }
        case CBORType.Map: {
          sb = sb ?? new StringBuilder();
          var first = true;
          sb.Append("{");
          if (depth >= 50) {
            sb.Append("...");
          } else {
            ICollection<KeyValuePair<CBORObject, CBORObject>> entries =
              obj.Entries;
            foreach (KeyValuePair<CBORObject, CBORObject> entry
              in entries) {
              CBORObject key = entry.Key;
              CBORObject value = entry.Value;
              if (!first) {
                sb.Append(", ");
              }
              sb.Append(ToStringHelper(key, depth + 1));
              sb.Append(": ");
              sb.Append(ToStringHelper(value, depth + 1));
              first = false;
            }
          }
          sb.Append("}");
          break;
        }
        default: {
          sb = sb ?? new StringBuilder();
          sb.Append("???");
          break;
        }
      }
      // Append closing tags if needed
      curobject = obj;
      while (curobject.IsTagged) {
        sb.Append(')');
        curobject = curobject.UntagOne();
      }
      return sb.ToString();
    }

    private const int MaxSafeInt = 214748363;

    /// <summary>Parses a number whose format follows the JSON
    /// specification. See #ParseJSONNumber(String, integersOnly,
    /// parseOnly) for more information.</summary>
    /// <param name='str'>A string to parse. The string is not allowed to
    /// contain white space characters, including spaces.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// positive zero if the number is a zero that starts with a minus sign
    /// (such as "-0" or "-0.0"). Returns null if the parsing fails,
    /// including if the string is null or empty.</returns>
    public static CBORObject ParseJSONNumber(string str) {
      return ParseJSONNumber(str, false, false);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259).</summary>
    /// <param name='str'>A string to parse as a JSON number.</param>
    /// <param name='integersOnly'>If true, no decimal points or exponents
    /// are allowed in the string. The default is false.</param>
    /// <param name='positiveOnly'>If true, only positive numbers are
    /// allowed (the leading minus is disallowed). The default is
    /// false.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// positive zero if the number is a zero that starts with a minus sign
    /// (such as "-0" or "-0.0"). Returns null if the parsing fails,
    /// including if the string is null or empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless the only digit is 0), an optional decimal point (".", full
    /// stop) with one or more basic digits, and an optional letter E or e
    /// with an optional plus or minus sign and one or more basic digits
    /// (the exponent). A string representing a valid JSON number is not
    /// allowed to contain white space characters, including
    /// spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      bool integersOnly,
      bool positiveOnly) {
      return ParseJSONNumber(str, integersOnly, positiveOnly, false);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259), in the form of a 64-bit binary
    /// floating-point number. See #ParseJSONDouble(String,
    /// preserveNegativeZero) for more information.</summary>
    /// <param name='str'>A string to parse as a JSON number.</param>
    /// <returns>A 64-bit binary floating-point number parsed from the
    /// given string. Returns NaN if the parsing fails, including if the
    /// string is null or empty. (To check for NaN, use
    /// <c>Double.IsNaN()</c> in.NET or <c>Double.isNaN()</c> in
    /// Java.)</returns>
    public static double ParseJSONDouble(string str) {
      return ParseJSONDouble(str, false);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259), in the form of a 64-bit binary
    /// floating-point number.</summary>
    /// <param name='str'>A string to parse as a JSON number.</param>
    /// <param name='preserveNegativeZero'>If true, returns positive zero
    /// if the number is a zero that starts with a minus sign (such as "-0"
    /// or "-0.0"). Otherwise, returns negative zero in this case. The
    /// default is false.</param>
    /// <returns>A 64-bit binary floating-point number parsed from the
    /// given string. Returns NaN if the parsing fails, including if the
    /// string is null or empty. (To check for NaN, use
    /// <c>Double.IsNaN()</c> in.NET or <c>Double.isNaN()</c> in
    /// Java.)</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless the only digit is 0), an optional decimal point (".", full
    /// stop) with one or more basic digits, and an optional letter E or e
    /// with an optional plus or minus sign and one or more basic digits
    /// (the exponent). A string representing a valid JSON number is not
    /// allowed to contain white space characters, including
    /// spaces.</remarks>
    public static double ParseJSONDouble(string str, bool
      preserveNegativeZero) {
      if (String.IsNullOrEmpty(str)) {
        return Double.NaN;
      }
      var offset = 0;
      var havenonzero = false;
      var negative = false;
      var haveExponent = false;
      var haveDecimalPoint = false;
      var haveDigits = false;
      var haveDigitsAfterDecimal = false;
      if (str[0] == '-') {
        negative = true;
        ++offset;
      }
      int i = offset;
      // Ordinary number
      if (i < str.Length && str[i] == '0') {
        ++i;
        haveDigits = true;
        if (i == str.Length) {
          return (preserveNegativeZero && negative) ?
            EFloat.Zero.Negate().ToDouble() : 0.0;
        }
        if (str[i] == '.') {
          haveDecimalPoint = true;
          ++i;
        } else if (str[i] == 'E' || str[i] == 'e') {
          haveExponent = true;
        } else {
          return Double.NaN;
        }
      }
      for (; i < str.Length; ++i) {
        if (str[i] >= '0' && str[i] <= '9') {
          haveDigits = true;
          havenonzero = havenonzero && (str[i] != '0');
          if (haveDecimalPoint) {
            haveDigitsAfterDecimal = true;
          }
        } else if (str[i] == '.') {
          if (!haveDigits) {
            // no digits before the decimal point
            return Double.NaN;
          }
          if (haveDecimalPoint) {
            return Double.NaN;
          }
          haveDecimalPoint = true;
        } else if (str[i] == 'E' || str[i] == 'e') {
          haveExponent = true;
          ++i;
          break;
        } else {
          return Double.NaN;
        }
      }
      if (!haveDigits || (haveDecimalPoint && !haveDigitsAfterDecimal)) {
        return Double.NaN;
      }
      if (haveExponent) {
        haveDigits = false;
        if (i == str.Length) {
          return Double.NaN;
        }
        if (str[i] == '+' || str[i] == '-') {
          ++i;
        }
        for (; i < str.Length; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
          } else {
            return Double.NaN;
          }
        }
        if (!haveDigits) {
          return Double.NaN;
        }
      }
      if (!havenonzero) {
        return (preserveNegativeZero && negative) ?
          EFloat.Zero.Negate().ToDouble() : 0.0;
      }
      return EFloat.FromString(str, EContext.Binary64).ToDouble();
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259), in the form of a CBOR integer if the
    /// number represents an integer at least -(2^53) and less than or
    /// equal to 2^53, or in the form of a CBOR (64-bit) floating-point
    /// number otherwise.</summary>
    /// <param name='str'>A string to parse as a JSON number.</param>
    /// <param name='integersOnly'>If true, no decimal points or exponents
    /// are allowed in the string. The default is false.</param>
    /// <param name='positiveOnly'>If true, the leading minus is disallowed
    /// in the string. The default is false.</param>
    /// <param name='doubleApprox'>If true, treats a JSON number as an
    /// integer noninteger based on its closest approximation as a CBOR
    /// (64-bit) floating-point number. If false, this treatment is based
    /// on the full precision of the given JSON number string. For example,
    /// given the string "0.99999999999999999999999999999999999", the
    /// nearest representable CBOR floating-point number is 1.0, and if
    /// this parameter is <c>true</c>, this string is treated as 1.0, an
    /// integer, so that the result is the CBOR integer 1, and if this
    /// parameter is <c>false</c>, this string is not treated as an
    /// integer so that the result is the closest CBOR floating-point
    /// approximation, 1.0.</param>
    /// <returns>A CBOR object that represents the parsed number or its
    /// closest approximation to it. Returns null if the parsing fails,
    /// including if the string is null or empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless the only digit is 0), an optional decimal point (".", full
    /// stop) with one or more basic digits, and an optional letter E or e
    /// with an optional plus or minus sign and one or more basic digits
    /// (the exponent). A string representing a valid JSON number is not
    /// allowed to contain white space characters, including
    /// spaces.</remarks>
    public static CBORObject ParseJSONNumberAsIntegerOrFloatingPoint(
      string str,
      bool integersOnly,
      bool positiveOnly,
      bool doubleApprox) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      CBORObject cbor = ParseJSONNumber(str, integersOnly, positiveOnly);
      if (cbor == null) {
        return null;
      }
      if (doubleApprox && cbor.Type != CBORType.FloatingPoint) {
        cbor = CBORObject.FromObject(cbor.AsDouble());
      }
      CBORNumber cn = cbor.AsNumber();
      if (cbor.IsIntegral && cn.CanFitInInt64()) {
         long v = cbor.AsInt64();
         if (v >= -(1 << 53) && v <= (1 << 53)) {
           return CBORObject.FromObject(v);
         } else {
             return CBORObject.FromObject(cbor.AsDouble());
         }
      } else {
         return CBORObject.FromObject(cbor.AsDouble());
      }
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259).</summary>
    /// <param name='str'>A string to parse as a JSON number.</param>
    /// <param name='integersOnly'>If true, no decimal points or exponents
    /// are allowed in the string. The default is false.</param>
    /// <param name='positiveOnly'>If true, the leading minus is disallowed
    /// in the string. The default is false.</param>
    /// <param name='preserveNegativeZero'>If true, returns positive zero
    /// if the number is a zero that starts with a minus sign (such as "-0"
    /// or "-0.0"). Otherwise, returns negative zero in this case. The
    /// default is false.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the string is null or
    /// empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless the only digit is 0), an optional decimal point (".", full
    /// stop) with one or more basic digits, and an optional letter E or e
    /// with an optional plus or minus sign and one or more basic digits
    /// (the exponent). A string representing a valid JSON number is not
    /// allowed to contain white space characters, including
    /// spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      bool integersOnly,
      bool positiveOnly,
      bool preserveNegativeZero) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      var offset = 0;
      var negative = false;
      if (str[0] == '-') {
        if (positiveOnly) {
          return null;
        }
        negative = true;
        ++offset;
      }
      var mantInt = 0;
      FastInteger2 mant = null;
      var mantBuffer = 0;
      var mantBufferMult = 1;
      var expBuffer = 0;
      var expBufferMult = 1;
      var haveDecimalPoint = false;
      var haveDigits = false;
      var haveDigitsAfterDecimal = false;
      var haveExponent = false;
      var newScaleInt = 0;
      FastInteger2 newScale = null;
      int i = offset;
      // Ordinary number
      if (i < str.Length && str[i] == '0') {
        ++i;
        haveDigits = true;
        if (i == str.Length) {
          if (preserveNegativeZero && negative) {
            // Negative zero in floating-point format
            // TODO: In next major version, return the following instead:
            // return CBORObject.FromFloatingPointBits(0x8000, 2);
            return CBORObject.FromObject (
                EDecimal.NegativeZero);
          }
          return CBORObject.FromObject(0);
        }
        if (!integersOnly) {
          if (str[i] == '.') {
            haveDecimalPoint = true;
            ++i;
          } else if (str[i] == 'E' || str[i] == 'e') {
            haveExponent = true;
          } else {
            return null;
          }
        } else {
          return null;
        }
      }
      for (; i < str.Length; ++i) {
        if (str[i] >= '0' && str[i] <= '9') {
          var thisdigit = (int)(str[i] - '0');
          if (mantInt > MaxSafeInt) {
            if (mant == null) {
              mant = new FastInteger2(mantInt);
              mantBuffer = thisdigit;
              mantBufferMult = 10;
            } else {
              if (mantBufferMult >= 1000000000) {
                mant.Multiply(mantBufferMult).AddInt(mantBuffer);
                mantBuffer = thisdigit;
                mantBufferMult = 10;
              } else {
                mantBufferMult *= 10;
                mantBuffer = (mantBuffer << 3) + (mantBuffer << 1);
                mantBuffer += thisdigit;
              }
            }
          } else {
            mantInt *= 10;
            mantInt += thisdigit;
          }
          haveDigits = true;
          if (haveDecimalPoint) {
            haveDigitsAfterDecimal = true;
            if (newScaleInt == Int32.MinValue) {
              newScale = newScale ??
                new FastInteger2(newScaleInt);
              newScale.AddInt(-1);
            } else {
              --newScaleInt;
            }
          }
        } else if (!integersOnly && str[i] == '.') {
          if (!haveDigits) {
            // no digits before the decimal point
            return null;
          }
          if (haveDecimalPoint) {
            return null;
          }
          haveDecimalPoint = true;
        } else if (!integersOnly && (str[i] == 'E' || str[i] == 'e')) {
          haveExponent = true;
          ++i;
          break;
        } else {
          return null;
        }
      }
      if (!haveDigits || (haveDecimalPoint && !haveDigitsAfterDecimal)) {
        return null;
      }
      if (mant != null && (mantBufferMult != 1 || mantBuffer != 0)) {
        mant.Multiply(mantBufferMult).AddInt(mantBuffer);
      }
      if (haveExponent) {
        FastInteger2 exp = null;
        var expInt = 0;
        offset = 1;
        haveDigits = false;
        if (i == str.Length) {
          return null;
        }
        if (str[i] == '+' || str[i] == '-') {
          if (str[i] == '-') {
            offset = -1;
          }
          ++i;
        }
        for (; i < str.Length; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
            var thisdigit = (int)(str[i] - '0');
            if (expInt > MaxSafeInt) {
              if (exp == null) {
                exp = new FastInteger2(expInt);
                expBuffer = thisdigit;
                expBufferMult = 10;
              } else {
                if (expBufferMult >= 1000000000) {
                  exp.Multiply(expBufferMult).AddInt(expBuffer);
                  expBuffer = thisdigit;
                  expBufferMult = 10;
                } else {
                  // multiply expBufferMult and expBuffer each by 10
                  expBufferMult = (expBufferMult << 3) + (expBufferMult << 1);
                  expBuffer = (expBuffer << 3) + (expBuffer << 1);
                  expBuffer += thisdigit;
                }
              }
            } else {
              expInt *= 10;
              expInt += thisdigit;
            }
          } else {
            return null;
          }
        }
        if (!haveDigits) {
          return null;
        }
        if (exp != null && (expBufferMult != 1 || expBuffer != 0)) {
          exp.Multiply(expBufferMult).AddInt(expBuffer);
        }
        if (offset >= 0 && newScaleInt == 0 && newScale == null && exp ==
          null) {
          newScaleInt = expInt;
        } else if (exp == null) {
          newScale = newScale ?? new FastInteger2(newScaleInt);
          if (offset < 0) {
            newScale.SubtractInt(expInt);
          } else if (expInt != 0) {
            newScale.AddInt(expInt);
          }
        } else {
          newScale = newScale ?? new FastInteger2(newScaleInt);
          if (offset < 0) {
            newScale.Subtract(exp);
          } else {
            newScale.Add(exp);
          }
        }
      }
      if (i != str.Length) {
        // End of the string wasn't reached, so isn't a number
        return null;
      }
      if ((newScale == null && newScaleInt == 0) || (newScale != null &&
          newScale.Sign == 0)) {
        // No fractional part
        if (mant != null && mant.CanFitInInt32()) {
          mantInt = mant.AsInt32();
          mant = null;
        }
        if (mant == null) {
          // NOTE: mantInt can only be 0 or greater, so overflow is impossible
          #if DEBUG
          if (mantInt < 0) {
            throw new ArgumentException("mantInt(" + mantInt +
              ") is less than 0");
          }
          #endif

          if (negative) {
            mantInt = -mantInt;
            if (preserveNegativeZero && mantInt == 0) {
              // TODO: In next major version, return the following instead:
              // return CBORObject.FromFloatingPointBits(0x8000, 2);
              return CBORObject.FromObject (
                  EDecimal.NegativeZero);
            }
          }
          return CBORObject.FromObject(mantInt);
        } else {
          EInteger bigmant2 = mant.AsBigInteger();
          if (negative) {
            bigmant2 = -(EInteger)bigmant2;
          }
          return CBORObject.FromObject(bigmant2);
        }
      } else {
        EInteger bigmant = (mant == null) ? ((EInteger)mantInt) :
          mant.AsBigInteger();
        EInteger bigexp = (newScale == null) ? ((EInteger)newScaleInt) :
          newScale.AsBigInteger();
        if (negative) {
          bigmant = -(EInteger)bigmant;
        }
        EDecimal edec;
        edec = EDecimal.Create(
          bigmant,
          bigexp);
        if (negative && preserveNegativeZero && bigmant.IsZero) {
          EDecimal negzero = EDecimal.NegativeZero;
          negzero = negzero.Quantize(bigexp, null);
          edec = negzero.Subtract(edec);
        }
        return CBORObject.FromObject(edec);
      }
    }
  }
}
