/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.IO;
using PeterO.Numbers;

namespace PeterO.Cbor {
  internal sealed class CBORJson2 {
    private readonly byte[] bytes;
    private readonly JSONOptions options;
    private readonly int endPos;
    private static readonly byte[] ValueEmptyBytes = new byte[0];
    private int index;

    // JSON parsing method
    private int SkipWhitespaceJSON() {
      while (this.index < this.endPos) {
        byte c = this.bytes[this.index++];
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

    private byte[] NextJSONString() {
      int c;
      int startIndex = this.index;
      int batchIndex = startIndex;
      byte[] jbytes = this.bytes;
      int batchEnd;
      while (true) {
        if (this.index >= this.endPos) {
          this.RaiseError("Unterminated string");
        }
        c = jbytes[this.index++] & 0xff;
        if (c < 0x20) {
          this.RaiseError("Invalid character in string literal");
        }
        if (c == '\\') {
          batchEnd = this.index - 1;
          break;
        } else if (c == 0x22) {
          int isize = this.index - startIndex - 1;
          if (isize == 0) {
            return ValueEmptyBytes;
          }
          var buf = new byte[isize];
          Array.Copy(jbytes, startIndex, buf, 0, isize);
          return buf;
        } else if (c < 0x80) {
          continue;
        } else if (c >= 0xc2 && c <= 0xdf) {
          int c1 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          if (c1 < 0x80 || c1 > 0xbf) {
            this.RaiseError("Invalid encoding");
          }
        } else if (c >= 0xe0 && c <= 0xef) {
          int c1 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          int c2 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          int lower = (c == 0xe0) ? 0xa0 : 0x80;
          int upper = (c == 0xed) ? 0x9f : 0xbf;
          if (c1 < lower || c1 > upper || c2 < 0x80 || c2 > 0xbf) {
            this.RaiseError("Invalid encoding");
          }
        } else if (c >= 0xf0 && c <= 0xf4) {
          int c1 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          int c2 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          int c3 = this.index < this.endPos ?
            this.bytes[this.index++] & 0xff : -1;
          int lower = (c == 0xf0) ? 0x90 : 0x80;
          int upper = (c == 0xf4) ? 0x8f : 0xbf;
          if (c1 < lower || c1 > upper || c2 < 0x80 || c2 > 0xbf ||
            c3 < 0x80 || c3 > 0xbf) {
            this.RaiseError("Invalid encoding");
          }
        } else {
          this.RaiseError("Invalid encoding");
        }
      }
      using (var ms = new MemoryStream()) {
        if (batchEnd > batchIndex) {
          ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
          this.index = batchEnd;
          batchIndex = batchEnd;
        } else {
          this.index = startIndex;
          batchIndex = startIndex;
        }
        while (true) {
          batchEnd = this.index;
          c = this.index < this.endPos ? jbytes[this.index++] &
            0xff : -1;
          if (c == -1) {
            this.RaiseError("Unterminated string");
          }
          if (c < 0x20) {
            this.RaiseError("Invalid character in string literal");
          }
          switch (c) {
            case '\\':
              c = this.index < this.endPos ? jbytes[this.index++] &
                0xff : -1;
              switch (c) {
                case '\\':
                case '/':
                case '\"':
                  // Slash is now allowed to be escaped under RFC 8259
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)c);
                  break;
                case 'b':
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)'\b');
                  break;
                case 'f':
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)'\f');
                  break;
                case 'n':
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)'\n');
                  break;
                case 'r':
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)'\r');
                  break;
                case 't':
                  if (batchEnd > batchIndex) {
                    ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                  }
                  batchIndex = this.index;
                  ms.WriteByte((byte)'\t');
                  break;
                case 'u':
                  { // Unicode escape
                    c = 0;
                    // Consists of 4 hex digits
                    for (int i = 0; i < 4; ++i) {
                      int ch = this.index < this.endPos ?
jbytes[this.index++] : -1;
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
                      if (batchEnd > batchIndex) {
                        ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                      }
                      batchIndex = this.index;
                      int ic = c;
                      if (c >= 0x800) {
                        ms.WriteByte((byte)(0xe0 | ((ic >> 12) & 0x0f)));
                        ms.WriteByte((byte)(0x80 | ((ic >> 6) & 0x3f)));
                        ms.WriteByte((byte)(0x80 | (ic & 0x3f)));
                      } else if (c >= 0x80) {
                        ms.WriteByte((byte)(0xc0 | ((ic >> 6) & 0x1f)));
                        ms.WriteByte((byte)(0x80 | (ic & 0x3f)));
                      } else {
                        ms.WriteByte((byte)ic);
                      }
                    } else if ((c & 0xfc00) == 0xd800) {
                      int ch;
                      if (this.index >= this.endPos - 1 ||
                        jbytes[this.index] != (byte)'\\' ||
                        jbytes[this.index + 1] != 0x75) {
                        this.RaiseError("Invalid escaped character");
                      }
                      this.index += 2;
                      var c2 = 0;
                      for (int i = 0; i < 4; ++i) {
                        ch = this.index < this.endPos ?
                          jbytes[this.index++] & 0xff : -1;
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
                        if (batchEnd > batchIndex) {
                          ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
                        }
                        batchIndex = this.index;
                        int ic = 0x10000 + (
                          (c & 0x3ff) << 10) + (c2 & 0x3ff);
                        ms.WriteByte((byte)(0xf0 | ((ic >> 18) & 0x07)));
                        ms.WriteByte((byte)(0x80 | ((ic >> 12) & 0x3f)));
                        ms.WriteByte((byte)(0x80 | ((ic >> 6) & 0x3f)));
                        ms.WriteByte((byte)(0x80 | (ic & 0x3f)));
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
              if (batchEnd > batchIndex) {
                ms.Write(jbytes, batchIndex, batchEnd - batchIndex);
              }
              return ms.ToArray();
            default: {
                if (c <= 0x7f) {
                  // Deliberately empty
                } else if (c >= 0xc2 && c <= 0xdf) {
                  int c1 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  if (c1 < 0x80 || c1 > 0xbf) {
                    this.RaiseError("Invalid encoding");
                  }
                } else if (c >= 0xe0 && c <= 0xef) {
                  int c1 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  int c2 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  int lower = (c == 0xe0) ? 0xa0 : 0x80;
                  int upper = (c == 0xed) ? 0x9f : 0xbf;
                  if (c1 < lower || c1 > upper || c2 < 0x80 || c2 > 0xbf) {
                    this.RaiseError("Invalid encoding");
                  }
                } else if (c >= 0xf0 && c <= 0xf4) {
                  int c1 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  int c2 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  int c3 = this.index < this.endPos ?
                    jbytes[this.index++] & 0xff : -1;
                  int lower = (c == 0xf0) ? 0x90 : 0x80;
                  int upper = (c == 0xf4) ? 0x8f : 0xbf;
                  if (c1 < lower || c1 > upper || c2 < 0x80 || c2 > 0xbf ||
                c3 < 0x80 || c3 > 0xbf) {
                    this.RaiseError("Invalid encoding");
                  }
                } else {
                  this.RaiseError("Invalid encoding");
                }
                break;
              }
          }
        }
      }
    }

    private CBORObject NextJSONNegativeNumber(
      int[] nextChar) {
      // Assumes the last character read was '-'
      CBORObject obj;
      int numberStartIndex = this.index - 1;
      int c = this.index < this.endPos ? this.bytes[this.index++] &
        0xff : -1;
      if (c < '0' || c > '9') {
        this.RaiseError("JSON number can't be parsed.");
      }
      int cstart = c;
      while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
        c == 'e' || c == 'E') {
        c = this.index < this.endPos ? this.bytes[this.index++] &
          0xff : -1;
      }
      // check if character can validly appear after a JSON number
      if (c != ',' && c != ']' && c != '}' && c != -1 &&
        c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
        this.RaiseError("Invalid character after JSON number");
      }
      int numberEndIndex = c < 0 ?
        this.endPos : this.index - 1;
      if (numberEndIndex - numberStartIndex == 2 && cstart != '0') {
        // Negative single digit other than negative zero
        obj = CBORDataUtilities.ParseSmallNumberAsNegative(cstart
              - '0',
            this.options);
      } else {
        obj = CBORDataUtilities.ParseJSONNumber(
            this.bytes,
            numberStartIndex,
            numberEndIndex - numberStartIndex,
            this.options);
#if DEBUG
        if (this.options.NumberConversion == JSONOptions.ConversionMode.Full &&
obj.ToEDecimal().CompareToValue(EDecimal.FromString(this.bytes,
           numberStartIndex,
           numberEndIndex - numberStartIndex)) != 0) {
          this.RaiseError(String.Empty + obj);
        }
#endif
        if (obj == null) {
          string errstr = String.Empty;
          // errstr = (str.Length <= 100) ? str : (str.Substring(0,
          // 100) + "...");
          this.RaiseError("JSON number can't be parsed. " + errstr);
        }
      }
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
      c = this.index < this.endPos ? this.bytes[this.index++] &
        0xff : -1;
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
        c = this.index < this.endPos ? this.bytes[this.index++] &
          0xff : -1;
        if (c >= '0' && c <= '9') {
          var digits = 2;
          while (digits < 9 && c >= '0' && c <= '9') {
            cval = (cval * 10) + (c - '0');
            c = this.index < this.endPos ? this.bytes[this.index++] & 0xff : -1;
            ++digits;
          }
          if (!(c == 'e' || c == 'E' || c == '.' || (c >= '0' && c <=
                '9'))) {
            // All-digit number that's short enough
            obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
#if DEBUG
            if (obj.ToEDecimal().CompareToValue(EDecimal.FromInt32(cval)) !=
0) {
              this.RaiseError(String.Empty + obj);
            }
#endif
            needObj = false;
          }
        } else if (!(c == '-' || c == '+' || c == '.' || c == 'e' || c
            == 'E')) {
          // Optimize for common case where JSON number
          // is two digits without sign, decimal point, or exponent
          obj = CBORDataUtilities.ParseSmallNumber(cval, this.options);
#if DEBUG
          if (obj.ToEDecimal().CompareToValue(EDecimal.FromInt32(cval)) != 0) {
            this.RaiseError(String.Empty + obj);
          }
#endif
          needObj = false;
        }
      }
      if (needObj) {
        while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
          c == 'e' || c == 'E') {
          c = this.index < this.endPos ? this.bytes[this.index++] &
            0xff : -1;
        }
        // check if character can validly appear after a JSON number
        if (c != ',' && c != ']' && c != '}' && c != -1 &&
          c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09) {
          this.RaiseError("Invalid character after JSON number");
        }
        int numberEndIndex = c < 0 ? this.endPos : this.index - 1;
        obj = CBORDataUtilities.ParseJSONNumber(
           this.bytes,
           numberStartIndex,
           numberEndIndex - numberStartIndex,
           this.options);
