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
    public JSONOptions(bool base64Padding) : this(base64Padding, false) {
    }

    /// <summary>Initializes a new instance of the JSONOptions
    /// class.</summary>
    /// <param name='base64Padding'>A Boolean object.</param>
    /// <param name='replaceSurrogates'>Another Boolean object.</param>
    public JSONOptions(bool base64Padding, bool replaceSurrogates) {
        this.Base64Padding = base64Padding;
        this.ReplaceSurrogates = replaceSurrogates;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.JSONOptions.Default"]/*'/>
    public static readonly JSONOptions Default = new JSONOptions();

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.JSONOptions.Base64Padding"]/*'/>
    public bool Base64Padding { get; private set; }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public bool ReplaceSurrogates { get; private set; }
   }
}
