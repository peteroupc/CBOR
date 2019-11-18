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
    private static readonly JSONOptions DefaultOptions =
        new JSONOptions(String.Empty);
    private static readonly JSONOptions PreserveNegZeroNo =
        new JSONOptions("preservenegativezero=0");
    private static readonly JSONOptions PreserveNegZeroYes =
        new JSONOptions("preservenegativezero=1");

    /// <summary>Parses a number whose format follows the JSON
    /// specification. The method uses a JSONOptions with all default
    /// properties except for a PreserveNegativeZero property of
    /// false.</summary>
    /// <param name='str'>A text string to parse as a JSON string.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// positive zero if the number is a zero that starts with a minus sign
    /// (such as "-0" or "-0.0"). Returns null if the parsing fails,
    /// including if the string is null or empty.</returns>
    public static CBORObject ParseJSONNumber(string str) {
      // TODO: Preserve negative zeros in next major version
      return ParseJSONNumber(str, PreserveNegZeroNo);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259). The method uses a JSONOptions with all
    /// default properties except for a PreserveNegativeZero property of
    /// false.</summary>
    /// <param name='str'>A text string to parse as a JSON number.</param>
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
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A string representing a
    /// valid JSON number is not allowed to contain white space characters,
    /// including spaces.</remarks>
    [Obsolete("Call the one-argument version of this method instead. If this" +
      "\u0020method call used positiveOnly = true, check that the string" +
"\u0020does not" +
      "\u0020begin" +
      "\u0020with '-' before calling that version. If this method call used" +
      "\u0020integersOnly" +
      "\u0020= true, check that the string does not contain '.', 'E', or 'e'" +
"\u0020before" +
      "\u0020calling that version.")]
    public static CBORObject ParseJSONNumber(
      string str,
      bool integersOnly,
      bool positiveOnly) {
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      if (integersOnly) {
        for (var i = 0; i < str.Length; ++i) {
if (str[i] >= '0' && str[i] <= '9' && (i > 0 || str[i] != '-')) {
  return null;
}
}
      }
      return (positiveOnly && str[0] == '-') ? null :
         ParseJSONNumber(
           str,
           0,
           str.Length,
           PreserveNegZeroNo);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259).</summary>
    /// <param name='str'>A text string to parse as a JSON number.</param>
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
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A string representing a
    /// valid JSON number is not allowed to contain white space characters,
    /// including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      bool integersOnly,
      bool positiveOnly,
      bool preserveNegativeZero) {
      // TODO: Deprecate integersOnly?
      // TODO: Deprecate this method eventually?
      if (String.IsNullOrEmpty(str)) {
        return null;
      }
      if (integersOnly) {
        for (var i = 0; i < str.Length; ++i) {
if (str[i] >= '0' && str[i] <= '9' && (i > 0 || str[i] != '-')) {
  return null;
}
}
      }
      JSONOptions jo = preserveNegativeZero ? PreserveNegZeroYes :
         PreserveNegZeroNo;
      return (positiveOnly && str[0] == '-') ? null :
         ParseJSONNumber(str,
           0,
           str.Length,
           jo);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259) and converts that number to a CBOR
    /// object.</summary>
    /// <param name='str'>A text string to parse as a JSON number.</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the string is null or
    /// empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A string representing a
    /// valid JSON number is not allowed to contain white space characters,
    /// including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      JSONOptions options) {
      return String.IsNullOrEmpty(str) ? null :
         ParseJSONNumber(str,
           0,
           str.Length,
           options);
    }

