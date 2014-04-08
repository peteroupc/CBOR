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

    private IList<Message> parts;

    private const int EncodingSevenBit = 0;
    private const int EncodingUnknown = -1;
    private const int EncodingEightBit = 3;
    private const int EncodingBinary = 4;
    private const int EncodingQuotedPrintable = 1;
    private const int EncodingBase64 = 2;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<Message> Parts {
      get {
        return this.parts;
      }
    }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public IList<string> Headers {
      get {
        return this.headers;
      }
    }

    private byte[] body;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    /// <returns>A byte[] object.</returns>
    public byte[] GetBody() {
      return this.body;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    public void SetBody(string str) {
      this.body = DataUtilities.GetUtf8Bytes(str, true);
      this.contentType = MediaType.Parse("text/plain; charset=utf-8");
    }

    public Message(Stream stream) {
      if (stream == null) {
        throw new ArgumentNullException("stream");
      }
      this.headers = new List<string>();
      this.parts = new List<Message>();
      this.ReadMessage(new WrappedStream(stream));
    }

    public Message() {
      this.headers = new List<string>();
      this.parts = new List<Message>();
    }

    internal static string ReplaceEncodedWords(string str) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      return ReplaceEncodedWords(str, 0, str.Length, false);
    }

    internal static string ReplaceEncodedWords(string str, int index, int endIndex, bool inComments) {
      #if DEBUG
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (endIndex < 0) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (endIndex > str.Length) {
        throw new ArgumentException("endIndex (" + Convert.ToString((long)endIndex, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      #endif

      if (endIndex - index < 9) {
        return str.Substring(index, endIndex - index);
      }
      if (str.IndexOf('=') < 0) {
        return str.Substring(index, endIndex - index);
      }
      StringBuilder builder = new StringBuilder();
      bool lastWordWasEncodedWord = false;
      int whitespaceStart = -1;
      int whitespaceEnd = -1;
      while (index < endIndex) {
        int charCount = 2;
        bool acceptedEncodedWord = false;
        string decodedWord = null;
        int startIndex = 0;
        bool havePossibleEncodedWord = false;
        bool startParen = false;
        if (index + 1 < endIndex && str[index]=='=' && str[index+1]=='?') {
          startIndex = index + 2;
          index += 2;
          havePossibleEncodedWord = true;
        } else if (inComments && index + 2 < endIndex && str[index]=='(' &&
                   str[index + 1] == '=' && str[index+2]=='?') {
          startIndex = index + 3;
          index += 3;
          startParen = true;
          havePossibleEncodedWord = true;
        }
        if (havePossibleEncodedWord) {
          bool maybeWord = true;
          int afterLast = endIndex;
          while (index < endIndex) {
            char c = str[index];
            ++index;
            // Check for a run of printable ASCII characters (except space)
            // with length up to 75 (also exclude '(' and ')' if 'inComments'
            // is true)
            if (c >= 0x21 && c < 0x7e && (!inComments || (c != '(' && c!=')'))) {
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
            // Console.WriteLine("maybe "+str.Substring(startIndex-2,afterLast-(startIndex-2)));
            index = startIndex;
            int i2;
            // Parse charset
            // (NOTE: Compatible with RFC 2231's addition of language
            // to charset, since charset is defined as a 'token' in
            // RFC 2047, which includes '*')
            int charsetEnd = -1;
            int encodedTextStart = -1;
            bool base64 = false;
            i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
            if (i2 != index && i2 < endIndex && str[i2]=='?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
              if (i2 != index && i2 < endIndex && str[i2]=='?') {
                // check for supported encoding (B or Q)
                char encodingChar = str[index];
                if (i2 - index == 1 && (encodingChar=='b' || encodingChar=='B' ||
                                    encodingChar == 'q' || encodingChar == 'Q')) {
                  // Parse encoded text
                  base64 = encodingChar=='b' || encodingChar=='B';
                  index = i2 + 1;
                  encodedTextStart = index;
                  i2 = MediaType.skipEncodedTextRfc2047(str, index, afterLast, inComments);
                  if (i2 != index && i2 + 1<endIndex && str[i2]=='?' && str[i2+1]=='=' &&
                      i2 + 2 == afterLast) {
                    acceptedEncodedWord = true;
                    i2 += 2;
                  }
                }
              }
            }
            if (acceptedEncodedWord) {
              string charset = str.Substring(startIndex, charsetEnd - startIndex);
              string encodedText = str.Substring(
encodedTextStart,
(afterLast - 2)- encodedTextStart);
              int asterisk = charset.IndexOf('*');
              if (asterisk >= 1) {
                charset = str.Substring(0, asterisk);
                string language = str.Substring(asterisk + 1, str.Length - (asterisk + 1));
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
                  Console.WriteLine("Unknown charset " + charset);
                  decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex-2));
                } else {
                  // Console.WriteLine("Encoded " + (base64 ? "B" : "Q") + " to: " + (encoding.GetString(transform)));
                  decodedWord = encoding.GetString(transform);
                }
                // TODO: decodedWord may itself be part of an encoded word
                // or contain ASCII control characters: encoded word decoding is
                // not idempotent; if this is a comment it could also contain '(', ')', and '\'
              } else {
                decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex-2));
              }
            } else {
              decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex-2));
            }
            index = afterLast;
          }
        }
        if (whitespaceStart >= 0 && whitespaceStart < whitespaceEnd &&
            (!acceptedEncodedWord || !lastWordWasEncodedWord)) {
          // Append whitespace as long as it doesn't occur between two
          // encoded words
          builder.Append(str.Substring(whitespaceStart, whitespaceEnd - whitespaceStart));
        }
        if (startParen) {
          builder.Append('(');
        }
        if (decodedWord != null) {
          builder.Append(decodedWord);
        }
        // Console.WriteLine("" + index + " " + endIndex + " [" + (index<endIndex ? str[index] : '~') + "]");
        // Read to whitespace
        int oldIndex = index;
        while (index < endIndex) {
          char c = str[index];
          if (c == 0x0d && index + 1 < endIndex && str[index + 1] == 0x0a) {
            break;
          } else if (c == 0x09 || c == 0x20) {
            break;
          } else {
            ++index;
          }
        }
        bool hasNonWhitespace = oldIndex != index;
        whitespaceStart = index;
        // Read to nonwhitespace
        index = HeaderParser.ParseFWS(str, index, endIndex, null);
        whitespaceEnd = index;
        if (builder.Length == 0 && oldIndex == 0 && index == str.Length) {
          // Nothing to replace, and the whole string
          // is being checked
          return str;
        }
        if (oldIndex != index) {
          // Append nonwhitespace only, unless this is the end
          if (index == endIndex) {
            builder.Append(str.Substring(oldIndex, index - oldIndex));
          } else {
            builder.Append(str.Substring(oldIndex, whitespaceStart - oldIndex));
          }
        }
        lastWordWasEncodedWord = acceptedEncodedWord;
      }
      return builder.ToString();
    }

    private MediaType contentType;
    private int transferEncoding;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
    public MediaType ContentType {
      get {
        return this.contentType;
      }
    }

    private void ProcessHeaders(bool assumeMime, bool digest) {
      bool haveContentType = false;
      bool mime = assumeMime;
      string transferEncodingValue = String.Empty;
      for (int i = 0;i < this.headers.Count;i+=2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("from")) {
          if (HeaderParser.ParseHeaderFrom(value, 0, value.Length, null) == 0) {
            Console.WriteLine(this.GetHeader("date"));
            // throw new InvalidDataException("Invalid From header: "+value);
          }
        }
        if (name.Equals("to") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderTo(value, 0, value.Length, null) == 0) {
            throw new InvalidDataException("Invalid To header: " + value);
          }
        }
        if (name.Equals("cc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderCc(value, 0, value.Length, null) == 0) {
            throw new InvalidDataException("Invalid Cc header: " + value);
          }
        }
        if (name.Equals("bcc") && !ParserUtility.IsNullEmptyOrWhitespace(value)) {
          if (HeaderParser.ParseHeaderBcc(value, 0, value.Length, null) == 0) {
            throw new InvalidDataException("Invalid Bcc header: " + value);
          }
        }
        if (name.Equals("content-transfer-encoding")) {
          int startIndex = HeaderParser.ParseCFWS(value, 0, value.Length, null);
          int endIndex = HeaderParser.ParseToken(value, startIndex, value.Length, null);
          if (HeaderParser.ParseCFWS(value, endIndex, value.Length, null) == value.Length) {
            transferEncodingValue = value.Substring(startIndex, endIndex - startIndex);
          } else {
            transferEncodingValue = String.Empty;
          }
        }
        if (name.Equals("mime-version")) {
          mime = true;
        }
      }
      bool haveFrom = false;
      bool haveSubject = false;
      bool haveTo = false;
      // TODO: Treat message/rfc822 specially
      this.contentType = digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii;
      for (int i = 0;i < this.headers.Count;i+=2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (mime && name.Equals("content-transfer-encoding")) {
          value = ParserUtility.ToLowerCaseAscii(transferEncodingValue);
          this.headers[i + 1] = value;
          if (value.Equals("7bit")) {
            this.transferEncoding = EncodingSevenBit;
          } else if (value.Equals("8bit")) {
            this.transferEncoding = EncodingEightBit;
          } else if (value.Equals("binary")) {
            this.transferEncoding = EncodingBinary;
          } else if (value.Equals("quoted-printable")) {
            this.transferEncoding = EncodingQuotedPrintable;
          } else if (value.Equals("base64")) {
            this.transferEncoding = EncodingBase64;
          } else {
            // Unrecognized transfer encoding
            this.transferEncoding = EncodingUnknown;
          }
          this.headers.RemoveAt(i);
          this.headers.RemoveAt(i);
          i -= 2;
        } else if (mime && name.Equals("content-type")) {
          if (haveContentType) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          this.contentType = MediaType.Parse(
value,
digest ? MediaType.MessageRfc822 : MediaType.TextPlainAscii);
          haveContentType = true;
        } else if (name.Equals("from")) {
          if (haveFrom) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveFrom = true;
        } else if (name.Equals("to")) {
          if (haveTo) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveTo = true;
        } else if (name.Equals("subject")) {
          if (haveSubject) {
            throw new InvalidDataException("Already have this header: " + name);
          }
          haveSubject = true;
        }
      }
      if (this.transferEncoding == EncodingUnknown) {
        this.contentType = MediaType.Parse("application/octet-stream");
      }
      if (this.transferEncoding == EncodingQuotedPrintable ||
          this.transferEncoding == EncodingBase64 ||
          this.transferEncoding == EncodingUnknown) {
        if (this.contentType.TopLevelType.Equals("multipart") ||
            this.contentType.TopLevelType.Equals("message")) {
          throw new InvalidDataException("Invalid content encoding for multipart or message");
        }
      }
    }

    private static bool IsWellFormedBoundary(string str) {
      if (str == null || str.Length < 1 || str.Length > 70) {
        return false;
      }
      for (int i = 0;i < str.Length; ++i) {
        char c = str[i];
        if (i == str.Length - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c >= 'A' && c <= 'Z') || (c>= 'a' && c<= 'z') || (c>= '0' && c<= '9') ||
          c == 0x20 || c == 0x2c || "'()-./+_:=?".IndexOf(c)>= 0)) {
          return false;
        }
      }
      return true;
    }

    private sealed class WrappedStream : ITransform {
      private Stream stream;

      public WrappedStream(Stream stream) {
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        return this.stream.ReadByte();
      }
    }

    internal sealed class StreamWithUnget : ITransform {
      private ITransform stream;
      private int lastByte;
      private bool unget;

      public StreamWithUnget(Stream stream) {
        this.lastByte = -1;
        this.stream = new WrappedStream(stream);
      }

      public StreamWithUnget(ITransform stream) {
        this.lastByte = -1;
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.unget) {
          this.unget = false;
        } else {
          this.lastByte = this.stream.ReadByte();
        }
        return this.lastByte;
      }

    /// <summary>Not documented yet.</summary>
      public void Unget() {
        this.unget = true;
      }
    }

    internal interface ITransform {
      int ReadByte();
    }

    private sealed class EightBitTransform : ITransform {
      private ITransform stream;

      public EightBitTransform(ITransform stream) {
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        int ret = this.stream.ReadByte();
        if (ret == 0) {
          throw new InvalidDataException("Invalid character in message body");
        }
        return ret;
      }
    }

    private sealed class BinaryTransform : ITransform {
      private ITransform stream;

      public BinaryTransform(ITransform stream) {
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        return this.stream.ReadByte();
      }
    }

    private sealed class SevenBitTransform : ITransform {
      private ITransform stream;

      public SevenBitTransform(ITransform stream) {
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        int ret = this.stream.ReadByte();
        if (ret > 0x80 || ret == 0) {
          throw new InvalidDataException("Invalid character in message body");
        }
        return ret;
      }
    }

    // A seven-bit transform used for text/plain data
    private sealed class LiberalSevenBitTransform : ITransform {
      private ITransform stream;

      public LiberalSevenBitTransform(ITransform stream) {
        this.stream = stream;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        int ret = this.stream.ReadByte();
        if (ret > 0x80 || ret == 0) {
          return '?';
        }
        return ret;
      }
    }

    private sealed class Base64Transform : ITransform {
      internal const int[] Alphabet = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
        52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
        -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
        15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
        -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1 };

      private StreamWithUnget input;
      private int lineCharCount;
      private bool lenientLineBreaks;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;

      private const int MaxLineSize = 76;

      public Base64Transform(ITransform input, bool lenientLineBreaks) {
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
        this.buffer = new byte[4];
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
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
          int c = this.input.ReadByte();
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
              this.buffer[0] = (byte)((value >> 8) & 0xff);
              return (byte)((value >> 16) & 0xff);
            }
            return -1;
          } else if (c == 0x0d) {
            c = this.input.ReadByte();
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
            if (this.lineCharCount > MaxLineSize) {
              throw new InvalidDataException("Encoded base64 line too long");
            }
          } else {
            ++this.lineCharCount;
            if (this.lineCharCount > MaxLineSize) {
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
        this.buffer[0] = (byte)((value >> 8) & 0xff);
        this.buffer[1] = (byte)(value & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    private sealed class BEncodingStringTransform : ITransform {
      private string input;
      private int inputIndex;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;

      private const int MaxLineSize = 76;

      public BEncodingStringTransform(String input) {
        this.input = input;
        this.buffer = new byte[4];
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
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
          int c = (this.inputIndex < this.input.Length) ? this.input[this.inputIndex++] : -1;
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
              this.buffer[0] = (byte)((value >> 8) & 0xff);
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
        this.buffer[0] = (byte)((value >> 8) & 0xff);
        this.buffer[1] = (byte)(value & 0xff);
        return (byte)((value >> 16) & 0xff);
      }
    }

    internal sealed class QEncodingStringTransform : ITransform {
      private String input;
      private int inputIndex;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;

      public QEncodingStringTransform(
        String input) {
        this.input = input;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size > this.buffer.Length) {
          byte[] newbuffer = new byte[size + 10];
          Array.Copy(this.buffer, newbuffer, this.buffer.Length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int endIndex = this.input.Length;
        while (true) {
          int c = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
            int b1 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
            int b2 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
              this.buffer[0] = (byte)b1;  // will be 0-9 or a-f or A-F
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

    internal sealed class PercentEncodingStringTransform : ITransform {
      private String input;
      private int inputIndex;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;

      public PercentEncodingStringTransform(
        String input) {
        this.input = input;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size > this.buffer.Length) {
          byte[] newbuffer = new byte[size + 10];
          Array.Copy(this.buffer, newbuffer, this.buffer.Length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
          int ret = this.buffer[this.bufferIndex];
          ++this.bufferIndex;
          if (this.bufferIndex == this.bufferCount) {
            this.bufferCount = 0;
            this.bufferIndex = 0;
          }
          ret &= 0xff;
          return ret;
        }
        int endIndex = this.input.Length;
        while (true) {
          int c = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
            int b1 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
            int b2 = (this.inputIndex < endIndex) ? this.input[this.inputIndex++] : -1;
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
              this.buffer[0] = (byte)b1;  // will be 0-9 or a-f or A-F
              return '?';
            }
            return c;
          } else if ((c < 0x20 || c >= 0x7f) && c!='\t') {
            // Can't occur in parameter value percent-encoding; replace
            return '?';
          } else {
            // printable ASCII, return that byte
            return c;
          }
        }
      }
    }

    internal sealed class BoundaryCheckerTransform : ITransform {
      private StreamWithUnget input;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;
      private bool started;
      private bool readingHeaders;
      private bool hasNewBodyPart;
      private List<string> boundaries;

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size > this.buffer.Length) {
          byte[] newbuffer = new byte[size + 10];
          Array.Copy(this.buffer, newbuffer, this.buffer.Length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

      public BoundaryCheckerTransform(ITransform stream) {
        this.input = new StreamWithUnget(stream);
        this.boundaries = new List<string>();
        this.started = true;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='boundary'>A string object.</param>
      public void PushBoundary(string boundary) {
        this.boundaries.Add(boundary);
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
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
          return this.input.ReadByte();
        }
        int c = this.input.ReadByte();
        if (c < 0) {
          this.started = false;
          return c;
        }
        if (c == '-' && this.started) {
          // Check for a boundary
          this.started = false;
          c = this.input.ReadByte();
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
          c = this.input.ReadByte();
          if (c == 0x0a) {
            // Line break was read
            c = this.input.ReadByte();
            if (c == -1) {
              this.ResizeBuffer(1);
              this.buffer[0] = 0x0a;
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              this.input.Unget();
              this.ResizeBuffer(1);
              this.buffer[0] = 0x0a;
              return 0x0d;
            } else if (c != '-') {
              this.ResizeBuffer(2);
              this.buffer[0] = 0x0a;
              this.buffer[1] = (byte)c;
              return 0x0d;
            }
            c = this.input.ReadByte();
            if (c == -1) {
              this.ResizeBuffer(2);
              this.buffer[0] = 0x0a;
              this.buffer[1] = (byte)'-';
              return 0x0d;
            } else if (c == 0x0d) {
              // Unget the CR, in case the next line is a boundary line
              this.input.Unget();
              this.ResizeBuffer(2);
              this.buffer[0] = 0x0a;
              this.buffer[1] = (byte)'-';
              return 0x0d;
            } else if (c != '-') {
              this.ResizeBuffer(3);
              this.buffer[0] = 0x0a;
              this.buffer[1] = (byte)'-';
              this.buffer[2] = (byte)c;
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

      private int CheckBoundaries(bool includeCrLf) {
        // Reached here when the "--" of a possible
        // boundary delimiter is read. We need to
        // check boundaries here in order to find out
        // whether to emit the CRLF before the "--".
        #if DEBUG
        if (this.bufferCount != 0) {
          throw new ArgumentException("this.bufferCount (" + Convert.ToString(
(long)this.bufferCount,
System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
        }
        #endif

        bool done = false;
        while (!done) {
          done = true;
          int bufferStart = 0;
          if (includeCrLf) {
            this.ResizeBuffer(3);
            bufferStart = 3;
            // store LF, '-', and '-' in the buffer in case
            // the boundary check fails, in which case
            // this method will return CR
            this.buffer[0] = 0x0a;
            this.buffer[1] = (byte)'-';
            this.buffer[2] = (byte)'-';
          } else {
            bufferStart = 1;
            this.ResizeBuffer(1);
            this.buffer[0] = (byte)'-';
          }
          // Check up to 72 bytes (the maximum size
          // of a boundary plus 2 bytes for the closing
          // hyphens)
          int c;
          int bytesRead = 0;
          for (int i = 0; i < 72; ++i) {
            c = this.input.ReadByte();
            if (c < 0 || c >= 0x80 || c == 0x0d) {
              this.input.Unget();
              break;
            }
            ++bytesRead;
            // Console.Write("" + ((char)c));
            this.ResizeBuffer(bytesRead + bufferStart);
            this.buffer[bytesRead + bufferStart - 1]=(byte)c;
          }
          // Console.WriteLine("--" + (bytesRead));
          // NOTE: All boundary strings are assumed to
          // have only ASCII characters (with values
          // less than 128). Check boundaries from
          // top to bottom in the stack.
          string matchingBoundary = null;
          int matchingIndex = -1;
          for (int i = this.boundaries.Count - 1;i >= 0; --i) {
            string boundary = this.boundaries[i];
            // Console.WriteLine("Check boundary " + (boundary));
            if (!String.IsNullOrEmpty(boundary) && boundary.Length <= bytesRead) {
              bool match = true;
              for (int j = 0;j < boundary.Length; ++j) {
                if ((boundary[j] & 0xff) != (int)(this.buffer[j + bufferStart] & 0xff)) {
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
            bool closingDelim = false;
            // Pop the stack until the matching body part
            // is on top
            while (this.boundaries.Count > matchingIndex + 1) {
              this.boundaries.RemoveAt(matchingIndex + 1);
            }
            // Boundary line found
            if (matchingBoundary.Length + 1 < bytesRead) {
              if (this.buffer[matchingBoundary.Length + bufferStart]=='-' &&
                  this.buffer[matchingBoundary.Length + 1+bufferStart]=='-') {
                closingDelim = true;
              }
            }
            // Clear the buffer, the boundary line
            // isn't part of any body data
            this.bufferCount = 0;
            this.bufferIndex = 0;
            if (closingDelim) {
              // Pop this entry, it's the top of the stack
              this.boundaries.RemoveAt(this.boundaries.Count - 1);
              if (this.boundaries.Count == 0) {
                // There's nothing else significant
                // after this boundary,
                // so return now
                return -1;
              }
              // Read to end of line. Since this is the last body
              // part, the rest of the data before the next boundary
              // is insignificant
              while (true) {
                c = this.input.ReadByte();
                if (c == -1) {
                  // The body higher up didn't end yet
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = this.input.ReadByte();
                  if (c == -1) {
                    // The body higher up didn't end yet
                    throw new InvalidDataException("Premature end of message");
                  } else if (c == 0x0a) {
                    // Start of new body part
                    c = this.input.ReadByte();
                    if (c == -1) {
                      throw new InvalidDataException("Premature end of message");
                    } else if (c == 0x0d) {
                      // Unget the CR, in case the next line is a boundary line
                      this.input.Unget();
                    } else if (c != '-') {
                      // Not a boundary delimiter
                      continue;
                    }
                    c = this.input.ReadByte();
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
                c = this.input.ReadByte();
                if (c == -1) {
                  throw new InvalidDataException("Premature end of message");
                } else if (c == 0x0d) {
                  c = this.input.ReadByte();
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

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int BoundaryCount() {
        return this.boundaries.Count;
      }

    /// <summary>Not documented yet.</summary>
      public void StartBodyPartHeaders() {
        #if DEBUG
        if (!this.hasNewBodyPart) {
          throw new ArgumentException("doesn't satisfy this.hasNewBodyPart");
        }
        if (this.readingHeaders) {
          throw new ArgumentException("doesn't satisfy !this.hasNewBodyPart");
        }
        if (!this.bufferCount.Equals(0)) {
          throw new ArgumentException("this.bufferCount (" + Convert.ToString((long)this.bufferCount, System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
        }
        #endif

        this.readingHeaders = true;
        this.hasNewBodyPart = false;
      }

    /// <summary>Not documented yet.</summary>
      public void EndBodyPartHeaders() {
        #if DEBUG
        if (!this.readingHeaders) {
          throw new ArgumentException("doesn't satisfy this.readingHeaders");
        }
        if (!this.bufferCount.Equals(0)) {
          throw new ArgumentException("this.bufferCount (" + Convert.ToString((long)this.bufferCount, System.Globalization.CultureInfo.InvariantCulture) + ") is not equal to " + "0");
        }
        #endif

        this.readingHeaders = false;
        this.hasNewBodyPart = false;
      }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public bool HasNewBodyPart {
        get {
          return this.hasNewBodyPart;
        }
      }
    }

    internal sealed class QuotedPrintableTransform : ITransform {
      private StreamWithUnget input;
      private int lineCharCount;
      private bool lenientLineBreaks;
      private byte[] buffer;
      private int bufferIndex;
      private int bufferCount;

      private int maxLineSize;

      public QuotedPrintableTransform(
        ITransform input,
        bool lenientLineBreaks,
        int maxLineSize) {
        this.maxLineSize = maxLineSize;
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
      }

      public QuotedPrintableTransform(
        Stream input,
        bool lenientLineBreaks,
        int maxLineSize) {
        this.maxLineSize = maxLineSize;
        this.input = new StreamWithUnget(new WrappedStream(input));
        this.lenientLineBreaks = lenientLineBreaks;
      }

      public QuotedPrintableTransform(
        ITransform input,
        bool lenientLineBreaks) {
        // DEVIATION: The max line size is actually 76, but some emails
        // write lines that exceed this size
        this.maxLineSize = 200;
        this.input = new StreamWithUnget(input);
        this.lenientLineBreaks = lenientLineBreaks;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='size'>A 32-bit signed integer.</param>
      private void ResizeBuffer(int size) {
        if (this.buffer == null) {
          this.buffer = new byte[size + 10];
        } else if (size > this.buffer.Length) {
          byte[] newbuffer = new byte[size + 10];
          Array.Copy(this.buffer, newbuffer, this.buffer.Length);
          this.buffer = newbuffer;
        }
        this.bufferCount = size;
        this.bufferIndex = 0;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
      public int ReadByte() {
        if (this.bufferIndex < this.bufferCount) {
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
          int c = this.input.ReadByte();
          if (c < 0) {
            // End of stream
            return -1;
          } else if (c == 0x0d) {
            c = this.input.ReadByte();
            if (c == 0x0a) {
              // CRLF
              this.ResizeBuffer(1);
              this.buffer[0] = 0x0a;
              this.lineCharCount = 0;
              return 0x0d;
            } else {
              this.input.Unget();
              if (!this.lenientLineBreaks) {
                throw new InvalidDataException("Expected LF after CR");
              }
              // CR, so write CRLF
              this.ResizeBuffer(1);
              this.buffer[0] = 0x0a;
              this.lineCharCount = 0;
              return 0x0d;
            }
          } else if (c == 0x0a) {
            if (!this.lenientLineBreaks) {
              throw new InvalidDataException("Expected LF after CR");
            }
            // LF, so write CRLF
            this.ResizeBuffer(1);
            this.buffer[0] = 0x0a;
            this.lineCharCount = 0;
            return 0x0d;
          } else if (c == '=') {
            ++this.lineCharCount;
            if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            int b1 = this.input.ReadByte();
            int b2 = this.input.ReadByte();
            if (b2 >= 0 && b1 >= 0) {
              if (b1 == '\r' && b2 == '\n') {
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
              this.lineCharCount += 2;
              if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
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
          } else if (c != '\t' && (c < 0x20 || c>= 0x7f)) {
            throw new InvalidDataException("Invalid character in quoted-printable");
          } else if (c == ' ' || c == '\t') {
            // Space or tab. Since the quoted-printable spec
            // requires decoders to delete spaces and tabs before
            // CRLF, we need to create a lookahead buffer for
            // tabs and spaces read to see if they precede CRLF.
            int spaceCount = 1;
            ++this.lineCharCount;
            if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            // In most cases, though, there will only be
            // one space or tab
            int c2 = this.input.ReadByte();
            if (c2 != ' ' && c2 != '\t' && c2!='\r' && c2!='\n' && c2>= 0) {
              // Simple: Space before a character other than
              // space, tab, CR, LF, or EOF
              this.input.Unget();
              return c;
            }
            bool endsWithLineBreak = false;
            while (true) {
              if ((c2 == '\n' && this.lenientLineBreaks) || c2 < 0) {
                this.input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2 == '\r' && this.lenientLineBreaks) {
                this.input.Unget();
                endsWithLineBreak = true;
                break;
              } else if (c2 == '\r') {
                // CR, may or may not be a line break
                c2 = this.input.ReadByte();
                // Add the CR to the
                // buffer, it won't be ignored
                this.ResizeBuffer(spaceCount);
                this.buffer[spaceCount - 1]=(byte)'\r';
                if (c2 == '\n') {
                  // LF, so it's a line break
                  this.lineCharCount = 0;
                  this.ResizeBuffer(spaceCount + 1);
                  this.buffer[spaceCount] = (byte)'\n';
                  endsWithLineBreak = true;
                  break;
                } else {
                  if (!this.lenientLineBreaks) {
                    throw new InvalidDataException("Expected LF after CR");
                  }
                  this.input.Unget();  // it's something else
                  ++this.lineCharCount;
                  if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
                    throw new InvalidDataException("Encoded quoted-printable line too long");
                  }
                  break;
                }
              } else if (c2 != ' ' && c2 != '\t') {
                // Not a space or tab
                this.input.Unget();
                break;
              } else {
                // An additional space or tab
                this.ResizeBuffer(spaceCount);
                this.buffer[spaceCount - 1]=(byte)c2;
                ++spaceCount;
                ++this.lineCharCount;
                if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
                  throw new InvalidDataException("Encoded quoted-printable line too long");
                }
              }
              c2 = this.input.ReadByte();
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
            if (this.maxLineSize >= 0 && this.lineCharCount > this.maxLineSize) {
              throw new InvalidDataException("Encoded quoted-printable line too long");
            }
            return c;
          }
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetHeader(string name) {
      name = ParserUtility.ToLowerCaseAscii(name);
      for (int i = 0;i < this.headers.Count;i+=2) {
        if (this.headers[i].Equals(name)) {
          return this.headers[i + 1];
        }
      }
      return null;
    }

    internal static bool CanOutputRaw(string s) {
      int len = s.Length;
      int chunkLength = 0;
      for (int i = 0; i < len; ++i) {
        char c = s[i];
        if (c == 0x0d) {
          if (i + 1 >= len || s[i + 1] != 0x0a) {
            // bare CR
            return false;
          } else if (i + 2 >= len || (s[i + 2] != 0x09 && s[i + 2] != 0x20)) {
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
    internal static bool HasTextToEscape(string s) {
      int len = s.Length;
      int chunkLength = 0;
      for (int i = 0; i < len; ++i) {
        char c = s[i];
        if (c == '=' && i + 1<len && c=='?') {
          // "=?" (start of an encoded word)
          return true;
        }
        if (c == 0x0d) {
          if (i + 1 >= len || s[i + 1] != 0x0a) {
            // bare CR
            return true;
          } else if (i + 2 >= len || (s[i + 2] != 0x09 && s[i + 2] != 0x20)) {
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

    private static int CharLength(string str, int index) {
      if (str == null || index < 0 || index >= str.Length) {
        return 1;
      }
      int c = str[index];
      if (c >= 0xd800 && c <= 0xdbff && index + 1 < str.Length &&
          str[index + 1] >= 0xdc00 && str[index + 1] <= 0xdfff) {
        return 2;
      }
      return 1;
    }

    public static string ConvertCommentsToEncodedWords(string str) {
      return ConvertCommentsToEncodedWords(str, 0, str.Length);
    }

    public static string ConvertCommentsToEncodedWords(string str, int index, int length) {
      // NOTE: Assumes that the comment is syntactically valid
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (index < 0) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (index > str.Length) {
        throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (length < 0) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (length > str.Length) {
        throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (str.Length - index < length) {
        throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((long)(str.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
      }
      int endIndex = index + length;
      if (length < 2 || str[index] != '(' || str[endIndex-1]!=')') {
        return str.Substring(index, length);
      }
      Message.EncodedWordEncoder encoder;
      int nextComment = str.IndexOf('(', index+1);
      int nextBackslash = str.IndexOf('\\', index+1);
      // don't count comments or backslashes beyond
      // the desired portion
      if (nextComment >= endIndex) {
        nextComment = -1;
      }
      if (nextBackslash >= endIndex) {
        nextBackslash = -1;
      }
      bool haveEscape = nextBackslash >= 0;
      if (!haveEscape) {
        // Check for possible folding whitespace
        nextBackslash = str.IndexOf('\n', index+1);
        if (nextBackslash >= endIndex) {
          nextBackslash = -1;
        }
        haveEscape = nextBackslash >= 0;
      }
      if (nextComment < 0 && nextBackslash < 0) {
        // No escapes or nested comments, so it's relatively easy
        if (length == 2) {
          return "()";
        }
        encoder = new Message.EncodedWordEncoder(String.Empty);
        encoder.AddPrefix("(");
        encoder.AddString(str, index + 1, length - 2);
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      if (nextBackslash < 0) {
        // No escapes; just look for '(' and ')'
        encoder = new Message.EncodedWordEncoder(String.Empty);
        while (true) {
          int parenStart = index;
          // Get the next run of parentheses
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              ++index;
            } else {
              break;
            }
          }
          // Get the next run of non-parentheses
          int parenEnd = index;
          while (index < endIndex) {
            if (str[index] == '(' || str[index] == ')') {
              break;
            } else {
              ++index;
            }
          }
          if (parenEnd == index) {
            encoder.FinalizeEncoding(str.Substring(parenStart, parenEnd - parenStart));
            break;
          } else {
            encoder.AddPrefix(str.Substring(parenStart, parenEnd - parenStart));
            encoder.AddString(str, parenEnd, index - parenEnd);
          }
        }
        return encoder.ToString();
      }
      StringBuilder builder = new StringBuilder();
      // escapes, but no nested comments
      if (nextComment < 0) {
        ++index;  // skip the first parenthesis
        while (index < endIndex) {
          if (str[index] == ')') {
            // End of the comment
            break;
          } else if (str[index] == '\r' && index + 2<endIndex &&
                     str[index + 1] == '\n' && str[index+2]==0x20 || str[index+2]==0x09) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1<endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.Append(str.Substring(index + 1, charLen));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        if (builder.Length == 0) {
          return "()";
        }
        encoder = new Message.EncodedWordEncoder(String.Empty);
        encoder.AddPrefix("(");
        encoder.AddString(builder.ToString());
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      // escapes and nested comments
      encoder = new Message.EncodedWordEncoder(String.Empty);
      while (true) {
        int parenStart = index;
        // Get the next run of parentheses
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            ++index;
          } else {
            break;
          }
        }
        // Get the next run of non-parentheses
        int parenEnd = index;
        builder.Clear();
        while (index < endIndex) {
          if (str[index] == '(' || str[index] == ')') {
            break;
          } else if (str[index] == '\r' && index + 2<endIndex &&
                     str[index + 1] == '\n' && str[index+2]==0x20 || str[index+2]==0x09) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1<endIndex) {
            // Quoted pair
            int charLen = CharLength(str, index + 1);
            builder.Append(str.Substring(index + 1, charLen));
            index += 1 + charLen;
          } else {
            // Other comment text
            builder.Append(str[index]);
            ++index;
          }
        }
        if (builder.Length == 0) {
          encoder.FinalizeEncoding(str.Substring(parenStart, parenEnd - parenStart));
          break;
        } else {
          encoder.AddPrefix(str.Substring(parenStart, parenEnd - parenStart));
          encoder.AddString(builder.ToString());
        }
      }
      return encoder.ToString();
    }

    private int TransferEncodingToUse() {
      string topLevel = this.contentType.TopLevelType;
      if (topLevel.Equals("message") || topLevel.Equals("multipart")) {
        return EncodingSevenBit;
      }
      if (topLevel.Equals("text")) {
        int lengthCheck = Math.Min(this.body.Length, 4096);
        int highBytes = 0;
        int lineLength = 0;
        // TODO: Don't use sevenbit if text contains "=_"
        bool allTextBytes = true;
        for (int i = 0; i < lengthCheck; ++i) {
          if ((this.body[i] & 0x80) != 0) {
            ++highBytes;
            allTextBytes = false;
          } else if (this.body[i] == 0) {
            allTextBytes = false;
          } else if (this.body[i] == (byte)'\r') {
            if (i + 1 >= this.body.Length || this.body[i+1]!=(byte)'\n') {
              // bare CR
              allTextBytes = false;
            } else if (i > 0 && (this.body[i - 1]==(byte)' ' || this.body[i-1]==(byte)'\t')) {
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
        if (lengthCheck == this.body.Length && allTextBytes) {
          return EncodingSevenBit;
        } if (highBytes > (lengthCheck / 3)) {
          return EncodingBase64;
        } else {
          return EncodingQuotedPrintable;
        }
      }
      return EncodingBase64;
    }

    internal sealed class EncodedWordEncoder {
      private StringBuilder currentWord;
      private StringBuilder fullString;
      private int spaceCount;

      private static string hex = "0123456789ABCDEF";

      public EncodedWordEncoder(string c) {
        this.currentWord = new StringBuilder();
        this.fullString = new StringBuilder();
        this.fullString.Append(c);
        this.spaceCount = (c.Length > 0) ? 1 : 0;
      }

      private void AppendChar(char ch) {
        this.PrepareToAppend(1);
        this.currentWord.Append(ch);
      }

      private void PrepareToAppend(int numChars) {
        // 2 for the ending "?="
        if (this.currentWord.Length + numChars + 2 > 75) {
          this.spaceCount = 1;
        }
        if (this.currentWord.Length + numChars + 2 > 75) {
          // Encoded word would be too big,
          // so output that word
          if (this.spaceCount > 0) {
            this.fullString.Append(' ');
          }
          this.fullString.Append(this.currentWord);
          this.fullString.Append("?=");
          this.currentWord.Clear();
          this.currentWord.Append("=?utf-8?q?");
          this.spaceCount = 1;
        }
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An EncodedWordEncoder object.</returns>
    /// <param name='suffix'>A string object.</param>
      public EncodedWordEncoder FinalizeEncoding(string suffix) {
        if (this.currentWord.Length > 0) {
          if (this.currentWord.Length + 2 + suffix.Length > 75) {
            // Too big to fit the current line,
            // create a new line
            this.spaceCount = 1;
          }
          if (this.spaceCount > 0) {
            this.fullString.Append(' ');
          }
          this.fullString.Append(this.currentWord);
          this.fullString.Append("?=");
          if (suffix.Length > 0) {
            this.fullString.Append(suffix);
          }
          this.spaceCount = 1;
          this.currentWord.Clear();
        }
        return this;
      }

    /// <summary>Not documented yet.</summary>
    /// <returns>An EncodedWordEncoder object.</returns>
      public EncodedWordEncoder FinalizeEncoding() {
        return this.FinalizeEncoding(String.Empty);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
      public EncodedWordEncoder AddPrefix(string str) {
        if (!String.IsNullOrEmpty(str)) {
          this.FinalizeEncoding();
          this.currentWord.Append(str);
          this.currentWord.Append("=?utf-8?q?");
          this.spaceCount = 0;
        }
        return this;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
    /// <param name='index'>A 32-bit signed integer.</param>
    /// <param name='length'>A 32-bit signed integer. (2).</param>
      public EncodedWordEncoder AddString(string str, int index, int length) {
        if (str == null) {
          throw new ArgumentNullException("str");
        }
        if (index < 0) {
          throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        if (index > str.Length) {
          throw new ArgumentException("index (" + Convert.ToString((long)index, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
        }
        if (length < 0) {
          throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
        }
        if (length > str.Length) {
          throw new ArgumentException("length (" + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)str.Length, System.Globalization.CultureInfo.InvariantCulture));
        }
        if (str.Length - index < length) {
          throw new ArgumentException("str's length minus " + index + " (" + Convert.ToString((long)(str.Length - index), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)length, System.Globalization.CultureInfo.InvariantCulture));
        }
        for (int j = index;j < index + length; ++j) {
          int c = str[j];
          if (c >= 0xd800 && c <= 0xdbff && j + 1 < str.Length &&
              str[j + 1] >= 0xdc00 && str[j + 1] <= 0xdfff) {
            // Get the Unicode code point for the surrogate pair
            c = 0x10000 + ((c - 0xd800) * 0x400) + (str[j + 1] - 0xdc00);
            ++j;
          } else if (c >= 0xd800 && c <= 0xdfff) {
            // unpaired surrogate
            c = 0xfffd;
          }
          this.AddChar(c);
        }
        return this;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
    /// <returns>An EncodedWordEncoder object.</returns>
      public EncodedWordEncoder AddString(string str) {
        return this.AddString(str, 0, str.Length);
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='ch'>A 32-bit signed integer.</param>
      public void AddChar(int ch) {
        if (this.currentWord.Length == 0) {
          this.currentWord.Append("=?utf-8?q?");
          this.spaceCount = 1;
        }
        if (ch == 0x20) {
          this.AppendChar('_');
        } else if (ch < 0x80 && ch > 0x20 && ch!=(char)'"' && ch!=(char)',' &&
                   "?()<>[]:;@\\.=_".IndexOf((char)ch) < 0) {
          this.AppendChar((char)ch);
        } else if (ch < 0x80) {
          this.PrepareToAppend(3);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[ch >> 4]);
          this.currentWord.Append(hex[ch & 15]);
        } else if (ch < 0x800) {
          int w = (byte)(0xc0 | ((ch >> 6) & 0x1f));
          int x = (byte)(0x80 | (ch & 0x3f));
          this.PrepareToAppend(6);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[w >> 4]);
          this.currentWord.Append(hex[w & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[x >> 4]);
          this.currentWord.Append(hex[x & 15]);
        } else if (ch < 0x10000) {
          this.PrepareToAppend(9);
          int w = (byte)(0xe0 | ((ch >> 12) & 0x0f));
          int x = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int y = (byte)(0x80 | (ch & 0x3f));
          this.currentWord.Append('=');
          this.currentWord.Append(hex[w >> 4]);
          this.currentWord.Append(hex[w & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[x >> 4]);
          this.currentWord.Append(hex[x & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[y >> 4]);
          this.currentWord.Append(hex[y & 15]);
        } else {
          this.PrepareToAppend(12);
          int w = (byte)(0xf0 | ((ch >> 18) & 0x07));
          int x = (byte)(0x80 | ((ch >> 12) & 0x3f));
          int y = (byte)(0x80 | ((ch >> 6) & 0x3f));
          int z = (byte)(0x80 | (ch & 0x3f));
          this.currentWord.Append('=');
          this.currentWord.Append(hex[w >> 4]);
          this.currentWord.Append(hex[w & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[x >> 4]);
          this.currentWord.Append(hex[x & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[y >> 4]);
          this.currentWord.Append(hex[y & 15]);
          this.currentWord.Append('=');
          this.currentWord.Append(hex[z >> 4]);
          this.currentWord.Append(hex[z & 15]);
        }
      }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
      public override string ToString() {
        return this.fullString.ToString();
      }
    }

    internal sealed class WordWrapEncoder {
      private string lastSpaces;
      private StringBuilder fullString;
      private int lineLength;

      private const int MaxLineLength = 76;

      public WordWrapEncoder(string c) {
        this.fullString = new StringBuilder();
        this.fullString.Append(c);
        if (this.fullString.Length >= MaxLineLength) {
          this.fullString.Append("\r\n");
          this.lastSpaces = " ";
          this.lineLength = 0;
        } else {
          this.lastSpaces = " ";
          this.lineLength = this.fullString.Length;
        }
      }

      private void AppendSpaces(string str) {
        if (this.lineLength + this.lastSpaces.Length + str.Length > MaxLineLength) {
          // Too big to fit the current line
          this.lastSpaces = " ";
        } else {
          this.lastSpaces = str;
        }
      }

      private void AppendWord(string str) {
        if (this.lineLength + this.lastSpaces.Length + str.Length > MaxLineLength) {
          // Too big to fit the current line,
          // create a new line
          this.fullString.Append("\r\n");
          this.lastSpaces = " ";
          this.lineLength = 0;
        }
        this.fullString.Append(this.lastSpaces);
        this.fullString.Append(str);
        this.lineLength += this.lastSpaces.Length;
        this.lineLength += str.Length;
        this.lastSpaces = String.Empty;
      }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A string object.</param>
      public void AddString(string str) {
        int wordStart = 0;
        for (int j = 0;j < str.Length; ++j) {
          int c = str[j];
          if (c == 0x20 || c == 0x09) {
            int wordEnd = j;
            if (wordStart != wordEnd) {
              this.AppendWord(str.Substring(wordStart, wordEnd - wordStart));
            }
            while (j < str.Length) {
              if (str[j] == 0x20 || str[j] == 0x09) {
                ++j;
              } else {
                break;
              }
            }
            wordStart = j;
            this.AppendSpaces(str.Substring(wordEnd, wordStart - wordEnd));
            --j;
          }
        }
        if (wordStart != str.Length) {
          this.AppendWord(str.Substring(wordStart, str.Length - wordStart));
        }
      }

    /// <summary>Converts this object to a text string.</summary>
    /// <returns>A string representation of this object.</returns>
      public override string ToString() {
        return this.fullString.ToString();
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    public string GenerateHeaders() {
      StringBuilder sb = new StringBuilder();
      bool haveMimeVersion = false;
      for (int i = 0;i < this.headers.Count;i+=2) {
        string name = this.headers[i];
        string value = this.headers[i + 1];
        if (name.Equals("mime-version")) {
          haveMimeVersion = true;
        }
        IHeaderFieldParser parser = HeaderFields.GetParser(name);
        if (!parser.IsStructured()) {
          if (CanOutputRaw(name + ":" + value)) {
            // TODO: Try to preserve header field name (before the colon)
            sb.Append(name);
            sb.Append(":");
            sb.Append(value);
          } else {
            var encoder = new WordWrapEncoder(name + ":");
            if (HasTextToEscape(value)) {
              // Convert the entire header field value to encoded
              // words
              encoder.AddString(
                new EncodedWordEncoder(String.Empty)
                .AddString(value)
                .FinalizeEncoding()
                .ToString());
            } else {
              encoder.AddString(value);
            }
            sb.Append(encoder.ToString());
            sb.Append("\r\n");
          }
        } else if (name.Equals("content-type") ||
                   name.Equals("content-transfer-encoding")) {
          // don't write now
        } else {
          if (HasTextToEscape(value)) {
            sb.Append(name);
            sb.Append(':');
            // TODO: Not perfect yet
            sb.Append(value);
          } else {
            var encoder = new WordWrapEncoder(name + ":");
            encoder.AddString(value);
            sb.Append(encoder.ToString());
          }
          sb.Append("\r\n");
        }
      }
      if (!haveMimeVersion) {
        sb.Append("MIME-Version: 1.0\r\n");
      }
      int transferEncoding = this.TransferEncodingToUse();
       switch (transferEncoding) {
        case EncodingBase64:
          sb.Append("Content-Transfer-Encoding: base64\r\n");
          break;
        case EncodingQuotedPrintable:
          sb.Append("Content-Transfer-Encoding: quoted-printable\r\n");
          break;
        default:
          sb.Append("Content-Transfer-Encoding: 7bit\r\n");
          break;
      }
      MediaTypeBuilder builder = new MediaTypeBuilder(this.ContentType);
      int index = 0;
      if (builder.TopLevelType.Equals("multipart")) {
        string boundary = "=_" + Convert.ToString((int)index,System.Globalization.CultureInfo.CurrentCulture);
        builder.SetParameter("boundary", boundary);
      } else if (builder.TopLevelType.Equals("text")) {
        if (transferEncoding == EncodingSevenBit) {
          builder.SetParameter("charset", "us-ascii");
        } else {
          builder.SetParameter("charset", "utf-8");
        }
      }
      sb.Append("Content-Type: " + builder.ToMediaType().ToString() +"\r\n");
      sb.Append("\r\n");
      return sb.ToString();
    }

    private static void ReadHeaders(
      ITransform stream,
      IList<string> headerList) {
      int lineCount = 0;
      StringBuilder sb = new StringBuilder();
      StreamWithUnget ungetStream = new StreamWithUnget(stream);
      while (true) {
        sb.Clear();
        bool first = true;
        bool endOfHeaders = false;
        bool wsp = false;
        lineCount = 0;
        while (true) {
          int c = ungetStream.ReadByte();
          if (c == -1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          ++lineCount;
          if (first && c == '\r') {
            if (ungetStream.ReadByte() =='\n') {
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
            if (c >= 'A' && c <= 'Z') {
              c += 0x20;
            }
            sb.Append((char)c);
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
        if (sb.Length == 0) {
          throw new InvalidDataException("Empty header field name");
        }
        string fieldName = sb.ToString();
        sb.Clear();
        // Read the header field value
        while (true) {
          int c = ungetStream.ReadByte();
          if (c == -1) {
            throw new InvalidDataException("Premature end before all headers were read");
          }
          if (c == '\r') {
            c = ungetStream.ReadByte();
            if (c == '\n') {
              lineCount = 0;
              // Parse obsolete folding whitespace (obs-fws) under RFC5322
              // (parsed according to errata), same as LWSP in RFC5234
              bool fwsFirst = true;
              bool haveFWS = false;
              while (true) {
                // Skip the CRLF pair, if any (except if iterating for
                // the first time, since CRLF was already parsed)
                if (!fwsFirst) {
                  c = ungetStream.ReadByte();
                  if (c == '\r') {
                    c = ungetStream.ReadByte();
                    if (c == '\n') {
                      // CRLF was read
                      sb.Append("\r\n");
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
                int c2 = ungetStream.ReadByte();
                if (c2 == 0x20 || c2 == 0x09) {
                  ++lineCount;
                  sb.Append((char)c2);
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
                // count found as above
                continue;
              }
              break;
            } else {
              sb.Append('\r');
              ungetStream.Unget();
              ++lineCount;
            }
          }
          if (lineCount > 998) {
            throw new InvalidDataException("Header field line too long");
          }
          if (c < 0x80) {
            sb.Append((char)c);
          } else {
            if (!HeaderFields.GetParser(fieldName).IsStructured()) {
              // DEVIATION: Some emails still have an unencoded subject line
              // or other unstructured header field
              sb.Append('\ufffd');
            } else {
              throw new InvalidDataException("Malformed header field value " + sb.ToString());
            }
          }
        }
        string fieldValue = sb.ToString();
        headerList.Add(fieldName);
        headerList.Add(fieldValue);
      }
    }

    private class MessageStackEntry {
      private Message message;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public Message Message {
        get {
          return this.message;
        }
      }

      private string boundary;

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public string Boundary {
        get {
          return this.boundary;
        }
      }

      public MessageStackEntry(Message msg) {
        #if DEBUG
        if (msg == null) {
          throw new ArgumentNullException("msg");
        }
        #endif

        this.message = msg;
        MediaType mediaType = msg.ContentType;
        if (mediaType.TopLevelType.Equals("multipart")) {
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
        this.ContentType.TypeAndSubType.Equals("text/plain"));
      IList<MessageStackEntry> multipartStack = new List<MessageStackEntry>();
      MessageStackEntry entry = new Message.MessageStackEntry(this);
      multipartStack.Add(entry);
      boundaryChecker.PushBoundary(entry.Boundary);
      Message leaf = null;
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.Length;
       using (MemoryStream ms = new MemoryStream()) {
        while (true) {
          int ch = 0;
          try {
            ch = currentTransform.ReadByte();
          } catch (InvalidDataException) {
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
buffer,
Math.Max(buffer.Length - 80, 0),
Math.Min(buffer.Length, 80),
true);
            Console.WriteLine(ss);
            throw;
          }
          if (ch < 0) {
            if (boundaryChecker.HasNewBodyPart) {
              Message msg = new Message();
              int stackCount = boundaryChecker.BoundaryCount();
              // Pop entries if needed to match the stack
              #if DEBUG
              if (multipartStack.Count < stackCount) {
                throw new ArgumentException("multipartStack.Count (" + Convert.ToString((long)multipartStack.Count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)stackCount, System.Globalization.CultureInfo.InvariantCulture));
              }
              #endif
              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.Write(buffer, 0, bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.ToArray();
              }
              while (multipartStack.Count > stackCount) {
                multipartStack.RemoveAt(stackCount);
              }
              Message parentMessage = multipartStack[multipartStack.Count - 1].Message;
              boundaryChecker.StartBodyPartHeaders();
              ReadHeaders(stream, msg.headers);
              bool parentIsDigest = parentMessage.ContentType.SubType.Equals("digest") &&
                parentMessage.ContentType.TopLevelType.Equals("multipart");
              msg.ProcessHeaders(true, parentIsDigest);
              entry = new MessageStackEntry(msg);
              // Add the body part to the multipart
              // message's list of parts
              parentMessage.Parts.Add(msg);
              multipartStack.Add(entry);
              ms.SetLength(0);
              if (msg.ContentType.TopLevelType.Equals("multipart")) {
                leaf = null;
              } else {
                leaf = msg;
              }
              boundaryChecker.PushBoundary(entry.Boundary);
              boundaryChecker.EndBodyPartHeaders();
              currentTransform = MakeTransferEncoding(
                boundaryChecker,
                msg.transferEncoding,
                msg.ContentType.TypeAndSubType.Equals("text/plain"));
            } else {
              // All body parts were read
              if (leaf != null) {
                if (bufferCount > 0) {
                  ms.Write(buffer, 0, bufferCount);
                  bufferCount = 0;
                }
                leaf.body = ms.ToArray();
              }
              return;
            }
          } else {
            buffer[bufferCount++] = (byte)ch;
            if (bufferCount >= bufferLength) {
              ms.Write(buffer, 0, bufferCount);
              bufferCount = 0;
            }
          }
        }
      }
    }

    private static ITransform MakeTransferEncoding(
ITransform stream,
int encoding,
bool plain) {
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
this.ContentType.TypeAndSubType.Equals("text/plain"));
      byte[] buffer = new byte[8192];
      int bufferCount = 0;
      int bufferLength = buffer.Length;
      // TODO: Support message/rfc822
       using (MemoryStream ms = new MemoryStream()) {
        while (true) {
          int ch = 0;
          try {
            ch = transform.ReadByte();
          } catch (InvalidDataException) {
            ms.Write(buffer, 0, bufferCount);
            buffer = ms.ToArray();
            string ss = DataUtilities.GetUtf8String(
buffer,
Math.Max(buffer.Length - 80, 0),
Math.Min(buffer.Length, 80),
true);
            Console.WriteLine(ss);
            throw;
          }
          if (ch < 0) {
            break;
          }
          buffer[bufferCount++] = (byte)ch;
          if (bufferCount >= bufferLength) {
            ms.Write(buffer, 0, bufferCount);
            bufferCount = 0;
          }
        }
        if (bufferCount > 0) {
          ms.Write(buffer, 0, bufferCount);
        }
        this.body = ms.ToArray();
      }
    }

    private void ReadMessage(ITransform stream) {
      ReadHeaders(stream, this.headers);
      this.ProcessHeaders(false, false);
      if (this.contentType.TopLevelType.Equals("multipart")) {
        this.ReadMultipartBody(stream);
      } else {
        if (this.contentType.TopLevelType.Equals("message")) {
          Console.WriteLine(this.contentType);
        }
        this.ReadSimpleBody(stream);
      }
    }
  }
}
