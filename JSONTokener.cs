// Modified by Peter O; originally based on the
// 2002 public domain
// code from json.org, to use generics and
// to use int and -1 as the terminating
// value rather than char and 0, among
// other things.
// Now much of this file has been rewritten and
// altered by Peter O. to support the CBOR project.
// Still in the public domain;
// public domain dedication: http://creativecommons.org/publicdomain/zero/1.0/
namespace PeterO {
  using System;
  using System.Globalization;
  using System.IO;
  using System.Text;
  using System.Collections.Generic;

  internal class JSONTokener {
    /**
     * Trailing commas are allowed in the JSON _string.
     */
    public const int OPTION_TRAILING_COMMAS = 8;
    /**
     * Empty array elements are allowed in array notation
     */
    public const int OPTION_EMPTY_ARRAY_ELEMENTS = 16;
    /**
     * Allow forward slashes to be escaped in strings.
     */
    public const int OPTION_ESCAPED_SLASHES = 64;
    /**
     * No duplicates are allowed in the JSON _string.
     */
    public const int OPTION_NO_DUPLICATES = 1;
    /**
     * Will parse Shell-style comments (beginning with "#").
     */
    public const int OPTION_SHELL_COMMENTS = 2;
    /**
     * Single quotes are allowed to delimit strings.
     */
    public const int OPTION_SINGLE_QUOTES = 32;
    /**
     * Allows comments in JSON texts.
     */
    public const int OPTION_ALLOW_COMMENTS = 128;
    /**
     * Get the hex value of a character (base16).
     * @param c A character between '0' and '9' or between 'A' and 'F' or
     * between 'a' and 'f'.
     * @return  An int between 0 and 15, or -1 if c was not a hex digit.
     */
    private static int dehexchar(int c) {
      if (c >= '0' && c <= '9') {
 return c - '0';
}
      if (c >= 'A' && c <= 'F') {
 return c + 10 - 'A';
}
      if (c >= 'a' && c <= 'f') {
 return c + 10 - 'a';
}
      return -1;
    }
    /**
     * The index of the next character.
     */
    private int myIndex;
    /**
     * The source string being tokenized.
     */
    private string mySource;
    private Stream stream;
    private int options;
    /**
     * Construct a JSONTokener from a string.
     *
     * @param s     A source _string.
     */
    public JSONTokener(string str, int options) {
      if (str == null) {
 throw new ArgumentNullException("str");
}
      this.mySource = str;
      this.options = options;
    }

    public JSONTokener(Stream stream, int options) {
      if (stream == null) {
 throw new ArgumentNullException("stream");
}
      this.stream = stream;
      this.options = options;
    }

