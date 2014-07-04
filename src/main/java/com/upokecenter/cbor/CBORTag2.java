package com.upokecenter.cbor;
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import com.upokecenter.util.*;

  class CBORTag2 implements ICBORTag {

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.ByteString;
    }

    private static CBORObject FromObjectAndInnerTags(
Object objectValue,
CBORObject objectWithTags) {
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
          x |= ((long)data[i]) & 0xff;
        }
        if (negative) {
          x = -x;
          x -= 1L;
        }
        return FromObjectAndInnerTags(x, o);
      }
      int neededLength = data.length;
      byte[] bytes;
      BigInteger bi;
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
          bytes[i] = (byte)((~((int)bytes[i])) & 0xff);
        }
      }
      if (extended) {
          bytes[bytes.length - 1] = negative ? (byte)0xff : (byte)0;
      }
      bi = BigInteger.fromByteArray(bytes, true);
      // NOTE: Here, any tags are discarded; when called from
      // the Read method, "o" will have no tags anyway (beyond tag 2),
      // and when called from FromObjectAndTag, we prefer
      // flexibility over throwing an error if the input
      // Object contains other tags. The tag 2 is also discarded
      // because we are returning a "natively" supported CBOR Object.
      return CBORObject.FromObject(bi);
    }

    public CBORObject ValidateObject(CBORObject obj) {
      return ConvertToBigNum(obj, false);
    }
  }
