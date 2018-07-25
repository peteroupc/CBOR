using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.None"]/*'/>
  [Obsolete("Use 'new CBOREncodeOptions(true,true)' instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions None =
      new CBOREncodeOptions(0);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoIndefLengthStrings"]/*'/>
  [Obsolete("Use  'new CBOREncodeOptions(false,true)' instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoIndefLengthStrings =
      new CBOREncodeOptions(1);

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBOREncodeOptions.NoDuplicateKeys"]/*'/>
  [Obsolete("Use  'new CBOREncodeOptions(true,false)' instead. Option classes in this library will follow the form seen in JSONOptions in a later version; the approach used in this class is too complicated.")]
    public static readonly CBOREncodeOptions NoDuplicateKeys =
      new CBOREncodeOptions(2);

    private readonly int value;

    /// <summary>Initializes a new instance of the CBOREncodeOptions
    /// class.</summary>
    public CBOREncodeOptions() : this(false, false) {}

    /// <summary>Initializes a new instance of the CBOREncodeOptions
    /// class.</summary>
    /// <param name='useIndefLengthStrings'>A Boolean object.</param>
    /// <param name='useDuplicateKeys'>Another Boolean object.</param>
    public CBOREncodeOptions(bool useIndefLengthStrings, bool
      useDuplicateKeys) {
      var val = 0;
      if (!useIndefLengthStrings) {
 val|=1;
}
      if (!useDuplicateKeys) {
 val|=2;
}
      this.value = val;
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public bool UseIndefLengthStrings {
      get {
        return (this.value & 1) == 0;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public bool UseDuplicateKeys {
      get {
        return (this.value & 2) == 0;
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
