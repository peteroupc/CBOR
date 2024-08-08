/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;

namespace PeterO.Cbor {
  internal class CBORUriConverter : ICBORToFromConverter<Uri>
  {
    private static CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("URI/IRI must be a text string");
      }
      bool isiri = obj.HasMostOuterTag(266);
      bool isiriref = obj.HasMostOuterTag(267);
      return isiriref && !URIUtility.IsValidIRI(
          obj.AsString(),
          URIUtility.ParseMode.IRIStrict) ?
        throw new CBORException("String is not a valid IRI Reference") :
        isiri && (!URIUtility.IsValidIRI(
            obj.AsString(),
            URIUtility.ParseMode.IRIStrict) ||
          !URIUtility.HasScheme(obj.AsString())) ?
        throw new CBORException("String is not a valid IRI") :
        !URIUtility.IsValidIRI(
          obj.AsString(),
          URIUtility.ParseMode.URIStrict) ||
        !URIUtility.HasScheme(obj.AsString()) ?
        throw new CBORException("String is not a valid URI") :
        obj;
    }

    public Uri FromCBORObject(CBORObject obj) {
      if (obj.HasMostOuterTag(32) ||
             obj.HasMostOuterTag(266) ||
             obj.HasMostOuterTag(267)) {
        _ = ValidateObject(obj);
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
      for (int i = 0; i < uriString.Length; ++i) {
        nonascii |= uriString[i] >= 0x80;
      }
      int tag = nonascii ? 266 : 32;
      if (!URIUtility.HasScheme(uriString)) {
        tag = 267;
      }
      return CBORObject.FromString(uriString).WithTag(tag);
    }
  }
}
