/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

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
      byte[] bytes = obj.GetByteString();
      var guidChars = new char[36];
      string hex = "0123456789abcdef";
      var index = 0;
      for (int i = 0; i < 16; ++i) {
        if (i == 4 || i == 6 || i == 8 || i == 10) {
          guidChars[index++] = '-';
        }
        guidChars[index++] = hex[(bytes[i] >> 4) & 15];
        guidChars[index++] = hex[bytes[i] & 15];
      }
      var guidString = new String(guidChars);
      // NOTE: Don't use the byte[] constructor of the DotNet Guid class,
      // since the bytes may have to be rearranged in order to generate a
      // Guid from a UUID; thus the exact Guid generated from a byte string
      // by that constructor may differ between little-endian and big-endian
      // computers, but I don't have access to a big-endian machine to test
      // this hypothesis.
      return new Guid(guidString);
    }
  }
}
