/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Text;

// NOTE: Certain differences from CBORJson2 are noted.
namespace PeterO.Cbor {
  internal sealed class CBORJson3 {
    // NOTE: Differs from CBORJson2
    private readonly string jstring;
    private readonly JSONOptions options;
    private readonly int endPos;
    private StringBuilder sb;
    private int index;

    // JSON parsing method
    private int SkipWhitespaceJSON() {
      while (this.index < this.endPos) {
        char c = this.jstring[this.index++];
        if (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
          return c;
        }
      }
      return -1;
    }

    internal void RaiseError(string str) {
      throw new CBORException(str + " (approx. offset: " +
        Math.Max(0, this.index - 1) + ")");
    }

    private string NextJSONString() {
      int c;
      int startIndex = this.index;
      int ep = this.endPos;
      string js = this.jstring;
      int idx = this.index;
      int endIndex;
      while (true) {
        c = idx < ep ? js[idx++] & 0xffff : -1;
        if (c == -1 || c < 0x20) {
          this.index = idx;
          this.RaiseError("Unterminated string");
        } else if (c == '"') {
          int iend = idx - 1;
          this.index = idx;
          return js.Substring(
              startIndex,
              iend - startIndex);
        } else if (c == '\\' || (c & 0xf800) == 0xd800) {
          this.index = idx - 1;
          endIndex = this.index;
          break;
        }
      }
      this.sb = this.sb ?? new StringBuilder();
      this.sb.Remove(0, this.sb.Length);
      this.sb.Append(js, startIndex, endIndex - startIndex);
      while (true) {
        c = this.index < ep ? js[this.index++] & 0xffff : -1;
        if (c == -1 || c < 0x20) {
          this.RaiseError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = this.index < ep ? js[this.index++] & 0xffff : -1;
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
                    int ch = this.index < ep ? js[this.index++] : -1;
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
                    int ch = this.index < ep ? js[this.index++] : -1;
                    if (ch != '\\' || (this.index < ep ?
                          js[this.index++] : -1) != 'u') {
                      this.RaiseError("Invalid escaped character");
                    }
                    var c2 = 0;
                    for (int i = 0; i < 4; ++i) {
                      ch = this.index < ep ? js[this.index++] : -1;
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
              // NOTE: Differs from CBORJson2
              if ((c & 0xf800) != 0xd800) {
                // Non-surrogate
                _ = this.sb.Append((char)c);
              } else if ((c & 0xfc00) == 0xd800 && this.index < ep &&
                (js[this.index] & 0xfc00) == 0xdc00) {
                // Surrogate pair
                _ = this.sb.Append((char)c);
                _ = this.sb.Append(js[this.index]);
                ++this.index;
              } else {
                this.RaiseError("Unpaired surrogate code point");
              }
              break;
            }
        }
      }
    }

    private CBORObject NextJSONNegativeNumber(
      int[] nextChar) {
      // Assumes the last character read was '-'
      // DebugUtility.Log("js=" + (jstring));
      CBORObject obj;
      int numberStartIndex = this.index - 1;
      int c = this.index < this.endPos ? this.jstring[this.index++] &
        0xffff : -1;
      if (c < '0' || c > '9') {
        this.RaiseError("JSON number can't be parsed.");
      }
      if (this.index < this.endPos && c != '0') {
        // Check for negative single-digit
        int c2 = this.jstring[this.index] & 0xffff;
        if (c2 == ',' || c2 == ']' || c2 == '}') {
          ++this.index;
          obj = CBORDataUtilities.ParseSmallNumberAsNegative(
              c - '0',
              this.options);
          nextChar[0] = c2;
          return obj;
        } else if (c2 == 0x20 || c2 == 0x0a || c2 == 0x0d || c2 == 0x09) {
          ++this.index;
          obj = CBORDataUtilities.ParseSmallNumberAsNegative(
              c - '0',
              this.options);
          nextChar[0] = this.SkipWhitespaceJSON();
          return obj;
        }
      }
      // NOTE: Differs from CBORJson2, notably because the whole
      // rest of the string is checked whether the beginning of the rest
      // is a JSON number
      var endIndex = new int[1];
      endIndex[0] = numberStartIndex;
      obj = CBORDataUtilitiesTextString.ParseJSONNumber(
          this.jstring,
          numberStartIndex,
          this.endPos - numberStartIndex,
          this.options,
          endIndex);
      int numberEndIndex = endIndex[0];
      this.index = numberEndIndex >= this.endPos ? this.endPos :
        (numberEndIndex + 1);
      if (obj == null) {
        int strlen = numberEndIndex - numberStartIndex;
        string errstr = this.jstring.Substring(numberStartIndex,
            Math.Min(100, strlen));
        if (strlen > 100) {
          errstr += "...";
        }
        this.RaiseError("JSON number can't be parsed. " + errstr);
      }
#if DEBUG
      if (numberEndIndex < numberStartIndex) {
        throw new ArgumentException("numberEndIndex (" + numberEndIndex +
          ") is not greater or equal to " + numberStartIndex);
      }
#endif
      c = numberEndIndex >= this.endPos ? -1 : this.jstring[numberEndIndex];
      // check if character can validly appear after a JSON number
      if (c != ',' && c != ']' && c != '}' && c != -1 &&
        c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
        this.RaiseError("Invalid character after JSON number");
      }
      // DebugUtility.Log("endIndex="+endIndex[0]+", "+
      // this.jstring.Substring(endIndex[0],
      // Math.Min(20, this.endPos-endIndex[0])));
      nextChar[0] = c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c !=
0x09) ? c : this.SkipWhitespaceJSON();
      return obj;
    }

