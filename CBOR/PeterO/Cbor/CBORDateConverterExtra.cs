/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using PeterO.Numbers;

namespace PeterO.Cbor {
  // Contains extra methods placed separately
  // because they are not CLS-compliant or they
  // are specific to the .NET version of the library.
  public sealed partial class CBORDateConverter :
ICBORToFromConverter<DateTime>
  {
    /// <summary>Tries to extract the fields of a date and time in the form
    /// of a CBOR object.</summary>
    /// <param name='obj'>A CBOR object that specifies a date/time
    /// according to the conversion type used to create this date
    /// converter.</param>
    /// <param name='year'>Will store the year. If this function fails, the
    /// year is set to null.</param>
    /// <param name='lesserFields'>An array that will store the fields
    /// (other than the year) of the date and time. The array's length must
    /// be 7 or greater. If this function fails, the first seven elements
    /// are set to 0. For more information, see the (EInteger[], int)
    /// overload of this method.</param>
    /// <returns>Either <c>true</c> if the method is successful, or
    /// <c>false</c> otherwise.</returns>
    public bool TryGetDateTimeFields(CBORObject obj, out EInteger year, int[]
      lesserFields) {
      if (lesserFields == null) {
        year = null;
        return false;
      }
      if (lesserFields.Length < 7) {
        year = null;
        return false;
      }
      var eint = new EInteger[1];
      bool ret = this.TryGetDateTimeFields(obj, eint, lesserFields);
      year = eint[0];
      return ret;
    }
  }
}
