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
using System.IO;
using System.Text;

namespace PeterO.Mail
{
    /// <summary>Represents an email message. <para><b>Thread safety:</b>
    /// This class is mutable; its properties can be changed. None of its methods
    /// are designed to be thread safe. Therefore, access to objects from
    /// this class must be synchronized if multiple threads can access them
    /// at the same time.</para>
    /// </summary>
  internal sealed class Message {
    private const int EncodingSevenBit = 0;
    private const int EncodingUnknown = -1;
    private const int EncodingEightBit = 3;
    private const int EncodingBinary = 4;
    private const int EncodingQuotedPrintable = 1;
    private const int EncodingBase64 = 2;

    private IList<string> headers;

    private IList<Message> parts;

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
      // TODO: Add "Content-Type" header to headers
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
        if (index + 1 < endIndex && str[index] == '=' && str[index + 1] == '?') {
          startIndex = index + 2;
          index += 2;
          havePossibleEncodedWord = true;
        } else if (inComments && index + 2 < endIndex && str[index] == '(' &&
                   str[index + 1] == '=' && str[index + 2] == '?') {
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
            if (c >= 0x21 && c < 0x7e && (!inComments || (c != '(' && c != ')'))) {
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
            if (i2 != index && i2 < endIndex && str[i2] == '?') {
              // Parse encoding
              charsetEnd = i2;
              index = i2 + 1;
              i2 = MediaType.skipMimeTokenRfc2047(str, index, afterLast);
              if (i2 != index && i2 < endIndex && str[i2] == '?') {
                // check for supported encoding (B or Q)
                char encodingChar = str[index];
                if (i2 - index == 1 && (encodingChar == 'b' || encodingChar == 'B' ||
                                        encodingChar == 'q' || encodingChar == 'Q')) {
                  // Parse encoded text
                  base64 = encodingChar == 'b' || encodingChar == 'B';
                  index = i2 + 1;
                  encodedTextStart = index;
                  i2 = MediaType.skipEncodedTextRfc2047(str, index, afterLast, inComments);
                  if (i2 != index && i2 + 1 < endIndex && str[i2] == '?' && str[i2 + 1] == '=' &&
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
                (afterLast - 2) - encodedTextStart);
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
                  decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex - 2));
                } else {
                  // Console.WriteLine("Encoded " + (base64 ? "B" : "Q") + " to: " + (encoding.GetString(transform)));
                  decodedWord = encoding.GetString(transform);
                }
                // TODO: decodedWord may itself be part of an encoded word
                // or contain ASCII control characters: encoded word decoding is
                // not idempotent; if this is a comment it could also contain '(', ')', and '\'
              } else {
                decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex - 2));
              }
            } else {
              decodedWord = str.Substring(startIndex - 2, afterLast - (startIndex - 2));
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
      for (int i = 0; i < this.headers.Count; i += 2) {
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
      for (int i = 0; i < this.headers.Count; i += 2) {
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
      for (int i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (i == str.Length - 1 && c == 0x20) {
          // Space not allowed at the end of a boundary
          return false;
        }
        if (!(
          (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
          c == 0x20 || c == 0x2c || "'()-./+_:=?".IndexOf(c) >= 0)) {
          return false;
        }
      }
      return true;
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='name'>A string object. (2).</param>
    /// <returns>A string object.</returns>
    public string GetHeader(string name) {
      name = ParserUtility.ToLowerCaseAscii(name);
      for (int i = 0; i < this.headers.Count; i += 2) {
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
        if (c == '=' && i + 1 < len && c == '?') {
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
      if (length < 2 || str[index] != '(' || str[endIndex - 1] != ')') {
        return str.Substring(index, length);
      }
      EncodedWordEncoder encoder;
      int nextComment = str.IndexOf('(', index + 1);
      int nextBackslash = str.IndexOf('\\', index + 1);
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
        nextBackslash = str.IndexOf('\n', index + 1);
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
        encoder = new EncodedWordEncoder(String.Empty);
        encoder.AddPrefix("(");
        encoder.AddString(str, index + 1, length - 2);
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      if (nextBackslash < 0) {
        // No escapes; just look for '(' and ')'
        encoder = new EncodedWordEncoder(String.Empty);
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
          } else if (str[index] == '\r' && index + 2 < endIndex &&
                     str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index + 2] == 0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
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
        encoder = new EncodedWordEncoder(String.Empty);
        encoder.AddPrefix("(");
        encoder.AddString(builder.ToString());
        encoder.FinalizeEncoding(")");
        return encoder.ToString();
      }
      // escapes and nested comments
      encoder = new EncodedWordEncoder(String.Empty);
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
          } else if (str[index] == '\r' && index + 2 < endIndex &&
                     str[index + 1] == '\n' && (str[index + 2] == 0x20 || str[index + 2] == 0x09)) {
            // Folding whitespace
            builder.Append(str[index + 2]);
            index += 3;
          } else if (str[index] == '\\' && index + 1 < endIndex) {
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
            if (i + 1 >= this.body.Length || this.body[i + 1] != (byte)'\n') {
              // bare CR
              allTextBytes = false;
            } else if (i > 0 && (this.body[i - 1] == (byte)' ' || this.body[i - 1] == (byte)'\t')) {
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

    /// <summary>Not documented yet.</summary>
    /// <returns>A string object.</returns>
    public string GenerateHeaders() {
      StringBuilder sb = new StringBuilder();
      bool haveMimeVersion = false;
      for (int i = 0; i < this.headers.Count; i += 2) {
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
        string boundary = "=_" + Convert.ToString((int)index, System.Globalization.CultureInfo.CurrentCulture);
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
      BoundaryCheckerTransform boundaryChecker = new BoundaryCheckerTransform(stream);
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
        this.transferEncoding,
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
