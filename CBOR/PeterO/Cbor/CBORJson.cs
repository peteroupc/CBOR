/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
  internal sealed class CBORJson {
    // JSON parsing methods
    private int SkipWhitespaceJSON() {
      while (true) {
        int c = this.ReadChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    // JSON parsing methods
    private int SkipWhitespaceJSON(int lastChar) {
      while (lastChar == 0x20 || lastChar == 0x0a || lastChar == 0x0d ||
        lastChar == 0x09) {
        lastChar = this.ReadChar();
      }
      return lastChar;
    }

    public void SkipToEnd() {
      if (this.jsonSequenceMode) {
        while (this.ReadChar() >= 0) {
          // Loop
        }
      }
    }

    public int ReadChar() {
      if (this.jsonSequenceMode) {
        if (this.recordSeparatorSeen) {
          return -1;
        }
        int rc = this.reader.ReadChar();
        if (rc == 0x1e) {
          this.recordSeparatorSeen = true;
          return -1;
        }
        return rc;
      } else {
        return this.reader.ReadChar();
      }
    }

    private void RaiseError(string str) {
      this.reader.RaiseError(str);
    }

    private readonly JSONOptions options;
    private readonly CharacterInputWithCount reader;
    private StringBuilder sb;
    private bool jsonSequenceMode;
    private bool recordSeparatorSeen;

    private string NextJSONString() {
      int c;
      this.sb = this.sb ?? new StringBuilder();
      _ = this.sb.Remove(0, this.sb.Length);
      while (true) {
        c = this.ReadChar();
        if (c == -1 || c < 0x20) {
          this.RaiseError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = this.ReadChar();
            switch (c) {
              case '\\':
              case '/':
              case '\"':
                // Slash is now allowed to be escaped under RFC 8259
                _ = this.sb.Append((char)c);
                break;
              case 'b':
                _ = this.sb.Append('\b');
                break;
              case 'f':
                _ = this.sb.Append('\f');
                break;
              case 'n':
                _ = this.sb.Append('\n');
                break;
              case 'r':
                _ = this.sb.Append('\r');
                break;
              case 't':
                _ = this.sb.Append('\t');
                break;
              case 'u':
                { // Unicode escape
                  c = 0;
                  // Consists of 4 hex digits
                  for (int i = 0; i < 4; ++i) {
                    int ch = this.ReadChar();
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
                    _ = this.sb.Append((char)c);
                  } else if ((c & 0xfc00) == 0xd800) {
                    int ch = this.ReadChar();
                    if (ch != '\\' || this.ReadChar() != 'u') {
                      this.RaiseError("Invalid escaped character");
                    }
                    var c2 = 0;
                    for (int i = 0; i < 4; ++i) {
                      ch = this.ReadChar();
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
                      _ = this.sb.Append((char)c);
                      _ = this.sb.Append((char)c2);
                    }
                  } else {
                    this.RaiseError("Unpaired surrogate code point");
                  }
                  break;
                }
              default:
                {
                  this.RaiseError("Invalid escaped character");
                  break;
                }
            }
            break;
          case 0x22: // double quote
            return this.sb.ToString();
          default:
            {
              // NOTE: Assumes the character reader
              // throws an error on finding illegal surrogate
              // pairs in the string or invalid encoding
              // in the stream
              if ((c >> 16) == 0) {
                _ = this.sb.Append((char)c);
              } else {
                _ = this.sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) |
                    0xd800));
                _ = this.sb.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
              }
              break;
            }
        }
      }
    }

    private CBORObject NextJSONNegativeNumber(
      int[] nextChar,
      int depth) {
      string str;
      CBORObject obj;
      int c = this.ReadChar();
      if (c < '0' || c > '9') {
        this.RaiseError("JSON number can't be parsed.");
      }

      int cstart = c;
      c = this.ReadChar();
      this.sb = this.sb ?? new StringBuilder();
      _ = this.sb.Remove(0, this.sb.Length);
      _ = this.sb.Append('-');
      _ = this.sb.Append((char)cstart);
      var charbuf = new char[32];
      var charbufptr = 0;
      while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
        c == 'e' || c == 'E') {
        charbuf[charbufptr++] = (char)c;
        if (charbufptr >= 32) {
          _ = this.sb.Append(charbuf, 0, 32);
          charbufptr = 0;
        }
        c = this.ReadChar();
      }
      if (charbufptr > 0) {
        _ = this.sb.Append(charbuf, 0, charbufptr);
      }
      // DebugUtility.Log("--nega=" + sw.ElapsedMilliseconds + " ms");
      // check if character can validly appear after a JSON number
      if (c != ',' && c != ']' && c != '}' && c != -1 &&
        c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
        this.RaiseError("Invalid character after JSON number");
      }
      str = this.sb.ToString();
      // DebugUtility.Log("negb=" + sw.ElapsedMilliseconds + " ms");
      obj = CBORDataUtilities.ParseJSONNumber(str, this.options);
      // DebugUtility.Log("str=" + str + " obj=" + (obj));
      // DebugUtility.Log("negc=" + sw.ElapsedMilliseconds + " ms");
      if (obj == null) {
        string errstr = (str.Length <= 100) ? str : (str.Substring(0,
              100) + "...");
        this.RaiseError("JSON number can't be parsed. " + errstr);
      }
      if (c == 0x20 || c == 0x0a || c == 0x0d || c == 0x09) {
        nextChar[0] = this.SkipWhitespaceJSON();
      } else if (this.jsonSequenceMode && depth == 0) {
        nextChar[0] = c;
        this.RaiseError("JSON whitespace expected after top-level " +
          "number in JSON sequence");
      } else {
        nextChar[0] = c;
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
        case '"':
          {
            // Parse a string
            // The tokenizer already checked the string for invalid
            // surrogate pairs, so just call the CBORObject
            // constructor directly
            obj = CBORObject.FromRaw(this.NextJSONString());
            nextChar[0] = this.SkipWhitespaceJSON();
            return obj;
          }
        case '{':
          {
            // Parse an object
            obj = this.ParseJSONObject(depth + 1);
            nextChar[0] = this.SkipWhitespaceJSON();
            return obj;
          }
        case '[':
          {
            // Parse an array
            obj = this.ParseJSONArray(depth + 1);
            nextChar[0] = this.SkipWhitespaceJSON();
            return obj;
          }
        case 't':
          {
            // Parse true
            if (this.ReadChar() != 'r' || this.ReadChar() != 'u' ||
this.ReadChar() != 'e') {
              this.RaiseError("Value can't be parsed.");
            }
            c = this.ReadChar();
            if (c == 0x20 || c == 0x0a || c == 0x0d || c == 0x09) {
              nextChar[0] = this.SkipWhitespaceJSON();
            } else if (this.jsonSequenceMode && depth == 0) {
              nextChar[0] = c;
              this.RaiseError("JSON whitespace expected after top-level " +
                "number in JSON sequence");
            } else {
              nextChar[0] = c;
            }
            return CBORObject.True;
          }
        case 'f':
          {
            // Parse false
            if (this.ReadChar() != 'a' || this.ReadChar() != 'l' ||
this.ReadChar() != 's' || this.ReadChar() != 'e') {
              this.RaiseError("Value can't be parsed.");
            }
            c = this.ReadChar();
            if (c == 0x20 || c == 0x0a || c == 0x0d || c == 0x09) {
              nextChar[0] = this.SkipWhitespaceJSON();
            } else if (this.jsonSequenceMode && depth == 0) {
              nextChar[0] = c;
              this.RaiseError("JSON whitespace expected after top-level " +
                "number in JSON sequence");
            } else {
              nextChar[0] = c;
            }
            return CBORObject.False;
          }
        case 'n':
          {
            // Parse null
            if (this.ReadChar() != 'u' || this.ReadChar() != 'l' ||
this.ReadChar() != 'l') {
              this.RaiseError("Value can't be parsed.");
            }
            c = this.ReadChar();
            if (c == 0x20 || c == 0x0a || c == 0x0d || c == 0x09) {
              nextChar[0] = this.SkipWhitespaceJSON();
            } else if (this.jsonSequenceMode && depth == 0) {
              nextChar[0] = c;
              this.RaiseError("JSON whitespace expected after top-level " +
                "number in JSON sequence");
            } else {
              nextChar[0] = c;
            }
            return CBORObject.Null;
          }
        case '-':
          {
            // Parse a negative number
            return this.NextJSONNegativeNumber(nextChar, depth);
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
        case '9':
          {
            // Parse a nonnegative number
            int cval = c - '0';
            int cstart = c;
            var needObj = true;
            c = this.ReadChar();
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
              cval = (cval * 10) + (c - '0');
              c = this.ReadChar();
              if (c >= '0' && c <= '9') {
                var digits = 2;
                var ctmp = new int[10];
                ctmp[0] = cstart;
                ctmp[1] = csecond;
                while (digits < 9 && c >= '0' && c <= '9') {
                  cval = (cval * 10) + (c - '0');
                  ctmp[digits++] = c;
                  c = this.ReadChar();
                }
                if (c == 'e' || c == 'E' || c == '.' || (c >= '0' && c <=
'9')) {
                  // Not an all-digit number, or too long
                  this.sb = this.sb ?? new StringBuilder();
                  _ = this.sb.Remove(0, this.sb.Length);
                  for (int vi = 0; vi < digits; ++vi) {
                    _ = this.sb.Append((char)ctmp[vi]);
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
                _ = this.sb.Remove(0, this.sb.Length);
                _ = this.sb.Append((char)cstart);
                _ = this.sb.Append((char)csecond);
              }
            } else {
              this.sb = this.sb ?? new StringBuilder();
              _ = this.sb.Remove(0, this.sb.Length);
              _ = this.sb.Append((char)cstart);
            }
            if (needObj) {
              var charbuf = new char[32];
              var charbufptr = 0;
              while (
                c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
                c == 'e' || c == 'E') {
                charbuf[charbufptr++] = (char)c;
                if (charbufptr >= 32) {
                  _ = this.sb.Append(charbuf, 0, 32);
                  charbufptr = 0;
                }
                c = this.ReadChar();
              }
              if (charbufptr > 0) {
                _ = this.sb.Append(charbuf, 0, charbufptr);
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
            } else {
              // check if character can validly appear after a JSON number
              if (c != ',' && c != ']' && c != '}' && c != -1 &&
                c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
                this.RaiseError("Invalid character after JSON number");
              }
            }
            if (c == 0x20 || c == 0x0a || c == 0x0d || c == 0x09) {
              nextChar[0] = this.SkipWhitespaceJSON();
            } else if (this.jsonSequenceMode && depth == 0) {
              nextChar[0] = c;
              this.RaiseError("JSON whitespace expected after top-level " +
                "number in JSON sequence");
            } else {
              nextChar[0] = c;
            }
            return obj;
          }
        default: this.RaiseError("Value can't be parsed.");
          break;
      }
      return null;
    }

    public CBORJson(CharacterInputWithCount reader, JSONOptions options) {
      this.reader = reader;
      this.sb = null;
      this.options = options;
      this.jsonSequenceMode = false;
      this.recordSeparatorSeen = false;
    }

    public CBORObject ParseJSON(int[] nextChar) {
      int c;
      CBORObject ret;
      c = this.jsonSequenceMode ? this.SkipWhitespaceJSON(nextChar[0]) :
        this.SkipWhitespaceJSON();
      if (c == '[') {
        ret = this.ParseJSONArray(0);
        nextChar[0] = this.SkipWhitespaceJSON();
        return ret;
      }
      if (c == '{') {
        ret = this.ParseJSONObject(0);
        nextChar[0] = this.SkipWhitespaceJSON();
        return ret;
      }
      return this.NextJSONValue(c, nextChar, 0);
    }

    private void SetJSONSequenceMode() {
      this.jsonSequenceMode = true;
      this.recordSeparatorSeen = false;
    }

    private void ResetJSONSequenceMode() {
      this.jsonSequenceMode = true;
      this.recordSeparatorSeen = false;
    }

    internal static CBORObject ParseJSONValue(
      CharacterInputWithCount reader,
      JSONOptions options,
      int[] nextChar) {
      var cj = new CBORJson(reader, options);
      return cj.ParseJSON(nextChar);
    }

    internal bool SkipRecordSeparators(int[] nextChar, bool
      recordSeparatorSeen) {
      if (this.jsonSequenceMode) {
        while (true) {
          int rc = this.reader.ReadChar();
          nextChar[0] = rc;
          if (rc == 0x1e) {
            recordSeparatorSeen = true;
          } else {
            return recordSeparatorSeen;
          }
        }
      } else {
        nextChar[0] = -1;
        return false;
      }
    }

    internal static CBORObject[] ParseJSONSequence(
      CharacterInputWithCount reader,
      JSONOptions options,
      int[] nextChar) {
      var cj = new CBORJson(reader, options);
      cj.SetJSONSequenceMode();
      bool seenSeparator = cj.SkipRecordSeparators(nextChar, false);
      if (nextChar[0] >= 0 && !seenSeparator) {
        // Stream is not empty and did not begin with
        // record separator
        cj.RaiseError("Not a JSON text sequence");
      } else if (nextChar[0] < 0 && !seenSeparator) {
        // Stream is empty
        return new CBORObject[0];
      } else if (nextChar[0] < 0) {
        // Stream had only record separators, so we found
        // a truncated JSON text
        return new CBORObject[] { null };
      }
      var cborList = new List<CBORObject>();
      while (true) {
        CBORObject co;
        try {
          co = cj.ParseJSON(nextChar);
        } catch (CBORException) {
          cj.SkipToEnd();
          co = null;
        }
        if (co != null && nextChar[0] >= 0) {
          // End of JSON text not reached
          cj.SkipToEnd();
          co = null;
        }
        cborList.Add(co);
        if (!cj.recordSeparatorSeen) {
          // End of the stream was reached
          nextChar[0] = -1;
          break;
        } else {
          // A record separator was seen, so
          // another JSON text follows
          cj.ResetJSONSequenceMode();
          _ = cj.SkipRecordSeparators(nextChar, true);
          if (nextChar[0] < 0) {
            // Rest of stream had only record separators, so we found
            // a truncated JSON text
            cborList.Add(null);
            break;
          }
        }
      }
      return cborList.ToArray();
    }

    private CBORObject ParseJSONObject(int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        this.RaiseError("Too deeply nested");
      }
      int c;
      CBORObject key = null;
      CBORObject obj;
      var nextChar = new int[1];
      var seenComma = false;
      IDictionary<CBORObject, CBORObject> myHashMap =
this.options.KeepKeyOrder ? PropertyMap.NewOrderedDict() : new
SortedDictionary<CBORObject, CBORObject>();
      while (true) {
        c = this.SkipWhitespaceJSON();
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
              // Parse a string that represents the object's key.
              // The tokenizer already checked the string for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(this.NextJSONString());
              key = obj;
              break;
            }
        }
        if (this.SkipWhitespaceJSON() != ':') {
          this.RaiseError("Expected a ':' after a key");
        }
        int oldCount = myHashMap.Count;
        // NOTE: Will overwrite existing value
        myHashMap[key] = this.NextJSONValue(
            this.SkipWhitespaceJSON(),
            nextChar,
            depth);
        int newCount = myHashMap.Count;
        if (!this.options.AllowDuplicateKeys &&
              oldCount == newCount) {
          this.RaiseError("Duplicate key already exists");
          return null;
        }
        switch (nextChar[0]) {
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
      var nextChar = new int[1];
      while (true) {
        int c = this.SkipWhitespaceJSON();
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
            nextChar,
            depth));
        c = nextChar[0];
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
