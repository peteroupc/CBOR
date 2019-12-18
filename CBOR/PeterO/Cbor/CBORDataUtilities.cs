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
        "\u0020does not" + "\u0020begin" +
        "\u0020with '-' before calling that version. If this method call used" +
        "\u0020integersOnly" +
        "\u0020= true, check that the string does not contain '.', 'E', or" +
        "\u0020'e'" + "\u0020before" + "\u0020calling that version.")]
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
    [Obsolete("Instead, call ParseJSONNumber(str, jsonoptions) with" +
        "\u0020a JSONOptions that sets preserveNegativeZero to the" +
        "\u0020desired value, either true or false. If this" +
        "\u0020method call used positiveOnly = true, check that the string" +
        "\u0020does not" + "\u0020begin" +
        "\u0020with '-' before calling that version. If this method call used" +
        "\u0020integersOnly" +
        "\u0020= true, check that the string does not contain '.', 'E', or" +
        "\u0020'e'" + "\u0020before" + "\u0020calling that version.")]
    public static CBORObject ParseJSONNumber(
      string str,
      bool integersOnly,
      bool positiveOnly,
      bool preserveNegativeZero) {
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

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259) from a portion of a text string, and
    /// converts that number to a CBOR object.</summary>
    /// <param name='str'>A text string containing the portion to parse as
    /// a JSON number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='str'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='str'/> (but not more than <paramref
    /// name='str'/> 's length).</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the string is null or
    /// empty.</returns>
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
      int count) {
      return String.IsNullOrEmpty(str) ? null :
        ParseJSONNumber(str,
          offset,
          count,
          JSONOptions.Default);
    }

    internal static CBORObject ParseSmallNumberAsNegative(
      int digit,
      JSONOptions options) {
#if DEBUG
       if (digit <= 0) {
         throw new ArgumentException("digit (" + digit + ") is not greater" +
"\u0020than 0");
       }
#endif

       if (options != null && options.NumberConversion ==
             JSONOptions.ConversionMode.Double) {
         return CBORObject.FromObject((double)(-digit));
       } else {
         // NOTE: Assumes digit is greater than zero, so PreserveNegativeZeros is
         // irrelevant
         return CBORObject.FromObject(-digit);
       }
    }

    internal static CBORObject ParseSmallNumber(int digit, JSONOptions
options) {
#if DEBUG
       if (digit < 0) {
         throw new ArgumentException("digit (" + digit + ") is not greater" +
"\u0020or equal to 0");
       }
#endif

       if (options != null && options.NumberConversion ==
JSONOptions.ConversionMode.Double) {
         return CBORObject.FromObject((double)digit);
       } else {
         // NOTE: Assumes digit is nonnegative, so PreserveNegativeZeros is irrelevant
         return CBORObject.FromObject(digit);
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
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='str'/> is null.</exception>
    /// <exception cref='ArgumentException'>Unsupported conversion
    /// kind.</exception>
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
      return ParseJSONNumber(str, offset, count, options, null);
    }
    internal static CBORObject ParseJSONNumber(
      string str,
      int offset,
      int count,
      JSONOptions options,
      int[] endOfNumber) {
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
      int initialOffset = offset;
      var negative = false;
      if (str[initialOffset] == '-') {
        ++offset;
        negative = true;
      }
      int numOffset = offset;
      var haveDecimalPoint = false;
      var haveDigits = false;
      var haveDigitsAfterDecimal = false;
      var haveExponent = false;
      int i = offset;
      var decimalPointPos = -1;
      // Check syntax
      int k = i;
      if (endPos - 1 > k && str[k] == '0' && str[k + 1] >= '0' &&
         str[k + 1] <= '9') {
        if (endOfNumber != null) {
          endOfNumber[0] = k + 2;
        }
        return null;
      }
      for (; k < endPos; ++k) {
        char c = str[k];
        if (c >= '0' && c <= '9') {
          haveDigits = true;
          haveDigitsAfterDecimal |= haveDecimalPoint;
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
      if (haveExponent) {
        haveDigits = false;
        if (k == endPos) {
          if (endOfNumber != null) {
            endOfNumber[0] = k;
          }
          return null;
        }
        char c = str[k];
        if (c == '+' || c == '-') {
          ++k;
        }
        for (; k < endPos; ++k) {
          c = str[k];
          if (c >= '0' && c <= '9') {
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
      if (!haveExponent && !haveDecimalPoint &&
         (endPos - numOffset) <= 16) {
        // Very common case of all-digit JSON number strings
        // less than 2^53 (with or without number sign)
        long v = 0L;
        int vi = numOffset;
        for (; vi < endPos; ++vi) {
          v = (v * 10) + (int)(str[vi] - '0');
        }
        if ((v != 0 || !negative) && v < (1L << 53) - 1) {
          if (negative) {
            v = -v;
          }
          if (kind == JSONOptions.ConversionMode.Double) {
            return CBORObject.FromObject((double)v);
          } else if (kind == JSONOptions.ConversionMode.Decimal128) {
            return CBORObject.FromObject(EDecimal.FromInt64(v));
          } else {
            return CBORObject.FromObject(v);
          }
        }
      }
      if (kind == JSONOptions.ConversionMode.Full) {
        if (!haveDecimalPoint && !haveExponent) {
          EInteger ei = EInteger.FromSubstring(str, initialOffset, endPos);
          if (preserveNegativeZero && ei.IsZero && negative) {
            // TODO: In next major version, change to EDecimal.NegativeZero
            return CBORObject.FromFloatingPointBits(0x8000, 2);
          }
          return CBORObject.FromObject(ei);
        }
        if (!haveExponent && haveDecimalPoint && (endPos - numOffset) <= 19) {
          // No more than 18 digits plus one decimal point (which
          // should fit a long)
          long lv = 0L;
          int expo = -(endPos - (decimalPointPos + 1));
          int vi = numOffset;
          for (; vi < decimalPointPos; ++vi) {
            lv = checked((lv * 10) + (int)(str[vi] - '0'));
          }
          for (vi = decimalPointPos + 1; vi < endPos; ++vi) {
            lv = checked((lv * 10) + (int)(str[vi] - '0'));
          }
          if (negative) {
            lv = -lv;
          }
          if (!negative || lv != 0) {
            CBORObject cbor = CBORObject.FromObject(
              new CBORObject[] {
                CBORObject.FromObject(expo),
                CBORObject.FromObject(lv),
              });
            return cbor.WithTag(4);
          }
        }
        EDecimal ed = EDecimal.FromString(
          str,
          initialOffset,
          endPos - initialOffset);
        if (ed.IsZero && negative) {
          if (preserveNegativeZero && ed.Exponent.IsZero) {
            // TODO: In next major version, use EDecimal
            return CBORObject.FromFloatingPointBits(0x8000, 2);
          } else if (!preserveNegativeZero) {
            ed = ed.Negate();
          }
        }
        return CBORObject.FromObject(ed);
      } else if (kind == JSONOptions.ConversionMode.Double) {
        double dbl = EFloat.FromString(
          str,
          initialOffset,
          endPos - initialOffset,
          EContext.Binary64).ToDouble();
        if (!preserveNegativeZero && dbl == 0.0) {
          dbl = 0.0;
        }
        return CBORObject.FromObject(dbl);
      } else if (kind == JSONOptions.ConversionMode.Decimal128) {
        EDecimal ed = EDecimal.FromString(
          str,
          initialOffset,
          endPos - initialOffset,
          EContext.Decimal128);
        if (!preserveNegativeZero && ed.IsNegative && ed.IsZero) {
          ed = ed.Negate();
        }
        return CBORObject.FromObject(ed);
      } else if (kind == JSONOptions.ConversionMode.IntOrFloatFromDouble) {
        double dbl = EFloat.FromString(
          str,
          initialOffset,
          endPos - initialOffset,
          EContext.Binary64).ToDouble();
        if (!Double.IsNaN(dbl) && dbl >= -9007199254740991.0 &&
          dbl <= 9007199254740991.0 && Math.Floor(dbl) == dbl) {
          var idbl = (long)dbl;
          return CBORObject.FromObject(idbl);
        }
        return CBORObject.FromObject(dbl);
      } else if (kind == JSONOptions.ConversionMode.IntOrFloat) {
        EContext ctx = EContext.Binary64.WithBlankFlags();
        double dbl = EFloat.FromString(
          str,
          initialOffset,
          endPos - initialOffset,
          ctx).ToDouble();
        if ((ctx.Flags & EContext.FlagInexact) != 0) {
          // Inexact conversion to double, meaning that the string doesn't
          // represent an integer in [-(2^53)+1, 2^53), which is representable
          // exactly as double, so treat as ConversionMode.Double
          if (!preserveNegativeZero && dbl == 0.0) {
            dbl = 0.0;
          }
          return CBORObject.FromObject(dbl);
        } else {
          // Exact conversion; treat as ConversionMode.IntToFloatFromDouble
          if (!Double.IsNaN(dbl) && dbl >= -9007199254740991.0 &&
            dbl <= 9007199254740991.0 && Math.Floor(dbl) == dbl) {
            var idbl = (long)dbl;
            return CBORObject.FromObject(idbl);
          }
          return CBORObject.FromObject(dbl);
        }
      } else {
        throw new ArgumentException("Unsupported conversion kind.");
      }
    }
  }
}
