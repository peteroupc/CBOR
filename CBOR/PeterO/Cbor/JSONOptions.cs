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

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.JSONOptions.#ctor(System.Boolean)"]/*'/>
    public JSONOptions(bool base64Padding) {
        this.Base64Padding = base64Padding;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.JSONOptions.Default"]/*'/>
    public readonly static JSONOptions Default = new JSONOptions();

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>The default is. <b>false</b>, no padding.</value>
    /// <remarks>The padding character is '='.</remarks>
    public bool Base64Padding { get; private set; }
   }
}
