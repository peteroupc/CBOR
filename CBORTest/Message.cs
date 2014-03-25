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
using System.IO;
using System.Text;

using NUnit.Framework;
using PeterO;
using PeterO.Cbor;

namespace CBORTest
{
  internal sealed class Message {
    private IList<string> headers;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<string> Headers {
      get {
        return headers;
      }
    }
    private string body;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public string Body {
      get {
        return body;
      }
    }
    public Message(string str) {
      headers = new List<string>();
      ParseMessage(str);
    }
    public Message() {
      headers = new List<string>();
    }
    
    private string boundary;
    
    private string Boundary {
      get { return boundary; }
      set { boundary = value; }
    }

    internal static int skipComment(string str, int index, int endIndex) {
      int startIndex = index;
      if (!(index<endIndex && str[index]=='('))
        return index;
      ++index;
      while (index<endIndex) {
        // Skip tabs and spaces (should skip
        // folding whitespace too, but this method assumes
        // unfolded values)
        index = ParserUtility.SkipSpaceAndTab(str, index, endIndex);
        char c = str[index];
        if (c==')') {
          return index + 1;
        }
        int oldIndex = index;
        // skip comment character (RFC5322 sec. 3.2.1)
        if (index<endIndex) {
          c = str[index];
          if (c>= 33 && c<= 126 && c!='(' && c!=')' && c!='\\') {
            ++index;
          } else if ((c<0x20 && c != 0x00 && c != 0x09 && c != 0x0a && c != 0x0d)  || c == 0x7f) {
            // obs-ctext
            index+=2;
          }
        }
        if (index != oldIndex) {
          continue;
        }
        // skip quoted-pair (RFC5322 sec. 3.2.1)
        if (index+1<endIndex && str[index]=='\\') {
          c = str[index + 1];
          if (c == 0x20 || c == 0x09 || (c >= 0x21 && c <= 0x7e)) {
            index+=2;
          }
          // obs-qp
          if ((c<0x20 && c != 0x09)  || c == 0x7f) {
            index+=2;
          }
        }
        // skip nested comment
        index = skipComment(str, index, endIndex);
        if (index != oldIndex) {
          continue;
        }
        break;
      }
      return startIndex;
    }

    internal static int SkipCommentsAndWhitespace(
      string str,
      int index,
      int endIndex) {
      int retIndex = index;
      int startIndex = index;
      while (index<endIndex) {
        int oldIndex = index;
        // Skip tabs and spaces (should skip
        // folding whitespace too, but this method assumes
        // unfolded values)
        index = ParserUtility.SkipSpaceAndTab(str, index, endIndex);
        // Skip comments
        index = skipComment(str, index, endIndex);
        retIndex = index;
        if (oldIndex == index) {
          break;
        }
      }
      return retIndex;
    }

    public static string StripCommentsAndExtraSpace(string s) {
      StringBuilder sb = null;
      int index = 0;
      while (index<s.Length) {
        char c = s[index];
        if (c=='(' || c==0x09 || c==0x20) {
          int wsp = SkipCommentsAndWhitespace(s, index, s.Length);
          if (sb == null) {
            sb = new StringBuilder();
            sb.Append(s.Substring(0, index));
          }
          if (sb.Length>0) {
            sb.Append(' ');
          }
          index = wsp;
          continue;
        } else {
          if (sb != null) {
            sb.Append(c);
          }
        }
        ++index;
      }
      string ret=(sb == null) ? s : sb.ToString();
      int trimLen = ret.Length;
      for (int i = trimLen-1;i >= 0; --i) {
        if (ret[i]==' ') {
          --trimLen;
        } else {
          break;
        }
      }
      if (trimLen != ret.Length) {
        return ret.Substring(0, trimLen);
      } else {
        return ret;
      }
    }

    private MediaType mediaType;
    private int transferEncoding;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public MediaType MediaType {
      get {
        return mediaType;
      }
    }

