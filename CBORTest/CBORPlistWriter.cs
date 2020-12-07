using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PeterO;
using PeterO.Cbor;

namespace Test {
  internal static class CBORPlistWriter {
    private const string Hex16 = "0123456789ABCDEF";

    internal static void WritePlistStringUnquoted(
      string str,
      StringOutput sb,
      JSONOptions options) {
      var i = 0;
      for (; i < str.Length; ++i) {
        char c = str[i];
        if (c < 0x20 || c >= 0x7f || c == '\\' || c == '"' || c == '&' ||
            c == '<' || c == '>') {
          sb.WriteString(str, 0, i);
          break;
        }
      }
      if (i == str.Length) {
        sb.WriteString(str, 0, i);
        return;
      }
      for (; i < str.Length; ++i) {
        char c = str[i];
        // TODO: XML doesn't support all Unicode code points, even if escaped.
        // Therefore, replace all unsupported code points with replacement
        // characters.
        if (c == '\\' || c == '"') {
          sb.WriteCodePoint((int)'\\');
          sb.WriteCodePoint((int)c);
        } else if (c < 0x20 || c == '&' || c == '<' || c == '>' || (c >= 0x7f &&
            (c == 0x2028 || c == 0x2029 ||
              (c >= 0x7f && c <= 0xa0) || c == 0xfeff || c == 0xfffe ||
              c == 0xffff))) {
          sb.WriteString("&#x");
          sb.WriteCodePoint((int)Hex16[(int)((c >> 12) & 15)]);
          sb.WriteCodePoint((int)Hex16[(int)((c >> 8) & 15)]);
          sb.WriteCodePoint((int)Hex16[(int)((c >> 4) & 15)]);
          sb.WriteCodePoint((int)Hex16[(int)(c & 15)]);
          sb.WriteString(";");
        } else if ((c & 0xfc00) == 0xd800) {
          if (i >= str.Length - 1 || (str[i + 1] & 0xfc00) != 0xdc00) {
            if (options.ReplaceSurrogates) {
              // Replace unpaired surrogate with U+FFFD
              sb.WriteCodePoint(0xfffd);
            } else {
              throw new CBORException("Unpaired surrogate in string");
            }
          } else {
            sb.WriteString(str, i, 2);
            ++i;
          }
        } else {
          sb.WriteCodePoint((int)c);
        }
      }
    }

    internal static string ToPlistString(CBORObject obj) {
      var builder = new StringBuilder();
      try {
         WritePlistToInternal(
            obj,
            new StringOutput(builder),
            JSONOptions.Default);
         return builder.ToString();
      } catch (IOException ex) {
         throw new CBORException(ex.Message, ex);
      }
    }

    internal static void WritePlistToInternal(
      CBORObject obj,
      StringOutput writer,
      JSONOptions options) {
      writer.WriteString("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST");
      writer.WriteString(" 1.0//EN\" \"http://www.apple.com/DTDs/");
      writer.WriteString("PropertyList-1.0.dtd\"><plist version='1.0'>");
      WritePlistToInternalCore(obj, writer, options);
      writer.WriteString("</plist>");
    }

    internal static void WritePlistToInternalCore(
      CBORObject obj,
      StringOutput writer,
      JSONOptions options) {
      if (obj.Type == CBORType.Array || obj.Type == CBORType.Map) {
        var stack = new List<CBORObject>();
        WritePlistToInternalCore(obj, writer, options, stack);
      } else {
        WritePlistToInternalCore(obj, writer, options, null);
      }
    }

    private static void PopRefIfNeeded(
      IList<CBORObject> stack,
      bool pop) {
      if (pop && stack != null) {
        stack.RemoveAt(stack.Count - 1);
      }
    }

    private static bool CheckCircularRef(
      IList<CBORObject> stack,
      CBORObject parent,
      CBORObject child) {
      if (child.Type != CBORType.Array && child.Type != CBORType.Map) {
        return false;
      }
      CBORObject childUntag = child.Untag();
      if (parent.Untag() == childUntag) {
        throw new CBORException("Circular reference in CBOR object");
      }
      if (stack != null) {
        foreach (CBORObject o in stack) {
          if (o.Untag() == childUntag) {
            throw new CBORException("Circular reference in CBOR object");
          }
        }
      }
      stack.Add(child);
      return true;
    }

