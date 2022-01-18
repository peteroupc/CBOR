/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

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

    private const long DoubleNegInfinity = unchecked((long)(0xfffL << 52));
    private const long DoublePosInfinity = unchecked((long)(0x7ffL << 52));

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
            sb.Append(')');
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
          long bits = obj.AsDoubleBits();
          simvalue = bits == DoubleNegInfinity ? "-Infinity" : (
              bits == DoublePosInfinity ? "Infinity" : (
                CBORUtilities.DoubleBitsNaN(bits) ? "NaN" :
obj.Untag().ToJSONString()));
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
          sb.Append((char)0x27);
          break;
        }
        case CBORType.TextString: {
          sb = sb == null ? new StringBuilder() : sb;
          sb.Append('\"');
          string ostring = obj.AsString();
          int length = ostring.Length;
          for (var i = 0; i < length; ++i) {
            int cp = DataUtilities.CodePointAt(ostring, i, 0);
            if (cp >= 0x10000) {
              sb.Append("\\U");
              sb.Append(HexAlphabet[(cp >> 20) & 15]);
              sb.Append(HexAlphabet[(cp >> 16) & 15]);
              sb.Append(HexAlphabet[(cp >> 12) & 15]);
              sb.Append(HexAlphabet[(cp >> 8) & 15]);
              sb.Append(HexAlphabet[(cp >> 4) & 15]);
              sb.Append(HexAlphabet[cp & 15]);
              ++i;
            } else if (cp >= 0x7F || cp < 0x20 || cp == (int)'\\' || cp ==
(int)'\"') {
              sb.Append("\\u");
              sb.Append(HexAlphabet[(cp >> 12) & 15]);
              sb.Append(HexAlphabet[(cp >> 8) & 15]);
              sb.Append(HexAlphabet[(cp >> 4) & 15]);
              sb.Append(HexAlphabet[cp & 15]);
            } else {
              sb.Append((char)cp);
            }
          }
          sb.Append('\"');
          break;
        }
        case CBORType.Array: {
          sb = sb ?? new StringBuilder();
          var first = true;
          sb.Append('[');
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
          sb.Append(']');
          break;
        }
        case CBORType.Map: {
          sb = sb ?? new StringBuilder();
          var first = true;
          sb.Append('{');
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
          sb.Append('}');
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

    internal static readonly JSONOptions DefaultOptions =
      new JSONOptions(String.Empty);
    private static readonly JSONOptions PreserveNegZeroNo =
      new JSONOptions("preservenegativezero=0");
    private static readonly JSONOptions PreserveNegZeroYes =
      new JSONOptions("preservenegativezero=1");

    /// <summary>Parses a number whose format follows the JSON
    /// specification. The method uses a JSONOptions with all default
    /// properties except for a PreserveNegativeZero property of
    /// false.</summary>
    /// <param name='str'>A text string to parse as a JSON number.</param>
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
    /// one or more basic digits (the exponent). A text string representing
    /// a valid JSON number is not allowed to contain white space
    /// characters, including spaces.</remarks>
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
    /// one or more basic digits (the exponent). A text string representing
    /// a valid JSON number is not allowed to contain white space
    /// characters, including spaces.</remarks>
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
    /// one or more basic digits (the exponent). A text string representing
    /// a valid JSON number is not allowed to contain white space
    /// characters, including spaces.</remarks>
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
    /// one or more basic digits (the exponent). A text string representing
    /// a valid JSON number is not allowed to contain white space
    /// characters, including spaces.</remarks>
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
        return CBORObject.FromFloatingPointBits(
           CBORUtilities.IntegerToDoubleBits(-digit),
           8);
      } else if (options != null && options.NumberConversion ==
        JSONOptions.ConversionMode.Decimal128) {
        return CBORObject.FromObject(EDecimal.FromInt32(-digit));
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
        return CBORObject.FromFloatingPointBits(
           CBORUtilities.IntegerToDoubleBits(digit),
           8);
      } else if (options != null && options.NumberConversion ==
        JSONOptions.ConversionMode.Decimal128) {
        return CBORObject.FromObject(EDecimal.FromInt32(digit));
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
    /// one or more basic digits (the exponent). A text string representing
    /// a valid JSON number is not allowed to contain white space
    /// characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      string str,
      int offset,
      int count,
      JSONOptions options) {
      return CBORDataUtilitiesTextString.ParseJSONNumber(
        str,
        offset,
        count,
        options,
        null);
    }

    /// <summary>Parses a number from a byte sequence whose format follows
    /// the JSON specification (RFC 8259) and converts that number to a
    /// CBOR object.</summary>
    /// <param name='bytes'>A sequence of bytes to parse as a JSON
    /// number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='bytes'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the byte sequence is null
    /// or empty or <paramref name='count'/> is 0 or less.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <exception cref='ArgumentException'>Unsupported conversion
    /// kind.</exception>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A byte sequence
    /// representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      byte[] bytes,
      int offset,
      int count,
      JSONOptions options) {
      return CBORDataUtilitiesByteArrayString.ParseJSONNumber(
        bytes,
        offset,
        count,
        options,
        null);
    }

    /// <summary>Parses a number from a byte sequence whose format follows
    /// the JSON specification (RFC 8259) and converts that number to a
    /// CBOR object.</summary>
    /// <param name='bytes'>A sequence of bytes to parse as a JSON
    /// number.</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the byte sequence is null
    /// or empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A byte sequence
    /// representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      byte[] bytes,
      JSONOptions options) {
      return (bytes == null || bytes.Length == 0) ? null :
        ParseJSONNumber(bytes,
          0,
          bytes.Length,
          options);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259) from a portion of a byte sequence, and
    /// converts that number to a CBOR object.</summary>
    /// <param name='bytes'>A sequence of bytes to parse as a JSON
    /// number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='bytes'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='bytes'/> (but not more than <paramref
    /// name='bytes'/> 's length).</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the byte sequence is null
    /// or empty.</returns>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='bytes'/> 's length, or <paramref
    /// name='bytes'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='count'/>.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bytes'/> is null.</exception>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A byte sequence
    /// representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      byte[] bytes,
      int offset,
      int count) {
      return (bytes == null || bytes.Length == 0) ? null :
        ParseJSONNumber(bytes,
          offset,
          count,
          JSONOptions.Default);
    }

    /// <summary>Parses a number from a byte sequence whose format follows
    /// the JSON specification. The method uses a JSONOptions with all
    /// default properties except for a PreserveNegativeZero property of
    /// false.</summary>
    /// <param name='bytes'>A byte sequence to parse as a JSON
    /// number.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// positive zero if the number is a zero that starts with a minus sign
    /// (such as "-0" or "-0.0"). Returns null if the parsing fails,
    /// including if the byte sequence is null or empty.</returns>
    public static CBORObject ParseJSONNumber(byte[] bytes) {
      // TODO: Preserve negative zeros in next major version
      return ParseJSONNumber(bytes, PreserveNegZeroNo);
    }

    /// <summary>Parses a number from a sequence of <c>char</c> s whose
    /// format follows the JSON specification (RFC 8259) and converts that
    /// number to a CBOR object.</summary>
    /// <param name='chars'>A sequence of <c>char</c> s to parse as a JSON
    /// number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='chars'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='chars'/> (but not more than <paramref
    /// name='chars'/> 's length).</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the sequence of <c>char</c>
    /// s is null or empty or <paramref name='count'/> is 0 or
    /// less.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    /// <exception cref='ArgumentException'>Unsupported conversion
    /// kind.</exception>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A sequence of <c>char</c>
    /// s representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      char[] chars,
      int offset,
      int count,
      JSONOptions options) {
      return CBORDataUtilitiesCharArrayString.ParseJSONNumber(
        chars,
        offset,
        count,
        options,
        null);
    }

    /// <summary>Parses a number from a sequence of <c>char</c> s whose
    /// format follows the JSON specification (RFC 8259) and converts that
    /// number to a CBOR object.</summary>
    /// <param name='chars'>A sequence of <c>char</c> s to parse as a JSON
    /// number.</param>
    /// <param name='options'>An object containing options to control how
    /// JSON numbers are decoded to CBOR objects. Can be null, in which
    /// case a JSONOptions object with all default properties is used
    /// instead.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the sequence of <c>char</c>
    /// s is null or empty.</returns>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A sequence of <c>char</c>
    /// s representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      char[] chars,
      JSONOptions options) {
      return (chars == null || chars.Length == 0) ? null :
        ParseJSONNumber(chars,
          0,
          chars.Length,
          options);
    }

    /// <summary>Parses a number whose format follows the JSON
    /// specification (RFC 8259) from a portion of a sequence of
    /// <c>char</c> s, and converts that number to a CBOR object.</summary>
    /// <param name='chars'>A sequence of <c>char</c> s to parse as a JSON
    /// number.</param>
    /// <param name='offset'>An index, starting at 0, showing where the
    /// desired portion of <paramref name='chars'/> begins.</param>
    /// <param name='count'>The length, in code units, of the desired
    /// portion of <paramref name='chars'/> (but not more than <paramref
    /// name='chars'/> 's length).</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// null if the parsing fails, including if the sequence of <c>char</c>
    /// s is null or empty.</returns>
    /// <exception cref='ArgumentException'>Either <paramref
    /// name='offset'/> or <paramref name='count'/> is less than 0 or
    /// greater than <paramref name='chars'/> 's length, or <paramref
    /// name='chars'/> 's length minus <paramref name='offset'/> is less
    /// than <paramref name='count'/>.</exception>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='chars'/> is null.</exception>
    /// <remarks>Roughly speaking, a valid JSON number consists of an
    /// optional minus sign, one or more basic digits (starting with 1 to 9
    /// unless there is only one digit and that digit is 0), an optional
    /// decimal point (".", full stop) with one or more basic digits, and
    /// an optional letter E or e with an optional plus or minus sign and
    /// one or more basic digits (the exponent). A sequence of <c>char</c>
    /// s representing a valid JSON number is not allowed to contain white
    /// space characters, including spaces.</remarks>
    public static CBORObject ParseJSONNumber(
      char[] chars,
      int offset,
      int count) {
      return (chars == null || chars.Length == 0) ? null :
        ParseJSONNumber(chars,
          offset,
          count,
          JSONOptions.Default);
    }

    /// <summary>Parses a number from a sequence of <c>char</c> s whose
    /// format follows the JSON specification. The method uses a
    /// JSONOptions with all default properties except for a
    /// PreserveNegativeZero property of false.</summary>
    /// <param name='chars'>A sequence of <c>char</c> s to parse as a JSON
    /// number.</param>
    /// <returns>A CBOR object that represents the parsed number. Returns
    /// positive zero if the number is a zero that starts with a minus sign
    /// (such as "-0" or "-0.0"). Returns null if the parsing fails,
    /// including if the sequence of <c>char</c> s is null or
    /// empty.</returns>
    public static CBORObject ParseJSONNumber(char[] chars) {
      // TODO: Preserve negative zeros in next major version
      return ParseJSONNumber(chars, PreserveNegZeroNo);
    }
  }
}
