/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;

namespace PeterO.Cbor {
  /// <summary>Represents a type that a CBOR object can have.</summary>
  public enum CBORType
  {
    /// <summary>This property is no longer used.</summary>
    [Obsolete("Since version 4.0, CBORObject.Type no longer returns this" +
"\u0020value for any CBOR object - this is a breaking change from " +
"earlier versions." +
"\u0020Instead, use the IsNumber property of CBORObject to determine" +
" whether a CBOR object represents a number, or use the two " +
"new CBORType values instead. CBORType.Integer " +
"covers CBOR objects representing" +
"\u0020integers of" +
"\u0020major type 0 and 1. " +
"CBORType.FloatingPoint covers CBOR objects representing " +
"16-, 32-, and 64-bit floating-point numbers. CBORType.Number " +
"may be removed in version 5.0 or later.")]
    Number,

    /// <summary>The simple values true and false.</summary>
    Boolean,

    /// <summary>A "simple value" other than floating point values, true,
    /// and false.</summary>
    SimpleValue,

    /// <summary>An array of bytes.</summary>
    ByteString,

    /// <summary>A text string.</summary>
    TextString,

    /// <summary>An array of CBOR objects.</summary>
    Array,

    /// <summary>A map of CBOR objects.</summary>
    Map,

    /// <summary>An integer in the interval [-(2^64), 2^64 - 1], or an
    /// integer of major type 0 and 1.</summary>
    Integer,

    /// <summary>A 16-, 32-, or 64-bit binary floating-point
    /// number.</summary>
    FloatingPoint,
  }
}
