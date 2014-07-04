package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

  class CBORTag32 implements ICBORTag, ICBORConverter<java.net.URI> {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() != CBORType.TextString) {
        throw new CBORException("URI must be a text String");
      }
      if (!URIUtility.isValidIRI(obj.AsString())) {
        throw new CBORException("String is not a valid URI/IRI");
      }
      return obj;
    }

    static void AddConverter() {
      CBORObject.AddConverter(java.net.URI.class, new CBORTag32());
    }

    public CBORObject ToCBORObject(java.net.URI uri) {
      if (uri == null) {
        throw new NullPointerException("uri");
      }
      return CBORObject.FromObjectAndTag(uri.toString(), (int)32);
    }
  }
