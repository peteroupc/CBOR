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
        } else if (data[i]==9 || data[i]==32) {
          if (i + 1 == length) {
            IncrementLineCount(str, 3, lineCount);
            str.Append(data[i]==9 ? "=09" : "=20");
            lineCount[0]=0;
          }
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
              lineStart = i + 1;
              continue;
            } else if (b1=='\n') {
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Bare LF not expected");
              }
              ++i;
              lineStart = i + 1;
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
              lineStart = i + 1;
              continue;
            } else if (b1=='\n') {
              // Soft line break
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Bare LF not expected");
              }
              ++i;
              lineStart = i + 1;
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
          if (lastSpace == length-1) {
            endsWithLineBreak = true;
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
    public void TestMessage() {
      foreach(string s in Directory.GetFiles(
        @"C:\Users\Peter\AppData\Local\Microsoft\Windows Live Mail",
        "*.eml",
        SearchOption.AllDirectories)) {
        using(FileStream fs = new FileStream(s, FileMode.Open)) {
          string msgstr = DataUtilities.ReadUtf8ToString(fs);
          try {
            Message msg = new Message(msgstr);
          } catch(NotSupportedException) {
          } catch(InvalidDataException ex) {
            Console.WriteLine(s);
            Console.WriteLine(ex.Message);
            //Console.WriteLine(ex.StackTrace);
          }
        }
      }
    }

    [Test]
    public void testCharset() {
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
      TestQuotedPrintable("te \t\r\nst","te \t=0D=0Ast","te =09\r\nst","te =09\r\nst");
      TestQuotedPrintable("te\t\r\nst","te\t=0D=0Ast","te=09\r\nst","te=09\r\nst");
      TestQuotedPrintable(Repeat("a",75),Repeat("a",75));
      TestQuotedPrintable(Repeat("a",76),Repeat("a",75)+"=\r\na");
      TestQuotedPrintable(Repeat("\u000c",30),Repeat("=0C",25)+"=\r\n"+Repeat("=0C",5));
    }
  }
}
