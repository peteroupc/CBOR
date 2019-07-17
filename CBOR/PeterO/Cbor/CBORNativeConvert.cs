/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal static class CBORNativeConvert {
    // TODO: Add TagCount and HasOneTag to CBORObject in 3.6 and later
    // TODO: Deprecate CBORType.Number in 3.6
    [Obsolete]
    private static CBORObject FromObjectAndInnerTags(
      object objectValue,
      CBORObject objectWithTags) {
      // TODO: Find out what is using this method; maybe there's
      // a better alternative or no need for this
      CBORObject newObject = CBORObject.FromObject(objectValue);
      if (!objectWithTags.IsTagged) {
        return newObject;
      }
      objectWithTags = objectWithTags.UntagOne();
      if (!objectWithTags.IsTagged) {
        return newObject;
      }
      EInteger[] tags = objectWithTags.GetAllTags();
      for (int i = tags.Length - 1; i >= 0; --i) {
        newObject = CBORObject.FromObjectAndTag(newObject, tags[i]);
      }
      return newObject;
    }
  }
}
