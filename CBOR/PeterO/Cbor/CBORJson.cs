/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/
 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal sealed class CBORJson {
    // JSON parsing methods
    private static int SkipWhitespaceJSON(CharacterInputWithCount reader) {
      while (true) {
        int c = reader.ReadChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    private void RaiseError(string str) {
      this.reader.RaiseError(str);
    }

    private CharacterInputWithCount reader;
    private StringBuilder sb;
    private readonly JSONOptions options;

    private string NextJSONString() {
      int c;
      this.sb = this.sb ?? new StringBuilder();
      this.sb.Remove(0, this.sb.Length);
      while (true) {
        c = this.reader.ReadChar();
        if (c == -1 || c < 0x20) {
          this.RaiseError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = this.reader.ReadChar();
            switch (c) {
              case '\\':
              case '/':
              case '\"':
                // Slash is now allowed to be escaped under RFC 8259
                this.sb.Append((char)c);
                break;
              case 'b':
                this.sb.Append('\b');
                break;
              case 'f':
                this.sb.Append('\f');
                break;
              case 'n':
                this.sb.Append('\n');
                break;
              case 'r':
                this.sb.Append('\r');
                break;
              case 't':
                this.sb.Append('\t');
                break;
              case 'u': { // Unicode escape
                c = 0;
                // Consists of 4 hex digits
                for (var i = 0; i < 4; ++i) {
                  int ch = this.reader.ReadChar();
                  if (ch >= '0' && ch <= '9') {
                    c <<= 4;
                    c |= ch - '0';
                  } else if (ch >= 'A' && ch <= 'F') {
                    c <<= 4;
                    c |= ch + 10 - 'A';
                  } else if (ch >= 'a' && ch <= 'f') {
                    c <<= 4;
                    c |= ch + 10 - 'a';
                  } else {
                    this.RaiseError(
                      "Invalid Unicode escaped character");
                  }
                }
                if ((c & 0xf800) != 0xd800) {
                  // Non-surrogate
                  this.sb.Append((char)c);
                } else if ((c & 0xfc00) == 0xd800) {
                  int ch = this.reader.ReadChar();
                  if (ch != '\\' || this.reader.ReadChar() != 'u') {
                    this.RaiseError("Invalid escaped character");
                  }
                  var c2 = 0;
                  for (var i = 0; i < 4; ++i) {
                    ch = this.reader.ReadChar();
                    if (ch >= '0' && ch <= '9') {
                      c2 <<= 4;
                      c2 |= ch - '0';
                    } else if (ch >= 'A' && ch <= 'F') {
                      c2 <<= 4;
                      c2 |= ch + 10 - 'A';
                    } else if (ch >= 'a' && ch <= 'f') {
                      c2 <<= 4;
                      c2 |= ch + 10 - 'a';
                    } else {
                      this.RaiseError(
                        "Invalid Unicode escaped character");
                    }
                  }
                  if ((c2 & 0xfc00) != 0xdc00) {
                    this.RaiseError("Unpaired surrogate code point");
                  } else {
                    this.sb.Append((char)c);
                    this.sb.Append((char)c2);
                  }
                } else {
                  this.RaiseError("Unpaired surrogate code point");
                }
                break;
              }
              default: {
                this.RaiseError("Invalid escaped character");
                break;
              }
            }
            break;
          case 0x22: // double quote
            return this.sb.ToString();
          default: {
            // NOTE: Assumes the character reader
            // throws an error on finding illegal surrogate
            // pairs in the string or invalid encoding
            // in the stream
            if ((c >> 16) == 0) {
              this.sb.Append((char)c);
            } else {
              this.sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) |
                  0xd800));
              this.sb.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
            }
            break;
          }
        }
      }
    }

    private CBORObject NextJSONNegativeNumber(
      int[] nextChar) {
          string str;
          CBORObject obj;
          int c = this.reader.ReadChar();
          if (c < '0' || c > '9') {
            this.RaiseError("JSON number can't be parsed.");
          }
          int cval = -(c - '0');
          int cstart = c;
          c = this.reader.ReadChar();
          this.sb = this.sb ?? new StringBuilder();
          this.sb.Remove(0, this.sb.Length);
          this.sb.Append('-');
          this.sb.Append((char)cstart);
          while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
            c == 'e' || c == 'E') {
            this.sb.Append((char)c);
            c = this.reader.ReadChar();
          }
          // check if character can validly appear after a JSON number
          if (c != ',' && c != ']' && c != '}' && c != -1 &&
            c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
            this.RaiseError("Invalid character after JSON number");
          }
          str = this.sb.ToString();
          obj = CBORDataUtilities.ParseJSONNumber(str, this.options);
          if (obj == null) {
            string errstr = (str.Length <= 100) ? str : (str.Substring(0,
                  100) + "...");
            this.RaiseError("JSON number can't be parsed. " + errstr);
          }
          if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
            nextChar[0] = c;
          } else {
            nextChar[0] = SkipWhitespaceJSON(this.reader);
          }
          return obj;
    }

    private CBORObject NextJSONValue(
      int firstChar,
      int[] nextChar,
      int depth) {
      string str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        this.RaiseError("Unexpected end of data");
      }
      switch (c) {
        case '"': {
          // Parse a string
          // The tokenizer already checked the string for invalid
          // surrogate pairs, so just call the CBORObject
          // constructor directly
          obj = CBORObject.FromRaw(this.NextJSONString());
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return obj;
        }
        case '{': {
          // Parse an object
          obj = this.ParseJSONObject(depth + 1);
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return obj;
        }
        case '[': {
          // Parse an array
          obj = this.ParseJSONArray(depth + 1);
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return obj;
        }
        case 't': {
          // Parse true
          if (this.reader.ReadChar() != 'r' || this.reader.ReadChar() != 'u' ||
            this.reader.ReadChar() != 'e') {
            this.RaiseError("Value can't be parsed.");
          }
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return CBORObject.True;
        }
        case 'f': {
          // Parse false
          if (this.reader.ReadChar() != 'a' || this.reader.ReadChar() != 'l' ||
            this.reader.ReadChar() != 's' || this.reader.ReadChar() != 'e') {
            this.RaiseError("Value can't be parsed.");
          }
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return CBORObject.False;
        }
        case 'n': {
          // Parse null
          if (this.reader.ReadChar() != 'u' || this.reader.ReadChar() != 'l' ||
            this.reader.ReadChar() != 'l') {
            this.RaiseError("Value can't be parsed.");
          }
          nextChar[0] = SkipWhitespaceJSON(this.reader);
          return CBORObject.Null;
        }
        case '-': {
          // Parse a negative number
          return this.NextJSONNegativeNumber(nextChar);
        }
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9': {
          // Parse a nonnegative number
          int cval = c - '0';
          int cstart = c;
          var needObj = true;
          c = this.reader.ReadChar();
          if (!(c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
              c == 'e' || c == 'E')) {
            // Optimize for common case where JSON number
            // is a single digit without sign or exponent
            obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
            needObj = false;
          } else if (c >= '0' && c <= '9') {
            int csecond = c;
            if (cstart == '0') {
              // Leading zero followed by any digit is not allowed
              this.RaiseError("JSON number can't be parsed.");
            }
            cval = (cval * 10) + (int)(c - '0');
            c = this.reader.ReadChar();
            if (c >= '0' && c <= '9') {
              var digits = 2;
              var ctmp = new int[10];
              ctmp[0] = cstart;
              ctmp[1] = csecond;
              while (digits < 9 && (c >= '0' && c <= '9')) {
                cval = (cval * 10) + (int)(c - '0');
                ctmp[digits++] = c;
                c = this.reader.ReadChar();
              }
              if (c == 'e' || c == 'E' || c == '.' || (c >= '0' && c <= '9')) {
                // Not an all-digit number, or too long
                this.sb = this.sb ?? new StringBuilder();
                this.sb.Remove(0, this.sb.Length);
                for (var vi = 0; vi < digits; ++vi) {
                  this.sb.Append((char)ctmp[vi]);
                }
              } else {
                obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
                needObj = false;
              }
            } else if (!(c == '-' || c == '+' || c == '.' || c == 'e' || c
                == 'E')) {
              // Optimize for common case where JSON number
              // is two digits without sign, decimal point, or exponent
              obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
              needObj = false;
            } else {
              this.sb = this.sb ?? new StringBuilder();
              this.sb.Remove(0, this.sb.Length);
              this.sb.Append((char)cstart);
              this.sb.Append((char)csecond);
            }
          } else {
            this.sb = this.sb ?? new StringBuilder();
            this.sb.Remove(0, this.sb.Length);
            this.sb.Append((char)cstart);
          }
          if (needObj) {
            while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
              c == 'e' || c == 'E') {
              this.sb.Append((char)c);
              c = this.reader.ReadChar();
            }
            // check if character can validly appear after a JSON number
            if (c != ',' && c != ']' && c != '}' && c != -1 &&
              c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
              this.RaiseError("Invalid character after JSON number");
            }
            str = this.sb.ToString();
            obj = CBORDataUtilities.ParseJSONNumber(str, this.options);
            if (obj == null) {
              string errstr = (str.Length <= 100) ? str : (str.Substring(0,
                    100) + "...");
              this.RaiseError("JSON number can't be parsed. " + errstr);
            }
          }
          if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
            nextChar[0] = c;
          } else {
            nextChar[0] = SkipWhitespaceJSON(this.reader);
          }
          return obj;
        }
        default:
          this.RaiseError("Value can't be parsed.");
          break;
      }
      return null;
    }

    public CBORJson(CharacterInputWithCount reader, JSONOptions options) {
      this.reader = reader;
      this.sb = null;
      this.options = options;
    }

    public CBORObject ParseJSON(bool objectOrArrayOnly, int[] nextchar) {
      int c;
      CBORObject ret;
      c = SkipWhitespaceJSON(this.reader);
      if (c == '[') {
        ret = this.ParseJSONArray(0);
        nextchar[0] = SkipWhitespaceJSON(this.reader);
        return ret;
      }
      if (c == '{') {
        ret = this.ParseJSONObject(0);
        nextchar[0] = SkipWhitespaceJSON(this.reader);
        return ret;
      }
      if (objectOrArrayOnly) {
        this.RaiseError("A JSON object must begin with '{' or '['");
      }
      return this.NextJSONValue(c, nextchar, 0);
    }

    internal static CBORObject ParseJSONValue(
      CharacterInputWithCount reader,
      JSONOptions options,
      bool objectOrArrayOnly,
      int[] nextchar) {
      var cj = new CBORJson(reader, options);
      return cj.ParseJSON(objectOrArrayOnly, nextchar);
    }

    private CBORObject ParseJSONObject(int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        this.RaiseError("Too deeply nested");
      }
      int c;
      CBORObject key = null;
      CBORObject obj;
      var nextchar = new int[1];
      var seenComma = false;
      var myHashMap = new Dictionary<CBORObject, CBORObject>();
      while (true) {
        c = SkipWhitespaceJSON(this.reader);
        switch (c) {
          case -1:
            this.RaiseError("A JSON object must end with '}'");
            break;
          case '}':
            if (seenComma) {
              // Situation like '{"0"=>1,}'
              this.RaiseError("Trailing comma");
              return null;
            }
            return CBORObject.FromRaw(myHashMap);
          default: {
            // Read the next string
            if (c < 0) {
              this.RaiseError("Unexpected end of data");
              return null;
            }
            if (c != '"') {
              this.RaiseError("Expected a string as a key");
              return null;
            }
            // Parse a string that represents the object's key
            // The tokenizer already checked the string for invalid
            // surrogate pairs, so just call the CBORObject
            // constructor directly
            obj = CBORObject.FromRaw(this.NextJSONString());
            key = obj;
            if (!this.options.AllowDuplicateKeys &&
              myHashMap.ContainsKey(obj)) {
              this.RaiseError("Key already exists: " + key);
              return null;
            }
            break;
          }
        }
        if (SkipWhitespaceJSON(this.reader) != ':') {
          this.RaiseError("Expected a ':' after a key");
        }
        // NOTE: Will overwrite existing value
        myHashMap[key] = this.NextJSONValue(
            SkipWhitespaceJSON(this.reader),
            nextchar,
            depth);
        switch (nextchar[0]) {
          case ',':
            seenComma = true;
            break;
          case '}':
            return CBORObject.FromRaw(myHashMap);
          default: this.RaiseError("Expected a ',' or '}'");
            break;
        }
      }
    }

    internal CBORObject ParseJSONArray(int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        this.RaiseError("Too deeply nested");
      }
      var myArrayList = new List<CBORObject>();
      var seenComma = false;
      var nextchar = new int[1];
      while (true) {
        int c = SkipWhitespaceJSON(this.reader);
        if (c == ']') {
          if (seenComma) {
            // Situation like '[0,1,]'
            this.RaiseError("Trailing comma");
          }
          return CBORObject.FromRaw(myArrayList);
        }
        if (c == ',') {
          // Situation like '[,0,1,2]' or '[0,,1]'
          this.RaiseError("Empty array element");
        }
        myArrayList.Add(
          this.NextJSONValue(
            c,
            nextchar,
            depth));
        c = nextchar[0];
        switch (c) {
          case ',':
            seenComma = true;
            break;
          case ']':
            return CBORObject.FromRaw(myArrayList);
          default: this.RaiseError("Expected a ',' or ']'");
            break;
        }
      }
    }
  }
}