    private int NextChar() {
      if (this.stream != null) {
        int cp = 0;
        int bytesSeen = 0;
        int bytesNeeded = 0;
        int lower = 0;
        int upper = 0;
        try {
          while (true) {
            int b = this.stream.ReadByte();
            if (b < 0) {
              if (bytesNeeded != 0) {
                bytesNeeded = 0;
                throw this.syntaxError("Invalid UTF-8");
              } else {
                return -1;
              }
            }
            if (bytesNeeded == 0) {
              if ((b & 0x7F) == b) {
                this.myIndex += 1;
                return b;
              } else if (b >= 0xc2 && b <= 0xdf) {
                bytesNeeded = 1;
                lower = 0x80;
                upper = 0xbf;
                cp = (b - 0xc0) << 6;
              } else if (b >= 0xe0 && b <= 0xef) {
                lower = (b == 0xe0) ? 0xa0 : 0x80;
                upper = (b == 0xed) ? 0x9f : 0xbf;
                bytesNeeded = 2;
                cp = (b - 0xe0) << 12;
              } else if (b >= 0xf0 && b <= 0xf4) {
                lower = (b == 0xf0) ? 0x90 : 0x80;
                upper = (b == 0xf4) ? 0x8f : 0xbf;
                bytesNeeded = 3;
                cp = (b - 0xf0) << 18;
              } else {
                throw this.syntaxError("Invalid UTF-8");
              }
              continue;
            }
            if (b < lower || b > upper) {
              cp = bytesNeeded = bytesSeen = 0;
              lower = 0x80;
              upper = 0xbf;
              throw this.syntaxError("Invalid UTF-8");
            }
            lower = 0x80;
            upper = 0xbf;
            bytesSeen++;
            cp += (b - 0x80) << (6 * (bytesNeeded - bytesSeen));
            if (bytesSeen != bytesNeeded) {
              continue;
            }
            int ret = cp;
            cp = 0;
            bytesSeen = 0;
            bytesNeeded = 0;
            this.myIndex += 1;
            return ret;
          }
        }  catch (IOException ex) {
          throw this.syntaxError("I/O error occurred", ex);
        }
      } else {
        int c = (this.myIndex < this.mySource.Length) ? this.mySource[this.myIndex] : -1;
        this.myIndex += 1;
        return c;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int GetOptions() {
      return this.options;
    }

    private int NextParseComment(int firstChar) {
      if ((this.options & JSONTokener.OPTION_ALLOW_COMMENTS) == 0) {
        if (firstChar == -1) {
 return this.NextChar();
}
        if (firstChar == '/' || firstChar == '#') {
 throw this.syntaxError("Comments not allowed");
}
        return firstChar;
      }
      bool first = true;
      while (true) {
        int c;
        if (first && firstChar >= 0) {
          c = firstChar;
        } else {
          c = this.NextChar();
        }
        first = false;
        if (c == '#' && (this.options & JSONTokener.OPTION_SHELL_COMMENTS) != 0) {
          // Shell-style single-line comment
          while (true) {
            c = this.NextChar();
            if (c != '\n' && c != -1) {
            } else {
 break;
}  // end of line
          }
        } else if (c == '/') {
          c = this.NextChar();
          switch (c) {
              case '/': { // single-line comment
                while (true) {
                  c = this.NextChar();
                  if (c != '\n' && c != -1) {
                  } else {
 break;
}  // end of line
                }
                break;
              }
              case '*': { // multi-line comment
                while (true) {
                  c = this.NextChar();
                  if (c == -1) {
 throw this.syntaxError("Unclosed comment.");
}
                  // use a while loop to deal with
                  // the case of multiple "*" followed by "/"
                  bool endOfComment = false;
                  while (c == '*') {
                    c = this.NextChar();
                    if (c == '/') {
                      endOfComment = true;
                      break;
                    }
                  }
                  if (endOfComment) {
 break;
}
                }
                break;
              }
            default:
              return c;
          }
        } else if (c == -1) {
          return c;  // reached end of string
        } else if (c > ' ') {
          return c;  // reached an ordinary character
        }
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A 32-bit signed integer.</returns>
    public int NextClean() {
      while (true) {
        int c = this.NextParseComment(-1);
        if (c == -1 || c > ' ') {
 return c;
}
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='lastChar'>A 32-bit signed integer. (2).</param>
    /// <returns>A 32-bit signed integer.</returns>
    public int NextClean(int lastChar) {
      while (true) {
        int c = this.NextParseComment(lastChar);
        if (c == -1 || c > ' ') {
 return c;
}
        lastChar = -1;
      }
    }
    /**
     * Return the characters up to the next close quote character.
     * Backslash processing is done. The formal JSON format does not
     * allow strings in single quotes, but an implementation is allowed to
     * accept them.
     * @param quote The quoting character, either <code>"</code>&#xa0;
     * <small>(double quote)</small> or <code>'</code>&#xa0;<small>(single quote)</small>.
     * @return      A string.
     * @exception FormatException Unterminated _string.
     */
    private string NextString(int quote) {
      int c;
      StringBuilder sb = new StringBuilder();
      bool surrogate = false;
      bool surrogateEscaped = false;
      bool escaped = false;
      while (true) {
        c = this.NextChar();
        if (c == -1 || c < 0x20) {
 throw this.syntaxError("Unterminated string");
}
        switch (c) {
          case '\\':
            c = this.NextChar();
            escaped = true;
            switch (c) {
              case '\\':
                c = '\\';
                break;
              case '/':
                if ((this.options & JSONTokener.OPTION_ESCAPED_SLASHES) != 0) {
                  // For compatibility (some JSON texts
                  // encode dates with an escaped slash),
                  // even though this is not allowed by RFC 4627
                  c = '/';
                } else {
                  throw this.syntaxError("Invalid escaped character");
                }
                break;
              case '\"':
                c = '\"';
                break;
              case 'b':
                c = '\b';
                break;
              case 't':
                c = '\t';
                break;
              case 'n':
                c = '\n';
                break;
              case 'f':
                c = '\f';
                break;
              case 'r':
                c = '\r';
                break;
                case 'u': { // Unicode escape
                  int c1 = dehexchar(this.NextChar());
                  int c2 = dehexchar(this.NextChar());
                  int c3 = dehexchar(this.NextChar());
                  int c4 = dehexchar(this.NextChar());
                  if (c1 < 0 || c2 < 0 || c3 < 0 || c4 < 0) {
 throw this.syntaxError("Invalid Unicode escaped character");
}
                  c = c4 | (c3 << 4) | (c2 << 8) | (c1 << 12);
                  break;
                }
              default:
                throw this.syntaxError("Invalid escaped character");
            }
            break;
          default:
            escaped = false;
            break;
        }
        if (surrogate) {
          if ((c & 0x1FFC00) != 0xDC00) {
            // Note: this includes the ending quote
            // and supplementary characters
            throw this.syntaxError("Unpaired surrogate code point");
          }
          if (escaped != surrogateEscaped) {
            throw this.syntaxError("Pairing escaped surrogate with unescaped surrogate");
          }
          surrogate = false;
        } else if ((c & 0x1FFC00) == 0xD800) {
          surrogate = true;
          surrogateEscaped = escaped;
        } else if ((c & 0x1FFC00) == 0xDC00) {
          throw this.syntaxError("Unpaired surrogate code point");
        }
        if (c == quote && !escaped)  // End quote reached
          return sb.ToString();
        if (c <= 0xFFFF) {
  { sb.Append((char)c);
}
  } else if (c <= 0x10FFFF) {
          sb.Append((char)((((c - 0x10000) >> 10) & 0x3FF) + 0xD800));
          sb.Append((char)(((c - 0x10000) & 0x3FF) + 0xDC00));
        }
      }
    }

    internal CBORException syntaxError(string message) {
      return new CBORException(message + this.ToString());
    }

    internal CBORException syntaxError(string message, Exception innerException) {
      return new CBORException(message + this.ToString(), innerException);
    }
    /**
     * Make a printable string of this JSONTokener.
     *
     * @return " at character [myIndex] of [mySource]"
     */
    public override string ToString() {
      if (this.mySource == null) {
        return " at character " + this.myIndex;
      } else {
        return " at character " + this.myIndex + " of " + this.mySource;
      }
    }

    private CBORObject NextJSONString(int firstChar) {
      int c = firstChar;
      if (c < 0) {
 throw this.syntaxError("Unexpected end of data");
}
      // Parse a string
      if (c == '"' || (c == '\'' && ((this.GetOptions() & JSONTokener.OPTION_SINGLE_QUOTES) != 0))) {
        // The tokenizer already checked the string for invalid
        // surrogate pairs, so just call the CBORObject
        // constructor directly
        return CBORObject.FromRaw(this.NextString(c));
      }
      throw this.syntaxError("Expected a string as a key");
    }

    // Based on the json.org implementation for JSONTokener,
    // now mostly rewritten
    private CBORObject NextJSONValue(int firstChar, int[] nextChar) {
      string str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
 throw this.syntaxError("Unexpected end of data");
}
      if (c == '"' || (c == '\'' && ((this.GetOptions() & JSONTokener.OPTION_SINGLE_QUOTES) != 0))) {
        // Parse a string
        // The tokenizer already checked the string for invalid
        // surrogate pairs, so just call the CBORObject
        // constructor directly
        obj = CBORObject.FromRaw(this.NextString(c));
        nextChar[0] = this.NextClean();
        return obj;
      } else if (c == '{') {
        // Parse an object
        obj = this.ParseJSONObject();
        nextChar[0] = this.NextClean();
        return obj;
      } else if (c == '[') {
        // Parse an array
        obj = this.ParseJSONArray();
        nextChar[0] = this.NextClean();
        return obj;
      } else if (c == 't') {
        // Parse true
        if (this.NextChar() != 'r' ||
           this.NextChar() != 'u' ||
           this.NextChar() != 'e') {
          throw this.syntaxError("Value can't be parsed.");
        }
        nextChar[0] = this.NextClean();
        return CBORObject.True;
      } else if (c == 'f') {
        // Parse false
        if (this.NextChar() != 'a' ||
           this.NextChar() != 'l' ||
           this.NextChar() != 's' ||
           this.NextChar() != 'e') {
          throw this.syntaxError("Value can't be parsed.");
        }
        nextChar[0] = this.NextClean();
        return CBORObject.False;
      } else if (c == 'n') {
        // Parse null
        if (this.NextChar() != 'u' ||
           this.NextChar() != 'l' ||
           this.NextChar() != 'l') {
          throw this.syntaxError("Value can't be parsed.");
        }
        nextChar[0] = this.NextClean();
        return CBORObject.False;
      } else if (c == '-' || (c >= '0' && c <= '9')) {
        // Parse a number
        StringBuilder sb = new StringBuilder();
        while (c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E' || (c >= '0' && c <= '9')) {
          sb.Append((char)c);
          c = this.NextChar();
        }
        str = sb.ToString();
        obj = CBORDataUtilities.ParseJSONNumber(str);
        if (obj == null) {
 throw this.syntaxError("JSON number can't be parsed.");
}
        nextChar[0] = this.NextClean(c);
        return obj;
      } else {
        throw this.syntaxError("Value can't be parsed.");
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <returns>A CBORObject object.</returns>
    public CBORObject ParseJSONObjectOrArray() {
      int c;
      c = this.NextClean();
      if (c == '[') {
        return this.ParseJSONArray();
      }
      if (c == '{') {
        return this.ParseJSONObject();
      }
      throw this.syntaxError("A JSON object must begin with '{' or '['");
    }
    // Based on the json.org implementation for JSONObject
    private CBORObject ParseJSONObject() {
      // Assumes that the last character read was '{'
      int c;
      CBORObject key;
      CBORObject obj;
      int[] nextchar = new int[1];
      bool seenComma = false;
      var myHashMap = new Dictionary<CBORObject, CBORObject>();
      while (true) {
        c = this.NextClean();
        switch (c) {
          case -1:
            throw this.syntaxError("A JSONObject must end with '}'");
          case '}':
            if (seenComma &&
                (this.GetOptions() & JSONTokener.OPTION_TRAILING_COMMAS) == 0) {
              // 2013-05-24 -- Peter O. Disallow trailing comma.
              throw this.syntaxError("Trailing comma");
            }
            return CBORObject.FromRaw(myHashMap);
          default:
            obj = this.NextJSONString(c);
            key = obj;
            if ((this.GetOptions() & JSONTokener.OPTION_NO_DUPLICATES) != 0 &&
                myHashMap.ContainsKey(obj)) {
              throw this.syntaxError("Key already exists: " + key);
            }
            break;
        }
        if (this.NextClean() != ':') {
 throw this.syntaxError("Expected a ':' after a key");
}
        // NOTE: Will overwrite existing value. --Peter O.
        myHashMap[key] = this.NextJSONValue(this.NextClean(), nextchar);
        switch (nextchar[0]) {
          case ',':
            seenComma = true;
            break;
          case '}':
            return CBORObject.FromRaw(myHashMap);
          default:
            throw this.syntaxError("Expected a ',' or '}'");
        }
      }
    }
    // Based on the json.org implementation for JSONArray
    private CBORObject ParseJSONArray() {
      var myArrayList = new List<CBORObject>();
      bool seenComma = false;
      int[] nextchar = new int[1];
      // This method assumes that the last character read was '['
      while (true) {
        int c = this.NextClean();
        if (c == ',') {
          if ((this.GetOptions() & JSONTokener.OPTION_EMPTY_ARRAY_ELEMENTS) == 0) {
            throw this.syntaxError("Two commas one after the other");
          }
          myArrayList.Add(CBORObject.Null);
          c = ',';  // Reuse the comma in the code that follows
        } else if (c == ']') {
          if (seenComma && (this.GetOptions() & JSONTokener.OPTION_TRAILING_COMMAS) == 0) {
            // 2013-05-24 -- Peter O. Disallow trailing comma.
            throw this.syntaxError("Trailing comma");
          }
          return CBORObject.FromRaw(myArrayList);
        } else {
          myArrayList.Add(this.NextJSONValue(c, nextchar));
          c = nextchar[0];
        }
        switch (c) {
          case ',':
            seenComma = true;
            break;
          case ']':
            return CBORObject.FromRaw(myArrayList);
          default:
            throw this.syntaxError("Expected a ',' or ']'");
        }
      }
    }
  }
}
