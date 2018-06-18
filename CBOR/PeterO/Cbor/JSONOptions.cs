using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor
{
    /// <include file='../../docs.xml'
  /// path='docs/doc[@name="T:PeterO.Cbor.JSONOptions"]/*'/>
    public sealed class JSONOptions
    {

    public JSONOptions() : this(true) {}

    public JSONOptions(bool base64Padding) { 
        this.base64Padding=base64Padding;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.JSONOptions.Default"]/*'/>
    public readonly static JSONOptions Default = new JSONOptions();

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.JSONOptions.Base64Padding"]/*'/>
    public bool Base64Padding { get; }
   }
}
