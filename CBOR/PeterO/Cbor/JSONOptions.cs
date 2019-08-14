using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
    /// <summary>Includes options to control how CBOR objects are converted
    /// to JSON.</summary>
  public sealed class JSONOptions {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with default
    /// options.</summary>
    public JSONOptions() : this(false) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with the given value
    /// for the Base64Padding option.</summary>
    /// <param name='base64Padding'>Whether padding is included when
    /// writing data in base64url or traditional base64 format to
    /// JSON.</param>
    public JSONOptions(bool base64Padding) : this(base64Padding, false) {
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
    /// with a replacement character (U + FFFD). The default is false; an
    /// exception is thrown when such code points are encountered.</param>
#pragma warning disable CS0618
    public JSONOptions(bool base64Padding, bool replaceSurrogates) {
      this.Base64Padding = base64Padding;
      this.ReplaceSurrogates = replaceSurrogates;
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
    /// any one of the following in any combination of case:
    /// <c>base64padding</c>, <c>replacesurrogates</c>. Other keys are
    /// ignored. If the same key appears more than once, the value given
    /// for the last such key is used. The two keys just given can have a
    /// value of <c>1</c>, <c>true</c>, <c>yes</c>, or <c>on</c> (in any
    /// combination of case), which means true, and any other value meaning
    /// false. For example, <c>base64padding=Yes</c> and
    /// <c>base64padding=1</c> both set the <c>Base64Padding</c> property
    /// to true.</param>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='paramString'/> is null.</exception>
    public JSONOptions(string paramString) {
      if (paramString == null) {
        throw new ArgumentNullException(nameof(paramString));
      }
      var parser = new OptionsParser(paramString);
      this.Base64Padding = parser.GetBoolean("base64padding", true);
      this.ReplaceSurrogates = parser.GetBoolean("replacesurrogates", true);
    }

    /// <summary>Gets the values of this options object's properties in
    /// text form.</summary>
    /// <returns>A text string containing the values of this options
    /// object's properties. The format of the string is the same as the
    /// one described in the String constructor for this class.</returns>
    public override string ToString() {
      return new StringBuilder()
           .Append("base64padding=")
           .Append(this.Base64Padding ? "true" : "false")
           .Append(";replacesurrogates=")
           .Append(this.ReplaceSurrogates ? "true" : "false")
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
    [Obsolete("This option now has no effect. This library now includes " +
         "necessary padding when writing traditional base64 to JSON and" +
         " includes no padding when writing base64url to JSON, in " +
         "accordance with the revision of the CBOR specification.")]
    public bool Base64Padding { get; private set; }

    /// <summary>Gets a value indicating whether surrogate code points not
    /// part of a surrogate pair (which consists of two consecutive
    /// <c>char</c> s forming one Unicode code point) are each replaced
    /// with a replacement character (U + FFFD). The default is false; an
    /// exception is thrown when such code points are
    /// encountered.</summary>
    /// <value>True, if surrogate code points not part of a surrogate pair
    /// are each replaced with a replacement character, or false if an
    /// exception is thrown when such code points are encountered.</value>
    public bool ReplaceSurrogates { get; private set; }
   }
}
