/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PeterO;

namespace PeterO.Cbor
{
    /// <summary>Description of CBORTag2.</summary>
  internal class CBORTag2 : ICBORTag
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    internal static CBORObject FromObjectAndInnerTags(object objectValue, CBORObject objectWithTags) {
      CBORObject newObject = CBORObject.FromObject(objectValue);
      if (!objectWithTags.IsTagged) {
        return newObject;
      }
      objectWithTags = objectWithTags.UntagOne();
      if (!objectWithTags.IsTagged) {
        return newObject;
      }
      BigInteger[] tags = objectWithTags.GetTags();
      for (int i = tags.Length - 1; i >= 0; --i) {
        newObject = CBORObject.FromObjectAndTag(newObject, tags[i]);
      }
      return newObject;
    }

    internal static CBORObject ConvertToBigNum(CBORObject o, bool negative) {
      if (o.Type != CBORType.ByteString) {
        throw new CBORException("Byte array expected");
      }
      byte[] data = o.GetByteString();
      if (data.Length <= 7) {
        long x = 0;
        for (int i = 0; i < data.Length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xFF;
        }
        if (negative) {
          x = -x;
          x -= 1L;
        }
        return FromObjectAndInnerTags(x, o);
      }
      int neededLength = data.Length;
      byte[] bytes;
      bool extended = false;
      if (((data[0] >> 7) & 1) != 0) {
        // Increase the needed length
        // if the highest bit is set, to
        // distinguish negative and positive
        // values
        ++neededLength;
        extended = true;
      }
      bytes = new byte[neededLength];
      for (int i = 0; i < data.Length; ++i) {
        bytes[i] = data[data.Length - 1 - i];
        if (negative) {
          bytes[i] = (byte)((~((int)bytes[i])) & 0xFF);
        }
      }
      if (extended) {
        if (negative) {
          bytes[bytes.Length - 1] = (byte)0xFF;
        } else {
          bytes[bytes.Length - 1] = 0;
        }
      }
      BigInteger bi = new BigInteger((byte[])bytes);
      // NOTE: Here, any tags are discarded; when called from
      // the Read method, "o" will have no tags anyway (beyond tag 2),
      // and when called from FromObjectAndTag, we prefer
      // flexibility over throwing an error if the input
      // object contains other tags. The tag 2 is also discarded
      // because we are returning a "natively" supported CBOR object.
      return CBORObject.FromObject(bi);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='obj'>A CBORObject object. (2).</param>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToBigNum(obj, false);
    }
  }
}
