using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.None"]/*'/>
  [Obsolete("Use UseIndefLengthStrings instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions None =
      new CBOREncodeOptions(0);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoIndefLengthStrings"]/*'/>
  [Obsolete("Use UseIndefLengthStrings instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
      new CBOREncodeOptions(1);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoDuplicateKeys"]/*'/>
  [Obsolete("Use UseDuplicateKeys instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoDuplicateKeys =
      new CBOREncodeOptions(2);

    private readonly int value;

    public CBOREncodeOptions() : this(false, false) {}

  public CBOREncodeOptions(bool useIndefLengthStrings, bool
      useDuplicateKeys) {
      var val = 0;
      if (!useIndefLengthStrings) {
 val|=NoIndefLengthStrings.value;
}
      if (!useDuplicateKeys) {
 val|=NoDuplicateKeys.value;
}
      this.value = val;
    }

    public bool UseIndefLengthStrings {
      get {
       CBOREncodeOptions o = this.Annd(NoIndefLengthStrings);
       return o.Value == 0;
      }
    }

    public bool UseDuplicateKeys {
      get {
       CBOREncodeOptions o = this.And(NoDuplicateKeys);
       return o.Value == 0;
      }
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Value"]/*'/>
  [Obsolete("Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
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
  [Obsolete("May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value | o.value);
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.And(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
  [Obsolete("May be removed in a later version. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public CBOREncodeOptions And(CBOREncodeOptions o) {
      return new CBOREncodeOptions(this.value & o.value);
    }
  }
}
