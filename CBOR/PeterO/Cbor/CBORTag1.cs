/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORTag1 : ICBORTag, ICBORObjectConverter<Date>
  {
    public CBORTypeFilter GetTypeFilter() {
      return
      CBORTypeFilter.UnsignedInteger.WithNegativeInteger().WithFloatingPoint();
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (!obj.IsFinite) {
        throw new CBORException("Not a valid date");
      }
      return obj;
    }

    public CBORObject ToCBORObject(DateTime obj) {
       // TODO
       throw new NotImplementedException();
    }
    public DateTime FromCBORObject(CBORObject obj) {
      if (!obj.HasMostOuterTag(1) || !obj.IsFinite) {
        throw new CBORException("Not a valid date");
      }
      EDecimal dec = obj.AsEDecimal();
      var lesserFields = new int[7];
      EInteger[] year = new int[1];
      CBORUtilities.BreakDownSecondsSinceEpoch(
              dec,
              year,
              lesserFields);
      return CBORUtilities.BuildUpDateTime(year[0], lesserFields);
    }
  }
}
