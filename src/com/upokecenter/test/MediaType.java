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

  final class MediaType {
    private String topLevelType;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getTopLevelType() {
        return topLevelType;
      }
    private String subType;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getSubType() {
        return subType;
      }

    internal MediaType(String type, String subtype, Map<String, String> parameters) {
      topLevelType = type;
      this.subType = subtype;
      parameters = new TreeMap<String, String>(parameters);
    }

    TreeMap<String, String> parameters;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public Map<String, String> Parameters {
      get {
        // TODO: Make read-only
        return parameters;
      }
    }

    enum QuotedStringRule {
      Http,
      Rfc5322,
      Smtp  // RFC5321
    }

    private static int skipQtextOrQuotedPair(
      String s,
      int index,
      int endIndex,
      QuotedStringRule rule) {
      if (index >= endIndex) {
        return index;
      }
      int i2;
      if (rule == QuotedStringRule.Http) {
        char c = s.charAt(index);
        if (c<0x100 && c>= 0x21 && c!='\\' && c!='"') {
          return index + 1;
        }
        i2 = skipQuotedPair(s, index, endIndex);
        if (index != i2) {
          return i2;
        }
        return i2;
      } else if (rule == QuotedStringRule.Rfc5322) {
        i2 = index;
        // qtext (RFC5322 sec. 3.2.1)
        if (i2<endIndex) {
          char c = s.charAt(i2);
          if (c>= 33 && c<= 126 && c!='\\' && c!='"') {
            ++i2;
          }
          // obs-qtext (same as obs-ctext)
          if ((c<0x20 && c != 0x00 && c != 0x09 && c != 0x0a && c != 0x0d)  || c == 0x7f) {
            ++i2;
          }
        }
        if (index != i2) {
          return i2;
        }
        index = i2;
        i2 = skipQuotedPair(s, index, endIndex);
        if (index != i2) {
          return i2;
        }
        return i2;
      } else if (rule == QuotedStringRule.Smtp) {
        char c = s.charAt(index);
        if (c>= 0x20 && c<= 0x7E && c!='\\' && c!='"') {
          return index + 1;
        }
        i2 = skipQuotedPairSMTP(s, index, endIndex);
        if (index != i2) {
          return i2;
        }
        return i2;
      } else {
        throw new IllegalArgumentException(rule.toString());
      }
    }

    // quoted-pair (RFC5322 sec. 3.2.1)
    static int skipQuotedPair(String s, int index, int endIndex) {
      if (index+1<endIndex && s.charAt(index)=='\\') {
        char c = s.charAt(index + 1);
        if (c == 0x20 || c == 0x09 || (c >= 0x21 && c <= 0x7e)) {
          return index + 2;
        }
        // obs-qp
        if ((c<0x20 && c != 0x09)  || c == 0x7f) {
          return index + 2;
        }
      }
      return index;
    }

    static int skipQuotedPairSMTP(String s, int index, int endIndex) {
      if (index+1<endIndex && s.charAt(index)=='\\') {
        char c = s.charAt(index + 1);
        if (c >= 0x20 && c <= 0x7e) {
          return index + 2;
        }
      }
      return index;
    }
    // quoted-String (RFC5322 sec. 3.2.4)
    static int skipQuotedString(String s, int index,
                                         int endIndex, StringBuilder builder) {
      return skipQuotedString(s, index, endIndex, builder, QuotedStringRule.Rfc5322);
    }

    static int skipQuotedString(
      String str,
      int index,
      int endIndex,
      StringBuilder builder,  // receives the unescaped version of the _string
      QuotedStringRule rule) {
      int startIndex = index;
      int bLength=(builder == null) ? 0 : builder.length();
      index=(rule != QuotedStringRule.Rfc5322) ? index :
        Message.SkipCommentsAndWhitespace(str, index, endIndex);
      if (!(index<endIndex && str.charAt(index)=='"')) {
        if (builder != null) {
          builder.length()=(bLength);
        }
        return startIndex;  // not a valid quoted-String
      }
      ++index;
      while (index<endIndex) {
        int i2 = index;
        if (rule == QuotedStringRule.Http) {
          i2 = skipLws(str, index, endIndex);
          if (i2 != index) {
            builder.append(' ');
          }
        } else if (rule == QuotedStringRule.Rfc5322) {
          // Skip tabs and spaces (should skip
          // folding whitespace too, but this method assumes
          // unfolded values)
          i2 = ParserUtility.SkipSpaceAndTab(str, index, endIndex);
          if (i2 != index) {
            builder.append(' ');
          }
        }
        index = i2;
        char c = str.charAt(index);
        if (c=='"') { // end of quoted-String
          ++index;
          if (rule == QuotedStringRule.Rfc5322) {
            return Message.SkipCommentsAndWhitespace(str, index, endIndex);
          } else {
            return index;
          }
        }
        int oldIndex = index;
        index = skipQtextOrQuotedPair(str, index, endIndex, rule);
        if (index == oldIndex) {
          if (builder != null) {
            builder.delete(bLength,(bLength)+((builder.length())-(bLength)));
          }
          return startIndex;
        }
        if (builder != null) {
          // this is a qtext or quoted-pair, so
          // append the last character read
          builder.append(str.charAt(index-1));
        }
      }
      if (builder != null) {
        builder.delete(bLength,(bLength)+((builder.length())-(bLength)));
      }
      return startIndex;  // not a valid quoted-String
    }

    private static int SafeSplit(String str, int index) {
      if (str == null) {
        return 0;
      }
      if (index< 0) {
        return 0;
      }
      if (index>str.length()) {
        return str.length();
      }
      if (index>0 && str.charAt(index)>= 0xdc00  && str.charAt(index)<= 0xdfff &&
          str.charAt(index-1)>= 0xd800 && str.charAt(index-1)<= 0xdbff) {
        // Avoid splitting legal surrogate pairs
        return index-1;
      }
      return index;
    }

    private static void AppendComplexParamValue(String name, String str, StringBuilder sb) {
      int length = 1;
      int contin = 0;
      String hex="0123456789ABCDEF";
      length+=name.length() + 12;
      int maxLength = 76;
      if (sb.length() + name.length() + 9 + str.length()*3 <= maxLength) {
        // Very short
        length = sb.length() + name.length() + 9;
        sb.append(name+"*=utf-8''");
      } else if (length + str.length()*3 <= maxLength) {
        // Short enough that no continuations
        // are needed
        length-=2;
        sb.append("\r\n ");
        sb.append(name+"*=utf-8''");
      } else {
        sb.append("\r\n ");
        sb.append(name+"*0*=utf-8''");
      }
      boolean first = true;
      int index = 0;
      while (index<str.length()) {
        int c = str.charAt(index);
        if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.length() &&
            str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) * 0x400) + (str.charAt(index + 1) - 0xdc00);
          ++index;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++index;
        if ((c>= 33 && c<= 126 && "()<>,;[]:@\"\\/?=*%'".indexOf((char)c) < 0)) {
          ++length;
          if (!first && length + 1>maxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString=name+"*"+
              Integer.toString((int)contin)+
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            ++length;
          }
          first = false;
          sb.append((char)c);
        } else if (c<0x80) {
          length+=3;
          if (!first && length + 1>maxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString=name+"*"+
              Integer.toString((int)contin)+
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length+=3;
          }
          first = false;
          sb.append((char)c);
        } else if (c<0x800) {
          length+=6;
          if (!first && length + 1>maxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString=name+"*"+
              Integer.toString((int)contin)+
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length+=6;
          }
          first = false;
          int w= (byte)(0xc0 | ((c >> 6) & 0x1f));
          int x = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(hex.charAt(w >> 4));
          sb.append(hex.charAt(w & 15));
          sb.append('%');
          sb.append(hex.charAt(x >> 4));
          sb.append(hex.charAt(x & 15));
        } else if (c<0x10000) {
          length+=9;
          if (!first && length + 1>maxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString=name+"*"+
              Integer.toString((int)contin)+
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length+=9;
          }
          first = false;
          int w = (byte)(0xe0 | ((c >> 12) & 0x0f));
          int x = (byte)(0x80 | ((c >> 6) & 0x3f));
          int y = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(hex.charAt(w >> 4));
          sb.append(hex.charAt(w & 15));
          sb.append('%');
          sb.append(hex.charAt(x >> 4));
          sb.append(hex.charAt(x & 15));
          sb.append('%');
          sb.append(hex.charAt(y >> 4));
          sb.append(hex.charAt(y & 15));
        } else {
          length+=12;
          if (!first && length + 1>maxLength) {
            sb.append(";\r\n ");
            first = true;
            ++contin;
            String continString=name+"*"+
              Integer.toString((int)contin)+
              "*=";
            sb.append(continString);
            length = 1 + continString.length();
            length+=12;
          }
          first = false;
          int w = (byte)(0xf0 | ((c >> 18) & 0x07));
          int x = (byte)(0x80 | ((c >> 12) & 0x3f));
          int y = (byte)(0x80 | ((c >> 6) & 0x3f));
          int z = (byte)(0x80 | (c & 0x3f));
          sb.append('%');
          sb.append(hex.charAt(w >> 4));
          sb.append(hex.charAt(w & 15));
          sb.append('%');
          sb.append(hex.charAt(x >> 4));
          sb.append(hex.charAt(x & 15));
          sb.append('%');
          sb.append(hex.charAt(y >> 4));
          sb.append(hex.charAt(y & 15));
          sb.append('%');
          sb.append(hex.charAt(z >> 4));
          sb.append(hex.charAt(z & 15));
        }
      }
    }

    private static boolean AppendSimpleParamValue(String name, String str, StringBuilder sb) {
      sb.append(name);
      sb.append('=');
      boolean simple = true;
      for (int i = 0;i<str.length(); ++i) {
        char c = str.charAt(i);
        if (!(c>= 33 && c<= 126 && "()<>,;[]:@\"\\/?=".indexOf(c) < 0)) {
          simple = false;
        }
      }
      if (simple) {
        sb.append(str);
        return true;
      }
      sb.append('"');
      for (int i = 0;i<str.length(); ++i) {
        char c = str.charAt(i);
        if (c >= 32 && c <= 126) {
          sb.append(c);
        } else if (c==0x20 || c==0x09 || c=='\\' || c=='"') {
          sb.append('\\');
          sb.append(c);
        } else {
          // Requires complex encoding
          return false;
        }
      }
      sb.append('"');
      return true;
    }

    private static void AppendParamValue(String name, String str, StringBuilder sb) {
      int sbStart = sb.length();
      if (!AppendSimpleParamValue(name, str, sb)) {
        sb.length() = sbStart;
        AppendComplexParamValue(name, str, sb);
        return;
      }
      if (sb.length()>76) {
        sb.length() = sbStart;
        sb.append("\r\n ");
        int sbStart2 = sb.length()-1;
        AppendSimpleParamValue(name, str, sb);
        if (sb.length()-sbStart2>76) {
          sb.length() = sbStart;
          AppendComplexParamValue(name, str, sb);
        }
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = new StringBuilder();
      sb.append(topLevelType);
      sb.append('/');
      sb.append(subType);
      for(String key : parameters.keySet()) {
        sb.append(';');
        AppendParamValue(key, parameters.get(key), sb);
      }
      return sb.toString();
    }

    static int skipMimeToken(String str, int index, int endIndex,
                                      StringBuilder builder, boolean httpRules) {
      int i = index;
      while (i<endIndex) {
        char c = str.charAt(i);
        if (c<= 0x20 || c>= 0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=".indexOf(c)>= 0)) {
          break;
        }
        if (httpRules && (c=='{' || c=='}')) {
          break;
        }
        if (builder != null) {
          builder.append(c);
        }
        ++i;
      }
      return i;
    }

    static int skipAttributeNameRfc2231(String str, int index, int endIndex,
                                                 StringBuilder builder, boolean httpRules) {
      if (httpRules) {
        return skipMimeToken(str, index, endIndex, builder, httpRules);
      }
      int i = index;
      while (i<endIndex) {
        char c = str.charAt(i);
        if (c <= 0x20 || c >= 0x7f ||
            ((c&0x7F)==c && "()<>@,;:\\\"/[]?='%*".indexOf(c)>= 0)) {
          break;
        }
        if (builder != null) {
          builder.append(c);
        }
        ++i;
      }
      if (i+1<endIndex && str.charAt(i)=='*' && str.charAt(i+1)=='0') {
        // initial-section
        i+=2;
        if (builder != null) {
          builder.append("*0");
        }
        if (i<endIndex && str.charAt(i)=='*') {
          ++i;
          if (builder != null) {
            builder.append("*");
          }
        }
        return i;
      }
      if (i+1<endIndex && str.charAt(i)=='*' && str.charAt(i+1)>= '1' && str.charAt(i+1)<= '9') {
        // other-sections
        if (builder != null) {
          builder.append('*');
          builder.append(str.charAt(i + 1));
        }
        i+=2;
        while (i<endIndex && str.charAt(i)>= '0' && str.charAt(i)<= '9') {
          if (builder != null) {
            builder.append(str.charAt(i));
          }
          ++i;
        }
        if (i<endIndex && str.charAt(i)=='*') {
          if (builder != null) {
            builder.append(str.charAt(i));
          }
          ++i;
        }
        return i;
      }
      if (i<endIndex && str.charAt(i)=='*') {
        if (builder != null) {
          builder.append(str.charAt(i));
        }
        ++i;
      }
      return i;
    }

    static int skipMimeTokenRfc2047(String str, int index, int endIndex) {
      int i = index;
      while (i<endIndex) {
        char c = str.charAt(i);
        if (c<= 0x20 || c>= 0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=.".indexOf(c)>= 0)) {
          break;
        }
        ++i;
      }
      return i;
    }
    static int skipEncodedTextRfc2047(String str, int index, int endIndex, boolean inComments) {
      int i = index;
      while (i<endIndex) {
        char c = str.charAt(i);
        if (c<= 0x20 || c>= 0x7F || c=='?') {
          break;
        }
        if (inComments && (c=='(' || c==')' || c=='\\')) {
          break;
        }
        ++i;
      }
      return i;
    }
    static int skipMimeTypeSubtype(String str, int index, int endIndex, StringBuilder builder) {
      int i = index;
      int count = 0;
      while (i<str.length()) {
        char c = str.charAt(i);
        // See RFC6838
        if ((c>= 'A' && c<= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9')) {
          if (builder != null) {
            builder.append(c);
          }
          ++i;
          ++count;
        } else if (count>0 && ((c&0x7F)==c && "!#$&-^_.+".indexOf(c)>= 0)) {
          if (builder != null) {
            builder.append(c);
          }
          ++i;
          ++count;
        } else {
          break;
        }
        // type or subtype too long
        if (count>127) {
          return index;
        }
      }
      return i;
    }

    /**
     * Returns the charset parameter, converted to ASCII lower-case, if
     * it exists, or "us-ascii" if the media type is ill-formed (RFC2045
     * sec. 5.2), or if the media type is "text/plain" or "text/xml" and doesn't
     * have a charset parameter (see RFC2046 and RFC3023, respectively),
     * or the empty string otherwise.
     * @return A string object.
     */
    public String GetCharset() {
      String param=GetParameter("charset");
      if (param != null) {
        return ParserUtility.ToLowerCaseAscii(param);
      }
      if (topLevelType.equals("text")) {
        if (subType.equals("xml") || subType.equals("plain")) {
          return "us-ascii";
        }
        if (subType.equals("csv")) {
          // see RFC 7111 sec. 5.1
          return "utf-8";
        }
        if (subType.equals("vnd.graphviz")) {
          // see http://iana.org/assignments/media-types/text/vnd.graphviz
          return "utf-8";
        }
      }
      return "";
    }

    /**
     * Not documented yet.
     * @param name A string object. (2).
     * @return A string object.
     */
    public String GetParameter(String name) {
      if ((name) == null) {
        throw new NullPointerException("name");
      }
      if ((name).length == 0) {
        throw new IllegalArgumentException("name is empty.");
      }
      name = ParserUtility.ToLowerCaseAscii(name);
      if (parameters.containsKey(name)) {
        return parameters.get(name);
      }
      return null;
    }

    private static String DecodeRfc2231Extension(String value) {
      int firstQuote=value.indexOf('\'');
      if (firstQuote< 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote=value.IndexOf('\'',firstQuote+1);
      if (secondQuote< 0) {
        // not a valid encoded parameter
        return null;
      }
      String charset = value.substring(0,firstQuote);
      // NOTE: Ignored
      // String language = value.substring(firstQuote + 1,(firstQuote + 1)+(secondQuote-(firstQuote + 1)));
      String paramValue = value.substring(secondQuote + 1);
      Charsets.ICharset cs = Charsets.GetCharset(charset);
      if (cs == null) {
        cs = Charsets.Ascii;
      }
      return DecodeRfc2231Encoding(paramValue, cs);
    }

    private static Charsets.ICharset GetRfc2231Charset(String value) {
      if (value == null) {
        return Charsets.Ascii;
      }
      int firstQuote=value.indexOf('\'');
      if (firstQuote< 0) {
        // not a valid encoded parameter
        return Charsets.Ascii;
      }
      int secondQuote=value.IndexOf('\'',firstQuote+1);
      if (secondQuote< 0) {
        // not a valid encoded parameter
        return Charsets.Ascii;
      }
      String charset = value.substring(0,firstQuote);
      // NOTE: Ignored
      // String language = value.substring(firstQuote + 1,(firstQuote + 1)+(secondQuote-(firstQuote + 1)));
      Charsets.ICharset cs = Charsets.GetCharset(charset);
      if (cs == null) {
        cs = Charsets.Ascii;
      }
      return cs;
    }

    private static String DecodeRfc2231Encoding(String value, Charsets.ICharset charset) {
      return charset.GetString(new Message.PercentEncodingStringTransform(value));
    }

    private boolean ExpandRfc2231Extensions() {
      if (parameters.size() == 0) {
        return true;
      }
      List<String> keyList = new ArrayList<String>(parameters.keySet());
      for(String name : keyList) {
        if (!parameters.containsKey(name)) {
          continue;
        }
        String value = parameters.get(name);
        int asterisk=name.indexOf('*');
        if (asterisk == name.length()-1 && asterisk>0) {
          // name*="value" (except when the parameter is just "*")
          String realName = name.substring(0,name.length()-1);
          String realValue = DecodeRfc2231Extension(value);
          if (realValue == null) {
            continue;
          }
          parameters.Remove(name);
          parameters.put(realName,realValue);
          continue;
        }
        // name*0 or name*0*
        if (asterisk > 0 &&
            ((asterisk==name.length()-2 && name.charAt(asterisk+1)=='0') ||
             (asterisk==name.length()-3 && name.charAt(asterisk+1)=='0' && name.charAt(asterisk+2)=='*')
)) {
          String realName = name.substring(0,asterisk);
          String realValue=(asterisk == name.length()-3) ? DecodeRfc2231Extension(value) :
            value;
          Charsets.ICharset charsetUsed = GetRfc2231Charset(
            (asterisk == name.length()-3) ? value : null);
          if (realValue == null) {
            realValue = value;
          }
          int pindex = 1;
          // search for name*1 or name*1*, then name*2 or name*2*,
          // and so on
          while (true) {
            String contin=realName+"*"+
              (pindex).toString();
            String continEncoded=contin+"*";
            boolean found = false;
            for(String keyJ : new ArrayList<String>(parameters.keySet())) {
              if (keyJ.equals(contin)) {
                // Unencoded continuation
                realValue+=parameters.get(keyJ);
                parameters.Remove(keyJ);
                found = true;
                break;
              }
              if (keyJ.equals(continEncoded)) {
                // Encoded continuation
                realValue+=DecodeRfc2231Encoding(parameters.get(keyJ), charsetUsed);
                parameters.Remove(keyJ);
                found = true;
                break;
              }
            }
            if (!found) {
              break;
            }
            ++pindex;
          }
          parameters.put(realName,realValue);
        }
      }
      for(String name : parameters.keySet()) {
        // Check parameter names using stricter format
        // in RFC6838
        if (skipMimeTypeSubtype(name, 0, name.length(), null) != name.length()) {
          // Illegal parameter name, so use default media type
          return false;
        }
      }
      return true;
    }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public String getTypeAndSubType() {
        return TopLevelType + "/" + SubType;
      }

    static int skipLws(String s, int index, int endIndex) {
      // While HTTP usually only allows CRLF, it also allows
      // us to be tolerant here
      int i2 = index;
      if (i2 + 1<endIndex && s.charAt(i2)==0x0d && s.charAt(i2)==0x0a) {
        i2+=2;
      } else if (i2<endIndex && (s.charAt(i2)==0x0d || s.charAt(i2)==0x0a)) {
        ++index;
      }
      while (i2<endIndex) {
        if (s.charAt(i2)==0x09||s.charAt(i2)==0x20) {
          ++index;
        }
        break;
      }
      return index;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return A Boolean object.
     */
    public boolean ParseMediaType(String str) {
      boolean httpRules = false;
      int index = 0;
      int endIndex = str.length();
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (httpRules) {
        index = skipLws(str, index, endIndex);
      } else {
        index = Message.SkipCommentsAndWhitespace(str, index, endIndex);
      }
      int i = skipMimeTypeSubtype(str, index, endIndex, null);
      if (i==index || i>= endIndex || str.charAt(i)!='/') {
        return false;
      }
      this.topLevelType = ParserUtility.ToLowerCaseAscii(str.substring(index,(index)+(i-index)));
      ++i;
      int i2 = skipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        return false;
      }
      this.subType = ParserUtility.ToLowerCaseAscii(str.substring(i,(i)+(i2-i)));
      if (i2<endIndex) {
        // if not at end
        int i3 = Message.SkipCommentsAndWhitespace(str, i2, endIndex);
        if (i3==endIndex || (i3<endIndex && str.charAt(i3)!=';' && str.charAt(i3)!=',')) {
          // at end, or not followed by ";" or ",", so not a media type
          return false;
        }
      }
      index = i2;
      int indexAfterTypeSubtype = index;
      while (true) {
        // RFC5322 uses skipCommentsAndWhitespace when skipping whitespace;
        // HTTP currently uses skipLws, though that may change
        // to skipWsp in a future revision of HTTP
        if (httpRules) {
          indexAfterTypeSubtype = skipLws(str, indexAfterTypeSubtype, endIndex);
        } else {
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(
            str,
            indexAfterTypeSubtype,
            endIndex);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            return ExpandRfc2231Extensions();
          }
          return true;
        }
        if (str.charAt(indexAfterTypeSubtype)!=';') {
          return false;
        }
        ++indexAfterTypeSubtype;
        if (httpRules) {
          indexAfterTypeSubtype = skipLws(str, indexAfterTypeSubtype, endIndex);
        } else {
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(
            str,
            indexAfterTypeSubtype,
            endIndex);
        }
        StringBuilder builder = new StringBuilder();
        // NOTE: RFC6838 restricts the format of parameter names to the same
        // ((syntax instanceof types and subtypes) ? (types and subtypes)syntax : null), but this syntax is incompatible with
        // the RFC2231 format
        int afteratt = skipAttributeNameRfc2231(
          str,
          indexAfterTypeSubtype,
          endIndex,
          builder,
          httpRules);
        if (afteratt == indexAfterTypeSubtype) {  // ill-formed attribute
          return false;
        }
        String attribute = builder.toString();
        indexAfterTypeSubtype = afteratt;
        if (!httpRules) {
          // NOTE: MIME implicitly doesn't restrict whether whitespace can appear
          // around the equal sign separating an attribute and value, while
          // HTTP explicitly forbids such whitespace
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(
            str,
            indexAfterTypeSubtype,
            endIndex);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          return false;
        }
        if (str.charAt(indexAfterTypeSubtype)!='=') {
          return false;
        }
        attribute = ParserUtility.ToLowerCaseAscii(attribute);
        if (parameters.containsKey(attribute)) {
          System.out.println("Contains duplicate attribute "+attribute);
          return false;
        }
        ++indexAfterTypeSubtype;
        if (!httpRules) {
          // See note above on whitespace around the equal sign
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(
            str,
            indexAfterTypeSubtype,
            endIndex);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            return ExpandRfc2231Extensions();
          }
          return true;
        }
        builder.setLength(0);
        int qs;
        // If the attribute name ends with '*' the value may not be a quoted String
        if (attribute.charAt(attribute.length()-1)!='*') {
          // try getting the value quoted
          qs = skipQuotedString(
            str,
            indexAfterTypeSubtype,
            endIndex,
            builder,
            httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
          if (qs != indexAfterTypeSubtype) {
            parameters.put(attribute,(builder.toString()));
            indexAfterTypeSubtype = qs;
            continue;
          }
          builder.setLength(0);
        }
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = skipMimeToken(str, indexAfterTypeSubtype, endIndex, builder, httpRules);
        if (qs != indexAfterTypeSubtype) {
          parameters.put(attribute,(builder.toString()));
          indexAfterTypeSubtype = qs;
          continue;
        }
        // no valid value, return
        return false;
      }
    }

    public static MediaType TextPlainAscii =
      new MediaTypeBuilder("text","plain").SetParameter("charset","us-ascii").ToMediaType();
    public static MediaType TextPlainUtf8 =
      new MediaTypeBuilder("text","plain").SetParameter("charset","utf-8").ToMediaType();
    public static MediaType MessageRfc822 =
      new MediaTypeBuilder("message","rfc822").ToMediaType();
    public static MediaType ApplicationOctetStream =
      new MediaTypeBuilder("application","octet-stream").ToMediaType();

    private MediaType() {
    }

    public static MediaType Parse(String str) {
      return Parse(str, TextPlainAscii);
    }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param defaultValue Can be null.
     * @return A MediaType object.
     */
    public static MediaType Parse(String str, MediaType defaultValue) {
      if ((str) == null) {
        throw new NullPointerException("str");
      }
      MediaType mt = new MediaType();
      mt.parameters = new TreeMap<String, String>();
      if (!mt.ParseMediaType(str)) {

        return defaultValue;
      }
      return mt;
    }
  }
