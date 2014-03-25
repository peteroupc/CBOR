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
  [TestFixture]
  public class EncodingTest
  {
    private static string HexAlphabet="0123456789ABCDEF";

    internal sealed class MediaType {
      private string topLevelType;
      
      public string TopLevelType {
        get { return topLevelType; }
      }
      private string subType;
      
      public string SubType {
        get { return subType; }
      }
      private IList<string> parameters;
      
      public IList<string> Parameters {
        get { return parameters; }
      }

      internal enum QuotedStringRule {
        Http,
        Rfc5322,
        Smtp // RFC5321
      }

      private static int skipQtextOrQuotedPair(
        string s, int index, int endIndex, QuotedStringRule rule){
        if(index>=endIndex)return index;
        int i2;
        if(rule==QuotedStringRule.Http){
          char c=s[index];
          if(c<0x100 && c>=0x21 && c!='\\' && c!='"')
            return index+1;
          i2=skipQuotedPair(s,index,endIndex);
          if(index!=i2)return i2;
          return i2;
        } else if(rule==QuotedStringRule.Rfc5322){
          i2=index;
          // qtext (RFC5322 sec. 3.2.1)
          if(i2<endIndex){
            char c=s[i2];
            if(c>=33 && c<=126 && c!='\\' && c!='"')
              i2++;
            // obs-qtext (same as obs-ctext)
            if((c<0x20 && c!=0x00 && c!=0x09 && c!=0x0a && c!=0x0d)  || c==0x7F)
              i2++;
          }
          if(index!=i2)return i2;
          index=i2;
          i2=skipQuotedPair(s,index,endIndex);
          if(index!=i2)return i2;
          return i2;
        } else if(rule==QuotedStringRule.Smtp){
          char c=s[index];
          if(c>=0x20 && c<=0x7E && c!='\\' && c!='"')
            return index+1;
          i2=skipQuotedPairSMTP(s,index,endIndex);
          if(index!=i2)return i2;
          return i2;
        } else
          throw new ArgumentException(rule.ToString());
      }

      // quoted-pair (RFC5322 sec. 3.2.1)
      internal static int skipQuotedPair(string s, int index, int endIndex){
        if(index+1<endIndex && s[index]=='\\'){
          char c=s[index+1];
          if(c==0x20 || c==0x09 || (c>=0x21 && c<=0x7e))
            return index+2;
          // obs-qp
          if((c<0x20 && c!=0x09)  || c==0x7F)
            return index+2;
        }
        return index;
      }

      internal static int skipQuotedPairSMTP(string s, int index, int endIndex){
        if(index+1<endIndex && s[index]=='\\'){
          char c=s[index+1];
          if((c>=0x20 && c<=0x7e))
            return index+2;
        }
        return index;
      }
      // quoted-_string (RFC5322 sec. 3.2.4)
      internal static int skipQuotedString(string s, int index,
                                           int endIndex, StringBuilder builder){
        return skipQuotedString(s,index,endIndex,builder,QuotedStringRule.Rfc5322);
      }

      internal static int skipQuotedString(
        string str,
        int index,
        int endIndex,
        StringBuilder builder, // receives the unescaped version of the _string
        QuotedStringRule rule // rule to follow for quoted _string
       ){
        int startIndex=index;
        int bLength=(builder==null) ? 0 : builder.Length;
        index=(rule!=QuotedStringRule.Rfc5322) ? index :
          Message.SkipCommentsAndWhitespace(str,index,endIndex);
        if(!(index<endIndex && str[index]=='"')){
          if(builder!=null) {
            builder.Length=(bLength);
          }
          return startIndex; // not a valid quoted-_string
        }
        index++;
        while(index<endIndex){
          int i2=index;
          if(rule==QuotedStringRule.Http) {
            i2=skipLws(str,index,endIndex);
            if(i2!=index){
              builder.Append(' ');
            }
          } else if(rule==QuotedStringRule.Rfc5322) {
            // Skip tabs and spaces (should skip
            // folding whitespace too, but this method assumes
            // unfolded values)
            i2=index;
            while(i2<endIndex){
              if(str[i2]==0x09 || str[i2]==0x20){
                i2++;
              } else {
                break;
              }
            }
            if(i2!=index){
              builder.Append(' ');
            }
          }
          index=i2;
          char c=str[index];
          if(c=='"'){ // end of quoted-_string
            index++;
            if(rule==QuotedStringRule.Rfc5322)
              return Message.SkipCommentsAndWhitespace(str,index,endIndex);
            else
              return index;
          }
          int oldIndex=index;
          index=skipQtextOrQuotedPair(str,index,endIndex,rule);
          if(index==oldIndex){
            if(builder!=null) {
              builder.Remove(bLength,(builder.Length)-(bLength));
            }
            return startIndex;
          }
          if(builder!=null){
            // this is a qtext or quoted-pair, so
            // append the last character read
            builder.Append(str[index-1]);
          }
        }
        if(builder!=null) {
          builder.Remove(bLength,(builder.Length)-(bLength));
        }
        return startIndex; // not a valid quoted-_string
      }
      
      internal static string ToLowerCaseAscii(string str){
        if(str==null)return null;
        int len=str.Length;
        char c=(char)0;
        bool hasUpperCase=false;
        for(int i=0;i<len;i++){
          c=str[i];
          if(c>='A' && c<='Z'){
            hasUpperCase=true;
            break;
          }
        }
        if(!hasUpperCase)
          return str;
        StringBuilder builder=new StringBuilder();
        for(int i=0;i<len;i++){
          c=str[i];
          if(c>='A' && c<='Z'){
            builder.Append((char)(c+0x20));
          } else {
            builder.Append(c);
          }
        }
        return builder.ToString();
      }
      
      private static void AppendParamValue(string str, StringBuilder sb){
        bool simple=true;
        for(int i=0;i<str.Length;i++){
          char c=str[i];
          if(!(c>=33 && c<=126 && c!='\\' && c!='"')){
            simple=false;
          }
        }
        if(simple){
          sb.Append(str);
          return;
        }
        for(int i=0;i<str.Length;i++){
          char c=str[i];
          if(c>=33 && c<=126 && c!='\\' && c!='"'){
            sb.Append(c);
          } else if(c==0x20 || c==0x09 || c=='\\' || c=='"'){
            sb.Append('\\');
            sb.Append(c);
          } else {
            // Unencodable character in the string
            // TODO: Use RFC 2231 encoding in this case
            sb.Append('?');
          }
        }
      }
      
      public override string ToString(){
        StringBuilder sb=new StringBuilder();
        sb.Append(topLevelType);
        sb.Append('/');
        sb.Append(subType);
        for(int i=0;i<parameters.Count;i+=2){
          sb.Append(';');
          sb.Append(parameters[i]);
          sb.Append('=');
          AppendParamValue(parameters[i+1],sb);
        }
        return sb.ToString();
      }
      
      internal static int skipMimeToken(string str, int index, int endIndex,
                                        StringBuilder builder, bool httpRules){
        int i=index;
        while(i<endIndex){
          char c=str[i];
          if(c<=0x20 || c>=0x7F || ((c&0x7F)==c && "()<>@,;:\\\"/[]?=".IndexOf(c)>=0)) {
            break;
          }
          if(httpRules && (c=='{' || c=='}')){
            break;
          }
          if(builder!=null) {
            builder.Append(c);
          }
          i++;
        }
        return i;
      }
      private static int skipMimeTypeSubtype(string str, int index, int endIndex, StringBuilder builder){
        int i=index;
        int count=0;
        while(i<str.Length){
          char c=str[i];
          // See RFC6838
          if((c>='A' && c<='Z') || (c>='a' && c<='z') || (c>='0' && c<='9')){
            if(builder!=null) {
              builder.Append(c);
            }
            i++;
            count++;
          } else if(count>0 && ((c&0x7F)==c && "!#$&-^_.+".IndexOf(c)>=0)){
            if(builder!=null) {
              builder.Append(c);
            }
            i++;
            count++;
          } else {
            break;
          }
          // type or subtype too long
          if(count>127)return index;
        }
        return i;
      }

      /**
       * Returns the charset parameter, converted to ASCII lower-case,
       * if it exists, or "us-ascii" if the media type is
       * ill-formed (RFC2045 sec. 5.2), or if the media type is
       * "text/plain" or "text/xml" and doesn't have a charset parameter
       * (see RFC2046 and RFC3023, respectively),
       * or the empty _string otherwise.
       */
      public string GetCharset(){
        for(int i=0;i<parameters.Count;i+=2){
          if(parameters[i].Equals("charset")){
            return ToLowerCaseAscii(parameters[i+1]);
          }
        }
        if(topLevelType.Equals("text")){
          if(subType.Equals("xml") || subType.Equals("plain")){
            return "us-ascii";
          }
        }
        return "";
      }
      
      public string GetParameter(string name){
        name=ToLowerCaseAscii(name);
        for(int i=0;i<parameters.Count;i+=2){
          if(parameters[i].Equals(name)){
            return parameters[i+1];
          }
        }
        return null;
      }

      private void expandParameterContinuations(){
        if(parameters.Count<2){
          return;
        }
        for(int i=0;i<parameters.Count;i+=2){
          if(parameters[i].IndexOf('*')<0){
            int pindex=1;
            // Support parameter continuations under RFC2231 sec. 3
            // TODO: Support situation in sec. 4.1
            while(true){
              string contin=parameters[i]+"*"+
                Convert.ToString(pindex,CultureInfo.InvariantCulture);
              bool found=false;
              for(int j=0;j<parameters.Count;j+=2){
                if(parameters[j].Equals(contin)){
                  parameters[i+1]+=parameters[j+1];
                  parameters.RemoveAt(j);
                  parameters.RemoveAt(j);
                  j-=2;
                  found=true;
                  break;
                }
              }
              if(!found)break;
              pindex++;
            }
          }
        }
      }

      private void UseDefault(){
        topLevelType="text";
        subType="plain";
        parameters.Clear();
        parameters.Add("charset");
        parameters.Add("us-ascii");
      }
      
      
      internal static int skipLws(string s, int index, int endIndex){
        // While HTTP usually only allows CRLF, it also allows
        // us to be tolerant here
        int i2=index;
        if(i2+1<endIndex && s[i2]==0x0d && s[i2]==0x0a){
          i2+=2;
        }
        else if(i2<endIndex && (s[i2]==0x0d || s[i2]==0x0a)){
          index++;
        }
        while(i2<endIndex){
          if(s[i2]==0x09||s[i2]==0x20)
            index++;
          break;
        }
        return index;
      }
      
      public void ParseMediaType(string str){
        bool httpRules=false;
        int index=0;
        int endIndex=str.Length;
        if(str==null){
          throw new ArgumentNullException("str");
        }
        if(httpRules) {
          index=skipLws(str,index,endIndex);
        } else {
          index=Message.SkipCommentsAndWhitespace(str,index,endIndex);
        }
        int i=skipMimeTypeSubtype(str,index,endIndex,null);
        if(i==index || i>=endIndex || str[i]!='/'){
          UseDefault();
          return;
        }
        this.topLevelType=ToLowerCaseAscii(str.Substring(index,i-index));
        i++;
        int i2=skipMimeTypeSubtype(str,i,endIndex,null);
        if(i==i2){
          UseDefault();
          return;
        }
        this.subType=ToLowerCaseAscii(str.Substring(i,i2-i));
        if(i2<endIndex){
          // if not at end
          int i3=Message.SkipCommentsAndWhitespace(str,i2,endIndex);
          if(i3==endIndex || (i3<endIndex && str[i3]!=';' && str[i3]!=',')){
            // at end, or not followed by ";" or ",", so not a media type
            UseDefault();
            return;
          }
        }
        index=i2;
        int indexAfterTypeSubtype=index;
        while(true){
          // RFC5322 uses skipCommentsAndWhitespace when skipping whitespace;
          // HTTP currently uses skipLws, though that may change
          // to skipWsp in a future revision of HTTP
          if(httpRules) {
            indexAfterTypeSubtype=skipLws(str,indexAfterTypeSubtype,endIndex);
          } else {
            indexAfterTypeSubtype=Message.SkipCommentsAndWhitespace(str,indexAfterTypeSubtype,endIndex);
          }
          if(indexAfterTypeSubtype>=endIndex){
            // No more parameters
            if(!httpRules){
              expandParameterContinuations();
            }
            return;
          }
          if(str[indexAfterTypeSubtype]!=';'){
            UseDefault();
            return;
          }
          indexAfterTypeSubtype++;
          if(httpRules) {
            indexAfterTypeSubtype=skipLws(str,indexAfterTypeSubtype,endIndex);
          } else {
            indexAfterTypeSubtype=Message.SkipCommentsAndWhitespace(str,indexAfterTypeSubtype,endIndex);
          }
          StringBuilder builder=new StringBuilder();
          int afteratt=skipMimeTypeSubtype(str,indexAfterTypeSubtype,endIndex,builder);
          if(afteratt==indexAfterTypeSubtype){ // ill-formed attribute
            UseDefault();
            return;
          }
          string attribute=builder.ToString();
          indexAfterTypeSubtype=afteratt;
          if(indexAfterTypeSubtype>=endIndex){
            UseDefault();
            return;
          }
          if(str[indexAfterTypeSubtype]!='='){
            UseDefault();
            return;
          }
          attribute=ToLowerCaseAscii(attribute);
          indexAfterTypeSubtype++;
          if(indexAfterTypeSubtype>=endIndex){
            // No more parameters
            if(!httpRules){
              expandParameterContinuations();
            }
            return;
          }
          builder.Clear();
          // try getting the value quoted
          int qs=skipQuotedString(
            str,indexAfterTypeSubtype,endIndex,builder,
            httpRules ? QuotedStringRule.Http : QuotedStringRule.Rfc5322);
          if(qs!=indexAfterTypeSubtype){
            parameters.Add(attribute);
            parameters.Add(builder.ToString());
            indexAfterTypeSubtype=qs;
            continue;
          }
          builder.Clear();
          // try getting the value unquoted
          // Note we don't use getAtom
          qs=skipMimeToken(str,indexAfterTypeSubtype,endIndex,builder,httpRules);
          if(qs!=indexAfterTypeSubtype){
            parameters.Add(attribute);
            parameters.Add(builder.ToString());
            indexAfterTypeSubtype=qs;
            continue;
          }
          // no valid value, return
          UseDefault();
          return;
        }
      }
      public MediaType(string str){
        parameters=new List<string>();
        ParseMediaType(str);
      }
    }
    
    
    
    internal sealed class Message {
      private IList<string> headers;
      
      public IList<string> Headers {
        get { return headers; }
      }
      private string body;
      
      public string Body {
        get { return body; }
      }
      public Message(string str){
        headers=new List<string>();
        ParseMessage(str);
      }
      
      
      internal static int skipComment(string str, int index, int endIndex){
        int startIndex=index;
        if(!(index<endIndex && str[index]=='('))
          return index;
        index++;
        while(index<endIndex){
          // Skip tabs and spaces (should skip
          // folding whitespace too, but this method assumes
          // unfolded values)
          while(index<endIndex){
            if(str[index]==0x09 || str[index]==0x20){
              index++;
            } else {
              break;
            }
          }
          char c=str[index];
          if(c==')')return index+1;
          int oldIndex=index;
          // skip comment character (RFC5322 sec. 3.2.1)
          if(index<endIndex){
            c=str[index];
            if(c>=33 && c<=126 && c!='(' && c!=')' && c!='\\')
              index++;
            else if((c<0x20 && c!=0x00 && c!=0x09 && c!=0x0a && c!=0x0d)  || c==0x7F){
              // obs-ctext
              index+=2;
            }
          }
          if(index!=oldIndex)continue;
          // skip quoted-pair (RFC5322 sec. 3.2.1)
          if(index+1<endIndex && str[index]=='\\'){
            c=str[index+1];
            if(c==0x20 || c==0x09 || (c>=0x21 && c<=0x7e)){
              index+=2;
            }
            // obs-qp
            if((c<0x20 && c!=0x09)  || c==0x7F){
              index+=2;
            }
          }
          // skip nested comment
          index=skipComment(str,index,endIndex);
          if(index!=oldIndex)continue;
          break;
        }
        return startIndex;
      }

      internal static int SkipCommentsAndWhitespace(
        string s, int index, int endIndex){
        int retIndex=index;
        int startIndex=index;
        while(index<endIndex){
          int oldIndex=index;
          // Skip tabs and spaces (should skip
          // folding whitespace too, but this method assumes
          // unfolded values)
          while(index<endIndex){
            if(s[index]==0x09 || s[index]==0x20){
              index++;
            } else {
              break;
            }
          }
          // Skip comments
          index=skipComment(s,index,endIndex);
          retIndex=index;
          if(oldIndex==index)break;
        }
        return retIndex;
      }
      
      public static string StripCommentsAndExtraSpace(string s){
        StringBuilder sb=null;
        int index=0;
        while(index<s.Length){
          char c=s[index];
          if(c=='(' || c==0x09 || c==0x20){
            int wsp=SkipCommentsAndWhitespace(s,index,s.Length);
            if(sb==null){
              sb=new StringBuilder();
              sb.Append(s.Substring(0,index));
            }
            if(sb.Length>0)
              sb.Append(' ');
            index=wsp;
            continue;
          }
          else {
            if(sb!=null){
              sb.Append(c);
            }
          }
          index++;
        }
        string ret=(sb==null) ? s : sb.ToString();
        int trimLen=ret.Length;
        for(int i=trimLen-1;i>=0;i--){
          if(ret[i]==' '){
            trimLen--;
          } else {
            break;
          }
        }
        if(trimLen!=ret.Length){
          return ret.Substring(0,trimLen);
        } else {
          return ret;
        }
      }
      
      private MediaType mediaType;
      private int transferEncoding;
      
      public EncodingTest.MediaType MediaType {
        get { return mediaType; }
      }
      
      private void ProcessHeaders(){
        bool haveContentType=false;
        bool mime=false;
        bool badContentEncoding=false;
        for(int i=0;i<headers.Count;i+=2){
          string name=headers[i];
          string value=headers[i+1];
          if(name.Equals("mime-version")){
            headers[i+1]=StripCommentsAndExtraSpace(value);
            mime=true;
          }
        }
        mediaType=new MediaType("text/plain");
        for(int i=0;i<headers.Count;i+=2){
          string name=headers[i];
          string value=headers[i+1];
          if(name.Equals("return-path")){
            if(i+2>=headers.Count || !headers[i+2].Equals("received")){
              // TODO: Find which files are affected
              //Console.WriteLine("Received must follow Return-Path");
            }
          }
          else if(mime && name.Equals("content-transfer-encoding")){
            value=MediaType.ToLowerCaseAscii(StripCommentsAndExtraSpace(value));
            headers[i+1]=value;
            if(value.Equals("7bit")){
              transferEncoding=0;
            } else if(value.Equals("8bit")){
              throw new NotSupportedException("8bit encoding not supported for strings");
            } else if(value.Equals("binary")){
              throw new NotSupportedException("Binary encoding not supported for strings");
            } else if(value.Equals("quoted-printable")){
              transferEncoding=1;
            } else if(value.Equals("base64")){
              transferEncoding=2;
            } else {
              badContentEncoding=true;
            }
          }
          else if(mime && name.Equals("content-type")){
            if(haveContentType){
              throw new InvalidDataException("Already have this header: "+name);
            }
            mediaType=new MediaType(value);
            if(headers[i+1].Contains("*")){
              Console.WriteLine(value);
              Console.WriteLine(mediaType);
            }
            haveContentType=true;
          }
        }
        if(badContentEncoding){
          mediaType=new EncodingTest.MediaType("application/octet-stream");
        }
      }
      
      private void ParseMessage(string str){
        int index=0;
        int lineCount=0;
        StringBuilder sb=new StringBuilder();
        while(index<str.Length){
          sb.Clear();
          bool first=true;
          bool endOfHeaders=false;
          bool wsp=false;
          lineCount=0;
          while(index<str.Length){
            char c=str[index];
            lineCount++;
            index++;
            if(first && c=='\r' && index<str.Length && str[index]=='\n'){
              endOfHeaders=true;
              index++;
              break;
            }
            if(lineCount>998){
              throw new InvalidDataException("Header field line too long");
            }
            if((c>=0x21 && c<=57) || (c>=59 && c<=0x7e)){
              if(wsp){
                foreach(string ss in headers){
                  Console.WriteLine(ss);
                }
                throw new InvalidDataException("Whitespace within header field");
              }
              first=false;
              if(c>='A' && c<='Z')c+=(char)0x20;
              sb.Append(c);
            } else if(!first && c==':'){
              break;
            } else if(c==0x20 || c==0x09){
              wsp=true;
              first=false;
            } else {
              throw new InvalidDataException("Malformed header field name");
            }
          }
          if(endOfHeaders){
            break;
          }
          if(sb.Length==0){
            throw new InvalidDataException("Empty header field name");
          }
          string fieldName=sb.ToString();
          sb.Clear();
          // Read the header field value
          while(index<str.Length){
            char c=str[index];
            index++;
            if(c=='\r' && index<str.Length && str[index]=='\n'){
              index++;
              int oldIndex=index;
              lineCount=0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              bool fwsFirst=true;
              while(true){
                int i2=index;
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                if(!fwsFirst && index+1<str.Length && str[index]==0x0d &&
                   str[index+1]==0x0a){
                  i2=index+2;
                  lineCount=0;
                }
                fwsFirst=false;
                // Skip space or tab, if any
                if(i2<str.Length && (str[i2]==0x20 || str[i2]==0x09)){
                  index=i2+1;
                  lineCount+=1;
                  sb.Append(str[i2]);
                  if(lineCount>998){
                    throw new InvalidDataException("Header field line too long");
                  }
                } else {
                  break;
                }
              }
              if(index!=oldIndex){
                // We have folding whitespace, line
                // count found as above
                continue;
              }
              break;
            }
            if(lineCount>998){
              throw new InvalidDataException("Header field line too long");
            }
            if(c<0x80){
              sb.Append(c);
            } else {
              if(fieldName.Equals("subject")){
                // Deviation: Some emails still have an unencoded subject line
                sb.Append(' ');
              } else {
                throw new InvalidDataException("Malformed header field value "+sb.ToString());
              }
            }
          }
          string fieldValue=sb.ToString();
          headers.Add(fieldName);
          // NOTE: Field value will no longer have folding whitespace
          // at this point
          headers.Add(fieldValue);
        }
        ProcessHeaders();
        string boundary=null;
        if(mediaType.TopLevelType.Equals("multipart")){
          boundary=mediaType.GetParameter("boundary");
          if(boundary==null){
            throw new InvalidDataException("Multipart message has no boundary defined");
          }
        }
        int messageStart=index;
        lineCount=0;
        while(index<str.Length){
          char c=str[index];
          index++;
          lineCount++;
          if(c=='\r' && index<str.Length && str[index]=='\n'){
            index++;
            lineCount=0;
            continue;
          }
          // NOTE: obs-body has no restrictions on line length
          if(c>=0x80){
            Console.WriteLine("{0}",str.Substring(Math.Max(0,index-20),20));
            throw new InvalidDataException("Invalid character in message body");
          }
        }
        body=str.Substring(messageStart);
      }
    }
    
    
    private static void IncrementLineCount(StringBuilder str, int length, int[] count) {
      if (count[0]+length>75) { // 76 including the final '='
        str.Append("=\r\n");
        count[0]=length;
      } else {
        count[0]+=length;
      }
    }

    // lineBreakMode:
    // 0 - no line breaks
    // 1 - treat CRLF as a line break
    // 2 - treat CR, LF, and CRLF as a line break
    private static void ToQuotedPrintableRfc2045(StringBuilder str, byte[] data,
                                                 int offset, int count, int lineBreakMode) {
      if ((str) == null) {
        throw new ArgumentNullException("str");
      }
      if ((data) == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)(offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)(offset), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(data.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(data.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length-offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((long)(data.Length-offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture));
      }
      int length = offset + count;
      int[] lineCount = new int[] { 0};
      int i = offset;
      for (i = offset; i < length; ++i) {
        if (data[i]==0x0d) {
          if (lineBreakMode == 0) {
            IncrementLineCount(str, 3, lineCount);
            str.Append("=0D");
          } else if (i + 1 >= length || data[i + 1]!=0x0a) {
            if (lineBreakMode == 2) {
              str.Append("\r\n");
              lineCount[0]=0;
            } else {
              IncrementLineCount(str, 3, lineCount);
              str.Append("=0D");
            }
          } else {
            ++i;
            str.Append("\r\n");
            lineCount[0]=0;
          }
        } else if (data[i]==0x0a) {
          if (lineBreakMode == 2) {
            str.Append("\r\n");
            lineCount[0]=0;
          } else {
            IncrementLineCount(str, 3, lineCount);
            str.Append("=0A");
          }
        } else if (data[i]==9 || data[i]==32) {
          if (i + 2<length && lineBreakMode>0) {
            if (data[i + 1]==0x0d && data[i + 2]==0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.Append(data[i]==9 ? "=09\r\n" : "=20\r\n");
              lineCount[0]=0;
              i+=2;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.Append((char)data[i]);
            }
          } else if (i + 1<length && lineBreakMode == 2) {
            if (data[i + 1]==0x0d || data[i + 1]==0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.Append(data[i]==9 ? "=09\r\n" : "=20\r\n");
              lineCount[0]=0;
              i+=1;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.Append((char)data[i]);
            }
          } else {
            IncrementLineCount(str, 1, lineCount);
            str.Append((char)data[i]);
          }
        } else if (data[i]==(byte)'=') {
          IncrementLineCount(str, 3, lineCount);
          str.Append("=3D");
        } else if (data[i]>0x20 && data[i]<0x7f) {
          IncrementLineCount(str, 1, lineCount);
          str.Append((char)data[i]);
        } else {
          IncrementLineCount(str, 3, lineCount);
          str.Append('=');
          str.Append(HexAlphabet[(data[i] >> 4) & 15]);
          str.Append(HexAlphabet[data[i] & 15]);
        }
      }
    }

    /// <summary>Note: If lenientLineBreaks is true, treats CR, LF, and
    /// CRLF as line breaks and writes CRLF when encountering these breaks. If
    /// unlimitedLineLength is true, doesn't
    /// check that no more than 76 characters are in each line. If an encoded
    /// line ends with spaces and/or tabs, those characters are deleted (RFC
    /// 2045, sec. 6.7, rule 3).</summary>
    /// <param name='outputStream'>A readable data stream.</param>
    /// <param name='data'>A byte[] object.</param>
    /// <param name='offset'>A 32-bit signed integer.</param>
    /// <param name='count'>A 32-bit signed integer. (2).</param>
    /// <param name='lenientLineBreaks'>A Boolean object.</param>
    /// <param name='unlimitedLineLength'>A Boolean object. (2).</param>
    private static void ReadQuotedPrintable(
      Stream outputStream,
      byte[] data,
      int offset,
      int count,
      bool lenientLineBreaks,
      bool unlimitedLineLength) {
      if ((outputStream) == null) {
        throw new ArgumentNullException("outputStream");
      }
      if ((data) == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)(offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)(offset), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(data.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)(data.Length), System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length-offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((long)(data.Length-offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)(count), System.Globalization.CultureInfo.InvariantCulture));
      }
      int length = offset + count;
      int lineStart = 0;
      for (int i = offset; i < length; ++i) {
        if (data[i]==0x0d) {
          if (i + 1 >= length || data[i + 1]!=0x0a) {
            // Treat as line break
            if (!lenientLineBreaks) {
              throw new InvalidDataException("Expected LF after CR");
            }
            outputStream.WriteByte(0x0d);
            outputStream.WriteByte(0x0a);
          } else {
            outputStream.WriteByte(0x0d);
            outputStream.WriteByte(0x0a);
            ++i;
          }
          lineStart = i + 1;
        } else if (data[i]==0x0a) {
          if (!lenientLineBreaks) {
            throw new InvalidDataException("Bare LF not expected");
          }
          outputStream.WriteByte(0x0d);
          outputStream.WriteByte(0x0a);
          lineStart = i + 1;
        } else if (data[i]=='=') {
          if (i + 2 < length) {
            int b1=((int)data[i + 1]) & 0xff;
            int b2=((int)data[i + 2]) & 0xff;
            if (b1=='\r' && b2=='\n') {
              // Soft line break
              i+=2;
              continue;
            } else if (b1=='\r') {
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Expected LF after CR");
              }
              ++i;
              lineStart=i+1;
              continue;
            } else if (b1=='\n') {
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Bare LF not expected");
              }
              ++i;
              lineStart=i+1;
              continue;
            }
            int c = 0;
            if (b1 >= '0' && b1 <= '9') {
              c <<= 4;
              c |= b1 - '0';
            } else if (b1 >= 'A' && b1 <= 'F') {
              c <<= 4;
              c |= b1 + 10 - 'A';
            } else if (b1 >= 'a' && b1 <= 'f') {
              c <<= 4;
              c |= b1 + 10 - 'a';
            } else {
              throw new InvalidDataException("Invalid hex character");
            }
            if (b2 >= '0' && b2 <= '9') {
              c <<= 4;
              c |= b2 - '0';
            } else if (b2 >= 'A' && b2 <= 'F') {
              c <<= 4;
              c |= b2 + 10 - 'A';
            } else if (b2 >= 'a' && b2 <= 'f') {
              c <<= 4;
              c |= b2 + 10 - 'a';
            } else {
              throw new InvalidDataException("Invalid hex character");
            }
            outputStream.WriteByte((byte)c);
            i+=2;
          } else if (i + 1<length) {
            int b1=((int)data[i + 1]) & 0xff;
            if (b1=='\r') {
              // Soft line break
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Expected LF after CR");
              }
              ++i;
              lineStart=i+1;
              continue;
            } else if (b1=='\n') {
              // Soft line break
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Bare LF not expected");
              }
              ++i;
              lineStart=i+1;
              continue;
            } else {
              throw new InvalidDataException("Invalid data after equal sign");
            }
          } else {
            throw new InvalidDataException("Equal sign at end");
          }
        } else if (data[i]!='\t' && (data[i]<0x20 || data[i]>= 0x7f)) {
          throw new InvalidDataException("Invalid character in quoted-printable");
        } else if (data[i]==' ' || data[i]=='\t') {
          bool endsWithLineBreak = false;
          int lastSpace = i;
          for (int j = i + 1; j < length; ++j) {
            if (data[j]=='\r' || data[j]=='\n') {
              endsWithLineBreak = true;
            } else if (data[j]!=' ' && data[j]!='\t') {
              break;
            } else {
              lastSpace = j;
            }
          }
          // Ignore space/tab runs if the line ends in that run
          if (!endsWithLineBreak) {
            outputStream.Write(data, i, (lastSpace-i)+1);
          }
          i = lastSpace;
        } else {
          outputStream.WriteByte(data[i]);
        }
        if (!unlimitedLineLength && i>lineStart && (i-lineStart)+1>76) {
          throw new InvalidDataException("Encoded line too long");
        }
      }
    }

    public void TestDecodeQuotedPrintable(string input, string expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using(MemoryStream ms = new MemoryStream()) {
        ReadQuotedPrintable(ms, bytes, 0, bytes.Length, true, true);
        Assert.AreEqual(expectedOutput, DataUtilities.GetUtf8String(ms.ToArray(), true));
      }
    }
    public void TestFailQuotedPrintable(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using(MemoryStream ms = new MemoryStream()) {
        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.Length, true, true);
          Assert.Fail("Should have failed");
        } catch (InvalidDataException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }

    public void TestFailQuotedPrintableNonLenient(string input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      using(MemoryStream ms = new MemoryStream()) {
        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.Length, false, false);
          Assert.Fail("Should have failed");
        } catch (InvalidDataException) {
        } catch (Exception ex) {
          Assert.Fail(ex.ToString());
          throw new InvalidOperationException(String.Empty, ex);
        }
      }
    }
    public void TestQuotedPrintable(string input, int mode, string expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      StringBuilder sb = new StringBuilder();
      ToQuotedPrintableRfc2045(sb, bytes, 0, bytes.Length, mode);
      Assert.AreEqual(expectedOutput, sb.ToString());
    }

    public void TestQuotedPrintable(string input, string a, string b, string c) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, b);
      TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(string input, string a) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, a);
      TestQuotedPrintable(input, 2, a);
    }

    public string Repeat(string s, int count) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.Append(s);
      }
      return sb.ToString();
    }
    [Test]
    public void TestMessage(){
      foreach(string s in Directory.GetFiles(
        @"C:\Users\Peter\AppData\Local\Microsoft\Windows Live Mail",
        "*.eml",SearchOption.AllDirectories)){
        using(FileStream fs=new FileStream(s,FileMode.Open)){
          string msgstr=DataUtilities.ReadUtf8ToString(fs);
          try {
            Message msg=new Message(msgstr);
          } catch(NotSupportedException){
          } catch(InvalidDataException ex){
            Console.WriteLine(s);
            Console.WriteLine(ex.Message);
            //Console.WriteLine(ex.StackTrace);
          }
        }
      }
    }
    
    [Test]
    public void testCharset(){
      Assert.AreEqual("us-ascii",new MediaType("text/plain").GetCharset());
      Assert.AreEqual("us-ascii",new MediaType("TEXT/PLAIN").GetCharset());
      Assert.AreEqual("us-ascii",new MediaType("TeXt/PlAiN").GetCharset());
      Assert.AreEqual("us-ascii",new MediaType("text/xml").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; CHARSET=UTF-8").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; ChArSeT=UTF-8").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; charset=UTF-8").GetCharset());
      Assert.AreEqual("us-ascii",new MediaType("text/plain; charset = UTF-8").GetCharset());
      Assert.AreEqual("'utf-8'",new MediaType("text/plain; charset='UTF-8'").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("us-ascii",new MediaType("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; foo='; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; foo=bar; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",new MediaType("text/plain; charset=\"UTF-\\8\"").GetCharset());
    }
    
    [Test]
    public void TestDecode() {
      TestDecodeQuotedPrintable("test","test");
      TestDecodeQuotedPrintable("te \tst","te \tst");
      TestDecodeQuotedPrintable("te=61st","teast");
      TestDecodeQuotedPrintable("te=3dst","te=st");
      TestDecodeQuotedPrintable("te=c2=a0st","te\u00a0st");
      TestDecodeQuotedPrintable("te=3Dst","te=st");
      TestDecodeQuotedPrintable("te=0D=0Ast","te\r\nst");
      TestDecodeQuotedPrintable("te=0Dst","te\rst");
      TestDecodeQuotedPrintable("te=0Ast","te\nst");
      TestDecodeQuotedPrintable("te=C2=A0st","te\u00a0st");
      TestFailQuotedPrintable("te=3st");
      TestDecodeQuotedPrintable(Repeat("a",100),Repeat("a",100));
      TestDecodeQuotedPrintable("te\r\nst","te\r\nst");
      TestDecodeQuotedPrintable("te\rst","te\r\nst");
      TestDecodeQuotedPrintable("te\nst","te\r\nst");
      TestDecodeQuotedPrintable("te=\r\nst","test");
      TestDecodeQuotedPrintable("te=\rst","test");
      TestDecodeQuotedPrintable("te=\nst","test");
      TestDecodeQuotedPrintable("te=\r","te");
      TestDecodeQuotedPrintable("te=\n","te");
      TestFailQuotedPrintable("te=xy");
      TestFailQuotedPrintable("te\u000cst");
      TestFailQuotedPrintable("te\u007fst");
      TestFailQuotedPrintable("te\u00a0st");
      TestFailQuotedPrintable("te=3");
      TestDecodeQuotedPrintable("te   \r\nst","te\r\nst");
      TestDecodeQuotedPrintable("te   w\r\nst","te   w\r\nst");
      TestDecodeQuotedPrintable("te   =\r\nst","te   st");
      TestDecodeQuotedPrintable("te \t\r\nst","te\r\nst");
      TestDecodeQuotedPrintable("te\t \r\nst","te\r\nst");
      TestDecodeQuotedPrintable("te   \nst","te\r\nst");
      TestDecodeQuotedPrintable("te \t\nst","te\r\nst");
      TestDecodeQuotedPrintable("te\t \nst","te\r\nst");
      TestFailQuotedPrintableNonLenient("te\rst");
      TestFailQuotedPrintableNonLenient("te\nst");
      TestFailQuotedPrintableNonLenient("te=\rst");
      TestFailQuotedPrintableNonLenient("te=\nst");
      TestFailQuotedPrintableNonLenient("te=\r");
      TestFailQuotedPrintableNonLenient(Repeat("a",77));
      TestFailQuotedPrintableNonLenient(Repeat("=7F",26));
      TestFailQuotedPrintableNonLenient("aa\r\n"+Repeat("a",77));
      TestFailQuotedPrintableNonLenient("aa\r\n"+Repeat("=7F",26));
      TestFailQuotedPrintableNonLenient("te=\n");
      TestFailQuotedPrintableNonLenient("te   \rst");
      TestFailQuotedPrintableNonLenient("te   \nst");
    }

    [Test]
    public void TestStrip(){
      Assert.AreEqual("xyz",Message.StripCommentsAndExtraSpace("(abc)xyz"));
      Assert.AreEqual("xyz",Message.StripCommentsAndExtraSpace("(abc)xyz(def)"));
      Assert.AreEqual("xyz",Message.StripCommentsAndExtraSpace("(a(b)c)xyz(def)"));
      Assert.AreEqual("xyz",Message.StripCommentsAndExtraSpace("(abc)  xyz  "));
      Assert.AreEqual("xy z",Message.StripCommentsAndExtraSpace("  xy(abc)z  "));
      Assert.AreEqual("xyz",Message.StripCommentsAndExtraSpace("  xyz\t\t"));
      Assert.AreEqual("xy z",Message.StripCommentsAndExtraSpace("  xy\tz  "));
    }
    
    [Test]
    public void TestEncode() {
      TestQuotedPrintable("test","test");
      TestQuotedPrintable("te\u000cst","te=0Cst");
      TestQuotedPrintable("te\u007Fst","te=7Fst");
      TestQuotedPrintable("te st","te st");
      TestQuotedPrintable("te=st","te=3Dst");
      TestQuotedPrintable("te\r\nst","te=0D=0Ast","te\r\nst","te\r\nst");
      TestQuotedPrintable("te\rst","te=0Dst","te=0Dst","te\r\nst");
      TestQuotedPrintable("te\nst","te=0Ast","te=0Ast","te\r\nst");
      TestQuotedPrintable("te  \r\nst","te  =0D=0Ast","te =20\r\nst","te =20\r\nst");
      TestQuotedPrintable("te \r\nst","te =0D=0Ast","te=20\r\nst","te=20\r\nst");
      TestQuotedPrintable("te \t\r\nst","te \t=0D=0Ast","te =09\r\nst","te =09\r\nst");
      TestQuotedPrintable("te\t\r\nst","te\t=0D=0Ast","te=09\r\nst","te=09\r\nst");
      TestQuotedPrintable(Repeat("a",75),Repeat("a",75));
      TestQuotedPrintable(Repeat("a",76),Repeat("a",75)+"=\r\na");
      TestQuotedPrintable(Repeat("\u000c",30),Repeat("=0C",25)+"=\r\n"+Repeat("=0C",5));
    }
  }
}
