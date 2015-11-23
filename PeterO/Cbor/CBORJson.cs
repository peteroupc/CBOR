/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.dreamhosters.com/articles/donate-now-2/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
  internal static class CBORJson {
    // JSON parsing methods
    internal static int SkipWhitespaceJSON(CharacterInputWithCount reader) {
      while (true) {
        int c = reader.ReadChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    internal static string NextJSONString(
CharacterInputWithCount reader,
int quote) {
      int c;
      StringBuilder sb = null;
      var surrogate = false;
      var surrogateEscaped = false;
      var escaped = false;
      while (true) {
        c = reader.ReadChar();
        if (c == -1 || c < 0x20) {
          reader.RaiseError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = reader.ReadChar();
            escaped = true;
            switch (c) {
              case '\\':
                c = '\\';
                break;
              case '/':
                // Now allowed to be escaped under RFC 7159
                c = '/';
                break;
              case '\"':
                c = '\"';
                break;
              case 'b':
                c = '\b';
                break;
              case 'f':
                c = '\f';
                break;
              case 'n':
                c = '\n';
                break;
              case 'r':
                c = '\r';
                break;
              case 't':
                c = '\t';
                break;
                case 'u': { // Unicode escape
                  c = 0;
                  // Consists of 4 hex digits
                  for (var i = 0; i < 4; ++i) {
                    int ch = reader.ReadChar();
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
                reader.RaiseError("Invalid Unicode escaped character");
                    }
                  }
                  break;
                }
                default: reader.RaiseError("Invalid escaped character");
                  break;
            }
            break;
            default: escaped = false;
            break;
        }
        if (surrogate) {
          if ((c & 0x1ffc00) != 0xdc00) {
            // Note: this includes the ending quote
            // and supplementary characters
            reader.RaiseError("Unpaired surrogate code point");
          }
          if (escaped != surrogateEscaped) {
            reader.RaiseError(
              "Pairing escaped surrogate with unescaped surrogate");
          }
          surrogate = false;
        } else if ((c & 0x1ffc00) == 0xd800) {
          surrogate = true;
          surrogateEscaped = escaped;
        } else if ((c & 0x1ffc00) == 0xdc00) {
          reader.RaiseError("Unpaired surrogate code point");
        }
        if (c == quote && !escaped) {
          // End quote reached
          return (sb == null) ? String.Empty : sb.ToString();
        }
        sb = sb ?? (new StringBuilder());
        if (c <= 0xffff) {
          sb.Append((char)c);
        } else {
          sb.Append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          sb.Append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
    }

    internal static CBORObject NextJSONValue(
      CharacterInputWithCount reader,
      int firstChar,
      bool noDuplicates,
      int[] nextChar,
      int depth) {
      string str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        reader.RaiseError("Unexpected end of data");
      }
      if (c == '"') {
        // Parse a string
        // The tokenizer already checked the string for invalid
        // surrogate pairs, so just call the CBORObject
        // constructor directly
        obj = CBORObject.FromRaw(NextJSONString(reader, c));
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      }
      if (c == '{') {
        // Parse an object
        obj = ParseJSONObject(reader, noDuplicates, depth + 1);
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      }
      if (c == '[') {
        // Parse an array
        obj = ParseJSONArray(reader, noDuplicates, depth + 1);
        nextChar[0] = SkipWhitespaceJSON(reader);
        return obj;
      }
      if (c == 't') {
        // Parse true
        if (reader.ReadChar() != 'r' || reader.ReadChar() != 'u' ||
            reader.ReadChar() != 'e') {
          reader.RaiseError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.True;
      }
      if (c == 'f') {
        // Parse false
        if (reader.ReadChar() != 'a' || reader.ReadChar() != 'l' ||
            reader.ReadChar() != 's' || reader.ReadChar() != 'e') {
          reader.RaiseError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.False;
      }
      if (c == 'n') {
        // Parse null
        if (reader.ReadChar() != 'u' || reader.ReadChar() != 'l' ||
            reader.ReadChar() != 'l') {
          reader.RaiseError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.Null;
      }
      if (c == '-' || (c >= '0' && c <= '9')) {
        // Parse a number
        var sb = new StringBuilder();
        while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
               c == 'e' || c == 'E') {
          sb.Append((char)c);
          c = reader.ReadChar();
        }
        str = sb.ToString();
        obj = CBORDataUtilities.ParseJSONNumber(str);
        if (obj == null) {
          reader.RaiseError("JSON number can't be parsed. " + str);
        }
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          nextChar[0] = c;
        } else {
          nextChar[0] = SkipWhitespaceJSON(reader);
        }
        return obj;
      }
      reader.RaiseError("Value can't be parsed.");
      return null;
    }

    internal static CBORObject ParseJSONValue(
      CharacterInputWithCount reader,
      bool noDuplicates,
      bool objectOrArrayOnly,
      int depth) {
      if (depth > 1000) {
        reader.RaiseError("Too deeply nested");
      }
      int c;
      c = SkipWhitespaceJSON(reader);
      if (c == '[') {
        return ParseJSONArray(reader, noDuplicates, depth);
      }
      if (c == '{') {
        return ParseJSONObject(reader, noDuplicates, depth);
      }
      if (objectOrArrayOnly) {
        reader.RaiseError("A JSON object must begin with '{' or '['");
      }
      var nextChar = new int[1];
      return NextJSONValue(reader, c, noDuplicates, nextChar, depth);
    }

    internal static CBORObject ParseJSONObject(
      CharacterInputWithCount reader,
      bool noDuplicates,
      int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        reader.RaiseError("Too deeply nested");
      }
      int c;
      CBORObject key = null;
      CBORObject obj;
      var nextchar = new int[1];
      var seenComma = false;
      var myHashMap = new Dictionary<CBORObject, CBORObject>();
      while (true) {
        c = SkipWhitespaceJSON(reader);
        switch (c) {
          case -1:
            reader.RaiseError("A JSONObject must end with '}'");
            break;
          case '}':
            if (seenComma) {
              // Situation like '{"0"=>1,}'
              reader.RaiseError("Trailing comma");
              return null;
            }
            return CBORObject.FromRaw(myHashMap);
            default: {
              // Read the next string
              if (c < 0) {
                reader.RaiseError("Unexpected end of data");
                return null;
              }
              if (c != '"') {
                reader.RaiseError("Expected a string as a key");
                return null;
              }
              // Parse a string that represents the object's key
              // The tokenizer already checked the string for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(NextJSONString(reader, c));
              key = obj;
              if (noDuplicates && myHashMap.ContainsKey(obj)) {
                reader.RaiseError("Key already exists: " + key);
                return null;
              }
              break;
            }
        }
        if (SkipWhitespaceJSON(reader) != ':') {
          reader.RaiseError("Expected a ':' after a key");
        }
        // NOTE: Will overwrite existing value
        myHashMap[key] = NextJSONValue(
          reader,
          SkipWhitespaceJSON(reader),
          noDuplicates,
          nextchar,
          depth);
        switch (nextchar[0]) {
          case ',':
            seenComma = true;
            break;
          case '}':
            return CBORObject.FromRaw(myHashMap);
            default: reader.RaiseError("Expected a ',' or '}'");
            break;
        }
      }
    }

    internal static CBORObject ParseJSONArray(
      CharacterInputWithCount reader,
      bool noDuplicates,
      int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        reader.RaiseError("Too deeply nested");
      }
      var myArrayList = new List<CBORObject>();
      var seenComma = false;
      var nextchar = new int[1];
      while (true) {
        int c = SkipWhitespaceJSON(reader);
        if (c == ']') {
          if (seenComma) {
            // Situation like '[0,1,]'
            reader.RaiseError("Trailing comma");
          }
          return CBORObject.FromRaw(myArrayList);
        }
        if (c == ',') {
          // Situation like '[,0,1,2]' or '[0,,1]'
          reader.RaiseError("Empty array element");
        }
        myArrayList.Add(
          NextJSONValue(
            reader,
            c,
            noDuplicates,
            nextchar,
            depth));
        c = nextchar[0];
        switch (c) {
          case ',':
            seenComma = true;
            break;
          case ']':
            return CBORObject.FromRaw(myArrayList);
          default:
            reader.RaiseError("Expected a ',' or ']'");
            break;
        }
      }
    }

    private const string Hex16 = "0123456789ABCDEF";

    internal static void WriteJSONStringUnquoted(
      string str,
      StringOutput sb) {
      // Surrogates were already verified when this
      // string was added to the CBOR object; that check
      // is not repeated here
      var first = true;
      for (var i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c == '\\' || c == '"') {
          if (first) {
            first = false;
            sb.WriteString(str, 0, i);
          }
          sb.WriteCodePoint((int)'\\');
          sb.WriteCodePoint((int)c);
        } else if (c < 0x20 || (c >= 0x85 && (c == 0x2028 || c == 0x2029 ||
                    c == 0x85 || c == 0xfeff || c == 0xfffe ||
                    c == 0xffff))) {
          // Control characters, and also the line and paragraph separators
          // which apparently can't appear in JavaScript (as opposed to
          // JSON) strings
          if (first) {
            first = false;
            sb.WriteString(str, 0, i);
          }
          if (c == 0x0d) {
            sb.WriteString("\\r");
          } else if (c == 0x0a) {
            sb.WriteString("\\n");
          } else if (c == 0x08) {
            sb.WriteString("\\b");
          } else if (c == 0x0c) {
            sb.WriteString("\\f");
          } else if (c == 0x09) {
            sb.WriteString("\\t");
          } else if (c == 0x85) {
            sb.WriteString("\\u0085");
          } else if (c >= 0x2028) {
            sb.WriteString("\\u");
            sb.WriteCodePoint((int)Hex16[(int)((c >> 12) & 15)]);
            sb.WriteCodePoint((int)Hex16[(int)((c >> 8) & 15)]);
            sb.WriteCodePoint((int)Hex16[(int)((c >> 4) & 15)]);
            sb.WriteCodePoint((int)Hex16[(int)(c & 15)]);
          } else {
            sb.WriteString("\\u00");
            sb.WriteCodePoint((int)Hex16[(int)(c >> 4)]);
            sb.WriteCodePoint((int)Hex16[(int)(c & 15)]);
          }
        } else if (!first) {
          if ((c & 0xfc00) == 0xd800) {
            sb.WriteString(str, i, 2);
            ++i;
          } else {
            sb.WriteCodePoint((int)c);
          }
        }
      }
      if (first) {
        sb.WriteString(str);
      }
    }

    internal static void WriteJSONToInternal(
      CBORObject obj,
      StringOutput writer) {
      int type = obj.ItemType;
      object thisItem = obj.ThisItem;
      switch (type) {
          case CBORObject.CBORObjectTypeSimpleValue: {
            if (obj.IsTrue) {
              writer.WriteString("true");
              return;
            }
            if (obj.IsFalse) {
              writer.WriteString("false");
              return;
            }
            writer.WriteString("null");
            return;
          }
          case CBORObject.CBORObjectTypeSingle: {
            var f = (float)thisItem;
            if (Single.IsNegativeInfinity(f) ||
                Single.IsPositiveInfinity(f) || Single.IsNaN(f)) {
              writer.WriteString("null");
              return;
            }
            writer.WriteString(
              CBORObject.TrimDotZero(
                Convert.ToString(
                  (float)f,
                  CultureInfo.InvariantCulture)));
            return;
          }
          case CBORObject.CBORObjectTypeDouble: {
            var f = (double)thisItem;
            if (Double.IsNegativeInfinity(f) || Double.IsPositiveInfinity(f) ||
                Double.IsNaN(f)) {
              writer.WriteString("null");
              return;
            }
            writer.WriteString(CBORObject.TrimDotZero(
              Convert.ToString(
                (double)f,
                CultureInfo.InvariantCulture)));
            return;
          }
          case CBORObject.CBORObjectTypeInteger: {
            var longItem = (long)thisItem;
            writer.WriteString(CBORUtilities.LongToString(longItem));
            return;
          }
          case CBORObject.CBORObjectTypeBigInteger: {
            writer.WriteString(
              CBORUtilities.BigIntToString((BigInteger)thisItem));
            return;
          }
          case CBORObject.CBORObjectTypeExtendedDecimal: {
            var dec = (ExtendedDecimal)thisItem;
            if (dec.IsInfinity() || dec.IsNaN()) {
              writer.WriteString("null");
            } else {
              writer.WriteString(dec.ToString());
            }
            return;
          }
          case CBORObject.CBORObjectTypeExtendedFloat: {
            var flo = (ExtendedFloat)thisItem;
            if (flo.IsInfinity() || flo.IsNaN()) {
              writer.WriteString("null");
              return;
            }
            if (flo.IsFinite &&
                BigInteger.Abs(flo.Exponent).CompareTo((BigInteger)2500) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              double f = flo.ToDouble();
              if (Double.IsNegativeInfinity(f) ||
                  Double.IsPositiveInfinity(f) || Double.IsNaN(f)) {
                writer.WriteString("null");
                return;
              }
              writer.WriteString(
                CBORObject.TrimDotZero(
                  Convert.ToString(
                    (double)f,
                    CultureInfo.InvariantCulture)));
              return;
            }
            writer.WriteString(flo.ToString());
            return;
          }
        case CBORObject.CBORObjectTypeByteString:
          {
            var byteArray = (byte[])thisItem;
            if (byteArray.Length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteCodePoint((int)'\"');
            if (obj.HasTag(22)) {
              Base64.WriteBase64(
                writer,
                byteArray,
                0,
                byteArray.Length,
                false);
            } else if (obj.HasTag(23)) {
              // Write as base16
              for (int i = 0; i < byteArray.Length; ++i) {
                writer.WriteCodePoint((int)Hex16[(byteArray[i] >> 4) & 15]);
                writer.WriteCodePoint((int)Hex16[byteArray[i] & 15]);
              }
            } else {
              Base64.WriteBase64URL(
                writer,
                byteArray,
                0,
                byteArray.Length,
                false);
            }
            writer.WriteCodePoint((int)'\"');
            break;
          }
          case CBORObject.CBORObjectTypeTextString: {
            var thisString = (string)thisItem;
            if (thisString.Length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteCodePoint((int)'\"');
            WriteJSONStringUnquoted(thisString, writer);
            writer.WriteCodePoint((int)'\"');
            break;
          }
          case CBORObject.CBORObjectTypeArray: {
            var first = true;
            writer.WriteCodePoint((int)'[');
            foreach (CBORObject i in obj.AsList()) {
              if (!first) {
                writer.WriteCodePoint((int)',');
              }
              WriteJSONToInternal(i, writer);
              first = false;
            }
            writer.WriteCodePoint((int)']');
            break;
          }
          case CBORObject.CBORObjectTypeExtendedRational: {
            var dec = (ExtendedRational)thisItem;
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.IsFinite) {
              writer.WriteString("null");
            } else {
              writer.WriteString(f.ToString());
            }
            break;
          }
          case CBORObject.CBORObjectTypeMap: {
            var first = true;
            var hasNonStringKeys = false;
            IDictionary<CBORObject, CBORObject> objMap = obj.AsMap();
            foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
              CBORObject key = entry.Key;
              if (key.ItemType != CBORObject.CBORObjectTypeTextString) {
                hasNonStringKeys = true;
                break;
              }
            }
            if (!hasNonStringKeys) {
              writer.WriteCodePoint((int)'{');
              foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteCodePoint((int)',');
                }
                writer.WriteCodePoint((int)'\"');
                WriteJSONStringUnquoted((string)key.ThisItem, writer);
                writer.WriteCodePoint((int)'\"');
                writer.WriteCodePoint((int)':');
                WriteJSONToInternal(value, writer);
                first = false;
              }
              writer.WriteCodePoint((int)'}');
            } else {
              // This map has non-string keys
              IDictionary<string, CBORObject> stringMap = new
                Dictionary<String, CBORObject>();
              // Copy to a map with String keys, since
              // some keys could be duplicates
              // when serialized to strings
              foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
           string str = (key.ItemType == CBORObject.CBORObjectTypeTextString) ?
                  ((string)key.ThisItem) : key.ToJSONString();
                stringMap[str] = value;
              }
              first = true;
              writer.WriteCodePoint((int)'{');
              foreach (KeyValuePair<string, CBORObject> entry in stringMap) {
                string key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteCodePoint((int)',');
                }
                writer.WriteCodePoint((int)'\"');
                WriteJSONStringUnquoted((string)key, writer);
                writer.WriteCodePoint((int)'\"');
                writer.WriteCodePoint((int)':');
                WriteJSONToInternal(value, writer);
                first = false;
              }
              writer.WriteCodePoint((int)'}');
            }
            break;
          }
        default:
          throw new InvalidOperationException("Unexpected item type");
      }
    }
  }
}