    // TODO: Support dates
    internal static void WritePlistToInternalCore(
      CBORObject obj,
      StringOutput writer,
      JSONOptions options,
      IList<CBORObject> stack) {
      if (obj.IsNumber) {
        if (obj.AsNumber().IsInteger()) {
          writer.WriteString("<integer>");
          writer.WriteString(obj.ToJSONString());
          writer.WriteString("</integer>");
        } else {
          writer.WriteString("<real>");
          writer.WriteString(obj.ToJSONString());
          writer.WriteString("</real>");
        }
        return;
      }
      switch (obj.Type) {
        case CBORType.Integer: {
          CBORObject untaggedObj = obj.Untag();
          writer.WriteString("<integer>");
          writer.WriteString(untaggedObj.ToJSONString());
          writer.WriteString("</integer>");
          break;
        }
        case CBORType.FloatingPoint: {
          CBORObject untaggedObj = obj.Untag();
          writer.WriteString("<real>");
          writer.WriteString(untaggedObj.ToJSONString());
          writer.WriteString("</real>");
          break;
        }
        case CBORType.Boolean: {
          if (obj.IsTrue) {
            writer.WriteString("<true/>");
            return;
          }
          if (obj.IsFalse) {
            writer.WriteString("<false/>");
            return;
          }
          return;
        }
        case CBORType.SimpleValue: {
          writer.WriteString("<string>null</string>");
          return;
        }
        case CBORType.ByteString: {
          byte[] byteArray = obj.GetByteString();
          if (byteArray.Length == 0) {
            writer.WriteString("<data></data>");
            return;
          }
          if (obj.HasTag(22)) {
            writer.WriteString("<data>");
            // Base64 with padding
            Base64.WriteBase64(
              writer,
              byteArray,
              0,
              byteArray.Length,
              true);
            writer.WriteString("</data>");
          } else if (obj.HasTag(23)) {
            writer.WriteString("<string>");
            // Write as base16
            for (int i = 0; i < byteArray.Length; ++i) {
              writer.WriteCodePoint((int)Hex16[(byteArray[i] >> 4) & 15]);
              writer.WriteCodePoint((int)Hex16[byteArray[i] & 15]);
            }
            writer.WriteString("</string>");
          } else {
            writer.WriteString("<data>");
            // Base64 with padding
            Base64.WriteBase64(
              writer,
              byteArray,
              0,
              byteArray.Length,
              true);
            writer.WriteString("</data>");
          }
          break;
        }
        case CBORType.TextString: {
          string thisString = obj.AsString();
          if (thisString.Length == 0) {
            writer.WriteString("<string></string>");
            return;
          }
          writer.WriteString("<string>");
          WritePlistStringUnquoted(thisString, writer, options);
          writer.WriteString("</string>");
          break;
        }
        case CBORType.Array: {
          writer.WriteString("<array>");
          for (var i = 0; i < obj.Count; ++i) {
            bool pop = CheckCircularRef(stack, obj, obj[i]);
            WritePlistToInternalCore(obj[i], writer, options, stack);
            PopRefIfNeeded(stack, pop);
          }
          writer.WriteString("</array>");
          break;
        }
        case CBORType.Map: {
          var hasNonStringKeys = false;
          ICollection<KeyValuePair<CBORObject, CBORObject>> entries =
            obj.Entries;
          foreach (KeyValuePair<CBORObject, CBORObject> entry in entries) {
            CBORObject key = entry.Key;
            if (key.Type != CBORType.TextString ||
              key.IsTagged) {
              // treat a non-text-string item or a tagged item
              // as having non-string keys
              hasNonStringKeys = true;
              break;
            }
          }
          if (!hasNonStringKeys) {
            writer.WriteString("<dict>");
            foreach (KeyValuePair<CBORObject, CBORObject> entry in entries) {
              CBORObject key = entry.Key;
              CBORObject value = entry.Value;
              writer.WriteString("<key>");
              WritePlistStringUnquoted(key.AsString(), writer, options);
              writer.WriteString("</key>");
              bool pop = CheckCircularRef(stack, obj, value);
              WritePlistToInternalCore(value, writer, options, stack);
              PopRefIfNeeded(stack, pop);
            }
            writer.WriteString("</dict>");
          } else {
            // This map has non-string keys
            IDictionary<string, CBORObject> stringMap = new
            Dictionary<string, CBORObject>();
            // Copy to a map with String keys, since
            // some keys could be duplicates
            // when serialized to strings
            foreach (KeyValuePair<CBORObject, CBORObject> entry
              in entries) {
              CBORObject key = entry.Key;
              CBORObject value = entry.Value;
              string str = null;
              switch (key.Type) {
                case CBORType.TextString:
                  str = key.AsString();
                  break;
                case CBORType.Array:
                case CBORType.Map: {
                  var sb = new StringBuilder();
                  var sw = new StringOutput(sb);
                  bool pop = CheckCircularRef(stack, obj, key);
                  WritePlistToInternalCore(key, sw, options, stack);
                  PopRefIfNeeded(stack, pop);
                  str = sb.ToString();
                  break;
                }
                default: str = key.ToJSONString(options);
                  break;
              }
              if (stringMap.ContainsKey(str)) {
                throw new CBORException(
                  "Duplicate Plist string equivalents of map" +
                  "\u0020keys");
              }
              stringMap[str] = value;
            }
            writer.WriteString("<dict>");
            foreach (KeyValuePair<string, CBORObject> entry in stringMap) {
              string key = entry.Key;
              CBORObject value = entry.Value;
              writer.WriteString("<key>");
              WritePlistStringUnquoted((string)key, writer, options);
              writer.WriteString("</key>");
              bool pop = CheckCircularRef(stack, obj, value);
              WritePlistToInternalCore(value, writer, options, stack);
              PopRefIfNeeded(stack, pop);
            }
            writer.WriteString("</dict>");
          }
          break;
        }
        default: throw new InvalidOperationException("Unexpected item" +
            "\u0020type");
      }
    }
  }
}
