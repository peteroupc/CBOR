/*
Written by Peter O.
Any copyright to this work is released to the Public Domain.
In case this is not possible, this work is also
licensed under the Unlicense: https://unlicense.org/

 */
using System;
using System.Collections.Generic;
using System.Text;
using PeterO;
using PeterO.Cbor;

// A JSON-like parser that supports nonstandard
// comments before JSON keys, but otherwise supports
// only the standard JSON format
namespace Test {
  internal sealed class JSONWithComments {
    private readonly string jstring;
    private readonly IList<CBORObject> currPointer;
    private readonly JSONOptions options;
    private readonly int endPos;
    private int currPointerStackSize;
    private int index;

    // JSON parsing method
    private int SkipWhitespaceJSON() {
      while (this.index < this.endPos) {
        char c = this.jstring[this.index++];
        if (c is not (char)0x20 and not (char)0x0a and not (char)0x0d and
not (char)0x09) {
          return c;
        }
      }
      return -1;
    }

    internal void RaiseError(string str) {
      throw new CBORException(str + " (approx. offset: " +
        Math.Max(0, this.index - 1) + ")");
    }

    private CBORObject NextJSONString() {
      int c;
      int startIndex = this.index;
      int ep = this.endPos;
      string js = this.jstring;
      int idx = this.index;
      var escaped = false;
      while (true) {
        c = idx < ep ? js[idx++] & 0xffff : -1;
        if (c is -1 or < 0x20) {
          this.index = idx;
          this.RaiseError("Unterminated string");
        } else if (c == '"') {
          int endIndex = idx;
          this.index = idx;
          return escaped ?
            CBORObject.FromJSONString(js[(startIndex - 1)..endIndex]) :
            CBORObject.FromString(js[startIndex..(endIndex - 1)]);
        } else if (c == '\\') {
          this.index = idx++;
          escaped = true;
        }
      }
    }

    private CBORObject NextJSONNumber(
      int[] nextChar) {
      CBORObject obj;
      int numberStartIndex = this.index - 1;
      int c;
      int numberEndIndex;
      while (true) {
        c = this.index < this.endPos ? this.jstring[this.index++] &
          0xffff : -1;
        if (c is not ('-' or '+' or '.' or (>= '0' and <= '9') or
            'e' or 'E')) {
          numberEndIndex = c < 0 ? this.index : this.index - 1;
          obj = CBORDataUtilities.ParseJSONNumber(
            this.jstring[numberStartIndex..numberEndIndex],
            this.options);
          if (obj == null) {
            this.RaiseError("Invalid JSON number");
          }
          break;
        }
      }
      c = numberEndIndex >= this.endPos ? -1 : this.jstring[numberEndIndex];
      // check if character can validly appear after a JSON number
      if (c is not ',' and not ']' and not '}' and not -1 and
        not 0x20 and not 0x0a and not 0x0d and not 0x09) {
        this.RaiseError("Invalid character after JSON number");
      }
      nextChar[0] = c is -1 or (not 0x20 and not 0x0a and not 0x0d and not
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
            obj = this.NextJSONString();
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
            return this.NextJSONNumber(nextChar);
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
            return this.NextJSONNumber(nextChar);
          }
        default: this.RaiseError("Value can't be parsed.");
          break;
      }
      return null;
    }

    internal JSONWithComments(string jstring, int index, int endPos, JSONOptions
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
      this.jstring = jstring;
      this.currPointerStackSize = 0;
      this.currPointer = new List<CBORObject>();
      this.index = index;
      this.Pointers = new List<string[]>();
      this.endPos = endPos;
      this.options = options;
    }