    private CBORObject NextJSONNonnegativeNumber(int c, int[] nextChar) {
      // Assumes the last character read was a digit
      CBORObject obj = null;
      int cval = c - '0';
      int cstart = c;
      int startIndex = this.index - 1;
      var needObj = true;
      int numberStartIndex = this.index - 1;
      // DebugUtility.Log("js=" + (jstring));
      c = this.index < this.endPos ? this.jstring[this.index++] &
        0xffff : -1;
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
        c = this.index < this.endPos ? this.jstring[this.index++] : -1;
        if (c >= '0' && c <= '9') {
          var digits = 2;
          while (digits < 9 && c >= '0' && c <= '9') {
            cval = (cval * 10) + (c - '0');
            c = this.index < this.endPos ? this.jstring[this.index++] : -1;
            ++digits;
          }
          if (!(c == 'e' || c == 'E' || c == '.' || (c >= '0' && c <=
                '9'))) {
            // All-digit number that's short enough
            obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
            needObj = false;
          }
        } else if (!(c == '-' || c == '+' || c == '.' || c == 'e' || c
            == 'E')) {
          // Optimize for common case where JSON number
          // is two digits without sign, decimal point, or exponent
          obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
          needObj = false;
        }
      }
      if (needObj) {
        // NOTE: Differs from CBORJson2, notably because the whole
        // rest of the string is checked whether the beginning of the rest
        // is a JSON number
        var endIndex = new int[1];
        endIndex[0] = numberStartIndex;
        obj = CBORDataUtilitiesTextString.ParseJSONNumber(
            this.jstring,
            numberStartIndex,
            this.endPos - numberStartIndex,
            this.options,
            endIndex);
        int numberEndIndex = endIndex[0];
        this.index = numberEndIndex >= this.endPos ? this.endPos :
          (numberEndIndex + 1);
        if (obj == null) {
          int strlen = numberEndIndex - numberStartIndex;
          string errstr = this.jstring.Substring(numberStartIndex,
              Math.Min(100, strlen));
          if (strlen > 100) {
            errstr += "...";
          }
          this.RaiseError("JSON number can't be parsed. " + errstr);
        }
#if DEBUG
        if (numberEndIndex < numberStartIndex) {
          throw new ArgumentException("numberEndIndex (" + numberEndIndex +
            ") is not greater or equal to " + numberStartIndex);
        }
#endif
        c = numberEndIndex >= this.endPos ? -1 : this.jstring[numberEndIndex];
        // check if character can validly appear after a JSON number
        if (c != ',' && c != ']' && c != '}' && c != -1 &&
          c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
          this.RaiseError("Invalid character after JSON number");
        }
        // DebugUtility.Log("endIndex="+endIndex[0]+", "+
        // this.jstring.Substring(endIndex[0],
        // Math.Min(20, this.endPos-endIndex[0])));
      } else {
        // check if character can validly appear after a JSON number
        if (c != ',' && c != ']' && c != '}' && c != -1 &&
          c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
          this.RaiseError("Invalid character after JSON number");
        }
      }
      nextChar[0] = c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c !=
0x09) ? c : this.SkipWhitespaceJSON();
      return obj;
    }

    private CBORObject NextJSONValue(
      int firstChar,
      int[] nextChar,
      int depth) {
      int c = firstChar;
      if (c < 0) {
        this.RaiseError("Unexpected end of data");
      }
      CBORObject obj;
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
            if (this.endPos - this.index <= 2 ||
              (this.jstring[this.index] & 0xFF) != 'r' ||
              (this.jstring[this.index + 1] & 0xFF) != 'u' ||
              (this.jstring[this.index + 2] & 0xFF) != 'e') {
              this.RaiseError("Value can't be parsed.");
            }
            this.index += 3;
            nextChar[0] = this.SkipWhitespaceJSON();
            return CBORObject.True;
          }
        case 'f':
          {
            // Parse false
            if (this.endPos - this.index <= 3 ||
              (this.jstring[this.index] & 0xFF) != 'a' ||
              (this.jstring[this.index + 1] & 0xFF) != 'l' ||
              (this.jstring[this.index + 2] & 0xFF) != 's' ||
              (this.jstring[this.index + 3] & 0xFF) != 'e') {
              this.RaiseError("Value can't be parsed.");
            }
            this.index += 4;
            nextChar[0] = this.SkipWhitespaceJSON();
            return CBORObject.False;
          }
        case 'n':
          {
            // Parse null
            if (this.endPos - this.index <= 2 ||
              (this.jstring[this.index] & 0xFF) != 'u' ||
              (this.jstring[this.index + 1] & 0xFF) != 'l' ||
              (this.jstring[this.index + 2] & 0xFF) != 'l') {
              this.RaiseError("Value can't be parsed.");
            }
            this.index += 3;
            nextChar[0] = this.SkipWhitespaceJSON();
            return CBORObject.Null;
          }
        case '-':
          {
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
        case '9':
          {
            // Parse a nonnegative number
            return this.NextJSONNonnegativeNumber(c, nextChar);
          }
        default:
          this.RaiseError("Value can't be parsed.");
          break;
      }
      return null;
    }

    public CBORJson3(string jstring, int index, int endPos, JSONOptions
      options) {
#if DEBUG
      if (jstring == null) {
        throw new ArgumentNullException(nameof(jstring));
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is not greater or" +
          "\u0020equal to 0");
      }
      if (index > jstring.Length) {
        throw new ArgumentException("index (" + index + ") is not less or" +
          "\u0020equal to " + jstring.Length);
      }
      if (endPos < 0) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to 0");
      }
      if (endPos > jstring.Length) {
        throw new ArgumentException("endPos (" + endPos + ") is not less or" +
          "\u0020equal to " + jstring.Length);
      }
      if (endPos < index) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to " + index);
      }
