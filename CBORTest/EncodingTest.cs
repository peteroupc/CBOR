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
        } else if (data[i]==9) {
          IncrementLineCount(str, 3, lineCount);
          str.Append("=09");
        } else if (lineCount[0]==0 &&
                   data[i]==(byte)'.' && i+1<length && (data[i]=='\r' || data[i]=='\n')) {
          IncrementLineCount(str, 3, lineCount);
          str.Append("=2E");
        } else if (lineCount[0]==0 && i + 4<length &&
                   data[i]==(byte)'F' &&
                   data[i+1]==(byte)'r' &&
                   data[i+2]==(byte)'o' &&
                   data[i+3]==(byte)'m' &&
                   data[i+4]==(byte)' ') {
          // See page 7-8 of RFC 2049
          IncrementLineCount(str, 7, lineCount);
          str.Append("=46rom ");
          i+=4;
        } else if (data[i]==32) {
          if (i + 1 == length) {
            IncrementLineCount(str, 3, lineCount);
            str.Append(data[i]==9 ? "=09" : "=20");
            lineCount[0]=0;
          } else if (i + 2<length && lineBreakMode>0) {
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
        } else if (data[i]>0x20 && data[i]<0x7f && data[i]!=',' &&
                   "()'+-./?:".IndexOf((char)data[i])< 0) {
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
    /// CRLF as line breaks and writes CRLF when encountering these breaks.
    /// If unlimitedLineLength is true, doesn't check that no more than 76
    /// characters are in each line. If an encoded line ends with spaces and/or
    /// tabs, those characters are deleted (RFC 2045, sec. 6.7, rule 3).</summary>
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
      using(MemoryStream ms = new MemoryStream(data, offset, count)) {
        Message.QuotedPrintableTransform t = new Message.QuotedPrintableTransform(
          ms,
          lenientLineBreaks,
          unlimitedLineLength ? -1 : 76);
        while (true) {
          int c = t.ReadByte();
          if (c< 0) {
            return;
          }
          outputStream.WriteByte((byte)c);
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
    public void TestWordWrapOne(string firstWord, string nextWords, string expected) {
      var ww = new Message.WordWrapEncoder(firstWord);
      ww.AddString(nextWords);
      Console.WriteLine(ww.ToString());
      Assert.AreEqual(expected, ww.ToString());
    }

    [Test]
    public void TestWordWrap() {
      TestWordWrapOne("Subject:",Repeat("xxxx ",10)+"y","Subject: "+Repeat("xxxx ",10)+"y");
      TestWordWrapOne("Subject:",Repeat("xxxx ",10),"Subject: "+Repeat("xxxx ",9)+"xxxx");
    }

    [Test]
    public void TestHeaderFields() {
      string testString="Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>";
      HeaderParser.ParseMailboxList(testString, 0, testString.Length, null);
    }


    [Test]
    public void testCharset() {
      Assert.AreEqual("us-ascii",MediaType.Parse("text/plain").GetCharset());
      Assert.AreEqual("us-ascii",MediaType.Parse("TEXT/PLAIN").GetCharset());
      Assert.AreEqual("us-ascii",MediaType.Parse("TeXt/PlAiN").GetCharset());
      Assert.AreEqual("us-ascii",MediaType.Parse("text/xml").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; CHARSET=UTF-8").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; ChArSeT=UTF-8").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; charset=UTF-8").GetCharset());
      // Note that MIME implicitly allows whitespace around the equal sign
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; charset = UTF-8").GetCharset());
      Assert.AreEqual("'utf-8'",MediaType.Parse("text/plain; charset='UTF-8'").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("us-ascii",MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"").GetCharset());
      Assert.AreEqual("utf-8",MediaType.Parse("text/plain; charset=\"UTF-\\8\"").GetCharset());
    }

    public void TestRfc2231Extension(string mtype, string param, string expected) {
      var mt = MediaType.Parse(mtype);
      Assert.AreEqual(expected, mt.GetParameter(param));
    }

    public void SingleTestMediaTypeEncoding(string value, string expected) {
      MediaType mt=new MediaTypeBuilder("x","y").SetParameter("z",value).ToMediaType();
      string topLevel = mt.TopLevelType;
      string sub = mt.SubType;
      var mtstring="MIME-Version: 1.0\r\nContent-Type: "+mt.ToString()+"\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      using(MemoryStream ms = new MemoryStream(DataUtilities.GetUtf8Bytes(mtstring, true))) {
        var msg = new Message(ms);
        Assert.AreEqual(topLevel, msg.ContentType.TopLevelType);
        Assert.AreEqual(sub, msg.ContentType.SubType);
        Assert.AreEqual(value,msg.ContentType.GetParameter("z"),mt.ToString());
      }
    }

    [Test]
    public void TestMediaTypeEncoding() {
      SingleTestMediaTypeEncoding("xyz","x/y;z=xyz");
      SingleTestMediaTypeEncoding("xy z","x/y;z=\"xy z\"");
      SingleTestMediaTypeEncoding("xy\u00a0z","x/y;z*=utf-8''xy%C2%A0z");
      SingleTestMediaTypeEncoding("xy\ufffdz","x/y;z*=utf-8''xy%C2z");
      SingleTestMediaTypeEncoding("xy"+Repeat("\ufffc",50)+"z","x/y;z*=utf-8''xy"+Repeat("%EF%BF%BD",50)+"z");
      SingleTestMediaTypeEncoding("xy"+Repeat("\u00a0",50)+"z","x/y;z*=utf-8''xy"+Repeat("%C2%A0",50)+"z");
    }

    [Test]
    public void TestRfc2231Extensions() {
      TestRfc2231Extension("text/plain; charset=\"utf-8\"","charset","utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii'en'utf-8","charset","utf-8");
      TestRfc2231Extension("text/plain; charset*=us-ascii''utf-8","charset","utf-8");
      TestRfc2231Extension("text/plain; charset*='en'utf-8","charset","utf-8");
      TestRfc2231Extension("text/plain; charset*=''utf-8","charset","utf-8");
      TestRfc2231Extension("text/plain; charset*0=a;charset*1=b","charset","ab");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%20b","charset","a b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b","charset","a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b","charset","a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b","charset","a\u00a0b");
      TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b","charset","a\u00a0b");
      TestRfc2231Extension("text/plain; charset*0=\"a\";charset*1=b","charset","ab");
      TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d","charset","a bc d");
      TestRfc2231Extension("text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
                           "charset",
                           "abiso-8859-1'en'xyz");
      TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
                           "charset",
                           "a biso-8859-1'en'xyz");
      TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1=a%20b",
                           "charset",
                           "a ba%20b");
    }

    [Test]
    public void TestDecode() {
      TestDecodeQuotedPrintable("test","test");
      TestDecodeQuotedPrintable("te \tst","te \tst");
      TestDecodeQuotedPrintable("te=20","te ");
      TestDecodeQuotedPrintable("te=09","te\t");
      TestDecodeQuotedPrintable("te ","te");
      TestDecodeQuotedPrintable("te\t","te");
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
      TestDecodeQuotedPrintable("te  \r\n","te\r\n");
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
      TestFailQuotedPrintableNonLenient("te=\n");
      TestFailQuotedPrintableNonLenient("te   \rst");
      TestFailQuotedPrintableNonLenient("te   \nst");
      TestFailQuotedPrintableNonLenient(Repeat("a",77));
      TestFailQuotedPrintableNonLenient(Repeat("=7F",26));
      TestFailQuotedPrintableNonLenient("aa\r\n"+Repeat("a",77));
      TestFailQuotedPrintableNonLenient("aa\r\n"+Repeat("=7F",26));
    }
    public void TestEncodedWordsPhrase(string expected, string input) {
      Assert.AreEqual(expected+" <test@example.com>",HeaderFields.GetParser("from")
                      .ReplaceEncodedWords(input+" <test@example.com>"));
    }

    public void TestEncodedWordsOne(string expected, string input) {
      Assert.AreEqual(expected, Message.ReplaceEncodedWords(input));
      Assert.AreEqual("("+expected+") en",HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("("+input+") en"));
      Assert.AreEqual("  ("+expected+") en",HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("  ("+input+") en"));
      Assert.AreEqual("  (comment (cmt "+expected+")comment) en",
                      HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("  (comment (cmt "+input+")comment) en"));
      Assert.AreEqual("  (comment (=?bad?= "+expected+")comment) en",
                      HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("  (comment (=?bad?= "+input+")comment) en"));
      Assert.AreEqual("  (comment ("+expected+")comment) en",
                      HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("  (comment ("+input+")comment) en"));
      Assert.AreEqual("  ("+expected+"()) en",HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords("  ("+input+"()) en"));
      Assert.AreEqual(" en ("+expected+")",HeaderFields.GetParser("content-language")
                      .ReplaceEncodedWords(" en ("+input+")"));
      Assert.AreEqual(expected,HeaderFields.GetParser("subject")
                      .ReplaceEncodedWords(input));
    }

    [Test]
    public void TestEncodedWords() {
      TestEncodedWordsPhrase("(sss) y","(sss) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("xy","=?us-ascii?q?x?= =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?bad1?= =?bad2?= =?bad3?=","=?bad1?= =?bad2?= =?bad3?=");
      TestEncodedWordsPhrase("y =?bad2?= =?bad3?=","=?us-ascii?q?y?= =?bad2?= =?bad3?=");
      TestEncodedWordsPhrase("=?bad1?= y =?bad3?=","=?bad1?= =?us-ascii?q?y?= =?bad3?=");
      TestEncodedWordsPhrase("xy","=?us-ascii?q?x?=    =?us-ascii?q?y?=");
      TestEncodedWordsPhrase(" xy"," =?us-ascii?q?x?= =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("xy (sss)","=?us-ascii?q?x?= =?us-ascii?q?y?= (sss)");
      TestEncodedWordsPhrase("x (sss) y","=?us-ascii?q?x?= (sss) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("x (z) y","=?us-ascii?q?x?= (=?utf-8?q?z?=) =?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=(sss)=?us-ascii?q?y?=",
                             "=?us-ascii?q?x?=(sss)=?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=(z)=?us-ascii?q?y?=",
                             "=?us-ascii?q?x?=(=?utf-8?q?z?=)=?us-ascii?q?y?=");
      TestEncodedWordsPhrase("=?us-ascii?q?x?=(z) y",
                             "=?us-ascii?q?x?=(=?utf-8?q?z?=) =?us-ascii?q?y?=");
      //
      TestEncodedWordsOne("x y",("=?utf-8?q?x_?= =?utf-8?q?y?="));
      TestEncodedWordsOne("abcde abcde",("abcde abcde"));
      TestEncodedWordsOne("abcde",("abcde"));
      TestEncodedWordsOne("abcde",("=?utf-8?q?abcde?="));
      TestEncodedWordsOne("=?utf-8?q?abcde?=extra",("=?utf-8?q?abcde?=extra"));
      TestEncodedWordsOne("abcde  ",("=?utf-8?q?abcde?=  "));
      TestEncodedWordsOne(" abcde",(" =?utf-8?q?abcde?="));
      TestEncodedWordsOne("  abcde",("  =?utf-8?q?abcde?="));
      TestEncodedWordsOne("ab\u00a0de",("=?utf-8?q?ab=C2=A0de?="));
      TestEncodedWordsOne("xy",("=?utf-8?q?x?= =?utf-8?q?y?="));
      TestEncodedWordsOne("x y",("x =?utf-8?q?y?="));
      TestEncodedWordsOne("x   y",("x   =?utf-8?q?y?="));
      TestEncodedWordsOne("x y",("=?utf-8?q?x?= y"));
      TestEncodedWordsOne("x   y",("=?utf-8?q?x?=   y"));
      TestEncodedWordsOne("xy",("=?utf-8?q?x?=   =?utf-8?q?y?="));
      TestEncodedWordsOne("abc de",("=?utf-8?q?abc=20de?="));
      TestEncodedWordsOne("abc de",("=?utf-8?q?abc_de?="));
      TestEncodedWordsOne("abc\ufffdde",("=?us-ascii?q?abc=90de?="));
      TestEncodedWordsOne("=?x-undefined?q?abcde?=",("=?x-undefined?q?abcde?="));
      TestEncodedWordsOne("=?utf-8?q?"+Repeat("x",200)+"?=",
                          ("=?utf-8?q?"+Repeat("x",200)+"?="));
    }

    [Test]
    public void TestStrip() {
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
      TestQuotedPrintable("te ","te=20");
      TestQuotedPrintable("te\t","te=09");
      TestQuotedPrintable("te st","te st");
      TestQuotedPrintable("te=st","te=3Dst");
      TestQuotedPrintable("te\r\nst","te=0D=0Ast","te\r\nst","te\r\nst");
      TestQuotedPrintable("te\rst","te=0Dst","te=0Dst","te\r\nst");
      TestQuotedPrintable("te\nst","te=0Ast","te=0Ast","te\r\nst");
      TestQuotedPrintable("te  \r\nst","te  =0D=0Ast","te =20\r\nst","te =20\r\nst");
      TestQuotedPrintable("te \r\nst","te =0D=0Ast","te=20\r\nst","te=20\r\nst");
      TestQuotedPrintable("te \t\r\nst","te =09=0D=0Ast","te =09\r\nst","te =09\r\nst");
      TestQuotedPrintable("te\t\r\nst","te=09=0D=0Ast","te=09\r\nst","te=09\r\nst");
      TestQuotedPrintable(Repeat("a",75),Repeat("a",75));
      TestQuotedPrintable(Repeat("a",76),Repeat("a",75)+"=\r\na");
      TestQuotedPrintable(Repeat("\u000c",30),Repeat("=0C",25)+"=\r\n"+Repeat("=0C",5));
    }
  }
}
