/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using PeterO; using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORTag30 : ICBORTag
  {
    public CBORTypeFilter GetTypeFilter() {
      return new CBORTypeFilter().WithArrayExactLength(
        2,
        CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithTags(2, 3),
        CBORTypeFilter.UnsignedInteger.WithTags(2));
    }

    public CBORObject ValidateObject(CBORObject obj) {
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
      // NOTE: Discards tags. See comment in CBORTag2.
      return denom.Equals(EInteger.One) ?
      CBORObject.FromObject(first.AsEInteger()) :
      CBORObject.FromObject(
  new ERational(
  first.AsEInteger(),
  denom));
    }
  }
}
