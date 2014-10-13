/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PeterO;

namespace PeterO.Cbor {
  internal static class CBORJson
  {
    // JSON parsing methods
    internal static int SkipWhitespaceJSON(CharacterReader reader) {
      while (true) {
        int c = reader.NextChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    internal static string NextJSONString(CharacterReader reader, int quote) {
      int c;
      StringBuilder sb = null;
      bool surrogate = false;
      bool surrogateEscaped = false;
      bool escaped = false;
      while (true) {
        c = reader.NextChar();
        if (c == -1 || c < 0x20) {
          throw reader.NewError("Unterminated string");
        }
        switch (c) {
          case '\\':
            c = reader.NextChar();
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
                    int ch = reader.NextChar();
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
                throw reader.NewError("Invalid Unicode escaped character");
                    }
                  }
                  break;
                }
              default: throw reader.NewError("Invalid escaped character");
            }
            break;
          default: escaped = false;
            break;
        }
        if (surrogate) {
          if ((c & 0x1ffc00) != 0xdc00) {
            // Note: this includes the ending quote
            // and supplementary characters
            throw reader.NewError("Unpaired surrogate code point");
          }
          if (escaped != surrogateEscaped) {
            throw reader.NewError(
              "Pairing escaped surrogate with unescaped surrogate");
          }
          surrogate = false;
        } else if ((c & 0x1ffc00) == 0xd800) {
          surrogate = true;
          surrogateEscaped = escaped;
        } else if ((c & 0x1ffc00) == 0xdc00) {
          throw reader.NewError("Unpaired surrogate code point");
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
      CharacterReader reader,
      int firstChar,
      bool noDuplicates,
      int[] nextChar,
      int depth) {
      string str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        throw reader.NewError("Unexpected end of data");
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
        if (reader.NextChar() != 'r' || reader.NextChar() != 'u' ||
            reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.True;
      }
      if (c == 'f') {
        // Parse false
        if (reader.NextChar() != 'a' || reader.NextChar() != 'l' ||
            reader.NextChar() != 's' || reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.False;
      }
      if (c == 'n') {
        // Parse null
        if (reader.NextChar() != 'u' || reader.NextChar() != 'l' ||
            reader.NextChar() != 'l') {
          throw reader.NewError("Value can't be parsed.");
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
          c = reader.NextChar();
        }
        str = sb.ToString();
        obj = CBORDataUtilities.ParseJSONNumber(str);
        if (obj == null) {
          throw reader.NewError("JSON number can't be parsed. " + str);
        }
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          nextChar[0] = c;
        } else {
          nextChar[0] = SkipWhitespaceJSON(reader);
        }
        return obj;
      }
      throw reader.NewError("Value can't be parsed.");
    }

    internal static CBORObject ParseJSONValue(
      CharacterReader reader,
      bool noDuplicates,
      bool objectOrArrayOnly,
      int depth) {
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
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
        throw reader.NewError("A JSON object must begin with '{' or '['");
      }
      var nextChar = new int[1];
      return NextJSONValue(reader, c, noDuplicates, nextChar, depth);
    }

    internal static CBORObject ParseJSONObject(
      CharacterReader reader,
      bool noDuplicates,
      int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      int c;
      CBORObject key;
      CBORObject obj;
      var nextchar = new int[1];
      bool seenComma = false;
      var myHashMap = new Dictionary<CBORObject, CBORObject>();
      while (true) {
        c = SkipWhitespaceJSON(reader);
        switch (c) {
          case -1:
            throw reader.NewError("A JSONObject must end with '}'");
          case '}':
            if (seenComma) {
              // Situation like '{"0"=>1,}'
              throw reader.NewError("Trailing comma");
            }
            return CBORObject.FromRaw(myHashMap);
            default: {
              // Read the next string
              if (c < 0) {
                throw reader.NewError("Unexpected end of data");
              }
              if (c != '"') {
                throw reader.NewError("Expected a string as a key");
              }
              // Parse a string that represents the object's key
              // The tokenizer already checked the string for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(NextJSONString(reader, c));
              key = obj;
              if (noDuplicates && myHashMap.ContainsKey(obj)) {
                throw reader.NewError("Key already exists: " + key);
              }
              break;
            }
        }
        if (SkipWhitespaceJSON(reader) != ':') {
          throw reader.NewError("Expected a ':' after a key");
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
          default: throw reader.NewError("Expected a ',' or '}'");
        }
      }
    }

