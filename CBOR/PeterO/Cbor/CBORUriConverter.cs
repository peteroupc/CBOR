/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORUriConverter : ICBORToFromConverter<Uri>
  {
    private CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("URI/IRI must be a text string");
      }
      int isuri = obj.HasMostOuterTag(32);
      ParseMode pm = isuri ? ParseMode.URIStrict : ParseMode.IRIStrict;
      if (!URIUtility.isValidIRI(obj.AsString(), pm)) {
        if (uri) {
 throw new CBORException("String is not a valid URI");
} else {
 throw new CBORException("String is not a valid IRI");
}
      }
      return obj;
    }

    public Uri FromCBORObject(CBORObject obj) {
      if (obj.HasMostOuterTag(32) || obj.HasMostOuterTag(266)) {
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
      bool nonascii = false;
      for (var i = 0;i<uriString.Length; ++i) {
        if (uriString[i]>= 0x80) {
 nonascii = true;
}
      }
      int tag=(nonascii) ? 266 : 32;
      return CBORObject.FromObjectAndTag(uriString, (int)tag);
    }
  }
}