#if DEBUG
        if (this.options.NumberConversion == JSONOptions.ConversionMode.Full &&
obj.ToEDecimal().CompareToValue(EDecimal.FromString(this.bytes,
           numberStartIndex,
           numberEndIndex - numberStartIndex)) != 0) {
          this.RaiseError(String.Empty + obj);
        }
#endif
        if (obj == null) {
          string errstr = String.Empty;
          // errstr = (str.Length <= 100) ? str : (str.Substring(0,
          // 100) + "...");
          this.RaiseError("JSON number can't be parsed. " + errstr);
        }
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
            obj = CBORObject.FromRawUtf8(this.NextJSONString());
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
              this.bytes[this.index] != 0x72 ||
              this.bytes[this.index + 1] != 0x75 ||
              this.bytes[this.index + 2] != 0x65) {
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
              this.bytes[this.index] != 0x61 ||
              this.bytes[this.index + 1] != 0x6c ||
              this.bytes[this.index + 2] != 0x73 ||
              this.bytes[this.index + 3] != 0x65) {
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
              this.bytes[this.index] != 0x75 ||
              this.bytes[this.index + 1] != 0x6c ||
              this.bytes[this.index + 2] != 0x6c) {
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

    public CBORJson2(byte[] bytes, int index, int endPos, JSONOptions
      options) {
#if DEBUG
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is not greater or" +
          "\u0020equal to 0");
      }
      if (index > bytes.Length) {
        throw new ArgumentException("index (" + index + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (endPos < 0) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to 0");
      }
      if (endPos > bytes.Length) {
        throw new ArgumentException("endPos (" + endPos + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (endPos < index) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to " + index);
      }
#endif

      this.bytes = bytes;
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
      byte[] bytes,
      int index,
      int endPos,
      JSONOptions options) {
      var nextchar = new int[1];
      var cj = new CBORJson2(bytes, index, endPos, options);
      CBORObject obj = cj.ParseJSON(nextchar);
      if (nextchar[0] != -1) {
        cj.RaiseError("End of bytes not reached");
      }
      return obj;
    }

    internal static CBORObject ParseJSONValue(
      byte[] bytes,
      int index,
      int endPos,
      JSONOptions options,
      int[] nextchar) {
#if DEBUG
      if (bytes == null) {
        throw new ArgumentNullException(nameof(bytes));
      }
      if (index < 0) {
        throw new ArgumentException("index (" + index + ") is not greater or" +
          "\u0020equal to 0");
      }
      if (index > bytes.Length) {
        throw new ArgumentException("index (" + index + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (endPos < 0) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to 0");
      }
      if (endPos > bytes.Length) {
        throw new ArgumentException("endPos (" + endPos + ") is not less or" +
          "\u0020equal to " + bytes.Length);
      }
      if (endPos < index) {
        throw new ArgumentException("endPos (" + endPos + ") is not greater" +
          "\u0020or equal to " + index);
      }
#endif

      var cj = new CBORJson2(bytes, index, endPos, options);
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
              obj = CBORObject.FromRawUtf8(this.NextJSONString());
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
