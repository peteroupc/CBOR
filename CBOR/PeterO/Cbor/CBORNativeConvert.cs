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
    private static CBORObject FromObjectAndInnerTags(
      object objectValue,
      CBORObject objectWithTags) {
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

    public static CBORObject ConvertToNativeObject(CBORObject o) {
      // TODO: Use something like HasOneTag rather than HasMostOuterTag,
      // or preserve inner tags
      if (o.HasMostOuterTag(2) || o.HasMostOuterTag(3)) {
        return CheckEInteger(o);
      }
      if (o.HasMostOuterTag(4)) {
        return ConvertToDecimalFrac(o, true, false);
      }
      if (o.HasMostOuterTag(5)) {
        return ConvertToDecimalFrac(o, false, false);
      }
      if (o.HasMostOuterTag(30)) {
        return CheckRationalNumber(o);
      }
      if (o.HasMostOuterTag(264)) {
        return ConvertToDecimalFrac(o, true, true);
      }
      return o.HasMostOuterTag(265) ?
              ConvertToDecimalFrac(o, false, true) : o;
    }

    private static CBORObject ConvertToDecimalFrac(
      CBORObject o,
      bool isDecimal,
      bool extended) {
      if (o.Type != CBORType.Array) {
        throw new CBORException("Big fraction must be an array");
      }
      if (o.Count != 2) {
        throw new CBORException("Big fraction requires exactly 2 items");
      }
      if (!o[0].IsIntegral) {
        throw new CBORException("Exponent is not an integer");
      }
      if (!o[1].IsIntegral) {
        throw new CBORException("Mantissa is not an integer");
      }
      EInteger exponent = o[0].AsEInteger();
      EInteger mantissa = o[1].AsEInteger();
      if (!extended &&
         exponent.GetSignedBitLengthAsEInteger().CompareTo(64) > 0) {
        throw new CBORException("Exponent is too big");
      }
      if (exponent.IsZero) {
        // Exponent is 0, so return mantissa instead
        return CBORObject.FromObject(mantissa);
      }
      // NOTE: Discards tags.
      return isDecimal ?
      CBORObject.FromObject(EDecimal.Create(mantissa, exponent)) :
      CBORObject.FromObject(EFloat.Create(mantissa, exponent));
    }

    private static CBORObject CheckEInteger(CBORObject o) {
      if (o.GetAllTags().Count != 1) {
        throw new CBORException("One tag expected");
      }
      if (o.Type != CBORType.ByteString) {
        throw new CBORException("Byte array expected");
      }
      return o;
    }

    internal static CBORNumber EIntegerObjectToCBORNumber(CBORObject o) {
      CheckBigNum(o);
      bool negative = o.HasMostOuterTag(3);
      byte[] data = o.GetByteString();
      if (data.Length <= 7) {
        long x = 0;
        for (var i = 0; i < data.Length; ++i) {
          x <<= 8;
          x |= ((long)data[i]) & 0xff;
        }
        if (negative) {
          x = -x;
          --x;
        }
        return FromObjectAndInnerTags(x, o);
      }
      int neededLength = data.Length;
      byte[] bytes;
      EInteger bi;
      var extended = false;
      if (((data[0] >> 7) & 1) != 0) {
        // Increase the needed length
        // if the highest bit is set, to
        // distinguish negative and positive
        // values
        ++neededLength;
        extended = true;
      }
      bytes = new byte[neededLength];
      for (var i = 0; i < data.Length; ++i) {
        bytes[i] = data[data.Length - 1 - i];
        if (negative) {
          bytes[i] = (byte)((~((int)bytes[i])) & 0xff);
        }
      }
      if (extended) {
        bytes[bytes.Length - 1] = negative ? (byte)0xff : (byte)0;
      }
      bi = EInteger.FromBytes(bytes, true);
      if(bi.CanFitInInt64()){
        return new CBORNumber(CBORNumber.Kind.Integer, bi.ToInt64Checked());
      } else {
        return new CBORNumber(CBORNumber.Kind.EInteger, bi);
      }
    }

    private static CBORObject CheckRationalNumber(CBORObject obj) {
      if (obj.Type != CBORType.Array) {
#if DEBUG
        throw new CBORException("Rational number must be an array\n" +
          obj.ToString());
#else
        throw new CBORException("Rational number must be an array");
#endif
      }
      if (obj.Count != 2) {
        throw new CBORException("Rational number requires exactly 2 items");
      }
      CBORObject first = obj[0];
      CBORObject second = obj[1];
      if (!first.IsIntegral) {
        throw new CBORException("Rational number requires integer numerator");
      }
      if (!second.IsIntegral) {
        throw new CBORException("Rational number requires integer denominator");
      }
      if (second.Sign <= 0) {
        throw new CBORException(
           "Rational number requires denominator greater than 0");
      }
      return obj;
    }
  }
}
