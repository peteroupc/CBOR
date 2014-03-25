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
import com.upokecenter.cbor.*;

  public class EncodingTest
  {
    private static String HexAlphabet="0123456789ABCDEF";

    private static void IncrementLineCount(StringBuilder str, int length, int[] count) {
      if (count[0]+length>75) { // 76 including the final '='
        str.append("=\r\n");
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
        throw new NullPointerException("str");
      }
      if ((data) == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)(offset)) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)(offset)) + ") is more than " + Long.toString((long)(data.length)));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)(count)) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)(count)) + ") is more than " + Long.toString((long)(data.length)));
      }
      if (data.length-offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length-offset)) + ") is less than " + Long.toString((long)(count)));
      }
      int length = offset + count;
      int[] lineCount = new int[] { 0};
      int i = offset;
      for (i = offset; i < length; ++i) {
        if (data[i]==0x0d) {
          if (lineBreakMode == 0) {
            IncrementLineCount(str, 3, lineCount);
            str.append("=0D");
          } else if (i + 1 >= length || data[i + 1]!=0x0a) {
            if (lineBreakMode == 2) {
              str.append("\r\n");
              lineCount[0]=0;
            } else {
              IncrementLineCount(str, 3, lineCount);
              str.append("=0D");
            }
          } else {
            ++i;
            str.append("\r\n");
            lineCount[0]=0;
          }
        } else if (data[i]==0x0a) {
          if (lineBreakMode == 2) {
            str.append("\r\n");
            lineCount[0]=0;
          } else {
            IncrementLineCount(str, 3, lineCount);
            str.append("=0A");
          }
        } else if (data[i]==9 || data[i]==32) {
          if (i + 2<length && lineBreakMode>0) {
            if (data[i + 1]==0x0d && data[i + 2]==0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.append(data[i]==9 ? "=09\r\n" : "=20\r\n");
              lineCount[0]=0;
              i+=2;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.append((char)data[i]);
            }
          } else if (i + 1<length && lineBreakMode == 2) {
            if (data[i + 1]==0x0d || data[i + 1]==0x0a) {
              IncrementLineCount(str, 3, lineCount);
              str.append(data[i]==9 ? "=09\r\n" : "=20\r\n");
              lineCount[0]=0;
              i+=1;
            } else {
              IncrementLineCount(str, 1, lineCount);
              str.append((char)data[i]);
            }
          } else {
            IncrementLineCount(str, 1, lineCount);
            str.append((char)data[i]);
          }
        } else if (data[i]==(byte)'=') {
          IncrementLineCount(str, 3, lineCount);
          str.append("=3D");
        } else if (data[i]>0x20 && data[i]<0x7f) {
          IncrementLineCount(str, 1, lineCount);
          str.append((char)data[i]);
        } else {
          IncrementLineCount(str, 3, lineCount);
          str.append('=');
          str.append(HexAlphabet.charAt((data[i] >> 4) & 15));
          str.append(HexAlphabet.charAt(data[i] & 15));
        }
      }
    }

    /**
     * Note: If lenientLineBreaks is true, treats CR, LF, and CRLF as line
     * breaks writes CRLF when encountering these breaks. Doesn't check
     * that no more than 76 characters are in each line. If an encoded line
     * ends with spaces and/or tabs, those characters are deleted (RFC 2045,
     * sec. 6.7, rule 3).
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
      if ((outputStream) == null) {
        throw new NullPointerException("outputStream");
      }
      if ((data) == null) {
        throw new NullPointerException("data");
      }
      if (offset < 0) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)(offset)) + ") is less than " + "0");
      }
      if (offset > data.length) {
        throw new IllegalArgumentException("offset (" + Long.toString((long)(offset)) + ") is more than " + Long.toString((long)(data.length)));
      }
      if (count < 0) {
        throw new IllegalArgumentException("count (" + Long.toString((long)(count)) + ") is less than " + "0");
      }
      if (count > data.length) {
        throw new IllegalArgumentException("count (" + Long.toString((long)(count)) + ") is more than " + Long.toString((long)(data.length)));
      }
      if (data.length-offset < count) {
        throw new IllegalArgumentException("data's length minus " + offset + " (" + Long.toString((long)(data.length-offset)) + ") is less than " + Long.toString((long)(count)));
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
            outputStream.write(0x0d);
            outputStream.write(0x0a);
          } else {
            outputStream.write(0x0d);
            outputStream.write(0x0a);
            ++i;
          }
          lineStart = i + 1;
        } else if (data[i]==0x0a) {
          if (!lenientLineBreaks) {
            throw new InvalidDataException("Bare LF not expected");
          }
          outputStream.write(0x0d);
          outputStream.write(0x0a);
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
            outputStream.write((byte)c);
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
          boolean endsWithLineBreak = false;
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
            outputStream.write(data,i,(lastSpace-i)+1);
          }
          i = lastSpace;
        } else {
          outputStream.write(data[i]);
        }
        if (!unlimitedLineLength && i>lineStart && (lineStart-i)+1>76) {
          throw new InvalidDataException("Encoded line too long");
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
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, b);
      TestQuotedPrintable(input, 2, c);
    }

    public void TestQuotedPrintable(String input, String a) {
      TestQuotedPrintable(input, 0, a);
      TestQuotedPrintable(input, 1, a);
      TestQuotedPrintable(input, 2, a);
    }

    public String Repeat(String s, int count) {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < count; ++i) {
        sb.append(s);
      }
      return sb.toString();
    }
    @Test
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

    @Test
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
