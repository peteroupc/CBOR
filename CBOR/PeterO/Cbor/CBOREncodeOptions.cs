using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBOREncodeOptions"]/*'/>
  public sealed class CBOREncodeOptions {
    /// <summary>No special options for encoding/decoding. Value:
    /// 0.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,true)' instead. Option classes" +
" in this library will follow a different form in a later" +
" version -- the approach used in this class is too complicated. " +
"'CBOREncodeOptions.Default' contains recommended default options that" +
" may be adopted by certain CBORObject methods in the next major " +
"version.")]
    public static readonly CBOREncodeOptions None =
 new CBOREncodeOptions(0);

    /// <summary>Default options for CBOR objects. Disallow duplicate keys,
    /// and always encode strings using definite-length encoding. These are
    /// recommended settings for the options that may be adopted by certain
    /// CBORObject methods in the next major version.</summary>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

    /// <summary>Always encode strings with a definite-length encoding.
    /// Used only when encoding CBOR objects. Value: 1.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(false,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
public static readonly CBOREncodeOptions NoIndefLengthStrings =
new CBOREncodeOptions(1);

    /// <summary>Disallow duplicate keys when reading CBOR objects from a
    /// data stream. Used only when decoding CBOR objects. Value:
    /// 2.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
public static readonly CBOREncodeOptions NoDuplicateKeys =
 new CBOREncodeOptions(2);

    private readonly int value;

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    public CBOREncodeOptions() : this(false, false) {
}

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    /// <param name='useIndefLengthStrings'>A value indicating whether to
    /// always encode strings with a definite-length encoding.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// disallow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys)
        : this(useIndefLengthStrings, allowDuplicateKeys, false) {
    }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    /// <param name='useIndefLengthStrings'>A value indicating whether to
    /// always encode strings with a definite-length encoding.</param>
    /// <param name='allowDuplicateKeys'>A value indicating whether to
    /// disallow duplicate keys when reading CBOR objects from a data
    /// stream.</param>
    /// <param name='ctap2Canonical'>Either <c>true</c> or <c>false</c>.</param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys,
      bool ctap2Canonical) {
      var val = 0;
      if (!useIndefLengthStrings) {
        val |= 1;
      }
      if (!allowDuplicateKeys) {
        val |= 2;
      }
      this.value = val;
      this.Ctap2Canonical = ctap2Canonical;
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.UseIndefLengthStrings"]/*'/>
    public bool UseIndefLengthStrings {
      get {
        return (this.value & 1) == 0;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.AllowDuplicateKeys"]/*'/>
    public bool AllowDuplicateKeys {
      get {
        return (this.value & 2) == 0;
      }
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Ctap2Canonical"]/*'/>
    public bool Ctap2Canonical { get; private set; }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Cbor.CBOREncodeOptions.Value"]/*'/>
    [Obsolete("Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public int Value { get {
  return this.value;
 } }

    private CBOREncodeOptions(int value)
    : this((value & 1) == 0, (value & 2) == 0) {
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.Or(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    [Obsolete("May be removed in a later version. Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      return new CBOREncodeOptions(this.value | o.value);
    }

    /// <include file='../../docs.xml'
    ///   path='docs/doc[@name="M:PeterO.Cbor.CBOREncodeOptions.And(PeterO.Cbor.CBOREncodeOptions)"]/*'/>
    [Obsolete("May be removed in a later version. Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
public CBOREncodeOptions And(CBOREncodeOptions o) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      return new CBOREncodeOptions(this.value & o.value);
    }
  }
}
