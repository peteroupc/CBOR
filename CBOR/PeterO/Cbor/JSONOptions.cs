using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor
{
    /// <summary>
    ///   CBOR to JSON conversion options.
    /// </summary>
    /// <seealso cref="CBORObject.ToJSONString()"/>
    public sealed class JSONOptions
    {
    /// <summary>
    ///   The default options for converting to JSON.
    /// </summary>
    public readonly static JSONOptions Default = new JSONOptions();

        /// <summary>
        ///   Pad any base-64 encoding.
        /// </summary>
        /// <value>
        ///   The default is <b>false</b>, no padding.
        /// </value>
        /// <remarks>
        ///   The padding character is '='.
        /// </remarks>
        public bool Base64Padding { get; set; }
    }
}
