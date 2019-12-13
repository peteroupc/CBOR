using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
  /// <summary>Includes options to control how CBOR objects are converted
  /// to JSON.</summary>
  public sealed class JSONOptions {
    /// <summary>Specifies how JSON numbers are converted to CBOR when
    /// decoding JSON.</summary>
    public enum ConversionMode {
       /// <summary>JSON numbers are decoded to CBOR using the full precision
       /// given in the JSON text. This may involve numbers being converted to
       /// arbitrary-precision integers or decimal numbers, where
       /// appropriate.</summary>
       Full,

       /// <summary>JSON numbers are decoded to CBOR as their closest-rounded
       /// approximation as 64-bit binary floating-point numbers. (In some
       /// cases, numbers extremely close to zero may underflow to positive or
       /// negative zero, and numbers of extremely large magnitude may
       /// overflow to infinity.).</summary>
       Double,

       /// <summary>A JSON number is decoded to CBOR either as a CBOR integer
       /// (major type 0 or 1) if the JSON number represents an integer at
       /// least -(2^53)+1 and less than 2^53, or as their closest-rounded
       /// approximation as 64-bit binary floating-point numbers otherwise.
       /// For example, the JSON number 0.99999999999999999999999999999999999
       /// is not an integer, so it's converted to its closest floating-point
       /// approximation, namely 1.0. (In some cases, numbers extremely close
       /// to zero may underflow to positive or negative zero, and numbers of
       /// extremely large magnitude may overflow to infinity.).</summary>
       IntOrFloat,

       /// <summary>A JSON number is decoded to CBOR either as a CBOR integer
       /// (major type 0 or 1) if the number's closest-rounded approximation
       /// as a 64-bit binary floating-point number represents an integer at
       /// least -(2^53)+1 and less than 2^53, or as that approximation
       /// otherwise. For example, the JSON number
       /// 0.99999999999999999999999999999999999 is the integer 1 when rounded
       /// to its closest floating-point approximation (1.0), so it's
       /// converted to the CBOR integer 1 (major type 0). (In some cases,
       /// numbers extremely close to zero may underflow to zero, and numbers
       /// of extremely large magnitude may overflow to infinity.).</summary>
       IntOrFloatFromDouble,

       /// <summary>JSON numbers are decoded to CBOR as their closest-rounded
       /// approximation to an IEEE 854 decimal128 value, using the rules for
       /// the EDecimal form of that approximation as given in the
       /// <c>CBORObject.FromObject(EDecimal)</c> method.</summary>
       Decimal128,
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with default
    /// options.</summary>
    public JSONOptions() : this(String.Empty) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with the given value
    /// for the Base64Padding option.</summary>
    /// <param name='base64Padding'>Whether padding is included when
    /// writing data in base64url or traditional base64 format to
    /// JSON.</param>
    [Obsolete("Use the string constructor instead.")]
    public JSONOptions(bool base64Padding)
      : this("base64Padding=" + (base64Padding ? "1" : "0")) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with the given values
    /// for the options.</summary>
    /// <param name='base64Padding'>Whether padding is included when
    /// writing data in base64url or traditional base64 format to
    /// JSON.</param>
    /// <param name='replaceSurrogates'>Whether surrogate code points not
    /// part of a surrogate pair (which consists of two consecutive
    /// <c>char</c> s forming one Unicode code point) are each replaced
    /// with a replacement character (U+FFFD). The default is false; an
    /// exception is thrown when such code points are encountered.</param>
#pragma warning disable CS0618
    [Obsolete("Use the string constructor instead.")]
    public JSONOptions(bool base64Padding, bool replaceSurrogates)
      : this("base64Padding=" + (base64Padding ? "1" : "0") +
           ";replacesurrogates=" + (replaceSurrogates ? "1" : "0")) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class.</summary>
    /// <param name='paramString'>A string setting forth the options to
    /// use. This is a semicolon-separated list of options, each of which
    /// has a key and a value separated by an equal sign ("="). Whitespace
    /// and line separators are not allowed to appear between the
    /// semicolons or between the equal signs, nor may the string begin or
    /// end with whitespace. The string can be empty, but cannot be null.
    /// The following is an example of this parameter:
    /// <c>base64padding=false;replacesurrogates=true</c>. The key can be
    /// any one of the following where the letters can be any combination
    /// of basic upper-case and/or basic lower-case letters:
    /// <c>base64padding</c>, <c>replacesurrogates</c>,
    /// <c>allowduplicatekeys</c>, <c>preservenegativezero</c>,
    /// <c>numberconversion</c>. Other keys are ignored. (Keys are
    /// compared using a basic case-insensitive comparison, in which two
    /// strings are equal if they match after converting the basic
    /// upper-case letters A to Z (U+0041 to U+005A) in both strings to
    /// basic lower-case letters.) If two or more key/value pairs have
    /// equal keys (in a basic case-insensitive comparison), the value
    /// given for the last such key is used. The first four keys just given
    /// can have a value of <c>1</c>, <c>true</c>, <c>yes</c>, or
    /// <c>on</c> (where the letters can be any combination of basic
    /// upper-case and/or basic lower-case letters), which means true, and
    /// any other value meaning false. The last key,
    /// <c>numberconversion</c>, can have a value of any name given in the
    /// <c>JSONOptions.ConversionMode</c> enumeration (where the letters
    /// can be any combination of basic upper-case and/or basic lower-case
    /// letters), and any other value is unrecognized. (If the
    /// <c>numberconversion</c> key is not given, its value is treated as
    /// <c>full</c>. If that key is given, but has an unrecognized value,
    /// an exception is thrown.) For example, <c>base64padding=Yes</c> and
    /// <c>base64padding=1</c> both set the <c>Base64Padding</c> property
    /// to true, and <c>numberconversion=double</c> sets the
    /// <c>NumberConversion</c> property to <c>ConversionMode.Double</c>
    /// .</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null. In the future, this class may allow
    /// other keys to store other kinds of values, not just true or
    /// false.</exception>
    /// <exception cref='ArgumentException'>An unrecognized value for
    /// <c>numberconversion</c> was given.</exception>
    public JSONOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.PreserveNegativeZero = parser.GetBoolean(
        "preservenegativezero",
        true);
      this.AllowDuplicateKeys = parser.GetBoolean(
        "allowduplicatekeys",
        false);
      this.Base64Padding = parser.GetBoolean("base64padding", true);
      this.ReplaceSurrogates = parser.GetBoolean(
        "replacesurrogates",
        false);
      this.NumberConversion = ToNumberConversion(parser.GetLCString(
        "numberconversion",
        null));
    }

    /// <summary>Gets the values of this options object's properties in
    /// text form.</summary>
    /// <returns>A text string containing the values of this options
    /// object's properties. The format of the string is the same as the
    /// one described in the String constructor for this class.</returns>
    public override string ToString() {
      return new StringBuilder()
        .Append("base64padding=").Append(this.Base64Padding ? "true" : "false")
        .Append(";replacesurrogates=")
        .Append(this.ReplaceSurrogates ? "true" : "false")
        .Append(this.PreserveNegativeZero ? "true" : "false")
        .Append(";numberconversion=").Append(this.FromNumberConversion())
        .Append(";allowduplicatekeys=")
        .Append(this.AllowDuplicateKeys ? "true" : "false")
        .ToString();
    }

    /// <summary>The default options for converting CBOR objects to
    /// JSON.</summary>
    public static readonly JSONOptions Default = new JSONOptions();

    /// <summary>Gets a value indicating whether the Base64Padding property
    /// is true. This property has no effect; in previous versions, this
    /// property meant that padding was written out when writing base64url
    /// or traditional base64 to JSON.</summary>
    /// <value>A value indicating whether the Base64Padding property is
    /// true.</value>
    [Obsolete("This property now has no effect. This library now includes " +
"\u0020necessary padding when writing traditional base64 to JSON and" +
"\u0020includes no padding when writing base64url to JSON, in " +
"\u0020accordance with the revision of the CBOR specification.")]
    public bool Base64Padding {
      get;
      private set;
    }

    private string FromNumberConversion() {
      ConversionMode kind = this.NumberConversion;
      if (kind == ConversionMode.Full) {
        return "full";
      }
      if (kind == ConversionMode.Double) {
        return "double";
      }
      if (kind == ConversionMode.Decimal128) {
        return "decimal128";
      }
      if (kind == ConversionMode.IntOrFloat) {
        return "intorfloat";
      }
      return (kind == ConversionMode.IntOrFloatFromDouble) ?
"intorfloatfromdouble" : "full";
    }

    private static ConversionMode ToNumberConversion(string str) {
      if (str != null) {
        if (str.Equals("full", StringComparison.Ordinal)) {
          return ConversionMode.Full;
        }
        if (str.Equals("double", StringComparison.Ordinal)) {
          return ConversionMode.Double;
        }
        if (str.Equals("decimal128", StringComparison.Ordinal)) {
          return ConversionMode.Decimal128;
        }
        if (str.Equals("intorfloat", StringComparison.Ordinal)) {
          return ConversionMode.IntOrFloat;
        }
        if (str.Equals("intorfloatfromdouble", StringComparison.Ordinal)) {
          return ConversionMode.IntOrFloatFromDouble;
        }
      } else {
        return ConversionMode.Full;
      }
      throw new ArgumentException("Unrecognized conversion mode");
    }

    /// <summary>Gets a value indicating whether the JSON decoder should
    /// preserve the distinction between positive zero and negative zero
    /// when the decoder decodes JSON to a floating-point number format
    /// that makes this distinction. For a value of <c>false</c>, if the
    /// result of parsing a JSON string would be a floating-point negative
    /// zero, that result is a positive zero instead. (Note that this
    /// property has no effect for conversion kind
    /// <c>IntOrFloatFromDouble</c>, where floating-point zeros are not
    /// possible.).</summary>
    /// <value>A value indicating whether to preserve the distinction
    /// between positive zero and negative zero when decoding JSON. The
    /// default is true.</value>
    public bool PreserveNegativeZero {
      get;
      private set;
    }

    /// <summary>Gets a value indicating how JSON numbers are decoded to
    /// CBOR.</summary>
    /// <value>A value indicating how JSON numbers are decoded to CBOR. The
    /// default is <c>ConversionMode.Full</c>.</value>
    public ConversionMode NumberConversion {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to allow duplicate keys
    /// when reading JSON. Used only when decoding JSON. If this property
    /// is <c>true</c> and a JSON object has two or more values with the
    /// same key, the last value of that key set forth in the JSON object
    /// is taken.</summary>
    /// <value>A value indicating whether to allow duplicate keys when
    /// reading JSON. The default is false.</value>
    public bool AllowDuplicateKeys {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether surrogate code points not
    /// part of a surrogate pair (which consists of two consecutive
    /// <c>char</c> s forming one Unicode code point) are each replaced
    /// with a replacement character (U+FFFD). If false, an exception is
    /// thrown when such code points are encountered.</summary>
    /// <value>True, if surrogate code points not part of a surrogate pair
    /// are each replaced with a replacement character, or false if an
    /// exception is thrown when such code points are encountered. The
    /// default is false.</value>
    public bool ReplaceSurrogates {
      get;
      private set;
    }
  }
}
