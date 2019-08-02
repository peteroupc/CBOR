using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
    /// <seealso cref='M:PeterO.Cbor.CBORObject.ToJSONString'/><summary>Includes options to control how CBOR objects are converted to JSON.</summary>
    public sealed class JSONOptions {
    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.JSONOptions'/> class with default options.</summary>
    public JSONOptions() : this(false) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.JSONOptions'/> class with the given values
    /// for the options.</summary>
    /// <param name='base64Padding'>Whether padding is included when
    /// writing data in base64url or traditional base64 format to
    /// JSON.</param>
    /// <remarks>NOTE: The base64Padding parameter may have no effect in
    /// the future. A future version may, by default, include necessary
    /// padding when writing traditional base64 to JSON and include no
    /// padding when writing base64url to JSON, in accordance with the
    /// revision of the CBOR specification.</remarks>
#pragma warning disable CS0618
    public JSONOptions(bool base64Padding) {
        this.Base64Padding = base64Padding;
    }
#pragma warning restore CS0618

    /// <summary>The default options for converting CBOR objects to JSON.</summary>
    public static readonly JSONOptions Default = new JSONOptions();

    /// <summary>Gets a value indicating whether padding is written out when writing
    /// base64url or traditional base64 to JSON.</summary><value>The default is false, no padding.
    /// </value><remarks>
    /// The padding character is '='.
    /// </remarks>
    [Obsolete("This option may have no effect in the future. A future version" +
" may, by default, include necessary padding when writing traditional" +
" base64" +
" to JSON and include no padding when writing base64url to JSON, in" +
" accordance" +
" with the revision of the CBOR specification.")]
    public bool Base64Padding { get; private set; }
  }
}
