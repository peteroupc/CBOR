package com.upokecenter.cbor;
/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

    /**
     * Represents a type that a CBOR object can have.
     */
  public enum CBORType {
    /**
     * A number of any kind, including integers, big integers, floating point
     * numbers, and decimal numbers. The floating-point value Not-a-Number
     * is also included in the Number type.
     */
    Number,

    /**
     * The simple values true and false.
     */
    Boolean,

    /**
     * A &quot;simple value&quot; other than floating point values, true, and
     * false.
     */
    SimpleValue,

    /**
     * An array of bytes.
     */
    ByteString,

    /**
     * A text string.
     */
    TextString,

    /**
     * An array of CBOR objects.
     */
    Array,

    /**
     * A map of CBOR objects.
     */
    Map
  }
