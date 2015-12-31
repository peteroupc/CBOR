/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORTag37 : ICBORTag, ICBORConverter<Guid>
  {
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.ByteString) {
        throw new CBORException("UUID must be a byte string");
      }
      byte[] bytes = obj.GetByteString();
      if (bytes.Length != 16) {
        throw new CBORException("UUID must be 16 bytes long");
      }
      return obj;
    }

    internal static void AddConverter() {
      CBORObject.AddConverter(typeof(Guid), new CBORTag37());
    }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Cbor.CBORTag37.ToCBORObject(System.Guid)"]/*'/>
    public CBORObject ToCBORObject(Guid obj) {
      byte[] bytes = obj.ToByteArray();
      var bytes2 = new byte[16];
      Array.Copy(bytes, bytes2, 16);
      // Swap the bytes to conform with the UUID RFC
      bytes2[0] = bytes[3];
      bytes2[1] = bytes[2];
      bytes2[2] = bytes[1];
      bytes2[3] = bytes[0];
      bytes2[4] = bytes[5];
      bytes2[5] = bytes[4];
      bytes2[6] = bytes[7];
      bytes2[7] = bytes[6];
      return CBORObject.FromObjectAndTag(bytes2, (int)37);
    }
  }
}
