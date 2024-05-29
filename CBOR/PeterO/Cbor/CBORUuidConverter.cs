/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
https://creativecommons.org/publicdomain/zero/1.0/

 */
using System;

namespace PeterO.Cbor {
  internal class CBORUuidConverter : ICBORToFromConverter<Guid>
  {
    private static CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.ByteString) {
        throw new CBORException("UUID must be a byte string");
      }
      byte[] bytes = obj.GetByteString();
      return bytes.Length != 16 ? throw new CBORException("UUID must be 16" +
"\u0020bytes long") : obj;
    }

    /// <summary>Internal API.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is an
    /// internal parameter.</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ToCBORObject(Guid obj) {
      byte[] bytes = PropertyMap.UUIDToBytes(obj);
      return CBORObject.FromByteArray(bytes).WithTag(37);
    }

    public Guid FromCBORObject(CBORObject obj) {
      if (!obj.HasMostOuterTag(37)) {
        throw new CBORException("Must have outermost tag 37");
      }
      _ = ValidateObject(obj);
      byte[] b2 = obj.GetByteString();
      byte[] bytes = {
        b2[3], b2[2], b2[1], b2[0], b2[5], b2[4], b2[7],
        b2[6], b2[8], b2[9], b2[10], b2[11], b2[12], b2[13], b2[14], b2[15],
      };
      return new Guid(bytes);
    }
  }
}
