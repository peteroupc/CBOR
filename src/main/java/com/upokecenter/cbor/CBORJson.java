package com.upokecenter.cbor;
/*
Written by Peter O. in 2014.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

import java.util.*;

import com.upokecenter.util.*;

  final class CBORJson {
private CBORJson() {
}
    // JSON parsing methods
    static int SkipWhitespaceJSON(CharacterReader reader) {
      while (true) {
        int c = reader.NextChar();
        if (c == -1 || (c != 0x20 && c != 0x0a && c != 0x0d && c != 0x09)) {
          return c;
        }
      }
    }

    static String NextJSONString(CharacterReader reader, int quote) {
      int c;
      StringBuilder sb = null;
      boolean surrogate = false;
      boolean surrogateEscaped = false;
      boolean escaped = false;
      while (true) {
        c = reader.NextChar();
        if (c == -1 || c < 0x20) {
          throw reader.NewError("Unterminated String");
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
                  for (int i = 0; i < 4; ++i) {
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
                      throw
                        reader.NewError(
                          "Invalid Unicode escaped character");
                    }
                  }
                  break;
                }
              default:
                throw reader.NewError("Invalid escaped character");
            }
            break;
          default:
            escaped = false;
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
          return (sb == null) ? "" : sb.toString();
        }
        sb = (sb == null) ? ((new StringBuilder())) : sb;
        if (c <= 0xffff) {
          sb.append((char)c);
        } else {
          sb.append((char)((((c - 0x10000) >> 10) & 0x3ff) + 0xd800));
          sb.append((char)(((c - 0x10000) & 0x3ff) + 0xdc00));
        }
      }
    }

    static CBORObject NextJSONValue(
      CharacterReader reader,
      int firstChar,
      boolean noDuplicates,
      int[] nextChar,
      int depth) {
      String str;
      int c = firstChar;
      CBORObject obj = null;
      if (c < 0) {
        throw reader.NewError("Unexpected end of data");
      }
      if (c == '"') {
        // Parse a String
        // The tokenizer already checked the String for invalid
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
        if (reader.NextChar() != 'r' ||
            reader.NextChar() != 'u' ||
            reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.True;
      }
      if (c == 'f') {
        // Parse false
        if (reader.NextChar() != 'a' ||
            reader.NextChar() != 'l' ||
            reader.NextChar() != 's' ||
            reader.NextChar() != 'e') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.False;
      }
      if (c == 'n') {
        // Parse null
        if (reader.NextChar() != 'u' ||
            reader.NextChar() != 'l' ||
            reader.NextChar() != 'l') {
          throw reader.NewError("Value can't be parsed.");
        }
        nextChar[0] = SkipWhitespaceJSON(reader);
        return CBORObject.Null;
      }
      if (c == '-' || (c >= '0' && c <= '9')) {
        // Parse a number
        StringBuilder sb = new StringBuilder();
        while (c == '-' || c == '+' || c == '.' || (c >= '0' && c <= '9') ||
               c == 'e' || c == 'E') {
          sb.append((char)c);
          c = reader.NextChar();
        }
        str = sb.toString();
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

    static CBORObject ParseJSONValue(
      CharacterReader reader,
      boolean noDuplicates,
      boolean objectOrArrayOnly,
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
        throw reader.NewError("A JSON Object must begin with '{' or '['");
      }
      int[] nextChar = new int[1];
      return NextJSONValue(reader, c, noDuplicates, nextChar, depth);
    }

    static CBORObject ParseJSONObject(
      CharacterReader reader,
      boolean noDuplicates,
      int depth) {
      // Assumes that the last character read was '{'
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      int c;
      CBORObject key;
      CBORObject obj;
      int[] nextchar = new int[1];
      boolean seenComma = false;
      HashMap<CBORObject, CBORObject> myHashMap = new HashMap<CBORObject, CBORObject>();
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
              // Read the next String
              if (c < 0) {
                throw reader.NewError("Unexpected end of data");
              }
              if (c != '"') {
                throw reader.NewError("Expected a String as a key");
              }
              // Parse a String that represents the Object's key
              // The tokenizer already checked the String for invalid
              // surrogate pairs, so just call the CBORObject
              // constructor directly
              obj = CBORObject.FromRaw(NextJSONString(reader, c));
              key = obj;
              if (noDuplicates && myHashMap.containsKey(obj)) {
                throw reader.NewError("Key already exists: " + key);
              }
              break;
            }
        }
        if (SkipWhitespaceJSON(reader) != ':') {
          throw reader.NewError("Expected a ':' after a key");
        }
        // NOTE: Will overwrite existing value
        myHashMap.put(key, NextJSONValue(
          reader,
          SkipWhitespaceJSON(reader),
          noDuplicates,
          nextchar,
          depth));
        switch (nextchar[0]) {
          case ',':
            seenComma = true;
            break;
          case '}':
            return CBORObject.FromRaw(myHashMap);
          default:
            throw reader.NewError("Expected a ',' or '}'");
        }
      }
    }

    static CBORObject ParseJSONArray(
      CharacterReader reader,
      boolean noDuplicates,
      int depth) {
      // Assumes that the last character read was '['
      if (depth > 1000) {
        throw reader.NewError("Too deeply nested");
      }
      ArrayList<CBORObject> myArrayList = new ArrayList<CBORObject>();
      boolean seenComma = false;
      int[] nextchar = new int[1];
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
        myArrayList.add(
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

    private static final String Hex16 = "0123456789ABCDEF";

    static void WriteJSONStringUnquoted(
      String str,
      Utf8Writer sb) {
      // Surrogates were already verified when this
      // String was added to the CBOR Object; that check
      // is not repeated here
      boolean first = true;
      for (int i = 0; i < str.length(); ++i) {
        char c = str.charAt(i);
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
            sb.WriteChar(Hex16.charAt((int)((c >> 12) & 15)));
            sb.WriteChar(Hex16.charAt((int)((c >> 8) & 15)));
            sb.WriteChar(Hex16.charAt((int)((c >> 4) & 15)));
            sb.WriteChar(Hex16.charAt((int)(c & 15)));
          } else {
            sb.WriteString("\\u00");
            sb.WriteChar(Hex16.charAt((int)(c >> 4)));
            sb.WriteChar(Hex16.charAt((int)(c & 15)));
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

    static void WriteJSONToInternal(
      CBORObject obj,
      Utf8Writer writer) {
      int type = obj.getItemType();
      Object thisItem = obj.getThisItem();
      switch (type) {
          case CBORObject.CBORObjectTypeSimpleValue: {
            if (obj.isTrue()) {
              writer.WriteString("true");
              return;
            }
            if (obj.isFalse()) {
              writer.WriteString("false");
              return;
            }
            writer.WriteString("null");
            return;
          }
          case CBORObject.CBORObjectTypeSingle: {
            float f = ((Float)thisItem).floatValue();
            if (((f) == Float.NEGATIVE_INFINITY) ||
                ((f) == Float.POSITIVE_INFINITY) ||
                Float.isNaN(f)) {
              writer.WriteString("null");
              return;
            }
            writer.WriteString(
              CBORObject.TrimDotZero(
                Float.toString((float)f)));
            return;
          }
          case CBORObject.CBORObjectTypeDouble: {
            double f = ((Double)thisItem).doubleValue();
            if (((f) == Double.NEGATIVE_INFINITY) ||
                ((f) == Double.POSITIVE_INFINITY) ||
                Double.isNaN(f)) {
              writer.WriteString("null");
              return;
            }
            writer.WriteString(
              CBORObject.TrimDotZero(
                Double.toString((double)f)));
            return;
          }
          case CBORObject.CBORObjectTypeInteger: {
            long longItem = (((Long)thisItem).longValue());
            writer.WriteString(
              CBORUtilities.LongToString(longItem));
            return;
          }
          case CBORObject.CBORObjectTypeBigInteger: {
            writer.WriteString(
              CBORUtilities.BigIntToString((BigInteger)thisItem));
            return;
          }
          case CBORObject.CBORObjectTypeExtendedDecimal: {
            ExtendedDecimal dec = (ExtendedDecimal)thisItem;
            if (dec.IsInfinity() || dec.IsNaN()) {
              writer.WriteString("null");
            } else {
              writer.WriteString(dec.toString());
            }
            return;
          }
          case CBORObject.CBORObjectTypeExtendedFloat: {
            ExtendedFloat flo = (ExtendedFloat)thisItem;
            if (flo.IsInfinity() || flo.IsNaN()) {
              writer.WriteString("null");
              return;
            }
            if (flo.isFinite() &&
                (flo.getExponent()).abs().compareTo(BigInteger.valueOf(2500)) > 0) {
              // Too inefficient to convert to a decimal number
              // from a bigfloat with a very high exponent,
              // so convert to double instead
              double f = flo.ToDouble();
              if (((f) == Double.NEGATIVE_INFINITY) ||
                  ((f) == Double.POSITIVE_INFINITY) ||
                  Double.isNaN(f)) {
                writer.WriteString("null");
                return;
              }
              writer.WriteString(
                CBORObject.TrimDotZero(
                  Double.toString((double)f)));
              return;
            }
            writer.WriteString(flo.toString());
            return;
          }
        case CBORObject.CBORObjectTypeByteString:
          {
            byte[] byteArray = (byte[])thisItem;
            if (byteArray.length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteChar('\"');
            if (obj.HasTag(22)) {
              Base64.WriteBase64(
                writer,
                byteArray,
                0,
                byteArray.length,
                false);
            } else if (obj.HasTag(23)) {
              // Write as base16
              for (int i = 0; i < byteArray.length; ++i) {
                writer.WriteChar(Hex16.charAt((byteArray[i] >> 4) & 15));
                writer.WriteChar(Hex16.charAt(byteArray[i] & 15));
              }
            } else {
              Base64.WriteBase64URL(
                writer,
                byteArray,
                0,
                byteArray.length,
                false);
            }
            writer.WriteChar('\"');
            break;
          }
          case CBORObject.CBORObjectTypeTextString: {
            String thisString = (String)thisItem;
            if (thisString.length() == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteChar('\"');
            WriteJSONStringUnquoted(thisString, writer);
            writer.WriteChar('\"');
            break;
          }
          case CBORObject.CBORObjectTypeArray: {
            boolean first = true;
            writer.WriteChar('[');
            for (CBORObject i : obj.AsList()) {
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
            ExtendedRational dec = (ExtendedRational)thisItem;
            ExtendedDecimal f = dec.ToExtendedDecimalExactIfPossible(
              PrecisionContext.Decimal128.WithUnlimitedExponents());
            if (!f.isFinite()) {
              writer.WriteString("null");
            } else {
              writer.WriteString(f.toString());
            }
            break;
          }
          case CBORObject.CBORObjectTypeMap: {
            boolean first = true;
            boolean hasNonStringKeys = false;
            Map<CBORObject, CBORObject> objMap = obj.AsMap();
            for (Map.Entry<CBORObject, CBORObject> entry : objMap.entrySet()) {
              CBORObject key = entry.getKey();
              if (key.getItemType() != CBORObject.CBORObjectTypeTextString) {
                hasNonStringKeys = true;
                break;
              }
            }
            if (!hasNonStringKeys) {
              writer.WriteChar('{');
              for (Map.Entry<CBORObject, CBORObject> entry : objMap.entrySet()) {
                CBORObject key = entry.getKey();
                CBORObject value = entry.getValue();
                if (!first) {
                  writer.WriteChar(',');
                }
                writer.WriteChar('\"');
                WriteJSONStringUnquoted((String)key.getThisItem(), writer);
                writer.WriteChar('\"');
                writer.WriteChar(':');
                WriteJSONToInternal(value, writer);
                first = false;
              }
              writer.WriteChar('}');
            } else {
              // This map has non-String keys
              Map<String, CBORObject> stringMap = new
                HashMap<String, CBORObject>();
              // Copy to a map with String keys, since
              // some keys could be duplicates
              // when serialized to strings
              for (Map.Entry<CBORObject, CBORObject> entry : objMap.entrySet()) {
                CBORObject key = entry.getKey();
                CBORObject value = entry.getValue();
                String str = (key.getItemType() ==
                              CBORObject.CBORObjectTypeTextString) ?
                  ((String)key.getThisItem()) : key.ToJSONString();
                stringMap.put(str, value);
              }
              first = true;
              writer.WriteChar('{');
              for (Map.Entry<String, CBORObject> entry : stringMap.entrySet()) {
                String key = entry.getKey();
                CBORObject value = entry.getValue();
                if (!first) {
                  writer.WriteChar(',');
                }
                writer.WriteChar('\"');
                WriteJSONStringUnquoted((String)key, writer);
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
          throw new IllegalStateException("Unexpected item type");
      }
    }
  }
