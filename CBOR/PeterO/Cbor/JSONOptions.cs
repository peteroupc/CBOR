using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
   ///
  /// <seealso cref='M:PeterO.Cbor.CBORObject.ToJSONString'/><summary>Includes options to control how CBOR objects are converted to JSON.
  /// </summary>
  ///
    public sealed class JSONOptions {
   ///
  /// <summary>Initializes a new instance of the <see cref='T:PeterO.Cbor.JSONOptions'/> class with default options.
  /// </summary>
  ///
    public JSONOptions() : this(false) {
}

   ///
  /// <summary>Initializes a new instance of the <see cref='T:PeterO.Cbor.JSONOptions'/> class with the given value for the Base64Padding option.
  /// </summary><param name='base64Padding'>Whether padding is included when writing data in base64url or traditional
  /// base64 format to JSON.
  /// </param>
  ///
    public JSONOptions(bool base64Padding) : this(base64Padding, false) {
    }

   /// <summary>
  /// Initializes a new instance of the
  /// <see cref='JSONOptions'/>
  /// class with the given values for the options.
  /// </summary>
  /// <param name='base64Padding'>
  /// Whether padding is included when writing data in base64url or traditional
  /// base64 format to JSON.
  /// </param>
  /// <param name='replaceSurrogates'>
  /// Whether surrogate code points not part of a surrogate pair (which consists
  /// of two consecutive
  /// <c>
  /// char
  /// </c>
  /// s forming one Unicode code point) are each replaced with a replacement
  /// character (U+FFFD). The default is false; an exception is thrown when such
  /// code points are encountered.
  /// </param>
    public JSONOptions(bool base64Padding, bool replaceSurrogates) {
        this.Base64Padding = base64Padding;
        this.ReplaceSurrogates = replaceSurrogates;
    }

   ///
  /// <summary>The default options for converting CBOR objects to JSON.
  /// </summary>
  ///
    public static readonly JSONOptions Default = new JSONOptions();

   ///
  /// <summary>Gets a value indicating whether padding is written out when writing
  /// base64url or traditional base64 to JSON.
  /// </summary><value>The default is false, no padding.
  /// </value><remarks>
  /// The padding character is '='.
  /// </remarks>
  ///
    [Obsolete("This option may have no effect in the future. A future version may, by default, include necessary padding when writing traditional base64 to JSON and include no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.")]
    public bool Base64Padding { get; private set; }

   ///
  /// <summary>Gets a value indicating whether surrogate code points not part of a
  /// surrogate pair (which consists of two consecutive
  /// <c>char</c> s forming one Unicode code point) are each replaced with a replacement
  /// character (U+FFFD). The default is false; an exception is thrown when such
  /// code points are encountered.
  /// </summary><value>True, if surrogate code points not part of a surrogate pair are each
  /// replaced with a replacement character, or false if an exception is thrown
  /// when such code points are encountered.
  /// </value>
  ///
    public bool ReplaceSurrogates { get; private set; }
   }
}