    private void ProcessHeaders() {
      bool haveContentType = false;
      bool mime = false;
      bool badContentEncoding = false;
      for (int i = 0;i<headers.Count;i+=2) {
        string name = headers[i];
        string value = headers[i + 1];
        if (name.Equals("mime-version")) {
          headers[i + 1]=StripCommentsAndExtraSpace(value);
          mime = true;
        }
      }
      mediaType=new MediaType("text/plain");
      for (int i = 0;i<headers.Count;i+=2) {
        string name = headers[i];
        string value = headers[i + 1];
        if (mime && name.Equals("content-transfer-encoding")) {
          value = MediaType.ToLowerCaseAscii(StripCommentsAndExtraSpace(value));
          headers[i + 1]=value;
          if (value.Equals("7bit")) {
            transferEncoding = 0;
          } else if (value.Equals("8bit")) {
            throw new NotSupportedException("8bit encoding not supported for strings");
          } else if (value.Equals("binary")) {
            throw new NotSupportedException("Binary encoding not supported for strings");
          } else if (value.Equals("quoted-printable")) {
            transferEncoding = 1;
          } else if (value.Equals("base64")) {
            transferEncoding = 2;
          } else {
            badContentEncoding = true;
          }
        } else if (mime && name.Equals("content-type")) {
          if (haveContentType) {
            throw new InvalidDataException("Already have this header: "+name);
          }
          mediaType = new MediaType(value);
          if (headers[i+1].Contains("*")) {
            Console.WriteLine(value);
            Console.WriteLine(mediaType);
          }
          haveContentType = true;
        }
      }
      if (badContentEncoding) {
        mediaType=new MediaType("application/octet-stream");
      }
    }

    private static bool IsValidBoundary(string str) {
      if (str == null || str.Length<1 || str.Length>70) {
        return false;
      }
      for (int i = 0;i<str.Length; ++i) {
        char c = str[i];
        if (i==str.Length-1 && c==0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c>= 'A' && c<= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9') || 
          "'()-,./+_:=?".IndexOf(c)>= 0)) {
          return false;
        }
      }
      return true;
    }

