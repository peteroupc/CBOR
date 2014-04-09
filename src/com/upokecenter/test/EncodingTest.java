package com.upokecenter.test; import com.upokecenter.util.*;
/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/24/2014
 * Time: 10:26 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

import java.io.*;

import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.util.*;
using PeterO.Mail;

  public class EncodingTest
  {
    private static String valueHexAlphabet = "0123456789ABCDEF";

    private static void IncrementLineCount(StringBuilder str, int length, int[] count) {
      if (count[0] + length > 75) { // 76 including the final '='
        str.append("=\r\n");
        count[0] = length;
      } else {
        count[0] += length;
      }
    }

    // lineBreakMode:
    // 0 - no line breaks
    // 1 - treat CRLF as a line break
    // 2 - treat CR, LF, and CRLF as a line break
    private static void ToQuotedPrintableRfc2045(
      StringBuilder str,
      byte[] data,
      int offset,
      int count,
      int lineBreakMode) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
      }
      int length = offset + count;
      int[] lineCount = new int[] { 0 };
      int i = offset;
      for (i = offset; i < length; ++i) {
        if (data[i] == 0x0d) {
          if (lineBreakMode == 0) {
            IncrementLineCount(str, 3, lineCount);
            str.append("=0D");
          } else if (i + 1 >= length || data[i + 1] != 0x0a) {
            if (lineBreakMode == 2) {
              str.append("\r\n");
              lineCount[0] = 0;
            } else {
              IncrementLineCount(str, 3, lineCount);
              str.append("=0D");
            }
          } else {
            ++i;
            str.append("\r\n");
            lineCount[0] = 0;
          }
        } else if (data[i] == 0x0a) {
          if (lineBreakMode == 2) {
            str.append("\r\n");
            lineCount[0] = 0;
          } else {
            IncrementLineCount(str, 3, lineCount);
            str.append("=0A");
          }
        } else if (data[i] == 9) {
          IncrementLineCount(str, 3, lineCount);
          str.append("=09");
        } else if (lineCount[0] == 0 &&
                   data[i] == (byte)'.' && i + 1 < length && (data[i] == '\r' || data[i] == '\n')) {
          IncrementLineCount(str, 3, lineCount);
          str.append("=2E");
        } else if (lineCount[0] == 0 && i + 4 < length &&
                   data[i] == (byte)'F' &&
                   data[i + 1] == (byte)'r' &&
                   data[i + 2] == (byte)'o' &&
                   data[i + 3] == (byte)'m' &&
                   data[i + 4] == (byte)' ') {
          // See page 7-8 of RFC 2049
          IncrementLineCount(str, 7, lineCount);
          str.append("=46rom ");
          i += 4;
        } else if (data[i] == 32) {
          if (i + 1 == length) {
            IncrementLineCount(str, 3, lineCount);
            str.append(data[i] == 9 ? "=09" : "=20");
            lineCount[0] = 0;
          } else if (i + 2 < length && lineBreakMode > 0) {
            if (data[i + 1] == 0x0d && data[i + 2] == 0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.append(data[i] == 9 ? "=09\r\n" : "=20\r\n");
              lineCount[0] = 0;
              i += 2;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.append((char)data[i]);
            }
          } else if (i + 1 < length && lineBreakMode == 2) {
            if (data[i + 1] == 0x0d || data[i + 1] == 0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.append(data[i] == 9 ? "=09\r\n" : "=20\r\n");
              lineCount[0] = 0;
              ++i;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.append((char)data[i]);
            }
          } else {
            IncrementLineCount(str, 1, lineCount);
            str.append((char)data[i]);
          }
        } else if (data[i] == (byte)'=') {
          IncrementLineCount(str, 3, lineCount);
          str.append("=3D");
        } else if (data[i] > 0x20 && data[i] < 0x7f && data[i] != ',' &&
                   "()'+-./?:".indexOf((char)data[i]) < 0) {
          IncrementLineCount(str, 1, lineCount);
          str.append((char)data[i]);
        } else {
          IncrementLineCount(str, 3, lineCount);
          str.append('=');
          str.append(valueHexAlphabet.charAt((data[i] >> 4) & 15));
          str.append(valueHexAlphabet.charAt(data[i] & 15));
        }
      }
    }

    /**
     * Note: If lenientLineBreaks is true, treats CR, LF, and CRLF as line
     * breaks and writes CRLF when encountering these breaks. If unlimitedLineLength
     * is true, doesn't check that no more than 76 characters are in each line.
     * If an encoded line ends with spaces and/or tabs, those characters
     * are deleted (RFC 2045, sec. 6.7, rule 3).
     * @param outputStream A readable data stream.
     * @param data A byte[] object.
     * @param offset A 32-bit signed integer.
     * @param count A 32-bit signed integer. (2).
     * @param lenientLineBreaks A Boolean object.
     * @param unlimitedLineLength A Boolean object. (2).
     */
    private static void ReadQuotedPrintable(
      OutputStream outputStream,
      byte[] data,
      int offset,
      int count,
      boolean lenientLineBreaks,
      boolean unlimitedLineLength) {
      if (outputStream == null) {
        throw new NullPointerException("outputStream");
      }
      if (data == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)offset) + ") is more than " + Long.toString((long)data.length));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)count) + ") is more than " + Long.toString((long)data.length));
      }
      if (data.length - offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length - offset)) + ") is less than " + Long.toString((long)count));
      }
      using (MemoryStream ms = new MemoryStream(data, offset, count)) {
        QuotedPrintableTransform t = new QuotedPrintableTransform(
          ms,
          lenientLineBreaks,
          unlimitedLineLength ? -1 : 76);
        while (true) {
          int c = t.read();
          if (c < 0) {
            return;
          }
          outputStream.write((byte)c);
        }
      }
    }

    public void TestDecodeQuotedPrintable(String input, String expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

        ReadQuotedPrintable(ms, bytes, 0, bytes.length, true, true);
        Assert.assertEquals(expectedOutput, DataUtilities.GetUtf8String(ms.toByteArray(), true));
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    public void TestFailQuotedPrintable(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.length, true, true);
          Assert.fail("Should have failed");
        } catch (InvalidDataException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    public void TestFailQuotedPrintableNonLenient(String input) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

        try {
          ReadQuotedPrintable(ms, bytes, 0, bytes.length, false, false);
          Assert.fail("Should have failed");
        } catch (InvalidDataException ex) {
        } catch (Exception ex) {
          Assert.fail(ex.toString());
          throw new IllegalStateException("", ex);
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    public void TestQuotedPrintable(String input, int mode, String expectedOutput) {
      byte[] bytes = DataUtilities.GetUtf8Bytes(input, true);
      StringBuilder sb = new StringBuilder();
      ToQuotedPrintableRfc2045(sb, bytes, 0, bytes.length, mode);
      Assert.assertEquals(expectedOutput, sb.toString());
    }

    public void TestQuotedPrintable(String input, String a, String b, String c) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, b);
      this.TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(String input, String a) {
      this.TestQuotedPrintable(input, 0, a);
      this.TestQuotedPrintable(input, 1, a);
      this.TestQuotedPrintable(input, 2, a);
    }

    public String Repeat(String s, int count) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.append(s);
      }
      return sb.toString();
    }

    private void TestParseDomain(String str, String expected) {
      Assert.assertEquals(str.length(), HeaderParser.ParseDomain(str, 0, str.length(), null));
      Assert.assertEquals(expected, HeaderParserUtility.ParseDomain(str, 0, str.length()));
    }

    private void TestParseLocalPart(String str, String expected) {
      Assert.assertEquals(str.length(), HeaderParser.ParseLocalPart(str, 0, str.length(), null));
      Assert.assertEquals(expected, HeaderParserUtility.ParseLocalPart(str, 0, str.length()));
    }

    @Test
    public void TestParseDomainAndLocalPart() {
      this.TestParseDomain("x","x");
      this.TestParseLocalPart("x","x");
      this.TestParseLocalPart("\"\"","");
      this.TestParseDomain("x.example","x.example");
      this.TestParseLocalPart("x.example","x.example");
      this.TestParseLocalPart("x.example\ud800\udc00.example.com","x.example\ud800\udc00.example.com");
      this.TestParseDomain("x.example\ud800\udc00.example.com","x.example\ud800\udc00.example.com");
      this.TestParseDomain("x.example.com","x.example.com");
      this.TestParseLocalPart("x.example.com","x.example.com");
      this.TestParseLocalPart("\"\"","");
      this.TestParseLocalPart("\"(not a comment)\"","(not a comment)");
      this.TestParseLocalPart("(comment1) x (comment2)","x");
      this.TestParseLocalPart("(comment1) example (comment2) . (comment3) com","example.com");
      this.TestParseDomain("(comment1) x (comment2)","x");
      this.TestParseDomain("(comment1) example (comment2) . (comment3) com","example.com");
      this.TestParseDomain("(comment1) [x] (comment2)","[x]");
      this.TestParseDomain("(comment1) [a.b.c.d] (comment2)","[a.b.c.d]");
      this.TestParseDomain("[]","[]");
      this.TestParseDomain("[a .\r\n b. c.d ]","[a.b.c.d]");
    }

    @Test
    public void TestWordWrapOne(String firstWord, String nextWords, String expected) {
      WordWrapEncoder ww=new WordWrapEncoder(firstWord);
      ww.AddString(nextWords);
      System.out.println(ww.toString());
      Assert.assertEquals(expected, ww.toString());
    }

    @Test
    public void TestWordWrap() {
      this.TestWordWrapOne("Subject:", this.Repeat("xxxx ", 10) +"y", "Subject: " + this.Repeat("xxxx ",10)+"y");
      this.TestWordWrapOne("Subject:", this.Repeat("xxxx ", 10), "Subject: " + this.Repeat("xxxx ", 9)+"xxxx");
    }

    @Test
    public void TestHeaderFields() {
      String testString = "Joe P Customer <customer@example.com>, Jane W Customer <jane@example.com>";
      HeaderParser.ParseMailboxList(testString, 0, testString.length(), null);
    }

    @Test
    public void testCharset() {
      Assert.assertEquals("us-ascii", MediaType.Parse("text/plain").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("TEXT/PLAIN").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("TeXt/PlAiN").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("text/xml").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; CHARSET=UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; ChArSeT=UTF-8").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=UTF-8").GetCharset());
      // Note that MIME implicitly allows whitespace around the equal sign
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset = UTF-8").GetCharset());
      Assert.assertEquals("'utf-8'", MediaType.Parse("text/plain; charset='UTF-8'").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo=\"\\\"\"; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("us-ascii", MediaType.Parse("text/plain; foo=\"; charset=\\\"UTF-8\\\"\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo='; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; foo=bar; charset=\"UTF-8\"").GetCharset());
      Assert.assertEquals("utf-8", MediaType.Parse("text/plain; charset=\"UTF-\\8\"").GetCharset());
    }

    public void TestRfc2231Extension(String mtype, String param, String expected) {
      var mt = MediaType.Parse(mtype);
      Assert.assertEquals(expected, mt.GetParameter(param));
    }

    public void SingleTestMediaTypeEncoding(String value, String expected) {
      MediaType mt = new MediaTypeBuilder("x", "y").SetParameter("z", value).ToMediaType();
      String topLevel = mt.getTopLevelType();
      String sub = mt.getSubType();
      var mtstring = "MIME-Version: 1.0\r\nContent-Type: " + mt.toString() +"\r\nContent-Transfer-Encoding: base64\r\n\r\n";
      java.io.ByteArrayInputStream ms=null;
try {
ms=new ByteArrayInputStream(DataUtilities.GetUtf8Bytes(mtstring, true));

        Message msg=new Message(ms);
        Assert.assertEquals(topLevel, msg.getContentType().TopLevelType);
        Assert.assertEquals(sub, msg.getContentType().SubType);
        Assert.assertEquals(mt.toString(),value,msg.getContentType().GetParameter("z"));
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    @Test
    public void TestMediaTypeEncoding() {
      this.SingleTestMediaTypeEncoding("xyz", "x/y;z=xyz");
      this.SingleTestMediaTypeEncoding("xy z", "x/y;z=\"xy z\"");
      this.SingleTestMediaTypeEncoding("xy\u00a0z", "x/y;z*=utf-8''xy%C2%A0z");
      this.SingleTestMediaTypeEncoding("xy\ufffdz", "x/y;z*=utf-8''xy%C2z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\ufffc", 50) +"z", "x/y;z*=utf-8''xy" + this.Repeat("%EF%BF%BD",50)+"z");
      this.SingleTestMediaTypeEncoding("xy" + this.Repeat("\u00a0", 50) +"z", "x/y;z*=utf-8''xy" + this.Repeat("%C2%A0",50)+"z");
    }

    @Test
    public void TestRfc2231Extensions() {
      this.TestRfc2231Extension("text/plain; charset=\"utf-8\"", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=us-ascii'en'utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=us-ascii''utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*='en'utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*=''utf-8", "charset", "utf-8");
      this.TestRfc2231Extension("text/plain; charset*0=a;charset*1=b", "charset", "ab");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%20b", "charset", "a b");
      this.TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=iso-8859-1''a%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*=utf-8''a%c2%a0b", "charset", "a\u00a0b");
      this.TestRfc2231Extension("text/plain; charset*0=\"a\";charset*1=b", "charset", "ab");
      this.TestRfc2231Extension("text/plain; charset*0*=utf-8''a%20b;charset*1*=c%20d", "charset", "a bc d");
      this.TestRfc2231Extension(
        "text/plain; charset*0=ab;charset*1*=iso-8859-1'en'xyz",
        "charset",
        "abiso-8859-1'en'xyz");
      this.TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1*=iso-8859-1'en'xyz",
        "charset",
        "a biso-8859-1'en'xyz");
      this.TestRfc2231Extension(
        "text/plain; charset*0*=utf-8''a%20b;charset*1=a%20b",
        "charset",
        "a ba%20b");
    }

    @Test
    public void TestDecode() {
      this.TestDecodeQuotedPrintable("test", "test");
      this.TestDecodeQuotedPrintable("te \tst", "te \tst");
      this.TestDecodeQuotedPrintable("te=20", "te ");
      this.TestDecodeQuotedPrintable("te=09", "te\t");
      this.TestDecodeQuotedPrintable("te ", "te");
      this.TestDecodeQuotedPrintable("te\t", "te");
      this.TestDecodeQuotedPrintable("te=61st", "teast");
      this.TestDecodeQuotedPrintable("te=3dst", "te=st");
      this.TestDecodeQuotedPrintable("te=c2=a0st", "te\u00a0st");
      this.TestDecodeQuotedPrintable("te=3Dst", "te=st");
      this.TestDecodeQuotedPrintable("te=0D=0Ast", "te\r\nst");
      this.TestDecodeQuotedPrintable("te=0Dst", "te\rst");
      this.TestDecodeQuotedPrintable("te=0Ast", "te\nst");
      this.TestDecodeQuotedPrintable("te=C2=A0st", "te\u00a0st");
      this.TestFailQuotedPrintable("te=3st");
      this.TestDecodeQuotedPrintable(this.Repeat("a", 100), this.Repeat("a", 100));
      this.TestDecodeQuotedPrintable("te\r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\rst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te=\r\nst", "test");
      this.TestDecodeQuotedPrintable("te=\rst", "test");
      this.TestDecodeQuotedPrintable("te=\nst", "test");
      this.TestDecodeQuotedPrintable("te=\r", "te");
      this.TestDecodeQuotedPrintable("te=\n", "te");
      this.TestFailQuotedPrintable("te=xy");
      this.TestFailQuotedPrintable("te\u000cst");
      this.TestFailQuotedPrintable("te\u007fst");
      this.TestFailQuotedPrintable("te\u00a0st");
      this.TestFailQuotedPrintable("te=3");
      this.TestDecodeQuotedPrintable("te \r\n", "te\r\n");
      this.TestDecodeQuotedPrintable("te \r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te w\r\nst", "te w\r\nst");
      this.TestDecodeQuotedPrintable("te =\r\nst", "te st");
      this.TestDecodeQuotedPrintable("te \t\r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\t \r\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te \nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te \t\nst", "te\r\nst");
      this.TestDecodeQuotedPrintable("te\t \nst", "te\r\nst");
      this.TestFailQuotedPrintableNonLenient("te\rst");
      this.TestFailQuotedPrintableNonLenient("te\nst");
      this.TestFailQuotedPrintableNonLenient("te=\rst");
      this.TestFailQuotedPrintableNonLenient("te=\nst");
      this.TestFailQuotedPrintableNonLenient("te=\r");
      this.TestFailQuotedPrintableNonLenient("te=\n");
      this.TestFailQuotedPrintableNonLenient("te \rst");
      this.TestFailQuotedPrintableNonLenient("te \nst");
      this.TestFailQuotedPrintableNonLenient(this.Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient(this.Repeat("=7F", 26));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + this.Repeat("a", 77));
      this.TestFailQuotedPrintableNonLenient("aa\r\n" + this.Repeat("=7F", 26));
    }

    public void TestEncodedWordsPhrase(String expected, String input) {
      Assert.assertEquals(
        expected + " <test@example.com>",
        HeaderFields.GetParser("from") .ReplaceEncodedWords(input + " <test@example.com>"));
    }

    public void TestEncodedWordsOne(String expected, String input) {
      String par = "(";
      Assert.assertEquals(expected, Message.ReplaceEncodedWords(input));
      Assert.assertEquals(
        "(" + expected + ") en",
        HeaderFields.GetParser("content-language") .ReplaceEncodedWords("(" + input + ") en"));
      Assert.assertEquals(
        " (" + expected + ") en",
        HeaderFields.GetParser("content-language") .ReplaceEncodedWords(" (" + input + ") en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "cmt "+expected+")comment) en",
        HeaderFields.GetParser("content-language")
        .ReplaceEncodedWords(" (comment (cmt " + input + ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par + "=?bad?= "+expected+")comment) en",
        HeaderFields.GetParser("content-language")
        .ReplaceEncodedWords(" (comment (=?bad?= " + input + ")comment) en"));
      Assert.assertEquals(
        " " + par + "comment " + par+""+expected+")comment) en",
        HeaderFields.GetParser("content-language")
        .ReplaceEncodedWords(" (comment (" + input + ")comment) en"));
      Assert.assertEquals(
        " (" + expected + "()) en",
        HeaderFields.GetParser("content-language").ReplaceEncodedWords(" (" + input + "()) en"));
      Assert.assertEquals(
        " en (" + expected + ")",
        HeaderFields.GetParser("content-language").ReplaceEncodedWords(" en (" + input + ")"));
      Assert.assertEquals(
        expected,
        HeaderFields.GetParser("subject") .ReplaceEncodedWords(input));
    }

    @Test
    public void TestCommentsToWords() {
      String par = "(";
      Assert.assertEquals("(=?utf-8?q?x?=)", Message.ConvertCommentsToEncodedWords("(x)"));
      Assert.assertEquals("(=?utf-8?q?xy?=)", Message.ConvertCommentsToEncodedWords("(x\\y)"));
      Assert.assertEquals("(=?utf-8?q?x_y?=)", Message.ConvertCommentsToEncodedWords("(x\r\n y)"));
      Assert.assertEquals("(=?utf-8?q?x=C2=A0?=)", Message.ConvertCommentsToEncodedWords("(x\u00a0)"));
      Assert.assertEquals("(=?utf-8?q?x=C2=A0?=)", Message.ConvertCommentsToEncodedWords("(x\\\u00a0)"));
      Assert.assertEquals("(=?utf-8?q?x?=())", Message.ConvertCommentsToEncodedWords("(x())"));
      Assert.assertEquals("(=?utf-8?q?x?=()=?utf-8?q?y?=)", Message.ConvertCommentsToEncodedWords("(x()y)"));
      Assert.assertEquals("(=?utf-8?q?x?=(=?utf-8?q?ab?=)=?utf-8?q?y?=)", Message.ConvertCommentsToEncodedWords("(x(a\\b)y)"));
      Assert.assertEquals("()", Message.ConvertCommentsToEncodedWords("()"));
      Assert.assertEquals("(test) x@x.example", HeaderFields.GetParser("from").DowngradeComments("(test) x@x.example"));
      Assert.assertEquals(
        "(=?utf-8?q?tes=C2=BEt?=) x@x.example",
        HeaderFields.GetParser("from").DowngradeComments("(tes\u00bet) x@x.example"));
      Assert.assertEquals(
        "(=?utf-8?q?tes=C2=BEt?=) en",
        HeaderFields.GetParser("content-language").DowngradeComments("(tes\u00bet) en"));
      Assert.assertEquals(
        par + "tes\u00bet) x@x.example",
        HeaderFields.GetParser("subject").DowngradeComments("(tes\u00bet) x@x.example"));
      Assert.assertEquals("(=?utf-8?q?tes=0Dt?=) x@x.example",HeaderFields.GetParser("from")
                      .DowngradeComments("(tes\rt) x@x.example"));
    }

    @Test
    public void TestEncodedWords() {
      String par = "(";
      this.TestEncodedWordsPhrase("(sss) y", "(sss) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("=?bad1?= =?bad2?= =?bad3?=", "=?bad1?= =?bad2?= =?bad3?=");
      this.TestEncodedWordsPhrase("y =?bad2?= =?bad3?=", "=?us-ascii?q?y?= =?bad2?= =?bad3?=");
      this.TestEncodedWordsPhrase("=?bad1?= y =?bad3?=", "=?bad1?= =?us-ascii?q?y?= =?bad3?=");
      this.TestEncodedWordsPhrase("xy", "=?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(" xy", " =?us-ascii?q?x?= =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("xy (sss)", "=?us-ascii?q?x?= =?us-ascii?q?y?= (sss)");
      this.TestEncodedWordsPhrase("x (sss) y", "=?us-ascii?q?x?= (sss) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase("x (z) y", "=?us-ascii?q?x?= (=?utf-8?q?z?=) =?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "sss)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(sss)=?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "z)=?us-ascii?q?y?=",
        "=?us-ascii?q?x?=(=?utf-8?q?z?=)=?us-ascii?q?y?=");
      this.TestEncodedWordsPhrase(
        "=?us-ascii?q?x?=" + par + "z) y",
        "=?us-ascii?q?x?=(=?utf-8?q?z?=) =?us-ascii?q?y?=");
      //
      this.TestEncodedWordsOne("x y", "=?utf-8?q?x_?= =?utf-8?q?y?=");
      this.TestEncodedWordsOne("abcde abcde", "abcde abcde");
      this.TestEncodedWordsOne("abcde", "abcde");
      this.TestEncodedWordsOne("abcde", "=?utf-8?q?abcde?=");
      this.TestEncodedWordsOne("=?utf-8?q?abcde?=extra", "=?utf-8?q?abcde?=extra");
      this.TestEncodedWordsOne("abcde ", "=?utf-8?q?abcde?= ");
      this.TestEncodedWordsOne(" abcde", " =?utf-8?q?abcde?=");
      this.TestEncodedWordsOne(" abcde", " =?utf-8?q?abcde?=");
      this.TestEncodedWordsOne("ab\u00a0de", "=?utf-8?q?ab=C2=A0de?=");
      this.TestEncodedWordsOne("xy", "=?utf-8?q?x?= =?utf-8?q?y?=");
      this.TestEncodedWordsOne("x y", "x =?utf-8?q?y?=");
      this.TestEncodedWordsOne("x y", "x =?utf-8?q?y?=");
      this.TestEncodedWordsOne("x y", "=?utf-8?q?x?= y");
      this.TestEncodedWordsOne("x y", "=?utf-8?q?x?= y");
      this.TestEncodedWordsOne("xy", "=?utf-8?q?x?= =?utf-8?q?y?=");
      this.TestEncodedWordsOne("abc de", "=?utf-8?q?abc=20de?=");
      this.TestEncodedWordsOne("abc de", "=?utf-8?q?abc_de?=");
      this.TestEncodedWordsOne("abc\ufffdde", "=?us-ascii?q?abc=90de?=");
      this.TestEncodedWordsOne("=?x-undefined?q?abcde?=", "=?x-undefined?q?abcde?=");
      this.TestEncodedWordsOne("=?utf-8?q?"+this.Repeat("x",200)+"?=",
                               "=?utf-8?q?" + this.Repeat("x", 200) +"?=");
    }

    @Test
    public void TestEncode() {
      this.TestQuotedPrintable("test", "test");
      this.TestQuotedPrintable("te\u000cst", "te=0Cst");
      this.TestQuotedPrintable("te\u007Fst", "te=7Fst");
      this.TestQuotedPrintable("te ", "te=20");
      this.TestQuotedPrintable("te\t", "te=09");
      this.TestQuotedPrintable("te st", "te st");
      this.TestQuotedPrintable("te=st", "te=3Dst");
      this.TestQuotedPrintable("te\r\nst", "te=0D=0Ast", "te\r\nst", "te\r\nst");
      this.TestQuotedPrintable("te\rst", "te=0Dst", "te=0Dst", "te\r\nst");
      this.TestQuotedPrintable("te\nst", "te=0Ast", "te=0Ast", "te\r\nst");
      this.TestQuotedPrintable("te \r\nst", "te =0D=0Ast", "te =20\r\nst", "te =20\r\nst");
      this.TestQuotedPrintable("te \r\nst", "te =0D=0Ast", "te=20\r\nst", "te=20\r\nst");
      this.TestQuotedPrintable("te \t\r\nst", "te =09=0D=0Ast", "te =09\r\nst", "te =09\r\nst");
      this.TestQuotedPrintable("te\t\r\nst", "te=09=0D=0Ast", "te=09\r\nst", "te=09\r\nst");
      this.TestQuotedPrintable(this.Repeat("a", 75), this.Repeat("a", 75));
      this.TestQuotedPrintable(this.Repeat("a", 76), this.Repeat("a", 75) +"=\r\na");
      this.TestQuotedPrintable(this.Repeat("\u000c", 30), this.Repeat("=0C", 25) +"=\r\n" + this.Repeat("=0C",5));
    }

    public static void Timeout(int duration, Action action) {
      String stackTrace = null;
      Object stackTraceLock = new Object();
      System.Threading.Thread thread = new Thread(new Runnable(){ public void run() {  {
          try {
            action();
          } catch (Exception ex) {
            synchronized(stackTraceLock) {
              stackTrace = ex.getClass().getFullName() + "\n" + ex.getMessage() + "\n" + ex.getStackTrace();
              System.Threading.Monitor.PulseAll(stackTraceLock);
            }
          }
        } }});
      thread.start();
      if (!thread.join(duration)) {
        @SuppressWarnings("deprecation") thread.stop();
        String trace = null;
        synchronized(stackTraceLock) {
          while (stackTrace == null) {
            System.Threading.Monitor.Wait(stackTraceLock);
          }
          trace = stackTrace;
        }
        if (trace != null) {
          Assert.fail(trace);
        }
      }
    }
  }
