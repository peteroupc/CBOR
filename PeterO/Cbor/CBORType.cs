/*
Written in 2013 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Cbor {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Cbor.CBORType"]/*'/>
  public enum CBORType {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.Number"]/*'/>
    Number,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.Boolean"]/*'/>
    Boolean,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.SimpleValue"]/*'/>
    SimpleValue,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.ByteString"]/*'/>
    ByteString,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.TextString"]/*'/>
    TextString,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.Array"]/*'/>
    Array,

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="F:PeterO.Cbor.CBORType.Map"]/*'/>
    Map
  }
}
