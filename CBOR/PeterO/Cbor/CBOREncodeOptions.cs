using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.None"]/*'/>
    public static readonly CBOREncodeOptions None =
      new CBOREncodeOptions(0);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoIndefLengthStrings"]/*'/>
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
      new CBOREncodeOptions(1);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoDuplicateKeys"]/*'/>
    public static readonly CBOREncodeOptions NoDuplicateKeys =
      new CBOREncodeOptions(2);

    private readonly int value;

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Value"]/*'/>
    public int Value {
      get {
        return this.value;
      }
    }

    private CBOREncodeOptions(int value) {
      this.value = value;
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.Or(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value | o.value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.And(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    public CBOREncodeOptions And(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value & o.value);
    }
  }
}
