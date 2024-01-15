using System;
using System.Text;

namespace PeterO.Cbor {
  /// <summary>Includes options to control how CBOR objects are converted
  /// to and from JavaScript Object Notation (JSON).</summary>
  public sealed class JSONOptions {
    /// <summary>Specifies how JSON numbers are converted to CBOR objects
    /// when decoding JSON (such as via <c>FromJSONString</c> or
    /// <c>ReadJSON</c> ). None of these conversion modes affects how CBOR
    /// objects are later encoded (such as via <c>EncodeToBytes</c>
    /// ).</summary>
    public enum ConversionMode
    {
      /// <summary>JSON numbers are decoded to CBOR using the full precision
      /// given in the JSON text. The number will be converted to a CBOR
      /// object as follows: If the number's exponent is 0 (after shifting
      /// the decimal point to the end of the number without changing its
      /// value), use the rules given in the
      /// <c>CBORObject.FromObject(EInteger)</c> method; otherwise, use the
      /// rules given in the <c>CBORObject.FromObject(EDecimal)</c> method.
      /// An exception in version 4.x involves negative zeros; if the
      /// negative zero's exponent is 0, it's written as a CBOR
      /// floating-point number; otherwise the negative zero is written as an
      /// EDecimal.</summary>
      Full,

      /// <summary>JSON numbers are decoded to CBOR as their closest-rounded
      /// approximation as 64-bit binary floating-point numbers (using the
      /// round-to-nearest/ties-to-even rounding mode). (In some cases,
      /// numbers extremely close to zero may underflow to positive or
      /// negative zero, and numbers of extremely large absolute value may
      /// overflow to infinity.). It's important to note that this mode
      /// affects only how JSON numbers are
      /// <i>decoded</i> to a CBOR object; it doesn't affect how
      /// <c>EncodeToBytes</c> and other methods encode CBOR objects.
      /// Notably, by default, <c>EncodeToBytes</c> encodes CBOR
      /// floating-point values to the CBOR format in their 16-bit
      /// ("half-float"), 32-bit ("single-precision"), or 64-bit
      /// ("double-precision") encoding form depending on the
      /// value.</summary>
      Double,

      /// <summary>A JSON number is decoded to CBOR objects either as a CBOR
      /// integer (major type 0 or 1) if the JSON number represents an
      /// integer at least -(2^53)+1 and less than 2^53, or as their
      /// closest-rounded approximation as 64-bit binary floating-point
      /// numbers (using the round-to-nearest/ties-to-even rounding mode)
      /// otherwise. For example, the JSON number
      /// 0.99999999999999999999999999999999999 is not an integer, so it's
      /// converted to its closest 64-bit binary floating-point
      /// approximation, namely 1.0. (In some cases, numbers extremely close
      /// to zero may underflow to positive or negative zero, and numbers of
      /// extremely large absolute value may overflow to infinity.). It's
      /// important to note that this mode affects only how JSON numbers are
      /// <i>decoded</i> to a CBOR object; it doesn't affect how
      /// <c>EncodeToBytes</c> and other methods encode CBOR objects.
      /// Notably, by default, <c>EncodeToBytes</c> encodes CBOR
      /// floating-point values to the CBOR format in their 16-bit
      /// ("half-float"), 32-bit ("single-precision"), or 64-bit
      /// ("double-precision") encoding form depending on the
      /// value.</summary>
      IntOrFloat,

      /// <summary>A JSON number is decoded to CBOR objects either as a CBOR
      /// integer (major type 0 or 1) if the number's closest-rounded
      /// approximation as a 64-bit binary floating-point number (using the
      /// round-to-nearest/ties-to-even rounding mode) represents an integer
      /// at least -(2^53)+1 and less than 2^53, or as that approximation
      /// otherwise. For example, the JSON number
      /// 0.99999999999999999999999999999999999 is the integer 1 when rounded
      /// to its closest 64-bit binary floating-point approximation (1.0), so
      /// it's converted to the CBOR integer 1 (major type 0). (In some
      /// cases, numbers extremely close to zero may underflow to zero, and
      /// numbers of extremely large absolute value may overflow to
      /// infinity.). It's important to note that this mode affects only how
      /// JSON numbers are
      /// <i>decoded</i> to a CBOR object; it doesn't affect how
      /// <c>EncodeToBytes</c> and other methods encode CBOR objects.
      /// Notably, by default, <c>EncodeToBytes</c> encodes CBOR
      /// floating-point values to the CBOR format in their 16-bit
      /// ("half-float"), 32-bit ("single-precision"), or 64-bit
      /// ("double-precision") encoding form depending on the
      /// value.</summary>
      IntOrFloatFromDouble,

