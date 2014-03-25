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
        return topLevelType;
      }
    }
    private string subType;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string SubType {
      get {
        return subType;
      }
    }
    private IList<string> parameters;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<string> Parameters {
      get {
        return parameters;
      }
    }

    internal enum QuotedStringRule {
      Http,
      Rfc5322,
      Smtp  // RFC5321
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
          char c = s[i2];
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
        char c = s[index];
        if (c>= 0x20 && c<= 0x7E && c!='\\' && c!='"') {
          return index + 1;
        }
        i2 = skipQuotedPairSMTP(s, index, endIndex);
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
      if (index+1<endIndex && s[index]=='\\') {
        char c = s[index + 1];
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

    internal static int skipQuotedPairSMTP(string s, int index, int endIndex) {
      if (index+1<endIndex && s[index]=='\\') {
        char c = s[index + 1];
        if (c >= 0x20 && c <= 0x7e) {
          return index + 2;
        }
      }
      return index;
    }
    // quoted-string (RFC5322 sec. 3.2.4)
    internal static int skipQuotedString(string s, int index,
                                         int endIndex, StringBuilder builder) {
      return skipQuotedString(s, index, endIndex, builder, QuotedStringRule.Rfc5322);
    }

    internal static int skipQuotedString(
      string str,
      int index,
      int endIndex,
      StringBuilder builder,  // receives the unescaped version of the _string
      QuotedStringRule rule){
      int startIndex = index;
      int bLength=(builder == null) ? 0 : builder.Length;
      index=(rule != QuotedStringRule.Rfc5322) ? index :
        Message.SkipCommentsAndWhitespace(str, index, endIndex);
      if (!(index<endIndex && str[index]=='"')) {
        if (builder != null) {
          builder.Length=(bLength);
        }
        return startIndex;  // not a valid quoted-string
      }
      ++index;
      while (index<endIndex) {
        int i2 = index;
        if (rule == QuotedStringRule.Http) {
          i2 = skipLws(str, index, endIndex);
          if (i2 != index) {
            builder.Append(' ');
          }
        } else if (rule == QuotedStringRule.Rfc5322) {
          // Skip tabs and spaces (should skip
          // folding whitespace too, but this method assumes
          // unfolded values)
          i2 = ParserUtility.SkipSpaceAndTab(str, index, endIndex);
          if (i2 != index) {
            builder.Append(' ');
          }
        }
        index = i2;
        char c = str[index];
        if (c=='"') { // end of quoted-string
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
            builder.Remove(bLength, (builder.Length)-(bLength));
          }
          return startIndex;
        }
        if (builder != null) {
          // this is a qtext or quoted-pair, so
          // append the last character read
          builder.Append(str[index-1]);
        }
      }
      if (builder != null) {
        builder.Remove(bLength, (builder.Length)-(bLength));
      }
      return startIndex;  // not a valid quoted-string
    }

    internal static string ToLowerCaseAscii(string str) {
      if (str == null) {
        return null;
      }
      int len = str.Length;
      char c=(char)0;
      bool hasUpperCase = false;
      for (int i = 0; i < len; ++i) {
        c = str[i];
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
        c = str[i];
        if (c>= 'A' && c<= 'Z') {
          builder.Append((char)(c + 0x20));
        } else {
          builder.Append(c);
        }
      }
      return builder.ToString();
    }

    private static void AppendParamValue(string str, StringBuilder sb) {
      bool simple = true;
      for (int i = 0;i<str.Length; ++i) {
        char c = str[i];
        if (!(c>= 33 && c<= 126 && "()<>,;[]:@\"\\/?=".IndexOf(c)< 0)) {
          simple = false;
        }
      }
      if (simple) {
        sb.Append(str);
        return;
      }
      sb.Append('"');
      for (int i = 0;i<str.Length; ++i) {
        char c = str[i];
        if (c >= 33 && c <= 126) {
          sb.Append(c);
        } else if (c==0x20 || c==0x09 || c=='\\' || c=='"') {
          sb.Append('\\');
          sb.Append(c);
        } else {
          // Unencodable character in the string
          // TODO: Use RFC 2231 encoding in this case
          sb.Append('?');
        }
      }
      sb.Append('"');
    }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(topLevelType);
      sb.Append('/');
      sb.Append(subType);
      for (int i = 0;i<parameters.Count;i+=2) {
        sb.Append(';');
        sb.Append(parameters[i]);
        sb.Append('=');
        AppendParamValue(parameters[i + 1], sb);
      }
      return sb.ToString();
    }

    internal static int skipMimeToken(string str, int index, int endIndex,
                                      StringBuilder builder, bool httpRules) {
      int i = index;
      while (i<endIndex) {
        char c = str[i];
        if (c<= 0x20 || c>= 0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=".IndexOf(c)>= 0)) {
          break;
        }
        if (httpRules && (c=='{' || c=='}')) {
          break;
        }
        if (builder != null) {
          builder.Append(c);
        }
        ++i;
      }
      return i;
    }
    private static int skipMimeTypeSubtype(string str, int index, int endIndex, StringBuilder builder) {
      int i = index;
      int count = 0;
      while (i<str.Length) {
        char c = str[i];
        // See RFC6838
        if ((c>= 'A' && c<= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9')) {
          if (builder != null) {
            builder.Append(c);
          }
          ++i;
          ++count;
        } else if (count>0 && ((c&0x7F)==c && "!#$&-^_.+".IndexOf(c)>= 0)) {
          if (builder != null) {
            builder.Append(c);
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

    /// <summary>Returns the charset parameter, converted to ASCII lower-case,
    /// if it exists, or "us-ascii" if the media type is ill-formed (RFC2045
    /// sec. 5.2), or if the media type is "text/plain" or "text/xml" and doesn't
    /// have a charset parameter (see RFC2046 and RFC3023, respectively),
    /// or the empty _string otherwise.</summary>
    /// <returns>A string object.</returns>
    public string GetCharset() {
      for (int i = 0;i<parameters.Count;i+=2) {
        if (parameters[i].Equals("charset")) {
          return ToLowerCaseAscii(parameters[i + 1]);
        }
      }
      if (topLevelType.Equals("text")) {
        if (subType.Equals("xml") || subType.Equals("plain")) {
          return "us-ascii";
        }
      }
      return "";
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetParameter(string name) {
      name = ToLowerCaseAscii(name);
      for (int i = 0;i<parameters.Count;i+=2) {
        if (parameters[i].Equals(name)) {
          return parameters[i + 1];
        }
      }
      return null;
    }

    private void expandParameterContinuations() {
      if (parameters.Count< 2) {
        return;
      }
      for (int i = 0;i<parameters.Count;i+=2) {
        if (parameters[i].IndexOf('*')< 0) {
          int pindex = 1;
          // Support parameter continuations under RFC2231 sec. 3
          // TODO: Support situation in sec. 4.1
          while (true) {
            string contin=parameters[i]+"*"+
              Convert.ToString(pindex, CultureInfo.InvariantCulture);
            bool found = false;
            for (int j = 0;j<parameters.Count;j+=2) {
              if (parameters[j].Equals(contin)) {
                parameters[i + 1]+=parameters[j + 1];
                parameters.RemoveAt(j);
                parameters.RemoveAt(j);
                j-=2;
                found = true;
                break;
              }
            }
            if (!found) {
              break;
            }
            ++pindex;
          }
        }
      }
    }

    private void UseDefault() {
      topLevelType="text";
      subType="plain";
      parameters.Clear();
      parameters.Add("charset");
      parameters.Add("us-ascii");
    }

    internal static int skipLws(string s, int index, int endIndex) {
      // While HTTP usually only allows CRLF, it also allows
      // us to be tolerant here
      int i2 = index;
      if (i2 + 1<endIndex && s[i2]==0x0d && s[i2]==0x0a) {
        i2+=2;
      } else if (i2<endIndex && (s[i2]==0x0d || s[i2]==0x0a)) {
        ++index;
      }
      while (i2<endIndex) {
        if (s[i2]==0x09||s[i2]==0x20) {
          ++index;
        }
        break;
      }
      return index;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    public void ParseMediaType(string str) {
      bool httpRules = false;
      int index = 0;
      int endIndex = str.Length;
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (httpRules) {
        index = skipLws(str, index, endIndex);
      } else {
        index = Message.SkipCommentsAndWhitespace(str, index, endIndex);
      }
      int i = skipMimeTypeSubtype(str, index, endIndex, null);
      if (i==index || i>= endIndex || str[i]!='/') {
        UseDefault();
        return;
      }
      this.topLevelType = ToLowerCaseAscii(str.Substring(index, i-index));
      ++i;
      int i2 = skipMimeTypeSubtype(str, i, endIndex, null);
      if (i == i2) {
        UseDefault();
        return;
      }
      this.subType = ToLowerCaseAscii(str.Substring(i, i2-i));
      if (i2<endIndex) {
        // if not at end
        int i3 = Message.SkipCommentsAndWhitespace(str, i2, endIndex);
        if (i3==endIndex || (i3<endIndex && str[i3]!=';' && str[i3]!=',')) {
          // at end, or not followed by ";" or ",", so not a media type
          UseDefault();
          return;
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
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(str, indexAfterTypeSubtype, endIndex);
        }
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            expandParameterContinuations();
          }
          return;
        }
        if (str[indexAfterTypeSubtype]!=';') {
          UseDefault();
          return;
        }
        ++indexAfterTypeSubtype;
        if (httpRules) {
          indexAfterTypeSubtype = skipLws(str, indexAfterTypeSubtype, endIndex);
        } else {
          indexAfterTypeSubtype = Message.SkipCommentsAndWhitespace(str, indexAfterTypeSubtype, endIndex);
        }
        StringBuilder builder = new StringBuilder();
        int afteratt = skipMimeTypeSubtype(str, indexAfterTypeSubtype, endIndex, builder);
        if (afteratt == indexAfterTypeSubtype) {  // ill-formed attribute
          UseDefault();
          return;
        }
        string attribute = builder.ToString();
        indexAfterTypeSubtype = afteratt;
        if (indexAfterTypeSubtype >= endIndex) {
          UseDefault();
          return;
        }
        if (str[indexAfterTypeSubtype]!='=') {
          UseDefault();
          return;
        }
        attribute = ToLowerCaseAscii(attribute);
        ++indexAfterTypeSubtype;
        if (indexAfterTypeSubtype >= endIndex) {
          // No more parameters
          if (!httpRules) {
            expandParameterContinuations();
          }
          return;
        }
        builder.Clear();
        // try getting the value quoted
        int qs = skipQuotedString(
          str,
          indexAfterTypeSubtype,
          endIndex,
          builder,
          httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
        if (qs != indexAfterTypeSubtype) {
          parameters.Add(attribute);
          parameters.Add(builder.ToString());
          indexAfterTypeSubtype = qs;
          continue;
        }
        builder.Clear();
        // try getting the value unquoted
        // Note we don't use getAtom
        qs = skipMimeToken(str, indexAfterTypeSubtype, endIndex, builder, httpRules);
        if (qs != indexAfterTypeSubtype) {
          parameters.Add(attribute);
          parameters.Add(builder.ToString());
          indexAfterTypeSubtype = qs;
          continue;
        }
        // no valid value, return
        UseDefault();
        return;
      }
    }
    public MediaType(string str) {
      parameters = new List<string>();
      ParseMediaType(str);
    }
  }
}
