/*
Written by Peter O. in 2013.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBORType"]/*'/>
  public enum CBORType {
    /// <summary>A number of any kind, including integers, big integers,
    /// floating point numbers, and decimal numbers. The floating-point
    /// value Not-a-Number is also included in the Number type.</summary>
    [Obsolete("Use the IsNumber property of CBORObject to determine" +
       " whether a CBOR object represents a number.")]
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
  }
}
