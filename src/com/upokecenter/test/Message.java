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

import java.io.*;

import org.junit.Assert;

import com.upokecenter.util.*;
import com.upokecenter.cbor.*;

  final class Message {
    private List<String> headers;

    private List<Message> parts;

    private static final int EncodingSevenBit = 0;
    private static final int EncodingUnknown=-1;
    private static final int EncodingEightBit = 3;
    private static final int EncodingBinary = 4;
    private static final int EncodingQuotedPrintable = 1;
    private static final int EncodingBase64 = 2;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<Message> getParts() {
        return parts;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<String> getHeaders() {
        return headers;
      }

    private byte[] body;

    /**
     * Gets a value not documented yet.
     * @return A byte[] object.
     */
    public byte[] GetBody() {
      return body;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     */
    public void SetBody(String str) {
      body = DataUtilities.GetUtf8Bytes(str, true);
      contentType=MediaType.Parse("text/plain; charset=utf-8");
    }

    public Message (InputStream stream) {
      if ((stream) == null) {
        throw new NullPointerException("stream");
      }
      headers = new ArrayList<String>();
      parts = new ArrayList<Message>();
      ReadMessage(new WrappedStream(stream));
    }
    public Message () {
      headers = new ArrayList<String>();
      parts = new ArrayList<Message>();
    }

    static int skipComment(String str, int index, int endIndex) {
      int startIndex = index;
      if (!(index<endIndex && str.charAt(index)=='('))
        return index;
      ++index;
      while (index<endIndex) {
        // Skip tabs and spaces (should skip
        // folding whitespace too, but this method assumes
        // unfolded values)
        index = ParserUtility.SkipSpaceAndTab(str, index, endIndex);
        char c = str.charAt(index);
        if (c==')') {
          return index + 1;
        }
        int oldIndex = index;
        // skip comment character (RFC5322 sec. 3.2.1)
        if (index<endIndex) {
          c = str.charAt(index);
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
        if (index+1<endIndex && str.charAt(index)=='\\') {
          c = str.charAt(index + 1);
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
    static String ReplaceEncodedWords(String str) {
      if ((str) == null) {
        throw new NullPointerException("str");
      }
      return ReplaceEncodedWords(str, 0, str.length(), false);
    }

    static String ReplaceEncodedWords(String str, int index, int endIndex, boolean inComments) {

      if (endIndex-index< 9) {
        return str.substring(index,(index)+(endIndex-index));
      }
      if (str.indexOf('=')< 0) {
        return str.substring(index,(index)+(endIndex-index));
      }
      StringBuilder builder = new StringBuilder();
      boolean lastWordWasEncodedWord = false;
      int whitespaceStart=-1;
      int whitespaceEnd=-1;
      while (index<endIndex) {
        int charCount = 2;
        boolean acceptedEncodedWord = false;
        String decodedWord = null;
        int startIndex = 0;
        boolean havePossibleEncodedWord = false;
        boolean startParen = false;
        if (index+1<endIndex && str.charAt(index)=='=' && str.charAt(index+1)=='?') {
          startIndex = index + 2;
          index+=2;
          havePossibleEncodedWord = true;
        } else if (inComments && index+2<endIndex && str.charAt(index)=='(' &&
                   str.charAt(index+1)=='=' && str.charAt(index+2)=='?') {
          startIndex = index + 3;
          index+=3;
          startParen = true;
          havePossibleEncodedWord = true;
        }
        if (havePossibleEncodedWord) {
          boolean maybeWord = true;
          int afterLast = endIndex;
          while (index<endIndex) {
            char c = str.charAt(index);
            ++index;
            // Check for a run of printable ASCII characters (except space)
            // with length up to 75 (also exclude '(' and ')' if 'inComments'
            // is true)
            if (c >= 0x21 && c<0x7e && (!inComments || (c!='(' && c!=')'))) {
              ++charCount;
              if (charCount>75) {
                maybeWord = false;
                index = startIndex-2;
                break;
              }
            } else {
              afterLast = index-1;
              break;
            }
          }
          if (maybeWord) {
            // May be an encoded word
            //System.out.println("maybe "+str.substring(startIndex-2,(startIndex-2)+(afterLast-(startIndex-2))));
            index = startIndex;
            int i2;
            // Parse charset
            // (NOTE: Compatible with RFC 2231's addition of language
            // to charset, since charset is defined as a 'token' in
            // RFC 2047, which includes '*')
            int charsetEnd=-1;
            int encodedTextStart=-1;
            boolean base64 = false;
            i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
            if (i2!=index && i2<endIndex && str.charAt(i2)=='?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
              if (i2!=index && i2<endIndex && str.charAt(i2)=='?') {
                // check for supported encoding (B or Q)
                char encodingChar = str.charAt(index);
                if (i2-index==1 && (encodingChar=='b' || encodingChar=='B' ||
                                    encodingChar=='q' || encodingChar=='Q')) {
                  // Parse encoded text
                  base64=(encodingChar=='b' || encodingChar=='B');
                  index = i2 + 1;
                  encodedTextStart = index;
                  i2 = MediaType.skipEncodedTextRfc2047(str, index, afterLast, inComments);
                  if (i2!=index && i2+1<endIndex && str.charAt(i2)=='?' && str.charAt(i2+1)=='=' &&
                      i2 + 2 == afterLast) {
                    acceptedEncodedWord = true;
                    i2+=2;
                  }
                }
              }
            }
            if (acceptedEncodedWord) {
              String charset = str.substring(startIndex,(startIndex)+(charsetEnd-startIndex));
              String encodedText = str.substring(encodedTextStart,(encodedTextStart)+((afterLast-2)-
                                                 encodedTextStart));
              int asterisk=charset.indexOf('*');
              if (asterisk >= 1) {
                charset = str.substring(0,asterisk);
                String language = str.substring(asterisk + 1,(asterisk + 1)+(str.length()-(asterisk + 1)));
                if (!ParserUtility.IsValidLanguageTag(language)) {
                  acceptedEncodedWord = false;
                }
              } else if (asterisk == 0) {
                // Impossible, a charset can't start with an asterisk
                acceptedEncodedWord = false;
              }
              if (acceptedEncodedWord) {
                ITransform transform=(base64) ?
                  (ITransform)new BEncodingStringTransform(encodedText) :
                  (ITransform)new QEncodingStringTransform(encodedText);
                Charsets.ICharset encoding = Charsets.GetCharset(charset);
                if (encoding == null) {
                  System.out.println("Unknown charset "+charset);
                  decodedWord = str.substring(startIndex-2,(startIndex-2)+(afterLast-(startIndex-2)));
                } else {
                  //System.out.println("Encoded " + (base64 ? "B" : "Q") + " to: " + (encoding.GetString(transform)));
                  decodedWord = encoding.GetString(transform);
                }
                // TODO: decodedWord may itself be part of an encoded word
                // or contain ASCII control characters: encoded word decoding is
                // not idempotent; if this is a comment it could also contain '(', ')', and '\'
              } else {
                decodedWord = str.substring(startIndex-2,(startIndex-2)+(afterLast-(startIndex-2)));
              }
            } else {
              decodedWord = str.substring(startIndex-2,(startIndex-2)+(afterLast-(startIndex-2)));
            }
            index = afterLast;
          }
        }
        if (whitespaceStart >= 0 && whitespaceStart<whitespaceEnd &&
            (!acceptedEncodedWord || !lastWordWasEncodedWord)) {
          // Append whitespace as long as it doesn't occur between two
          // encoded words
          builder.append(str.substring(whitespaceStart,(whitespaceStart)+(whitespaceEnd-whitespaceStart)));
        }
        if (startParen) {
          builder.append('(');
        }
        if (decodedWord != null) {
          builder.append(decodedWord);
        }
        // System.out.println("" + index + " " + endIndex + " [" + (index<endIndex ? str.charAt(index) : '~') + "]");
        // Read to whitespace
        int oldIndex = index;
        while (index<endIndex) {
          char c = str.charAt(index);
          if (c == 0x09 || c == 0x20) {
            break;
          } else {
            ++index;
          }
        }
        boolean hasNonWhitespace=(oldIndex != index);
        whitespaceStart = index;
        // Read to nonwhitespace
        while (index<endIndex) {
          char c = str.charAt(index);
          if (c == 0x09 || c == 0x20) {
            ++index;
          } else {
            break;
          }
        }
        whitespaceEnd = index;
        if (builder.length() == 0 && oldIndex == 0 && index == str.length()) {
          // Nothing to replace, and the whole String
          // is being checked
          return str;
        }
        if (oldIndex != index) {
          // Append nonwhitespace only, unless this is the end
          if (index == endIndex) {
            builder.append(str.substring(oldIndex,(oldIndex)+(index-oldIndex)));
          } else {
            builder.append(str.substring(oldIndex,(oldIndex)+(whitespaceStart-oldIndex)));
          }
        }
        lastWordWasEncodedWord = acceptedEncodedWord;
      }
      return builder.toString();
    }

    static int SkipCommentsAndWhitespace(
      String str,
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

    static String StripCommentsAndExtraSpace(String s) {
      StringBuilder sb = null;
      int index = 0;
      while (index<s.length()) {
        char c = s.charAt(index);
        if (c=='(' || c==0x09 || c==0x20) {
          int wsp = SkipCommentsAndWhitespace(s, index, s.length());
          if (sb == null) {
            sb = new StringBuilder();
            sb.append(s.substring(0,index));
          }
          if (sb.length()>0) {
            sb.append(' ');
          }
          if (index == wsp) {
            // Might not be a valid comment or whitespace
            ++index;
          } else {
            index = wsp;
          }
          continue;
        } else {
          if (sb != null) {
            sb.append(c);
          }
        }
        ++index;
      }
      String ret=(sb == null) ? s : sb.toString();
      int trimLen = ret.length();
      for (int i = trimLen-1;i >= 0; --i) {
        if (ret.charAt(i)==' ') {
          --trimLen;
        } else {
          break;
        }
      }
      if (trimLen != ret.length()) {
        return ret.substring(0,trimLen);
      } else {
        return ret;
      }
    }

    private MediaType contentType;
    private int transferEncoding;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public MediaType getContentType() {
        return contentType;
      }

    private void ProcessHeaders(boolean assumeMime, boolean digest) {
      boolean haveContentType = false;
      boolean mime = assumeMime;
      for (int i = 0;i<headers.size();i+=2) {
        String name = headers.get(i);
        String value = headers.get(i + 1);
        if (name.equals("from")) {
          if (HeaderParser.ParseHeaderFrom(value, 0, value.length(), null) == 0) {
            System.out.println(GetHeader("date"));
            // throw new InvalidDataException("Invalid From header: "+value);
          }
        }
        if (name.equals("to") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid To header: "+value);
          }
        }
        if (name.equals("cc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderCc(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid Cc header: "+value);
          }
        }
        if (name.equals("bcc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderBcc(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid Bcc header: "+value);
          }
        }
        value = HeaderFields.GetParser(name).ReplaceEncodedWords(value);
        if (name.equals("content-transfer-encoding")) {
          value = StripCommentsAndExtraSpace(value);
        }
        if (name.equals("mime-version")) {
          mime = true;
        }
        headers.set(i + 1,value);
      }
      boolean haveFrom = false;
      boolean haveSubject = false;
      boolean haveTo = false;
      // TODO: Treat message/rfc822 specially
      contentType = digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii;
      for (int i = 0;i<headers.size();i+=2) {
        String name = headers.get(i);
        String value = headers.get(i + 1);
        if (mime && name.equals("content-transfer-encoding")) {
          value = ParserUtility.ToLowerCaseAscii(value);
          headers.set(i + 1,value);
          if (value.equals("7bit")) {
            transferEncoding = EncodingSevenBit;
          } else if (value.equals("8bit")) {
            transferEncoding = EncodingEightBit;
          } else if (value.equals("binary")) {
            transferEncoding = EncodingBinary;
          } else if (value.equals("quoted-printable")) {
            transferEncoding = EncodingQuotedPrintable;
          } else if (value.equals("base64")) {
            transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            transferEncoding = EncodingUnknown;
          }
          headers.remove(i);
          headers.remove(i);
          i-=2;
        } else if (mime && name.equals("content-type")) {
          if (haveContentType) {
            throw new InvalidDataException("Already have this header: "+name);
          }
          contentType = MediaType.Parse(value,
                                        digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
          headers.remove(i);
          headers.remove(i);
          i-=2;
        } else if (name.equals("from")) {
          if (haveFrom) {
            throw new InvalidDataException("Already have this header: "+name);
          }
          haveFrom = true;
        } else if (name.equals("to")) {
          if (haveTo) {
            throw new InvalidDataException("Already have this header: "+name);
          }
          haveTo = true;
        } else if (name.equals("subject")) {
          if (haveSubject) {
            throw new InvalidDataException("Already have this header: "+name);
          }
          haveSubject = true;
        }
      }
      if (transferEncoding == EncodingUnknown) {
        contentType=MediaType.Parse("application/octet-stream");
      }
      if (transferEncoding == EncodingQuotedPrintable ||
          transferEncoding == EncodingBase64 ||
          transferEncoding == EncodingUnknown) {
        if (contentType.getTopLevelType().equals("multipart") ||
            contentType.getTopLevelType().equals("message")) {
          throw new InvalidDataException("Invalid content encoding for multipart or message");
        }
      }
    }

    private static boolean IsWellFormedBoundary(String str) {
      if (str == null || str.length()<1 || str.length()>70) {
        return false;
      }
      for (int i = 0;i<str.length(); ++i) {
        char c = str.charAt(i);
        if (i == str.length()-1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c>= 'A' && c<= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9') ||
          c==0x20 || c==0x2c || "'()-./+_:=?".indexOf(c)>= 0)) {
          return false;
        }
      }
      return true;
    }

    private static final class WrappedStream implements ITransform {
      InputStream stream;
      public WrappedStream (InputStream stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        return stream.read();
      }
    }

    private static final class StreamWithUnget implements ITransform {
      ITransform stream;
      int lastByte;
      boolean unget;
      public StreamWithUnget (InputStream stream) {
        lastByte=-1;
        this.stream = new WrappedStream(stream);
      }

      public StreamWithUnget (ITransform stream) {
        lastByte=-1;
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (unget) {
          unget = false;
        } else {
          lastByte = stream.read();
        }
        return lastByte;
      }

    /**
     * Not documented yet.
     */
      public void Unget() {
        unget = true;
      }
    }

    interface ITransform {
      int ReadByte();
    }

    private static final class EightBitTransform implements ITransform {
      ITransform stream;
      public EightBitTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        int ret = this.stream.read();
        if (ret == 0) {
          throw new InvalidDataException("Invalid character in message body");
        }
        return ret;
      }
    }

    private static final class BinaryTransform implements ITransform {
      ITransform stream;
      public BinaryTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        return this.stream.read();
      }
    }

    private static final class SevenBitTransform implements ITransform {
      ITransform stream;

      public SevenBitTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        int ret = this.stream.read();
        if (ret>0x80 || ret == 0) {
          throw new InvalidDataException("Invalid character in message body");
        }
        return ret;
      }
    }

    // A seven-bit transform used for text/plain data
    private static final class LiberalSevenBitTransform implements ITransform {
      ITransform stream;

      public LiberalSevenBitTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        int ret = this.stream.read();
        if (ret>0x80 || ret == 0) {
          return '?';
        }
        return ret;
      }
    }

    private static final class Base64Transform implements ITransform {
      static int[] Alphabet = new int[] {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1
      };
      StreamWithUnget input;
      int lineCharCount;
      boolean lenientLineBreaks;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;

      private static final int MaxLineSize = 76;

      public Base64Transform (ITransform input, boolean lenientLineBreaks) {
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
        this.buffer = new byte[4];
      }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        bufferCount = size;
        bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        int value = 0;
        int count = 0;
        while (count< 4) {
          int c = input.read();
          if (c< 0) {
            // End of stream
            if (count == 1) {
              // Not supposed to happen
              throw new InvalidDataException("Invalid number of base64 characters");
            } else if (count == 2) {
              input.Unget();
              value <<= 12;
              return (byte)((value >> 16) & 0xff);
            } else if (count == 3) {
              input.Unget();
              value <<= 18;
              ResizeBuffer(1);
              buffer[0]=(byte)((value >> 8) & 0xff);
              return (byte)((value >> 16) & 0xff);
            }
            return -1;
          } else if (c == 0x0d) {
            c = input.read();
            if (c == 0x0a) {
              lineCharCount = 0;
            } else {
              input.Unget();
              if (lenientLineBreaks) {
                lineCharCount = 0;
              }
            }
          } else if (c == 0x0a) {
            if (lenientLineBreaks) {
              lineCharCount = 0;
            }
          } else if (c >= 0x80) {
            ++lineCharCount;
            if (lineCharCount>MaxLineSize) {
              throw new InvalidDataException("Encoded base64 line too long");
            }
          } else {
            ++lineCharCount;
            if (lineCharCount>MaxLineSize) {
              throw new InvalidDataException("Encoded base64 line too long");
            }
            c = Alphabet[c];
            if (c >= 0) {
              value <<= 6;
              value|=c;
              ++count;
            }
          }
        }
        ResizeBuffer(2);
        buffer[0]=(byte)((value >> 8) & 0xff);
        buffer[1]=(byte)((value) & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    private static final class BEncodingStringTransform implements ITransform {
      String input;
      int inputIndex;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;

      private static final int MaxLineSize = 76;

      public BEncodingStringTransform (String input) {
        this.input = input;
        this.buffer = new byte[4];
      }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        bufferCount = size;
        bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        int value = 0;
        int count = 0;
        while (count< 4) {
          int c = (inputIndex<input.length()) ? input.charAt(inputIndex++) : -1;
          if (c< 0) {
            // End of stream
            if (count == 1) {
              // Not supposed to happen;
              // invalid number of base64 characters
              return '?';
            } else if (count == 2) {
              --inputIndex;
              value <<= 12;
              return (byte)((value >> 16) & 0xff);
            } else if (count == 3) {
              --inputIndex;
              value <<= 18;
              ResizeBuffer(1);
              buffer[0]=(byte)((value >> 8) & 0xff);
              return (byte)((value >> 16) & 0xff);
            }
            return -1;
          } else if (c >= 0x80) {
            // ignore
          } else {
            c = Base64Transform.Alphabet[c];
            if (c >= 0) {
              value <<= 6;
              value|=c;
              ++count;
            }
          }
        }
        ResizeBuffer(2);
        buffer[0]=(byte)((value >> 8) & 0xff);
        buffer[1]=(byte)((value) & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    final class QEncodingStringTransform implements ITransform {
      String input;
      int inputIndex;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;

      public QEncodingStringTransform (
        String input) {
        this.input = input;
      }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        if (buffer == null) {
          buffer = new byte[size + 10];
        } else if (size>buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(buffer, 0, newbuffer, 0, buffer.length);
          buffer = newbuffer;
        }
        bufferCount = size;
        bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        int endIndex = input.length();
        while (true) {
          int c = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
          if (c< 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c == 0x0a) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c=='=') {
            int b1 = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
            c = 0;
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
              --inputIndex;
              return '?';
            }
            int b2 = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
            if (b2 >= '0' && b2 <= '9') {
              c <<= 4;
              c |= b2 - '0';
            } else if (b2 >= 'A' && b2 <= 'F') {
              c <<= 4;
              c |= b2 + 10 - 'A';
            } else if (b1 >= 'a' && b2 <= 'f') {
              c <<= 4;
              c |= b2 + 10 - 'a';
            } else {
              --inputIndex;
              ResizeBuffer(1);
              buffer[0]=(byte)b1;  // will be 0-9 or a-f or A-F
              return '?';
            }
            return c;
          } else if (c <= 0x20 || c >= 0x7f) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c=='_') {
            // Underscore, use space
            return ' ';
          } else {
            // printable ASCII, return that byte
            return c;
          }
        }
      }
    }

    final class PercentEncodingStringTransform implements ITransform {
      String input;
      int inputIndex;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;

      public PercentEncodingStringTransform (
        String input) {
        this.input = input;
      }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        if (buffer == null) {
          buffer = new byte[size + 10];
        } else if (size>buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(buffer, 0, newbuffer, 0, buffer.length);
          buffer = newbuffer;
        }
        bufferCount = size;
        bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        int endIndex = input.length();
        while (true) {
          int c = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
          if (c< 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else if (c == 0x0a) {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else if (c=='%') {
            int b1 = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
            c = 0;
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
              --inputIndex;
              return '?';
            }
            int b2 = (inputIndex<endIndex) ? input.charAt(inputIndex++) : -1;
            if (b2 >= '0' && b2 <= '9') {
              c <<= 4;
              c |= b2 - '0';
            } else if (b2 >= 'A' && b2 <= 'F') {
              c <<= 4;
              c |= b2 + 10 - 'A';
            } else if (b1 >= 'a' && b2 <= 'f') {
              c <<= 4;
              c |= b2 + 10 - 'a';
            } else {
              --inputIndex;
              ResizeBuffer(1);
              buffer[0]=(byte)b1;  // will be 0-9 or a-f or A-F
              return '?';
            }
            return c;
          } else if ((c<0x20 || c>= 0x7f) && c!='\t') {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else {
            // printable ASCII, return that byte
            return c;
          }
        }
      }
    }

    final class BoundaryCheckerTransform implements ITransform {
      StreamWithUnget input;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;
      boolean started;
      boolean readingHeaders;
      boolean hasNewBodyPart;
      ArrayList<String> boundaries;

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        if (buffer == null) {
          buffer = new byte[size + 10];
        } else if (size>buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(buffer, 0, newbuffer, 0, buffer.length);
          buffer = newbuffer;
        }
        bufferCount = size;
        bufferIndex = 0;
      }

      public BoundaryCheckerTransform (ITransform stream) {
        this.input = new StreamWithUnget(stream);
        this.boundaries = new ArrayList<String>();
        this.started = true;
      }

    /**
     * Not documented yet.
     * @param boundary A string object.
     */
      public void PushBoundary(String boundary) {
        this.boundaries.add(boundary);
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        if (hasNewBodyPart) {
          return -1;
        }
        if (readingHeaders) {
          return input.read();
        }
        int c = input.read();
        if (c< 0) {
          started = false;
          return c;
        }
        if (c=='-' && started) {
          // Check for a boundary
          started = false;
          c = input.read();
          if (c=='-') {
            // Possible boundary candidate
            return CheckBoundaries(false);
          } else {
            input.Unget();
            return '-';
          }
        } else {
          started = false;
        }
        if (c == 0x0d) {
          c = input.read();
          if (c == 0x0a) {
            // Line break was read
            c = input.read();
            if (c==-1) {
              ResizeBuffer(1);
              buffer[0]=0x0a;
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              input.Unget();
              ResizeBuffer(1);
              buffer[0]=0x0a;
              return 0x0d;
            } else if (c!='-') {
              ResizeBuffer(2);
              buffer[0]=0x0a;
              buffer[1]=(byte)c;
              return 0x0d;
            }
            c = input.read();
            if (c==-1) {
              ResizeBuffer(2);
              buffer[0]=0x0a;
              buffer[1]=(byte)'-';
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              input.Unget();
              ResizeBuffer(2);
              buffer[0]=0x0a;
              buffer[1]=(byte)'-';
              return 0x0d;
            } else if (c!='-') {
              ResizeBuffer(3);
              buffer[0]=0x0a;
              buffer[1]=(byte)'-';
              buffer[2]=(byte)c;
              return 0x0d;
            }
            // Possible boundary candidate
            return CheckBoundaries(true);
          } else {
            input.Unget();
            return 0x0d;
          }
        } else {
          return c;
        }
      }

      private int CheckBoundaries(boolean includeCrLf) {
        // Reached here when the "--" of a possible
        // boundary delimiter is read.  We need to
        // check boundaries here in order to find out
        // whether to emit the CRLF before the "--".

        boolean done = false;
        while (!done) {
          done = true;
          int bufferStart = 0;
          if (includeCrLf) {
            ResizeBuffer(3);
            bufferStart = 3;
            // store LF, '-', and '-' in the buffer in case
            // the boundary check fails, in which case
            // this method will return CR
            buffer[0]=0x0a;
            buffer[1]=(byte)'-';
            buffer[2]=(byte)'-';
          } else {
            bufferStart = 1;
            ResizeBuffer(1);
            buffer[0]=(byte)'-';
          }
          // Check up to 72 bytes (the maximum size
          // of a boundary plus 2 bytes for the closing
          // hyphens)
          int c;
          int bytesRead = 0;
          for (int i = 0; i < 72; ++i) {
            c = input.read();
            if (c<0 || c >= 0x80 || c == 0x0d) {
              input.Unget();
              break;
            }
            ++bytesRead;
            //Console.Write("" + ((char)c));
            ResizeBuffer(bytesRead + bufferStart);
            buffer[bytesRead + bufferStart-1]=(byte)c;
          }
          //System.out.println("--" + (bytesRead));
          // NOTE: All boundary strings are assumed to
          // have only ASCII characters (with values
          // less than 128).  Check boundaries from
          // top to bottom in the stack.
          String matchingBoundary = null;
          int matchingIndex=-1;
          for (int i = boundaries.size()-1;i >= 0; --i) {
            String boundary = boundaries.get(i);
            //System.out.println("Check boundary " + (boundary));
            if (!((boundary)==null || (boundary).length()==0) && boundary.length() <= bytesRead) {
              boolean match = true;
              for (int j = 0;j<boundary.length(); ++j) {
                if ((boundary.charAt(j)&0xff) != (int)((buffer[j + bufferStart]) & 0xff)) {
                  match = false;
                }
              }
              if (match) {
                matchingBoundary = boundary;
                matchingIndex = i;
                break;
              }
            }
          }
          if (matchingBoundary != null) {
            boolean closingDelim = false;
            // Pop the stack until the matching body part
            // is on top
            while (boundaries.size()>matchingIndex + 1) {
              boundaries.remove(matchingIndex + 1);
            }
            // Boundary line found
            if (matchingBoundary.length() + 1<bytesRead) {
              if (buffer[matchingBoundary.length()+bufferStart]=='-' &&
                  buffer[matchingBoundary.length()+1+bufferStart]=='-') {
                closingDelim = true;
              }
            }
            // Clear the buffer, the boundary line
            // isn't part of any body data
            bufferCount = 0;
            bufferIndex = 0;
            if (closingDelim) {
              // Pop this entry, it's the top of the stack
              boundaries.remove(boundaries.size()-1);
              if (boundaries.size() == 0) {
                // There's nothing else significant
                // after this boundary,
                // so return now
                return -1;
              }
              // Read to end of line.  Since this is the last body
              // part, the rest of the data before the next boundary
              // is insignificant
              while (true) {
                c = input.read();
                if (c==-1) {
                  // The body higher up didn't end yet
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = input.read();
                  if (c==-1) {
                    // The body higher up didn't end yet
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0a) {
                    // Start of new body part
                    c = input.read();
                    if (c==-1) {
                      throw new InvalidDataException("Premature end of message");
                    } else if (c == 0x0d) {
                      // Unget the CR, in case the next line is a boundary line
                      input.Unget();
                    } else if (c!='-') {
                      // Not a boundary delimiter
                      continue;
                    }
                    c = input.read();
                    if (c==-1) {
                      throw new InvalidDataException("Premature end of message");
                    } else if (c == 0x0d) {
                      // Unget the CR, in case the next line is a boundary line
                      input.Unget();
                    } else if (c!='-') {
                      // Not a boundary delimiter
                      continue;
                    }
                    // Found the next boundary delimiter
                    done = false;
                    break;
                  } else {
                    input.Unget();
                  }
                }
              }
              if (!done) {
                // Recheck the next line for a boundary delimiter
                continue;
              }
            } else {
              // Read to end of line (including CRLF; the
              // next line will start the headers of the
              // next body part).
              while (true) {
                c = input.read();
                if (c==-1) {
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = input.read();
                  if (c==-1) {
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0a) {
                    // Start of new body part
                    hasNewBodyPart = true;
                    return -1;
                  } else {
                    input.Unget();
                  }
                }
              }
            }
          }
          // Not a boundary, return CR (the
          // ReadByte method will then return LF,
          // the hyphens, and the other bytes
          // already read)
          return (includeCrLf) ? 0x0d : '-';
        }
        // Not a boundary, return CR (the
        // ReadByte method will then return LF,
        // the hyphens, and the other bytes
        // already read)
        return (includeCrLf) ? 0x0d : '-';
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int BoundaryCount() {
        return boundaries.size();
      }

    /**
     * Not documented yet.
     */
      public void StartBodyPartHeaders() {

        this.readingHeaders = true;
        this.hasNewBodyPart = false;
      }

    /**
     * Not documented yet.
     */
      public void EndBodyPartHeaders() {

        this.readingHeaders = false;
        this.hasNewBodyPart = false;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public boolean getHasNewBodyPart() {
          return hasNewBodyPart;
        }
    }

    final class QuotedPrintableTransform implements ITransform {
      StreamWithUnget input;
      int lineCharCount;
      boolean lenientLineBreaks;
      byte[] buffer;
      int bufferIndex;
      int bufferCount;

      private int maxLineSize;

      public QuotedPrintableTransform (
        ITransform input,
        boolean lenientLineBreaks,
        int maxLineSize) {
        this.maxLineSize = maxLineSize;
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
      }

      public QuotedPrintableTransform (
        InputStream input,
        boolean lenientLineBreaks,
        int maxLineSize) {
        this.maxLineSize = maxLineSize;
        this.input = new StreamWithUnget(new WrappedStream(input));
        this.lenientLineBreaks = lenientLineBreaks;
      }

      public QuotedPrintableTransform (
        ITransform input,
        boolean lenientLineBreaks) {
        // DEVIATION: The max line size is actually 76, but some emails
        // write lines that exceed this size
        this.maxLineSize = 200;
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
      }

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        if (buffer == null) {
          buffer = new byte[size + 10];
        } else if (size>buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(buffer, 0, newbuffer, 0, buffer.length);
          buffer = newbuffer;
        }
        bufferCount = size;
        bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (bufferIndex<bufferCount) {
          int ret = buffer[bufferIndex];
          ++bufferIndex;
          if (bufferIndex == bufferCount) {
            bufferCount = 0;
            bufferIndex = 0;
          }
          ret&=0xff;
          return ret;
        }
        while (true) {
          int c = input.read();
          if (c < 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            c = input.read();
            if (c == 0x0a) {
              // CRLF
              ResizeBuffer(1);
              buffer[0]=0x0a;
              lineCharCount = 0;
              return 0x0d;
            } else {
              input.Unget();
              if (!lenientLineBreaks) {
                throw new InvalidDataException("Expected LF after CR");
              }
              // CR, so write CRLF
              ResizeBuffer(1);
              buffer[0]=0x0a;
              lineCharCount = 0;
              return 0x0d;
            }
          } else if (c == 0x0a) {
            if (!lenientLineBreaks) {
              throw new InvalidDataException("Expected LF after CR");
            }
            // LF, so write CRLF
            ResizeBuffer(1);
            buffer[0]=0x0a;
            lineCharCount = 0;
            return 0x0d;
          } else if (c=='=') {
            ++lineCharCount;
            if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            int b1 = input.read();
            int b2 = input.read();
            if (b2 >= 0 && b1 >= 0) {
              if (b1=='\r' && b2=='\n') {
                // Soft line break
                lineCharCount = 0;
                continue;
              } else if (b1=='\r') {
                if (!lenientLineBreaks) {
                  throw new InvalidDataException("Expected LF after CR");
                }
                lineCharCount = 0;
                input.Unget();
                continue;
              } else if (b1=='\n') {
                if (!lenientLineBreaks) {
                  throw new InvalidDataException("Bare LF not expected");
                }
                lineCharCount = 0;
                input.Unget();
                continue;
              }
              c = 0;
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
                throw new InvalidDataException(String.Format("Invalid hex character"));
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
                throw new InvalidDataException(String.Format("Invalid hex character"));
              }
              lineCharCount+=2;
              if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
                throw new InvalidDataException("Encoded quoted-printable line too long");
              }
              return c;
            } else if (b1 >= 0) {
              if (b1=='\r') {
                // Soft line break
                if (!lenientLineBreaks) {
                  throw new InvalidDataException("Expected LF after CR");
                }
                lineCharCount = 0;
                input.Unget();
                continue;
              } else if (b1=='\n') {
                // Soft line break
                if (!lenientLineBreaks) {
                  throw new InvalidDataException("Bare LF not expected");
                }
                lineCharCount = 0;
                input.Unget();
                continue;
              } else {
                throw new InvalidDataException("Invalid data after equal sign");
              }
            } else {
              // Equal sign at end; ignore
              return -1;
            }
          } else if (c!='\t' && (c<0x20 || c>= 0x7f)) {
            throw new InvalidDataException("Invalid character in quoted-printable");
          } else if (c==' ' || c=='\t') {
            // Space or tab.  Since the quoted-printable spec
            // requires decoders to delete spaces and tabs before
            // CRLF, we need to create a lookahead buffer for
            // tabs and spaces read to see if they precede CRLF.
            int spaceCount = 1;
            ++lineCharCount;
            if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            // In most cases, though, there will only be
            // one space or tab
            int c2 = input.read();
            if (c2!=' ' && c2!='\t' && c2!='\r' && c2!='\n' && c2>= 0) {
              // Simple: Space before a character other than
              // space, tab, CR, LF, or EOF
              input.Unget();
              return c;
            }
            boolean endsWithLineBreak = false;
            while (true) {
              if ((c2=='\n' && lenientLineBreaks) || c2 < 0) {
                input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2=='\r' && lenientLineBreaks) {
                input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2=='\r') {
                // CR, may or may not be a line break
                c2 = input.read();
                // Add the CR to the
                // buffer, it won't be ignored
                ResizeBuffer(spaceCount);
                buffer[spaceCount-1]=(byte)'\r';
                if (c2=='\n') {
                  // LF, so it's a line break
                  lineCharCount = 0;
                  ResizeBuffer(spaceCount + 1);
                  buffer[spaceCount]=(byte)'\n';
                  endsWithLineBreak = true;
                  break;
                } else {
                  // It's something else {
 input.Unget();
}
                  ++lineCharCount;
                  if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
                    throw new InvalidDataException("Encoded quoted-printable line too long");
                  }
                  break;
                }
              } else if (c2!=' ' && c2!='\t') {
                // Not a space or tab
                input.Unget();
                break;
              } else {
                // An additional space or tab
                ResizeBuffer(spaceCount);
                buffer[spaceCount-1]=(byte)c2;
                ++spaceCount;
                ++lineCharCount;
                if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
                  throw new InvalidDataException("Encoded quoted-printable line too long");
                }
              }
              c2 = input.read();
            }
            // Ignore space/tab runs if the line ends in that run
            if (!endsWithLineBreak) {
              return c;
            } else {
              bufferCount = 0;
              continue;
            }
          } else {
            ++lineCharCount;
            if (maxLineSize >= 0 && lineCharCount>maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            return c;
          }
        }
      }
    }

    /**
     * Not documented yet.
     * @param name A string object. (2).
     * @return A string object.
     */
    public String GetHeader(String name) {
      name = ParserUtility.ToLowerCaseAscii(name);
      if (name.equals("content-type")) {
        return ContentType.toString();
      }
      for (int i = 0;i<headers.size();i+=2) {
        if (headers.get(i).equals(name)) {
          return headers.get(i + 1);
        }
      }
      return null;
    }

    private static boolean HasNonAsciiOrCtlOrTooLongWord(String s) {
      int len = s.length();
      int wordLength = 0;
      for (int i = 0; i < len; ++i) {
        char c = s.charAt(i);
        if (c >= 0x7f || (c<0x20 && c != 0x09)) {
          return true;
        }
        if (c == 0x20 || c == 0x09) {
          wordLength = 0;
        } else {
          ++wordLength;
          if (wordLength>75) {
            return true;
          }
        }
      }
      return false;
    }

    private int TransferEncodingToUse() {
      String topLevel = contentType.getTopLevelType();
      if (topLevel.equals("message") || topLevel.equals("multipart")) {
        return EncodingSevenBit;
      }
      if (topLevel.equals("text")) {
        int lengthCheck = Math.min(this.body.length, 4096);
        int highBytes = 0;
        int lineLength = 0;
        // TODO: Check line lengths
        boolean allTextBytes = true;
        for (int i = 0; i < lengthCheck; ++i) {
          if ((this.body[i]&0x80) != 0) {
            highBytes+=1;
            allTextBytes = false;
          } else if (this.body[i]==0) {
            allTextBytes = false;
          } else if (this.body[i]==(byte)'\r') {
            if (i+1>= this.body.length || this.body[i+1]!=(byte)'\n') {
              // bare CR
              allTextBytes = false;
            } else if (i>0 && (this.body[i-1]==(byte)' ' || this.body[i-1]==(byte)'\t')) {
              // Space followed immediately by CRLF
              allTextBytes = false;
            } else {
              i+=1;
              lineLength = 0;
              continue;
            }
          } else if (this.body[i]==(byte)'\n') {
            // bare LF
            allTextBytes = false;
          }
          ++lineLength;
          if (lineLength>76) {
            allTextBytes = false;
          }
        }
        if (lengthCheck == this.body.length && allTextBytes) {
          return EncodingSevenBit;
        } if (highBytes>(lengthCheck/3)) {
          return EncodingBase64;
        } else {
          return EncodingQuotedPrintable;
        }
      }
      return EncodingBase64;
    }

    final class EncodedWordEncoder {
      StringBuilder currentWord;
      StringBuilder fullString;
      int lineLength;

      private static String hex="0123456789ABCDEF";

      public EncodedWordEncoder (String c) {
        currentWord = new StringBuilder();
        fullString = new StringBuilder();
        fullString.append(c);
        lineLength = c.length();
      }

      private void AppendChar(char ch) {
        PrepareToAppend(1);
        currentWord.append(ch);
      }

      private void PrepareToAppend(int numChars) {
        // 1 for space and 2 for the ending "?="
        if (lineLength + 1 + currentWord.length() + numChars + 2>76) {
          // Too big to fit the current line,
          // create a new line
          fullString.append("\r\n");
          lineLength = 0;
        }
        if (lineLength + 1 + currentWord.length() + numChars + 2>76) {
          // Encoded word would be too big,
          // so output that word
          fullString.append(' ');
          fullString.append(currentWord);
          fullString.append("?=");
          lineLength+=3 + currentWord.length();
          currentWord.Clear();
          currentWord.append("=?utf-8?q?");
        }
      }

    /**
     * Not documented yet.
     */
      public void FinalizeEncoding() {
        if (currentWord.length()>0) {
          // 1 for space
          if (lineLength + 1 + currentWord.length() + 2>76) {
            // Too big to fit the current line,
            // create a new line
            fullString.append("\r\n");
            lineLength = 0;
          }
          fullString.append(' ');
          fullString.append(currentWord);
          fullString.append("?=");
          lineLength+=3 + currentWord.length();
          currentWord.Clear();
        }
      }

    /**
     * Not documented yet.
     * @param str A string object.
     */
      public void AddString(String str) {
        for (int j = 0;j<str.length(); ++j) {
          int c = str.charAt(j);
          if (c >= 0xd800 && c <= 0xdbff && j + 1 < str.length() &&
              str.charAt(j + 1) >= 0xdc00 && str.charAt(j + 1) <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str.charAt(j + 1) - 0xdc00);
            ++j;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          AddChar(c);
        }
      }

    /**
     * Not documented yet.
     * @param ch A 32-bit signed integer.
     */
      public void AddChar(int ch) {
        if (currentWord.length() == 0) {
          currentWord.append("=?utf-8?q?");
        }
        if (ch == 0x20) {
          AppendChar('_');
        } else if (ch<0x80 && ch>0x20 && ch!=(char)'"' && ch!=(char)',' &&
                   "?()<>[]:;@\\.=_".indexOf((char)ch)< 0) {
          AppendChar((char)ch);
        } else if (ch<0x80) {
          PrepareToAppend(3);
          currentWord.append('=');
          currentWord.append(hex.charAt(ch >> 4));
          currentWord.append(hex.charAt(ch & 15));
        } else if (ch<0x800) {
          int w= (byte)(0xc0 | ((ch >> 6) & 0x1f));
          int x = (byte)(0x80 | (ch & 0x3f));
          PrepareToAppend(6);
          currentWord.append('=');
          currentWord.append(hex.charAt(w >> 4));
          currentWord.append(hex.charAt(w & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(x >> 4));
          currentWord.append(hex.charAt(x & 15));
        } else if (ch<0x10000) {
          PrepareToAppend(9);
          int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
          int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int y = (byte)(0x80 | (ch & 0x3f));
          currentWord.append('=');
          currentWord.append(hex.charAt(w >> 4));
          currentWord.append(hex.charAt(w & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(x >> 4));
          currentWord.append(hex.charAt(x & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(y >> 4));
          currentWord.append(hex.charAt(y & 15));
        } else {
          PrepareToAppend(12);
          int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
          int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
          int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int z = (byte)(0x80 | (ch & 0x3f));
          currentWord.append('=');
          currentWord.append(hex.charAt(w >> 4));
          currentWord.append(hex.charAt(w & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(x >> 4));
          currentWord.append(hex.charAt(x & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(y >> 4));
          currentWord.append(hex.charAt(y & 15));
          currentWord.append('=');
          currentWord.append(hex.charAt(z >> 4));
          currentWord.append(hex.charAt(z & 15));
        }
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
      @Override public String toString() {
        return fullString.toString();
      }
    }

    final class WordWrapEncoder {
      String lastSpaces;
      StringBuilder fullString;
      int lineLength;

      private static final int MaxLineLength = 76;

      public WordWrapEncoder (String c) {
        fullString = new StringBuilder();
        fullString.append(c);
        if (fullString.length() >= MaxLineLength) {
          fullString.append("\r\n");
          lastSpaces=" ";
          lineLength = 0;
        } else {
          lastSpaces=" ";
          lineLength = fullString.length();
        }
      }

      private void AppendSpaces(String str) {
        if (lineLength + lastSpaces.length() + str.length()>MaxLineLength) {
          // Too big to fit the current line
          lastSpaces=" ";
        } else {
          lastSpaces = str;
        }
      }

      private void AppendWord(String str) {
        if (lineLength + lastSpaces.length() + str.length()>MaxLineLength) {
          // Too big to fit the current line,
          // create a new line
          fullString.append("\r\n");
          lastSpaces=" ";
          lineLength = 0;
        }
        fullString.append(lastSpaces);
        fullString.append(str);
        lineLength+=lastSpaces.length();
        lineLength+=str.length();
        lastSpaces = "";
      }

    /**
     * Not documented yet.
     * @param str A string object.
     */
      public void AddString(String str) {
        int wordStart = 0;
        for (int j = 0;j<str.length(); ++j) {
          int c = str.charAt(j);
          if (c == 0x20 || c == 0x09) {
            int wordEnd = j;
            if (wordStart != wordEnd) {
              AppendWord(str.substring(wordStart,(wordStart)+(wordEnd-wordStart)));
            }
            while (j<str.length()) {
              if (str.charAt(j)==0x20 || str.charAt(j)==0x09) {
 ++j;
  } else {
 break;
}
            }
            wordStart = j;
            AppendSpaces(str.substring(wordEnd,(wordEnd)+(wordStart-wordEnd)));
            --j;
          }
        }
        if (wordStart != str.length()) {
          AppendWord(str.substring(wordStart,(wordStart)+(str.length()-wordStart)));
        }
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
      @Override public String toString() {
        return fullString.toString();
      }
    }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
    @Override public String toString() {
      StringBuilder sb = new StringBuilder();
      for (int i = 0;i<headers.size();i+=2) {
        String name = headers.get(i);
        String value = headers.get(i + 1);
        IHeaderFieldParser parser = HeaderFields.GetParser(name);
        if (!parser.IsStructured()) {
          if (HasNonAsciiOrCtlOrTooLongWord(value)) {
            EncodedWordEncoder encoder=new EncodedWordEncoder(name+":");
            encoder.AddString(value);
            encoder.FinalizeEncoding();
          } else {
            WordWrapEncoder encoder=new WordWrapEncoder(name+":");
            encoder.AddString(value);
          }
        } else if (name.equals("content-type") ||
                   name.equals("mime-version") ||
                   name.equals("content-transfer-encoding")) {
          // don't write now
        } else {
          if (HasNonAsciiOrCtlOrTooLongWord(value) || value.indexOf("=?") >= 0) {
            sb.append(name);
            sb.append(':');
            // TODO: Not perfect yet
            sb.append(value);
          } else {
            WordWrapEncoder encoder=new WordWrapEncoder(name+":");
            encoder.AddString(value);
          }
        }
        sb.append("\r\n");
      }
      sb.append("MIME-Version: 1.0\r\n");
      int transferEncoding = TransferEncodingToUse();
      switch(transferEncoding) {
        case EncodingBase64:
          sb.append("Content-Transfer-Encoding: base64\r\n");
          break;
        case EncodingQuotedPrintable:
          sb.append("Content-Transfer-Encoding: quoted-printable\r\n");
          break;
        default:
          sb.append("Content-Transfer-Encoding: 7bit\r\n");
          break;
      }
      sb.append("\r\n");
      if (this.getContentType().getTopLevelType().equals("multipart")) {
        String boundary=this.getContentType().GetParameter("boundary");
      }
      return sb.toString();
    }

    private static void ReadHeaders(
      ITransform stream,
      List<String> headerList) {
      int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      StreamWithUnget ungetStream = new StreamWithUnget(stream);
      while (true) {
        sb.setLength(0);
        boolean first = true;
        boolean endOfHeaders = false;
        boolean wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetStream.read();
          if (c==-1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c=='\r') {
            if (ungetStream.read()=='\n') {
              endOfHeaders = true;
              break;
            } else {
              throw new InvalidDataException("CR not followed by LF");
            }
          }
          if (lineCount > 998) {
            throw new InvalidDataException("Header field line too long");
          }
          if ((c >= 0x21 && c <= 57) || (c >= 59 && c <= 0x7e)) {
            if (wsp) {
              throw new InvalidDataException("Whitespace within header field");
            }
            first = false;
            if (c>= 'A' && c<= 'Z') {
              c+=0x20;
            }
            sb.append((char)c);
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
        if (sb.length() == 0) {
          throw new InvalidDataException("Empty header field name");
        }
        String fieldName = sb.toString();
        sb.setLength(0);
        // Read the header field value
        while (true) {
          int c = ungetStream.read();
          if (c==-1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          if (c=='\r') {
            c = ungetStream.read();
            if (c=='\n') {
              lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              boolean fwsFirst = true;
              boolean haveFWS = false;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                if (!fwsFirst) {
                  c = ungetStream.read();
                  if (c=='\r') {
                    c = ungetStream.read();
                    if (c=='\n') {
                      // Skipping CRLF
                      lineCount = 0;
                    } else {
                      // It's the first part of the line, where the header name
                      // should be, so the CR here is illegal
                      throw new InvalidDataException("CR not followed by LF");
                    }
                  } else {
                    // anything else, unget
                    ungetStream.Unget();
                  }
                }
                fwsFirst = false;
                int c2 = ungetStream.read();
                if (c2 == 0x20 || c2 == 0x09) {
                  lineCount+=1;
                  sb.append((char)c2);
                  haveFWS = true;
                  if (lineCount>998) {
                    throw new InvalidDataException("Header field line too long");
                  }
                } else {
                  ungetStream.Unget();
                  break;
                }
              }
              if (haveFWS) {
                // We have folding whitespace, line
                // count ((found instanceof above
                continue) ? (above
                continue)found : null);
              }
              break;
            } else {
              sb.append('\r');
              ungetStream.Unget();
              ++lineCount;
            }
          }
          if (lineCount>998) {
            throw new InvalidDataException("Header field line too long");
          }
          if (c<0x80) {
            sb.append((char)c);
          } else {
            if (!HeaderFields.GetParser(fieldName).IsStructured()) {
              // DEVIATION: Some emails still have an unencoded subject line
              // or other unstructured header field
              sb.append('\ufffd');
            } else {
              throw new InvalidDataException("Malformed header field value "+sb.toString());
            }
          }
        }
        String fieldValue = sb.toString();
        headerList.add(fieldName);
        // NOTE: Field value will no longer have folding whitespace
        // at this point
        headerList.add(fieldValue);
      }
    }

    private class MessageStackEntry {
      Message message;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public Message getMessage() {
          return message;
        }
      String boundary;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public String getBoundary() {
          return boundary;
        }

      public MessageStackEntry (Message msg) {

        this.message = msg;
        MediaType mediaType = msg.getContentType();
        if (mediaType.getTopLevelType().equals("multipart")) {
          this.boundary=mediaType.GetParameter("boundary");
          if (this.boundary == null) {
            throw new InvalidDataException("Multipart message has no boundary defined");
          }
          if (!IsWellFormedBoundary(this.boundary)) {
            throw new InvalidDataException("Multipart message has an invalid boundary defined");
          }
        }
      }
    }

    private void ReadMultipartBody(ITransform stream) {
      int baseTransferEncoding = transferEncoding;
      BoundaryCheckerTransform boundaryChecker = new Message.BoundaryCheckerTransform(stream);
      ITransform currentTransform = MakeTransferEncoding(
        boundaryChecker,
        baseTransferEncoding,
        this.getContentType().getTypeAndSubType().equals("text/plain"));
      List<MessageStackEntry> multipartStack = new ArrayList<MessageStackEntry>();
      MessageStackEntry entry = new Message.MessageStackEntry(this);
      multipartStack.add(entry);
      boundaryChecker.PushBoundary(entry.getBoundary());
      Message leaf = null;
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

        while (true) {
          int ch = 0;
          try {
            ch = currentTransform.read();
          } catch (InvalidDataException ex) {
            ms.write(buffer,0,bufferCount);
            buffer = ms.toByteArray();
            String ss = DataUtilities.GetUtf8String(buffer,
                                                    Math.max(buffer.length-80, 0),
                                                    Math.min(buffer.length, 80), true);
            System.out.println(ss);
            throw ex;
          }
          if (ch < 0) {
            if (boundaryChecker.getHasNewBodyPart()) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack

              if (leaf != null) {
                if (bufferCount>0) {
                  ms.write(buffer,0,bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.toByteArray();
              }
              while (multipartStack.size()>stackCount) {
                multipartStack.remove(stackCount);
              }
              Message parentMessage = multipartStack.get(multipartStack.size()-1).getMessage();
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              boolean parentIsDigest=parentMessage.getContentType().SubType.equals("digest") &&
                parentMessage.getContentType().TopLevelType.equals("multipart");
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.getParts().add(msg);
              multipartStack.add(entry);
              ms.SetLength(0);
              if (msg.getContentType().TopLevelType.equals("multipart")) {
                leaf = null;
              } else {
                leaf = msg;
              }
              boundaryChecker.PushBoundary(entry.getBoundary());
              boundaryChecker.EndBodyPartHeaders();
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                msg.getContentType().TypeAndSubType.equals("text/plain"));
            } else {
              // All body parts were read
              if (leaf != null) {
                if (bufferCount>0) {
                  ms.write(buffer,0,bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.toByteArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++]=(byte)ch;
            if (bufferCount >= bufferLength) {
              ms.write(buffer,0,bufferCount);
              bufferCount = 0;
            }
          }
        }
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    private static ITransform MakeTransferEncoding(ITransform stream,
                                                   int encoding, boolean plain) {
      ITransform transform = new EightBitTransform(stream);
      if (encoding == EncodingQuotedPrintable) {
        transform = new QuotedPrintableTransform(stream, false);
      } else if (encoding == EncodingBase64) {
        transform = new Base64Transform(stream, false);
      } else if (encoding == EncodingEightBit) {
        transform = new EightBitTransform(stream);
      } else if (encoding == EncodingBinary) {
        transform = new BinaryTransform(stream);
      } else if (encoding == EncodingSevenBit) {
        if (plain) {
          // DEVIATION: Replace 8-bit bytes and null with the
          // question mark character for text/plain and non-MIME
          // messages
          transform = new LiberalSevenBitTransform(stream);
        } else {
          transform = new SevenBitTransform(stream);
        }
      }
      return transform;
    }

    private void ReadSimpleBody(ITransform stream) {
      ITransform transform = MakeTransferEncoding(stream, transferEncoding,
                                                  this.getContentType().getTypeAndSubType().equals("text/plain"));
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.length;
      // TODO: Support message/rfc822
      java.io.ByteArrayOutputStream ms=null;
try {
ms=new ByteArrayOutputStream();

        while (true) {
          int ch = 0;
          try {
            ch = transform.read();
          } catch (InvalidDataException ex) {
            ms.write(buffer,0,bufferCount);
            buffer = ms.toByteArray();
            String ss = DataUtilities.GetUtf8String(buffer,
                                                    Math.max(buffer.length-80, 0),
                                                    Math.min(buffer.length, 80), true);
            System.out.println(ss);
            throw ex;
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++]=(byte)ch;
          if (bufferCount >= bufferLength) {
            ms.write(buffer,0,bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount>0) {
          ms.write(buffer,0,bufferCount);
        }
        this.body = ms.toByteArray();
}
finally {
try { if(ms!=null)ms.close(); } catch (IOException ex){}
}
    }

    private void ReadMessage(ITransform stream) {
      ReadHeaders(stream, this.headers);
      ProcessHeaders(false, false);
      if (contentType.getTopLevelType().equals("multipart")) {
        ReadMultipartBody(stream);
      } else {
        if (contentType.getTopLevelType().equals("message")) {
          System.out.println(contentType);
        }
        ReadSimpleBody(stream);
      }
    }
  }
