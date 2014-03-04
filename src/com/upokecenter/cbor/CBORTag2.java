package com.upokecenter.cbor;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/27/2014
 * Time: 2:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import com.upokecenter.util.*;

    /**
     * Description of CBORTag2.
     */
  class CBORTag2 implements ICBORTag
  {
    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    static CBORObject FromObjectAndInnerTags(Object objectValue, CBORObject objectWithTags) {
      CBORObject newObject = CBORObject.FromObject(objectValue);
      if (!objectWithTags.isTagged()) {
        return newObject;
      }
      objectWithTags = objectWithTags.UntagOne();
      if (!objectWithTags.isTagged()) {
        return newObject;
      }
      BigInteger[] tags = objectWithTags.GetTags();
      for (int i = tags.length - 1; i >= 0; --i) {
        newObject = CBORObject.FromObjectAndTag(newObject, tags[i]);
      }
      return newObject;
    }

    static CBORObject ConvertToBigNum(CBORObject o, boolean negative) {
      if (o.getType() != CBORType.ByteString) {
        throw new CBORException("Byte array expected");
      }
      byte[] data = o.GetByteString();
      if (data.length <= 7) {
        long x = 0;
        for (int i = 0; i < data.length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xFF;
        }
        if (negative) {
          x = -x;
          x -= 1L;
        }
        return FromObjectAndInnerTags(x, o);
      }
      int neededLength = data.length;
      byte[] bytes;
      boolean extended = false;
      if (((data[0] >> 7) & 1) != 0) {
        // Increase the needed length
        // if the highest bit is set, to
        // distinguish negative and positive
        // values
        ++neededLength;
        extended = true;
      }
      bytes = new byte[neededLength];
      for (int i = 0; i < data.length; ++i) {
        bytes[i] = data[data.length - 1 - i];
        if (negative) {
          bytes[i] = (byte)((~((int)bytes[i])) & 0xFF);
        }
      }
      if (extended) {
        if (negative) {
          bytes[bytes.length - 1] = (byte)0xFF;
        } else {
          bytes[bytes.length - 1] = 0;
        }
      }
      BigInteger bi = BigInteger.fromByteArray((byte[])bytes,true);
      // NOTE: Here, any tags are discarded; when called from
      // the Read method, "o" will have no tags anyway (beyond tag 2),
      // and when called from FromObjectAndTag, we prefer
      // flexibility over throwing an error if the input
      // Object contains other tags. The tag 2 is also discarded
      // because we are returning a "natively" supported CBOR Object.
      return CBORObject.FromObject(bi);
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToBigNum(obj, false);
    }
  }
