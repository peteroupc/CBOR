/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;

namespace PeterO.Cbor {
  internal class CBORTag0 : ICBORTag, ICBORObjectConverter<DateTime> {
    private static string DateTimeToString(DateTime bi) {
      int[] dt = PropertyMap.BreakDownDateTime(bi);
      // TODO: Change to true in next major version
      return CBORUtilities.ToAtomDateTimeString(dt, false);
    }

    internal static void AddConverter() {
      // TODO: FromObject with Dates has different behavior
      // in Java version, which has to be retained until
      // the next major version for backward compatibility.
      // However, since ToObject is new, we can convert
      // to Date in the .NET and Java versions
      if (PropertyMap.DateTimeCompatHack) {
        CBORObject.AddConverter(typeof(DateTime), new CBORTag0());
      }
    }

    public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    public CBORObject ValidateObject(CBORObject obj) {
      if (obj.Type != CBORType.TextString) {
        throw new CBORException("Not a text string");
      }
      return obj;
    }

    public DateTime FromCBORObject(CBORObject obj) {
      // TODO: Support tag 1
      if (!obj.HasMostOuterTag(0)) {
        throw new CBORException("Not tag 0");
      }
      return StringToDateTime(obj.AsString());
    }

    public static DateTime StringToDateTime(string str) {
      int[] dt = CBORUtilities.ParseAtomDateTimeString(str);
      return PropertyMap.BuildUpDateTime(dt);
    }

    public CBORObject ToCBORObject(DateTime obj) {
      return CBORObject.FromObjectAndTag(DateTimeToString(obj), 0);
    }
  }
}
