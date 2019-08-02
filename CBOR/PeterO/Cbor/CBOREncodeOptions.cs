using System;

namespace PeterO.Cbor {
    /// <summary>Specifies options for encoding and decoding CBOR objects.</summary>
  public sealed class CBOREncodeOptions {
    /// <summary>No special options for encoding/decoding. Value: 0.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,true)' instead. Option classes" +
" in this library will follow a different form in a later" +
" version -- the approach used in this class is too complicated. " +
"'CBOREncodeOptions.Default' contains recommended default options that" +
" may be adopted by certain CBORObject methods in the next major " +
"version.")]
    public static readonly CBOREncodeOptions None =
 new CBOREncodeOptions(0);

    /// <summary>Default options for CBOR objects. Disallow duplicate keys, and always
    /// encode strings using definite-length encoding. These are recommended
    /// settings for the options that may be adopted by certain CBORObject methods
    /// in the next major version.</summary>
    public static readonly CBOREncodeOptions Default =
      new CBOREncodeOptions(false, false);

    /// <summary>Always encode strings with a definite-length encoding. Used only when
    /// encoding CBOR objects. Value: 1.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(false,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
public static readonly CBOREncodeOptions NoIndefLengthStrings =
new CBOREncodeOptions(1);

    /// <summary>Disallow duplicate keys when reading CBOR objects from a data stream. Used
    /// only when decoding CBOR objects. Value: 2.</summary>
    [Obsolete("Use 'new CBOREncodeOptions(true,false)' instead. Option" +
" classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
public static readonly CBOREncodeOptions NoDuplicateKeys =
 new CBOREncodeOptions(2);

    private readonly int value;

    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary>
    public CBOREncodeOptions() : this(false, false) {
}

    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary><param name='useIndefLengthStrings'>A value indicating whether to always encode strings with a definite-length
    /// encoding.
    /// </param><param name='allowDuplicateKeys'>A value indicating whether to disallow duplicate keys when reading CBOR
    /// objects from a data stream.
    /// </param>
    public CBOREncodeOptions(
      bool useIndefLengthStrings,
      bool allowDuplicateKeys)
        : this(useIndefLengthStrings, allowDuplicateKeys, false) {
    }

    /// <summary>Initializes a new instance of the <see cref='PeterO.Cbor.CBOREncodeOptions'/> class.</summary><param name='useIndefLengthStrings'>A value indicating whether to always encode strings with a definite-length
    /// encoding.
    /// </param><param name='allowDuplicateKeys'>A value indicating whether to disallow duplicate keys when reading CBOR
    /// objects from a data stream.
    /// </param><param name='ctap2Canonical'>Either
    /// <c>true
    /// </c>
    /// or
    /// <c>false
    /// </c>
    /// .
    /// </param>
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

    /// <summary>Gets a value indicating whether to always encode strings with a
    /// definite-length encoding.</summary><value>A value indicating whether to always encode strings with a definite-length
    /// encoding.
    /// </value>
    public bool UseIndefLengthStrings {
      get {
        return (this.value & 1) == 0;
      }
    }

    /// <summary>Gets a value indicating whether to disallow duplicate keys when reading
    /// CBOR objects from a data stream. Used only when decoding CBOR objects.</summary><value>A value indicating whether to disallow duplicate keys when reading CBOR
    /// objects from a data stream.
    /// </value>
    public bool AllowDuplicateKeys {
      get {
        return (this.value & 2) == 0;
      }
    }

    /// <summary>Gets a value indicating whether CBOR objects are written out using the
    /// CTAP2 canonical CBOR encoding form. In this form, CBOR tags are not used,
    /// map keys are written out in a canonical order, and non-integer numbers and
    /// integers 2^63 or greater are written as 64-bit binary floating-point
    /// numbers.</summary><value><c>true
    /// </c>
    /// if CBOR objects are written out using the CTAP2 canonical CBOR encoding
    /// form; otherwise,
    /// <c>false
    /// </c>
    /// .. In this form, CBOR tags are not used, map keys are written out in a
    /// canonical order, and non-integer numbers and integers 2^63 or greater are
    /// written as 64-bit binary floating-point numbers.
    /// </value>
    public bool Ctap2Canonical { get; private set; }

    /// <summary>Gets this options object's value.</summary><value>This options object's value.
    /// </value>
    [Obsolete("Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public int Value { get {
  return this.value;
 } }

    private CBOREncodeOptions(int value)
    : this((value & 1) == 0, (value & 2) == 0) {
    }

    /// <summary>Returns an options object containing the combined flags of
    /// this and another options object.</summary>
    /// <param name='o'>The parameter <paramref name='o'/> is a
    /// CBOREncodeOptions object.</param>
    /// <returns>A new CBOREncodeOptions object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='o'/> is null.</exception>
    [Obsolete("May be removed in a later version. Option classes" +
"\u0020in this library will follow a different form in a later" +
"\u0020version -- the approach used in this class is too complicated.")]
    public CBOREncodeOptions Or(CBOREncodeOptions o) {
      if (o == null) {
        throw new ArgumentNullException(nameof(o));
      }
      return new CBOREncodeOptions(this.value | o.value);
    }

    /// <summary>Returns an options object containing the flags shared by
    /// this and another options object.</summary>
    /// <param name='o'>The parameter <paramref name='o'/> is a
    /// CBOREncodeOptions object.</param>
    /// <returns>A CBOREncodeOptions object.</returns>
    /// <exception cref='System.ArgumentNullException'>The parameter <paramref name='o'/> is null.</exception>
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
