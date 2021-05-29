/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;

namespace PeterO.Cbor {
  internal class CBORUriConverter : ICBORToFromConverter<Uri> {
    private static CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("URI/IRI must be a text string");
      }
      bool isiri = obj.HasMostOuterTag(266);
      bool isiriref = obj.HasMostOuterTag(267);
      if (
        isiriref && !URIUtility.IsValidIRI(
          obj.AsString(),
          URIUtility.ParseMode.IRIStrict)) {
        throw new CBORException("String is not a valid IRI Reference");
      }
      if (
        isiri && (!URIUtility.IsValidIRI(
            obj.AsString(),
            URIUtility.ParseMode.IRIStrict) ||
          !URIUtility.HasScheme(obj.AsString()))) {
        throw new CBORException("String is not a valid IRI");
      }
      if (!URIUtility.IsValidIRI(
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
        ValidateObject(obj);
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
