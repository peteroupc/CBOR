package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.util.*;

  final class ParserUtility {
    static String ToLowerCaseAscii(String str) {
      if (str == null) {
        return null;
      }
      int len = str.length();
      char c=(char)0;
      boolean hasUpperCase = false;
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c>= 'A' && c<= 'Z') {
          hasUpperCase = true;
          break;
        }
      }
      if (!hasUpperCase) {
        return str;
      }
      StringBuilder builder = new StringBuilder();
      for (int i = 0; i < len; ++i) {
        c = str.charAt(i);
        if (c>= 'A' && c<= 'Z') {
          builder.append((char)(c + 0x20));
        } else {
          builder.append(c);
        }
      }
      return builder.toString();
    }

    public static boolean EndsWith(String str, String suffix, int index) {
      if (str == null || suffix == null || index<0 || index >= str.length()) {
        throw new IllegalArgumentException();
      }
      int endpos = suffix.length() + index;
      if (endpos>str.length()) {
        return false;
      }
      return str.substring(index,endpos).equals(suffix);
    }

    public static boolean StartsWith(String str, String prefix) {
      if (str == null || prefix == null) {
        throw new IllegalArgumentException();
      }
      if (prefix.length()<str.length()) {
        return false;
      }
      return str.substring(0,prefix.length()).equals(prefix);
    }

    public static String TrimSpaceAndTab(String s) {
      if (s == null || s.length() == 0) {
        return s;
      }
      int index = 0;
      int sLength = s.length();
      while (index<sLength) {
        char c = s.charAt(index);
        if (c != 0x09 && c != 0x20) {
          break;
        }
        ++index;
      }
      if (index == sLength) {
        return "";
      }
      int startIndex = index;
      index = sLength-1;
      while (index >= 0) {
        char c = s.charAt(index);
        if (c != 0x09 && c != 0x20) {
          return s.substring(startIndex,index + 1);
        }
        --index;
      }
      return "";
    }

    public static boolean IsNullEmptyOrWhitespace(String str) {
      return ((str)==null || (str).length()==0) || SkipSpaceAndTab(str, 0, str.length()) == str.length();
    }

    // Wsp, a.k.a. 1*LWSP-char under RFC 822
    public static int SkipSpaceAndTab(String str, int index, int endIndex) {
      while (index<endIndex) {
        if (str.charAt(index)==0x09 || str.charAt(index)==0x20) {
          ++index;
        } else {
          break;
        }
      }
      return index;
    }
    public static int SkipCrLf(String str, int index, int endIndex) {
      if (index + 1<endIndex && str.charAt(index)==0x0d && str.charAt(index + 1)==0x0a) {
        return index + 2;
      } else {
        return index;
      }
    }

    public static boolean IsValidLanguageTag(String str) {
      int index = 0;
      int endIndex = str.length();
      int startIndex = index;
      if (index + 1<endIndex) {
        char c1 = str.charAt(index);
        char c2 = str.charAt(index + 1);
        if (
          ((c1>= 'A' && c1<= 'Z') || (c1>= 'a' && c1<= 'z')) &&
          ((c2>= 'A' && c2<= 'Z') || (c2>= 'a' && c2<= 'z'))
) {
          index+=2;
          if (index == endIndex) {
 return true;  // case AA
}
          index+=2;
          // convert the language tag to lower case
          // to simplify handling
          str = ParserUtility.ToLowerCaseAscii(str);
          c1 = str.charAt(index);
          // Straightforward cases
          if (c1>= 'a' && c1<= 'z') {
            ++index;
            // case AAA
            if (index == endIndex) {
 return true;
}
            c1 = str.charAt(index);  // get the next character
          }
          if (c1=='-') { // case AA- or AAA-
            ++index;
            if (index + 2 == endIndex) {  // case AA-?? or AAA-??
              c1 = str.charAt(index);
              c2 = str.charAt(index);
              if ((c1>= 'a' && c1<= 'z') && (c2>= 'a' && c2<= 'z')) {
 return true;  // case AA-BB or AAA-BB
}
            }
          }
          // match grandfathered language tags
          if (str.equals("sgn-be-fr") || str.equals("sgn-be-nl") || str.equals("sgn-ch-de") ||
             str.equals("en-gb-oed")) {
 return true;
}
          // More complex cases
          String[] splitString = null;  // TODO: Add splitAt
          //StringUtility.splitAt(str.substring(startIndex,endIndex),"-");
          if (splitString.length == 0) {
 return false;
}
          int splitIndex = 0;
          int splitLength = splitString.length;
          int len = lengthIfAllAlpha(splitString[splitIndex]);
          if (len<2 || len>8) {
 return false;
}
          if (len == 2 || len == 3) {
            ++splitIndex;
            // skip optional extended language subtags
            for (int i = 0; i < 3; ++i) {
              if (splitIndex<splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 3) {
                if (i >= 1)
                  // point 4 in section 2.2.2 renders two or
                  // more extended language subtags invalid
                  return false;
                ++splitIndex;
              } else {
                break;
              }
            }
          }
          // optional script
          if (splitIndex<splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 4) {
            ++splitIndex;
          }
          // optional region
          if (splitIndex<splitLength && lengthIfAllAlpha(splitString[splitIndex]) == 2) {
            ++splitIndex;
          } else if (splitIndex<splitLength && lengthIfAllDigit(splitString[splitIndex]) == 3) {
            ++splitIndex;
          }
          // variant, any number
          List<String> variants = null;
          while (splitIndex<splitLength) {
            String curString = splitString[splitIndex];
            len = lengthIfAllAlphaNum(curString);
            if (len >= 5 && len <= 8) {
              if (variants == null) {
                variants = new ArrayList<String>();
              }
              if (!variants.Contains(curString)) {
                variants.add(curString);
              } else {
 return false;  // variant already exists; see point 5 in section 2.2.5
}
              ++splitIndex;
            } else if (len==4 && (curString.charAt(0)>= '0' && curString.charAt(0)<= '9')) {
              if (variants == null) {
                variants = new ArrayList<String>();
              }
              if (!variants.Contains(curString)) {
                variants.add(curString);
              } else {
 return false;  // variant already exists; see point 5 in section 2.2.5
}
              ++splitIndex;
            } else {
              break;
            }
          }
          // extension, any number
          if (variants != null) {
            variants.Clear();
          }
          while (splitIndex<splitLength) {
            String curString = splitString[splitIndex];
            int curIndex = splitIndex;
            if (lengthIfAllAlphaNum(curString) == 1 &&
               !curString.equals("x")) {
              if (variants == null) {
                variants = new ArrayList<String>();
              }
              if (!variants.Contains(curString)) {
                variants.add(curString);
              } else {
 return false;  // extension already exists
}
              ++splitIndex;
              boolean havetoken = false;
              while (splitIndex<splitLength) {
                curString = splitString[splitIndex];
                len = lengthIfAllAlphaNum(curString);
                if (len >= 2 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
                break;
              }
            } else {
              break;
            }
          }
          // optional private use
          if (splitIndex<splitLength) {
            int curIndex = splitIndex;
            if (splitString[splitIndex].equals("x")) {
              ++splitIndex;
              boolean havetoken = false;
              while (splitIndex<splitLength) {
                len = lengthIfAllAlphaNum(splitString[splitIndex]);
                if (len >= 1 && len <= 8) {
                  havetoken = true;
                  ++splitIndex;
                } else {
                  break;
                }
              }
              if (!havetoken) {
                splitIndex = curIndex;
              }
            }
          }
          // check if all the tokens were used
          return (splitIndex == splitLength);
        } else if (c2=='-' && (c1=='x' || c1=='X')) {
          // private use
          ++index;
          while (index<endIndex) {
            int count = 0;
            if (str.charAt(index)!='-') {
 return false;
}
            ++index;
            while (index<endIndex) {
              c1 = str.charAt(index);
              if ((c1>= 'A' && c1<= 'Z') || (c1>= 'a' && c1<= 'z') || (c1>= '0' && c1<= '9')) {
                ++count;
                if (count>8) {
 return false;
}
              } else if (c1=='-') {
                break;
              } else {
 return false;
}
              ++index;
            }
            if (count< 1) {
 return false;
}
          }
          return true;
        } else if (c2=='-' && (c1=='i' || c1=='I')) {
          // grandfathered language tags
          str = ToLowerCaseAscii(str);
          return (str.equals("i-ami") || str.equals("i-bnn") ||
                  str.equals("i-default") || str.equals("i-enochian") ||
                  str.equals("i-hak") || str.equals("i-klingon") ||
                  str.equals("i-lux") || str.equals("i-navajo") ||
                  str.equals("i-mingo") || str.equals("i-pwn") ||
                  str.equals("i-tao") || str.equals("i-tay") ||
                  str.equals("i-tsu"));
        } else {
 return false;
}
      } else {
 return false;
}
    }

    private static int lengthIfAllAlpha(String str) {
      int len=(str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1>= 'A' && c1<= 'Z') || (c1>= 'a' && c1<= 'z'))) {
 return 0;
}
      }
      return len;
    }

    private static int lengthIfAllAlphaNum(String str) {
      int len=(str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!((c1>= 'A' && c1<= 'Z') || (c1>= 'a' && c1<= 'z') || (c1>= '0' && c1<= '9'))) {
 return 0;
}
      }
      return len;
    }

    private static int lengthIfAllDigit(String str) {
      int len=(str == null) ? 0 : str.length();
      for (int i = 0; i < len; ++i) {
        char c1 = str.charAt(i);
        if (!(c1>= '0' && c1<= '9')) {
 return 0;
}
      }
      return len;
    }
  }
