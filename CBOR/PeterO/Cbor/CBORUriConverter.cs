/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORUriConverter : ICBORToFromConverter<Uri> {
    private CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("URI/IRI must be a text string");
      }
      bool isiri = obj.HasMostOuterTag(266);
      bool isiriref = obj.HasMostOuterTag(267);
      if (
        isiriref && !URIUtility.IsValidIRI (
          obj.AsString(),
          URIUtility.ParseMode.IRIStrict)) {
        throw new CBORException("String is not a valid IRI Reference");
      }
      if (
        isiri && (!URIUtility.IsValidIRI (
            obj.AsString(),
            URIUtility.ParseMode.IRIStrict) ||
          !URIUtility.HasScheme(obj.AsString()))) {
        throw new CBORException("String is not a valid IRI");
      }
      if (!URIUtility.IsValidIRI (
          obj.AsString(),
          URIUtility.ParseMode.URIStrict) ||
        !URIUtility.HasScheme(obj.AsString())) {
        throw new CBORException("String is not a valid URI");
      }
      return obj;
    }

    public Uri FromCBORObject(CBORObject obj) {
      if (obj.HasMostOuterTag(32) ||
             obj.HasMostOuterTag(266) ||
             obj.HasMostOuterTag(267)) {
        this.ValidateObject(obj);
        try {
          return new Uri(obj.AsString());
        } catch (Exception ex) {
          throw new CBORException(ex.Message, ex);
        }
      }
      throw new CBORException();
    }

    public CBORObject ToCBORObject(Uri uri) {
      if (uri == null) {
        throw new ArgumentNullException(nameof(uri));
      }
      string uriString = uri.ToString();
      var nonascii = false;
      for (var i = 0; i < uriString.Length; ++i) {
        nonascii |= uriString[i] >= 0x80;
      }
      int tag = nonascii ? 266 : 32;
      if (!URIUtility.HasScheme(uriString)) {
        tag = 267;
      }
      return CBORObject.FromObjectAndTag(uriString, (int)tag);
    }
  }
}
