package com.upokecenter.util;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 2/28/2014
 * Time: 11:49 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

    /**
     * Description of CBORTag0.
     */
  class CBORTag0 implements ICBORTag, ICBORConverter<DateTime>
  {
    private static String DateTimeToString(DateTime bi) {
      DateTime dt = bi.ToUniversalTime();
      int year = dt.getYear();
      int month = dt.getMonth();
      int day = dt.getDay();
      int hour = dt.getHour();
      int minute = dt.getMinute();
      int second = dt.getSecond();
      int millisecond = dt.getMillisecond();
      char[] charbuf = new char[millisecond > 0 ? 24 : 20];
      charbuf[0] = (char)('0' + ((year / 1000) % 10));
      charbuf[1] = (char)('0' + ((year / 100) % 10));
      charbuf[2] = (char)('0' + ((year / 10) % 10));
      charbuf[3] = (char)('0' + (year % 10));
      charbuf[4] = '-';
      charbuf[5] = (char)('0' + ((month / 10) % 10));
      charbuf[6] = (char)('0' + (month % 10));
      charbuf[7] = '-';
      charbuf[8] = (char)('0' + ((day / 10) % 10));
      charbuf[9] = (char)('0' + (day % 10));
      charbuf[10] = 'T';
      charbuf[11] = (char)('0' + ((hour / 10) % 10));
      charbuf[12] = (char)('0' + (hour % 10));
      charbuf[13] = ':';
      charbuf[14] = (char)('0' + ((minute / 10) % 10));
      charbuf[15] = (char)('0' + (minute % 10));
      charbuf[16] = ':';
      charbuf[17] = (char)('0' + ((second / 10) % 10));
      charbuf[18] = (char)('0' + (second % 10));
      if (millisecond > 0) {
        charbuf[19] = '.';
        charbuf[20] = (char)('0' + ((millisecond / 100) % 10));
        charbuf[21] = (char)('0' + ((millisecond / 10) % 10));
        charbuf[22] = (char)('0' + (millisecond % 10));
        charbuf[23] = 'Z';
      } else {
        charbuf[19] = 'Z';
      }
      return new String(charbuf);
    }

    /**
     * Not documented yet.
     * @return A CBORTypeFilter object.
     */
public CBORTypeFilter GetTypeFilter() {
      return CBORTypeFilter.TextString;
    }

    /**
     * Not documented yet.
     * @param obj A CBORObject object. (2).
     * @return A CBORObject object.
     */
public CBORObject ValidateObject(CBORObject obj) {
      if (obj.getType() != CBORType.TextString) {
 throw new CBORException("Not a text String");
}
      return obj;
    }

    /**
     * Not documented yet.
     * @param obj A DateTime object.
     * @return A CBORObject object.
     */
public CBORObject ToCBORObject(DateTime obj) {
      return CBORObject.FromObjectAndTag(DateTimeToString(obj), 0);
    }
  }
