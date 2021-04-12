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
  /// <summary>
  /// <para>A class for converting date-time objects to and from tagged
  /// CBOR objects.</para>
  /// <para>In this method's documentation, the "number of seconds since
  /// the start of 1970" is based on the POSIX definition of "seconds
  /// since the Epoch", a definition that does not count leap seconds.
  /// This number of seconds assumes the use of a proleptic Gregorian
  /// calendar, in which the rules regarding the number of days in each
  /// month and which years are leap years are the same for all years as
  /// they were in 1970 (including without regard to time zone
  /// differences or transitions from other calendars to the
  /// Gregorian).</para></summary>
  public sealed class CBORDateConverter : ICBORToFromConverter<DateTime> {
    private readonly ConversionType convType;

       /// <summary>A converter object where FromCBORObject accepts CBOR
       /// objects with tag 0 (date/time strings) and tag 1 (number of seconds
       /// since the start of 1970), and ToCBORObject converts date/time
       /// objects (DateTime in DotNet, and Date in Java) to CBOR objects of
       /// tag 0.</summary>
    public static readonly CBORDateConverter TaggedString =
       new CBORDateConverter(ConversionType.TaggedString);

       /// <summary>A converter object where FromCBORObject accepts CBOR
       /// objects with tag 0 (date/time strings) and tag 1 (number of seconds
       /// since the start of 1970), and ToCBORObject converts date/time
       /// objects (DateTime in DotNet, and Date in Java) to CBOR objects of
       /// tag 1. The ToCBORObject conversion is lossless only if the number
       /// of seconds since the start of 1970 can be represented exactly as an
       /// integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit
       /// floating-point number in the IEEE 754r binary64 format; the
       /// conversion is lossy otherwise. The ToCBORObject conversion will
       /// throw an exception if the conversion to binary64 results in
       /// positive infinity, negative infinity, or not-a-number.</summary>
    public static readonly CBORDateConverter TaggedNumber =
       new CBORDateConverter(ConversionType.TaggedNumber);

       /// <summary>A converter object where FromCBORObject accepts untagged
       /// CBOR integer or CBOR floating-point objects that give the number of
       /// seconds since the start of 1970, and where ToCBORObject converts
       /// date/time objects (DateTime in DotNet, and Date in Java) to such
       /// untagged CBOR objects. The ToCBORObject conversion is lossless only
       /// if the number of seconds since the start of 1970 can be represented
       /// exactly as an integer in the interval [-(2^64), 2^64 - 1] or as a
       /// 64-bit floating-point number in the IEEE 754r binary64 format; the
       /// conversion is lossy otherwise. The ToCBORObject conversion will
       /// throw an exception if the conversion to binary64 results in
       /// positive infinity, negative infinity, or not-a-number.</summary>
    public static readonly CBORDateConverter UntaggedNumber =
       new CBORDateConverter(ConversionType.UntaggedNumber);

  /// <summary>Conversion type for date-time conversion.</summary>
    public enum ConversionType {
       /// <summary>FromCBORObject accepts CBOR objects with tag 0 (date/time
       /// strings) and tag 1 (number of seconds since the start of 1970), and
       /// ToCBORObject converts date/time objects to CBOR objects of tag
       /// 0.</summary>
       TaggedString,

       /// <summary>FromCBORObject accepts objects with tag 0 (date/time
       /// strings) and tag 1 (number of seconds since the start of 1970), and
       /// ToCBORObject converts date/time objects to CBOR objects of tag 1.
       /// The ToCBORObject conversion is lossless only if the number of
       /// seconds since the start of 1970 can be represented exactly as an
       /// integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit
       /// floating-point number in the IEEE 754r binary64 format; the
       /// conversion is lossy otherwise. The ToCBORObject conversion will
       /// throw an exception if the conversion to binary64 results in
       /// positive infinity, negative infinity, or not-a-number.</summary>
       TaggedNumber,

       /// <summary>FromCBORObject accepts untagged CBOR integer or CBOR
       /// floating-point objects that give the number of seconds since the
       /// start of 1970, and ToCBORObject converts date/time objects
       /// (DateTime in DotNet, and Date in Java) to such untagged CBOR
       /// objects. The ToCBORObject conversion is lossless only if the number
       /// of seconds since the start of 1970 can be represented exactly as an
       /// integer in the interval [-(2^64), 2^64 - 1] or as a 64-bit
       /// floating-point number in the IEEE 754r binary64 format; the
       /// conversion is lossy otherwise. The ToCBORObject conversion will
       /// throw an exception if the conversion to binary64 results in
       /// positive infinity, negative infinity, or not-a-number.</summary>
       UntaggedNumber,
    }

  /// <summary>Initializes a new instance of the
  /// <see cref='PeterO.Cbor.CBORDateConverter'/> class.</summary>
    public CBORDateConverter() : this(ConversionType.TaggedString) {
}

  /// <summary>Initializes a new instance of the
  /// <see cref='PeterO.Cbor.CBORDateConverter'/> class.</summary>
  /// <param name='convType'>The parameter <paramref name='convType'/> is
  /// a Cbor.CBORDateConverter.ConversionType object.</param>
    public CBORDateConverter(ConversionType convType) {
       this.convType = convType;
    }

    private static string DateTimeToString(DateTime bi) {
      try {
        var lesserFields = new int[7];
        var year = new EInteger[1];
        PropertyMap.BreakDownDateTime(bi, year, lesserFields);
        return CBORUtilities.ToAtomDateTimeString(year[0], lesserFields);
      } catch (ArgumentException ex) {
          throw new CBORException(ex.Message, ex);
      }
    }

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is a
  /// Cbor.CBORObject object.</param>
  /// <returns>The return value is not documented yet.</returns>
  /// <exception cref='ArgumentNullException'>The parameter <paramref
  /// name='obj'/> is null.</exception>
    public DateTime FromCBORObject(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      var lesserFields = new int[7];
      var year = new EInteger[1];
      string str = this.TryGetDateTimeFieldsInternal(obj, year, lesserFields);
      if (str == null) {
        return PropertyMap.BuildUpDateTime(year[0], lesserFields);
      }
      throw new CBORException(str);
    }

  /// <summary>Tries to extract the fields of a date and time in the form
  /// of a CBOR object.</summary>
  /// <returns>Either <c>true</c> if the method is successful, or
  /// <c>false</c> otherwise.</returns>
  /// <param name='obj'>Not documented yet.</param>
  /// <param name='year'>An array whose first element will store the
  /// year. The array's length must be 1 or greater. If this function
  /// fails, the first element is set to null.</param>
  /// <param name='lesserFields'>An array that will store the fields
  /// (other than the year) of the date and time. The array's length must
  /// be 7 or greater. If this function fails, the first seven elements
  /// are set to 0. If this method is successful, the first seven
  /// elements of the array (starting at 0) will be as follows:
  /// <list>
  /// <item>0 - Month of the year, from 1 (January) through 12
  /// (December).</item>
  /// <item>1 - Day of the month, from 1 through 31.</item>
  /// <item>2 - Hour of the day, from 0 through 23.</item>
  /// <item>3 - Minute of the hour, from 0 through 59.</item>
  /// <item>4 - Second of the minute, from 0 through 59.</item>
  /// <item>5 - Fractional seconds, expressed in nanoseconds. This value
  /// cannot be less than 0.</item>
  /// <item>6 - Number of minutes to subtract from this date and time to
  /// get global time. This number can be positive or negative, but
  /// cannot be less than -1439 or greater than 1439. For tags 0 and 1,
  /// this value is always 0.</item></list></param>
  /// <exception cref='ArgumentNullException'>The parameter <paramref
  /// name='year'/> or <paramref name='lesserFields'/> is null, or
  /// contains fewer elements than required.</exception>
    public bool TryGetDateTimeFields(CBORObject obj, EInteger[] year, int[]
lesserFields) {
       if (year == null) {
         throw new ArgumentNullException(nameof(year));
       }
       if (year.Length < 1) {
         throw new ArgumentException("\"year\" + \"'s length\" (" +
year.Length + ") is not greater or equal to 1");
       }
       if (lesserFields == null) {
         throw new ArgumentNullException(nameof(lesserFields));
       }
       if (lesserFields.Length < 7) {
         throw new ArgumentException("\"lesserFields\" + \"'s length\" (" +
lesserFields.Length + ") is not greater or equal to 7");
       }
       string str = this.TryGetDateTimeFieldsInternal(obj, year, lesserFields);
       if (str == null) {
          // No error string
          return true;
       } else {
          // With error string
          year[0] = null;
          for (var i = 0; i < 7; ++i) {
            lesserFields[i] = 0;
          }
          return false;
       }
    }

    private string TryGetDateTimeFieldsInternal(
      CBORObject obj,
      EInteger[] year,
      int[] lesserFields) {
if (obj == null) {
         return "Object is null";
       }
       if (year == null) {
         throw new ArgumentNullException(nameof(year));
       }
       if (year.Length < 1) {
         throw new ArgumentException("\"year\" + \"'s length\" (" +
year.Length + ") is not greater or equal to 1");
       }
       if (lesserFields == null) {
         throw new ArgumentNullException(nameof(lesserFields));
       }
       if (lesserFields.Length < 7) {
         throw new ArgumentException("\"lesserFields\" + \"'s length\" (" +
lesserFields.Length + ") is not greater or equal to 7");
       }
       if (this.convType == ConversionType.UntaggedNumber) {
         if (obj.IsTagged) {
         return "May not be tagged";
      }
      CBORObject untagobj = obj;
      if (!untagobj.IsNumber) {
          return "Not a finite number";
      }
      CBORNumber num = untagobj.AsNumber();
      if (!num.IsFinite()) {
          return "Not a finite number";
      }
      if (num.CompareTo(Int64.MinValue) < 0 ||
            num.CompareTo(Int64.MaxValue) > 0) {
          return "Too big or small to fit a DateTime";
      }
      EDecimal dec;
      dec = (EDecimal)untagobj.ToObject(typeof(EDecimal));
      CBORUtilities.BreakDownSecondsSinceEpoch(
          dec,
          year,
          lesserFields);
        return null; // no error
      }
      if (obj.HasMostOuterTag(0)) {
        string str = obj.AsString();
        try {
          CBORUtilities.ParseAtomDateTimeString(str, year, lesserFields);
          return null; // no error
        } catch (OverflowException ex) {
          return ex.Message;
        } catch (InvalidOperationException ex) {
          return ex.Message;
        } catch (ArgumentException ex) {
          return ex.Message;
        }
      } else if (obj.HasMostOuterTag(1)) {
        CBORObject untagobj = obj.UntagOne();
        if (!untagobj.IsNumber) {
          return "Not a finite number";
        }
        CBORNumber num = untagobj.AsNumber();
        if (!num.IsFinite()) {
          return "Not a finite number";
        }
        if (num.CompareTo(Int64.MinValue) < 0 ||
            num.CompareTo(Int64.MaxValue) > 0) {
          return "Too big or small to fit a DateTime";
        }
        EDecimal dec;
        dec = (EDecimal)untagobj.ToObject(typeof(EDecimal));
        CBORUtilities.BreakDownSecondsSinceEpoch(
          dec,
          year,
          lesserFields);
        return null; // No error
      }
      return "Not tag 0 or 1";
    }

    public CBORObject DateFieldsToCBORObject(int smallYear, int month, int
day) {
      return DateFieldsToCBORObject(EInteger.FromInt32(smallYear), new int[] { month, day, 0, 0, 0, 0, 0 });
    }

    public CBORObject DateFieldsToCBORObject(int smallYear, int month, int
day, int hour, int minute, int second) {
      return DateFieldsToCBORObject(EInteger.FromInt32(smallYear), new int[] { month, day, hour, minute, second, 0, 0 });
    }

    public CBORObject DateFieldsToCBORObject(EInteger bigYear, int[]
lesserFields) {
       if (year == null) {
         throw new ArgumentNullException(nameof(year));
       }
       if (lesserFields == null) {
         throw new ArgumentNullException(nameof(lesserFields));
       }
       if (lesserFields.Length < 7) {
         throw new ArgumentException("\"lesserFields\" + \"'s length\" (" +
lesserFields.Length + ") is not greater or equal to 7");
       }
       try {
        switch (this.convType) {
          case ConversionType.TaggedString: {
             string str = CBORUtilities.ToAtomDateTimeString(bigYear,
  lesserFields);
             return CBORObject.FromObjectAndTag(str, 0);
          }
        case ConversionType.TaggedNumber:
        case ConversionType.UntaggedNumber:
        try {
         var status = new int[1];
         EFloat ef = CBORUtilities.DateTimeToIntegerOrDouble(
           bigYear,
           lesserFields,
           status);
         if (status[0] == 0) {
          return this.convType == ConversionType.TaggedNumber ?
             CBORObject.FromObjectAndTag(ef.ToEInteger(), 1) :
             CBORObject.FromObject(ef.ToEInteger());
        } else if (status[0] == 1) {
          return this.convType == ConversionType.TaggedNumber ?
             CBORObject.FromObjectAndTag(ef.ToDouble(), 1) :
             CBORObject.FromObject(ef.ToDouble());
        } else {
          throw new CBORException("Too big or small to fit an integer or" +
"\u0020floating-point number");
        }
         } catch (ArgumentException ex) {
          throw new CBORException(ex.Message, ex);
      }
      default: throw new CBORException("Internal error");
        }
      } catch (ArgumentException ex) {
          throw new CBORException(ex.Message, ex);
      }
    }

  /// <summary>Not documented yet.</summary>
  /// <param name='obj'>The parameter <paramref name='obj'/> is a
  /// DateTime object.</param>
  /// <returns>The return value is not documented yet.</returns>
    public CBORObject ToCBORObject(DateTime obj) {
        try {
           var lesserFields = new int[7];
           var year = new EInteger[1];
           PropertyMap.BreakDownDateTime(obj, year, lesserFields);
           return DateFieldsToCBORObject(year[0], lesserFields);
         } catch (ArgumentException ex) {
          throw new CBORException(ex.Message, ex);
      }
      default: throw new CBORException("Internal error");
      }
    }
  }
}