      /// <summary>JSON numbers are decoded to CBOR as their closest-rounded
      /// approximation to an IEEE 854 decimal128 value, using the
      /// round-to-nearest/ties-to-even rounding mode and the rules for the
      /// EDecimal form of that approximation as given in the
      /// <c>CBORObject.FromObject(EDecimal)</c> method. (In some cases,
      /// numbers extremely close to zero may underflow to zero, and numbers
      /// of extremely large absolute value may overflow to
      /// infinity.).</summary>
      Decimal128,
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with default
    /// options.</summary>
    public JSONOptions() : this(String.Empty) {
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
    /// <c>writebasic=false;replacesurrogates=true</c>. The key can be any
    /// one of the following where the letters can be any combination of
    /// basic upper-case and/or basic lower-case letters:
    /// <c>replacesurrogates</c>, <c>allowduplicatekeys</c>,
    /// <c>preservenegativezero</c>, <c>numberconversion</c>,
    /// <c>writebasic</c>, <c>keepkeyorder</c>. Other keys are ignored in
    /// this version of the CBOR library. (Keys are compared using a basic
    /// case-insensitive comparison, in which two strings are equal if they
    /// match after converting the basic upper-case letters A to Z (U+0041
    /// to U+005A) in both strings to basic lower-case letters.) If two or
    /// more key/value pairs have equal keys (in a basic case-insensitive
    /// comparison), the value given for the last such key is used. The
    /// first four keys just given can have a value of <c>1</c>,
    /// <c>true</c>, <c>yes</c>, or <c>on</c> (where the letters can be
    /// any combination of basic upper-case and/or basic lower-case
    /// letters), which means true, and any other value meaning false. The
    /// last key, <c>numberconversion</c>, can have a value of any name
    /// given in the <c>JSONOptions.ConversionMode</c> enumeration (where
    /// the letters can be any combination of basic upper-case and/or basic
    /// lower-case letters), and any other value is unrecognized. (If the
    /// <c>numberconversion</c> key is not given, its value is treated as
    /// <c>intorfloat</c> (formerly <c>full</c> in versions earlier than
    /// 5.0). If that key is given, but has an unrecognized value, an
    /// exception is thrown.) For example, <c>allowduplicatekeys=Yes</c>
    /// and <c>allowduplicatekeys=1</c> both set the
    /// <c>AllowDuplicateKeys</c> property to true, and
    /// <c>numberconversion=double</c> sets the <c>NumberConversion</c>
    /// property to <c>ConversionMode.Double</c>.</param>
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
      this.KeepKeyOrder = parser.GetBoolean(
        "keepkeyorder",
        false);
      this.ReplaceSurrogates = parser.GetBoolean(
        "replacesurrogates",
        false);
      this.NumberConversion = ToNumberConversion(parser.GetLCString(
        "numberconversion",
        null));
      this.WriteBasic = parser.GetBoolean(
        "writebasic",
        false);
    }

    /// <summary>Gets the values of this options object's properties in
    /// text form.</summary>
    /// <returns>A text string containing the values of this options
    /// object's properties. The format of the string is the same as the
    /// one described in the String constructor for this class.</returns>
    public override string ToString() {
      return new StringBuilder()
        .Append("replacesurrogates=")
        .Append(this.ReplaceSurrogates ? "true" : "false")
        .Append(";preservenegativezero=")
        .Append(this.PreserveNegativeZero ? "true" : "false")
        .Append(";keepkeyorder=").Append(this.KeepKeyOrder ? "true" : "false")
        .Append(";numberconversion=").Append(this.FromNumberConversion())
        .Append(";allowduplicatekeys=")
        .Append(this.AllowDuplicateKeys ? "true" : "false")
        .Append(";writebasic=").Append(this.WriteBasic ? "true" : "false")
        .ToString();
    }

    /// <summary>The default options for converting CBOR objects to
    /// JSON.</summary>
    public static readonly JSONOptions Default = new JSONOptions();

    private string FromNumberConversion() {
      ConversionMode kind = this.NumberConversion;
      return kind == ConversionMode.Full ? "full" :
        kind == ConversionMode.Double ? "double" :
        kind == ConversionMode.Decimal128 ? "decimal128" :
        kind == ConversionMode.IntOrFloat ? "intorfloat" :
        (kind == ConversionMode.IntOrFloatFromDouble) ?
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
        return ConversionMode.IntOrFloat;
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
    public bool PreserveNegativeZero
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating how JSON numbers are decoded to
    /// CBOR objects. None of the conversion modes affects how CBOR objects
    /// are later encoded (such as via <c>EncodeToBytes</c> ).</summary>
    /// <value>A value indicating how JSON numbers are decoded to CBOR. The
    /// default is <c>ConversionMode.Full</c>.</value>
    public ConversionMode NumberConversion
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether JSON is written using only
    /// code points from the Basic Latin block (U+0000 to U+007F), also
    /// known as ASCII.</summary>
    /// <value>A value indicating whether JSON is written using only code
    /// points from the Basic Latin block (U+0000 to U+007F), also known as
    /// ASCII. Default is false.</value>
    public bool WriteBasic
    {
      get;
      private set;
    }

    /// <summary>Gets a value indicating whether to preserve the order in
    /// which a map's keys appear when decoding JSON, by using maps created
    /// as though by CBORObject.NewOrderedMap. If false, key order is not
    /// guaranteed to be preserved when decoding JSON.</summary>
    /// <value>A value indicating whether to preserve the order in which a
    /// CBOR map's keys appear when decoding JSON. The default is
    /// false.</value>
    public bool KeepKeyOrder
    {
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
    public bool AllowDuplicateKeys
    {
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
    public bool ReplaceSurrogates
    {
      get;
      private set;
    }
  }
}
