/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CBORTest
{
  internal sealed class MediaType {
    private string topLevelType;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string TopLevelType {
      get {
        return this.topLevelType;
      }
    }

    private string subType;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string SubType {
      get {
        return this.subType;
      }
    }

    internal MediaType(string type, string subtype, IDictionary<string, string> parameters) {
      this.topLevelType = type;
      this.subType = subtype;
      this.parameters = new SortedDictionary<string, string>(parameters);
    }

    private SortedDictionary<string, string> parameters;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IDictionary<string, string> Parameters {
      get {
        // TODO: Make read-only
        return this.parameters;
      }
    }

    internal enum QuotedStringRule {
      Http,
      Rfc5322
    }

    private static int skipQtextOrQuotedPair(
      string s,
      int index,
      int endIndex,
      QuotedStringRule rule) {
      if (index >= endIndex) {
        return index;
      }
      int i2;
      if (rule == QuotedStringRule.Http) {
        char c = s[index];
        if (c < 0x100 && c >= 0x21 && c!='\\' && c!='"') {
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
        if (i2 < endIndex) {
          char c = s[i2];
          if (c >= 33 && c <= 126 && c!='\\' && c!='"') {
            ++i2;
          }
          // obs-qtext (same as obs-ctext)
          if ((c < 0x20 && c != 0x00 && c != 0x09 && c != 0x0a && c != 0x0d) || c == 0x7f) {
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
      } else {
        throw new ArgumentException(rule.ToString());
      }
    }

    // quoted-pair (RFC5322 sec. 3.2.1)
    internal static int skipQuotedPair(string s, int index, int endIndex) {
      if (index + 1 < endIndex && s[index]=='\\') {
        char c = s[index + 1];
        if (c == 0x20 || c == 0x09 || (c >= 0x21 && c <= 0x7e)) {
          return index + 2;
        }
        // obs-qp
        if ((c < 0x20 && c != 0x09) || c == 0x7f) {
          return index + 2;
        }
      }
      return index;
    }

    // quoted-string (RFC5322 sec. 3.2.4)
    internal static int skipQuotedString(
      string s,
      int index,
      int endIndex,
      StringBuilder builder) {
      return skipQuotedString(s, index, endIndex, builder, QuotedStringRule.Rfc5322);
    }

    internal static int skipQuotedString(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,  // receives the unescaped version of the _string
      QuotedStringRule rule) {
      int startIndex = index;
      int valueBLength = (builder == null) ? 0 : builder.Length;
      index = (rule != QuotedStringRule.Rfc5322) ? index :
        HeaderParser.ParseCFWS(str, index, endIndex, null);
      if (!(index < endIndex && str[index] == '"')) {
        if (builder != null) {
          builder.Length = valueBLength;
        }
        return startIndex;  // not a valid quoted-string
      }
      ++index;
      while (index < endIndex) {
        int i2 = index;
        if (rule == QuotedStringRule.Http) {
          i2 = skipLws(str, index, endIndex);
          if (i2 != index) {
            builder.Append(' ');
          }
        } else if (rule == QuotedStringRule.Rfc5322) {
          // Skip tabs, spaces, and folding whitespace
          i2 = HeaderParser.ParseFWS(str, index, endIndex, null);
          if (i2 != index) {
            builder.Append(' ');
          }
        }
        index = i2;
        char c = str[index];
        if (c == '"') { // end of quoted-string
          ++index;
          if (rule == QuotedStringRule.Rfc5322) {
            return HeaderParser.ParseCFWS(str, index, endIndex, null);
          } else {
            return index;
          }
        }
        int oldIndex = index;
        index = skipQtextOrQuotedPair(str, index, endIndex, rule);
        if (index == oldIndex) {
          if (builder != null) {
            builder.Remove(valueBLength, (builder.Length)-valueBLength);
          }
          return startIndex;
        }
        if (builder != null) {
          // this is a qtext or quoted-pair, so
          // append the last character read
          builder.Append(str[index - 1]);
        }
      }
      if (builder != null) {
        builder.Remove(valueBLength, (builder.Length)-valueBLength);
      }
      return startIndex;  // not a valid quoted-string
    }

    private static int SafeSplit(string str, int index) {
      if (str == null) {
        return 0;
      }
      if (index < 0) {
        return 0;
      }
      if (index > str.Length) {
        return str.Length;
      }
      if (index > 0 && str[index] >= 0xdc00 && str[index]<= 0xdfff &&
          str[index - 1] >= 0xd800 && str[index-1]<= 0xdbff) {
        // Avoid splitting legal surrogate pairs
        return index - 1;
      }
      return index;
    }

    private static void AppendComplexParamValue(string name, string str, StringBuilder sb) {
      int length = 1;
      int contin = 0;
      string hex = "0123456789ABCDEF";
      length += name.Length + 12;
      int maxLength = 76;
      if (sb.Length + name.Length + 9 + str.Length * 3 <= maxLength) {
        // Very short
        length = sb.Length + name.Length + 9;
        sb.Append(name + "*=utf-8''");
      } else if (length + str.Length * 3 <= maxLength) {
        // Short enough that no continuations
        // are needed
        length -= 2;
        sb.Append("\r\n ");
        sb.Append(name + "*=utf-8''");
      } else {
        sb.Append("\r\n ");
        sb.Append(name + "*0*=utf-8''");
      }
      bool first = true;
      int index = 0;
      while (index < str.Length) {
        int c = str[index];
        if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.Length &&
            str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
          // Get the Unicode code point for the surrogate pair
          c = 0x10000 + ((c - 0xd800) * 0x400) + (str[index + 1] - 0xdc00);
          ++index;
        } else if (c >= 0xd800 && c <= 0xdfff) {
          // unpaired surrogate
          c = 0xfffd;
        }
        ++index;
        if (c >= 33 && c<= 126 && "()<>,;[]:@\"\\/?=*%'".IndexOf((char)c) < 0) {
          ++length;
          if (!first && length + 1 > maxLength) {
            sb.Append(";\r\n ");
            first = true;
            ++contin;
            string continString = name + "*"+
              Convert.ToString((int)contin, System.Globalization.CultureInfo.InvariantCulture) +
              "*=";
            sb.Append(continString);
            length = 1 + continString.Length;
            ++length;
          }
          first = false;
          sb.Append((char)c);
        } else if (c < 0x80) {
          length += 3;
          if (!first && length + 1 > maxLength) {
            sb.Append(";\r\n ");
            first = true;
            ++contin;
            string continString = name + "*"+
              Convert.ToString((int)contin, System.Globalization.CultureInfo.InvariantCulture) +
              "*=";
            sb.Append(continString);
            length = 1 + continString.Length;
            length += 3;
          }
          first = false;
          sb.Append((char)c);
        } else if (c < 0x800) {
          length += 6;
          if (!first && length + 1 > maxLength) {
            sb.Append(";\r\n ");
            first = true;
            ++contin;
            string continString = name + "*"+
              Convert.ToString((int)contin, System.Globalization.CultureInfo.InvariantCulture) +
              "*=";
            sb.Append(continString);
            length = 1 + continString.Length;
            length += 6;
          }
          first = false;
          int w = (byte)(0xc0 | ((c >> 6) & 0x1f));
          int x = (byte)(0x80 | (c & 0x3f));
          sb.Append('%');
          sb.Append(hex[w >> 4]);
          sb.Append(hex[w & 15]);
          sb.Append('%');
          sb.Append(hex[x >> 4]);
          sb.Append(hex[x & 15]);
        } else if (c < 0x10000) {
          length += 9;
          if (!first && length + 1 > maxLength) {
            sb.Append(";\r\n ");
            first = true;
            ++contin;
            string continString = name + "*"+
              Convert.ToString((int)contin, System.Globalization.CultureInfo.InvariantCulture) +
              "*=";
            sb.Append(continString);
            length = 1 + continString.Length;
            length += 9;
          }
          first = false;
          int w = (byte)(0xe0 | ((c >> 12) & 0x0f));
          int x = (byte)(0x80 | ((c >> 6) & 0x3f));
          int y = (byte)(0x80 | (c & 0x3f));
          sb.Append('%');
          sb.Append(hex[w >> 4]);
          sb.Append(hex[w & 15]);
          sb.Append('%');
          sb.Append(hex[x >> 4]);
          sb.Append(hex[x & 15]);
          sb.Append('%');
          sb.Append(hex[y >> 4]);
          sb.Append(hex[y & 15]);
        } else {
          length += 12;
          if (!first && length + 1 > maxLength) {
            sb.Append(";\r\n ");
            first = true;
            ++contin;
            string continString = name + "*"+
              Convert.ToString((int)contin, System.Globalization.CultureInfo.InvariantCulture) +
              "*=";
            sb.Append(continString);
            length = 1 + continString.Length;
            length += 12;
          }
          first = false;
          int w = (byte)(0xf0 | ((c >> 18) & 0x07));
          int x = (byte)(0x80 | ((c >> 12) & 0x3f));
          int y = (byte)(0x80 | ((c >> 6) & 0x3f));
          int z = (byte)(0x80 | (c & 0x3f));
          sb.Append('%');
          sb.Append(hex[w >> 4]);
          sb.Append(hex[w & 15]);
          sb.Append('%');
          sb.Append(hex[x >> 4]);
          sb.Append(hex[x & 15]);
          sb.Append('%');
          sb.Append(hex[y >> 4]);
          sb.Append(hex[y & 15]);
          sb.Append('%');
          sb.Append(hex[z >> 4]);
          sb.Append(hex[z & 15]);
        }
      }
    }

    private static bool AppendSimpleParamValue(string name, string str, StringBuilder sb) {
      sb.Append(name);
      sb.Append('=');
      bool simple = true;
      for (int i = 0;i < str.Length; ++i) {
        char c = str[i];
        if (!(c >= 33 && c <= 126 && "()<>,;[]:@\"\\/?=".IndexOf(c) < 0)) {
          simple = false;
        }
      }
      if (simple) {
        sb.Append(str);
        return true;
      }
      sb.Append('"');
      for (int i = 0;i < str.Length; ++i) {
        char c = str[i];
        if (c >= 32 && c <= 126) {
          sb.Append(c);
        } else if (c == 0x20 || c == 0x09 || c=='\\' || c=='"') {
          sb.Append('\\');
          sb.Append(c);
        } else {
          // Requires complex encoding
          return false;
        }
      }
      sb.Append('"');
      return true;
    }

    private static void AppendParamValue(string name, string str, StringBuilder sb) {
      int valueSbStart = sb.Length;
      if (!AppendSimpleParamValue(name, str, sb)) {
        sb.Length = valueSbStart;
        AppendComplexParamValue(name, str, sb);
        return;
      }
      if (sb.Length > 76) {
        sb.Length = valueSbStart;
        sb.Append("\r\n ");
        int valueSbStart2 = sb.Length - 1;
        AppendSimpleParamValue(name, str, sb);
        if (sb.Length - valueSbStart2 > 76) {
          sb.Length = valueSbStart;
          AppendComplexParamValue(name, str, sb);
        }
      }
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.topLevelType);
      sb.Append('/');
      sb.Append(this.subType);
       foreach (string key in this.parameters.Keys) {
        sb.Append(';');
        AppendParamValue(key, this.parameters[key], sb);
      }
      return sb.ToString();
    }

    internal static int skipMimeToken(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,
      bool httpRules) {
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=".IndexOf(c)>= 0)) {
          break;
        }
        if (httpRules && (c == '{' || c == '}')) {
          break;
        }
        if (builder != null) {
          builder.Append(c);
        }
        ++i;
      }
      return i;
    }

    internal static int skipAttributeNameRfc2231(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,
      bool httpRules) {
      if (httpRules) {
        return skipMimeToken(str, index, endIndex, builder, httpRules);
      }
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7f ||
            ((c & 0x7F) ==c && "()<>@,;:\\\"/[]?='%*".IndexOf(c)>= 0)) {
          break;
        }
        if (builder != null) {
          builder.Append(c);
        }
        ++i;
      }
      if (i + 1 < endIndex && str[i]=='*' && str[i+1]=='0') {
        // initial-section
        i += 2;
        if (builder != null) {
          builder.Append("*0");
        }
        if (i < endIndex && str[i] == '*') {
          ++i;
          if (builder != null) {
            builder.Append("*");
          }
        }
        return i;
      }
      if (i + 1 < endIndex && str[i]=='*' && str[i+1]>= '1' && str[i+1]<= '9') {
        // other-sections
        if (builder != null) {
          builder.Append('*');
          builder.Append(str[i + 1]);
        }
        i += 2;
        while (i < endIndex && str[i] >= '0' && str[i]<= '9') {
          if (builder != null) {
            builder.Append(str[i]);
          }
          ++i;
        }
        if (i < endIndex && str[i] == '*') {
          if (builder != null) {
            builder.Append(str[i]);
          }
          ++i;
        }
        return i;
      }
      if (i < endIndex && str[i] == '*') {
        if (builder != null) {
          builder.Append(str[i]);
        }
        ++i;
      }
      return i;
    }

    internal static int skipMimeTokenRfc2047(string str, int index, int endIndex) {
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=.".IndexOf(c)>= 0)) {
          break;
        }
        ++i;
      }
      return i;
    }

    internal static int skipEncodedTextRfc2047(string str, int index, int endIndex, bool inComments) {
      int i = index;
      while (i < endIndex) {
        char c = str[i];
        if (c <= 0x20 || c >= 0x7F || c=='?') {
          break;
        }
        if (inComments && (c == '(' || c == ')' || c=='\\')) {
          break;
        }
        ++i;
      }
      return i;
    }

    internal static int skipMimeTypeSubtype(string str, int index, int endIndex, StringBuilder builder) {
      int i = index;
      int count = 0;
      while (i < str.Length) {
        char c = str[i];
        // See RFC6838
        if ((c >= 'A' && c <= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9')) {
          if (builder != null) {
            builder.Append(c);
          }
          ++i;
          ++count;
        } else if (count > 0 && ((c & 0x7F)==c && "!#$&-^_.+".IndexOf(c)>= 0)) {
          if (builder != null) {
            builder.Append(c);
          }
          ++i;
          ++count;
        } else {
          break;
        }
        // type or subtype too long
        if (count > 127) {
          return index;
        }
      }
      return i;
    }

    /// <summary>Returns the charset parameter, converted to ASCII lower-case,
    /// if it exists, or "us-ascii" if the media type is ill-formed (RFC2045
    /// sec. 5.2), or if the media type is "text/plain" or "text/xml" and doesn't
    /// have a charset parameter (see RFC2046 and RFC3023, respectively),
    /// or the empty string otherwise.</summary>
    /// <returns>A string object.</returns>
    public string GetCharset() {
      string param = this.GetParameter("charset");
      if (param != null) {
        return ParserUtility.ToLowerCaseAscii(param);
      }
      if (this.topLevelType.Equals("text")) {
        if (this.subType.Equals("xml") || this.subType.Equals("plain")) {
          return "us-ascii";
        }
        if (this.subType.Equals("csv")) {
          // see RFC 7111 sec. 5.1
          return "utf-8";
        }
        if (this.subType.Equals("vnd.graphviz")) {
          // see http://iana.org/assignments/media-types/text/vnd.graphviz
          return "utf-8";
        }
      }
      return String.Empty;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetParameter(string name) {
      if (name == null) {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0) {
        throw new ArgumentException("name is empty.");
      }
      name = ParserUtility.ToLowerCaseAscii(name);
      if (this.parameters.ContainsKey(name)) {
        return this.parameters[name];
      }
      return null;
    }

    private static string DecodeRfc2231Extension(string value) {
      int firstQuote = value.IndexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      int secondQuote = value.IndexOf('\'', firstQuote+1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return null;
      }
      string charset = value.Substring(0, firstQuote);
      // NOTE: Ignored
      // string language = value.Substring(firstQuote + 1, secondQuote-(firstQuote + 1));
      string paramValue = value.Substring(secondQuote + 1);
      Charsets.ICharset cs = Charsets.GetCharset(charset);
      if (cs == null) {
        cs = Charsets.Ascii;
      }
      return DecodeRfc2231Encoding(paramValue, cs);
    }

    private static Charsets.ICharset GetRfc2231Charset(string value) {
      if (value == null) {
        return Charsets.Ascii;
      }
      int firstQuote = value.IndexOf('\'');
      if (firstQuote < 0) {
        // not a valid encoded parameter
        return Charsets.Ascii;
      }
      int secondQuote = value.IndexOf('\'', firstQuote+1);
      if (secondQuote < 0) {
        // not a valid encoded parameter
        return Charsets.Ascii;
      }
      string charset = value.Substring(0, firstQuote);
      // NOTE: Ignored
      // string language = value.Substring(firstQuote + 1, secondQuote-(firstQuote + 1));
      Charsets.ICharset cs = Charsets.GetCharset(charset);
      if (cs == null) {
        cs = Charsets.Ascii;
      }
      return cs;
    }

    private static string DecodeRfc2231Encoding(string value, Charsets.ICharset charset) {
      return charset.GetString(new Message.PercentEncodingStringTransform(value));
    }

    private bool ExpandRfc2231Extensions() {
      if (this.parameters.Count == 0) {
        return true;
      }
      IList<string> keyList = new List<string>(this.parameters.Keys);
      foreach (string name in keyList) {
        if (!this.parameters.ContainsKey(name)) {
          continue;
        }
        string value = this.parameters[name];
        int asterisk = name.IndexOf('*');
        if (asterisk == name.Length - 1 && asterisk > 0) {
          // name*="value" (except when the parameter is just "*")
          string realName = name.Substring(0, name.Length - 1);
          string realValue = DecodeRfc2231Extension(value);
          if (realValue == null) {
            continue;
          }
          this.parameters.Remove(name);
          this.parameters[realName] = realValue;
          continue;
        }
        // name*0 or name*0*
        if (asterisk > 0 &&
            ((asterisk == name.Length - 2 && name[asterisk+1]=='0') ||
             (asterisk == name.Length - 3 && name[asterisk+1]=='0' && name[asterisk+2]=='*')
)) {
          string realName = name.Substring(0, asterisk);
          string realValue = (asterisk == name.Length - 3) ? DecodeRfc2231Extension(value) :
            value;
          Charsets.ICharset charsetUsed = GetRfc2231Charset(
            (asterisk == name.Length - 3) ? value : null);
          this.parameters.Remove(name);
          if (realValue == null) {
            realValue = value;
          }
          int pindex = 1;
          // search for name*1 or name*1*, then name*2 or name*2*,
          // and so on
          while (true) {
            string contin = realName + "*"+
              Convert.ToString(pindex, CultureInfo.InvariantCulture);
            string continEncoded = contin + "*";
            if (this.parameters.ContainsKey(contin)) {
              // Unencoded continuation
              realValue += this.parameters[contin];
              this.parameters.Remove(contin);
            } else if (this.parameters.ContainsKey(continEncoded)) {
              // Encoded continuation
              realValue += DecodeRfc2231Encoding(this.parameters[continEncoded], charsetUsed);
              this.parameters.Remove(continEncoded);
            } else {
              break;
            }
            ++pindex;
          }
          this.parameters[realName] = realValue;
        }
      }
       foreach (string name in this.parameters.Keys) {
        // Check parameter names using stricter format
        // in RFC6838
        if (skipMimeTypeSubtype(name, 0, name.Length, null) != name.Length) {
          // Illegal parameter name, so use default media type
          return false;
        }
      }
      return true;
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string TypeAndSubType {
      get {
        return this.TopLevelType + "/" + this.SubType;
      }
    }

    internal static int skipLws(string s, int index, int endIndex) {
      // While HTTP usually only allows CRLF, it also allows
      // us to be tolerant here
      int i2 = index;
      if (i2 + 1 < endIndex && s[i2] == 0x0d && s[i2]==0x0a) {
        i2 += 2;
      } else if (i2 < endIndex && (s[i2] == 0x0d || s[i2]==0x0a)) {
        ++index;
      }
      while (i2 < endIndex) {
        if (s[i2] == 0x09 || s[i2]==0x20) {
          ++index;
        }
        break;
      }
      return index;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>A Boolean object.</returns>
    public bool ParseMediaType(string str) {
      bool httpRules = false;
      int index = 0;
      int endIndex = str.Length;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (httpRules) {
        index = skipLws(str, index, endIndex);
      } else {
        index = HeaderParser.ParseCFWS(str, index, endIndex, null);
      }
      int i = skipMimeTypeSubtype(str, index, endIndex, null);
      if (i == index || i >= endIndex || str[i]!='/') {
        return false;
      }
      this.topLevelType = ParserUtility.ToLowerCaseAscii(str.Substring(index, i - index));
      ++i;
      int i2 = skipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        return false;
      }
      this.subType = ParserUtility.ToLowerCaseAscii(str.Substring(i, i2 - i));
      if (i2 < endIndex) {
        // if not at end
        int i3 = HeaderParser.ParseCFWS(str, i2, endIndex, null);
        if (i3 == endIndex || (i3 < endIndex && str[i3]!=';' && str[i3]!=',')) {
          // at end, or not followed by ";" or ",", so not a media type
          return false;
        }
      }
      index = i2;
      int indexAfterTypeSubtype = index;
      while (true) {
        // RFC5322 uses ParseCFWS when skipping whitespace;
        // HTTP currently uses skipLws, though that may change
        // to skipWsp in a future revision of HTTP
        if (httpRules) {
          indexAfterTypeSubtype = skipLws(str, indexAfterTypeSubtype, endIndex);
        } else {
          indexAfterTypeSubtype = HeaderParser.ParseCFWS(
            str,
            indexAfterTypeSubtype,
            endIndex,
            null);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            return this.ExpandRfc2231Extensions();
          }
          return true;
        }
        if (str[indexAfterTypeSubtype] != ';') {
          return false;
        }
        ++indexAfterTypeSubtype;
        if (httpRules) {
          indexAfterTypeSubtype = skipLws(str, indexAfterTypeSubtype, endIndex);
        } else {
          indexAfterTypeSubtype = HeaderParser.ParseCFWS(
            str,
            indexAfterTypeSubtype,
            endIndex,
            null);
        }
        StringBuilder builder = new StringBuilder();
        // NOTE: RFC6838 restricts the format of parameter names to the same
        // syntax as types and subtypes, but this syntax is incompatible with
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
        string attribute = builder.ToString();
        indexAfterTypeSubtype = afteratt;
        if (!httpRules) {
          // NOTE: MIME implicitly doesn't restrict whether whitespace can appear
          // around the equal sign separating an attribute and value, while
          // HTTP explicitly forbids such whitespace
          indexAfterTypeSubtype = HeaderParser.ParseCFWS(
            str,
            indexAfterTypeSubtype,
            endIndex,
            null);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          return false;
        }
        if (str[indexAfterTypeSubtype] != '=') {
          return false;
        }
        attribute = ParserUtility.ToLowerCaseAscii(attribute);
        if (this.parameters.ContainsKey(attribute)) {
          Console.WriteLine("Contains duplicate attribute " + attribute);
          return false;
        }
        ++indexAfterTypeSubtype;
        if (!httpRules) {
          // See note above on whitespace around the equal sign
          indexAfterTypeSubtype = HeaderParser.ParseCFWS(
            str,
            indexAfterTypeSubtype,
            endIndex,
            null);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            return this.ExpandRfc2231Extensions();
          }
          return true;
        }
        builder.Clear();
        int qs;
        // If the attribute name ends with '*' the value may not be a quoted string
        if (attribute[attribute.Length - 1] != '*') {
          // try getting the value quoted
          qs = skipQuotedString(
            str,
            indexAfterTypeSubtype,
            endIndex,
            builder,
            httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
          if (qs != indexAfterTypeSubtype) {
            this.parameters[attribute] = builder.ToString();
            indexAfterTypeSubtype = qs;
            continue;
          }
          builder.Clear();
        }
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = skipMimeToken(str, indexAfterTypeSubtype, endIndex, builder, httpRules);
        if (qs != indexAfterTypeSubtype) {
          this.parameters[attribute] = builder.ToString();
          indexAfterTypeSubtype = qs;
          continue;
        }
        // no valid value, return
        return false;
      }
    }

    public static readonly MediaType TextPlainAscii =
      new MediaTypeBuilder("text", "plain").SetParameter("charset", "us-ascii").ToMediaType();

    public static readonly MediaType TextPlainUtf8 =
      new MediaTypeBuilder("text", "plain").SetParameter("charset", "utf-8").ToMediaType();

    public static readonly MediaType MessageRfc822 =
      new MediaTypeBuilder("message", "rfc822").ToMediaType();

    private static MediaType valueApplicationOctetStream =
      new MediaTypeBuilder("application", "octet-stream").ToMediaType();

    private MediaType() {
    }

    public static MediaType Parse(string str) {
      return Parse(str, TextPlainAscii);
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <param name='defaultValue'>Can be null.</param>
    /// <returns>A MediaType object.</returns>
    public static MediaType Parse(string str, MediaType defaultValue) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      MediaType mt = new MediaType();
      mt.parameters = new SortedDictionary<string, string>();
      if (!mt.ParseMediaType(str)) {
        #if DEBUG
        Console.WriteLine("Unparsable: " + str);
        #endif
        return defaultValue;
      }
      return mt;
    }
  }
}
