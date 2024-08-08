/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Diagnostics.CodeAnalysis;
using PeterO.Numbers;

namespace PeterO.Cbor {
  /// <summary>
  /// <para>A class for converting date-time objects to and from tagged
  /// CBOR objects.</para>
  /// <para>In this class's documentation, the "number of seconds since
  /// the start of 1970" is based on the POSIX definition of "seconds
  /// since the Epoch", a definition that does not count leap seconds.
  /// This number of seconds assumes the use of a proleptic Gregorian
  /// calendar, in which the rules regarding the number of days in each
  /// month and which years are leap years are the same for all years as
  /// they were in 1970 (including without regard to time zone
  /// differences or transitions from other calendars to the
  /// Gregorian).</para></summary>
  public sealed partial class CBORDateConverter :
ICBORToFromConverter<DateTime>
  {
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
    public enum ConversionType
    {
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

    /// <summary>Gets the conversion type for this date
    /// converter.</summary>
    /// <value>The conversion type for this date converter.</value>
    public ConversionType Type { get; }

    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Cbor.CBORDateConverter'/> class.</summary>
    /// <param name='convType'>Conversion type giving the rules for
    /// converting dates and times to and from CBOR objects.</param>
    public CBORDateConverter(ConversionType convType) {
      this.Type = convType;
    }

    /// <summary>Converts a CBOR object to a DateTime (in DotNet) or a Date
    /// (in Java).</summary>
    /// <param name='obj'>A CBOR object that specifies a date/time
    /// according to the conversion type used to create this date
    /// converter.</param>
    /// <returns>A DateTime or Date that encodes the date/time specified in
    /// the CBOR object.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='obj'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>The format of the CBOR
    /// object is not supported, or another error occurred in
    /// conversion.</exception>
    public DateTime FromCBORObject(CBORObject obj) {
      if (obj == null) {
        throw new ArgumentNullException(nameof(obj));
      }
      var lesserFields = new int[7];
      var outYear = new EInteger[1];
      string str = this.TryGetDateTimeFieldsInternal(
          obj,
          outYear,
          lesserFields);
      return str == null ? PropertyMap.BuildUpDateTime(outYear[0],
  lesserFields) : throw new CBORException(str);
    }

    /// <summary>Tries to extract the fields of a date and time in the form
    /// of a CBOR object.</summary>
    /// <param name='obj'>A CBOR object that specifies a date/time
    /// according to the conversion type used to create this date
    /// converter.</param>
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
    /// cannot be less than 0 and must be less than 1000*1000*1000.</item>
    /// <item>6 - Number of minutes to subtract from this date and time to
    /// get global time. This number can be positive or negative, but
    /// cannot be less than -1439 or greater than 1439. For tags 0 and 1,
    /// this value is always 0.</item></list>.</param>
    /// <returns>Either <c>true</c> if the method is successful, or
    /// <c>false</c> otherwise.</returns>
    public bool TryGetDateTimeFields(CBORObject obj, EInteger[] year, int[]
      lesserFields) {
      if (year == null) {
        return false;
      }
      EInteger[] outYear = year;
      if (outYear.Length < 1) {
        return false;
      }
      if (lesserFields == null) {
        return false;
      }
      if (lesserFields.Length < 7) {
        return false;
      }
      string str = this.TryGetDateTimeFieldsInternal(
          obj,
          outYear,
          lesserFields);
      if (str == null) {
        // No error string was returned
        return true;
      } else {
        // An error string was returned
        outYear[0] = null;
        for (int i = 0; i < 7; ++i) {
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
        return "Year is null";
      }
      EInteger[] outYear = year;
      if (outYear.Length < 1) {
        return "\"year\" + \"'s length\" (" +
          outYear.Length + ") is not greater or equal to 1";
      }
      if (lesserFields == null) {
        return "Lesser fields is null";
      }
      if (lesserFields.Length < 7) {
        return "\"lesserFields\" + \"'s length\" (" +
          lesserFields.Length + ") is not greater or equal to 7";
      }
      ConversionType thisType = this.Type;
      if (thisType == ConversionType.UntaggedNumber) {
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
        if (num.CanFitInInt64()) {
          CBORUtilities.BreakDownSecondsSinceEpoch(
            num.ToInt64Checked(),
            outYear,
            lesserFields);
        } else {
          EDecimal dec;
          dec = untagobj.ToEDecimal();
          CBORUtilities.BreakDownSecondsSinceEpoch(
            dec,
            outYear,
            lesserFields);
        }
        return null; // no error
      }
      if (obj.HasMostOuterTag(0)) {
        string str = obj.AsString();
        try {
          CBORUtilities.ParseAtomDateTimeString(str, outYear, lesserFields);
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
        if (num.CanFitInInt64()) {
          CBORUtilities.BreakDownSecondsSinceEpoch(
            num.ToInt64Checked(),
            outYear,
            lesserFields);
        } else {
          EDecimal dec;
          dec = untagobj.ToEDecimal();
          CBORUtilities.BreakDownSecondsSinceEpoch(
            dec,
            outYear,
            lesserFields);
        }
        return null; // No error
      }
      return "Not tag 0 or 1";
    }

    /// <summary>Converts a date/time in the form of a year, month, and day
    /// to a CBOR object. The hour, minute, and second are treated as
    /// 00:00:00 by this method, and the time offset is treated as 0 by
    /// this method.</summary>
    /// <param name='smallYear'>The year.</param>
    /// <param name='month'>Month of the year, from 1 (January) through 12
    /// (December).</param>
    /// <param name='day'>Day of the month, from 1 through 31.</param>
    /// <returns>A CBOR object encoding the given date fields according to
    /// the conversion type used to create this date converter.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in
    /// conversion.</exception>
    public CBORObject DateTimeFieldsToCBORObject(int smallYear, int month, int
      day) {
      return this.DateTimeFieldsToCBORObject(EInteger.FromInt32(smallYear),
          new int[] { month, day, 0, 0, 0, 0, 0 });
    }

    /// <summary>Converts a date/time in the form of a year, month, day,
    /// hour, minute, and second to a CBOR object. The time offset is
    /// treated as 0 by this method.</summary>
    /// <param name='smallYear'>The year.</param>
    /// <param name='month'>Month of the year, from 1 (January) through 12
    /// (December).</param>
    /// <param name='day'>Day of the month, from 1 through 31.</param>
    /// <param name='hour'>Hour of the day, from 0 through 23.</param>
    /// <param name='minute'>Minute of the hour, from 0 through 59.</param>
    /// <param name='second'>Second of the minute, from 0 through
    /// 59.</param>
    /// <returns>A CBOR object encoding the given date fields according to
    /// the conversion type used to create this date converter.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in
    /// conversion.</exception>
    public CBORObject DateTimeFieldsToCBORObject(
      int smallYear,
      int month,
      int day,
      int hour,
      int minute,
      int second) {
      return this.DateTimeFieldsToCBORObject(EInteger.FromInt32(smallYear),
          new int[] { month, day, hour, minute, second, 0, 0 });
    }

    /// <summary>Converts a date/time in the form of a year, month, day,
    /// hour, minute, second, fractional seconds, and time offset to a CBOR
    /// object.</summary>
    /// <param name='year'>The year.</param>
    /// <param name='lesserFields'>An array that will store the fields
    /// (other than the year) of the date and time. See the
    /// TryGetDateTimeFields method for information on the "lesserFields"
    /// parameter.</param>
    /// <returns>A CBOR object encoding the given date fields according to
    /// the conversion type used to create this date converter.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='lesserFields'/> is null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in
    /// conversion.</exception>
    public CBORObject DateTimeFieldsToCBORObject(int year, int[]
      lesserFields) {
      return this.DateTimeFieldsToCBORObject(EInteger.FromInt32(year),
  lesserFields);
    }

    /// <summary>Converts a date/time in the form of a year, month, day,
    /// hour, minute, second, fractional seconds, and time offset to a CBOR
    /// object.</summary>
    /// <param name='bigYear'>The year.</param>
    /// <param name='lesserFields'>An array that will store the fields
    /// (other than the year) of the date and time. See the
    /// TryGetDateTimeFields method for information on the "lesserFields"
    /// parameter.</param>
    /// <returns>A CBOR object encoding the given date fields according to
    /// the conversion type used to create this date converter.</returns>
    /// <exception cref='ArgumentNullException'>The parameter <paramref
    /// name='bigYear'/> or <paramref name='lesserFields'/> is
    /// null.</exception>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in
    /// conversion.</exception>
    public CBORObject DateTimeFieldsToCBORObject(EInteger bigYear, int[]
      lesserFields) {
      if (bigYear == null) {
        throw new ArgumentNullException(nameof(bigYear));
      }
      if (lesserFields == null) {
        throw new ArgumentNullException(nameof(lesserFields));
      }
      // TODO: Make into CBORException in next major version
      if (lesserFields.Length < 7) {
        throw new ArgumentException("\"lesserFields\" + \"'s length\" (" +
          lesserFields.Length + ") is not greater or equal to 7");
      }
      try {
        CBORUtilities.CheckYearAndLesserFields(bigYear, lesserFields);
        ConversionType thisType = this.Type;
        switch (thisType) {
          case ConversionType.TaggedString:
            {
              string str = CBORUtilities.ToAtomDateTimeString(bigYear,
                  lesserFields);
              return CBORObject.FromString(str).WithTag(0);
            }
          case ConversionType.TaggedNumber:
          case ConversionType.UntaggedNumber:
            try {
              var status = new int[1];
              EFloat ef = CBORUtilities.DateTimeToIntegerOrDouble(
                  bigYear,
                  lesserFields,
                  status);
              switch (status[0]) {
                case 0: {
                    CBORObject cbor = CBORObject.FromEInteger(ef.ToEInteger());
                    return thisType == ConversionType.TaggedNumber ?
                      CBORObject.FromCBORObjectAndTag(cbor, 1) :
                      cbor;
                  }
                case 1:
                  return thisType == ConversionType.TaggedNumber ?
                    CBORObject.FromFloatingPointBits(ef.ToDoubleBits(), 8)
                    .WithTag(1) :
                    CBORObject.FromFloatingPointBits(ef.ToDoubleBits(), 8);
                default: throw new CBORException("Too big or small to fit an" +
                    "\u0020integer" + "\u0020or floating-point number");
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

    /// <summary>Converts a DateTime (in DotNet) or Date (in Java) to a
    /// CBOR object in a manner specified by this converter's conversion
    /// type.</summary>
    /// <param name='obj'>The parameter <paramref name='obj'/> is a
    /// DateTime object.</param>
    /// <returns>A CBOR object encoding the date/time in the DateTime or
    /// Date according to the conversion type used to create this date
    /// converter.</returns>
    /// <exception cref='PeterO.Cbor.CBORException'>An error occurred in
    /// conversion.</exception>
    public CBORObject ToCBORObject(DateTime obj) {
      try {
        var lesserFields = new int[7];
        var outYear = new EInteger[1];
        PropertyMap.BreakDownDateTime(obj, outYear, lesserFields);
        return this.DateTimeFieldsToCBORObject(outYear[0], lesserFields);
      } catch (ArgumentException ex) {
        throw new CBORException(ex.Message, ex);
      }
    }
  }
}
