using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.JSONOptions"]/*'/>
    public sealed class JSONOptions {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.JSONOptions.#ctor"]/*'/>
    public JSONOptions() : this(false) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='T:PeterO.Cbor.JSONOptions'/> class with the given values
    /// for the options.</summary>
    /// <param name='base64Padding'>Whether padding is included when
    /// writing data in base64url or traditional base64 format to
    /// JSON.</param>
    /// <remarks>NOTE: The base64Padding parameter may have no effect in
    /// the future. A future version may, by default, include necessary
    /// padding when writing traditional base64 to JSON and include no
    /// padding when writing base64url to JSON, in accordance with the
    /// revision of the CBOR specification.</remarks>
    public JSONOptions(bool base64Padding) {
        this.Base64Padding = base64Padding;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.JSONOptions.Default"]/*'/>
    public static readonly JSONOptions Default = new JSONOptions();

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.JSONOptions.Base64Padding"]/*'/>
  [Obsolete("This option may have no effect in the future. A future version may, by default, include necessary padding when writing traditional base64 to JSON and include no padding when writing base64url to JSON, in accordance with the revision of the CBOR specification.")]
    public bool Base64Padding { get; private set; }
   }
}
