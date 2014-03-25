/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CBORTest
{
  internal sealed class ParserUtility {
    // Wsp, a.k.a. 1*LWSP-char under RFC 822
    public static int SkipSpaceAndTab(string str, int index, int endIndex) {
      while (index<endIndex) {
        if (str[index]==0x09 || str[index]==0x20) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }
    public static int SkipCrLf(string str, int index, int endIndex) {
      if (index + 1<endIndex && str[index]==0x0d && str[index]==0x0a) {
        return index + 2;
      } else {
        return index;
      }
    }
  }
}