private static EInteger FastEIntegerFromString(string str, int index, int
endIndex) {
#if DEBUG
  if (str == null) {
    throw new ArgumentNullException(nameof(str));
  }
  if (index < 0) {
    throw new ArgumentException("index (" + index + ") is not greater or" +
"\u0020equal to 0");
  }
  if (index > str.Length) {
    throw new ArgumentException("index (" + index + ") is not less or equal" +
"\u0020to " + str.Length);
  }
  if (endIndex < 0) {
    throw new ArgumentException("endIndex plus 1 (" + endIndex + ") is not" +
"\u0020greater or" +
"\u0020equal to 0");
  }
  if (endIndex > str.Length) {
    throw new ArgumentException("endIndex plus 1 (" + endIndex + ") is not" +
"\u0020less or equal" +
"\u0020to " + str.Length);
  }
#endif

  if (endIndex - index > 32) {
     // DebugUtility.Log("FastEInteger " + index + " " + endIndex + " len=" +
    // (endIndex - index));
    int midIndex = index + (endIndex - index) / 2;
    EInteger eia = FastEIntegerFromString(str, index, midIndex);
    EInteger eib = FastEIntegerFromString(str, midIndex, endIndex);
    EInteger mult = EInteger.FromInt32(10).Pow(endIndex - midIndex);
    return eia.Multiply(mult).Add(eib);
  } else {
    return EInteger.FromSubstring(str, index, endIndex);
  }
}

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259) and converts that number to a CBOR
    /// object.</summary>
    /// <param name='str'>A text string to parse as a JSON number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='str'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the string is null or empty
    /// or <paramref name='count'/> is 0 or less.</returns>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='str'/> 's length, or <paramref
    /// name='str'/> 's length minus <paramref name='offset'/> is less than
    /// <paramref name='count'/>.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A string representing a
    /// valid JSON number is not allowed to contain white space characters,
    /// including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      int offset,
      int count,
      JSONOptions options) {
      if (String.IsNullOrEmpty(str) || count <= 0) {
        return null;
      }
      if (offset < 0 || offset > str.Length) {
        return null;
      }
      if (count > str.Length || str.Length - offset < count) {
        return null;
      }
      JSONOptions opt = options ?? DefaultOptions;
      bool preserveNegativeZero = options.PreserveNegativeZero;
      JSONOptions.ConversionMode kind = options.NumberConversion;
      int endPos = offset + count;
      var negative = false;
      if (str[0] == '-') {
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
      if (i < endPos && str[i] == '0') {
        ++i;
        haveDigits = true;
        if (i == endPos) {
          if (preserveNegativeZero && negative) {
             // Negative zero in floating-point format
             return (kind == JSONOptions.ConversionMode.Double) ?
CBORObject.FromFloatingPointBits(0x8000, 2) :
CBORObject.FromObject(EDecimal.NegativeZero);
           }
          return CBORObject.FromObject(0);
        }
        if (str[i] == '.') {
          haveDecimalPoint = true;
          ++i;
        } else if (str[i] == 'E' || str[i] == 'e') {
          haveExponent = true;
        } else {
          return null;
        }
      }
      if (endPos - i >= 400) {
        // Check for certain fast cases
        int k = i;
        bool hdp = haveDecimalPoint;
        bool hd = haveDigits;
        bool hdad = haveDigitsAfterDecimal;
        bool hex = haveExponent;
        var haveManyDigits = false;
        var eplus = true;
        for (; k < endPos; ++k) {
          if (str[k] >= '0' && str[k] <= '9') {
            haveDigits = true;
            if (k - i > 400 && !haveDecimalPoint) {
               // Indicates that this value is bigger
               // than a decimal can store
               haveManyDigits = true;
            }
            if (haveDecimalPoint) {
              haveDigitsAfterDecimal = true;
            }
          } else if (str[k] == '.') {
            if (!haveDigits) {
              // no digits before the decimal point
              return null;
            } else if (haveDecimalPoint) {
              return null;
            }
            haveDecimalPoint = true;
          } else if (str[k] == 'E' || str[k] == 'e') {
            ++k;
            haveExponent = true;
            break;
          } else {
            return null;
          }
        }
        if (!haveDigits || (haveDecimalPoint && !haveDigitsAfterDecimal)) {
          return null;
        }
        if (haveExponent) {
          haveDigits = false;
          if (k == endPos) {
            return null;
          }
          if (str[k] == '+' || str[k] == '-') {
            if (str[k] == '-') {
              eplus = false;
            }
            ++k;
           }
           for (; k < endPos; ++k) {
             if (str[k] >= '0' && str[k] <= '9') {
               haveDigits = true;
             } else {
               return null;
             }
           }
           if (!haveDigits) {
             return null;
           }
        }
        if (k != endPos) {
          return null;
        }
        if (kind == JSONOptions.ConversionMode.Double ||
           kind == JSONOptions.ConversionMode.IntOrFloat ||
           kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
          if (haveManyDigits && (!haveExponent || eplus)) {
             return negative ? CBORObject.FromObject(Double.NegativeInfinity) :
                  CBORObject.FromObject(Double.PositiveInfinity);
          }
          // TODO: Fast cases involving negative exponents
        }
        haveDigitsAfterDecimal = hdad;
        haveDigits = hd;
        haveDecimalPoint = hdp;
        haveExponent = hex;
      }
      int digitStart = (haveDecimalPoint || haveExponent) ? -1 : i;
      int digitEnd = (haveDecimalPoint || haveExponent) ? -1 : i;
      int decimalDigitStart = haveDecimalPoint ? i : -1;
      int decimalDigitEnd = haveDecimalPoint ? i : -1;
      for (; i < endPos; ++i) {
        if (str[i] >= '0' && str[i] <= '9') {
          var thisdigit = (int)(str[i] - '0');
          if (haveDecimalPoint) {
            decimalDigitEnd = i + 1;
          } else {
 digitEnd = i + 1;
}
          if (mantInt <= MaxSafeInt) {
  mantInt *= 10;
  mantInt += thisdigit;
          }
          haveDigits = true;
          if (haveDecimalPoint) {
            haveDigitsAfterDecimal = true;
            if (newScaleInt == Int32.MinValue) {
              newScale = newScale ?? new FastInteger2(newScaleInt);
              newScale.AddInt(-1);
            } else {
              --newScaleInt;
            }
          }
        } else if (str[i] == '.') {
          if (!haveDigits) {
            // no digits before the decimal point
            return null;
          }
          if (haveDecimalPoint) {
            return null;
          }
          haveDecimalPoint = true;
          decimalDigitStart = i + 1;
          decimalDigitEnd = i + 1;
        } else if (str[i] == 'E' || str[i] == 'e') {
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
      if (mantInt > MaxSafeInt) {
        if (haveDecimalPoint) {
  if (digitStart < 0) {
    EInteger eidec = FastEIntegerFromString(
      str,
      decimalDigitStart,
      decimalDigitEnd);
    mant = new FastInteger2(eidec);
  } else {
    #if DEBUG
    if (digitStart >= digitEnd) {
      throw new InvalidOperationException();
    }
    #endif
    EInteger eidec;
    string tmpstr = str.Substring(digitStart, digitEnd - digitStart) +
              str.Substring(decimalDigitStart, decimalDigitEnd -
decimalDigitStart);
    eidec = FastEIntegerFromString(tmpstr, 0, tmpstr.Length);
    mant = new FastInteger2(eidec);
  }
} else if (digitStart >= 0) {
  EInteger eimant = FastEIntegerFromString(str, digitStart, digitEnd);
  mant = new FastInteger2(eimant);
}
      }
      if (haveExponent) {
        FastInteger2 exp = null;
        var expInt = 0;
        offset = 1;
        haveDigits = false;
        if (i == endPos) {
          return null;
        }
        if (str[i] == '+' || str[i] == '-') {
          if (str[i] == '-') {
            offset = -1;
          }
          ++i;
        }
        for (; i < endPos; ++i) {
          if (str[i] >= '0' && str[i] <= '9') {
            haveDigits = true;
            var thisdigit = (int)(str[i] - '0');
            if (expInt > MaxSafeInt) {
              // TODO: Use same optimization as in mant above
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
      if (i != endPos) {
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
          // NOTE: mantInt can only be 0 or greater, so overflow is
          // impossible with Int32.MinValue
          #if DEBUG
          if (mantInt < 0) {
            throw new InvalidOperationException("mantInt(" + mantInt +
              ") is less than 0");
          }
          #endif

          if (negative) {
            mantInt = -mantInt;
            if (preserveNegativeZero && mantInt == 0) {
              if (kind == JSONOptions.ConversionMode.Double) {
                return CBORObject.FromFloatingPointBits(0x8000, 2);
              }
              return CBORObject.FromObject(
                  EDecimal.NegativeZero);
            }
          }
          if (kind == JSONOptions.ConversionMode.Double) {
            return CBORObject.FromObject((double)mantInt);
          } else {
            // mantInt is a 32-bit integer, write as CBOR integer in
            // all current kinds other than Double
            return CBORObject.FromObject(mantInt);
          }
        } else {
          EInteger bigmant2 = mant.AsEInteger();
          if (negative) {
            bigmant2 = -(EInteger)bigmant2;
          }
          if (kind == JSONOptions.ConversionMode.Double) {
            // An arbitrary-precision integer; convert to double
            return CBORObject.FromObject(
              EFloat.FromEInteger(bigmant2).ToDouble());
          } else if (kind == JSONOptions.ConversionMode.IntOrFloat ||
               kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
            if (bigmant2.CanFitInInt64()) {
              long longmant2 = bigmant2.ToInt64Checked();
              if (longmant2 >= (-(1 << 53)) + 1 && longmant2 <= (1 << 53) - 1) {
                // An arbitrary-precision integer that's "small enough";
                // return a CBOR object of that integer
                return CBORObject.FromObject(bigmant2);
              }
            }
            return CBORObject.FromObject(
              EFloat.FromEInteger(bigmant2).ToDouble());
          }
          return CBORObject.FromObject(bigmant2);
        }
      } else {
        EInteger bigmant = (mant == null) ? ((EInteger)mantInt) :
          mant.AsEInteger();
        EInteger bigexp = (newScale == null) ? ((EInteger)newScaleInt) :
          newScale.AsEInteger();
        if (negative) {
          bigmant = -(EInteger)bigmant;
        }
        EDecimal edec;
        edec = EDecimal.Create(
          bigmant,
          bigexp);
        if (negative && preserveNegativeZero && bigmant.IsZero) {
          if (kind == JSONOptions.ConversionMode.Double) {
            return CBORObject.FromFloatingPointBits(0x8000, 2);
          }
          EDecimal negzero = EDecimal.NegativeZero;
          negzero = negzero.Quantize(bigexp, null);
          edec = negzero.Subtract(edec);
        }
        // Converting the EDecimal to a CBOR object
        if (kind == JSONOptions.ConversionMode.Double) {
          double dbl = edec.ToDouble();
          if (preserveNegativeZero && dbl >= 0.0) {
            dbl = Math.Abs(dbl);
          }
          return CBORObject.FromObject(dbl);
        } else if (kind == JSONOptions.ConversionMode.IntOrFloat ||
              kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
          double dbl;
          CBORObject cbor = (kind == JSONOptions.ConversionMode.IntOrFloat) ?
            CBORObject.FromObject(edec) :
            CBORObject.FromObject(edec.ToDouble());
          CBORNumber cn = cbor.AsNumber();
          if (cn.IsInteger() && cn.CanFitInInt64()) {
             long v = cn.ToInt64Checked();
             if (v >= (-(1 << 53)) + 1 && v <= (1 << 53) - 1) {
               return CBORObject.FromObject(v);
             } else {
               dbl = cbor.AsDouble();
            }
          } else {
            dbl = edec.ToDouble();
          }
          if (preserveNegativeZero && dbl >= 0.0) {
            dbl = Math.Abs(dbl);
          }
          return CBORObject.FromObject(dbl);
        }
        return CBORObject.FromObject(edec);
      }
    }
  }
}
