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
    private static final int EncodingUnknown = -1;
    private static final int EncodingEightBit = 3;
    private static final int EncodingBinary = 4;
    private static final int EncodingQuotedPrintable = 1;
    private static final int EncodingBase64 = 2;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<Message> getParts() {
        return this.parts;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public List<String> getHeaders() {
        return this.headers;
      }

    private byte[] body;

    /**
     * Gets a value not documented yet.
     * @return A byte[] object.
     */
    public byte[] GetBody() {
      return this.body;
    }

    /**
     * Not documented yet.
     * @param str A string object.
     */
    public void SetBody(String str) {
      this.body = DataUtilities.GetUtf8Bytes(str, true);
      this.contentType=MediaType.Parse("text/plain; charset=utf-8");
    }

    public Message (InputStream stream) {
      if (stream == null) {
        throw new NullPointerException("stream");
      }
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
      this.ReadMessage(new WrappedStream(stream));
    }

    public Message () {
      this.headers = new ArrayList<String>();
      this.parts = new ArrayList<Message>();
    }

    static String ReplaceEncodedWords(String str) {
      if (str == null) {
        throw new NullPointerException("str");
      }
      return ReplaceEncodedWords(str, 0, str.length(), false);
    }

    static String ReplaceEncodedWords(String str, int index, int endIndex, boolean inComments) {

      if (endIndex - index< 9) {
        return str.substring(index,(index)+(endIndex - index));
      }
      if (str.indexOf('=') < 0) {
        return str.substring(index,(index)+(endIndex - index));
      }
      StringBuilder builder = new StringBuilder();
      boolean lastWordWasEncodedWord = false;
      int whitespaceStart = -1;
      int whitespaceEnd = -1;
      while (index < endIndex) {
        int charCount = 2;
        boolean acceptedEncodedWord = false;
        String decodedWord = null;
        int startIndex = 0;
        boolean havePossibleEncodedWord = false;
        boolean startParen = false;
        if (index + 1<endIndex && str.charAt(index)=='=' && str.charAt(index+1)=='?') {
          startIndex = index + 2;
          index += 2;
          havePossibleEncodedWord = true;
        } else if (inComments && index + 2<endIndex && str.charAt(index)=='(' &&
                   str.charAt(index + 1)=='=' && str.charAt(index+2)=='?') {
          startIndex = index + 3;
          index += 3;
          startParen = true;
          havePossibleEncodedWord = true;
        }
        if (havePossibleEncodedWord) {
          boolean maybeWord = true;
          int afterLast = endIndex;
          while (index < endIndex) {
            char c = str.charAt(index);
            ++index;
            // Check for a run of printable ASCII characters (except space)
            // with length up to 75 (also exclude '(' and ')' if 'inComments'
            // is true)
            if (c >= 0x21 && c < 0x7e && (!inComments || (c!='(' && c!=')'))) {
              ++charCount;
              if (charCount > 75) {
                maybeWord = false;
                index = startIndex - 2;
                break;
              }
            } else {
              afterLast = index - 1;
              break;
            }
          }
          if (maybeWord) {
            // May be an encoded word
            // System.out.println("maybe "+str.substring(startIndex-2,(startIndex-2)+(afterLast-(startIndex-2))));
            index = startIndex;
            int i2;
            // Parse charset
            // (NOTE: Compatible with RFC 2231's addition of language
            // to charset, since charset is defined as a 'token' in
            // RFC 2047, which includes '*')
            int charsetEnd = -1;
            int encodedTextStart = -1;
            boolean base64 = false;
            i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
            if (i2 != index && i2<endIndex && str.charAt(i2)=='?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
              if (i2 != index && i2<endIndex && str.charAt(i2)=='?') {
                // check for supported encoding (B or Q)
                char encodingChar = str.charAt(index);
                if (i2 - index==1 && (encodingChar=='b' || encodingChar=='B' ||
                                    encodingChar == 'q' || encodingChar=='Q')) {
                  // Parse encoded text
                  base64=encodingChar=='b' || encodingChar=='B';
                  index = i2 + 1;
                  encodedTextStart = index;
                  i2 = MediaType.skipEncodedTextRfc2047(str, index, afterLast, inComments);
                  if (i2 != index && i2+1<endIndex && str.charAt(i2)=='?' && str.charAt(i2+1)=='=' &&
                      i2 + 2 == afterLast) {
                    acceptedEncodedWord = true;
                    i2 += 2;
                  }
                }
              }
            }
            if (acceptedEncodedWord) {
              String charset = str.substring(startIndex,(startIndex)+(charsetEnd - startIndex));
              String encodedText = str.substring(
encodedTextStart,(
encodedTextStart)+((afterLast-2)- encodedTextStart));
              int asterisk = charset.indexOf('*');
              if (asterisk >= 1) {
                charset = str.substring(0,asterisk);
                String language = str.substring(asterisk + 1,(asterisk + 1)+(str.length() - (asterisk + 1)));
                if (!ParserUtility.IsValidLanguageTag(language)) {
                  acceptedEncodedWord = false;
                }
              } else if (asterisk == 0) {
                // Impossible, a charset can't start with an asterisk
                acceptedEncodedWord = false;
              }
              if (acceptedEncodedWord) {
                ITransform transform = base64 ?
                  (ITransform)new BEncodingStringTransform(encodedText) :
                  (ITransform)new QEncodingStringTransform(encodedText);
                Charsets.ICharset encoding = Charsets.GetCharset(charset);
                if (encoding == null) {
                  System.out.println("Unknown charset " + charset);
                  decodedWord = str.substring(startIndex - 2,(startIndex - 2)+(afterLast-(startIndex-2)));
                } else {
                  // System.out.println("Encoded " + (base64 ? "B" : "Q") + " to: " + (encoding.GetString(transform)));
                  decodedWord = encoding.GetString(transform);
                }
                // TODO: decodedWord may itself be part of an encoded word
                // or contain ASCII control characters: encoded word decoding is
                // not idempotent; if this is a comment it could also contain '(', ')', and '\'
              } else {
                decodedWord = str.substring(startIndex - 2,(startIndex - 2)+(afterLast-(startIndex-2)));
              }
            } else {
              decodedWord = str.substring(startIndex - 2,(startIndex - 2)+(afterLast-(startIndex-2)));
            }
            index = afterLast;
          }
        }
        if (whitespaceStart >= 0 && whitespaceStart < whitespaceEnd &&
            (!acceptedEncodedWord || !lastWordWasEncodedWord)) {
          // Append whitespace as long as it doesn't occur between two
          // encoded words
          builder.append(str.substring(whitespaceStart,(whitespaceStart)+(whitespaceEnd - whitespaceStart)));
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
        while (index < endIndex) {
          char c = str.charAt(index);
          if (c == 0x0d && index + 1 < endIndex && str.charAt(index + 1)==0x0a) {
            break;
          } else if (c == 0x09 || c == 0x20) {
            break;
          } else {
            ++index;
          }
        }
        boolean hasNonWhitespace = oldIndex != index;
        whitespaceStart = index;
        // Read to nonwhitespace
        index = HeaderParser.ParseFWS(str, index, endIndex, null);
        whitespaceEnd = index;
        if (builder.length() == 0 && oldIndex == 0 && index == str.length()) {
          // Nothing to replace, and the whole String
          // is being checked
          return str;
        }
        if (oldIndex != index) {
          // Append nonwhitespace only, unless this is the end
          if (index == endIndex) {
            builder.append(str.substring(oldIndex,(oldIndex)+(index - oldIndex)));
          } else {
            builder.append(str.substring(oldIndex,(oldIndex)+(whitespaceStart - oldIndex)));
          }
        }
        lastWordWasEncodedWord = acceptedEncodedWord;
      }
      return builder.toString();
    }

    private MediaType contentType;
    private int transferEncoding;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
    public MediaType getContentType() {
        return this.contentType;
      }

    private void ProcessHeaders(boolean assumeMime, boolean digest) {
      boolean haveContentType = false;
      boolean mime = assumeMime;
      String transferEncodingValue = "";
      for (int i = 0;i<this.headers.size();i+=2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (name.equals("from")) {
          if (HeaderParser.ParseHeaderFrom(value, 0, value.length(), null) == 0) {
            System.out.println(this.GetHeader("date"));
            // throw new InvalidDataException("Invalid From header: "+value);
          }
        }
        if (name.equals("to") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid To header: " + value);
          }
        }
        if (name.equals("cc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderCc(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid Cc header: " + value);
          }
        }
        if (name.equals("bcc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderBcc(value, 0, value.length(), null) == 0) {
            throw new InvalidDataException("Invalid Bcc header: " + value);
          }
        }
        if (name.equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(value, 0, value.length(), null);
          int endIndex = HeaderParser.ParseToken(value, startIndex, value.length(), null);
          if (HeaderParser.ParseCFWS(value, endIndex, value.length(), null) == value.length()) {
            transferEncodingValue = value.substring(startIndex,(startIndex)+(endIndex - startIndex));
          } else {
            transferEncodingValue = "";
          }
        }
        if (name.equals("mime-version")) {
          mime = true;
        }
      }
      boolean haveFrom = false;
      boolean haveSubject = false;
      boolean haveTo = false;
      // TODO: Treat message/rfc822 specially
      this.contentType = digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii;
      for (int i = 0;i<this.headers.size();i+=2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (mime && name.equals("content-transfer-encoding")) {
          value = ParserUtility.ToLowerCaseAscii(transferEncodingValue);
          this.headers.set(i + 1,value);
          if (value.equals("7bit")) {
            this.transferEncoding = EncodingSevenBit;
          } else if (value.equals("8bit")) {
            this.transferEncoding = EncodingEightBit;
          } else if (value.equals("binary")) {
            this.transferEncoding = EncodingBinary;
          } else if (value.equals("quoted-printable")) {
            this.transferEncoding = EncodingQuotedPrintable;
          } else if (value.equals("base64")) {
            this.transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            this.transferEncoding = EncodingUnknown;
          }
          this.headers.remove(i);
          this.headers.remove(i);
          i -= 2;
        } else if (mime && name.equals("content-type")) {
          if (haveContentType) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          this.contentType = MediaType.Parse(
value,
digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
        } else if (name.equals("from")) {
          if (haveFrom) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveFrom = true;
        } else if (name.equals("to")) {
          if (haveTo) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveTo = true;
        } else if (name.equals("subject")) {
          if (haveSubject) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveSubject = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        this.contentType=MediaType.Parse("application/octet-stream");
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64 ||
          this.transferEncoding == EncodingUnknown) {
        if (this.contentType.getTopLevelType().equals("multipart") ||
            this.contentType.getTopLevelType().equals("message")) {
          throw new InvalidDataException("Invalid content encoding for multipart or message");
        }
      }
    }

    private static boolean IsWellFormedBoundary(String str) {
      if (str == null || str.length() < 1 || str.length()>70) {
        return false;
      }
      for (int i = 0;i < str.length(); ++i) {
        char c = str.charAt(i);
        if (i == str.length() - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c >= 'A' && c<= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9') ||
          c == 0x20 || c==0x2c || "'()-./+_:=?".indexOf(c)>= 0)) {
          return false;
        }
      }
      return true;
    }

    private static final class WrappedStream implements ITransform {
      internal InputStream stream;

      public WrappedStream (InputStream stream) {
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

    private static final class StreamWithUnget implements ITransform {
      internal ITransform stream;
      int lastByte;
      internal boolean unget;

      public StreamWithUnget (InputStream stream) {
        this.lastByte=-1;
        this.stream = new WrappedStream(stream);
      }

      public StreamWithUnget (ITransform stream) {
        this.lastByte=-1;
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.unget) {
          this.unget = false;
        } else {
          this.lastByte = this.stream.read();
        }
        return this.lastByte;
      }

    /**
     * Not documented yet.
     */
      public void Unget() {
        this.unget = true;
      }
    }

    interface ITransform {
      int ReadByte();
    }

    private static final class EightBitTransform implements ITransform {
      internal ITransform stream;

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
      internal ITransform stream;

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
      internal ITransform stream;

      public SevenBitTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        int ret = this.stream.read();
        if (ret > 0x80 || ret == 0) {
          throw new InvalidDataException("Invalid character in message body");
        }
        return ret;
      }
    }

    // A seven-bit transform used for text/plain data
    private static final class LiberalSevenBitTransform implements ITransform {
      internal ITransform stream;

      public LiberalSevenBitTransform (ITransform stream) {
        this.stream = stream;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        int ret = this.stream.read();
        if (ret > 0x80 || ret == 0) {
          return '?';
        }
        return ret;
      }
    }

    private static final class Base64Transform implements ITransform {
      private static int[] Alphabet = new int[] {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1
      };

      internal StreamWithUnget input;
      int lineCharCount;
      internal boolean lenientLineBreaks;
      internal byte[] buffer;
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
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int value = 0;
        int count = 0;
        while (count < 4) {
          int c = this.input.read();
          if (c < 0) {
            // End of stream
            if (count == 1) {
              // Not supposed to happen
              throw new InvalidDataException("Invalid number of base64 characters");
            } else if (count == 2) {
              this.input.Unget();
              value <<= 12;
              return (byte)((value >> 16) & 0xff);
            } else if (count == 3) {
              this.input.Unget();
              value <<= 18;
              this.ResizeBuffer(1);
              this.buffer[0]=(byte)((value >> 8) & 0xff);
              return (byte)((value >> 16) & 0xff);
            }
            return -1;
          } else if (c == 0x0d) {
            c = this.input.read();
            if (c == 0x0a) {
              this.lineCharCount = 0;
            } else {
              this.input.Unget();
              if (this.lenientLineBreaks) {
                this.lineCharCount = 0;
              }
            }
          } else if (c == 0x0a) {
            if (this.lenientLineBreaks) {
              this.lineCharCount = 0;
            }
          } else if (c >= 0x80) {
            ++this.lineCharCount;
            if (this.lineCharCount>MaxLineSize) {
              throw new InvalidDataException("Encoded base64 line too long");
            }
          } else {
            ++this.lineCharCount;
            if (this.lineCharCount>MaxLineSize) {
              throw new InvalidDataException("Encoded base64 line too long");
            }
            c = Alphabet[c];
            if (c >= 0) {
              value <<= 6;
              value |= c;
              ++count;
            }
          }
        }
        this.ResizeBuffer(2);
        this.buffer[0]=(byte)((value >> 8) & 0xff);
        this.buffer[1]=(byte)(value & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    private static final class BEncodingStringTransform implements ITransform {
      internal String input;
      int inputIndex;
      internal byte[] buffer;
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
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int value = 0;
        int count = 0;
        while (count < 4) {
          int c = (this.inputIndex<this.input.length()) ? this.input.charAt(this.inputIndex++) : -1;
          if (c < 0) {
            // End of stream
            if (count == 1) {
              // Not supposed to happen;
              // invalid number of base64 characters
              return '?';
            } else if (count == 2) {
              --this.inputIndex;
              value <<= 12;
              return (byte)((value >> 16) & 0xff);
            } else if (count == 3) {
              --this.inputIndex;
              value <<= 18;
              this.ResizeBuffer(1);
              this.buffer[0]=(byte)((value >> 8) & 0xff);
              return (byte)((value >> 16) & 0xff);
            }
            return -1;
          } else if (c >= 0x80) {
            // ignore
          } else {
            c = Base64Transform.Alphabet[c];
            if (c >= 0) {
              value <<= 6;
              value |= c;
              ++count;
            }
          }
        }
        this.ResizeBuffer(2);
        this.buffer[0]=(byte)((value >> 8) & 0xff);
        this.buffer[1]=(byte)(value & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    final class QEncodingStringTransform implements ITransform {
      internal String input;
      int inputIndex;
      internal byte[] buffer;
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
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size>this.buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int endIndex = this.input.length();
        while (true) {
          int c = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
          if (c < 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c == 0x0a) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c == '=') {
            int b1 = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
              --this.inputIndex;
              return '?';
            }
            int b2 = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
              --this.inputIndex;
              this.ResizeBuffer(1);
              this.buffer[0]=(byte)b1;  // will be 0-9 or a-f or A-F
              return '?';
            }
            return c;
          } else if (c <= 0x20 || c >= 0x7f) {
            // Can't occur in the Q-encoding; replace
            return '?';
          } else if (c == '_') {
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
      internal String input;
      int inputIndex;
      internal byte[] buffer;
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
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size>this.buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int endIndex = this.input.length();
        while (true) {
          int c = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
          if (c < 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else if (c == 0x0a) {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else if (c == '%') {
            int b1 = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
              --this.inputIndex;
              return '?';
            }
            int b2 = (this.inputIndex<endIndex) ? this.input.charAt(this.inputIndex++) : -1;
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
              --this.inputIndex;
              this.ResizeBuffer(1);
              this.buffer[0]=(byte)b1;  // will be 0-9 or a-f or A-F
              return '?';
            }
            return c;
          } else if ((c < 0x20 || c>= 0x7f) && c!='\t') {
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
      internal StreamWithUnget input;
      internal byte[] buffer;
      int bufferIndex;
      int bufferCount;
      internal boolean started;
      internal boolean readingHeaders;
      internal boolean hasNewBodyPart;
      internal ArrayList<String> boundaries;

    /**
     * Not documented yet.
     * @param size A 32-bit signed integer.
     */
      private void ResizeBuffer(int size) {
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size>this.buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
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
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        if (this.hasNewBodyPart) {
          return -1;
        }
        if (this.readingHeaders) {
          return this.input.read();
        }
        int c = this.input.read();
        if (c < 0) {
          this.started = false;
          return c;
        }
        if (c=='-' && this.started) {
          // Check for a boundary
          this.started = false;
          c = this.input.read();
          if (c == '-') {
            // Possible boundary candidate
            return this.CheckBoundaries(false);
          } else {
            this.input.Unget();
            return '-';
          }
        } else {
          this.started = false;
        }
        if (c == 0x0d) {
          c = this.input.read();
          if (c == 0x0a) {
            // Line break was read
            c = this.input.read();
            if (c == -1) {
              this.ResizeBuffer(1);
              this.buffer[0]=0x0a;
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              this.input.Unget();
              this.ResizeBuffer(1);
              this.buffer[0]=0x0a;
              return 0x0d;
            } else if (c != '-') {
              this.ResizeBuffer(2);
              this.buffer[0]=0x0a;
              this.buffer[1]=(byte)c;
              return 0x0d;
            }
            c = this.input.read();
            if (c == -1) {
              this.ResizeBuffer(2);
              this.buffer[0]=0x0a;
              this.buffer[1]=(byte)'-';
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              this.input.Unget();
              this.ResizeBuffer(2);
              this.buffer[0]=0x0a;
              this.buffer[1]=(byte)'-';
              return 0x0d;
            } else if (c != '-') {
              this.ResizeBuffer(3);
              this.buffer[0]=0x0a;
              this.buffer[1]=(byte)'-';
              this.buffer[2]=(byte)c;
              return 0x0d;
            }
            // Possible boundary candidate
            return this.CheckBoundaries(true);
          } else {
            this.input.Unget();
            return 0x0d;
          }
        } else {
          return c;
        }
      }

      private int CheckBoundaries(boolean includeCrLf) {
        // Reached here when the "--" of a possible
        // boundary delimiter is read. We need to
        // check boundaries here in order to find out
        // whether to emit the CRLF before the "--".

        boolean done = false;
        while (!done) {
          done = true;
          int bufferStart = 0;
          if (includeCrLf) {
            this.ResizeBuffer(3);
            bufferStart = 3;
            // store LF, '-', and '-' in the buffer in case
            // the boundary check fails, in which case
            // this method will return CR
            this.buffer[0]=0x0a;
            this.buffer[1]=(byte)'-';
            this.buffer[2]=(byte)'-';
          } else {
            bufferStart = 1;
            this.ResizeBuffer(1);
            this.buffer[0]=(byte)'-';
          }
          // Check up to 72 bytes (the maximum size
          // of a boundary plus 2 bytes for the closing
          // hyphens)
          int c;
          int bytesRead = 0;
          for (int i = 0; i < 72; ++i) {
            c = this.input.read();
            if (c < 0 || c >= 0x80 || c == 0x0d) {
              this.input.Unget();
              break;
            }
            ++bytesRead;
            // Console.Write("" + ((char)c));
            this.ResizeBuffer(bytesRead + bufferStart);
            this.buffer[bytesRead + bufferStart-1]=(byte)c;
          }
          // System.out.println("--" + (bytesRead));
          // NOTE: All boundary strings are assumed to
          // have only ASCII characters (with values
          // less than 128). Check boundaries from
          // top to bottom in the stack.
          String matchingBoundary = null;
          int matchingIndex = -1;
          for (int i = this.boundaries.size()-1;i >= 0; --i) {
            String boundary = this.boundaries.get(i);
            // System.out.println("Check boundary " + (boundary));
            if (!((boundary)==null || (boundary).length()==0) && boundary.length() <= bytesRead) {
              boolean match = true;
              for (int j = 0;j < boundary.length(); ++j) {
                if ((boundary.charAt(j)&0xff) != (int)(this.buffer[j + bufferStart] & 0xff)) {
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
            while (this.boundaries.size()>matchingIndex + 1) {
              this.boundaries.remove(matchingIndex + 1);
            }
            // Boundary line found
            if (matchingBoundary.length() + 1 < bytesRead) {
              if (this.buffer[matchingBoundary.length()+bufferStart]=='-' &&
                  this.buffer[matchingBoundary.length()+1+bufferStart]=='-') {
                closingDelim = true;
              }
            }
            // Clear the buffer, the boundary line
            // isn't part of any body data
            this.bufferCount = 0;
            this.bufferIndex = 0;
            if (closingDelim) {
              // Pop this entry, it's the top of the stack
              this.boundaries.remove(this.boundaries.size()-1);
              if (this.boundaries.size() == 0) {
                // There's nothing else significant
                // after this boundary,
                // so return now
                return -1;
              }
              // Read to end of line. Since this is the last body
              // part, the rest of the data before the next boundary
              // is insignificant
              while (true) {
                c = this.input.read();
                if (c == -1) {
                  // The body higher up didn't end yet
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = this.input.read();
                  if (c == -1) {
                    // The body higher up didn't end yet
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0a) {
                    // Start of new body part
                    c = this.input.read();
                    if (c == -1) {
                      throw new InvalidDataException("Premature end of message");
                    } else if (c == 0x0d) {
                      // Unget the CR, in case the next line is a boundary line
                      this.input.Unget();
                    } else if (c != '-') {
                      // Not a boundary delimiter
                      continue;
                    }
                    c = this.input.read();
                    if (c == -1) {
                      throw new InvalidDataException("Premature end of message");
                    } else if (c == 0x0d) {
                      // Unget the CR, in case the next line is a boundary line
                      this.input.Unget();
                    } else if (c != '-') {
                      // Not a boundary delimiter
                      continue;
                    }
                    // Found the next boundary delimiter
                    done = false;
                    break;
                  } else {
                    this.input.Unget();
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
                c = this.input.read();
                if (c == -1) {
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = this.input.read();
                  if (c == -1) {
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0a) {
                    // Start of new body part
                    this.hasNewBodyPart = true;
                    return -1;
                  } else {
                    this.input.Unget();
                  }
                }
              }
            }
          }
          // Not a boundary, return CR (the
          // ReadByte method will then return LF,
          // the hyphens, and the other bytes
          // already read)
          return includeCrLf ? 0x0d : '-';
        }
        // Not a boundary, return CR (the
        // ReadByte method will then return LF,
        // the hyphens, and the other bytes
        // already read)
        return includeCrLf ? 0x0d : '-';
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int BoundaryCount() {
        return this.boundaries.size();
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
          return this.hasNewBodyPart;
        }
    }

    final class QuotedPrintableTransform implements ITransform {
      internal StreamWithUnget input;
      int lineCharCount;
      internal boolean lenientLineBreaks;
      internal byte[] buffer;
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
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size>this.buffer.length) {
          byte[] newbuffer = new byte[size + 10];
          System.arraycopy(this.buffer, 0, newbuffer, 0, this.buffer.length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /**
     * Not documented yet.
     * @return A 32-bit signed integer.
     */
      public int ReadByte() {
        if (this.bufferIndex<this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        while (true) {
          int c = this.input.read();
          if (c < 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            c = this.input.read();
            if (c == 0x0a) {
              // CRLF
              this.ResizeBuffer(1);
              this.buffer[0]=0x0a;
              this.lineCharCount = 0;
              return 0x0d;
            } else {
              this.input.Unget();
              if (!this.lenientLineBreaks) {
                throw new InvalidDataException("Expected LF after CR");
              }
              // CR, so write CRLF
              this.ResizeBuffer(1);
              this.buffer[0]=0x0a;
              this.lineCharCount = 0;
              return 0x0d;
            }
          } else if (c == 0x0a) {
            if (!this.lenientLineBreaks) {
              throw new InvalidDataException("Expected LF after CR");
            }
            // LF, so write CRLF
            this.ResizeBuffer(1);
            this.buffer[0]=0x0a;
            this.lineCharCount = 0;
            return 0x0d;
          } else if (c == '=') {
            ++this.lineCharCount;
            if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            int b1 = this.input.read();
            int b2 = this.input.read();
            if (b2 >= 0 && b1 >= 0) {
              if (b1 == '\r' && b2=='\n') {
                // Soft line break
                this.lineCharCount = 0;
                continue;
              } else if (b1 == '\r') {
                if (!this.lenientLineBreaks) {
                  throw new InvalidDataException("Expected LF after CR");
                }
                this.lineCharCount = 0;
                this.input.Unget();
                continue;
              } else if (b1 == '\n') {
                if (!this.lenientLineBreaks) {
                  throw new InvalidDataException("Bare LF not expected");
                }
                this.lineCharCount = 0;
                this.input.Unget();
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
              this.lineCharCount+=2;
              if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
                throw new InvalidDataException("Encoded quoted-printable line too long");
              }
              return c;
            } else if (b1 >= 0) {
              if (b1 == '\r') {
                // Soft line break
                if (!this.lenientLineBreaks) {
                  throw new InvalidDataException("Expected LF after CR");
                }
                this.lineCharCount = 0;
                this.input.Unget();
                continue;
              } else if (b1 == '\n') {
                // Soft line break
                if (!this.lenientLineBreaks) {
                  throw new InvalidDataException("Bare LF not expected");
                }
                this.lineCharCount = 0;
                this.input.Unget();
                continue;
              } else {
                throw new InvalidDataException("Invalid data after equal sign");
              }
            } else {
              // Equal sign at end; ignore
              return -1;
            }
          } else if (c != '\t' && (c<0x20 || c>= 0x7f)) {
            throw new InvalidDataException("Invalid character in quoted-printable");
          } else if (c == ' ' || c=='\t') {
            // Space or tab. Since the quoted-printable spec
            // requires decoders to delete spaces and tabs before
            // CRLF, we need to create a lookahead buffer for
            // tabs and spaces read to see if they precede CRLF.
            int spaceCount = 1;
            ++this.lineCharCount;
            if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            // In most cases, though, there will only be
            // one space or tab
            int c2 = this.input.read();
            if (c2 != ' ' && c2!='\t' && c2!='\r' && c2!='\n' && c2>= 0) {
              // Simple: Space before a character other than
              // space, tab, CR, LF, or EOF
              this.input.Unget();
              return c;
            }
            boolean endsWithLineBreak = false;
            while (true) {
              if ((c2=='\n' && this.lenientLineBreaks) || c2 < 0) {
                this.input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2=='\r' && this.lenientLineBreaks) {
                this.input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2 == '\r') {
                // CR, may or may not be a line break
                c2 = this.input.read();
                // Add the CR to the
                // buffer, it won't be ignored
                this.ResizeBuffer(spaceCount);
                this.buffer[spaceCount-1]=(byte)'\r';
                if (c2 == '\n') {
                  // LF, so it's a line break
                  this.lineCharCount = 0;
                  this.ResizeBuffer(spaceCount + 1);
                  this.buffer[spaceCount]=(byte)'\n';
                  endsWithLineBreak = true;
                  break;
                } else {
                  if (!this.lenientLineBreaks) {
                    throw new InvalidDataException("Expected LF after CR");
                  }
                  this.input.Unget();  // it's something else
                  ++this.lineCharCount;
                  if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
                    throw new InvalidDataException("Encoded quoted-printable line too long");
                  }
                  break;
                }
              } else if (c2 != ' ' && c2!='\t') {
                // Not a space or tab
                this.input.Unget();
                break;
              } else {
                // An additional space or tab
                this.ResizeBuffer(spaceCount);
                this.buffer[spaceCount-1]=(byte)c2;
                ++spaceCount;
                ++this.lineCharCount;
                if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
                  throw new InvalidDataException("Encoded quoted-printable line too long");
                }
              }
              c2 = this.input.read();
            }
            // Ignore space/tab runs if the line ends in that run
            if (!endsWithLineBreak) {
              return c;
            } else {
              this.bufferCount = 0;
              continue;
            }
          } else {
            ++this.lineCharCount;
            if (this.maxLineSize >= 0 && this.lineCharCount>this.maxLineSize) {
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
      for (int i = 0;i<this.headers.size();i+=2) {
        if (this.headers.get(i).equals(name)) {
          return this.headers.get(i + 1);
        }
      }
      return null;
    }

    static boolean CanOutputRaw(String s) {
      int len = s.length();
      int chunkLength = 0;
      for (int i = 0; i < len; ++i) {
        char c = s.charAt(i);
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            return false;
          } else if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2)!=0x20)) {
            // CRLF not followed by whitespace
            return false;
          }
          chunkLength = 0;
          continue;
        }
        if (c >= 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
          // CTLs (except TAB, SPACE, and CR) and non-ASCII
          // characters
          return false;
        }
        ++chunkLength;
        if (chunkLength > 75) {
          return false;
        }
      }
      return true;
    }

    // Has non-ASCII characters, "=?", CTLs other than tab,
    // or a word longer than 75 characters
    static boolean HasTextToEscape(String s) {
      int len = s.length();
      int chunkLength = 0;
      for (int i = 0; i < len; ++i) {
        char c = s.charAt(i);
        if (c == '=' && i+1<len && c=='?') {
          // "=?" (start of an encoded word)
          return true;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s.charAt(i + 1) != 0x0a) {
            // bare CR
            return true;
          } else if (i + 2 >= len || (s.charAt(i + 2) != 0x09 && s.charAt(i + 2)!=0x20)) {
            // CRLF not followed by whitespace
            return true;
          }
          chunkLength = 0;
          continue;
        }
        if (c >= 0x7f || (c < 0x20 && c != 0x09 && c != 0x0d)) {
          // CTLs (except TAB, SPACE, and CR) and non-ASCII
          // characters
          return true;
        }
        if (c == 0x20 || c == 0x09) {
          chunkLength = 0;
        } else {
          ++chunkLength;
          if (chunkLength > 75) {
            return true;
          }
        }
      }
      return false;
    }

    private static int CharLength(String str, int index) {
      if (str == null || index < 0 || index >= str.length()) {
        return 1;
      }
      int c = str.charAt(index);
      if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.length() &&
          str.charAt(index + 1) >= 0xdc00 && str.charAt(index + 1) <= 0xdfff) {
        return 2;
      }
      return 1;
    }

    public static String ConvertCommentsToEncodedWords(String str) {
      return ConvertCommentsToEncodedWords(str, 0, str.length());
    }

    public static String ConvertCommentsToEncodedWords(String str, int index, int length) {
      // NOTE: Assumes that the comment is syntactically valid
      if (str == null) {
        throw new NullPointerException("str");
      }
      if (index < 0) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is less than " + "0");
      }
      if (index > str.length()) {
        throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is more than " + Long.toString((long)(str.length())));
      }
      if (length < 0) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
      }
      if (length > str.length()) {
        throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)(str.length())));
      }
      if (str.length() - index < length) {
        throw new IllegalArgumentException("str's length minus " + index + " (" + Long.toString((long)(str.length()-index)) + ") is less than " + Long.toString((long)length));
      }
      int endIndex = index + length;
      if (length < 2 || str.charAt(index)!='(' || str.charAt(endIndex-1)!=')') {
        return str.substring(index,(index)+(length));
      }
      Message.EncodedWordEncoder encoder;
      int nextComment = str.IndexOf('(',index+1);
      int nextBackslash = str.IndexOf('\\',index+1);
      // don't count comments or backslashes beyond
      // the desired portion
      if (nextComment >= endIndex) {
        nextComment = -1;
      }
      if (nextBackslash >= endIndex) {
        nextBackslash = -1;
      }
      boolean haveEscape = nextBackslash >= 0;
      if (!haveEscape) {
        // Check for possible folding whitespace
        nextBackslash = str.IndexOf('\n',index+1);
        if (nextBackslash >= endIndex) {
          nextBackslash = -1;
        }
        haveEscape = nextBackslash >= 0;
      }
      if (nextComment < 0 && nextBackslash< 0) {
        // No escapes or nested comments, so it's relatively easy
        if (length == 2) {
          return "()";
        }
        encoder = new Message.EncodedWordEncoder("");
        encoder.AddPrefix("(");
        encoder.AddString(str, index + 1, length - 2);
        encoder.FinalizeEncoding(")");
        return encoder.toString();
      }
      if (nextBackslash < 0) {
        // No escapes; just look for '(' and ')'
        encoder = new Message.EncodedWordEncoder("");
        while (true) {
          int parenStart = index;
          // Get the next run of parentheses
          while (index < endIndex) {
            if (str.charAt(index) == '(' || str.charAt(index)==')') {
              ++index;
            } else {
              break;
            }
          }
          // Get the next run of non-parentheses
          int parenEnd = index;
          while (index < endIndex) {
            if (str.charAt(index) == '(' || str.charAt(index)==')') {
              break;
            } else {
              ++index;
            }
          }
          if (parenEnd == index) {
            encoder.FinalizeEncoding(str.substring(parenStart,(parenStart)+(parenEnd - parenStart)));
            break;
          } else {
            encoder.AddPrefix(str.substring(parenStart,(parenStart)+(parenEnd - parenStart)));
            encoder.AddString(str, parenEnd, index - parenEnd);
          }
        }
        return encoder.toString();
      }
      StringBuilder builder = new StringBuilder();
      // escapes, but no nested comments
      if (nextComment < 0) {
        ++index;  // skip the first parenthesis
        while (index < endIndex) {
          if (str.charAt(index) == ')') {
            // End of the comment
            break;
          } else if (str.charAt(index) == '\r' && index+2<endIndex &&
                     str.charAt(index + 1)=='\n' && str.charAt(index+2)==0x20 || str.charAt(index+2)==0x09) {
            // Folding whitespace
            builder.append(str.charAt(index + 2));
            index += 3;
          } else if (str.charAt(index) == '\\' && index+1<endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.append(str.substring(index + 1,(index + 1)+(charLen)));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.append(str.charAt(index));
            ++index;
          }
        }
        if (builder.length() == 0) {
          return "()";
        }
        encoder = new Message.EncodedWordEncoder("");
        encoder.AddPrefix("(");
        encoder.AddString(builder.toString());
        encoder.FinalizeEncoding(")");
        return encoder.toString();
      }
      // escapes and nested comments
      encoder = new Message.EncodedWordEncoder("");
      while (true) {
        int parenStart = index;
        // Get the next run of parentheses
        while (index < endIndex) {
          if (str.charAt(index) == '(' || str.charAt(index)==')') {
            ++index;
          } else {
            break;
          }
        }
        // Get the next run of non-parentheses
        int parenEnd = index;
        builder.setLength(0);
        while (index < endIndex) {
          if (str.charAt(index) == '(' || str.charAt(index)==')') {
            break;
          } else if (str.charAt(index) == '\r' && index+2<endIndex &&
                     str.charAt(index + 1)=='\n' && str.charAt(index+2)==0x20 || str.charAt(index+2)==0x09) {
            // Folding whitespace
            builder.append(str.charAt(index + 2));
            index += 3;
          } else if (str.charAt(index) == '\\' && index+1<endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.append(str.substring(index + 1,(index + 1)+(charLen)));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.append(str.charAt(index));
            ++index;
          }
        }
        if (builder.length() == 0) {
          encoder.FinalizeEncoding(str.substring(parenStart,(parenStart)+(parenEnd - parenStart)));
          break;
        } else {
          encoder.AddPrefix(str.substring(parenStart,(parenStart)+(parenEnd - parenStart)));
          encoder.AddString(builder.toString());
        }
      }
      return encoder.toString();
    }

    private int TransferEncodingToUse() {
      String topLevel = this.contentType.getTopLevelType();
      if (topLevel.equals("message") || topLevel.equals("multipart")) {
        return EncodingSevenBit;
      }
      if (topLevel.equals("text")) {
        int lengthCheck = Math.min(this.body.length, 4096);
        int highBytes = 0;
        int lineLength = 0;
        // TODO: Don't use sevenbit if text contains "=_"
        boolean allTextBytes = true;
        for (int i = 0; i < lengthCheck; ++i) {
          if ((this.body[i] & 0x80) != 0) {
            ++highBytes;
            allTextBytes = false;
          } else if (this.body[i] == 0) {
            allTextBytes = false;
          } else if (this.body[i] == (byte)'\r') {
            if (i + 1>= this.body.length || this.body[i+1]!=(byte)'\n') {
              // bare CR
              allTextBytes = false;
            } else if (i > 0 && (this.body[i-1]==(byte)' ' || this.body[i-1]==(byte)'\t')) {
              // Space followed immediately by CRLF
              allTextBytes = false;
            } else {
              ++i;
              lineLength = 0;
              continue;
            }
          } else if (this.body[i] == (byte)'\n') {
            // bare LF
            allTextBytes = false;
          }
          ++lineLength;
          if (lineLength > 76) {
            allTextBytes = false;
          }
        }
        if (lengthCheck == this.body.length && allTextBytes) {
          return EncodingSevenBit;
        } if (highBytes > (lengthCheck/3)) {
          return EncodingBase64;
        } else {
          return EncodingQuotedPrintable;
        }
      }
      return EncodingBase64;
    }

    final class EncodedWordEncoder {
      internal StringBuilder currentWord;
      internal StringBuilder fullString;
      int spaceCount;

      private static String hex = "0123456789ABCDEF";

      public EncodedWordEncoder (String c) {
        this.currentWord = new StringBuilder();
        this.fullString = new StringBuilder();
        this.fullString.append(c);
        this.spaceCount = (c.length()>0) ? 1 : 0;
      }

      private void AppendChar(char ch) {
        this.PrepareToAppend(1);
        this.currentWord.append(ch);
      }

      private void PrepareToAppend(int numChars) {
        // 2 for the ending "?="
        if (this.currentWord.length() + numChars + 2>75) {
          this.spaceCount = 1;
        }
        if (this.currentWord.length() + numChars + 2>75) {
          // Encoded word would be too big,
          // so output that word
          if (this.spaceCount>0) {
            this.fullString.append(' ');
          }
          this.fullString.append(this.currentWord);
          this.fullString.append("?=");
          this.currentWord.Clear();
          this.currentWord.append("=?utf-8?q?");
          this.spaceCount = 1;
        }
      }

    /**
     * Not documented yet.
     * @param suffix A string object.
     * @return An EncodedWordEncoder object.
     */
      public EncodedWordEncoder FinalizeEncoding(String suffix) {
        if (this.currentWord.length()>0) {
          if (this.currentWord.length() + 2 + suffix.length()>75) {
            // Too big to fit the current line,
            // create a new line
            this.spaceCount = 1;
          }
          if (this.spaceCount>0) {
            this.fullString.append(' ');
          }
          this.fullString.append(this.currentWord);
          this.fullString.append("?=");
          if (suffix.length() > 0) {
            this.fullString.append(suffix);
          }
          this.spaceCount = 1;
          this.currentWord.Clear();
        }
        return this;
      }

    /**
     * Not documented yet.
     * @return An EncodedWordEncoder object.
     */
      public EncodedWordEncoder FinalizeEncoding() {
        return this.FinalizeEncoding("");
      }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return An EncodedWordEncoder object.
     */
      public EncodedWordEncoder AddPrefix(String str) {
        if (!((str)==null || (str).length()==0)) {
          this.FinalizeEncoding();
          this.currentWord.append(str);
          this.currentWord.append("=?utf-8?q?");
          this.spaceCount = 0;
        }
        return this;
      }

    /**
     * Not documented yet.
     * @param str A string object.
     * @param index A 32-bit signed integer.
     * @param length A 32-bit signed integer. (2).
     * @return An EncodedWordEncoder object.
     */
      public EncodedWordEncoder AddString(String str, int index, int length) {
        if (str == null) {
          throw new NullPointerException("str");
        }
        if (index < 0) {
          throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is less than " + "0");
        }
        if (index > str.length()) {
          throw new IllegalArgumentException("index (" + Long.toString((long)index) + ") is more than " + Long.toString((long)(str.length())));
        }
        if (length < 0) {
          throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is less than " + "0");
        }
        if (length > str.length()) {
          throw new IllegalArgumentException("length (" + Long.toString((long)length) + ") is more than " + Long.toString((long)(str.length())));
        }
        if (str.length() - index < length) {
          throw new IllegalArgumentException("str's length minus " + index + " (" + Long.toString((long)(str.length()-index)) + ") is less than " + Long.toString((long)length));
        }
        for (int j = index;j < index + length; ++j) {
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
          this.AddChar(c);
        }
        return this;
      }

    /**
     * Not documented yet.
     * @param str A string object.
     * @return An EncodedWordEncoder object.
     */
      public EncodedWordEncoder AddString(String str) {
        return this.AddString(str, 0, str.length());
      }

    /**
     * Not documented yet.
     * @param ch A 32-bit signed integer.
     */
      public void AddChar(int ch) {
        if (this.currentWord.length() == 0) {
          this.currentWord.append("=?utf-8?q?");
          this.spaceCount = 1;
        }
        if (ch == 0x20) {
          this.AppendChar('_');
        } else if (ch < 0x80 && ch>0x20 && ch!=(char)'"' && ch!=(char)',' &&
                   "?()<>[]:;@\\.=_".indexOf((char)ch) < 0) {
          this.AppendChar((char)ch);
        } else if (ch < 0x80) {
          this.PrepareToAppend(3);
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(ch >> 4));
          this.currentWord.append(hex.charAt(ch & 15));
        } else if (ch < 0x800) {
          int w = (byte)(0xc0 | ((ch >> 6) & 0x1f));
          int x = (byte)(0x80 | (ch & 0x3f));
          this.PrepareToAppend(6);
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(w >> 4));
          this.currentWord.append(hex.charAt(w & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(x >> 4));
          this.currentWord.append(hex.charAt(x & 15));
        } else if (ch < 0x10000) {
          this.PrepareToAppend(9);
          int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
          int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int y = (byte)(0x80 | (ch & 0x3f));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(w >> 4));
          this.currentWord.append(hex.charAt(w & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(x >> 4));
          this.currentWord.append(hex.charAt(x & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(y >> 4));
          this.currentWord.append(hex.charAt(y & 15));
        } else {
          this.PrepareToAppend(12);
          int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
          int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
          int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int z = (byte)(0x80 | (ch & 0x3f));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(w >> 4));
          this.currentWord.append(hex.charAt(w & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(x >> 4));
          this.currentWord.append(hex.charAt(x & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(y >> 4));
          this.currentWord.append(hex.charAt(y & 15));
          this.currentWord.append('=');
          this.currentWord.append(hex.charAt(z >> 4));
          this.currentWord.append(hex.charAt(z & 15));
        }
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
      @Override public String toString() {
        return this.fullString.toString();
      }
    }

    final class WordWrapEncoder {
      internal String lastSpaces;
      internal StringBuilder fullString;
      int lineLength;

      private static final int MaxLineLength = 76;

      public WordWrapEncoder (String c) {
        this.fullString = new StringBuilder();
        this.fullString.append(c);
        if (this.fullString.length() >= MaxLineLength) {
          this.fullString.append("\r\n");
          this.lastSpaces=" ";
          this.lineLength = 0;
        } else {
          this.lastSpaces=" ";
          this.lineLength = this.fullString.length();
        }
      }

      private void AppendSpaces(String str) {
        if (this.lineLength + this.lastSpaces.length() + str.length()>MaxLineLength) {
          // Too big to fit the current line
          this.lastSpaces=" ";
        } else {
          this.lastSpaces = str;
        }
      }

      private void AppendWord(String str) {
        if (this.lineLength + this.lastSpaces.length() + str.length()>MaxLineLength) {
          // Too big to fit the current line,
          // create a new line
          this.fullString.append("\r\n");
          this.lastSpaces=" ";
          this.lineLength = 0;
        }
        this.fullString.append(this.lastSpaces);
        this.fullString.append(str);
        this.lineLength+=this.lastSpaces.length();
        this.lineLength+=str.length();
        this.lastSpaces = "";
      }

    /**
     * Not documented yet.
     * @param str A string object.
     */
      public void AddString(String str) {
        int wordStart = 0;
        for (int j = 0;j < str.length(); ++j) {
          int c = str.charAt(j);
          if (c == 0x20 || c == 0x09) {
            int wordEnd = j;
            if (wordStart != wordEnd) {
              this.AppendWord(str.substring(wordStart,(wordStart)+(wordEnd-wordStart)));
            }
            while (j < str.length()) {
              if (str.charAt(j) == 0x20 || str.charAt(j)==0x09) {
                ++j;
              } else {
                break;
              }
            }
            wordStart = j;
            this.AppendSpaces(str.substring(wordEnd,(wordEnd)+(wordStart-wordEnd)));
            --j;
          }
        }
        if (wordStart != str.length()) {
          this.AppendWord(str.substring(wordStart,(wordStart)+(str.length()-wordStart)));
        }
      }

    /**
     * Converts this object to a text string.
     * @return A string representation of this object.
     */
      @Override public String toString() {
        return this.fullString.toString();
      }
    }

    /**
     * Not documented yet.
     * @return A string object.
     */
    public String GenerateHeaders() {
      StringBuilder sb = new StringBuilder();
      boolean haveMimeVersion = false;
      for (int i = 0;i<this.headers.size();i+=2) {
        String name = this.headers.get(i);
        String value = this.headers.get(i + 1);
        if (name.equals("mime-version")) {
          haveMimeVersion = true;
        }
        IHeaderFieldParser parser = HeaderFields.GetParser(name);
        if (!parser.IsStructured()) {
          if (CanOutputRaw(name + ":"+value)) {
            // TODO: Try to preserve header field name (before the colon)
            sb.append(name);
            sb.append(":");
            sb.append(value);
          } else {
            WordWrapEncoder encoder=new WordWrapEncoder(name+":");
            if (HasTextToEscape(value)) {
              // Convert the entire header field value to encoded
              // words
              encoder.AddString(
                new EncodedWordEncoder("")
                .AddString(value)
                .FinalizeEncoding()
                .toString());
            } else {
              encoder.AddString(value);
            }
            sb.append(encoder.toString());
            sb.append("\r\n");
          }
        } else if (name.equals("content-type") ||
                   name.equals("content-transfer-encoding")) {
          // don't write now
        } else {
          if (HasTextToEscape(value)) {
            sb.append(name);
            sb.append(':');
            // TODO: Not perfect yet
            sb.append(value);
          } else {
            WordWrapEncoder encoder=new WordWrapEncoder(name+":");
            encoder.AddString(value);
            sb.append(encoder.toString());
          }
          sb.append("\r\n");
        }
      }
      if (!haveMimeVersion) {
        sb.append("MIME-Version: 1.0\r\n");
      }
      int transferEncoding = this.TransferEncodingToUse();
       switch (transferEncoding) {
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
      MediaTypeBuilder builder = new MediaTypeBuilder(this.getContentType());
      int index = 0;
      if (builder.getTopLevelType().equals("multipart")) {
        String boundary = "=_"+Integer.toString((int)index,System.Globalization.CultureInfo.CurrentCulture);
        builder.SetParameter("boundary", boundary);
      } else if (builder.getTopLevelType().equals("text")) {
        if (transferEncoding == EncodingSevenBit) {
          builder.SetParameter("charset", "us-ascii");
        } else {
          builder.SetParameter("charset", "utf-8");
        }
      }
      sb.append("Content-Type: " + builder.ToMediaType().toString()+"\r\n");
      sb.append("\r\n");
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
          if (c == -1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (ungetStream.read() =='\n') {
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
            if (c >= 'A' && c<= 'Z') {
              c += 0x20;
            }
            sb.append((char)c);
          } else if (!first && c == ':') {
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
          if (c == -1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          if (c == '\r') {
            c = ungetStream.read();
            if (c == '\n') {
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
                  if (c == '\r') {
                    c = ungetStream.read();
                    if (c == '\n') {
                      // CRLF was read
                      sb.append("\r\n");
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
                  ++lineCount;
                  sb.append((char)c2);
                  haveFWS = true;
                  if (lineCount > 998) {
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
          if (lineCount > 998) {
            throw new InvalidDataException("Header field line too long");
          }
          if (c < 0x80) {
            sb.append((char)c);
          } else {
            if (!HeaderFields.GetParser(fieldName).IsStructured()) {
              // DEVIATION: Some emails still have an unencoded subject line
              // or other unstructured header field
              sb.append('\ufffd');
            } else {
              throw new InvalidDataException("Malformed header field value " + sb.toString());
            }
          }
        }
        String fieldValue = sb.toString();
        headerList.add(fieldName);
        headerList.add(fieldValue);
      }
    }

    private class MessageStackEntry {
      internal Message message;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public Message getMessage() {
          return this.message;
        }

      internal String boundary;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public String getBoundary() {
          return this.boundary;
        }

      public MessageStackEntry (Message msg) {

        this.message = msg;
        MediaType mediaType = msg.getContentType();
        if (mediaType.getTopLevelType().equals("multipart")) {
          this.boundary = mediaType.GetParameter("boundary");
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
      int baseTransferEncoding = this.transferEncoding;
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
            String ss = DataUtilities.GetUtf8String(
buffer,
Math.max(buffer.length-80, 0),
Math.min(buffer.length, 80),
true);
            System.out.println(ss);
            throw ex;
          }
          if (ch < 0) {
            if (boundaryChecker.getHasNewBodyPart()) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack

              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.write(buffer,0,bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.toByteArray();
              }
              while (multipartStack.size() > stackCount) {
                multipartStack.remove(stackCount);
              }
              Message parentMessage = multipartStack.get(multipartStack.size() - 1).getMessage();
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              boolean parentIsDigest = parentMessage.getContentType().SubType.equals("digest") &&
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
                if (bufferCount > 0) {
                  ms.write(buffer,0,bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.toByteArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
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

    private static ITransform MakeTransferEncoding(
ITransform stream,
int encoding,
boolean plain) {
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
      ITransform transform = MakeTransferEncoding(
stream,
transferEncoding,
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
            String ss = DataUtilities.GetUtf8String(
buffer,
Math.max(buffer.length-80, 0),
Math.min(buffer.length, 80),
true);
            System.out.println(ss);
            throw ex;
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            ms.write(buffer,0,bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
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
      this.ProcessHeaders(false, false);
      if (this.contentType.getTopLevelType().equals("multipart")) {
        this.ReadMultipartBody(stream);
      } else {
        if (this.contentType.getTopLevelType().equals("message")) {
          System.out.println(this.contentType);
        }
        this.ReadSimpleBody(stream);
      }
    }
  }
