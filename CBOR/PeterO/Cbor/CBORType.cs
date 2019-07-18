/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
    /// <summary>Represents a type that a CBOR object can have.</summary>
  public enum CBORType {
    /// <summary>A number of any kind, including integers, big integers,
    /// floating point numbers, and decimal numbers. The floating-point
    /// value Not-a-Number is also included in the Number type.</summary>
    [Obsolete("Use the IsNumber property of CBORObject to determine" +
       " whether a CBOR object represents a number, or use the two " +
       "new CBORType values instead.  CBORType.Integer " +
       "covers CBOR objects representing integers of major type 0 and 1. " +
       "CBORType.FloatingPoint covers CBOR objects representing " +
       "16-, 32-, and 64-bit floating-point numbers.")]
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

    /// <summary>An integer in the interval [-2^64, 2^64 - 1], or an
    /// integer of major type 0 and 1.</summary>
    Integer,

    /// <summary>A 16-, 32-, or 64-bit binary floating-point
    /// number.</summary>
    FloatingPoint,
  }
}