#endif
      this.sb = null;
      this.jstring = jstring;
      this.index = index;
      this.endPos = endPos;
      this.options = options;
    }

    public CBORObject ParseJSON(int[] nextchar) {
      int c;
      CBORObject ret;
      c = this.SkipWhitespaceJSON();
      if (c == '[') {
        ret = this.ParseJSONArray(0);
        nextchar[0] = this.SkipWhitespaceJSON();
        return ret;
      }
      if (c == '{') {
        ret = this.ParseJSONObject(0);
        nextchar[0] = this.SkipWhitespaceJSON();
        return ret;
      }
      return this.NextJSONValue(c, nextchar, 0);
    }

    internal static CBORObject ParseJSONValue(
      string jstring,
      int index,
      int endPos,
      JSONOptions options) {
      var nextchar = new int[1];
      var cj = new CBORJson3(jstring, index, endPos, options);
      CBORObject obj = cj.ParseJSON(nextchar);
      if (nextchar[0] != -1) {
        cj.RaiseError("End of string not reached");
      }
      return obj;
    }

    internal static CBORObject ParseJSONValue(
      string jstring,
      int index,
      int endPos,
      JSONOptions options,
      int[] nextchar) {
#if DEBUG
      if (jstring == null) {
        throw new ArgumentNullException(nameof(jstring));
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is not greater or" +
          "\u0020equal to 0");
      }
      if (index > jstring.Length) {
        throw new ArgumentException("index (" + index + ") is not less or" +
          "\u0020equal to " + jstring.Length);
      }
      if (endPos < 0) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to 0");
      }
      if (endPos > jstring.Length) {
        throw new ArgumentException("endPos (" + endPos + ") is not less or" +
          "\u0020equal to " + jstring.Length);
      }
      if (endPos < index) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to " + index);
      }
#endif

      var cj = new CBORJson3(jstring, index, endPos, options);
      return cj.ParseJSON(nextchar);
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
              // Parse a string that represents the object's key
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
            nextchar,
            depth);
        int newCount = myHashMap.Count;
        if (!this.options.AllowDuplicateKeys &&
              oldCount == newCount) {
          this.RaiseError("Duplicate key already exists");
          return null;
        }
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