    internal static CBORObject ParseJSONArray(
      CharacterReader reader,
      bool noDuplicates,
      int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      var myArrayList = new List<CBORObject>();
      bool seenComma = false;
      var nextchar = new int[1];
      while (true) {
        int c = SkipWhitespaceJSON(reader);
        if (c == ']') {
          if (seenComma) {
            // Situation like '[0,1,]'
            throw reader.NewError("Trailing comma");
          }
          return CBORObject.FromRaw(myArrayList);
        }
        if (c == ',') {
          // Situation like '[,0,1,2]' or '[0,,1]'
          throw reader.NewError("Empty array element");
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
            throw reader.NewError("Expected a ',' or ']'");
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
      bool first = true;
      for (var i = 0; i < str.Length; ++i) {
        char c = str[i];
        if (c == '\\' || c == '"') {
          if (first) {
            first = false;
            sb.WriteString(str, 0, i);
          }
          sb.WriteChar('\\');
          sb.WriteChar(c);
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
            sb.WriteChar(Hex16[(int)((c >> 12) & 15)]);
            sb.WriteChar(Hex16[(int)((c >> 8) & 15)]);
            sb.WriteChar(Hex16[(int)((c >> 4) & 15)]);
            sb.WriteChar(Hex16[(int)(c & 15)]);
          } else {
            sb.WriteString("\\u00");
            sb.WriteChar(Hex16[(int)(c >> 4)]);
            sb.WriteChar(Hex16[(int)(c & 15)]);
          }
        } else if (!first) {
          if ((c & 0xfc00) == 0xd800) {
            sb.WriteString(str, i, 2);
            ++i;
          } else {
            sb.WriteChar(c);
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
       writer.WriteString(CBORUtilities.BigIntToString((BigInteger)thisItem));
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
            writer.WriteChar('\"');
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
                writer.WriteChar(Hex16[(byteArray[i] >> 4) & 15]);
                writer.WriteChar(Hex16[byteArray[i] & 15]);
              }
            } else {
              Base64.WriteBase64URL(
writer,
byteArray,
0,
byteArray.Length,
false);
            }
            writer.WriteChar('\"');
            break;
          }
          case CBORObject.CBORObjectTypeTextString: {
            var thisString = (string)thisItem;
            if (thisString.Length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteChar('\"');
            WriteJSONStringUnquoted(thisString, writer);
            writer.WriteChar('\"');
            break;
          }
          case CBORObject.CBORObjectTypeArray: {
            bool first = true;
            writer.WriteChar('[');
            foreach (CBORObject i in obj.AsList()) {
              if (!first) {
                writer.WriteChar(',');
              }
              WriteJSONToInternal(i, writer);
              first = false;
            }
            writer.WriteChar(']');
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
            bool first = true;
            bool hasNonStringKeys = false;
            IDictionary<CBORObject, CBORObject> objMap = obj.AsMap();
            foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
              CBORObject key = entry.Key;
              if (key.ItemType != CBORObject.CBORObjectTypeTextString) {
                hasNonStringKeys = true;
                break;
              }
            }
            if (!hasNonStringKeys) {
              writer.WriteChar('{');
              foreach (KeyValuePair<CBORObject, CBORObject> entry in objMap) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteChar(',');
                }
                writer.WriteChar('\"');
                WriteJSONStringUnquoted((string)key.ThisItem, writer);
                writer.WriteChar('\"');
                writer.WriteChar(':');
                WriteJSONToInternal(value, writer);
                first = false;
              }
              writer.WriteChar('}');
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
              writer.WriteChar('{');
              foreach (KeyValuePair<string, CBORObject> entry in stringMap) {
                string key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteChar(',');
                }
                writer.WriteChar('\"');
                WriteJSONStringUnquoted((string)key, writer);
                writer.WriteChar('\"');
                writer.WriteChar(':');
                WriteJSONToInternal(value, writer);
                first = false;
              }
              writer.WriteChar('}');
            }
            break;
          }
        default:
          throw new InvalidOperationException("Unexpected item type");
      }
    }
  }
}