    private int ParseHeaders(
      string str,
      int index,
      IList<string> headerList) {
      int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      while (index<str.Length) {
        sb.Clear();
        bool first = true;
        bool endOfHeaders = false;
        bool wsp = false;
        int lineStart = index;
        lineCount = 0;
        while (index<str.Length) {
          char c = str[index];
          ++lineCount;
          ++index;
          if (first && c=='\r' && index<str.Length && str[index]=='\n') {
            endOfHeaders = true;
            ++index;
            break;
          }
          if (lineCount>998) {
            throw new InvalidDataException("Header field line too long");
          }
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            if (wsp) {
              foreach(string ss in headerList) {
                Console.WriteLine(ss);
              }
              throw new InvalidDataException("Whitespace within header field");
            }
            first = false;
            if (c>= 'A' && c<= 'Z') {
              c+=(char)0x20;
            }
            sb.Append(c);
          } else if (!first && c==':') {
            break;
          } else if (c == 0x20 || c == 0x09) {
            wsp = true;
            first = false;
          } else {
            throw new InvalidDataException("Malformed header field name");
          }
        }
        if (endOfHeaders) {
          break;
        }
        if (sb.Length == 0) {
          throw new InvalidDataException("Empty header field name");
        }
        string fieldName = sb.ToString();
        sb.Clear();
        // Read the header field value
        while (index<str.Length) {
          char c = str[index];
          ++index;
          if (c=='\r' && index<str.Length && str[index]=='\n') {
            ++index;
            int oldIndex = index;
            lineCount = 0;
            lineStart = index;
            // Parse obsolete folding whitespace (obs-fws) under RFC5322
            // (parsed according to errata), same as LWSP in RFC5234
            bool fwsFirst = true;
            while (true) {
              int i2 = index;
              // Skip the CRLF pair, if any (except if iterating for
              // the first time, since CRLF was already parsed)
              if (!fwsFirst && index + 1<str.Length && str[index]==0x0d &&
                  str[index + 1]==0x0a) {
                i2 = index + 2;
                lineCount = 0;
                lineStart = i2;
              }
              fwsFirst = false;
              // Skip space or tab, if any
              if (i2<str.Length && (str[i2]==0x20 || str[i2]==0x09)) {
                index = i2 + 1;
                lineCount+=1;
                sb.Append(str[i2]);
                if (lineCount>998) {
                  throw new InvalidDataException("Header field line too long");
                }
              } else {
                break;
              }
            }
            if (index != oldIndex) {
              // We have folding whitespace, line
              // count found as above
              continue;
            }
            break;
          }
          if (lineCount>998) {
            throw new InvalidDataException("Header field line too long");
          }
          if (c<0x80) {
            sb.Append(c);
          } else {
            if (fieldName.Equals("subject")) {
              // Deviation: Some emails still have an unencoded subject line
              sb.Append(' ');
            } else {
              throw new InvalidDataException("Malformed header field value "+sb.ToString());
            }
          }
        }
        string fieldValue = sb.ToString();
        headerList.Add(fieldName);
        // NOTE: Field value will no longer have folding whitespace
        // at this point
        headerList.Add(fieldValue);
      }
      return index;
    }

    private void ParseMultipartBody(string str, int index) {
      int lineCount = 0;
      string boundary=mediaType.GetParameter("boundary");
      Console.WriteLine(mediaType);
      if (boundary == null) {
        throw new InvalidDataException("Multipart message has no boundary defined");
      }
      if (!IsValidBoundary(boundary)) {
        throw new InvalidDataException("Multipart message has an invalid boundary defined");
      }
      IList<Message> multipartStack = new List<Message>();
      int messageStart = index;
      lineCount = 0;
      while (index<str.Length) {
        char c = str[index];
        ++index;
        ++lineCount;
        if (c=='\r' && index<str.Length && str[index]=='\n') {
          ++index;
          lineCount = 0;
          continue;
        }
        if (lineCount==1 && c=='-') {
          if (index+boundary.Length<str.Length && str[index]=='-') {
            string maybeBoundary = str.Substring(index + 1, boundary.Length);
            if (boundary.Equals(maybeBoundary)) {
              int i2 = index + 1 + boundary.Length;
              bool closed = false;
              if (i2+1<str.Length && str[i2]=='-' && str[i2+1]=='-') {
                i2+=2;
                closed = true;
              }
              i2 = ParserUtility.SkipSpaceAndTab(str, i2, str.Length);
              if (!closed) {
                Console.WriteLine("Found closed boundary");
              } else {
                int i3 = ParserUtility.SkipCrLf(str, i2, str.Length);
                if (i3 != i2) {
                  // Found boundary for the next part
                  ++index;
                  lineCount = 0;
                  continue;
                }
                Console.WriteLine("Found open boundary");
              }
            }
          }
        }
        if (c >= 0x80) {
          Console.WriteLine("" + (str.Substring(Math.Max(0,index-20),20)));
          throw new InvalidDataException("Invalid character in message body");
        }
      }
      body = str.Substring(messageStart);
    }

    private void ParseSimpleBody(string str, int index) {
      int messageStart = index;
      int lineCount = 0;
      while (index<str.Length) {
        char c = str[index];
        ++index;
        ++lineCount;
        if (c=='\r' && index<str.Length && str[index]=='\n') {
          ++index;
          lineCount = 0;
          continue;
        }
        // NOTE: obs-body has no restrictions on line length
        if (c >= 0x80) {
          Console.WriteLine("" + (str.Substring(Math.Max(0,index-20),20)));
          throw new InvalidDataException("Invalid character in message body");
        }
      }
      body = str.Substring(messageStart);
    }

    private void ParseMessage(string str) {
      int index = 0;
      this.ParseHeaders(str, 0, this.headers);
      ProcessHeaders();
      if (mediaType.TopLevelType.Equals("multipart")) {
        ParseMultipartBody(str, index);
      } else {
        ParseSimpleBody(str, index);
      }
    }
  }
}
