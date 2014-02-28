/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Cbor
{
    /// <summary>Description of CBORTag2.</summary>
  public class CBORTag2 : ICBORTag
  {
    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORTypeFilter object.</returns>
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
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
          x =-1L - x;
        }
        return CBORObject.FromObject(x);
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
