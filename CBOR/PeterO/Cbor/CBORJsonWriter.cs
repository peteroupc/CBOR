using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Cbor {
  internal static class CBORJsonWriter {
    private const string Hex16 = "0123456789ABCDEF";

    internal static void WriteJSONStringUnquoted(
      string str,
      StringOutput sb,
      JSONOptions options) {
      var i = 0;
      for (; i < str.Length; ++i) {
        char c = str[i];
        if (c < 0x20 || c >= 0x7f || c == '\\' || c == '"') {
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
        if (c == '\\' || c == '"') {
          sb.WriteCodePoint('\\');
          sb.WriteCodePoint(c);
        } else if (c < 0x20 || (c >= 0x7f && (c == 0x2028 || c == 0x2029 ||
              (c >= 0x7f && c <= 0xa0) || c == 0xfeff || c == 0xfffe ||
              c == 0xffff))) {
          // Control characters, and also the line and paragraph separators
          // which apparently can't appear in JavaScript (as opposed to
          // JSON) strings
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
          } else if (c >= 0x100) {
            sb.WriteString("\\u");
            sb.WriteCodePoint(Hex16[(c >> 12) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 8) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 4) & 15]);
            sb.WriteCodePoint(Hex16[c & 15]);
          } else {
            sb.WriteString("\\u00");
            sb.WriteCodePoint(Hex16[c >> 4]);
            sb.WriteCodePoint(Hex16[c & 15]);
          }
        } else if ((c & 0xfc00) == 0xd800) {
          if (i >= str.Length - 1 || (str[i + 1] & 0xfc00) != 0xdc00) {
            // NOTE: RFC 8259 doesn't prohibit any particular
            // error-handling behavior when a writer of JSON
            // receives a string with an unpaired surrogate.
            if (options.ReplaceSurrogates) {
              // Replace unpaired surrogate with U+FFFD
              sb.WriteCodePoint(0xfffd);
            } else {
              throw new CBORException("Unpaired surrogate in string");
            }
          } else if (c >= 0x80 && options.WriteBasic) {
            c = str[i];
            sb.WriteString("\\u");
            sb.WriteCodePoint(Hex16[(c >> 12) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 8) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 4) & 15]);
            sb.WriteCodePoint(Hex16[c & 15]);
            c = str[i + 1];
            sb.WriteString("\\u");
            sb.WriteCodePoint(Hex16[(c >> 12) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 8) & 15]);
            sb.WriteCodePoint(Hex16[(c >> 4) & 15]);
            sb.WriteCodePoint(Hex16[c & 15]);
          } else {
            sb.WriteString(str, i, 2);
          }
          ++i;
        } else if (c >= 0x80 && options.WriteBasic) {
          sb.WriteString("\\u");
          sb.WriteCodePoint(Hex16[(c >> 12) & 15]);
          sb.WriteCodePoint(Hex16[(c >> 8) & 15]);
          sb.WriteCodePoint(Hex16[(c >> 4) & 15]);
          sb.WriteCodePoint(Hex16[c & 15]);
        } else {
          sb.WriteCodePoint(c);
        }
      }
    }

    internal static void WriteJSONToInternal(
      CBORObject obj,
      StringOutput writer,
      JSONOptions options) {
      if (obj.Type == CBORType.Array || obj.Type == CBORType.Map) {
        var stack = new List<CBORObject>();
        WriteJSONToInternal(obj, writer, options, stack);
      } else {
        WriteJSONToInternal(obj, writer, options, null);
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

    internal static void WriteJSONToInternal(
      CBORObject obj,
      StringOutput writer,
      JSONOptions options,
      IList<CBORObject> stack) {
      if (obj.IsNumber) {
        writer.WriteString(CBORNumber.FromCBORObject(obj).ToJSONString());
        return;
      }
      switch (obj.Type) {
        case CBORType.Integer:
        case CBORType.FloatingPoint:
          {
            CBORObject untaggedObj = obj.Untag();
            writer.WriteString(
              CBORNumber.FromCBORObject(untaggedObj).ToJSONString());
            break;
          }
        case CBORType.Boolean:
          {
            if (obj.IsTrue) {
              writer.WriteString("true");
              return;
            }
            if (obj.IsFalse) {
              writer.WriteString("false");
              return;
            }
            return;
          }
        case CBORType.SimpleValue:
          {
            writer.WriteString("null");
            return;
          }
        case CBORType.ByteString:
          {
            byte[] byteArray = obj.GetByteString();
            if (byteArray.Length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteCodePoint('\"');
            if (obj.HasTag(22)) {
              // Base64 with padding
              Base64.WriteBase64(
                writer,
                byteArray,
                0,
                byteArray.Length,
                true);
            } else if (obj.HasTag(23)) {
              // Write as base16
              for (int i = 0; i < byteArray.Length; ++i) {
                writer.WriteCodePoint(Hex16[(byteArray[i] >> 4) & 15]);
                writer.WriteCodePoint(Hex16[byteArray[i] & 15]);
              }
            } else {
              // Base64url no padding
              Base64.WriteBase64URL(
                writer,
                byteArray,
                0,
                byteArray.Length,
                false);
            }
            writer.WriteCodePoint('\"');
            break;
          }
        case CBORType.TextString:
          {
            string thisString = obj.AsString();
            if (thisString.Length == 0) {
              writer.WriteString("\"\"");
              return;
            }
            writer.WriteCodePoint('\"');
            WriteJSONStringUnquoted(thisString, writer, options);
            writer.WriteCodePoint('\"');
            break;
          }
        case CBORType.Array:
          {
            writer.WriteCodePoint('[');
            for (int i = 0; i < obj.Count; ++i) {
              if (i > 0) {
                writer.WriteCodePoint(',');
              }
              bool pop = CheckCircularRef(stack, obj, obj[i]);
              WriteJSONToInternal(obj[i], writer, options, stack);
              PopRefIfNeeded(stack, pop);
            }
            writer.WriteCodePoint(']');
            break;
          }
        case CBORType.Map:
          {
            var first = true;
            var hasNonStringKeys = false;
            ICollection<KeyValuePair<CBORObject, CBORObject>> entries =
              obj.Entries;
            foreach (KeyValuePair<CBORObject, CBORObject> entry in entries) {
              CBORObject key = entry.Key;
              if (key.Type != CBORType.TextString || key.IsTagged) {
                // treat a non-text-string item or a tagged item
                // as having non-string keys
                hasNonStringKeys = true;
                break;
              }
            }
            if (!hasNonStringKeys) {
              writer.WriteCodePoint('{');
              foreach (KeyValuePair<CBORObject, CBORObject> entry in entries) {
                CBORObject key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteCodePoint(',');
                }
                writer.WriteCodePoint('\"');
                WriteJSONStringUnquoted(key.AsString(), writer, options);
                writer.WriteCodePoint('\"');
                writer.WriteCodePoint(':');
                bool pop = CheckCircularRef(stack, obj, value);
                WriteJSONToInternal(value, writer, options, stack);
                PopRefIfNeeded(stack, pop);
                first = false;
              }
              writer.WriteCodePoint('}');
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
                  case CBORType.Map:
                    {
                      var sb = new StringBuilder();
                      var sw = new StringOutput(sb);
                      bool pop = CheckCircularRef(stack, obj, key);
                      WriteJSONToInternal(key, sw, options, stack);
                      PopRefIfNeeded(stack, pop);
                      str = sb.ToString();
                      break;
                    }
                  default: str = key.ToJSONString(options);
                    break;
                }
                if (stringMap.ContainsKey(str)) {
                  throw new CBORException(
                    "Duplicate JSON string equivalents of map" +
                    "\u0020keys");
                }
                stringMap[str] = value;
              }
              first = true;
              writer.WriteCodePoint('{');
              foreach (KeyValuePair<string, CBORObject> entry in stringMap) {
                string key = entry.Key;
                CBORObject value = entry.Value;
                if (!first) {
                  writer.WriteCodePoint(',');
                }
                writer.WriteCodePoint('\"');
                WriteJSONStringUnquoted(key, writer, options);
                writer.WriteCodePoint('\"');
                writer.WriteCodePoint(':');
                bool pop = CheckCircularRef(stack, obj, value);
                WriteJSONToInternal(value, writer, options, stack);
                PopRefIfNeeded(stack, pop);
                first = false;
              }
              writer.WriteCodePoint('}');
            }
            break;
          }
        default:
          throw new InvalidOperationException("Unexpected item" +
            "\u0020type");
      }
    }
  }
}