    internal CBORObject ParseJSON(int[] nextchar) {
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

    public static CBORObject FromJSONString(
      string jstring) {
      return jstring == null ? throw new
ArgumentNullException(nameof(jstring)) : FromJSONString(jstring,
  JSONOptions.Default);
    }

    public static CBORObject FromJSONString(
      string jstring,
      JSONOptions options) {
      return jstring == null ? throw new
        ArgumentNullException(nameof(jstring)) :
        ParseJSONValue(jstring, 0, jstring.Length, options);
    }

    public static CBORObject FromJSONStringWithPointers(
      string jstring,
      IDictionary<string, string> valpointers) {
      return jstring == null ?
        throw new ArgumentNullException(nameof(jstring)) :
        FromJSONStringWithPointers(
          jstring,
          JSONOptions.Default,
          valpointers);
    }

    public static CBORObject FromJSONStringWithPointers(
      string jstring,
      JSONOptions options,
      IDictionary<string, string> valpointers) {
      return jstring == null ?
        throw new ArgumentNullException(nameof(jstring)) :
        ParseJSONValueWithPointers(
          jstring,
          0,
          jstring.Length,
          options,
          valpointers);
    }

    internal IList<string[]> Pointers { get; }

    internal static CBORObject ParseJSONValueWithPointers(
      string jstring,
      int index,
      int endPos,
      JSONOptions options,
      IDictionary<string, string> valpointers) {
      // Parse nonstandard comments before JSON keys
      var hasHash = false;
      int i;
      for (i = index; i < endPos; ++i) {
        if (jstring[i] == '#') {
          {
            hasHash = true;
          }
          break;
        }
      }
      // No nonstandard comments, so just use FromJSONString
      if (!hasHash) {
        return CBORObject.FromJSONString(jstring, index, endPos, options);
      }
      var nextchar = new int[1];
      var cj = new JSONWithComments(jstring, index, endPos, options);
      CBORObject obj = cj.ParseJSON(nextchar);
      if (nextchar[0] != -1) {
        cj.RaiseError("End of string not reached");
      }
      if (valpointers != null) {
        IList<string[]> cjpointers = cj.Pointers;
        foreach (string[] sa in cjpointers) {
          string key = sa[0];
          string val = sa[1];
          valpointers[key] = val;
        }
      }
      return obj;
    }

    internal static CBORObject ParseJSONValue(
      string jstring,
      int index,
      int endPos,
      JSONOptions options) {
      // Parse nonstandard comments before JSON keys
      var hasHash = false;
      int i;
      for (i = index; i < endPos; ++i) {
        if (jstring[i] == '#') {
          {
            hasHash = true;
          }
          break;
        }
      }
      // No nonstandard comments, so just use FromJSONString
      if (!hasHash) {
        return CBORObject.FromJSONString(jstring, index, endPos, options);
      }
      var nextchar = new int[1];
      var cj = new JSONWithComments(jstring, index, endPos, options);
      CBORObject obj = cj.ParseJSON(nextchar);
      if (nextchar[0] != -1) {
        cj.RaiseError("End of string not reached");
      }
      return obj;
    }

    // Get sequence of comments starting with '#', and
    // concatenate them (after the '#'). Collapse sequences
    // of U+000D/U+0009/U+0020 into space (U+0020), except
    // ignore those three characters if they begin the first
    // comment. This is rather arbitrary, but in any case,
    // a JSON-like format that supports comments is nonstandard.
    private int NextComment(StringBuilder sb) {
      while (this.index < this.endPos) {
        int c = this.jstring[this.index++];
        if (c is not 0x0d and not 0x09 and not 0x20) {
          --this.index;
          break;
        }
      }
      while (this.index < this.endPos) {
        int c = DataUtilities.CodePointAt(this.jstring, this.index, 2);
        if (c < 0) {
          this.RaiseError("Invalid text");
        }
        if (c < 0x10000) {
          ++this.index;
        } else {
          this.index += 2;
        }
        if (c is 0x0d or 0x09 or 0x20) {
          while (this.index < this.endPos) {
            c = this.jstring[this.index++];
            if (c is not 0x0d and not 0x09 and not 0x20) {
              --this.index;
              break;
            }
          }
          _ = sb.Append((char)0x20);
        } else if (c == 0x0a) {
          c = this.SkipWhitespaceJSON();
          if (c != 0x23) { // '#' character
            // Console.WriteLine("last: " + ((char)c));
            return c;
          }
        } else {
          if (c <= 0xffff) {
            {
              _ = sb.Append((char)c);
            }
          } else if (c <= 0x10ffff) {
            _ = sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) | 0xd800));
            _ = sb.Append((char)(((c - 0x10000) & 0x3ff) | 0xdc00));
          }
        }
      }
      return -1;
    }

    private string GetJSONPointer() {
      var sb = new StringBuilder();
      for (int i = 0; i < this.currPointerStackSize; ++i) {
        CBORObject obj = this.currPointer[i];
        if (obj.Type == CBORType.Integer) {
          _ = sb.Append("/");
          _ = sb.Append(obj.ToJSONString());
        } else if (obj.Type == CBORType.TextString) {
          _ = sb.Append("/");
          string str = obj.AsString();
          for (int j = 0; j < str.Length; ++j) {
            sb = str[j] == '/' ? sb.Append("~1") : str[j] == '~' ?
sb.Append("~0") : sb.Append(str[j]);
          }
        } else {
          this.RaiseError("Internal error");
        }
      }
      return sb.ToString();
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
      var myHashMap = new SortedDictionary<CBORObject, CBORObject>();
      this.PushPointer();
      string commentKey = null;
      while (true) {
        c = this.SkipWhitespaceJSON();
        if (c == '#') {
          // Nonstandard comment
          if (myHashMap.Count == 0) {
            var sb = new StringBuilder();
            c = this.NextComment(sb);
            commentKey = sb.ToString();
          } else {
            this.RaiseError("Unexpected comment");
          }
        }
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
            if (commentKey != null) {
              string[] keyptr = { commentKey, this.GetJSONPointer() };
              this.Pointers.Add(keyptr);
            }
            this.PopPointer();
            return CBORObject.FromObject(myHashMap);
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
              obj = this.NextJSONString();
              key = obj;
              break;
            }
        }
        if (this.SkipWhitespaceJSON() != ':') {
          this.RaiseError("Expected a ':' after a key");
        }
        this.SetPointer(key);
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
            this.PopPointer();
            if (commentKey != null) {
              string[] keyptr = { commentKey, this.GetJSONPointer() };
              this.Pointers.Add(keyptr);
            }
            return CBORObject.FromObject(myHashMap);
          default:
            this.RaiseError("Expected a ',' or '}'");
            break;
        }
      }
    }

    private void SetPointer(CBORObject obj) {
      this.currPointer[this.currPointerStackSize - 1] = obj;
    }

    private void PushPointer() {
      if (this.currPointerStackSize > this.currPointer.Count) {
        this.RaiseError("Internal error");
      }
      if (this.currPointerStackSize == this.currPointer.Count) {
        this.currPointer.Add(CBORObject.Null);
      } else {
        this.currPointer[this.currPointerStackSize] = CBORObject.Null;
      }
      ++this.currPointerStackSize;
    }
    private void PopPointer() {
      if (this.currPointerStackSize < 0) {
        this.RaiseError("Internal error");
      }
      --this.currPointerStackSize;
    }

    internal CBORObject ParseJSONArray(int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        this.RaiseError("Too deeply nested");
      }
      long arrayIndex = 0;
      var myArrayList = CBORObject.NewArray();
      var seenComma = false;
      var nextchar = new int[1];
      this.PushPointer();
      while (true) {
        int c = this.SkipWhitespaceJSON();
        if (c == ']') {
          if (seenComma) {
            // Situation like '[0,1,]'
            this.RaiseError("Trailing comma");
          }
          this.PopPointer();
          return myArrayList;
        }
        if (c == ',') {
          // Situation like '[,0,1,2]' or '[0,,1]'
          this.RaiseError("Empty array element");
        }
        this.SetPointer(CBORObject.FromInt64(arrayIndex));
        arrayIndex = checked(arrayIndex + 1);
        _ = myArrayList.Add(
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
            return myArrayList;
          default: this.RaiseError("Expected a ',' or ']'");
            break;
        }
      }
    }
  }
}
