/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under Creative Commons Zero (CC0):
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal class CBORDateConverterUntagged : ICBORToFromConverter<DateTime> {
    public DateTime FromCBORObject(CBORObject obj) {
      if (obj.IsTagged) {
         throw new CBORException("May not be tagged");
      }
      CBORObject untagobj = obj;
      if (!untagobj.IsNumber) {
          throw new CBORException("Not a finite number");
        }
        CBORNumber num = untagobj.AsNumber();
        if (!num.IsFinite()) {
          throw new CBORException("Not a finite number");
        }
        if (num.CompareTo(Int64.MinValue) < 0 ||
            num.CompareTo(Int64.MaxValue) > 0) {
          throw new CBORException("Too big or small to fit a DateTime");
        }
        EDecimal dec;
        dec = (EDecimal)untagobj.ToObject(typeof(EDecimal));
        var lesserFields = new int[7];
        var year = new EInteger[1];
        CBORUtilities.BreakDownSecondsSinceEpoch(
          dec,
          year,
          lesserFields);
        return PropertyMap.BuildUpDateTime(year[0], lesserFields);
    }

    public CBORObject ToCBORObject(DateTime bi) {
      try {
        var lesserFields = new int[7];
        var year = new EInteger[1];
        PropertyMap.BreakDownDateTime(bi, year, lesserFields);
        var status = new int[1];
        EFloat ef = CBORUtilities.DateTimeToIntegerOrDouble(
          year[0],
          lesserFields,
          status);
        if (status[0] == 0) {
          return CBORObject.FromObject(ef.ToEInteger());
        } else if (status[0] == 1) {
          return CBORObject.FromObject(ef.ToDouble());
        } else {
          throw new CBORException("Too big or small to fit an integer or" +
"\u0020floating-point number");
        }
      } catch (ArgumentException ex) {
          throw new CBORException(ex.Message, ex);
      }
    }
  }
}
