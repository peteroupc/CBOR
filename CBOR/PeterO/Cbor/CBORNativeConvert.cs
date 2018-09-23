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
  internal static class CBORNativeConvert
  {
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

    public static CBORObject ConvertToNativeObject(CBORObject o){
     if(o.HasMostOuterTag(2))return ConvertToBigNum(o,false);
     if(o.HasMostOuterTag(3))return ConvertToBigNum(o,false);
     if(o.HasMostOuterTag(4))return ConvertToDecimalFrac(o,true,false);
     if(o.HasMostOuterTag(5))return ConvertToDecimalFrac(o,false,false);
     if(o.HasMostOuterTag(30))return ConvertToRationalNumber(o);
     if(o.HasMostOuterTag(264))return ConvertToDecimalFrac(o,true,true);
     if(o.HasMostOuterTag(265))return ConvertToDecimalFrac(o,false,true);
     return o;
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
      if (exponent.GetSignedBitLength() > 64 && !extended) {
        throw new CBORException("Exponent is too big");
      }
      if (exponent.IsZero) {
        // Exponent is 0, so return mantissa instead
        return CBORObject.FromObject(mantissa);
      }
      // NOTE: Discards tags. See comment in CBORTag2.
      return isDecimal ?
      CBORObject.FromObject(EDecimal.Create(mantissa, exponent)) :
      CBORObject.FromObject(EFloat.Create(mantissa, exponent));
    }

    private static CBORObject ConvertToBigNum(CBORObject o, bool negative) {
      if (o.Type != CBORType.ByteString) {
        throw new CBORException("Byte array expected");
      }
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
      // NOTE: Here, any tags are discarded; when called from
      // the Read method, "o" will have no tags anyway (beyond tag 2),
      // and when called from FromObjectAndTag, we prefer
      // flexibility over throwing an error if the input
      // object contains other tags. The tag 2 is also discarded
      // because we are returning a "natively" supported CBOR object.
      return CBORObject.FromObject(bi);
    }

    private static CBORObject ConvertToRationalNumber(CBORObject obj) {
      if (obj.Type != CBORType.Array) {
        throw new CBORException("Rational number must be an array");
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
throw new CBORException("Rational number requires denominator greater than 0");
      }
      EInteger denom = second.AsEInteger();
      // NOTE: Discards tags.
      return denom.Equals(EInteger.One) ?
      CBORObject.FromObject(first.AsEInteger()) :
      CBORObject.FromObject(
  ERational.Create(
  first.AsEInteger(),
  denom));
    }
  }
}
